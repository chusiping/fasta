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


namespace fasta2011
{
    public partial class Form1 : Form,IForm2
    {
         #region 初始化Form1和ListView
        public static bool IsOpen = false;
        private int ListItemIndex = -1;
        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        IProcessData processData;
        public Form1()
        {
            InitializeComponent();
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
            int w1 = AppConfig.ConfigGetValue("ListView1_c1_Width") == "" ? 250 : int.Parse(AppConfig.ConfigGetValue("ListView1_c1_Width"));
            int w2 = AppConfig.ConfigGetValue("ListView1_c2_Width") == "" ? 900 : int.Parse(AppConfig.ConfigGetValue("ListView1_c2_Width"));
            this.listView1.Columns.Add("别名", w1);
            this.listView1.Columns.Add("路径", w2);
            
        }
        #endregion 

         #region 增加添加按钮 button1_Click
        private void button1_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text.Trim();
            string s2 = textBox2.Text.Trim();
            if (s1 == "") return;   //if (s1 == "" || s2 == "") return;
            SetXmlFilePath(s1, s2);          //if (IsContainHttp(s2)) { xls.GetXml2(); } else { xls.GetXml1(); }
            var alias = new Alias { Name = s1, Path = s2, Type = "", AddTime = DateTime.Now };
            processData = new FastaDB(alias);
            int i = processData.Add();
            //if (i == 1) MessageBox.Show("此别名已存在！");
            if (i == 0) MessageBox.Show("添加失败！");
            LoadListView();
            textBox1.Clear(); textBox2.Clear(); // 添加完后清空输入框
            RefrushOwner();
        }
        #endregion 
      
         #region 根据aliase得到xml名称 data|data_html|data_autokey
        void SetXmlFilePath(string s1,string s2)
        {
            string fuhao = AppConfig.ConfigGetValue("app", "autokey_symbol");  //从配置App.config里取
            if (fuhao.Contains(s1.Substring(0, 1)))
            {
                Xmlalias.XmlFilePath = AppSetting.xmlName3;
                return;
            }
            if (s2.IndexOf("://") >= 0)
                Xmlalias.XmlFilePath = AppSetting.xmlName2;
            else
                Xmlalias.XmlFilePath = AppSetting.xmlName1;
        }
        #endregion 

