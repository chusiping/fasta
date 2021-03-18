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
        private bool windowCreate = false;
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        //定义变量,句柄类型
        public IntPtr Handle1;
        private DataBase db = new sqliteData(); private Alias _al;
        LogMa log = new LogMa();Cmd.Func fc = new Cmd.Func();
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
        }
        #endregion

        private void LoadData(bool IsFromDB = true)   //是否从库中加载
        {
            if (IsFromDB) db.ReadData();
            List<Alias> ls = db.AliasSet;                      
            comboBox1.Items.Clear();            
            ls.ForEach(p => comboBox1.Items.Add(new ComboBoxItem { ID = p.ID, Text = p.Name, Value = p.Path, Type = p.Type  }));
        }
        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            RegAdd();
            this.Text = GetAssemblyVersion();
            comboBox1.Focus();
            Handle1 = this.Handle;            
            Suggest();            
        }
        #endregion


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
            /************************************ 响应热键 按快捷键**********************************/
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

        #region 执行命令
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            RunAlias(e);
        }
        void DeleteItem()
        {
            Alias al = GetAlias();
            if (al != null)
            {
                db.DelItem(al);
                LoadData(true);
                Suggest();
            }
        }
        Alias GetAlias()
        {
            string s = comboBox1.Text.ToString();
            s = s.Replace("--d","").Trim();
            foreach (Alias item in db.AliasSet)
            {
                if (s == item.Name) return item;
            }
            return null;
        }
        int IsExtraProcess()
        {
            string s = comboBox1.Text.ToString().Trim();
            if (s.Contains("--d")) return 1;
            return 0;
        }
        void RunAlias(KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            var aa = (ComboBoxItem)comboBox1.SelectedItem;
            int rt = IsExtraProcess();
            if ( rt > 0)
            {
                switch (rt)
                {
                    case 1:
                        DeleteItem();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Cmd.RunAlias(aa);                
            }
        }
        #endregion 
        #region void 中文字符
        public bool IsChinese(string CString)
        {
            return Regex.IsMatch(CString, @"^[\u4e00-\u9fa5]+$");
        }
        #endregion 
        #region 自动匹配下拉
        public void Suggest()
        {
            listcb = new List<ComboBoxItem>();
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();            
            List<Alias> ls = db.AliasSet;
            ls.ForEach(p => acsc.Add(p.Name));            
            ls.ForEach(p => listcb.Add(new ComboBoxItem { Text = p.Name,Value = p.Path  }));
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.comboBox1.AutoCompleteCustomSource = acsc;
        }
        #endregion 

        #region  注册热键
        private void Form1_Activated(object sender, EventArgs e)
        {
            HotKey.RegisterHotKey(Handle, 123, AppSetting.key_Alt, AppSetting.key_Word);
            HotKey.RegisterHotKey(Handle, 124, 0, System.Windows.Forms.Keys.F1);
            HotKey.RegisterHotKey(Handle, 125, HotKey.KeyModifiers.Ctrl, Keys.F1);
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
            this.Hide();
            this.Visible = false;
#endif
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
            LoadData();
            Suggest();
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


        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {            
            e.ToolTipSize = new Size(800,50);
        }
        bool IsMatchText(string input ,string vlaue)
        {
            var _Fist = string.IsNullOrEmpty(input) ? "" : input.Substring(0, 1);
            if (IsChinese(_Fist))
            {
                if (vlaue.Contains(input)) return true;
            }
            else
            {
                if (vlaue.Substring(0, input.Length) == input) return true;
            }
            return false;
        }
        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            var input = this.comboBox1.Text.ToUpper();
            var _Fist = string.IsNullOrEmpty(input) ? "" : input.Substring(0, 1);
            if (IsChinese(input))
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
                        if (x.Text.Contains(input))
                        {
                            newList.Add(x);
                        }
                    }
                    if (newList.Count > 0)
                        this.comboBox1.Items.AddRange(newList.ToArray());
                    else
                        this.comboBox1.Items.AddRange(listcb.ToArray());

                }
                //this.comboBox1.Select(this.comboBox1.Text.Length, 0);
                comboBox1.SelectionStart = comboBox1.Text.Length;
                this.comboBox1.DroppedDown = true;
                Cursor = Cursors.Default;

            }
        }
        //tooltip提示快捷指向
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {            
            string s = comboBox1.Text.Trim();
            foreach (var c in db.AliasSet)
            {
                if (c.Type != AliasType.txt.ToString() && s == c.Name )
                {
                    comboBox1.ShowTooltip(toolTip1, c.Path,8000);                    
                    break;
                }
            }
        }
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
