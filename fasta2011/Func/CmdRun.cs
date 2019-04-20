using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace fasta2011
{
    public class Cmd
    {
        #region 变量设定
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
        #endregion

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

        public static ExeAlias AnalyseUrl(Alias al)
        {
            string stringIn = al.Name, OutString = al.Path;
            var fc = new Func(); var ea = new ExeAlias();
            //1 是网页
            //2 dos命令
            //3 k 杀死进程
            //4 资源管理器
            //5 


            if (stringIn.IndexOf("kill ") >= 0 || stringIn.IndexOf(" kill") >= 0 || stringIn.IndexOf(" k") >= 0)
            {
                string NameOrID = stringIn.Replace("kill", ""); NameOrID = NameOrID.Replace("k", ""); NameOrID = NameOrID.Replace(" ", "");
                if (fc.IsInt(NameOrID)) OutString = "ntsd -c q -p " + NameOrID;
                else
                {
                    OutString = string.Format("tskill \"{0}\"", fc.GetCmdString(NameOrID.Trim(), null));
                }
                //kill 结束进程命令
                ea.text = stringIn;
                ea.value = OutString;
                ea.cmdType = CmdType.kill;
                return ea;
            }
            else if (OutString.IndexOf("dos:") >= 0)
            {
                OutString = OutString.Replace("dos:", "");
                ea.text = stringIn;
                ea.value = OutString;
                ea.cmdType = CmdType.Dos;
                return ea;
            }
            else if (fc.IsNumber(stringIn))
            {
                //i = 3;  //是数字符串               
            }
            else if (OutString == "")
            {
                //i = 2;      //cmd命令
                ea.text = stringIn;
                ea.value = stringIn;
                ea.cmdType = CmdType.cmd;
            }
            ea.text = stringIn;
            ea.value = OutString;
            ea.cmdType = CmdType.exe;
            return ea;
        }

        //打开文件和目录,wangye
        public static void ExeExe(string al)
        {            
            var ParStart = new ParameterizedThreadStart(Cmd.Run);
            Thread myThread = new Thread(ParStart);
            myThread.Start(al);
        }
        //打开文件和目录,wangye
        public static void ProcessStar(string StarInfoFileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = StarInfoFileName;
            proc.StartInfo.Arguments = " ";
            proc.Start();
        }        
        //执行dos
        public static void ExeProcessCmd(string CmdString)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = " /c " + CmdString;
            proc.Start();
        }

        public static void RunAlias(ComboBoxItem cbi)
        {
            //comboBox1.SelectedItem 取出 后
            //根据 type 选择处理的方式执行就好
            AliasType AT = (new Cmd.Func()).GetAliasType(cbi.Type);
            string s = cbi.Value.ToString();
            string[] arr = s.Split(';');
            switch (AT)
            {
                case AliasType.http:
                    Array.ForEach(arr, item => ProcessStar(item));
                    break;
                case AliasType.exe:
                    ProcessStar(s);  // ExeExe(s);
                    break;
                case AliasType.dos:
                    ExeProcessCmd(s.Replace("dos:",""));  //因为历史数据问题，所有要替换dos：成空
                    break;
                case AliasType.txt:
                    (new Func()).Open_Form("RichForm",s);//显示富文本
                    break;
                default:
                    break;
            }

        }
        public static void ExeCmd(ExeAlias al)
        {
            switch (al.cmdType)
            {
                case CmdType.cmd: ExeExe(al.value); break;
                case CmdType.kill: Cmd.Run(al.text); break;
                case CmdType.exe:
                    break;
                case CmdType.Dos: ExeProcessCmd(al.text); break;
                default: break;
            }
        }

        public class Func
        {
            //获取枚举类型
            public AliasType GetAliasType(string s)
            {
                AliasType AT = new AliasType();
                foreach (AliasType hs1 in Enum.GetValues(typeof(AliasType)))
                {
                    if (hs1.ToString() == s) { AT = hs1; break; }
                }
                return AT;
            }
            #region void 验证是否是数字
            public bool IsInt(string str)
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


            public bool IsNumber(string oText)
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
            public string GetCmdString(string alase, ComboBox comboBox1)
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

            public void Open_Form(string fmName, string ArgStr = "")
            {
                bool IsOpen = false;
                foreach (Form f in Application.OpenForms)
                {
                    if (f.Name == fmName)
                    {
                        IsOpen = true;
                        try
                        {
                            Application.OpenForms[fmName].Close();
                        }
                        catch { }
                        break;
                    }
                }
                if (IsOpen == false)
                {
                    if (fmName == "Form1")
                    {
                        Form1 fm1 = new Form1();
                        fm1.Show();
                        fm1.Focus();
                        fm1.TopMost = true;
                        fm1.textBox1.Focus();
                    }
                    if (fmName == "Form_JinCheng")
                    {
                        Form_JinCheng fm1 = new Form_JinCheng();
                        fm1.Show();
                        fm1.Focus();
                    }
                    if (fmName == "RichForm")
                    {
                        RichForm fm1 = new RichForm(ArgStr);                        
                        fm1.Show();
                        fm1.Focus();                        
                    }
                }
            }
        }
    }
}
