using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ImageMagick;
namespace ModSkinLOLUpdater
{
    class UpdateIcon
    {
        private string HTTPURL = "https://lol-skin.weblog.vc/img/champion/";
        private string EXTENTION = ".png";
        private static string root_directory = Directory.GetDirectoryRoot(Environment.SystemDirectory);
        private string fraps_directory_120img = root_directory + "\\Fraps\\data\\120\\";
        private string fraps_directory_80img = root_directory + "\\Fraps\\data\\80\\";
        private string fraps_directory = root_directory + "\\Fraps\\";
        private string fraps_directory_data = root_directory + "\\Fraps\\data\\";
        private string fraps_directory_default = root_directory + "\\Fraps\\data\\Default";

        public void DownloadIcon()
        {

            WebClient web = new WebClient();
            web.Headers.Set("Content-Type", "image/jpeg");
            web.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 6.0; " +
                                  "Windows NT 5.2; .NET CLR 1.0.3705;)");
            if (!Directory.Exists(fraps_directory_120img)) Directory.CreateDirectory(fraps_directory_120img);
            // web.DownloadFile(HTTPURL + "Ahri" + EXTENTION, root_directory + "\\Fraps\\data\\120\\" + "ahri" + EXTENTION);
            ModSkinLoLUpdater.DownloadFile downloadFile = new ModSkinLoLUpdater.DownloadFile();
            int count = 0;
            var name = ReadChampionName();
            foreach (var n in name)
            {

                Console.Clear();
                Console.WriteLine(count + ": loaded of :" + name.Count);
                //  web.DownloadFileAsync(new Uri(HTTPURL + n+ EXTENTION), root_directory + "\\Fraps\\data\\120\\" + n + EXTENTION);
                bool file = FileSizes(n);
                Console.WriteLine(n);
                if (file) {
                    Console.WriteLine(n+"www");
                    downloadFile.DownloadFiles(HTTPURL + n + EXTENTION, root_directory + "\\Fraps\\data\\120\\" + n + EXTENTION);
                    while (!downloadFile.DownloadCompleted)
                        Thread.Sleep(3);
                    count++;
                }
            }

            //foreach (var n in name)
            //{
            //    var file_size =new FileInfo(fraps_directory_120img + n+EXTENTION);
                
            //    if (file_size.Exists)
            //    {
            //        long size = file_size.Length;
            //        if (size != 0)
            //            ResizeImage(fraps_directory_120img, fraps_directory_80img + n, 80, n);
            //            ResizeImage(fraps_directory_120img, fraps_directory_data + n, 57, n);
            //    }
            //    else
            //    {
            //        downloadFile.DownloadFiles(HTTPURL + n + EXTENTION, root_directory + "\\Fraps\\data\\120\\" + n + EXTENTION);
            //        while (!downloadFile.DownloadCompleted)
            //            Thread.Sleep(3);
            //    }
            //}
        }


        private bool FileSizes(string name)
        {
            var file_size = new FileInfo(fraps_directory_120img + name + EXTENTION);

            if (file_size.Exists)
            {
                long size = file_size.Length;
                if (size >= 2096)
                    return false;
            }

           return true;

        }
            private List<string> ReadChampionName()
        {
            List<string> names = new List<string>();
            string[] name =  File.ReadAllLines(fraps_directory_default + "\\All.ini");
            foreach (var n in name)
            {
                string regex = Regex.Replace(n, "[0-9.,]", string.Empty);
                if (regex == "Fiddlesticks") regex = "Fiddlesticks";
                names.Add(regex);
            }
            return names;
        }

        private static void ResizeImage(string inp_dir,string out_dir,int size,string name,string ext_inp=".png",string ext_out = ".jpg")
        {

            using (var image = new MagickImage(inp_dir+name+ ext_inp))
            {

                var resize = new MagickGeometry(size, size);

                resize.IgnoreAspectRatio = true;

                image.Resize(resize);

                // Save the result
                image.Write(out_dir + ext_out);
            }

        }

    }
}
