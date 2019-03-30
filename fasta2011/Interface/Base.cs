using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace fasta2011
{
   public class DataBase
   {
        public List<Alias> AliasSet {get;set;}
        public virtual void ReadData()
        {

        }
   }
}
