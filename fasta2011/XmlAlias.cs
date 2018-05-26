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
    //**********************  定义接口用来刷新父窗口  ****************************
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
    /// <summary>
    /// XmlHelper 的摘要说明
    /// </summary>
    public class Xmlalias
    {
        #region 设定xml
        private static string _XmlFilePath = "";
        private static string _XmlFilePath1 = AppSetting.xmlPath1;
        private static string _XmlFilePath2 = AppSetting.xmlPath2;
        private static string _XmlFilePath3 = AppSetting.xmlPath3;
        private static XmlDocument xmlDoc = new XmlDocument();
        public void GetXml1()
        {
            _XmlFilePath = _XmlFilePath1;
        }
        public void GetXml2()
        {
            _XmlFilePath = _XmlFilePath2;
        }
        public void GetXml3()
        {
            _XmlFilePath = _XmlFilePath3;
        }
        public static string XmlFilePath
        {
            set { _XmlFilePath = value; }
            get { return _XmlFilePath; }
        }
         #endregion

        #region 初始化创建三种不同的xml文档
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
                    doc.Save(_XmlFilePath);
                }
                catch (Exception ex)
                {
                    //Logger.Trace(ex);
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
                    //Logger.Trace(ex);
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
                    //Logger.Trace(ex);
                    System.Windows.Forms.Application.Exit();
                }
            }
        }
        #endregion

        #region 读取xml文件
        //***************读取xml文件***********************
        public static void ReadXml()
        {
            xmlDoc.RemoveAll();
            if (string.IsNullOrEmpty(_XmlFilePath))
            {
                MessageBox.Show("参数为空！");
                return;
            }
            xmlDoc.Load(_XmlFilePath);
        }
        public static void ReadXml(int WhichXmlPath)
        {
            xmlDoc.RemoveAll();
            _XmlFilePath = WhichXmlPath == 1 ? _XmlFilePath1 : _XmlFilePath2;
            xmlDoc.Load(_XmlFilePath);
        }
        #endregion

        //***************保存xml文件***********************
        public static void SaveXml()
        {            
            xmlDoc.Save(_XmlFilePath);
            Zone.DebugNew.WriteLog("保存xml成功---" + _XmlFilePath);
        }
        //*************************************************


        //***************添加节点***********************
        public static bool Add(string s1, string s2)
        {
            bool bl = true;
            ReadXml();
            XmlNode root = xmlDoc.SelectSingleNode("Element");

            XmlNodeList xnl = xmlDoc.SelectSingleNode("Element").ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("alias") == s1)
                {
                    bl = false;
                    break;
                }
            }

            if (bl == true)
            {
                XmlElement xe1 = xmlDoc.CreateElement("alias");
                xe1.SetAttribute("alias", s1);
                xe1.SetAttribute("cmd", s2);
                root.AppendChild(xe1);
                SaveXml();
            }

            return bl;

        }
        public int Add2(string s1, string s2)
        {
            int bl = -1;
            ReadXml();
            XmlNode root = xmlDoc.SelectSingleNode("Element");

            XmlNodeList xnl = xmlDoc.SelectSingleNode("Element").ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute("alias") == s1)
                {
                    bl = 1;
                    break;
                }
            }

            if (bl == -1)
            {
                XmlElement xe1 = xmlDoc.CreateElement("alias");
                xe1.SetAttribute("alias", s1);
                xe1.SetAttribute("cmd", s2);
                root.AppendChild(xe1);
                SaveXml();
                Zone.DebugNew.WriteLog("alias：" + s1);
                Zone.DebugNew.WriteLog("root.AppendChild(xe1) ok!");
                Zone.DebugNew.WriteLog("SaveXml() ok!");

                bl = 0;
            }

            return bl;

        }


        //***************删除节点***********************
        public static void Del(string s1, string s2)
        {
            ReadXml();

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
            SaveXml();
        }

        //***************更新节点***********************
        public static void Update(string Old_ss1,string old_ss2,string new_s1, string new_s2)
        {
            //ReadXml();
            if (new_s2.IndexOf("://") >= 0) { ReadXml(2); } else { ReadXml(1); }
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
            SaveXml();
        }














    }
}