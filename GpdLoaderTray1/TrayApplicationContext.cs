using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GpdLoaderTray1
{
    public class TrayApplicationContext : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();

        public MainHidden mhf;
        public GpdLoader gpd_form;

        public TrayApplicationContext()
        {
            MenuItem showMenuItem = new MenuItem("Show", new EventHandler(ShowForm));
            MenuItem hideMenuItem = new MenuItem("Hide", new EventHandler(HideForm));
            //MenuItem configMenuItem = new MenuItem("Import GBA Games", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.Icon = new Icon(SystemIcons.Application, 100, 100);
            //notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] {showMenuItem,hideMenuItem,configMenuItem, exitMenuItem });
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { showMenuItem, hideMenuItem, exitMenuItem });
            notifyIcon.Visible = true;

            mhf = new MainHidden(this);
            gpd_form = new GpdLoader(this);
        }

        //void ShowConfig(object sender, EventArgs e)
        //{
        //    //MessageBox.Show(mhf.Handle.ToString());
        //    Configuration cfg = new Configuration();
        //    cfg.Show();
        //}

        void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            ExitProcess();
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);

        void ShowForm(object sender, EventArgs e)
        {
            gpd_form.Start();
        }

        void HideForm(object sender, EventArgs e)
        {
            gpd_form.Stop();
        }

        public static void ExitProcess()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
