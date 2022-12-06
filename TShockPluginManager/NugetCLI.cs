/*
TShock, a server mod for Terraria
Copyright (C) 2022 Janet Blackquill

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

global using static TShockPluginManager.I18n;

using System.CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace TShockPluginManager
{
	public static class NugetCLI
	{
		static public async Task<int> Main(List<string> args)
		{
			RootCommand root = new RootCommand(
				description: C.GetString("Manage plugins and their requirements")
			);
			Command cmdSync = new Command(
				name: "sync",
				description: C.GetString("Install the plugins as specified in the plugins.json")
			);
			cmdSync.SetHandler(Sync);
			root.Add(cmdSync);
			return await root.InvokeAsync(args.ToArray());
		}
		class SyncManifest
		{
			[JsonPropertyName("packages")]
			public Dictionary<string, NuGetVersion> Packages { get; set; } = new();
			public PackageIdentity[] GetPackageIdentities() =>
				Packages.Select((kvp) => new PackageIdentity(kvp.Key, kvp.Value))
						.OrderBy(kvp => kvp.Id)
						.ToArray();
		}
		public class NuGetVersionConverter : JsonConverter<NuGetVersion>
		{
			public override NuGetVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				return NuGetVersion.Parse(reader.GetString()!);
			}

			public override void Write(Utf8JsonWriter writer, NuGetVersion value, JsonSerializerOptions options)
			{
				writer.WriteStringValue(value.ToNormalizedString());
			}
		}
		static async Task Sync()
		{
			var opts = new JsonSerializerOptions
			{
				ReadCommentHandling = JsonCommentHandling.Skip,
				Converters =
				{
					new NuGetVersionConverter()
				}
			};
			SyncManifest manifest;
			try
			{
				string txt = await File.ReadAllTextAsync("packages.json");
				manifest = JsonSerializer.Deserialize<SyncManifest>(txt, opts)!;
			}
			catch (System.IO.FileNotFoundException)
			{
				CLIHelpers.WriteLine(C.GetString("You're trying to sync, but you don't have a packages.json file."));
				CLIHelpers.WriteLine(C.GetString("Without a list of plugins to install, no plugins can be installed."));
				return;
			}
			catch (System.Text.Json.JsonException e)
			{
				CLIHelpers.WriteLine(C.GetString("There was an issue reading the packages.json."));
				CLIHelpers.WriteLine($"{e.Message}");
				return;
			}
			foreach (var item in manifest.GetPackageIdentities())
			{
				CLIHelpers.WriteLine($"<green>{item.Id}<black> [{item.Version}]");
			}
			var numWanted = manifest.GetPackageIdentities().Count();
			CLIHelpers.WriteLine(C.GetPluralString("This is the plugin you requested to install.", "These are the plugins you requested to install", numWanted));
			CLIHelpers.WriteLine(C.GetString("Connect to the internet to figure out what to download?"));
			if (!CLIHelpers.YesNo())
				return;
			CLIHelpers.WriteLine(C.GetString("One moment..."));

			var nugetter = new Nugetter();
			PackageIdentity[] userRequests;
			IEnumerable<SourcePackageDependencyInfo> packagesToInstall;
			IEnumerable<SourcePackageDependencyInfo> builtinDependencies;
			IEnumerable<SourcePackageDependencyInfo> directlyRequestedPackages;
			IEnumerable<SourcePackageDependencyInfo> indirectlyRequiredPackages;
			try
			{
				userRequests = manifest.GetPackageIdentities();
				packagesToInstall = await nugetter.GetPackagesToInstallFor(manifest.GetPackageIdentities());
				builtinDependencies = await nugetter.GetAllBuiltinDependencies();
				directlyRequestedPackages = packagesToInstall.Where(x => userRequests.Any(y => x.Id == y.Id));
				indirectlyRequiredPackages = packagesToInstall.Where(x => !userRequests.Any(y => x.Id == y.Id));
			}
			catch (NuGet.Resolver.NuGetResolverInputException e)
			{
				CLIHelpers.WriteLine(C.GetString("There was an issue figuring out what to download."));
				CLIHelpers.WriteLine($"{e.Message}");
				return;
			}
			catch (NuGet.Resolver.NuGetResolverConstraintException e)
			{
				CLIHelpers.WriteLine(C.GetString("The versions of plugins you requested aren't compatible with eachother."));
				CLIHelpers.WriteLine(C.GetString("Read the message below to find out more."));
				CLIHelpers.WriteLine($"{e.Message}");
				return;
			}

			CLIHelpers.WriteLine(C.GetPluralString("=== Requested Plugin ===", "=== Requested Plugins ===", directlyRequestedPackages.Count()));
			foreach (var item in directlyRequestedPackages)
				DumpOne(item, builtinDependencies);
			CLIHelpers.WriteLine(C.GetPluralString("=== Dependency ===", "=== Dependencies ===", indirectlyRequiredPackages.Count()));
			foreach (var item in indirectlyRequiredPackages)
				DumpOne(item, builtinDependencies);

			CLIHelpers.WriteLine(C.GetString("Download and install the given packages?"));
			CLIHelpers.WriteLine(C.GetString("Make sure that you trust the plugins you're installing."));
			CLIHelpers.WriteLine(C.GetString("If you want to know which plugins need which dependencies, press E."));

			bool ok = false;
			do
			{
				switch (CLIHelpers.YesNoExplain())
				{
					case CLIHelpers.Answers.Yes:
						ok = true;
						break;

					case CLIHelpers.Answers.No:
						return;

					case CLIHelpers.Answers.Explain:
						foreach (var pkg in directlyRequestedPackages)
						{
							DumpGraph(pkg, packagesToInstall, builtinDependencies, 0);
						}
						CLIHelpers.WriteLine(C.GetString("Download and install the given packages?"));
						CLIHelpers.WriteLine(C.GetString("If you'd like to see which plugins need which dependencies again, press E."));
						break;
				}
			} while (!ok);

			await nugetter.DownloadAndInstall(userRequests);

			CLIHelpers.WriteLine(C.GetString("All done! :)"));
		}
		static public void DumpOne(SourcePackageDependencyInfo pkg, IEnumerable<SourcePackageDependencyInfo> builtins)
		{
			if (builtins.Any(x => x.Id == pkg.Id))
				return;

			var initial = Console.ForegroundColor;

			CLIHelpers.WriteLine(C.GetString($"<green>{pkg.Id}<black> from <blue>{pkg.Source.PackageSource.Name} <black>[{pkg.Source.PackageSource.Source}]"));

			Console.ForegroundColor = initial;
		}
		static public void DumpGraph(SourcePackageDependencyInfo from, IEnumerable<SourcePackageDependencyInfo> data, IEnumerable<SourcePackageDependencyInfo> builtins, int level)
		{
			var indent = new String('\t', level);
			Console.Write(indent);

			CLIHelpers.WriteLine(C.GetString($"<green>{from.Id} <black>from <blue>{from.Source.PackageSource.Name} <black>[{from.Source.PackageSource.Source}]"));

			foreach (var dep in from.Dependencies)
			{
				if (!builtins.Any(x => x.Id == dep.Id))
				{
					DumpGraph(data.Single(x => x.Id == dep.Id), data, builtins, level + 1);
				}
			}
		}
	}
}
