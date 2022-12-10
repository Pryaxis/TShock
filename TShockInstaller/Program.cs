using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine($"TShock Installer {typeof(Program).Assembly.GetName().Version}.");

// reference: https://github.com/dotnet/install-scripts/blob/main/src/dotnet-install.sh
// ./dotnet-install.sh -verbose -version 6.0.11 --runtime dotnet

Console.WriteLine("Determining dotnet runtime url...");

var arch = RuntimeInformation.ProcessArchitecture switch
{
	Architecture.X64 => "x64",
	Architecture.Arm64 => "arm64",
	_ => null
};

if (arch is null)
{
	Console.WriteLine($"{RuntimeInformation.ProcessArchitecture} is not yet supported via this installer.");
	return;
}

string? url = null;
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
	url = $"https://dotnetcli.azureedge.net/dotnet/Runtime/6.0.11/dotnet-runtime-6.0.11-osx-{arch}.tar.gz";
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
	url = $"https://dotnetcli.azureedge.net/dotnet/Runtime/6.0.11/dotnet-runtime-6.0.11-win-{arch}.zip";
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
	url = $"https://dotnetcli.azureedge.net/dotnet/Runtime/6.0.11/dotnet-runtime-6.0.11-linux-{arch}.tar.gz";

if(url is null)
{
	Console.WriteLine("Unable to determine .net runtime to install. " +
		"Refer to https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script " +
		"and install using --install-dir dotnet, so that the dotnet folder is beside TShock.Server[.exe]");
	return;
}

Console.WriteLine("Using url: " + url);

var filename = url.Split('/').Last();
var is_targz = filename.EndsWith(".tar.gz");

var download_info = new FileInfo(filename);
if (!download_info.Exists) // todo hash check
{
	Console.WriteLine($"Downloading: {filename}...");

	using var client = new HttpClient();
	using var resp = await client.GetStreamAsync(url);
	using var fs = new FileStream(filename, FileMode.Create);
	await resp.CopyToAsync(fs);
}
else
{
	Console.WriteLine("Using existing download on disk: " + filename);
}

var dotnet_path = Path.Combine("dotnet", "dotnet" + (is_targz ? "" : ".exe"));
var tshock_path = "TShock.Server" + (is_targz ? "" : ".exe");

if (!File.Exists(dotnet_path))
{
	try
	{
		Console.WriteLine("Extracting to ./dotnet/");
		if (is_targz)
		{
			using var srm_dotnet_file = File.OpenRead(filename);
			using var srm_gzip = new GZipInputStream(srm_dotnet_file);

			using var tar_archive = TarArchive.CreateInputTarArchive(srm_gzip, System.Text.Encoding.UTF8);
			tar_archive.ExtractContents("dotnet");

			[DllImport("libc", SetLastError = true)]
			static extern int chmod(string pathname, int mode);

			chmod(dotnet_path, 755);
		}
		else
		{
			ZipFile.ExtractToDirectory(filename, "dotnet");
		}
	}
	catch (Exception ex)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.Error.WriteLine($"Failed to extract {filename}. The archive will be removed. Restart the installer to begin the download again.");
		Console.Error.WriteLine(ex);

		if (File.Exists(filename))
			File.Delete(filename);

		return;
	}
}
else
{
	Console.WriteLine($"Extract skipped, existing found at: {dotnet_path}");
}

var dotnet_root = System.IO.Path.GetFullPath("dotnet");
Console.WriteLine($"To be able to run {tshock_path} yourself set the environment variable DOTNET_ROOT={dotnet_root}");

Environment.SetEnvironmentVariable("DOTNET_ROOT", dotnet_root);

Console.WriteLine($"Extracted, launching: {tshock_path}");

var proc = new Process();

Console.CancelKeyPress += (sender, e) =>
{
	e.Cancel = !proc.HasExited;
};

proc.StartInfo = new()
{
	 FileName = tshock_path,
};
foreach (var arg in args)
	proc.StartInfo.ArgumentList.Add(arg);
proc.Start();
await proc.WaitForExitAsync();
