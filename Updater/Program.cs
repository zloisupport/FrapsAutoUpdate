using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModSkinLoLUpdater
{
    class Program 
    {
        public static string jsonValue { get; private set; }

        static void Main(string[] args)
        {
            RemoteSettings websitePosts = new RemoteSettings();
            if (File.Exists("Config.json")){
                StreamReader reader = new StreamReader("Config.json");
                jsonValue = reader.ReadToEnd();
                reader.Close();
                websitePosts = JsonConvert.DeserializeObject<RemoteSettings>(jsonValue);
            }
            else
            {
                Console.WriteLine("Config.json file not found!");
                Environment.Exit(0);
            }
            float version = websitePosts.version;
            bool update = websitePosts.update;
            if (update)
            {
                var pg = new Program();
                pg.updateAndUnpack(version);
                string json = File.ReadAllText("Config.json");
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["update"] = false;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("Config.json", output);

            }



        }

        private void updateAndUnpack(float version)
        {
            string url = "http://weather/net" + version + ".zip";
            Console.WriteLine(version);
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
                try
                {
                   // File.Copy(newPath, newPath.Replace(".\\Temp\\", ""), true);
                  //  File.Delete(".//update.zip");
                    //    File.Move("_Config.json", "Config.json",true);
                    //Directory.Delete("Temp", true);
                }
                catch
                {

                }
        }
    }


}
