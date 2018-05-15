using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using System.Diagnostics;

namespace ImportExe1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Usage();
                return;
            }

            //IMPORT
            string emu_path = args[0];
            string file = args[1];
            string outp_dir = args[2];

            string name = Path.GetFileNameWithoutExtension(file);
            //name = name.Substring(name.IndexOf("\\") + 1);

            Directory.CreateDirectory(outp_dir);
            
            string emu_path_directory = Path.GetFullPath(emu_path);

            System.IO.File.WriteAllText(
                    outp_dir + "\\" + name + ".meta",
                    "{\"title\":\"" + name + "\",\"platform\":\"GBA\",\"shortcut\":\"" + name + ".lnk\"}");
            CreateShortcut(Path.GetFullPath(outp_dir) + "\\" + name + ".lnk", Path.GetFullPath(emu_path), " \"" + Path.GetFullPath(file) + "\" -noconsole -fullscreen", emu_path_directory);
            return;
        }

        private static void Usage()
        {
            Console.WriteLine("Imports one dos game for use with GPDTRAY.");
            Console.WriteLine("");
            Console.WriteLine("IMPORTEXE DosEmulatorPath ExePath OutputDir");
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
