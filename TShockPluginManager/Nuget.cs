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

using System.Reflection;
using System.Runtime.InteropServices;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;

namespace TShockPluginManager
{

	public class Nugetter
	{
		// this object can figure out the right framework folders to use from a set of packages
		private FrameworkReducer FrameworkReducer;
		// the package framework we want to install
		private NuGetFramework NuGetFramework;
		// nuget settings
		private ISettings Settings;
		// this is responsible for bookkeeping the folders that nuget touches
		private NuGetPathContext PathContext;
		// this is responsible for managing the package sources
		private PackageSourceProvider PackageSourceProvider;
		// this is responsible for managing the repositories of packages from all of the package sources
		private SourceRepositoryProvider SourceRepositoryProvider;
		// this can tell us the paths of local packages
		private PackagePathResolver PackagePathResolver;
		// this is possible for bookkeeping the extraction state of packages
		private PackageExtractionContext PackageExtractionContext;

		public Nugetter()
		{
			FrameworkReducer = new FrameworkReducer();
			NuGetFramework = NuGetFramework.ParseFolder("net6.0");
			Settings = NuGet.Configuration.Settings.LoadDefaultSettings(root: null);
			PathContext = NuGetPathContext.Create(Settings);
			PackageSourceProvider = new PackageSourceProvider(Settings);
			SourceRepositoryProvider = new SourceRepositoryProvider(PackageSourceProvider, Repository.Provider.GetCoreV3());
			PackagePathResolver = new PackagePathResolver(Path.GetFullPath("packages"));
			PackageExtractionContext = new PackageExtractionContext(
				PackageSaveMode.Defaultv3,
				XmlDocFileSaveMode.Skip,
				ClientPolicyContext.GetClientPolicy(Settings, NullLogger.Instance),
				NullLogger.Instance);
		}

		async Task GetPackageDependencies(
			PackageIdentity package,
			NuGetFramework framework,
			SourceCacheContext cacheContext,
			ILogger logger,
			IEnumerable<SourceRepository> repositories,
			ISet<SourcePackageDependencyInfo> availablePackages)
		{
			// if we've already gotten dependencies for this package, don't
			if (availablePackages.Contains(package)) return;

			foreach (var sourceRepository in repositories)
			{
				// make sure the source repository can actually tell us about dependencies
				var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
				// get the try and dependencies
				// (the above function returns a nullable value, but doesn't properly indicate it as such)
				#pragma warning disable CS8602
				var dependencyInfo = await dependencyInfoResource?.ResolvePackage(
					package, framework, cacheContext, logger, CancellationToken.None);
				#pragma warning restore CS8602

				// oop, we don't have the ability to get dependency info from this repository, or
				// it wasn't found. let's try the next source repository!
				if (dependencyInfo == null) continue;

				availablePackages.Add(dependencyInfo);
				foreach (var dependency in dependencyInfo.Dependencies)
				{
					// make sure we get the dependencies of the dependencies of the dependencies ... as well
					await GetPackageDependencies(
						new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
						framework, cacheContext, logger, repositories, availablePackages);
				}
			}
		}

		/// <returns>all the packages representing dependencies bundled with TShock.Server</returns>
		public async Task<IEnumerable<SourcePackageDependencyInfo>> GetAllBuiltinDependencies()
		{
			// this is a convenient approximation of what dependencies will be included with TShock.Server
			// and really only needs to be updated if new third-party dependencies are added

			var knownBundles = new[] {
				new PackageIdentity("GetText.NET", NuGetVersion.Parse("1.6.6")),
				new PackageIdentity("OTAPI.Upcoming", NuGetVersion.Parse("3.1.8-alpha")),
				new PackageIdentity("TSAPI", NuGetVersion.Parse("5.0.0-beta")),
				new PackageIdentity("TShock", NuGetVersion.Parse("5.0.0-beta")),
			};

			return await GetAllDependenciesFor(knownBundles);
		}

