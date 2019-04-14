using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace fasta2011
{
    public partial class test : Form
    {
        const int WM_NCPAINT = 0x85;
        const int WM_NCLBUTTONDOWN = 0xA1;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void DisableProcessWindowsGhosting();

        [DllImport("UxTheme.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        public test()
        {
            // This could be called from main.
            DisableProcessWindowsGhosting();

            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            SetWindowTheme(this.Handle, "", "");
            base.OnHandleCreated(e);
        }
        Rectangle m_rect = new Rectangle(205, 6, 20, 20);
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    {
                        //IntPtr hdc = GetWindowDC(m.HWnd);
                        //using (Graphics g = Graphics.FromHdc(hdc))
                        //{
                        //    g.FillEllipse(Brushes.Red, new Rectangle((Width - 20) / 2, 8, 20, 20));
                        //}
                        //int r = ReleaseDC(m.HWnd, hdc);
                        IntPtr hDC = GetWindowDC(m.HWnd);
                        //把DC转换为.NET的Graphics就可以很方便地使用Framework提供的绘图功能了
                        Graphics gs = Graphics.FromHdc(hDC);
                        gs.FillRectangle(new LinearGradientBrush(m_rect, Color.Pink, Color.Purple, LinearGradientMode.BackwardDiagonal), m_rect);
                        StringFormat strFmt = new StringFormat();
                        strFmt.Alignment = StringAlignment.Center;
                        strFmt.LineAlignment = StringAlignment.Center;
                        gs.DrawString("√", this.Font, Brushes.BlanchedAlmond, m_rect, strFmt);
                        gs.Dispose();
                        //释放GDI资源
                        ReleaseDC(m.HWnd, hDC);
                        break;
                    }
                case WM_NCLBUTTONDOWN:
                    {
                        Point mousePoint = new Point((int)m.LParam);
                        mousePoint.Offset(-this.Left, -this.Top);
                        if (m_rect.Contains(mousePoint))
                        {
                            MessageBox.Show("hello");

                        }
                        break;
                    }
            }
        }
    }
}