         #region 启动窗口 Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {            
            LoadListView();
            textBox1.Focus();
            this.listView1.ListViewItemSorter = new Common.ListViewColumnSorter();
            this.listView1.ColumnClick += new ColumnClickEventHandler(Common.ListViewHelper.ListView_ColumnClick);
            this.Text = ((AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute))).Title;
        }
         #endregion 

         #region 点击修改按钮
        private void button2_Click(object sender, EventArgs e)
        {
            ListViewItem p = new ListViewItem();
            p = listView1.SelectedItems[0];

            string ss1 = p.SubItems[0].Text;
            string ss2 = p.SubItems[1].Text;

            string s1 = textBox1.Text.Trim();
            string s2 = textBox2.Text.Trim();

            if (textBox2.Visible == false)
            {
                s2 = "";
                foreach (var item in this.Controls)
                {
                    if (item is ComboBox)
                    {
                        ComboBox cb = (ComboBox)item;
                        cb.Visible = false;
                        textBox2.Visible = true;
                        bool IsSeleNone = (cbmSeleIndex == -1);
                        for (int i = 0; i < cb.Items.Count; i++)
                        {
                            ComboBoxItem cbi = new ComboBoxItem();
                            cbi = (ComboBoxItem)cb.Items[i];
                            if (cbmSeleIndex == i)
                            {
                                cbi.Text = cb.Text;
                                if (cbi.Text == "") continue;
                            }
                            bool IsDele = (cbmSeleIndex != -1) && (i == cb.Items.Count - 1);
                            s2 += cbi.Text.Trim() + (i == cb.Items.Count - 1 ? "" : ";");
                        }
                        if (IsSeleNone)
                        {
                            s2 += cb.Text.Trim() == "" ? "" : ";" + cb.Text;
                        }
                    }
                }
            }
            SetXmlFilePath(s1, s2);
            Xmlalias.Update(ss1, ss2, s1, s2, Xmlalias.XmlFilePath);


            //textBox1.Text = "";
            //textBox2.Text = "";


            LoadListView();
            //SendMessage(listView1.Handle, 500, 200, 200);
            button1.Visible = true;
            button2.Visible = false;
            RefrushOwner();
        }
        #endregion 
   
         #region 重新刷新 增删改列表
        void LoadListView()
        {
            LoadListView(0);
            LoadListView2(0);
            LoadListView3(0);
        }
        #endregion 

         #region 读取data.xml绑定到Listview
        void LoadListView(int i)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(System.Windows.Forms.Application.StartupPath + "\\" + "data.xml");
            xmlDoc.Load(AppSetting.xmlName1);
            XmlNode xn = xmlDoc.SelectSingleNode("Element");
            XmlNodeList xnl = xn.ChildNodes;
            ListViewItem p = new ListViewItem();
            listView1.Items.Clear();

            
            imageList1.ImageSize = new Size(15,15);
            imageList1.Images.Add(Image.FromFile("1.jpg"));
            imageList1.Images.Add(Image.FromFile("2.jpg"));
            listView1.SmallImageList = imageList1;

            foreach (XmlNode xnf in xnl)
            {
                XmlElement xe = (XmlElement)xnf;
                p = new ListViewItem(new string[] { xe.GetAttribute("alias"), xe.GetAttribute("cmd") });
                p.ImageIndex = 0;
                this.listView1.Items.Add(p);
            }
            try
            {
                listView1.Items[i].Selected = true;
                listView1.Items[i].EnsureVisible();
            }
            catch 
            {
                try
                {
                    i = i - 1;
                    listView1.Items[i].Selected = true;
                    listView1.Items[i].EnsureVisible();

                }
                catch 
                {
                           
                    
                }
            }
        }
        #endregion

         #region  读取data_html.xml绑定到Listview
        void LoadListView2(int i)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppSetting.xmlName2);
            XmlNode xn = xmlDoc.SelectSingleNode("Element");
            XmlNodeList xnl = xn.ChildNodes;
            ListViewItem p = new ListViewItem();            
            foreach (XmlNode xnf in xnl)
            {
                XmlElement xe = (XmlElement)xnf;
                p = new ListViewItem(new string[] { xe.GetAttribute("alias"), xe.GetAttribute("cmd") });
                this.listView1.Items.Add(p);
            }
            try
            {
                listView1.Items[i].Selected = true;
                listView1.Items[i].EnsureVisible();
            }
            catch
            {
                try
                {
                    i = i - 1;
                    listView1.Items[i].Selected = true;
                    listView1.Items[i].EnsureVisible();

                }
                catch
                {


                }
            }
        }
         #endregion

         #region  读取data_autokey.xml绑定到Listview
        void LoadListView3(int i)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppSetting.xmlName3);
            XmlNode xn = xmlDoc.SelectSingleNode("Element");
            XmlNodeList xnl = xn.ChildNodes;
            ListViewItem p = new ListViewItem();
            foreach (XmlNode xnf in xnl)
            {
                XmlElement xe = (XmlElement)xnf;
                p = new ListViewItem(new string[] { xe.GetAttribute("alias"), xe.GetAttribute("cmd") });
                this.listView1.Items.Add(p);
            }
            try
            {
                listView1.Items[i].Selected = true;
                listView1.Items[i].EnsureVisible();
            }
            catch
            {
                try
                {
                    i = i - 1;
                    listView1.Items[i].Selected = true;
                    listView1.Items[i].EnsureVisible();

                }
                catch
                {


                }
            }
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
            foreach (var item in this.Controls)
            {
                if (item is ComboBox)
                {
                    this.Controls.Remove((ComboBox)item);
                }
            }
            string[] arr = p.SubItems[1].Text.Split(';');
            if (arr.Length > 1)
            {
                ComboBox cb = new ComboBox();
                cb.Location = new Point(textBox2.Location.X, textBox2.Location.Y);
                cb.Width = 445;                
                cb.DropDownWidth = 440;
                cb.DropDownClosed += new EventHandler(newComboxDropDown);
               
                for (int i = 0; i < arr.Length; i++)
                {
                    ComboBoxItem cm = new ComboBoxItem();
                    cm.Text = arr[i].ToString();
                    if (cm.Text.Trim() != "") cb.Items.Add(cm);                    
                }
                textBox2.Visible = false;
                cbmSeleIndex = -1;
                this.Controls.Add(cb);
            }
            else
            {
                textBox2.Visible = true;
                textBox2.Text = p.SubItems[1].Text;
            }
            button1.Visible = false;
            button2.Visible = true;
            textBox1.Text = p.SubItems[0].Text;
            RefrushOwner();                       
        }
        #endregion 

         #region 点击删除
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ListViewItem p = new ListViewItem();
            p = listView1.Items[ListItemIndex];

            string s1 = p.SubItems[0].Text;
            string s2 = p.SubItems[1].Text;

            SetXmlFilePath(s1, s2);
            Xmlalias.Del(s1, s2,Xmlalias.XmlFilePath);
            LoadListView();
            RefrushOwner();
            //SendMessage(listView1.Handle, 500, 200, 200);
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
        //************  按下ESC后 退出 *************************
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                RefrushOwner();
                this.Close();
            }
        }
        #endregion 

         #region 回车确认删除修改
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (button1.Visible == true)
                {
                    button1_Click(null, null);  //Add();
                }
                else
                {
                    button2_Click(null, null);  //update();
                }
            }
        }
        #endregion 

         #region 双击选中进行修改
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            listView1_Click(sender, e);
            toolStripMenuItem1_Click(sender, e);
        }
        #endregion 

         #region 别名快速搜索
        int s = 0;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {          
            if (button1.Visible == true)
            {
                string sc = textBox1.Text.Trim().ToLower();
                if ( (sc == ""|| sc.Length < 2 ))
                {
                    if (sc.Length < 2) s = int.Parse(DateTime.Now.ToString("HHssmmfff"));
                    return;
                }
                if (s != 0)
                {
                    int dt2 = int.Parse(DateTime.Now.ToString("HHssmmfff"));
                    int i2 = (dt2 - s);
                    s = int.Parse(DateTime.Now.ToString("HHssmmfff"));
                }
                
                foreach (var item in listView1.Items)
                {
                    ListViewItem lv = (ListViewItem)item;
                    if (lv.Text.Contains(sc)|| lv.SubItems[1].Text.Contains(sc))
                    {
                        //SetXmlFilePath(lv.Text, lv.SubItems[1].Text);  //等待改进
                        //int w= AppConfig.ConfigGetValue("ListView1_c2_max") == "" ? 250 : int.Parse(AppConfig.ConfigGetValue("ListView1_c2_max"));
                        //if(Xmlalias.XmlFilePath == AppSetting.xmlName3) this.listView1.Columns[0].Width = w; // 如果是对autohotkey的字段搜索，则拓宽
                        lv.ForeColor = Color.Red;                        
                        listView1.TopItem = listView1.Items[lv.Index];
                    }
                    else
                    {
                        listView1.Items.Remove(lv);
                    }                    
                }
            }            
        }
        #endregion 

         #region 回车重新载入
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                LoadListView();
            }
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
    } 
}
