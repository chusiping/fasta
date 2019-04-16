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

namespace fasta2011
{
    public partial class Main : Form,IForm
    {
        #region 定义变量 委托
        private delegate void DG_ReadXml(XmlDocument doc1, string s, ref AutoCompleteStringCollection acsc, ref List<ComboBoxItem> listcb);
        List<ComboBoxItem> listcb = null;
        CmdType ct = new CmdType();
        private bool windowCreate = false;
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        //定义变量,句柄类型
        public IntPtr Handle1;
        private DataBase db = new sqliteData(); private Alias _al;
        LogMa log = new LogMa();
        #endregion 

        #region 获得版本号
        public string GetAssemblyVersion()
        {
            AssemblyTitleAttribute copyright = (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            string strDebug = " - debug";
#if !DEBUG
            strDebug = "- Release";
#endif               
            return copyright.Title+strDebug;
        }
        #endregion 

        #region 初始化窗体
        //******************  初始化  ******************************
        public Main()
        {
            InitializeComponent();
            //Suggest();
        }
        #endregion

        private void LoadData(bool IsFromDB = true)   //是否从库中加载
        {
            if (IsFromDB) db.ReadData();
            List<Alias> ls = db.AliasSet;                      
            comboBox1.Items.Clear();
            ls.ForEach(p => comboBox1.Items.Add(new ComboBoxItem { Text = p.Name, Value = p.Path }));                         
        }
        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            //ReadXml(AppSetting.xmlPath1);
            //ReadXml(AppSetting.xmlPath2);
            //ReadXml(AppSetting.xmlPath3);
            LoadData();
            RegAdd();
            this.Text = GetAssemblyVersion();
            comboBox1.Focus();
            Handle1 = this.Handle;
            CreateSqliteDB();
            Suggest();
        }
        #endregion

        private void CreateSqliteDB()
        {
            var db = FastaContext.Instance;
            //var al = new Alias { Name = "test", Path = "aaaaa", Type = "dos" };  //测试
            //db.AliasSet.Add(al);db.SaveChanges(); //测试
        }

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
            Console.WriteLine("m.Msg" + m.Msg);
            if (m.Msg == null || m.WParam == null) return;
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
        #endregion

        #region Event 隐藏窗口
        public void HideForm()
        {
            if (this.Visible == true)  // 隐藏ing
            {
                this.Visible = false;
                this.notifyIcon1.Visible = false;
                this.TopMost = true;
                this.ShowInTaskbar = false;
            }
            else                     //显示
            {
                this.Visible = true; 
                this.Show();
                this.notifyIcon1.Visible = false;
                this.TopMost = true;
                this.comboBox1.Focus();
                this.Activate();       //360对这个有影响
            }
        }
        #endregion

        #region 激活窗口
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
        #endregion 

