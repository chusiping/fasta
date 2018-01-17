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
using System.IO;
using System.Threading;
using System.Collections.Specialized;


namespace fasta2011
{
    
    public partial class Form_JinCheng : Form,IForm2
    {
        public static bool IsOpen = false;
        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public void CloseNew()
        {
            this.Close();
        }
        public Form_JinCheng()
        {
            InitializeComponent();

            listView1.CheckBoxes = true;//设置listView1的复选框属性为真
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.FullRowSelect = true;//是否可以选择行
            this.listView1.Columns.Add("进程ID", 80);
            this.listView1.Columns.Add("名称", 100);
            this.listView1.Columns.Add("内存", 60);
            this.listView1.Columns.Add("描述", 200);
            this.listView1.Columns.Add("路径", 800);                       
        }

        private void checkAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem tempItem in listView1.Items)//循环遍历listView控件中的每一项
            {
                if (tempItem.Checked == false)//如果当前项处于未选中状态
                {
                    tempItem.Checked = true;//设置当前项为选中状态
                }
            }
        }
        private void cleanUp_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem tempItem in listView1.Items)//循环遍历listView控件中的每一项
            {
                if (tempItem.Checked)//如果当前项处于选中状态
                {
                    tempItem.Checked = false;//设置当前项为未选中状态
                }
            }
        }
        [DllImport("Psapi.dll", EntryPoint = "GetModuleFileNameEx")]
        public static extern uint GetModuleFileNameEx(int handle, IntPtr hModule, [Out] StringBuilder lpszFileName, uint nSize);
        void ShowProcess()
        {
            Process[] processes = System.Diagnostics.Process.GetProcesses();


            for (int i = 0; i < processes.Length - 1; i++)
            {
                string Pstime = "", FilePath = "",MiaoShu = "";
                try
                {
                    Pstime = string.Format("{0}", processes[i].StartTime);
                    FilePath = processes[i].MainModule.FileName;
                    MiaoShu = processes[i].MainModule.FileVersionInfo.FileDescription;
                }
                catch 
                {
                }
                //if (processes[i].MainWindowHandle == IntPtr.Zero) continue;
                ListViewItem lvi = new ListViewItem(new string[] { 
                    processes[i].Id.ToString(),
                    processes[i].ProcessName,
                    string.Format("{0:###,##0.00} MB", processes[i].WorkingSet64 / 1024.0f / 1024.0f),
                    MiaoShu,
                    FilePath
                    //processes[i].MainModule.FileName.ToString()
                });
                listView1.Items.Add(lvi);
            }
            label1.Text = "进程总数：" + processes.Length.ToString();
        }

      


        //**********************  启动窗口  *****************************
        private void Form1_Load(object sender, EventArgs e)
        {
            this.listView1.ListViewItemSorter = new Common.ListViewColumnSorter();
            this.listView1.ColumnClick += new ColumnClickEventHandler(Common.ListViewHelper.ListView_ColumnClick);
            (listView1.ListViewItemSorter as Common.ListViewColumnSorter).SortColumn = 4;
            (listView1.ListViewItemSorter as Common.ListViewColumnSorter).Order = System.Windows.Forms.SortOrder.Ascending;
            ShowProcess();
            OnLoadCheck();
            this.TopMost = true;
        }
       

        //************  按下ESC后 退出 *************************
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool IsEnter = false;
            if(e.KeyChar == (char)13) IsEnter =true;
            if (IsEnter)
            {
                //if (button1.Visible == true)
                //{
                //}
                //else
                //{
                //}
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                if (this.listView1.Items[i].Checked)
                {                                       
                    string ins = this.listView1.Items[i].SubItems[0].Text;
                    Kill(ins);
                }
            }
            ShowProcess();
        }
        void Kill(string ExePid)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.CreateNoWindow = false; 
            if (IsInt(ExePid))
            {
                proc.StartInfo.Arguments = " /c ntsd -c q -p " + ExePid.ToString();
            }
            else
            {
                proc.StartInfo.Arguments = " /c tskill \"" + ExePid.ToString()+"\"";
            }
            proc.Start();
        }
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

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected)
                {
                    string ins = this.listView1.Items[i].SubItems[0].Text;
                    Kill(ins);
                    listView1.Items.Remove(listView1.Items[i]);
                    //break;
                }
            }
            label1.Text = "进程总数：" + listView1.Items.Count.ToString();
      
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected)
                {
                    string ins = this.listView1.Items[i].SubItems[4].Text;
                    if (!string.IsNullOrEmpty(ins))
                    {
                        string ins2 = ins.Substring(0,ins.LastIndexOf('\\'));
                        ParameterizedThreadStart ParStart = new ParameterizedThreadStart(OpenDir);
                        try
                        {
                            Thread myThread = new Thread(ParStart);
                            myThread.Start(ins2);   
                        }
                        catch 
                        {                                                   
                        }
                              
                    }                   
                }
            }
        }
        void OpenDir(object s)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = s.ToString();
            proc.StartInfo.Arguments = " ";
            proc.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //NameValueCollection nvc = new NameValueCollection();
            Dictionary<string,string> nvc = new Dictionary<string,string>();      
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                if (this.listView1.Items[i].Checked)
                {
                    string insa = this.listView1.Items[i].SubItems[1].Text;
                    string ins = this.listView1.Items[i].SubItems[4].Text;
                    if (!nvc.ContainsKey(insa)) nvc.Add(insa, ins);
                }
            }
            try
            {
                ObjectToBin.Serialize(nvc, System.Environment.CurrentDirectory + "\\jincheng.Bin");
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex )
            {
                
                throw;
            }
            
            
        }
        void OnLoadCheck()
        {
            Dictionary<string, string> nvc = null;
            nvc = ObjectToBin.DeSerialize<Dictionary<string, string>>(System.Environment.CurrentDirectory + "\\jincheng.Bin");
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {                
                if (nvc != null && nvc.Keys.Count > 0)
                {
                    foreach (var item in nvc.Keys)
                    {
                        string insa = this.listView1.Items[i].SubItems[1].Text;
                        string ins = this.listView1.Items[i].SubItems[4].Text;
                        if (item.ToString() == insa && nvc[item.ToString()].ToString() == ins)
                        {
                            this.listView1.Items[i].Checked = true;
                            this.listView1.Items[i].Selected = true ;
                        }
                    }                   
                }                                                      
            }
        }
    }
}
