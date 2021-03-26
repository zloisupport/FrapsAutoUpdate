using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
  

    public class Employee
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
       

        static void Main(string[] args)
        {
            string url = "https://raw.githubusercontent.com/zloisupport/testsss/main/json.json";
            string app_old_ver = "";
           // HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            string jsonValue = "";

          

              Employee websitePosts = JsonConvert.DeserializeObject<Employee>(jsonValue);


            //Check connections 
            if (ChkIntConnect() == true)
            {
                Console.ForegroundColor= ConsoleColor.Green;
                Console.WriteLine("Connection done!");
                Console.ResetColor();

                WebClient wb = new WebClient();
                wb.DownloadFile(url, "json.json");

                // using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                //{
                StreamReader reader = new StreamReader("json.json");
                jsonValue = reader.ReadToEnd();
                //  }
                reader.Close();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Connection error!");
                Console.ResetColor();
                Console.WriteLine("Application Exit");
             
            }



            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(websitePosts.app_patch_url);
          // Console.WriteLine(websitePosts.site_patch_url);
            Console.ResetColor();
        
            Settings settings = new Settings();
            HtmlWeb client = new HtmlWeb();

            settings.app_last_dir = Directory.GetDirectoryRoot(Environment.SystemDirectory + @"\Fraps");
            string jsonValueaa = "";
            if(File.Exists(settings.app_last_dir + "Fraps\\setting.json")) { 
                StreamReader readers = new StreamReader(settings.app_last_dir+ "Fraps\\setting.json");
                jsonValueaa = readers.ReadToEnd();
                Settings websitePost = JsonConvert.DeserializeObject<Settings>(jsonValueaa);
                Console.WriteLine("Installed: "+ websitePost.app_version);
                app_old_ver = websitePost.app_version;
                readers.Close();
            }

           
            //Parse site
            HtmlAgilityPack.HtmlDocument doc = client.Load(websitePosts.app_patch_url);
            HtmlNodeCollection Nodes = doc.DocumentNode.SelectNodes("//a[@id]");
            foreach (var link in Nodes)
            {
                string[] CurVerRecord = new string[] { link.Attributes["href"].Value };
                //   Console.WriteLine(CurVerRecord[0]);
             
                string CurVerRepHttp = CurVerRecord[0].Replace(websitePosts.replace_mask_http, "");
                string CurVerRepZip = CurVerRepHttp.Replace(websitePosts.replace_mask_exten, "");
             
               
                settings.app_http = CurVerRecord[0];
                settings.app_version = CurVerRepZip;
          
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Actual version :" + CurVerRepZip);
                Console.WriteLine("Download address :" + settings.app_http);
                Console.WriteLine("System Directory :" + settings.app_last_dir+"Fraps");
                Console.ResetColor();
           
            }
          if(settings.app_version != app_old_ver) {

                if(File.Exists(settings.app_last_dir + "Fraps\\setting.json")){
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
            while(!DF.DownloadCompleted)
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

                Directory.Delete(settings.app_last_dir + "Fraps\\Temp\\",true);

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
      
            //Process ExternalProcess = new Process();
            //ExternalProcess.StartInfo.FileName = settings.app_last_dir + "Fraps\\LOLPRO "+settings.app_version+".exe";
            //ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            //ExternalProcess.Start();

        }



    

        public static bool ChkIntConnect()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://vk.com/groups");
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