		/// <returns>all the dependencies for the provided package identities</returns>
		public async Task<IEnumerable<SourcePackageDependencyInfo>> GetAllDependenciesFor(IEnumerable<PackageIdentity> targets)
		{
			using var cacheContext = new SourceCacheContext();

			// get all of the possible packages in our dependency tree
			var possiblePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
			foreach (var target in targets)
			{
				await GetPackageDependencies(
					target,
					NuGetFramework,
					cacheContext,
					NullLogger.Instance,
					SourceRepositoryProvider.GetRepositories(),
					possiblePackages
				);
			}

			var resolverContext = new PackageResolverContext(
				// select minimum possible versions
				DependencyBehavior.Lowest,
				// these are the packages the user wanted
				targets.Select(x => x.Id),
				// we don't hard-require anything
				Enumerable.Empty<string>(),
				// we don't have a lockfile
				Enumerable.Empty<PackageReference>(),
				// we don't have fancy versioning
				Enumerable.Empty<PackageIdentity>(),
				// these are the packages that we figured out are in the dependency tree from nuget
				possiblePackages,
				// all the package sources
				SourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
				NullLogger.Instance
			);

			var resolver = new PackageResolver();
			var packagesToInstall =
				// get the resolved versioning info from the resolver
				resolver.Resolve(resolverContext, CancellationToken.None)
					// and use that to select the specific packages to install from the possible packages
					.Select(p => possiblePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));

			return packagesToInstall;
		}

		/// <returns>whether or not subPath is a subpath of basePath</returns>
		public static bool IsSubPathOf(string subPath, string basePath)
		{
			var rel = Path.GetRelativePath(basePath, subPath);
			return rel != "."
				&& rel != ".."
				&& !rel.StartsWith("../")
				&& !rel.StartsWith(@"..\")
				&& !Path.IsPathRooted(rel);
		}

		/// <returns>items required for end-user running of a package</returns>
		public IEnumerable<FrameworkSpecificGroup> ItemsToInstall(PackageReaderBase packageReader)
		{
			var libItems = packageReader.GetLibItems();
			var libnearest = FrameworkReducer.GetNearest(NuGetFramework, libItems.Select(x => x.TargetFramework));
			libItems = libItems.Where(x => x.TargetFramework.Equals(libnearest));

			var frameworkItems = packageReader.GetFrameworkItems();
			var fwnearest = FrameworkReducer.GetNearest(NuGetFramework, frameworkItems.Select(x => x.TargetFramework));
			frameworkItems = frameworkItems.Where(x => x.TargetFramework.Equals(fwnearest));

			return libItems.Concat(frameworkItems);
		}

		/// <returns>path to package folder and metadata reader</returns>
		public async Task<(string, PackageReaderBase)> GetOrDownloadPackage(SourcePackageDependencyInfo pkg)
		{
			using var cacheContext = new SourceCacheContext();

			PackageReaderBase packageReader;
			string pkgPath;
			// already installed?
			if (PackagePathResolver.GetInstalledPath(pkg) is string path)
			{
				// we're gaming
				packageReader = new PackageFolderReader(path);
				pkgPath = path;
			}
			else
			{
				// gotta download it...
				var downloadResource = await pkg.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
				Console.WriteLine($"Downloading {pkg.Id}...");
				var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
					pkg,
					new PackageDownloadContext(cacheContext),
					SettingsUtility.GetGlobalPackagesFolder(Settings),
					NullLogger.Instance, CancellationToken.None);
				packageReader = downloadResult.PackageReader;
				Console.WriteLine($"Extracting {pkg.Id}...");
				// and extract the package
				await PackageExtractor.ExtractPackageAsync(
					downloadResult.PackageSource,
					downloadResult.PackageStream,
					PackagePathResolver,
					PackageExtractionContext,
					CancellationToken.None);

				if (PackagePathResolver.GetInstalledPath(pkg) is string loc)
				{
					pkgPath = loc;
				}
				else
				{
					pkgPath = null;
					// die somehow
				}
			}
			return (pkgPath, packageReader);
		}

