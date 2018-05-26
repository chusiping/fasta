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
    #region 定义接口用来刷新父窗口
    public interface IForm
    {
        void ReLoadXml();
        void HideForm();
    }
    public interface IForm2
    {
        void CloseNew();
    }
    #endregion 

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
            ReadXml(Xmlalias.XmlFilePath);
            XmlNode root = xmlDoc.SelectSingleNode("Element");
            XmlNodeList xnl = xmlDoc.SelectSingleNode("Element").ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("alias") == s1)
                {
                    blint = 1;             // 1 有重复,则返回
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
                blint = 0;  // 0 添加成功
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