using System.Data.Entity;
using System.Data.Common;
using System.Data.SQLite.EF6;
using SQLite.CodeFirst;
using System;
using System.ComponentModel.DataAnnotations;

namespace Zone
{
    public class FastaDB : IProcessData
    {
        FastaContext db = FastaContext.Instance;
        Alias _a;
        public FastaDB(Alias a)
        {
            this._a = a;
        }
        public int Add()
        {            
            db.AliasSet.Add(_a);
            return db.SaveChanges();
        }
        public int Edit()
        {
            var a = db.AliasSet.Find(_a);            
            return db.SaveChanges();
        }
        public int Del()
        {
            var a = db.AliasSet.Find(_a);
            db.AliasSet.Remove(a);
            return db.SaveChanges();
        }
    }
    public interface IProcessData
    {
        int Add();
        int Edit();
        int Del();
    }
}

