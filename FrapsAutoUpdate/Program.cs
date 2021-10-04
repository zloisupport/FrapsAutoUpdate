using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

namespace FrapsAutoUpdate
{


    public class Fraps
    {
        public string version { get; set; }
        public string app_patch_url { get; set; }
        public string site_patch_url { get; set; }
        public string replace_mask_http { get; set; }
        public string replace_mask_exten { get; set; }
    }

    public class Settings
    {
        public string app_exe { get; set; }
        public string app_version { get; set; }
        public string app_last_dir { get; set; }
        public string app_http { get; set; }
    }


    class Program
    {

        public string app_old_ver { get; set; }
        static void Main(string[] args)
        {
            string url = "https://raw.githubusercontent.com/zloisupport/testsss/main/json.json";

            // HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            string jsonValue = "";


            Console.WriteLine("League of Legends Mods Skin Auto Updater");
            Console.WriteLine("Author: zloisupport");
            Console.WriteLine("Version: 0.1.0");
            //Check connections 
            if (ChkIntConnect() == true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("There is a connection");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No connection");
                Console.ResetColor();
            }


            WebClient wb = new WebClient();
            wb.DownloadFile(url, "json.json");
            StreamReader reader = new StreamReader("json.json");
            jsonValue = reader.ReadToEnd();
            reader.Close();
            Fraps websitePosts = JsonConvert.DeserializeObject<Fraps>(jsonValue);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("URL: {0}", websitePosts.app_patch_url);






            Console.ForegroundColor = ConsoleColor.Blue;
            Settings settings = new Settings();
            settings.app_last_dir = Directory.GetDirectoryRoot(Environment.SystemDirectory+"\\Fraps");


            Program program = new Program();

            if (File.Exists(settings.app_last_dir+"\\Fraps\\setting.json"))
            {
                StreamReader readers = new StreamReader(settings.app_last_dir + "\\Fraps\\setting.json");
                string jsonValueaa = readers.ReadToEnd();
                Settings websitePost = JsonConvert.DeserializeObject<Settings>(jsonValueaa);
                program.app_old_ver = websitePost.app_version;
                readers.Close();
            }
            Console.WriteLine("Installed: {0}", program.app_old_ver);

            HtmlWeb client = new HtmlWeb();

            HtmlAgilityPack.HtmlDocument doc = client.Load(websitePosts.app_patch_url);
            HtmlNodeCollection Nodes = doc.DocumentNode.SelectNodes("//a[@id]");
            foreach (var link in Nodes)
            {
                string[] CurVerRecord = new string[] { link.Attributes["href"].Value };

                string CurVerRepHttp = CurVerRecord[0].Replace(websitePosts.replace_mask_http, "");
                string CurVerRepZip = CurVerRepHttp.Replace(websitePosts.replace_mask_exten, "");


                settings.app_http = CurVerRecord[0];
                settings.app_version = CurVerRepZip.Substring(35);
                settings.app_exe = "LOLPRO.exe";

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Actual version :" + CurVerRepZip.Substring(35));
            }
            Console.WriteLine("Update to enter: Y");
            string readline = Console.ReadLine().ToLower();
            bool _true = true;
            bool _false = false;
            bool z = readline == "y" ? _true : _false;
            if (z)
            {
               
            

            if (settings.app_version != program.app_old_ver)
            {

                if (File.Exists(settings.app_last_dir + "Fraps\\setting.json"))
                {
                    File.Delete(settings.app_last_dir + "Fraps\\setting.json");
                }
                Console.WriteLine("We are looking for whether the process started");
                ///Kill proccess 
                foreach (var process in Process.GetProcessesByName("LOLPRO"))
                {
                    process.Kill();

                }

                //Download file
                DownloadFile DF = new DownloadFile();
                Directory.CreateDirectory(settings.app_last_dir + "Fraps\\Temp\\");
                DF.DownloadFiles(settings.app_http, settings.app_last_dir + "Fraps\\Temp\\app.zip");
                while (!DF.DownloadCompleted)
                    Thread.Sleep(1000);
                string JSONresult = JsonConvert.SerializeObject(settings);
                string path = settings.app_last_dir + "Fraps\\setting.json";


                using (var tw = new StreamWriter(path, true))
                {
                    string jsonFormatted = JValue.Parse(JSONresult).ToString(Formatting.Indented);
                    tw.WriteLine(jsonFormatted);
                    tw.Close();
                }
                ExtractArhive ExtArhive = new ExtractArhive();
                ExtArhive.ExtractZipContent(settings.app_last_dir + "Fraps\\Temp\\app.zip", null, settings.app_last_dir + "Fraps\\Temp\\");
                ExtArhive.ExtractZipContent(settings.app_last_dir + "Fraps\\Temp\\Data.lol", null, settings.app_last_dir + "Fraps\\Temp\\Data");

                foreach (string dirPath in Directory.GetDirectories(settings.app_last_dir + "Fraps\\Temp\\Data", "*.*",
                        SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(settings.app_last_dir + "Fraps\\Temp\\Data\\Fraps\\", settings.app_last_dir + "Fraps\\"));
                foreach (string newPath in Directory.GetFiles(settings.app_last_dir + "Fraps\\Temp\\Data", "*.*",
                         SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(settings.app_last_dir + "Fraps\\Temp\\Data\\Fraps\\", settings.app_last_dir + "Fraps\\"), true);

                Directory.Delete(settings.app_last_dir + "Fraps\\Temp\\", true);

                //create Config.ini not  file not work Fraps
                string file = (settings.app_last_dir + "Fraps\\data\\My\\Config.ini");

                Directory.CreateDirectory(settings.app_last_dir + "Fraps\\data\\My\\");
                if (!System.IO.File.Exists(file))
                {
                    using (StreamWriter text = System.IO.File.AppendText(file))
                    {
                        string wrText = @"[CONFIG]
            MY_LOCATION = EN
            LANGUAGE = EN
            GAME_PATH_0 =
            GAME_PATH = GARENA
            MOD_ONLY = 0
            MOD_LOAD_FRAME = 0
                            ";
                        text.WriteLine(wrText);
                        text.Close();
                    }
                }
                }
            }

            try
            {
                ProcessStartInfo info = new ProcessStartInfo(settings.app_last_dir + "Fraps\\LOLPRO.exe");
                info.UseShellExecute = true;
                info.Verb = "runas";
                Process.Start(info);
                Console.WriteLine("The application is running");
            }
            catch
            {
                Console.WriteLine("The application is not running");
            }
        }
   




        public static bool ChkIntConnect()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ya.ru");
                request.Timeout = 5000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


    }
}
