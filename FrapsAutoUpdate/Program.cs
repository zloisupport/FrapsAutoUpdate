using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;
using Timer = System.Timers.Timer;

namespace FrapsAutoUpdate
{
   
    class Program
    { 
        static void Main(string[] args)
        {
            if (ChkIntConnect() == true)
            {
                Console.WriteLine("Connection done!");
            }
            else { 
                Console.WriteLine("Connection error!"); 
            }

            WebClient wbc = new WebClient();
          string str =  wbc.DownloadString("https://raw.githubusercontent.com/zloisupport/BaiduTransInstaller/master/License.md?token=AMSGRZZT6BCEVNEBRL7B6ZDALW6QY");
            Console.WriteLine(str);
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
