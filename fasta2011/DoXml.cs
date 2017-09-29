using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Zone;

namespace fasta2011
{
    public class DoXml
    {
        public static string FileName =  AppSetting.xmlPath1;
        public static void CreateExec()
        {
            if (!File.Exists(FileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(
                            "<?xml   version=\"1.0\"   encoding=\"gb2312\"?>" +
                            "<Element>" +
                                "<alias alias=\"cmd\" cmd=\"c:\\windows\\notepad.exe\" />" +
                            "</Element>");
                try
                {
                    doc.Save(FileName);
                }
                catch(Exception ex)
                {
                    Logger.Trace(ex);
                    System.Windows.Forms.Application.Exit();
                }                
            }
        }
    }
}
