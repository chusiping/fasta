using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace fasta2011
{
    class AppSetting
    {
        public static string xmlName = @"data.xml";
        public static string xmlPath = System.Windows.Forms.Application.StartupPath + "\\"  + xmlName;
        public static string keyWord = "alias";


        public static HotKey.KeyModifiers key_Alt = HotKey.KeyModifiers.Alt;
        public static Keys key_Word = (Keys)Enum.Parse(typeof(Keys), "R");
           
    }
}
