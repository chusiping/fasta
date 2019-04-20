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
        public RichForm(string s)
        {            
            _s = s;
            InitializeComponent();
            this.webBrowser1.Url = new System.Uri(Application.StartupPath + "\\kindeditor\\WinForm2.html", System.UriKind.Absolute);
            this.webBrowser1.ObjectForScripting = this; webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.webBrowser1.Document.InvokeScript("setContent", new object[] { WebUtility.HtmlDecode(_s) });
            timer1.Stop();
        }
    }
}