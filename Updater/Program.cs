using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace ModSkinLoLUpdater
{
    class Program 
    {
        public static string jsonValue { get; private set; }

        static void Main(string[] args)
        {
            RemoteSettings websitePosts = new RemoteSettings();
            if (File.Exists("_Config.json")){
                StreamReader reader = new StreamReader("_Config.json");
                jsonValue = reader.ReadToEnd();
                reader.Close();
                websitePosts = JsonConvert.DeserializeObject<RemoteSettings>(jsonValue);
            }
            else
            {
                Console.WriteLine("_Config file not found!");
                Environment.Exit(0);
            }
            float version =  float.Parse(websitePosts.version);
            string url = "http://weather/net" + version+".zip";
            Console.WriteLine(websitePosts.version);
            DownloadFile df = new DownloadFile();
            df.DownloadFiles(url, ".//update.zip");
            while (!df.DownloadCompleted)
                Thread.Sleep(1000);




            ExtractArhive ExtArhive = new ExtractArhive();
            ExtArhive.ExtractZipContent("update.zip", null, ".\\Temp\\");
           
            foreach (string dirPath in Directory.GetDirectories(".\\Temp\\", "*.*",
                    SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(".\\Temp\\", ".\\Temp\\"));
            foreach (string newPath in Directory.GetFiles(".\\Temp\\", "*.*",
                     SearchOption.AllDirectories))
              try {
                    File.Copy(newPath, newPath.Replace(".\\Temp\\", ""), true);
                    File.Delete(".//update.zip");
                //    File.Move("_Config.json", "Config.json",true);
                    Directory.Delete("Temp", true);
                }
                catch
                {

                }

            
            //ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            ////info.UseShellExecute = true;
            ////info.Verb = "runas";
            //startInfo.Arguments="/c xcopy ./temp/*.* ./*.*";
            //try
            //{
            //    Process.Start(startInfo);

                //    Console.WriteLine("The application is running");

                //}
                //catch
                //{
                //    Console.WriteLine("The application is not running");
                //}

        }
    }
}
