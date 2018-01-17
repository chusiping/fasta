using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace fasta2011
{
    public partial class DataGridViewNew : DataGridView
    {
        /// DataGridView添加行号  
        /// </summary>  
        /// <param name="dgv">DataGridView控件ID</param>  
        public void AddRowIndex(DataGridView dgv)
        {
            dgv.RowPostPaint += delegate(object sender, DataGridViewRowPostPaintEventArgs e)
            {
                SolidBrush b = new SolidBrush(dgv.RowHeadersDefaultCellStyle.ForeColor);
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), dgv.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            };
        }
        /// <summary>  
        /// DataGridView添加全选  
        /// </summary>  
        /// <param name="dgv">DataGridView控件ID</param>  
        /// <param name="columnIndex">全选所在列序号</param>  
        public void AddFullSelect(DataGridView dgv, int columnIndex)
        {
            if (dgv.Rows.Count < 1)
            {
                return;
            }
            CheckBox ckBox = new CheckBox();
            Rectangle rect = dgv.GetCellDisplayRectangle(1, -1, true);
            ckBox.Size = new Size(dgv.Columns[1].Width - 12, 12); //大小                 
            Point point = new Point(rect.X + 10, rect.Y + 3);//位置  
            ckBox.Location = point;
            ckBox.CheckedChanged += delegate(object sender, EventArgs e)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    dgv.Rows[i].Cells[columnIndex].Value = ((CheckBox)sender).Checked;
                }
                dgv.EndEdit();
            };
            dgv.Controls.Add(ckBox);
        }
    }

}
