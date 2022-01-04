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
        private static object _version;
        private static bool _update;
        private static string _update_url;
        private static string _extention;

        public static string jsonValue { get; set; }

        static void Main(string[] args)
        {

            if (File.Exists("Config.json")){
                var websitePosts = new RemoteSettings();
                StreamReader reader = new StreamReader("Config.json");
                jsonValue = reader.ReadToEnd();
                reader.Close();

                websitePosts = JsonConvert.DeserializeObject<RemoteSettings>(jsonValue);
                _version = websitePosts.version;
                _update = websitePosts.update;
                _update_url = websitePosts.update_url;
                _extention = websitePosts.replace_mask_exten;

    }
            else
            {
                Console.WriteLine("Config.json file not found!");
                Environment.Exit(0);
            }
         
            if (_update)
            {
                var pg = new Program();
                pg.updateAndUnpack(_update_url, _version,_extention);
                string json = File.ReadAllText("Config.json");
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["update"] = false;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("Config.json", output);

            }



        }

        private void updateAndUnpack(string update_url, object version, string extention)
        {
            string url = update_url + version + "/ModSkinLOLUpdater" + extention;
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
