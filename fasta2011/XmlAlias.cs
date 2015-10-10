﻿using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml;
using System.IO;
using System.Collections.Generic;
namespace Zone
{

    //**********************  定义接口用来刷新父窗口  *****************************
    public interface IForm
    {
        void ReLoadXml();
        void HideForm();
    }
    public interface IForm2
    {
        void CloseNew();
    }
    


    /// <summary>
    /// XmlHelper 的摘要说明
    /// </summary>
    public class Xmlalias
    {
        private static string _XmlFilePath ="data.xml";
        private static XmlDocument xmlDoc = new XmlDocument();





        public static string XmlFilePath
        {
            set { _XmlFilePath = value; }
            get { return _XmlFilePath; }
        }


        //********************初始化创建xml文档***************************
        public static void CreateXml()
        {
            if (!File.Exists(_XmlFilePath))
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
        }
        //*********************************************************************



        //***************读取xml文件***********************
        public static void ReadXml()
        {
            xmlDoc.RemoveAll();         
            xmlDoc.Load(_XmlFilePath);
        }
        //*************************************************

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
            ReadXml();

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