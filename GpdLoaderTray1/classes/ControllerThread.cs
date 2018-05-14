using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XInputDotNetPure;

namespace GpdLoaderTray1
{
    public class ControllerThread
    {
        private delegate void myDelegate(int dir);
        public Thread thread;
        public GpdLoader parent;

        public PlayerIndex pi = PlayerIndex.One;
        public bool dpadDownDown = false;
        public bool dpadUpDown = false;

        public ControllerThread(GpdLoader _parent)
        {
            this.parent = _parent;
        }

        public void WorkThreadFunction()
        {
            GpdLoader frm = this.parent;
            

            bool xPressed = false;
            bool yPressed = false;

            bool firstDown = true;

            try
            {

                while (true)
                {
                    GamePadState state = GamePad.GetState(pi);
                    if (state.IsConnected == false)
                    {
                        GamePadState state1 = GamePad.GetState(PlayerIndex.One);
                        GamePadState state2 = GamePad.GetState(PlayerIndex.Two);
                        GamePadState state3 = GamePad.GetState(PlayerIndex.Three);
                        GamePadState state4 = GamePad.GetState(PlayerIndex.Four);
                        if (!state1.IsConnected && !state2.IsConnected && !state3.IsConnected && !state4.IsConnected)
                        {
                            break;
                        }
                        else
                        {
                            if (state1.IsConnected) { pi = PlayerIndex.One; }
                            if (state2.IsConnected) { pi = PlayerIndex.Two; }
                            if (state3.IsConnected) { pi = PlayerIndex.Three; }
                            if (state4.IsConnected) { pi = PlayerIndex.Four; }
                            continue;
                        }
                    }

                    if (state.DPad.Down == XInputDotNetPure.ButtonState.Pressed)
                    {
                        if (state.Buttons.B == XInputDotNetPure.ButtonState.Pressed)
                        {
                            frm.Invoke(new myDelegate(frm.InvokeScriptMove), new Object[] { 1 });
                        }
                        else
                        {
                            dpadDownDown = true;
                        }


                        
                    }

                    if (state.DPad.Down == XInputDotNetPure.ButtonState.Released)
                    {
                        if(dpadDownDown == true)
                        {
                            dpadDownDown = false;
                            frm.Invoke(new myDelegate(frm.InvokeScriptMove), new Object[] { 1 });

                        }
                    }

                    if (state.DPad.Up == XInputDotNetPure.ButtonState.Pressed)
                    {
                        if (state.Buttons.B == XInputDotNetPure.ButtonState.Pressed)
                        {
                            frm.Invoke(new myDelegate(frm.InvokeScriptMove), new Object[] { -1 });
                        }
                        else
                        {
                            dpadUpDown = true;
                        }
                    }

                    if (state.DPad.Up == XInputDotNetPure.ButtonState.Released)
                    {
                        if (dpadUpDown == true)
                        {

                            dpadUpDown = false;
                            frm.Invoke(new myDelegate(frm.InvokeScriptMove), new Object[] { -1 });
                        }
                    }

                    if (state.Buttons.X == XInputDotNetPure.ButtonState.Pressed)
                    {
                        xPressed = true;
                    }

                    if (state.Buttons.X == XInputDotNetPure.ButtonState.Released)
                    {
                        if (xPressed == true)
                        {

                            xPressed = false;
                            frm.Invoke(new myDelegate(frm.InvokeScriptEnter), new Object[] { -1 });
                        }
                    }

                    if (state.Buttons.Y == XInputDotNetPure.ButtonState.Pressed)
                    {
                        yPressed = true;
                    }

                    if (state.Buttons.Y == XInputDotNetPure.ButtonState.Released)
                    {
                        if (yPressed == true)
                        {

                            yPressed = false;
                            frm.Invoke(new myDelegate(frm.InvokeScriptBack), new Object[] { -1 });
                        }
                    }

                    Thread.Sleep(16);
                }
            }
            catch (ThreadAbortException e)
            {
                ;
            }
            finally
            {
                dpadUpDown = false;
                dpadDownDown = false;

                xPressed = false;
                yPressed = false;

            }

            return;
        }

        public void Stop()
        {
            thread.Abort();
        }

        public void Start()
        {
            if (thread != null) thread.Join();
            thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread.Start();
        }
    }
}
