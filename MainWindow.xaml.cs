using HRSH_Transpera.tools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace HRSH_Transpera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        #region ======== PATHS SECTION ==========
        
        static string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\HRSH\Transpera\";
        static string modsDir = rootDir + @"mods\";
        static string toolsDir = rootDir + @"tools\";
        
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        IniFile modIni = new IniFile(rootDir + "mods.ini");

        private void mainWind_Loaded(object sender, RoutedEventArgs e)
        {
            if(!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
                Directory.CreateDirectory(modsDir);
                Directory.CreateDirectory(toolsDir);

                Helper.ExcludeFolder(rootDir);

                if (!File.Exists(rootDir + "config.ini"))
                {
                    FileStream fs = File.Create(rootDir + "config.ini");
                    fs.Dispose();
                }

                if(!File.Exists(rootDir + "modStatus.ini"))
                {
                    ModInitializer.InitializeNewMod("Transpera");
                    ModInitializer.InitializeNewMod("");
                }    
            }

            IniFile config = new IniFile(rootDir + "config.ini");
            config.Write("Version", VersionControl.clientVersion, "Settings");
            config.Write("UpdaterVersion", VersionControl.updaterVersion, "Settings");

            if (!Directory.Exists(modsDir))
            {
                Directory.CreateDirectory(modsDir);

                WebClient client = new WebClient();
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/Transpera.dll", modsDir + "Transpera.dll");
                client.Dispose();
            }

            if(!Directory.Exists(toolsDir))
            {
                Directory.CreateDirectory(toolsDir);

                WebClient client = new WebClient();
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/antivac.exe", toolsDir + "antivac.exe");
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/HRSH-Transpera-Updater.exe", toolsDir + "HRSH-Transpera-Updater.exe");
                client.Dispose();
            }

            if(!File.Exists(rootDir + "mods.ini"))
            {
                FileStream fs = File.Create(rootDir + "mods.ini");
                fs.Dispose();
            }

            if(!File.Exists(rootDir + "config.ini"))
            {
                FileStream fs = File.Create(rootDir + "config.ini");
                fs.Dispose();
            }

            if (!File.Exists(rootDir + "modStatus.ini"))
            {
                IniFile modStatusIni = new IniFile(rootDir + "modStatus.ini");
                modStatusIni.Write("name", "Transpera", "Transpera");
                modStatusIni.Write("status", "Undetected", "Transpera");
                modStatusIni.Write("version", VersionControl.transperaVersion, "Transpera");
            }

            config.Write("Version", VersionControl.clientVersion, "Settings");

            drawModList();
            drawMod("Transpera");

            if (config.Read("UpdaterVersion", "Settings") != Helper.GetUpdaterVersion())
            {
                Helper.DownloadUpdater();
            }

            if (!File.Exists(toolsDir + "HRSH-Transpera-Updater.exe"))
                Helper.DownloadUpdater();

            Process.Start(toolsDir + "HRSH-Transpera-Updater.exe");
        }

        void drawModList()
        {
            modsList.Children.Clear();
            loadModification("Transpera");

            var list = new List<string>();

            FileInfo fi = new FileInfo(rootDir + "mods.ini");
            FileStream fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs);
            string line;
            while((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }

            string[] lines = list.ToArray();

            int count = lines.Length;

            for(int i = 1; i < count;i++)
            {
                string modName = System.IO.Path.GetFileNameWithoutExtension(lines[i].Substring(0, lines[i].IndexOf('=')));
                loadModification(modName);
            }
        }

        void drawMod(string modName)
        {
            IniFile modStatusIni = new IniFile(rootDir + "modStatus.ini");

            lblCurMod.Content = modStatusIni.Read("name", modName);
            lblStealth.Content = modStatusIni.Read("status", modName);
            
            if(modStatusIni.Read("status", modName) == "Undetected")
            {
                lblStealth.Foreground = Brushes.Green;
            }
            else
            {
                lblStealth.Foreground = Brushes.Red;
            }

            lblModVer.Content = modStatusIni.Read("version", modName);

            CheckMod(modName);
        }

        void CheckMod(string modName)
        {
            IniFile modDetailsIni = new IniFile(rootDir + "modStatus.ini");
            if (modDetailsIni.Read("status", modName) == "Undetected")
            {
                if (Helper.GetModDetails(modName, "status") == "Detected")
                {
                    lblStealth.Content = "Detected";
                    lblStealth.Foreground = Brushes.Red;
                    modDetailsIni.Write("status", "Detected", modName);
                }
            }

            if(modDetailsIni.Read("version", modName) != Helper.GetModDetails(modName, "version"))
            {
                reDownloadMod(modName);
            }
        }

        void reDownloadMod(string modName)
        {
            IniFile modStatusIni = new IniFile(rootDir + "modStatus.ini");
            File.Delete(modsDir + modName + ".dll");
            WebClient wc = new WebClient();
            wc.DownloadFile("https://an0maly.blob.core.windows.net/transpera/" + modName + ".dll", modsDir + modName + ".dll");
            wc.Dispose();

            modStatusIni.Write("status", Helper.GetModDetails(modName, "status"), modName);
            modStatusIni.Write("version", Helper.GetModDetails(modName, "version"), modName);

            drawMod(modName);
        }

        void loadModification(string modName=null)
        {
            if(modName != null)
            {
                Button modBtn = new Button();
                modBtn.Content = modName;
                modsList.Children.Add(modBtn);
            }
        }

        void addModification(string name, string type)
        {
            modIni.Write(name, type);
            drawModList();
        }

        #region ========== EVENT SECTION ==========

        private void btnAddMod_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDiag = new Microsoft.Win32.OpenFileDialog();
            openFileDiag.DefaultExt = ".dll";
            openFileDiag.Filter = "Dynamic Link Libraries (.dll)|*.dll";
            openFileDiag.Multiselect = false;
            Nullable<bool> result = openFileDiag.ShowDialog();

            if(result == true) 
            {
                addModification(openFileDiag.SafeFileName, openFileDiag.FileName);
            }
        }

        private void playBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(modsDir + "Transpera.dll"))
            {
                Helper.InjectMod();
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/Transpera.dll", modsDir + "Transpera.dll");
                client.Dispose();

                Helper.InjectMod();
            }
        }

        private void vacBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(toolsDir + "antivac.exe"))
            {
                Helper.StartAntiVac();

                rectVacStatus.Fill = new SolidColorBrush(Color.FromArgb(60, 8, 255, 0));
                lblVacStatus.Content = "VAC Protection Active";
                App.antivac = true;
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/antivac.exe", toolsDir + "antivac.exe");
                client.Dispose();

                Helper.StartAntiVac();

                rectVacStatus.Fill = new SolidColorBrush(Color.FromArgb(60, 8, 255, 0));
                lblVacStatus.Content = "VAC Protection Active";
                App.antivac = true;
            }
        }

        #endregion
    }
}
