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

        public static ExeAlias AnalyseUrl(string stringIn, string OutString)
        {
            var ea = new ExeAlias();
            //1 是网页
            //2 dos命令
            //3 k 杀死进程
            //4 资源管理器
            //5 
            

            if (stringIn.IndexOf("kill ") >= 0 || stringIn.IndexOf(" kill") >= 0 || stringIn.IndexOf(" k") >= 0)
            {
                string NameOrID = stringIn.Replace("kill", ""); NameOrID = NameOrID.Replace("k", ""); NameOrID = NameOrID.Replace(" ", "");
                if (Cmd.IsInt(NameOrID))
                {
                    OutString = "ntsd -c q -p " + NameOrID;
                }
                else
                {
                    OutString = string.Format("tskill \"{0}\"", GetCmdString(NameOrID.Trim(),null));
                }
                      //kill 结束进程命令
            }
            else if (OutString.IndexOf("dos:") >= 0)
            {
                OutString = OutString.Replace("dos:", "");
                //return 4;
            }
            else if (IsNumber(stringIn))
            {
                //i = 3;  //是数字符串               
            }
            else if (OutString == "")
            {
                //i = 2;      //cmd命令
            }

            return ea;
        }

        #region void 验证是否是数字
        static bool IsInt(string str)
        {
            bool bl = false;
            try
            {
                int i = int.Parse(str);
            }
            catch
            {
                bl = false;
            }
            return bl;
        }
        #endregion

        static bool IsNumber(string oText)
        {
            if (oText == null) return false;
            oText.Trim();
            string[] arr = oText.Replace(" ", ",").Split(',');
            try
            {
                foreach (string a in arr)
                {
                    if (a.Trim() != "" && a != null)
                    {
                        long var1 = Convert.ToInt64(a.Trim());
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 取得进程的名称
        static string GetCmdString(string alase,ComboBox comboBox1)
        {
            string str = "";
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)comboBox1.Items[i];
                if (alase == cbi.Text)
                {
                    str = cbi.Value.ToString();
                    break;
                }
            }
            if (str.IndexOf(".exe") > 0)
            {
                str = System.IO.Path.GetFileName(str).Replace(".exe", "");
            }
            if (str == "") str = alase;
            return str;
        }
        #endregion 
    }


}
