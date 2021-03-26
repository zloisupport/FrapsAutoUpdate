using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace FrapsAutoUpdate
{
   
    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmpId { get; set; }
    }


    class Program
    { 



        static void Main(string[] args)
        {

            Employee emp = new Employee();
            string JSONresult = JsonConvert.SerializeObject(emp);
            string path = @"D:\json\employee.json";

            if (File.Exists(path))
            {
                File.Delete(path);
                using (var tw = new StreamWriter(path, true))
                {
                    Directory.CreateDirectory(@"D:\json");
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            else
            {
                using (var tw = new StreamWriter(path, true))
                {
                    Directory.CreateDirectory(@"D:\json");
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }

            string url = "";

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            string jsonValue = "";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                jsonValue = reader.ReadToEnd();
            }
            //if (ChkIntConnect() == true)
            //{
            //    Console.WriteLine("Connection done!");
            //} 
            //else { 
            //    Console.WriteLine("Connection error!"); 
            //}

            //  WebClient wbc = new WebClient();
            //string str =  wbc.DownloadString("https://raw.githubusercontent.com/zloisupport/BaiduTransInstaller/master/License.md?token=AMSGRZZT6BCEVNEBRL7B6ZDALW6QY");
            //  Console.WriteLine(str);
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
