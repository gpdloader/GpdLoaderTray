using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GpdLoaderTray1
{
    public partial class MainHidden : Form
    {
        public TrayApplicationContext parent;

        public MainHidden(TrayApplicationContext _parent)
        {
            this.parent = _parent;
            UsbNotification.RegisterUsbDeviceNotification(this.Handle);
        }

        public bool deviceRemoved = false;
        public int cnt = 0;
        public int cnt2 = 0;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange)
            {
                

                

                switch ((int)m.WParam)
                {



                    case UsbNotification.DbtDeviceremovecomplete:
                        parent.gpd_form.Stop();
                        deviceRemoved = true;
                        cnt = 4;
                        
                        //toggle = true;
                        break;
                    case UsbNotification.DbtDevicearrival:
                        parent.gpd_form.Start();
                        break;
                    

                }
            }
        }
    }
}

