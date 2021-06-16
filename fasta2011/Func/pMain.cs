using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using Zone;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fasta2011.Func
{
    public partial class Main : Form
    {
      /*  [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体

        Cmd.Func fc = new Cmd.Func();

        #region 重载最小化按钮
        const int WM_SYSCOMMAND = 0x112;
        const int SC_CLOSE = 0xF060;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    this.WindowState = FormWindowState.Normal;
                    fc.Open_Form("Form1");
                    return;
                }
                if (m.WParam.ToInt32() == SC_CLOSE)
                {
                    IsSureExitApp();
                    return;
                }
            }
            *//************************************ 响应热键 按快捷键**********************************//*
            const int WM_HOTKEY = 0x0312;
            //Console.WriteLine("m.Msg : " + m.Msg);
            if (m.WParam == null) return;
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 123:    //按下的是Alt+R 
                            //此处填写快捷键响应代码                               
                            HideForm();
                            break;
                        case 124:    //按下的是f1   
                            //此处填写快捷键响应代码                               
                            fc.Open_Form("Form1"); //OpenForm("Form1");
                            break;
                        case 125:    //按下的是ctrl + f1   
                            //此处填写快捷键响应代码                               
                            fc.Open_Form("Form_JinCheng");
                            break;

                    }
                    break;
            }

            base.WndProc(ref m);
        }
        #endregion

        #region 确定退出程序
        private void IsSureExitApp()
        {
            DialogResult response = DialogResult.Yes;
            if (response == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
                Application.Exit();
            }
            else
            {
                Restart();
            }
        }
        #endregion

        #region 重启Restart
        private void Restart()
        {
            Thread thtmp = new Thread(new ParameterizedThreadStart(run));
            object appName = Application.ExecutablePath;
            Thread.Sleep(2000);
            thtmp.Start(appName);
        }
        #endregion

        #region 运行一个进程
        private void run(Object obj)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.Start();
        }
        #endregion*/

    }
}
