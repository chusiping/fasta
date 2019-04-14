using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace fasta2011
{
    public enum CmdType
    {
        cmd = 0,
        kill = 1,
        exe = 2,
        stock = 3,
        Dos = 4
    }
    public class ComboBoxItem
    {
        private string _text = null;
        private object _value = null;
        public string Text { get { return this._text; } set { this._text = value; } }
        public object Value { get { return this._value; } set { this._value = value; } }
        public override string ToString()
        {
            return this._text;
        }
    }
    public class Alias
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public DateTime AddTime { get; set; }
    }
    public class ExeAlias
    {
        public string text { get; set; }
        public string value { get; set; }        
        public CmdType cmdType { get; set; }
    }
}
