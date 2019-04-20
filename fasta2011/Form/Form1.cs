using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Zone;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Net;

namespace fasta2011
{
    public partial class Form1 : Form,IForm2
    {
        #region 初始化全局变量
        public static bool IsOpen = false;
        private int ListItemIndex = -1;
        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);  
        private DataBase db = new sqliteData(); private Alias _al;
        private LogMa log = new LogMa();
        enum ActType { Add,Edit,Del };
        //private System.Windows.Forms.CheckedListBox checkedListBox1;
        ColorCodedCheckedListBox checkedListBox1 = new ColorCodedCheckedListBox();
        public Form1()
        {
            InitializeComponent();
            InitlistView1();
            InitCheckListBox();
            GetRichTextControl();
        }
        #endregion

        private void InitlistView1()
        {
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.LabelEdit = false; //是否可编辑,ListView只可编辑第一列。
            this.listView1.Scrollable = true;//有滚动条
            this.listView1.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView1.FullRowSelect = true;//是否可以选择行


            //this.listView1.HotTracking = true;// 当选择此属性时则HoverSelection自动为true和Activation属性为oneClick
            //this.listView1.HoverSelection = true;
            //this.listView1.Activation = ItemActivation.Standard; //
            //添加表头
            int w0 = AppConfig.ConfigGetValue("ListView1_c1_Width") == "" ? 35 : int.Parse(AppConfig.ConfigGetValue("ListView1_c1_Width"));
            int w1 = AppConfig.ConfigGetValue("ListView1_c1_Width") == "" ? 250 : int.Parse(AppConfig.ConfigGetValue("ListView1_c1_Width"));
            int w2 = AppConfig.ConfigGetValue("ListView1_c2_Width") == "" ? 900 : int.Parse(AppConfig.ConfigGetValue("ListView1_c2_Width"));
            this.listView1.Columns.Add("ID", w0);
            this.listView1.Columns.Add("别名", w1);
            this.listView1.Columns.Add("路径", w2);
        }
        private void InitCheckListBox()
        {
            foreach (AliasType hs1 in Enum.GetValues(typeof(AliasType)))
            {
                checkedListBox1.Items.Add(hs1.ToString());
            }
            checkedListBox1.Location = new System.Drawing.Point(55, 15);
            checkedListBox1.Size = new System.Drawing.Size(800, 22);
            checkedListBox1.MultiColumn = true;
            checkedListBox1.Name = "checkedListBox1";            
            checkedListBox1.CheckOnClick = true;
            //checkedListBox1.SelectionMode = SelectionMode.One;
            //checkedListBox1.SelectedIndexChanged += new EventHandler(SelectIndex);
            checkedListBox1.ItemCheck += new ItemCheckEventHandler(SelectIndex);
            this.Controls.Add(checkedListBox1);
        }
        private void SelectIndex(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Checked) return;//取消选中就不用进行以下操作
            for (int i = 0; i < ((CheckedListBox)sender).Items.Count; i++)
            {
                ((CheckedListBox)sender).SetItemChecked(i, false);//将所有选项设为不选中
            }
            e.NewValue = CheckState.Checked;//刷新
            ShowMaxRich();
            //foreach (AliasType hs1 in Enum.GetValues(typeof(AliasType)))
            //{
            //    if (AliasType.txt == hs1  &&   hs1.ToString() == checkedListBox1.SelectedValue.ToString() )
            //    {  }
            //}
        }
        //获取实体，准备增删改
        private void GetAlias(ActType act)
        {
            string s1 = textBox1.Text.Trim();
            string s2 = LineProcess();
            string type = checkedListBox1.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(type)) { MessageBox.Show("选择类别！");return; }
            ListViewItem p = new ListViewItem();
            p = ListItemIndex == -1 ? null : listView1.Items[ListItemIndex];  //鼠标点击时，已经得到行号
            switch (act)
            {
                case ActType.Add:
                    _al = new Alias { Name = s1, Path = s2, Type = type, AddTime = DateTime.Now };
                    break;
                case ActType.Edit:
                    _al = new Alias { ID = int.Parse(p.SubItems[0].Text), Name = s1, Path = s2, Type = type, AddTime = DateTime.Now };
                    break;
                case ActType.Del:
                    _al = new Alias { ID = int.Parse(p.SubItems[0].Text) };
                    break;
                default:
                    break;
            }
            ListItemIndex = -1;
        }
        private void LoadDataToListView(bool IsFromDB = true)   //是否从库中加载
        {
            if(IsFromDB)db.ReadData();
            List<Alias> ls = db.AliasSet;                       // text1搜索的时候改为内存集合搜索，不用每次都找数据库
            listView1.Items.Clear();
            ls.ForEach(p => this.listView1.Items.Add(new ListViewItem(new string[] {p.ID.ToString(), p.Name, p.Path })));
            textBox1.Clear(); textBox2.Clear();                 // 添加完后清空输入框
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDataToListView();
            textBox1.Focus();
            this.listView1.ListViewItemSorter = new Common.ListViewColumnSorter();
            this.listView1.ColumnClick += new ColumnClickEventHandler(Common.ListViewHelper.ListView_ColumnClick);
            this.Text = ((AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute))).Title;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text.Trim())) return;
            GetAlias(ActType.Add);
            int i = db.AddItem(_al);
            if (i == 2) { MessageBox.Show("此别名已存在！"); return; }
            if (i == 0) { MessageBox.Show("添加失败！"); return; }
            if (i == 1) {
                LoadDataToListView();                
                RefrushOwner();
                LineChange(false);
            }
        }
         #region 点击修改按钮
        private void button2_Click(object sender, EventArgs e)
        {
            GetAlias(ActType.Edit);
            db.EditItem(_al,_al);
            LoadDataToListView();
            button1.Visible = true;
            button2.Visible = false;
            RefrushOwner();
            LineChange(false);
        }
        #endregion 

        int cbmSeleIndex = -1;
        private void newComboxDropDown(object sender, EventArgs s)
        {
            if (textBox2.Visible == false)
            {
                foreach (var item in this.Controls)
                {
                    if (item is ComboBox)
                    {
                        ComboBox cb = (ComboBox)item;
                        cbmSeleIndex = cb.SelectedIndex;
                    }
                }
            }
        }

         #region 修改s1s2快捷键和值
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem p = new ListViewItem();
            p = listView1.Items[ListItemIndex];

            string s = p.SubItems[2].Text;

            //string[] arr = p.SubItems[2].Text.Split(';');
            //if (arr.Length > 1)
            //{
            //    s = s.Replace(";", "\r\n");
            //    textBox2.Height = textBox2.Height * arr.Length;
            //    LineChange();
            //    button3.Visible = true;
            //}
            button1.Visible = false;
            button2.Visible = true;
            textBox1.Text = p.SubItems[1].Text;
            //textBox2.Text = s;   
            SetRichText(s);
            RefrushOwner();                       
        }
        #endregion 

         #region 点击删除
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            GetAlias(ActType.Del);
            db.DelItem(_al); 
            LoadDataToListView();
            RefrushOwner();           
        }
         #endregion
        
         #region listView1选中行，得到index号
        private void listView1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected)
                {
                    ListItemIndex = i;
                }
            }
        }
        #endregion 

         #region 关闭本窗
        public void CloseNew()
        {
            this.Close();
        }
        #endregion 

         #region ESC 热键
        //************  按键  *************************
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                RefrushOwner();
                this.Close();
            }
            if(e.Modifiers == Keys.Control && e.KeyCode == Keys.Enter)
            {
                LineChange();
            }
            if ((int)e.Modifiers == ((int)Keys.Control + (int)Keys.Shift) && e.KeyCode == Keys.Enter)
            {
                LineChange(false);
            }
        }
        private void LineChange(bool IsMultiline = true)
        {
            textBox2.Multiline = IsMultiline;
            if(IsMultiline && textBox2.Height < 380) textBox2.Height += 20;
            listView1.Visible = !IsMultiline;
            textBox2.ScrollBars = ScrollBars.Both;
            button3.Visible = IsMultiline;
        }
        #endregion 

         #region 回车确认删除修改
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (button1.Visible == true)
                {
                    Add_Click(null, null);  //Add();
                }
                else
                {
                    LineChange();                   
                }
            }
        }
        #endregion 

         #region 双击选中进行修改
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            listView1_Click(sender, e);
            toolStripMenuItem1_Click(sender, e);
            LineProcess();
        }
        #endregion 

        bool CountTime(int star,int _long)
        {
            int dt2 = int.Parse(DateTime.Now.ToString("HHssmmfff"));
            if ((dt2 - star) > _long) { return true; }
            return false;
        }

         #region 别名快速搜索
        int s = 0;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            timer1.Stop();timer1.Start(); string sc = textBox1.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(sc)) {  LoadDataToListView(false); return; }
            //string sc = textBox1.Text.Trim().ToLower();
            //if (string.IsNullOrWhiteSpace(sc)) { s = 0; return; }
            //if (sc.Length > 0 ) s = int.Parse(DateTime.Now.ToString("HHssmmfff"));
            //else
            //{

            //}
            //if (CountTime(s, 1000))
            //    Console.WriteLine(textBox1.Text);//log.WriteLog(textBox1.Text);


            //TODO  把这个条件分离出来，解耦到最低
            //if (button1.Visible == true)
            //{
            //    string sc = textBox1.Text.Trim().ToLower();
            //    if (string.IsNullOrWhiteSpace(sc)) { s = 0; LoadDataToListView(false); }


            //    if ( (sc == ""|| sc.Length < 2 ))
            //    {

            //        if (sc.Length < 2) s = int.Parse(DateTime.Now.ToString("HHssmmfff"));
            //        return;
            //    }
            //    if (s != 0)
            //    {
            //        int dt2 = int.Parse(DateTime.Now.ToString("HHssmmfff"));
            //        int i2 = (dt2 - s);
            //        s = int.Parse(DateTime.Now.ToString("HHssmmfff"));
            //    }

            //    foreach (var item in listView1.Items)
            //    {
            //        ListViewItem lv = (ListViewItem)item;
            //        if (lv.Text.Contains(sc)|| lv.SubItems[1].Text.Contains(sc))
            //        {
            //            //SetXmlFilePath(lv.Text, lv.SubItems[1].Text);  //等待改进
            //            //int w= AppConfig.ConfigGetValue("ListView1_c2_max") == "" ? 250 : int.Parse(AppConfig.ConfigGetValue("ListView1_c2_max"));
            //            //if(Xmlalias.XmlFilePath == AppSetting.xmlName3) this.listView1.Columns[0].Width = w; // 如果是对autohotkey的字段搜索，则拓宽
            //            lv.ForeColor = Color.Red;                        
            //            listView1.TopItem = listView1.Items[lv.Index];
            //        }
            //        else
            //        {                        
            //            listView1.Items.Remove(lv);
            //        }                    
            //    }
            //}            
        }
        #endregion

        #region 刷新命令行
        void RefrushOwner()
        {         
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name == "Main")
                {
                    (f as IForm).ReLoadXml(); //(this.Owner as IForm).ReLoadXml();
                }
            }
        }
        #endregion 

        private string LineProcess()
        {
            string s =  GetRichText();
            if(checkedListBox1.SelectedItem.ToString() == AliasType.http.ToString()) s = s.Replace("\r\n", ";");
            return s;


            //if (textBox2.Multiline == false) return textBox2.Text.Trim();
            //string s = textBox2.Text.Trim();
            //s = s.Replace("\r\n", ";");
            //return s;
        }
        //取消多行
        private void button3_Click(object sender, EventArgs e)
        {
            LineChange(false);
            button3.Visible = false;
            button2.Visible = false;
            button1.Visible = true;
            LoadDataToListView(false);
        }
        private void GetRichTextControl()
        {
            this.webBrowser1.Url = new System.Uri(Application.StartupPath + "\\kindeditor\\WinForm.html", System.UriKind.Absolute);
            this.webBrowser1.ObjectForScripting = this; webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示              
        }
        enum RichType { Text,Html }
        string GetRichText()
        {
            string ty = "get_text";
            if (checkedListBox1.SelectedItem.ToString() == AliasType.txt.ToString()) ty = "getContent";
            string body = webBrowser1.Document.InvokeScript(ty).ToString();//
            return WebUtility.HtmlEncode(body);
        }
        void SetRichText(string s)
        {
            this.webBrowser1.Document.InvokeScript("setContent", new object[] { WebUtility.HtmlDecode(s) });
        }
        void ShowRichText()
        {
            ////默认显示textbox2 web1
            // 判断是txt还是rich
            // 如果是txt，则web1隐藏
            // 如果是rich，并且text隐藏 是编辑 则web1 显示，web2 隐藏
        }
        void MaxRich(bool ShowMax)
        {
            listView1.Visible = !ShowMax;
            if (ShowMax) webBrowser1.Height = 600;
            else
                webBrowser1.Height = 60;
        }
        void ShowMaxRich()
        {
            string s = checkedListBox1.SelectedItem.ToString();
            if (s == AliasType.http.ToString() || s == AliasType.txt.ToString()) MaxRich(true);
            else
                MaxRich(false);
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string sc = textBox1.Text.Trim().ToLower();
            if (sc.Length > 1 && sc.Length < 8 )
            { Console.WriteLine("seek..." + sc); Seek(sc); timer1.Stop(); }
        }
        private void Seek(string keyString)
        {
            if (button1.Visible == true)
            {
                var li = new List<ListViewItem>();
                foreach (var item in listView1.Items)
                {
                    ListViewItem lv = (ListViewItem)item;
                    if (lv.Text.Contains(keyString) || lv.SubItems[1].Text.Contains(keyString))
                    {
                        lv.ForeColor = Color.Red;
                        listView1.TopItem = listView1.Items[lv.Index];
                        li.Add(listView1.Items[lv.Index]);
                    }
                }
                listView1.Items.Clear();
                listView1.Items.AddRange(li.ToArray());
            }
        }
    } 
}