		/// <returns>resolved packages to be installed for what the user requested</returns>
		public async Task<IEnumerable<SourcePackageDependencyInfo>> GetPackagesToInstallFor(PackageIdentity[] userRequest)
		{
			using var cacheContext = new SourceCacheContext();
			return (await GetAllDependenciesFor(userRequest)).OrderBy(v => v.Id);
		}

		/// <summary>installs a locally downloaded package</summary>
		public void InstallPackage(SourcePackageDependencyInfo pkg, string pkgPath, PackageReaderBase packageReader)
		{
			// objects to help us detect if packages already come with the .NET distribution
			string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
			var paResolver = new PathAssemblyResolver(runtimeAssemblies);
			using var mlc = new MetadataLoadContext(paResolver);

			// packages can declare themselves as plugin via the TShockPlugin package type
			var isPlugin = packageReader.NuspecReader.GetPackageTypes().Any(v => v.Name == "TShockPlugin");

			Console.WriteLine($"Installing {pkg.Id}...");

			foreach (var item in ItemsToInstall(packageReader))
			{
				var files = item.Items;
				if (item.Items.Count() == 0)
					continue;

				// the common ancestor directory of all files in the package.
				// if a package has the following files:
				// - /home/orwell/packages/FooBar/hi.dll
				// - /home/orwell/packages/FooBar/en-US/hi.resources.dll
				// - /home/orwell/packages/FooBar/de-DE/hi.resources.dll
				// - /home/orwell/packages/FooBar/ar-AR/hi.resources.dll
				// this will be /home/orwell/packages/FooBar
				var rootmostPath = files
					.Select(x => Path.Join(pkgPath, x))
					.Aggregate(Path.GetDirectoryName(Path.Join(pkgPath, files.First())), (acc, x) =>
						IsSubPathOf(acc!, Path.GetDirectoryName(x)!) ?
							Path.GetDirectoryName(x) :
							acc);

				foreach (var file in files)
				{
					// the absolute path of the package on the filesystem
					var filePath = Path.Join(pkgPath, file);
					// the path of the package relative to the package root
					var packageRelativeFilePath = filePath.Substring(rootmostPath!.Length);
					bool alreadyExists;
					// if it's a .dll, we try to detect if we already have the assemblies
					// (e.g. because we bundle them in TShock.Server or the .NET runtime comes)
					// with them
					if (file.EndsWith(".dll"))
					{
						var asms = AppDomain.CurrentDomain.GetAssemblies();
						var asm = mlc.LoadFromAssemblyPath(filePath);
						alreadyExists = asms.Any(a => a.GetName().Name == asm.GetName().Name);
					}
					else alreadyExists = false;

					// if it already exists, skip. but only if it's not an explicitly requested plugin.
					if (alreadyExists && !isPlugin)
						continue;

					var relativeFolder = Path.GetDirectoryName(packageRelativeFilePath);
					var targetFolder = Path.Join(isPlugin ? "./ServerPlugins" : "./bin", relativeFolder);
					Directory.CreateDirectory(targetFolder);
					File.Copy(filePath, Path.Join(targetFolder, Path.GetFileName(filePath)), true);
				}
			}
		}

		/// <summary>downloads and installs the given packages</summary>
		public async Task DownloadAndInstall(PackageIdentity[] userRequest)
		{
			var packagesToInstall = await GetAllDependenciesFor(userRequest);
			var builtins = await GetAllBuiltinDependencies();

			foreach (var pkg in packagesToInstall)
			{
				var bundled = builtins!.Where(x => x.Id == pkg.Id).FirstOrDefault();
				if (bundled != null)
					continue;

				(string pkgPath, PackageReaderBase packageReader) = await GetOrDownloadPackage(pkg);
				InstallPackage(pkg, pkgPath, packageReader);
			}
		}
	}
}
