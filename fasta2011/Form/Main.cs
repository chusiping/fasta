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
using fasta2011.Model;

namespace fasta2011
{
    

    public partial class Main : Form,IForm
    {
        private const int HOTKEY_ALT_R = 123;
        private const int HOTKEY_F1 = 124;
        private const int HOTKEY_CTRL_F1 = 125;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RegisterHotkeys();
        }
        private void RegisterHotkeys()
        {
            bool r1 = HotKey.RegisterHotKey(this.Handle, HOTKEY_ALT_R, AppSetting.key_Alt, AppSetting.key_Word);
            bool r2 = HotKey.RegisterHotKey(this.Handle, HOTKEY_F1, 0, Keys.F1);
            bool r3 = HotKey.RegisterHotKey(this.Handle, HOTKEY_CTRL_F1, HotKey.KeyModifiers.Ctrl, Keys.F1);

            if (!r1 || !r2 || !r3)
            {
                MessageBox.Show("热键注册失败，可能被占用！");
            }
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            HotKey.UnregisterHotKey(this.Handle, HOTKEY_ALT_R);
            HotKey.UnregisterHotKey(this.Handle, HOTKEY_F1);
            HotKey.UnregisterHotKey(this.Handle, HOTKEY_CTRL_F1);

            base.OnHandleDestroyed(e);
        }

