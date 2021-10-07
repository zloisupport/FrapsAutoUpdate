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
            string update_url = websitePosts.update_url;
            string extention = websitePosts.replace_mask_exten;
            if (update)
            {
                var pg = new Program();
                pg.updateAndUnpack(update_url, version,extention);
                string json = File.ReadAllText("Config.json");
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["update"] = false;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("Config.json", output);

            }



        }

        private void updateAndUnpack(string update_url, float version, string extention)
        {
            string url = update_url + version + extention;
            Console.WriteLine(url);
            DownloadFile df = new DownloadFile();
            df.DownloadFiles(url, ".//update.zip");
            while (!df.DownloadCompleted)
                Thread.Sleep(1000);



            try
            {
                ExtractArhive ExtArhive = new ExtractArhive();
                ExtArhive.ExtractZipContent("update.zip", null, ".\\Temp\\");
            }
            catch
            {
                Console.WriteLine("Unpacking error!");
                File.Delete("update.zip");
            }

            foreach (string dirPath in Directory.GetDirectories(".\\Temp\\", "*.*",
        SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(".\\Temp\\", ".\\Temp\\"));
            foreach (string newPath in Directory.GetFiles(".\\Temp\\", "*.*",
                     SearchOption.AllDirectories))
                try
                {
                    File.Copy(newPath, newPath.Replace(".\\Temp\\", ""), true);
                }
                catch 
                {
                    Console.WriteLine("Remove error");
                }
         
                File.Delete(".//update.zip");
                Directory.Delete("Temp", true);
     
        }
    }


}
