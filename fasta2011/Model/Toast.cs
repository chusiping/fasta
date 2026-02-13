using System;
using System.Drawing;
using System.Windows.Forms;

namespace fasta2011.Model
{
    #region 显示框
    public class Toast : Form
    {
        private Timer lifeTimer = new Timer();
        private Timer fadeTimer = new Timer();

        private Control ownerControl;

        public Toast(Control owner, string message, int duration = 2000)
        {
            ownerControl = owner;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.Opacity = 0.9;

            Label lbl = new Label();
            lbl.ForeColor = Color.White;
            lbl.BackColor = Color.Transparent;
            lbl.AutoSize = true;
            lbl.Padding = new Padding(15);
            lbl.MaximumSize = new Size(400, 0);
            lbl.Text = message;

            this.Controls.Add(lbl);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            lifeTimer.Interval = duration;
            lifeTimer.Tick += (s, e) =>
            {
                lifeTimer.Stop();
                fadeTimer.Start();
            };

            fadeTimer.Interval = 30;
            fadeTimer.Tick += (s, e) =>
            {
                this.Opacity -= 0.08;
                if (this.Opacity <= 0)
                {
                    fadeTimer.Stop();
                    this.Close();
                }
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // 🔥 用控件计算屏幕坐标
            if (ownerControl != null)
            {
                Rectangle rect = ownerControl.RectangleToScreen(ownerControl.ClientRectangle);

                int x = rect.Left + (rect.Width - this.Width) / 2;
                int y = rect.Top - this.Height - 10;

                this.Location = new Point(x, y);
            }

            lifeTimer.Start();
        }
    }
    #endregion

    #region 调用
    public static class ToastTool
    {
        public static void Show(Control owner, string message, int duration = 2000)
        {
            if (owner.InvokeRequired)
            {
                owner.Invoke(new MethodInvoker(() =>
                {
                    new Toast(owner, message, duration).Show();
                }));
            }
            else
            {
                new Toast(owner, message, duration).Show();
            }
        }
    }
    #endregion
}