using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GpdLoaderTray1
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class Exports
    {
        public GpdLoader parent;
        public string root_path;

        public Exports(GpdLoader _parent)
        {
            this.parent = _parent;
            root_path = Directory.GetCurrentDirectory() + "\\root";
        }

        public string GetItemInfo(string item_name,string directory)
        {
            try
            {
                return File.ReadAllText(directory.Replace("root:\\\\", root_path + "\\") + item_name);
            }
            catch { return "-1"; };
        }

        public string GetFolderData()
        {

            return Traverse(new List<string>() { root_path });
        }

        public string Traverse(List<string> path)
        {
            List<string> folder_items = new List<string>();

            for (int j = 0; j < path.Count; ++j)
            {
                List<string> items = new List<string>();

                if (j != 0) items.Add("{ \"name\": \"..\",\"tip\": \"prev_folder\"}");

                string[] dirs = Directory.GetDirectories(path[j]);
                for (int i = 0; i < dirs.Length; ++i)
                {
                    string js = "{ \"name\":\"" + Path.GetFileName(dirs[i]) + "\", \"tip\": \"folder\" }";
                    items.Add(js);
                    path.Add(dirs[i]);
                }

                string[] files = Directory.GetFiles(path[j], "*.meta");
                for (int i = 0; i < files.Length; ++i)
                {
                    string js = "{ \"name\":\"" + Path.GetFileName(files[i]) + "\", \"tip\": \"file\" }";
                    items.Add(js);
                }

                string fn;
                if (j == 0) fn = "";
                else
                {
                    fn = path[j].Replace(path[0] + "\\", "");
                    fn = fn.Replace("\\", "\\\\") + "\\\\";
                }


                string js_f = "\"root:\\\\\\\\" + fn + "\": [" + String.Join(",", items.ToArray()) + "]";
                folder_items.Add(js_f);
            }
            string ret = "{" + String.Join(",", folder_items.ToArray()) + "}";
            return ret;


        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);

        public void Run(string filename,string current_folder)
        {
            

            Process proc = new Process();
            proc.StartInfo.FileName = current_folder.Replace("root:\\\\", root_path + "\\") + filename;
            proc.Start();
            parent.Stop();
            proc.WaitForExit();
            parent.Start();

            return;
        }

    }
}
