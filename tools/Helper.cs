using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Windows;
using Microsoft.Win32;

namespace HRSH_Transpera.tools
{
    public class Helper
    {
        static string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\HRSH\Transpera\";
        static string modsDir = rootDir + @"mods\";
        static string toolsDir = rootDir + @"tools\";

        public static void ExcludeFolder(string folderPath)
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Add-MpPreference").AddParameter("ExclusionPath", folderPath).Invoke();
            ps.Dispose();
        }

        public static string FindCSGOPath()
        {
            string steamInstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
            string csgoPath = System.IO.Path.Combine(steamInstallPath, @"steamapps\common\Counter-Strike Global Offensive\csgo.exe");

            if(!File.Exists(csgoPath))
            {
                MessageBox.Show("CSGO is not installed on this machine.", "CSGO Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            else
            {
                return csgoPath;
            }
        }

        public static void DownloadUpdater()
        {
            WebClient client = new WebClient();
            client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/HRSH-Transpera-Updater.exe", toolsDir + "HRSH-Transpera-Updater.exe");
            client.Dispose();
        }

        public static string GetUpdaterVersion()
        { 
            if(File.Exists(toolsDir + "HRSH-Transpera-Updater.exe"))
            {
                File.Delete(toolsDir + "HRSH-Transpera-Updater.exe");
            }

            WebClient wc = new WebClient();
            string version = wc.DownloadString("https://an0maly.blob.core.windows.net/transpera/UpdaterVersion.txt");
            wc.Dispose();
            return version;
        }
    }
}
