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



// 1 异步处理
// 2 配置热键 

namespace fasta2011
{
    public partial class Main : Form,IForm
    {

        CmdType ct = new CmdType();
        private bool windowCreate = false;

        public string GetAssemblyVersion()
        {
            AssemblyTitleAttribute copyright = (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            return copyright.Title;
        }
       
        //******************  初始化  ******************************
        public Main()
        {
            InitializeComponent();
            Suggest();
        }

        //******************  打开窗口  ******************************
        private void Form1_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            DoXml.CreateExec();
            ReadXml();
            RegAdd();
            this.Text = GetAssemblyVersion();
        }



        /******************************* 重载最小化按钮 **********************************/
        const int WM_SYSCOMMAND = 0x112;
        const int SC_CLOSE = 0xF060;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;


        /************************************ 最小化事件 **********************************/
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    this.WindowState = FormWindowState.Normal;
                    ShowEdit("Form1");
                    return;
                }
                if (m.WParam.ToInt32() == SC_CLOSE)
                {
                    IsSureExitApp();
                    return;
                }
            }
            /************************************ 响应热键 按快捷键**********************************/
            const int WM_HOTKEY = 0x0312;
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 123:    //按下的是Alt+D   
                            //此处填写快捷键响应代码                               
                            HideForm();
                            break;
                        case 124:    //按下的是f1   
                            //此处填写快捷键响应代码                               
                            ShowEdit("Form1");
                            break;
                        case 125:    //按下的是ctrl + f1   
                            //此处填写快捷键响应代码                               
                            ShowEdit("Form_JinCheng");
                            break;

                    }
                    break;
            }

            base.WndProc(ref m);
        }
        /******************************* 隐藏窗口 **********************************/
        public void HideForm()
        {
            //throw new Exception();
            if (this.Visible == true)
            {

                this.Visible = false;
                this.notifyIcon1.Visible = false;
                this.TopMost = true;



                //this.ShowInTaskbar = false;
            }
            else
            {
                this.Visible = true;
                this.showWindows();
                this.notifyIcon1.Visible = false;
                this.TopMost = true;
                this.Activate();
                try
                {
                    //************ 隐藏form1 ***************************
                    //Application.OpenForms["Form1"].Dispose();
                }
                catch { }
                //this.BringToFront();
                //this.ShowInTaskbar = false;                                
                //this.WindowState = FormWindowSta
                //this.Visible = true;
                ////this.ShowInTaskbar = true;
                //this.WindowState = FormWindowState.Normal;
                //this.Show();
            }
        }
        /*********************************************************************/

        protected override void OnActivated(EventArgs e)
        {
            if (windowCreate)
            {
                base.Visible = false;
                windowCreate = false;
                this.notifyIcon1.Visible = false;
            }

            base.OnActivated(e);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //showWindows();
        }
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            //this.notifyIcon1.Visible = false;
            HideForm();
        }
        //********************* 隐藏窗口 *********************
        public void hidWindows()
        {
            this.Hide();
            this.ShowInTaskbar = false;
        }

        //***********************  显示窗口  *******************
        public void showWindows()
        {
            this.Visible = true;
        }
        public void ReadXml()
        {

            string s = "";
            XmlDocument doc = new XmlDocument();

            doc.Load(AppSetting.xmlPath);

            XmlNodeReader reader = new XmlNodeReader(doc);

            while (reader.Read())
            {

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        s = reader.Name;
                        if (s.Equals(AppSetting.keyWord))
                        {

                            ComboBoxItem cbi1 = new ComboBoxItem();
                            cbi1.Text = reader.GetAttribute(0);
                            cbi1.Value = reader.GetAttribute(1);
                            comboBox1.Items.Add(cbi1);
                        }
                        break;

                }
            }


        }

        //******************  执行调用  ******************************
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                string exeUrl = "";
                string vlaue = "";
                try
                {
                    exeUrl = comboBox1.Text;
                    if (exeUrl.Trim() == "") return;


                    //******************  遍历aliase检测是否存在exe  ******************************
                    for (int i = 0; i < comboBox1.Items.Count; i++)
                    {

                        ComboBoxItem cbi = (ComboBoxItem)comboBox1.Items[i];


                        if (exeUrl == cbi.Text)
                        {
                            vlaue = cbi.Value.ToString();
                            ct = CmdType.exe;
                            break;
                        }
                    }

                    string OutString = vlaue;
                    int analyse_i = analyse(exeUrl, ref OutString);
                    //******************  输入的命令没有在aliase列表内   ******************************
                    
                        if (analyse_i == 1) ct = CmdType.kill;
                        if (analyse_i == 2) ct = CmdType.cmd;
                        if (analyse_i == 3) ct = CmdType.stock;                                           
                        if (analyse_i == 4) ct = CmdType.Dos;
                    switch (ct)
                    {
                        //**********  开始 运行 *********************
                        case CmdType.cmd:
                            ParameterizedThreadStart ParStart = new ParameterizedThreadStart(Cmd.Run);
                            Thread myThread = new Thread(ParStart);
                            myThread.Start(exeUrl);                            
                            //Cmd.Run(exeUrl);
                            break;
                        case CmdType.Dos:
                            Cmd.Run(OutString);
                            break;

                        //**********  kill 掉进程  *********************
                        case CmdType.kill:
                            ExeProcessCmd(OutString);
                            break;

                        //**********  查看股票代码的网页  *********************
                        case CmdType.stock:
                            string[] StockArr = exeUrl.Replace(" ","").Split(',');
                            for (int i = 0; i < StockArr.Length; i++)
                            {
                                if (IsNumber(StockArr[i].ToString().Trim()))
                                {
                                    string code = StockArr[i].Substring(0, 1) == "6" ? "sh" : "sz";
                                    /*   搜狐网页        
                                    ShellExecute(IntPtr.Zero, "open", @"http://q.stock.sohu.com/cn/" + StockArr[i].ToString() + "/index_kp.shtml#1", "", "", ShowCommands.SW_SHOWNOACTIVATE); // //System.Diagnostics.Process.Start(@"D:\绿色软件\TheWorld1.43\TheWorldFull\TheWorld.exe", HttpArr[i].ToString());                                            
                                    ShellExecute(IntPtr.Zero, "open", @"http://biz.finance.sina.com.cn/suggest/lookup_n.php?q=" + StockArr[i].ToString() + "&country=stock", "", "", ShowCommands.SW_SHOWNOACTIVATE); // //System.Diagnostics.Process.Start(@"D:\绿色软件\TheWorld1.43\TheWorldFull\TheWorld.exe", HttpArr[i].ToString());                                               
                                    */
                                    ShellExecute(IntPtr.Zero, "open", string.Format(@"http://quote.eastmoney.com/{0}.html", code + StockArr[i]), "", "", ShowCommands.SW_SHOWNOACTIVATE); 
                                }
                            }
                            break;

                        //**********  alalise 命令 *********************
                        case CmdType.exe:

                            string[] HttpArr = vlaue.Split(';');
                            for (int i = 0; i < HttpArr.Length; i++)
                            {
                                if (HttpArr[i].IndexOf("://") >= 0)
                                {
                                    ShellExecute(IntPtr.Zero, "open", HttpArr[i].ToString(), "", "", ShowCommands.SW_SHOWNOACTIVATE); // //System.Diagnostics.Process.Start(@"D:\绿色软件\TheWorld1.43\TheWorldFull\TheWorld.exe", HttpArr[i].ToString());        
                                }
                                else
                                {
                                    ExeProcess(HttpArr[i].ToString());
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(vlaue.ToString() + "  不存在", "消息内容", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Logger.Trace(ex, false);

                }
            }
        }


        /// <summary>
        /// 验证是否是数字，中间用","隔开，例： 1001,1002,1003,并计算出其中的个数
        /// </summary>
        /// <param name="oText"></param>
        /// <returns></returns>
        public bool IsNumber(string oText)
        {
            if (oText == null) return false;
            oText.Trim();
            string[] arr = oText.Replace(" ",",").Split(',');
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

        //******************  验证是否是数字   ******************************
        bool IsInt(string str)
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
        //******************  拆解输入的字符   ******************************
        int analyse(string stringIn, ref string OutString)
        {
            int i = 0;

            if (stringIn.IndexOf("kill ") >= 0 || stringIn.IndexOf(" kill") >= 0 || stringIn.IndexOf(" k") >= 0)
            {
                string NameOrID = stringIn.Replace("kill", ""); NameOrID = NameOrID.Replace("k", ""); NameOrID = NameOrID.Replace(" ", "");
                if (IsInt(NameOrID))
                {
                    OutString = "ntsd -c q -p " + NameOrID;
                }
                else
                {
                    OutString = string.Format("tskill \"{0}\"", GetCmdString(NameOrID.Trim()));
                }
                i = 1;      //kill 结束进程命令
            }
            else if (OutString.IndexOf("dos:") >= 0)
            {
                OutString = OutString.Replace("dos:", "");
                return 4;
            }
            else if (IsNumber(stringIn))
            {
                i = 3;  //是数字符串               
            }
            else if (OutString=="")
            {
                i = 2;      //cmd命令
            }

            return i;

        }

        //******************  取得进程的名称   ******************************
        string GetCmdString(string alase)
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
        public enum CmdType
        {
            cmd = 0,
            kill = 1,
            exe = 2,
            stock = 3,
            Dos = 4
        }
        //******************  独立调用Process   ******************************
        void ExeProcess(string StarInfoFileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = StarInfoFileName;
            proc.StartInfo.Arguments = " ";
            proc.Start();
        }

        //******************  独立调用cmd  ******************************
        void ExeProcessCmd(string CmdString)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = " /c " + CmdString;
            proc.Start();
        }


        //******************  添加智能提示的列表  ******************************
        public void Suggest()
        {
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            ComboBoxItem cbi1 = new ComboBoxItem();
            string s = "";
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(AppSetting.xmlPath);
            }
            catch
            {
                if (System.IO.File.Exists(AppSetting.xmlPath))
                {
                    MessageBox.Show("消息内容", "读取文件" + AppSetting.xmlPath + "错误！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    DoXml.CreateExec();
                }

            }


            XmlNodeReader reader = new XmlNodeReader(doc);
            // 读取XML文件中的数据，并显示出来
            while (reader.Read())
            {
                //判断当前读取得节点类型
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        s = reader.Name;
                        if (s.Equals(AppSetting.keyWord))
                        {

                            cbi1.Text = reader.GetAttribute(0);
                            cbi1.Value = reader.GetAttribute(1);

                            acsc.Add(cbi1.Text);
                        }
                        break;

                }
            }
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.comboBox1.AutoCompleteCustomSource = acsc;

        }

        //******************  注册热键  ******************************
        private void Form1_Activated(object sender, EventArgs e)
        {
            HotKey.RegisterHotKey(Handle, 123, AppSetting.key_Alt, AppSetting.key_Word);
            HotKey.RegisterHotKey(Handle, 124, 0, System.Windows.Forms.Keys.F1);
            HotKey.RegisterHotKey(Handle, 125, HotKey.KeyModifiers.Ctrl, Keys.F1);

            //原写死 HotKey.RegisterHotKey(Handle, 123, HotKey.KeyModifiers.Alt, Keys.R);

        }


        //******************  反注册热键  ******************************
        private void Form1_Leave(object sender, EventArgs e)
        {
            HotKey.UnregisterHotKey(Handle, 123);
            HotKey.UnregisterHotKey(Handle, 124);
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
        }


        private void Editxml()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "notepad.exe";
            proc.StartInfo.Arguments = AppSetting.xmlPath;
            proc.Start();
        }
        private void ShowEdit(string fmName)
        {
            bool IsOpen = false;
           
            //fm1.Owner = this;
           
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
                    fm1.textBox1.Focus();
                }
                if (fmName == "Form_JinCheng")
                {
                    Form_JinCheng fm1 = new Form_JinCheng();
                    fm1.Show();
                    fm1.Focus();
                }    
            }           
        }

        private void IsSureExitApp()
        {
            //DialogResult response = MessageBox.Show("退出请确认，从启动请按否", "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            DialogResult response = DialogResult.Yes;
            if (response == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
                Application.Exit();
            }
            else
            {
                //    comboBox1.Items.Clear();
                //    Suggest();
                //    ReadXml();
                //Application.ExitThread();
                Restart();
            }
        }

        public void ReLoadXml()
        {
            comboBox1.Items.Clear();
            Suggest();
            ReadXml();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {

        }
        //重启程序
        private void Restart()
        {
            Thread thtmp = new Thread(new ParameterizedThreadStart(run));
            object appName = Application.ExecutablePath;
            Thread.Sleep(2000);
            thtmp.Start(appName);
        }

        private void run(Object obj)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.Start();
        }
        /// <summary>
        /// 从注册表中取值
        /// </summary>
        /// <returns></returns>
        public string GetRegistryKey()
        {
            string str = "";
            RegistryKey pregkey;
            pregkey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command ", true);







            if (pregkey == null)
            {
                str = @"C:\Program Files\Internet Explorer\IEXPLORE.EXE";
            }

            return str;
        }


        /// <summary>
        /// 添加启动项到注册表
        /// </summary>
        void RegAdd()
        {
            string FullPathFile = Application.ExecutablePath;       //获取带全路径的本程序   
            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true).SetValue("Fast2011", FullPathFile);//将本程序加入到注册表的RUN中  
        }
        //============================调用默认的浏览器===============================
        public enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(
        IntPtr hwnd,
        string lpOperation,
        string lpFile,
        string lpParameters,
        string lpDirectory,
        ShowCommands nShowCmd);

        private void Main_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
        }

        //**********  监视输入的字符 如果是kill *********************
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "kill?")
            {
                Cmd.Run("taskmgr");
            }
        }
    }
}