        #region 读取data.xml
        public void ReadXml(string path)
        {

            string s = "";
            XmlDocument doc = new XmlDocument();

            doc.Load(path);

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
                            var al = new Alias { Name = reader.GetAttribute(0), Path = reader.GetAttribute(1), AddTime = DateTime.Now };
                            db.AddItem(al);
                            //comboBox1.Items.Add(cbi1);
                        }
                        break;

                }
            }

            ReadXml2();
        }
        #endregion 

        #region 读取html.xml
        public void ReadXml2()
        {

            string s = "";
            XmlDocument doc = new XmlDocument();

            doc.Load(AppSetting.xmlPath2);

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
            ReadXml3();
        }
        #endregion 

        #region 读取autokey.xml
        public void ReadXml3()
        {

            string s = "";
            XmlDocument doc = new XmlDocument();

            doc.Load(AppSetting.xmlPath3);

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
        #endregion 

        #region 执行命令
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //1. 取出combox1的值，通过连续空格分割成数组，取出text和value
            //2. 通过值去判断使用exe http，还是别
            //3. 执行

            string exeUrl = "";
            string vlaue = "";
            if (e.KeyValue == 13)
            {
                try
                {
                    exeUrl = comboBox1.Text;
                    if (exeUrl.Trim() == "")return;
                    


                    //******************  遍历aliase检测是否存在exe  ******************************
                    //for (int i = 0; i < comboBox1.Items.Count; i++)
                    for (int i = 0; i < listcb.Count; i++)
                    {

                        ComboBoxItem cbi = (ComboBoxItem)listcb[i];


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
                        case CmdType.Dos:   //ping www.163.com
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
                                    ExeProcess(HttpArr[i].ToString());  // 打开文件夹 和 exe 
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex + vlaue.ToString() + "  不存在", "消息内容", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Logger.Trace(ex, false);

                }
            }
        }
        #endregion 

        #region 验证是否是数字，中间用","隔开，例： 1001,1002,1003,并计算出其中的个数
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
         # endregion

        #region void 验证是否是数字
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
        #endregion 

        #region 判断输入字符类型
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
        #endregion 

        #region 取得进程的名称
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
        #endregion 

        #region 定义枚举
        public enum CmdType
        {
            cmd = 0,
            kill = 1,
            exe = 2,
            stock = 3,
            Dos = 4
        }
        #endregion 

        #region 独立调用Process
        void ExeProcess(string StarInfoFileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = StarInfoFileName;
            proc.StartInfo.Arguments = " ";
            proc.Start();
        }
        #endregion 

        #region 独立调用cmd
        void ExeProcessCmd(string CmdString)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = " /c " + CmdString;
            proc.Start();
        }
        #endregion 

        #region 添加智能提示的列表
        private void ReadXml(XmlDocument doc1, string s, ref AutoCompleteStringCollection acsc, ref List<ComboBoxItem> listcb)
        {
            XmlNodeReader reader = new XmlNodeReader(doc1);
            while (reader.Read())
            {
                //判断当前读取得节点类型
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        s = reader.Name;
                        if (s.Equals(AppSetting.keyWord))
                        {
                            ComboBoxItem cbi1 = new ComboBoxItem();
                            cbi1.Text = reader.GetAttribute(0);
                            cbi1.Value = reader.GetAttribute(1);
                            listcb.Add(cbi1);
                            acsc.Add(cbi1.Text);
                        }
                        break;

                }
            }
            //return acsc;
        }
        #endregion 

        #region void 中文字符
        public bool IsChinese(string CString)
        {
            return Regex.IsMatch(CString, @"^[\u4e00-\u9fa5]+$");
        }
        #endregion 
        private string KongGeProcess(string s)
        {
            string ss = s;
            while(ss.Length < 60)
            {
                ss += " ";
            }
            return ss;
        }
        #region 自动匹配下拉
        public void Suggest()
        {
            listcb = new List<ComboBoxItem>();
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();            
            List<Alias> ls = db.AliasSet;
            ls.ForEach(p => acsc.Add(p.Name));
            //ls.ForEach(p => acsc.Add(KongGeProcess(p.Name) + p.Path));
            ls.ForEach(p => listcb.Add(new ComboBoxItem { Text = p.Name,Value = p.Path  }));
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.comboBox1.AutoCompleteCustomSource = acsc;            

            #region 新增加的模糊匹配方案，试行中
            this.comboBox1.TextUpdate += (a, b) =>
            {
                //this.comboBox1.Items.AddRange(listcb.ToArray();
                var input = this.comboBox1.Text.ToUpper();
                var _Fist = string.IsNullOrEmpty(input) ? "" : input.Substring(0, 1);
                if (IsChinese(_Fist))
                {
                    
                    if (string.IsNullOrEmpty(input)) this.comboBox1.Items.AddRange(listcb.ToArray());
                    else
                    {
                        this.comboBox1.Items.Clear();
                        //var newList = new List<string>();
                        var newList = new List<ComboBoxItem>();
                        //data.Where(x => x.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) != -1).ToArray();
                        foreach (var x in listcb)
                        {
                            //if (x.Text.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) != -1)
                            if (x.Text.Contains(input))
                            {
                                newList.Add(x);
                                Console.WriteLine("x:" + x.Text);
                                Console.WriteLine("input:" + input);
                            }
                        }
                        if (newList.Count > 0)
                            this.comboBox1.Items.AddRange(newList.ToArray());
                        else
                            this.comboBox1.Items.AddRange(listcb.ToArray());

                    }
                    this.comboBox1.Select(this.comboBox1.Text.Length, 0);
                    this.comboBox1.DroppedDown = true;
                    Cursor = Cursors.Default;

                }
            };
              #endregion
        }
        #endregion 

        

        #region  注册热键
        private void Form1_Activated(object sender, EventArgs e)
        {
            HotKey.RegisterHotKey(Handle, 123, AppSetting.key_Alt, AppSetting.key_Word);
            HotKey.RegisterHotKey(Handle, 124, 0, System.Windows.Forms.Keys.F1);
            HotKey.RegisterHotKey(Handle, 125, HotKey.KeyModifiers.Ctrl, Keys.F1);

            //原写死 HotKey.RegisterHotKey(Handle, 123, HotKey.KeyModifiers.Alt, Keys.R);

        }
        #endregion 

        #region 反注册热键
        private void Form1_Leave(object sender, EventArgs e)
        {
            HotKey.UnregisterHotKey(Handle, 123);
            HotKey.UnregisterHotKey(Handle, 124);
        }
        #endregion
        
        #region 失去窗口焦点
        private void Form1_Deactivate(object sender, EventArgs e)
        {
#if !DEBUG
            //this.Hide();
            //this.Visible = false;
#endif
        }
        #endregion

        #region ShowEdit
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
                    fm1.TopMost = true;
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

        #region 刷新重载入
        public void ReLoadXml()
        {
            comboBox1.Items.Clear();
            Suggest();
            LoadData();
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
        #endregion
      
        #region 添加启动项到注册表
        void RegAdd()
        {
            try
            {
                string FullPathFile = Application.ExecutablePath;       //获取带全路径的本程序   
                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true).SetValue("Fast-Pro", FullPathFile);//将本程序加入到注册表的RUN中  
            }
            catch (Exception)
            {

            }
        }
        #endregion
        
        #region 调用默认的浏览器
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
        #endregion

        #region 监视键盘输入
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
        }
        #endregion

        #region
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "kill?")
            {
                Cmd.Run("taskmgr");
            }
        }
        #endregion


    }
}
/*  修改日志
 *  2016-01-27 
        将遍历xml文件的方法，改成委托，只有一部分改了
        将combox的搜索方法，由从左向右搜索改为模糊匹配搜索
 *  2018-5-26 
        增加autokey.xml文件,为autohotkey的快捷键做提示
    2019-3-26
        将.net框架升级为4.0
*/
