using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModSkinLOLUpdater
{
   internal class UserSetting
    {

        public string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\Fraps";
        private string appPath =  Directory.GetDirectoryRoot(Environment.SystemDirectory)+"Fraps\\data\\My\\";
        private string settingFile = "Config.ini";

        public UserSetting()
        {
            CheckingDir();
        }




        public void CheckingDir()
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            if (!Directory.Exists(appPath))
                Directory.CreateDirectory(appPath);
        }

        public  void SaveSetting()
        {
            if (File.Exists(appPath + settingFile))
            {
                Console.WriteLine($"Save setting: {settingFile}");
                Console.ResetColor();
                File.Copy(appPath + settingFile, savePath + "\\" + settingFile,true);
            }
        }   

        public void RestoreSetting()
        {
            if (File.Exists(savePath + "\\" + settingFile))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Resore setting: {settingFile}");
                Console.ResetColor();
                File.Copy(savePath + "\\" + settingFile, appPath + settingFile, true);
            }
        }

    }
}
