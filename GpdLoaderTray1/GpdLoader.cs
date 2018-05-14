using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GpdLoaderTray1
{
    public partial class GpdLoader : Form
    {
        private delegate void myDelegate(int dir);

        //public Int32 totalItems;
        public System.Media.SoundPlayer player = new System.Media.SoundPlayer(Directory.GetCurrentDirectory() + "\\html\\01.wav");

        public ControllerThread xControllerThread;
        public TrayApplicationContext parent;

        public GpdLoader(TrayApplicationContext _parent)
        {
            this.parent = _parent;

            InitializeComponent();

            //custom
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this.webBrowser1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.webBrowser1_PreviewKeyDown);
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ClientSize = new System.Drawing.Size(1200, 600);
            this.Location = new Point(40,35);
            this.TopMost = true;

            //this.BackColor = Color.Fuchsia;
            //this.TransparencyKey = Color.Fuchsia;
            //this.Opacity = 0.5;
            

            


            //other
            xControllerThread = new ControllerThread(this);
            //xControllerThread.parent = this;

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            key.SetValue("GpdLoaderTray1.exe", 0x2AF9);

            string curDir = Directory.GetCurrentDirectory();
            webBrowser1.Url = new System.Uri(Directory.GetCurrentDirectory() + "\\html\\index2.html");// new Uri(String.Format("file:///{0}/index2.html", curDir));
            webBrowser1.ObjectForScripting = new Exports(this);
            return;
        }

        public void InvokeScriptEnter(int fn)
        {
            player.Play();
            webBrowser1.Document.InvokeScript("enterFolder");
            return;
        }

        public void InvokeScriptBack(int fn)
        {
            player.Play();
            webBrowser1.Document.InvokeScript("backFolder");
            return;
        }


        public void InvokeScriptMove(int fn)
        {
            if (fn == -1) //moves up
            {

                //player.Play();
                webBrowser1.Document.InvokeScript("moveUp");


            }

            if (fn == 1) //moves down
            {

                //player.Play();
                webBrowser1.Document.InvokeScript("moveDown");

            }

            return;


           
        }



        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //e.IsInputKey = true;


            if (e.KeyCode == Keys.Escape)
            {
                this.Stop();
            }

            if (e.KeyCode == Keys.D)
            {
                //MessageBox.Show("keydown");
                //this.Invoke(new myDelegate(this.InvokeScriptMove), new Object[] { 1 });
                //InvokeScriptMove(1);
                //webBrowser1.Document.Focus();
                //SendKeys.Send("{DOWN}");
                webBrowser1.Document.InvokeScript("eval", new object[] { "setTimeout(function(){ moveDown(); }, 0);" });

            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);

        public bool toggle = false;

        public void Start()
        {
            //kd.stop();
            //kd.run(function() {
            //    kd.tick();
            //});


            toggle = !toggle;
            CaptureScreen(toggle);
            webBrowser1.Document.InvokeScript("eval", new object[] { "updateBackground(" + toggle.ToString().ToLower() +");kd.run(function() {kd.tick();});" } );
            this.Show();
            Cursor.Hide();
            SetFocus(this.Handle);
            xControllerThread.Start();
            return;
        }

        public void Stop()
        {
            webBrowser1.Document.InvokeScript("eval", new object[] { "kd.stop();" });
            xControllerThread.Stop();
            this.Hide();
            //this.ActiveControl = null;
            Cursor.Show();
            return;
        }

        public void CaptureScreen(bool t)
        {
            

            Rectangle rect = new Rectangle(40, 35, 1200, 600);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            bmp = Lighten(bmp, -150);
            if (t)
            {
                bmp.Save("html\\background0.jpg", ImageFormat.Jpeg);
            }
            else
            {
                bmp.Save("html\\background1.jpg", ImageFormat.Jpeg);
            }


        }

        public Bitmap Lighten(Bitmap bitmap, int amount)
        {
            if (amount < -255 || amount > 255)
                return bitmap;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            int nVal = 0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bitmap.Width * 3;
                int nWidth = bitmap.Width * 3;

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nVal = (int)(p[0] + amount);

                        if (nVal < 0) nVal = 0;
                        if (nVal > 255) nVal = 255;

                        p[0] = (byte)nVal;

                        ++p;
                    }
                    p += nOffset;
                }
            }

            bitmap.UnlockBits(bmData);

            return bitmap;
        }

        private void GpdLoader_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Down)
            //{
            //    SendKeys.SendWait("{DOWN}");
            //}

        }
    }
}
