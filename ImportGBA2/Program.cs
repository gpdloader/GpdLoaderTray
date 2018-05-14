using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using System.IO;
using System.Diagnostics;

namespace ImportGBA2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    Usage();
                    return;
                }

                //IMPORT
                string emu_path = args[0];
                string games_dir = args[1];
                string outp_dir = args[2];


                string[] files = Directory.GetFiles(games_dir, "*.gba");

                Directory.CreateDirectory(outp_dir);

                int j = 0;
                string folder = "";
                for (int i = 0; i < files.Length; ++i)
                {
                    string file = files[i];

                    string name = Path.GetFileNameWithoutExtension(file);
                    if (i % 50 == 0)
                    {
                        ++j;
                        folder = j.ToString();
                        Directory.CreateDirectory(outp_dir + "\\" + folder);
                    }

                    string emu_path_directory = Path.GetFullPath(emu_path);


                    System.IO.File.WriteAllText(
                        outp_dir + "\\" + folder + "\\" + name + ".meta",
                        "{\"title\":\"" + name + "\",\"platform\":\"GBA\",\"shortcut\":\"" + name + ".lnk\"}");
                    CreateShortcut(Path.GetFullPath(outp_dir) + "\\" + folder + "\\" + name + ".lnk", Path.GetFullPath(emu_path), " \"" + Path.GetFullPath(file) + "\"", emu_path_directory);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        private static void Usage()
        {
            Console.WriteLine("Imports gba games for use with GPDTRAY.");
            Console.WriteLine("");
            Console.WriteLine("IMPORTGBA GbaEmulatorPath GbaGamesDir OutputDir");
            return;
        }

        private static void CreateShortcut(string path, string exe, string arguments, string location)
        {
            WshShell shell = new WshShell();
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(path);
            link.TargetPath = exe;
            link.Arguments = arguments;
            link.WorkingDirectory = location;
            link.Save();


        }
    }
}
