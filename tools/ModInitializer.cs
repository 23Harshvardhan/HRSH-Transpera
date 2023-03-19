using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSH_Transpera.tools
{
    public class ModInitializer
    {
        #region ========== PATHS SECTION ==========

        static string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\HRSH\Transpera\";
        static string modsDir = rootDir + @"mods\";
        static string toolsDir = rootDir + @"tools\";

        #endregion

        static IniFile modStatusIni = new IniFile(rootDir + "modStatus.ini");

        public static void InitializeNewMod(string modName)
        {
            modStatusIni.Write("name", modName, modName);
            modStatusIni.Write("status", Helper.GetModDetails(modName, "status"), modName);
            modStatusIni.Write("version", Helper.GetModDetails(modName, "version"), modName);
        }
    }
}
