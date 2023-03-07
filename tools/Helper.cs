using System;
using System.IO;
using System.Management.Automation;
using System.Windows;
using Microsoft.Win32;

namespace HRSH_Transpera.tools
{
    public class Helper
    {
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
    }
}
