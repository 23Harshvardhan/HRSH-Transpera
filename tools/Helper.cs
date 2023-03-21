using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Windows;
using System.Windows.Media;
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
            //MessageBox.Show("DownloadUpdater function called");

            WebClient client = new WebClient();
            client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/HRSH-Transpera-Updater.exe", toolsDir + "HRSH-Transpera-Updater.exe");
            client.Dispose();
            IniFile config = new IniFile(rootDir + "config.ini");

            if (config.Read("UpdaterVersion", "Settings") != Helper.GetUpdaterVersion())
            {
                //MessageBox.Show("Version mismatch in DownloadUpdater so getting latest updater version that is: " + GetUpdaterVersion());
                config.Write("UpdaterVersion", GetUpdaterVersion(), "Settings");
            }
        }

        public static string GetUpdaterVersion()
        {
            //MessageBox.Show("GetUpdaterVersion function called");
            //if(File.Exists(toolsDir + "HRSH-Transpera-Updater.exe"))
            //{
            //    MessageBox.Show("Updater already exists in GetUpdaterVersion ")
            //    File.Delete(toolsDir + "HRSH-Transpera-Updater.exe");
            //}

            WebClient wc = new WebClient();
            string version = wc.DownloadString("https://an0maly.blob.core.windows.net/transpera/UpdaterVersion.txt");
            wc.Dispose();
            return version;
        }

        public static void StartAntiVac()
        {
            foreach (var process in Process.GetProcessesByName("csgo"))
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("Steam"))
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("steamwebhelper"))
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("SteamService"))
            {
                process.Kill();
            }

            Process proc = new Process();
            proc.StartInfo.FileName = toolsDir + "antivac.exe";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }

        public static void InjectMod()
        {
            if (App.antivac == true)
            {
                MessageBoxResult result2 = MessageBox.Show("Launch the game and press OK once reached main menu.", "Waiting for CSGO", MessageBoxButton.OK);

                if (result2 == MessageBoxResult.OK)
                {
                    Injection.Run(modsDir + "Transpera.dll");
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("AntiVAC is not enabled. Continue to launch?", "AntiVAC warning!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBoxResult result2 = MessageBox.Show("Launch the game and press OK once reached main menu.", "Waiting for CSGO", MessageBoxButton.OK);

                    if (result2 == MessageBoxResult.OK)
                    {
                        Injection.Run(modsDir + "Transpera.dll");
                    }
                }
            }
        }

        public static string GetModDetails(string modName, string type)
        {
            WebClient wc = new WebClient();
            string status = "";

            try
            {
                status = wc.DownloadString("https://an0maly.blob.core.windows.net/transpera/Transpera.txt");
            }
            catch
            {
                MessageBox.Show("Unable to get modification details. Please check you connection and try again.", "Error While Getting Modification Details", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            wc.Dispose();

            if (type == "status")
            {
                return status.Substring(0, status.IndexOf(';'));
            }
            else if (type == "version")
            {
                return status.Substring(status.IndexOf(';') + 1);
            }
            else
            {
                MessageBox.Show("Unknown type requested in mod detail.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "error";
            }
        }

        public static void VerifyVersion()
        {
            IniFile config = new IniFile(rootDir + "config.ini");

            string clientVersion = config.Read("Version", "Settings");
            string updaterVersion = config.Read("UpdaterVersion", "Settings");

            if(clientVersion != VersionControl.clientVersion)
            {
                config.Write("Version", VersionControl.clientVersion, "Settings");
            }

            if(updaterVersion != VersionControl.updaterVersion)
            {
                config.Write("UpdaterVersion", VersionControl.updaterVersion, "Settings");
            }
        }
    }
}
