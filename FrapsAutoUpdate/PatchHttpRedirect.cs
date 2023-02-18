using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ModSkinLOLUpdater
{
    
     class PatchHttpRedirect
    {
        private static string root_directory = Directory.GetDirectoryRoot(Environment.SystemDirectory)+@"\FRAPS\";

        public  void PathExe()

        {  if (!File.Exists(root_directory + @"LOLPRO.exe")) return;
            KillLOLPRO();
            byte[] frpasExe = File.ReadAllBytes(root_directory + @"LOLPRO.exe");
            Console.WriteLine(root_directory);
            byte[] replacePattern = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] findPattern = new byte[] { 0x68, 0x00, 0x74, 0x00, 0x74, 0x00, 0x70 };
            byte[] newFrpasExe =  ReplaceBytes(frpasExe, findPattern, replacePattern);
            int count = 0;
            while (count < 10)
            {
                newFrpasExe = ReplaceBytes(newFrpasExe, findPattern, replacePattern);
                count++;
            }
            try
            {
                File.Delete(root_directory + @"LOLPRO.exe");
                File.WriteAllBytes(root_directory + @"\\LOLPRO.exe", newFrpasExe);
                Console.WriteLine("Pathed!");
            }
            catch
            {
                Console.WriteLine("Oops ,Writeng problem");
            }

        }
        protected static void KillLOLPRO()
        {
            foreach (var process in Process.GetProcessesByName("LOLPRO"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Process: LOLPRO running");
                Console.WriteLine("Kill process!");
                process.Kill();
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        protected static int FindBytes(byte[] src, byte[] find)
        {
            int index = -1;
            int matchIndex = 0;
            // handle the complete source array
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else if (src[i] == find[0])
                {
                    matchIndex = 1;
                }
                else
                {
                    matchIndex = 0;
                }

            }
            return index;
        }
        protected static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            byte[] dst = null;
            int index = FindBytes(src, search);
            if (index >= 0)
            {
                dst = new byte[src.Length - search.Length + repl.Length];
                // before found array
                Buffer.BlockCopy(src, 0, dst, 0, index);
                // repl copy
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                // rest of src array
                Buffer.BlockCopy(
                    src,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    src.Length - (index + search.Length));
            }
            return dst;
        }
    }
}
