using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

namespace ModSkinLoLUpdater
{
    public class RemoteSettings
    {
        public object version { get; set; }
        public bool update { get; set; }
        public string app_patch_url { get; set; }
        public string site_patch_url { get; set; }
        public string replace_mask_http { get; set; }
        public string replace_mask_exten { get; set; }
        public string update_url { get; set; }
    }

    public class LocalSettings
    {
        public string app_exe { get; set; }
        public string app_version { get; set; }
        public string app_last_dir { get; set; }
        public string app_http { get; set; }
    }


  public class Program
    {

        private string app_old_ver { get; set; }
        public string app_path { set; get; }
        public object settings { get;set; }
        public object updater_old_version { get; set; }
        public object updater_new_version { get; set; }

        private static string json_value = null;
        private static string url_config = "https://raw.githubusercontent.com/zloisupport/ModSkinLolUpdater/master/FrapsAutoUpdate/Config.json";
        private static string current_directory = Directory.GetCurrentDirectory();
        private static string root_directory = Directory.GetDirectoryRoot(Environment.SystemDirectory+"\\Fraps");

        static void Main(string[] args)
        {


            var run_time_ver = RuntimeInformation.FrameworkDescription;
            var app_ver_info = FileVersionInfo.GetVersionInfo(current_directory + "//ModSkinLOLUpdater.exe");

            LocalSettings settings = new LocalSettings();
            settings.app_last_dir = root_directory;

            Console.WriteLine(@"League of Legends Mods Skin Auto Updater");
            Console.WriteLine("Author: zloisupport");
            Console.WriteLine(run_time_ver);
            Console.WriteLine("Version: " + app_ver_info.FileVersion);

            //Check connections 
            if (ChkIntConnect())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("There is a connection");
                Console.ResetColor();
                Program program = new Program();
                program.downloadApp();
                string paths = Directory.GetDirectoryRoot(Environment.SystemDirectory + "\\Fraps");
                program.runningApp(paths);
              

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No connection");
                Console.ResetColor();

            }

        }


        
        public void downloadApp()
        {

            RemoteSettings websitePosts = new RemoteSettings();
            if (File.Exists("Config.json"))
            {
                RemoteSettings websitePost = new RemoteSettings();
                StreamReader reader_ = new StreamReader("Config.json");
                json_value = reader_.ReadToEnd();
                reader_.Close();
                websitePost = JsonConvert.DeserializeObject<RemoteSettings>(json_value);
                updater_old_version = websitePost.version;
                File.Delete("Config.json");
            }

            WebClient wb = new WebClient();
            wb.DownloadFile(url_config, "Config.json");
            StreamReader reader = new StreamReader("Config.json");
            json_value = reader.ReadToEnd();
                   reader.Close();
            websitePosts = JsonConvert.DeserializeObject<RemoteSettings>(json_value);

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("URL: {0}", websitePosts.app_patch_url);
            updater_new_version = websitePosts.version;
          
            
            

            LocalSettings settings = new LocalSettings();
            settings.app_last_dir = Directory.GetDirectoryRoot(Environment.SystemDirectory + "\\Fraps");





            Console.ForegroundColor = ConsoleColor.Blue;


            Program program = new Program();

            if (File.Exists(settings.app_last_dir + "\\Fraps\\setting.json"))
            {
                StreamReader readers = new StreamReader(settings.app_last_dir + "\\Fraps\\setting.json");
                string jsonValueaa = readers.ReadToEnd();
                LocalSettings websitePost = JsonConvert.DeserializeObject<LocalSettings>(jsonValueaa);
                program.app_old_ver = websitePost.app_version;
                readers.Close();
            }
            if (program.app_old_ver == null)
                Console.WriteLine("Not installed");
            else
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

            string readline = "";
            bool _true = true;
            bool _false = false;

            if (settings.app_version != program.app_old_ver)
            {
                Console.WriteLine("Update to enter: y");
                readline = Console.ReadLine().ToLower();
            }


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
                    Thread iconFixThread = new Thread(new ThreadStart(UpdateIcon));
                    iconFixThread.Start(); //start Thread
                }
               
            }
                



        }

        public static void UpdateIcon()
        {
            ModSkinLOLUpdater.UpdateIcon updateIcon = new ModSkinLOLUpdater.UpdateIcon();
            updateIcon.DownloadIcon();
        }
    
       public void runningApp(string path)
        {
         
                ProcessStartInfo info = new ProcessStartInfo(path + "Fraps\\LOLPRO.exe");
                info.UseShellExecute = true;
                info.Verb = "runas";
            try
            {
                Process.Start(info); 
           
                Console.WriteLine("The application is running");
               
            }
            catch
            {
                Console.WriteLine("The application is not running");
            }
        }

        public static bool ChkIntConnect() {
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
