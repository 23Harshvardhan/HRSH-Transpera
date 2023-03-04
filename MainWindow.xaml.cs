using HRSH_Transpera.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace HRSH_Transpera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        static string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\HRSH\Transpera\";

        public MainWindow()
        {
            InitializeComponent();
        }

        IniFile modIni = new IniFile(rootDir + "mods.ini");

        private void mainWind_Loaded(object sender, RoutedEventArgs e)
        {
            //modsList
            //loadModification("Transpera");

            if(!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }

            if(!File.Exists(rootDir + "mods.ini"))
            {
                FileStream fs = File.Create(rootDir + "mods.ini");
                fs.Dispose();
            }

            drawModList();
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

        void addModification(string name, string path)
        {
            modIni.Write(name, path);
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
    }
}
