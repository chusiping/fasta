﻿using System.Data.Entity;
using System.Data.Common;
using System.Data.SQLite.EF6;
using SQLite.CodeFirst;
using System;
using System.ComponentModel.DataAnnotations;

namespace fasta2011
{

    public class FastaContext : DbContext
    {
        static string dbPath = $"Data Source={System.Environment.CurrentDirectory}\\fasta.db";
        public static FastaContext Instance
        {
            get
            {
                DbConnection sqliteCon = SQLiteProviderFactory.Instance.CreateConnection();
                sqliteCon.ConnectionString = dbPath;
                return new FastaContext(sqliteCon);
            }
        }
        private FastaContext(DbConnection con) : base(con, true) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<FastaContext>(modelBuilder));//如果不存在数据库，则创建
        }
        public DbSet<Alias> AliasSet { get; set; }
    }
   
}

