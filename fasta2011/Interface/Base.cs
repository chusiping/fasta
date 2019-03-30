using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Zone;

namespace fasta2011
{
    //定义抽象类获得数据的List集合  数据可以从xml获得，从可以sqlite获得，sqlserver获得
    public abstract class DataBase
    {
        public List<Alias> AliasSet {get;set;}
        public ComboBox cb { get; set; }
        public abstract void ReadData();
        public abstract void SetComboBox();
        public abstract int AddItem(Alias al);
        public abstract void  DelItem(Alias al);
        public abstract void EditItem(Alias al,Alias NewAl);
    }
    public class XmlData : DataBase
    {
        public override void ReadData()
        {
            AliasSet.Clear();
            ReadXml(AppSetting.xmlPath1);
            ReadXml(AppSetting.xmlPath2);
            ReadXml(AppSetting.xmlPath3);
        }
        public void ReadXml(string xmlPathX)
        {
            string s = "";
            XmlDocument doc = new XmlDocument();doc.Load(xmlPathX);
            XmlNodeReader reader = new XmlNodeReader(doc);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    s = reader.Name;
                    if (s.Equals(AppSetting.keyWord))
                            AliasSet.Add(new Alias { Name = reader.GetAttribute(0), Path = reader.GetAttribute(1) });
                    break;
                }
            }
        }
        public override void SetComboBox()
        {
            cb.Items.Clear();
            foreach (var x in AliasSet)
            {
                string s = x.Name, s2 = x.Path;
                cb.Items.Add(new ComboBoxItem { Text = x.Name, Value = x.Path });
            }            
        }
        public override int AddItem(Alias al)
        {
            return Xmlalias.Add(al.Name, al.Path);
        }
        public override void DelItem(Alias al)
        {
            string path = SetXmlFilePath(al.Name, al.Path);
            Xmlalias.Del(al.Name, al.Path, "");
        }
        string SetXmlFilePath(string s1, string s2)
        {
            string fuhao = AppConfig.ConfigGetValue("app", "autokey_symbol");  //从配置App.config里取
            if (fuhao.Contains(s1.Substring(0, 1)))
            {
                Xmlalias.XmlFilePath = AppSetting.xmlName3;
                return Xmlalias.XmlFilePath;
            }
            if (s2.IndexOf("://") >= 0)
                Xmlalias.XmlFilePath = AppSetting.xmlName2;
            else
                Xmlalias.XmlFilePath = AppSetting.xmlName1;
            return Xmlalias.XmlFilePath;
        }
        public override  void EditItem(Alias al, Alias NewAl)
        {
            string path = SetXmlFilePath(al.Name, al.Path);
            Xmlalias.Update(al.Name, al.Path, NewAl.Name, NewAl.Path, Xmlalias.XmlFilePath);
        }
    }
}
