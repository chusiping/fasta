using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace fasta2011
{
    class AppSetting
    {
        public static string xmlName1 = @"data.xml";
        public static string xmlName2 = @"data_html.xml";
        public static string xmlPath1 = System.Windows.Forms.Application.StartupPath + "\\" + xmlName1;
        public static string xmlPath2 = System.Windows.Forms.Application.StartupPath + "\\" + xmlName2;
        public static string keyWord = "alias";


        public static HotKey.KeyModifiers key_Alt = HotKey.KeyModifiers.Alt;
        public static Keys key_Word = (Keys)Enum.Parse(typeof(Keys), "R");
           
    }
}
