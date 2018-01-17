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


namespace fasta2011
{
    public partial class Form1 : Form,IForm2
    {
        public static bool IsOpen = false;
        private int ListItemIndex = -1;
        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

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
            this.listView1.Columns.Add("别名",120);
            this.listView1.Columns.Add("路径",450);
            
        }


        //***************  增加按钮 事件  ***********************
        private void button1_Click(object sender, EventArgs e)
        {
            Add();
        }


        //***************  增加的具体方法  ***********************
        void Add()
        {
            string s1 = textBox1.Text.Trim();
            string s2 = textBox2.Text.Trim();
            if (s1 == "" || s2 == "") return;
            Xmlalias xls = new Xmlalias();
            if (IsContainHttp(s2)) { xls.GetXml2(); } else { xls.GetXml1(); }
            int i = xls.Add2(s1, s2);
            if (i == 1) MessageBox.Show("此别名已存在！");
            if (i == -1) MessageBox.Show("添加失败！");
            LoadListView(ListItemIndex);
            RefrushOwner();
        }
        bool IsContainHttp(string text)
        {
            if (text.IndexOf("://") >= 0) return true;            
            return false;
        }

        //**********************  启动窗口  *****************************
        private void Form1_Load(object sender, EventArgs e)
        {            
            LoadListView();
            textBox1.Focus();
            this.listView1.ListViewItemSorter = new Common.ListViewColumnSorter();
            this.listView1.ColumnClick += new ColumnClickEventHandler(Common.ListViewHelper.ListView_ColumnClick);
        }


        //**********************  修改 事件  *****************************
        private void button2_Click(object sender, EventArgs e)
        {
            update();
        }

        //***************  修改的具体方法  ***********************

        void update()
        {
            ListViewItem p = new ListViewItem();
            p = listView1.SelectedItems[0];

            string ss1 = p.SubItems[0].Text;
            string ss2 = p.SubItems[1].Text;

            string s1 = textBox1.Text.Trim();
            string s2 = textBox2.Text.Trim();

            if (textBox2.Visible == false)
            {
                s2 ="";
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
                            s2 += cbi.Text.Trim() + (i == cb.Items.Count-1 ? "" : ";");
                        }
                        if (IsSeleNone)
                        {
                            s2 += cb.Text.Trim() == "" ? "" : ";" + cb.Text;
                        }                    
                    }
                }
            }


            Xmlalias.Update(ss1, ss2, s1, s2);


            textBox1.Text = "";
            textBox2.Text = "";


            LoadListView(ListItemIndex);
            //SendMessage(listView1.Handle, 500, 200, 200);
            button1.Visible = true;
            button2.Visible = false;
            RefrushOwner();
        }

        void LoadListView()
        {
            LoadListView(0);
            LoadListView2(0);
        }

        //************  给listview 绑定数据 *************************
        void LoadListView(int i)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(System.Windows.Forms.Application.StartupPath + "\\" + "data.xml");


            XmlNode xn = xmlDoc.SelectSingleNode("Element");

            XmlNodeList xnl = xn.ChildNodes;

            ListViewItem p = new ListViewItem();
            listView1.Items.Clear();
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
        //************** edit by jarry 2016-1-23 增加保存html的的xml  ************************
        void LoadListView2(int i)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(System.Windows.Forms.Application.StartupPath + "\\" + "data_html.xml");


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

        //***************  修改按钮事件  ********************************************
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
        //***************  删除按钮事件  ********************************************
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ListViewItem p = new ListViewItem();
            p = listView1.Items[ListItemIndex];

            string s1 = p.SubItems[0].Text;
            string s2 = p.SubItems[1].Text;
            

            Xmlalias.Del(s1, s2);
            LoadListView(ListItemIndex);
            RefrushOwner();
            //SendMessage(listView1.Handle, 500, 200, 200);
        }

        //***************  listView1选中行，得到index号  *****************
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

        public void CloseNew()
        {
            this.Close();
        }


        //************  按下ESC后 退出 *************************
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                RefrushOwner();
                this.Close();
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool IsEnter = false;
            if(e.KeyChar == (char)13) IsEnter =true;
            if (IsEnter)
            {
                if (button1.Visible == true)
                {
                    Add();
                }
                else
                {
                    update();
                }
            }
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            listView1_Click(sender, e);
            toolStripMenuItem1_Click(sender, e);
        }

        //************  别名快速搜索  *************************
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

//                     if ((i2) < 1000)
//                     {                        
//                         return; 
//                     }
                }
                
                foreach (var item in listView1.Items)
                {
                    ListViewItem lv = (ListViewItem)item;
                    if (lv.Text.Contains(sc)|| lv.SubItems[1].Text.Contains(sc))
                    {
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                LoadListView();
            }
        }
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
    }
}