        const int WM_SYSCOMMAND = 0x112;
        const int SC_CLOSE = 0xF060;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        protected override void WndProc(ref Message m)
        {
            //重置最小化按钮
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

            if (m.Msg == 0x0312) // WM_HOTKEY
            {
                switch (m.WParam.ToInt32())
                {
                    case HOTKEY_ALT_R:
                        ToggleWindow();
                        break;

                    case HOTKEY_F1:
                        MessageBox.Show("F1 按下");
                        break;

                    case HOTKEY_CTRL_F1:
                        MessageBox.Show("Ctrl+F1 按下");
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void ToggleWindow()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Hide();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


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
        private static DataBase Db_sqlite = new sqliteData(); 
        LogMa log = new LogMa();Cmd.Func fc = new Cmd.Func();
        #endregion 

       

        #region 初始化窗体
        //******************  初始化  ******************************
        public Main()
        {
            InitializeComponent();            
        }
        #endregion

        #region 是否从库中加载
        private void LoadData(bool IsFromDB = true)   
        {
            if (IsFromDB) Db_sqlite.ReadData();
            List<Alias> ls = Db_sqlite.AliasSet;                      
            comboBox1.Items.Clear();  ///aaa          
            ls.ForEach(p => comboBox1.Items.Add(new ComboBoxItem { ID = p.ID, Text = p.Name, Value = p.Path, Type = p.Type  }));
        }
        #endregion

        public void LoadData2()
        {
            DataBase _db = new sqliteData();
            _db.ReadData();
            List<Alias> ls = _db.AliasSet;
            Db_sqlite.AliasSet = _db.AliasSet;
            this.Invoke((EventHandler)delegate { 
                this.comboBox1.Items.Clear(); 
                ls.ForEach(p => comboBox1.Items.Add(new ComboBoxItem { ID = p.ID, Text = p.Name, Value = p.Path, Type = p.Type }));
            });
           
            


            listcb = new List<ComboBoxItem>();
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();  //数据源头          
            ls.ForEach(p => acsc.Add(p.Name));
            ls.ForEach(p => listcb.Add(new ComboBoxItem { Text = p.Name, Value = p.Path }));

            this.Invoke((EventHandler)delegate {
                comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;   //可用的属性有3种：Suggest Append SuggestAppend
                comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource; //要让Textbox有自动完成的功能，必须指定AutoCompleteSource属性，预设为None，也就是不使用，总共有7种来源可选
                comboBox1.AutoCompleteCustomSource = acsc;
            });
          

            LogAsyncWriter.Default.Info("读取sqlite初始化数据：LoadData2()", "Main.cs", "");
        }

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            Task task0 = new Task(LoadData2); task0.Start();
            //LoadData();
            GetAssemblyVersion();
            comboBox1.Focus();
            Handle1 = this.Handle;
            
            //Suggest();

            Task task1 = new Task(CommonWay.RegAdd);task1.Start();                          //异步加载注册表
            LogAsyncWriter.Default.Info("Form1_Load", "Main.cs", "");
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
                LogAsyncWriter.Default.Info(" HideForm 不显示", "Main.cs", "");
            }
            else                     //显示
            {
                this.Visible = true; 
                this.Show();
                this.notifyIcon1.Visible = false;
                this.TopMost = true;
                this.comboBox1.Focus();
                this.Activate();       //360对这个有影响
                LogAsyncWriter.Default.Info(" HideForm 显示", "Main.cs", "");
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
                Db_sqlite.DelItem(al);
                MessageBox.Show("\"" + al.Name + "\" 已删除");
                LoadData(true);
                Suggest();
            }
        }
        Alias GetAlias()
        {
            string s = comboBox1.Text.ToString();
            s = s.Replace(" del","").Trim();
            foreach (Alias item in Db_sqlite.AliasSet)
            {
                if (s == item.Name) return item;
            }
            return null;
        }
        //删除item
        int IsExtraProcess()
        {
            string s = comboBox1.Text.ToString().Trim();
            if (s.Contains(" del")) return 1;
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
        public void Suggest(DataBase _db)
        {
            listcb = new List<ComboBoxItem>();
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();  //数据源头          
            List<Alias> ls = _db.AliasSet; //old: List<Alias> ls = db.AliasSet;
            ls.ForEach(p => acsc.Add(p.Name));            
            ls.ForEach(p => listcb.Add(new ComboBoxItem { Text = p.Name,Value = p.Path  }));
            comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;   //可用的属性有3种：Suggest Append SuggestAppend
            comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource; //要让Textbox有自动完成的功能，必须指定AutoCompleteSource属性，预设为None，也就是不使用，总共有7种来源可选
            comboBox1.AutoCompleteCustomSource = acsc;
        }
        public void Suggest()
        {
            listcb = new List<ComboBoxItem>();
            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();  //数据源头          
            List<Alias> ls = Db_sqlite.AliasSet; //old: List<Alias> ls = db.AliasSet;
            ls.ForEach(p => acsc.Add(p.Name));
            ls.ForEach(p => listcb.Add(new ComboBoxItem { Text = p.Name, Value = p.Path }));
            comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;   //可用的属性有3种：Suggest Append SuggestAppend
            comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource; //要让Textbox有自动完成的功能，必须指定AutoCompleteSource属性，预设为None，也就是不使用，总共有7种来源可选
            comboBox1.AutoCompleteCustomSource = acsc;
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
            if (Db_sqlite.AliasSet == null) 
            {
                comboBox1.ShowTooltip(toolTip1, "数据未加载成功！",3000);   
                // ToastTool.Show(comboBox1, "数据未加载成功！", 3000);
                return;
            }
            foreach (var c in Db_sqlite.AliasSet)
            {
                if (c.Type != AliasType.txt.ToString() && s == c.Name )
                {
                    comboBox1.ShowTooltip(toolTip1, c.Path,20000);                    
                    break;
                }
            }
        }
        #region 获得并显示版本号
        public void GetAssemblyVersion()
        {
            AssemblyTitleAttribute copyright = (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            string strDebug = " - 测试版";
#if !DEBUG
            strDebug = "- Release";
#endif
            //return copyright.Title + strDebug;
            this.Text = copyright.Title + strDebug;
            LogAsyncWriter.Default.Info("获得并显示版本号：CommonWay.GetAssemblyVersion()", "hotkey.cs", "");
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
*   2021-6-15
        1. 改4.0 框架改为4.5
        2. LoadData2()替换ReLoadXml()
        3. 增加LogAsyncWriter.Default.Info日志
        4. 增加异步加载数据处理
*/
