using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

    public enum AliasType
    {
        http,exe,dos,txt
    }
    public class ComboBoxItem
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
        public override string ToString()
        {
            return this.Text;
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
    class ColorCodedCheckedListBox : CheckedListBox
    {
        //Color.Orange为颜色，你可修改
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            DrawItemEventArgs e2 = new DrawItemEventArgs
                (e.Graphics, 
                e.Font, 
                new Rectangle(e.Bounds.Location, e.Bounds.Size), 
                e.Index, 
                (e.State & DrawItemState.Focus) == DrawItemState.Focus ? DrawItemState.Focus : DrawItemState.None, 
                Color.Black, 
                this.BackColor
                //this.CheckedIndices.Contains(e.Index) ? Color.Red : SystemColors.Window
                );
            base.OnDrawItem(e2);
        }
    }
}
