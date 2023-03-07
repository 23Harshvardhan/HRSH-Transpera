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
        static string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\HRSH\Transpera\";
        static string modsDir = rootDir + @"mods\";
        static string toolsDir = rootDir + @"tools\";

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
            }

            IniFile config = new IniFile(rootDir + "config.ini");
            config.Write("Version", "2.0.0", "Settings");

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

            config.Write("Version", "2.0.0", "Settings");

            drawModList();

            if (!File.Exists(toolsDir + "HRSH-Transpera-Updater.exe"))
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/HRSH-Transpera-Updater.exe", toolsDir + "HRSH-Transpera-Updater.exe");
                client.Dispose();
            }
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
                injectMod();
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/Transpera.dll", modsDir + "Transpera.dll");
                client.Dispose();

                injectMod();
            }
        }

        void injectMod()
        {
            if (App.antivac == true)
            {
                Inject();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("AntiVAC is not enabled. Continue to launch?", "AntiVAC warning!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Inject();
                }
            }
        }

        void Inject()
        {
            MessageBoxResult result2 = MessageBox.Show("Launch the game and press OK once reached main menu.", "Waiting for CSGO", MessageBoxButton.OK);

            if (result2 == MessageBoxResult.OK)
            {
                Injection.Run(modsDir + "Transpera.dll");
            }
        }

        void StartAntiVac()
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

            rectVacStatus.Fill = new SolidColorBrush(Color.FromArgb(60, 8, 255, 0));
            lblVacStatus.Content = "VAC Protection Active";
            App.antivac = true;

            //try
            //{
            //    if(antiVacProc())
            //    {
            //        rectVacStatus.Fill = new SolidColorBrush(Color.FromArgb(60, 8, 255, 0));
            //        lblVacStatus.Content = "VAC Protection Active";
            //        App.antivac = true;

            //        await Task.Run(() => {
            //            foreach (var process in Process.GetProcessesByName("Steam"))
            //            {
            //                process.WaitForExit();
            //            }

            //            App.antivac = false;
            //            rectVacStatus.Fill = new SolidColorBrush(Color.FromArgb(60, 255, 140, 0));
            //            lblVacStatus.Content = "VAC Protection Not Active";
            //        });
            //    }
            //    else
            //    {
            //        MessageBox.Show("Failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString(), "Error");
            //}
        }

        //bool antiVacProc()
        //{
        //    Process proc = new Process();
        //    proc.StartInfo.FileName = toolsDir + "antivac.exe";
        //    proc.StartInfo.UseShellExecute = true;
        //    proc.StartInfo.Verb = "runas";
        //    proc.Start();

        //    return true;
        //}

        private void vacBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(toolsDir + "antivac.exe"))
            {
                StartAntiVac();
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://an0maly.blob.core.windows.net/transpera/antivac.exe", toolsDir + "antivac.exe");
                client.Dispose();

                StartAntiVac();
            }
        }
    }
}
