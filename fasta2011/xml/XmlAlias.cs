using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using fasta2011;
namespace Zone
{
   

    public class Xmlalias 
    {
        #region 设定xml
        private static string _XmlFilePath = "";
        private static string _XmlFilePath1 = AppSetting.xmlPath1;
        private static string _XmlFilePath2 = AppSetting.xmlPath2;
        private static string _XmlFilePath3 = AppSetting.xmlPath3;
        private static XmlDocument xmlDoc = new XmlDocument();
        #endregion 

        #region 属性字段 XmlFilePath
        public static string XmlFilePath
        {
            set { _XmlFilePath = value; }
            get { return _XmlFilePath; }
        }
        #endregion 

        #region 初始化创建三种不同的xml文档(三个方法可以合并一个)
        //***************初始化创建三种不同的xml文档***********************
        public static void CreateXml()
        {
            if (!File.Exists(_XmlFilePath1))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(
                            "<?xml   version=\"1.0\"   encoding=\"gb2312\"?>" +
                            "<Element>" +
                                "<alias alias=\"cmd\" cmd=\"c:\\windows\\notepad.exe\" />" +
                            "</Element>");
                try
                {
                    doc.Save(_XmlFilePath1);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
            if (!File.Exists(_XmlFilePath2))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(
                            "<?xml   version=\"1.0\"   encoding=\"gb2312\"?>" +
                            "<Element>" +
                                "<alias alias=\"baidu\" cmd=\"https://www.baidu.com\" />" +
                            "</Element>");
                try
                {
                    doc.Save(_XmlFilePath2);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
            if (!File.Exists(_XmlFilePath3))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(
                            "<?xml   version=\"1.0\"   encoding=\"gb2312\"?>" +
                            "<Element>" +
                                "<alias alias=\"/auto\" cmd=\"\" />" +
                            "</Element>");
                try
                {
                    doc.Save(_XmlFilePath3);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
        }
        #endregion

        #region 读取xml文件
        public static void ReadXml(string path)
        {
            xmlDoc.RemoveAll();
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("参数为空！");
                return;
            }
            xmlDoc.Load(path);
        }
        //public static void ReadXml(int WhichXmlPath)
        //{
        //    xmlDoc.RemoveAll();
        //    _XmlFilePath = WhichXmlPath == 1 ? _XmlFilePath1 : _XmlFilePath2;
        //    xmlDoc.Load(_XmlFilePath);
        //}
        #endregion

        #region 写入xml文件 
        public static void SaveXml(string path)
        {
            xmlDoc.Save(path);
            Zone.DebugNew.WriteLog("保存xml成功---" + _XmlFilePath);
        }
        #endregion 

        #region 添加xml节点
        public static int Add(string s1, string s2)
        {
            int blint = -1; // -1 失败
            ReadXml(Xmlalias._XmlFilePath);
            XmlNode root = xmlDoc.SelectSingleNode("Element");
            XmlNodeList xnl = xmlDoc.SelectSingleNode("Element").ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("alias") == s1)
                {
                    blint = 2;             // 1 有重复,则返回
                    break;
                }
            }
            if (blint == -1)                 // 如果没有重复,则写入xml
            {
                XmlElement xe1 = xmlDoc.CreateElement("alias");
                xe1.SetAttribute("alias", s1);
                xe1.SetAttribute("cmd", s2);
                root.AppendChild(xe1);
                SaveXml(Xmlalias.XmlFilePath);
                blint = 1;  // 0 添加成功
            }
            return blint;
        }
        #endregion 

        #region 删除xml节点
        public static void Del(string s1, string s2,string path)
        {
            ReadXml(path);
            XmlNodeList xnl = xmlDoc.SelectSingleNode("Element").ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("alias") == s1 && xe.GetAttribute("cmd") == s2)
                {
                    xe.ParentNode.RemoveChild(xe);
                    break;
                }
            }
            SaveXml(path);
        }
        #endregion

        #region 更新xml节点
        public static void Update(string Old_ss1,string old_ss2,string new_s1, string new_s2,string xml_path)
        {
            ReadXml(xml_path);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("Element").ChildNodes;
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("alias") == Old_ss1 && xe.GetAttribute("cmd") == old_ss2)
                {
                    xe.SetAttribute("alias", new_s1);
                    xe.SetAttribute("cmd", new_s2);
                    break;
                }
            }
            SaveXml(xml_path);
        }
        #endregion 
    }

   
}
//2019-3-26 修改逻辑
//0  添加Model，使用接口获得数据的List集合  数据可以从xml获得，从可以sqlite获得，sqlserver获得
//1  将数据集合赋值给combobox  ,赋值给 auto acs
//2  使用通用model，使用接口，添加一条数据，修改一条数据，删除一条数据
//3  继承接口，使用 add()添加到sqlite 的db中，添加时，允许关键字重复
//4  添加帮助窗口 ， 指示操作规范



// https://blog.csdn.net/zxsean/article/details/52045950  listviewitem 加按钮
// https://books.google.com.sg/books?id=cvhIDwAAQBAJ&pg=PT301&lpg=PT301&dq=ListViewItem+%E6%B7%BB%E5%8A%A0%E5%9B%BE%E6%A0%87&source=bl&ots=EPmK_RMIDa&sig=ACfU3U3R1rMTwU2qH0R0bR1fGzuU48ObvQ&hl=zh-CN&sa=X&ved=2ahUKEwi4uLS_0qLhAhUTg-YKHWwnCfsQ6AEwBnoECAkQAQ#v=onepage&q&f=false