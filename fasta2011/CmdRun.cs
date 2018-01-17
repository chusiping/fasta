using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace fasta2011
{
    public class Cmd
    {
        private const byte VK_LWIN = (byte)Keys.LWin;
        private const byte VK_R = (byte)Keys.R;
        private const int KEYEVENTF_KEYUP = 0x02;
        private const int WM_SETTEXT = 0x000C;
        private const int WM_CLICK = 0x00F5;

        [DllImport("user32")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int parentHandle, int childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(int hWnd, uint Msg, int wParam, string lParam);


        public static void Run(object _CmdText)
        {
            string CmdText = _CmdText.ToString();
            keybd_event(VK_LWIN, 0, 0, 0);
            keybd_event(VK_R, 0, 0, 0);
            keybd_event(VK_R, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0);

            System.Threading.Thread.Sleep(50);

            int h1 = 0, h2 = 0, h3;
            h1 = FindWindow(null, "运行");    //这要修改为窗口标题 
            if (h1 != 0)
            {
                h2 = FindWindowEx(h1, 0, "ComboBox", null);
                if (h2 != 0)
                {
                    SendMessage(h2, WM_SETTEXT, 0, CmdText);
                    System.Threading.Thread.Sleep(10);

                    h3 = FindWindowEx(h1, 0, null, "确定");    //这要修改为按钮文本 
                    if (h3 != 0)
                    {
                        SendMessage(h3, WM_CLICK, 0, null);
                    }
                    else
                    {
                        MessageBox.Show("h3==0");
                    }
                }
                else
                {
                    MessageBox.Show("h2==0");
                }
            }
            else
            {
                MessageBox.Show("h1==0");
            }
        }
    }
}
