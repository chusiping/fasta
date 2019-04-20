using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Zone;

namespace fasta2011
{
    //定义抽象类获得数据的List集合  数据可以从xml获得，从可以sqlite获得，sqlserver获得
    public abstract class DataBase
    {
        public List<Alias> AliasSet { get; set; }
        public abstract void ReadData();
        public abstract int AddItem(Alias al);
        public abstract void DelItem(Alias al);
        public abstract void EditItem(Alias al, Alias NewAl);
    }
    public class XmlData : DataBase
    {
        public override void ReadData()
        {
            AliasSet = new List<Alias>();
            ReadXml(AppSetting.xmlPath1);
            ReadXml(AppSetting.xmlPath2);
            ReadXml(AppSetting.xmlPath3);
        }
        public void ReadXml(string xmlPathX)
        {
            string s = "";
            XmlDocument doc = new XmlDocument(); doc.Load(xmlPathX);
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
        public override int AddItem(Alias al)
        {
            Xmlalias.XmlFilePath = SetXmlFilePath(al.Name, al.Path);
            string s = al.Name, s2 = al.Path;
            return Xmlalias.Add(al.Name, al.Path);
        }
        public override void DelItem(Alias al)
        {
            Xmlalias.XmlFilePath = SetXmlFilePath(al.Name, al.Path);
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
        public override void EditItem(Alias al, Alias NewAl)
        {
            string path = SetXmlFilePath(al.Name, al.Path);
            Xmlalias.Update(al.Name, al.Path, NewAl.Name, NewAl.Path, Xmlalias.XmlFilePath);
        }
    }

    public class sqliteData : DataBase
    {
        FastaContext db = FastaContext.Instance;
        public override void ReadData()
        {
            //ToDo 富文本体较大，不使用循环载入
            AliasSet = db.AliasSet.Where(m => true).ToList();

            //ToDo 富文本体较大，不使用循环载入
            string s = AliasType.txt.ToString();
            //List<Alias> a = db.AliasSet.Where(m => m.Type != s).ToList();
            //var Alias = db.Database.SqlQuery<Alias>("SELECT ID,Name,Type  FROM [Aliases] where Type = '"+ s +"'");
            //var lb = db.AlasSet.Where(m => m.Type == s).Select(s2 => new { s2.ID, s2.Name,s2.Type }).SingleOrDefault();
            //List<Alias> lb;
            //lb = (from u in db.AliasSet
            //           where u.Type == "txt").Select(n=>new Alias { ID = u.ID })).ToList();
            //AliasSet = la.Union(list.ToList()).ToList<Alias>();  

            //var va= db.AliasSet.Select(s2 => new { s2.Name,s2.ID,s2.Type }).Where(m=>m.Type == s);
            //foreach (var item in va)
            //{
            //    Alias al = new Alias { ID = item.ID, Name = item.Name, Type = item.Type };
            //    AliasSet.Add(al);
            //    Console.WriteLine(item.ID + item.Name);
            //}       
        }
        public override int AddItem(Alias al)
        {
            var aal = db.AliasSet.Where(u => u.Name == al.Name).FirstOrDefault();
            if (aal != null) return 2; //重复名称的
            db.AliasSet.Add(al);
            int rt = db.SaveChanges();
            return rt;
        }
        public override void EditItem(Alias al, Alias NewAl)
        {            
            var aal = db.AliasSet.Find(al.ID);
            aal.Name = NewAl.Name; aal.Path = NewAl.Path; aal.Type = NewAl.Type;        
            db.SaveChanges();   
        }
        public override void DelItem(Alias al)
        {
            var aal = db.AliasSet.Find(al.ID); 
            db.AliasSet.Remove(aal);
            db.SaveChanges();
        }
    }
}
