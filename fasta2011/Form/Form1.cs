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

            this.listView1.Columns.Add("ID",   GetWidth("c1_w"));
            this.listView1.Columns.Add("别名", GetWidth("c2_w"));
            this.listView1.Columns.Add("路径", GetWidth("c3_w"));
            this.listView1.Columns.Add("类型", GetWidth("c4_w"));
        }
        private int GetWidth(string ss)
        {
            string s = AppConfig.ConfigGetValue("app", ss);
            int rt = string.IsNullOrEmpty(s) ? 35 : int.Parse(s);
            return rt;
        }
        private void InitCheckListBox()
        {
            foreach (AliasType hs1 in Enum.GetValues(typeof(AliasType)))
            {
                checkedListBox1.Items.Add(hs1.ToString());
            }
            checkedListBox1.Location = new System.Drawing.Point(62, 20);
            checkedListBox1.Size = new System.Drawing.Size(800, 30);
            checkedListBox1.MultiColumn = true;
            checkedListBox1.Name = "checkedListBox1";            
            checkedListBox1.CheckOnClick = true;            
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
        }
        private void SetCheckListBoxValue(string v)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                var c = checkedListBox1.Items[i];
                if (c.ToString() == v)
                { checkedListBox1.SetItemChecked(i, true); checkedListBox1.SetSelected(i, true); }
            }
        }
        //获取实体，准备增删改
        private void GetAlias(ActType act)
        {
            string s1 = textBox1.Text.Trim(); var c = checkedListBox1.SelectedItem;
            string s2 = LineProcess();
            string type = c == null ? AliasType.http.ToString() : c.ToString();
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
        }
        private void LoadDataToListView(bool IsFromDB = true)   //是否从库中加载
        {
            if(IsFromDB)db.ReadData();
            List<Alias> ls = db.AliasSet;                       // text1搜索的时候改为内存集合搜索，不用每次都找数据库
            listView1.Items.Clear();
            ls.ForEach(p => this.listView1.Items.Add(new ListViewItem(new string[] {p.ID.ToString(), p.Name, p.Path,p.Type })));
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
            ListItemIndex = -1;  GetAlias(ActType.Add);            
            int i = db.AddItem(_al);
            if (i == 2) { MessageBox.Show("此别名已存在！"); return; }
            if (i == 0) { MessageBox.Show("添加失败！"); return; }
            if (i == 1) {
                LoadDataToListView();                
                RefrushOwner();
                LineChange(false);
                MaxRich(false);
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
            MaxRich(false);
        }
        #endregion 

        #region 修改s1s2快捷键和值
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var p = new ListViewItem(); string ht = AliasType.txt.ToString();
            p = listView1.Items[ListItemIndex];
            Alias al = new Alias { ID = int.Parse(p.SubItems[0].Text), Name = p.SubItems[1].Text, Path = p.SubItems[2].Text, Type = p.SubItems[3].Text };
            SetCheckListBoxValue(al.Type);textBox1.Text = al.Name; 
            if (al.Type == AliasType.txt.ToString())
            {
                MaxRich(true);
                SetRichText(al.Path);  //TODO 富文本从数据库提取
            }
            else
            {
                textBox2.Text = al.Path;
                MaxRich(false);
                LineChange();
            }
            button1.Visible = false;
            button2.Visible = true;
            RefrushOwner();
        }
        #endregion 

         #region 点击删除
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            GetAlias(ActType.Del);
            db.DelItem(_al);
            listView1.Items.RemoveAt(ListItemIndex); //LoadDataToListView(); TODO 删除后立即刷新或者不刷新
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


         #region 双击选中进行修改
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            listView1_Click(sender, e);
            toolStripMenuItem1_Click(sender, e);
            LineProcess();
        }
        #endregion 

         #region 别名快速搜索
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            timer1.Stop();timer1.Start(); string sc = textBox1.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(sc)) {  LoadDataToListView(false); return; }
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
        //http多行处理
        private string LineProcess()
        {
            string s = ""; var c = checkedListBox1.SelectedItem;
            if (c!=null && c.ToString() == AliasType.txt.ToString())
                s = GetRichText();
            else
                s = textBox2.Text.Trim();            
            //s = s.Replace("\r\n", ";");
            return s;
        }
        //取消多行
        private void button3_Click(object sender, EventArgs e)
        {
            LineChange(false);
            button3.Visible = false;
            button2.Visible = false;
            button1.Visible = true;
            //LoadDataToListView(false);
            MaxRich(false);
        }
        private void GetRichTextControl()
        {
            this.webBrowser1.Url = new System.Uri(Application.StartupPath + "\\kindeditor\\WinForm.html", System.UriKind.Absolute);
            this.webBrowser1.ObjectForScripting = this; webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示              
            webBrowser1.Visible = false;
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
        void MaxRich(bool ShowMax)
        {
            listView1.Visible = !ShowMax;
            if (ShowMax) { webBrowser1.Visible = true; webBrowser1.Height = 600; }
            else
            {
                webBrowser1.Visible = false; //webBrowser1.Height = 60;
                textBox2.Multiline = false;
            }
        }
        //是否显示富文本框
        void ShowMaxRich()
        {
            var c = checkedListBox1.SelectedItem; if (c == null) return;            
            if (c.ToString() == AliasType.txt.ToString()) MaxRich(true);
            else
                MaxRich(false);
        }

        //延时触发搜索
        private void timer1_Tick(object sender, EventArgs e)
        {
            string sc = textBox1.Text.Trim().ToLower();
            if (sc.Length > 1 && sc.Length < 8 )
            { Seek(sc); timer1.Stop(); }
        }
        //模糊搜索
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
