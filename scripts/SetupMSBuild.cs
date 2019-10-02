using System;

public class SetupMSBuild
{
    public static void Download(string directory)
    {
        try
        {
            var wc = new System.Net.WebClient();
            var content = wc.DownloadString("https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=BuildTools&rel=16");
            var url = System.Text.RegularExpressions.Regex.Match(content, 
                "https://download.visualstudio.microsoft.com/download/.*\\.exe").Value;
            Console.WriteLine(url);
            wc.DownloadFile(url, System.IO.Path.Combine(directory, "vs_BuildTools.exe"));
            wc.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", System.IO.Path.Combine(directory, "nuget.exe"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}