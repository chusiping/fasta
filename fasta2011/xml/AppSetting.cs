using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
namespace fasta2011
{
    class AppSetting
    {
         #region 配置const常量
        public static string xmlName1 = @"data.xml";
        public static string xmlName2 = @"data_html.xml";
        public static string xmlName3 = @"data_autokey.xml";
        public static string xmlPath1 = System.Windows.Forms.Application.StartupPath + "\\" + xmlName1;
        public static string xmlPath2 = System.Windows.Forms.Application.StartupPath + "\\" + xmlName2;
        public static string xmlPath3 = System.Windows.Forms.Application.StartupPath + "\\" + xmlName3;
        public static string keyWord = "alias";
          
        public static HotKey.KeyModifiers key_Alt = HotKey.KeyModifiers.Alt;
        public static Keys key_Word = (Keys)Enum.Parse(typeof(Keys), "R");
         #endregion
    }
    public class AppConfig
    {
        public AppConfig() {}

         #region 写入App.config
        public static void ConfigSetValue(string strExecutablePath, string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            //获取可执行文件的路径和名称
            xDoc.Load(strExecutablePath + ".config");

            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");

            /*------------增加了新的处理方法检测添加appSettings node-------*/
            XmlNode xNode0;
            XmlElement xElem3;
            xNode0 = xDoc.SelectSingleNode("//configuration");
            if (xNode == null)
            {
                xNode = xDoc.CreateNode(XmlNodeType.Element, "appSettings", "");
                xNode0.AppendChild(xNode);
            }
            /*--------------------------------------------------------------*/

            // xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
                xElem1.SetAttribute("value", AppValue);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(strExecutablePath + ".config");
        }
         #endregion

         #region 读取App.config
        public static string ConfigGetValue(string strExecutablePath= "",string appKey ="")
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                strExecutablePath = strExecutablePath == "" ? "app" : strExecutablePath;
                xDoc.Load(strExecutablePath + ".config");

                XmlNode xNode;
                XmlElement xElem;
                xNode = xDoc.SelectSingleNode("//appSettings");
                xElem = (XmlElement)xNode.SelectSingleNode("//add[@key='" + appKey + "']");
                if (xElem != null)
                    return xElem.GetAttribute("value");
                else
                    return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion 
    }
}
