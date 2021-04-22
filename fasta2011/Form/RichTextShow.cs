using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace fasta2011
{
    [ComVisible(true)]//这句很重要
    public partial class RichForm : Form
    {
        public string _s;
        private DataBase db = new sqliteData(); public Alias _al;
        private Alias newAl = new Alias();
        public RichForm(string s, Alias Ali)
        {            
            _s = s;
            _al = Ali; //##将对象从run窗口传进来，才可以修改
            InitializeComponent();
            this.webBrowser1.Url = new System.Uri(Application.StartupPath + "\\kindeditor\\WinForm2.html", System.UriKind.Absolute);
            this.webBrowser1.ObjectForScripting = this; webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示            
        }
    
        //### 延时载入html代码内容，不然都是空白
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.webBrowser1.Document.InvokeScript("setContent", new object[] { WebUtility.HtmlDecode(_s) });
            timer1.Stop();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Url = new System.Uri(Application.StartupPath + "\\kindeditor\\WinForm.html", System.UriKind.Absolute);
            timer1.Start();
            button1.Visible = false;
            button2.Visible = true;
        }
        //## 保存修改
        private void button2_Click(object sender, EventArgs e)
        {
            //## webBrowser1取值
            string ty = "getContent";
            string body = webBrowser1.Document.InvokeScript(ty).ToString();//
            //## 更新对象
            newAl = _al; newAl.Path = body; newAl.Name = textBox1.Text.Trim();
            //## 修改_s值
            _s = body;
            db.EditItem(_al, newAl);
            //## 编辑和确定按钮转换
            button1.Visible = true;
            button2.Visible = false;
            //## 恢复到只读页面
            this.webBrowser1.Url = new System.Uri(Application.StartupPath + "\\kindeditor\\WinForm2.html", System.UriKind.Absolute);
            timer1.Start();

            RefrushOwner();
        }
        private void RichForm_Load(object sender, EventArgs e)
        {
            button2.Visible = false;
            textBox1.Text = _al.Name;
        }
        void RefrushOwner()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name == "Main")
                {
                    (f as IForm).ReLoadXml(); //(this.Owner as IForm).ReLoadXml();
                    (f as IForm).Suggest();
                }
            }
        }
    }
}