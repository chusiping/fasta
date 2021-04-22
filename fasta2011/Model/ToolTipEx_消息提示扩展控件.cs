using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

namespace fasta2011
{
    public static class TooltipToolV2
    {
        /// <summary>
        /// 为控件提供Tooltip
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="tip">ToolTip</param>
        /// <param name="message">提示消息</param>
        public static void ShowTooltip(this Control control, ToolTip tip, string message)
        {
            Point _mousePoint = Control.MousePosition;
            int _x = control.PointToClient(_mousePoint).X + message.Length;
            int _y = control.PointToClient(_mousePoint).Y - 300;
            tip.Show(message, control, _x, _y);
            tip.Active = true;
        }
        /// <summary>
        /// 为控件提供Tooltip
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="tip">ToolTip</param>
        /// <param name="message">提示消息</param>
        /// <param name="durationTime">保持提示的持续时间</param>
        public static void ShowTooltip(this Control control, ToolTip tip, string message, int durationTime)
        {
            Point _mousePoint = control.Location;
            int _x = _mousePoint.X + 100 ;
            int _y = _mousePoint.Y - 100;
            //int _x = control.PointToClient(_mousePoint).X;
            //int _y = control.PointToClient(_mousePoint).Y;
            tip.Show(message, control, _x , _y, durationTime);            
            tip.Active = true;
        }
        /// <summary>
        /// 为控件提供Tooltip
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="tip">ToolTip</param>
        /// <param name="message">提示消息</param>
        /// <param name="xoffset">水平偏移量</param>
        /// <param name="yoffset">垂直偏移量</param>
        public static void ShowTooltip(this Control control, ToolTip tip, string message, int xoffset, int yoffset)
        {
            Point _mousePoint = Control.MousePosition;
            int _x = control.PointToClient(_mousePoint).X;
            int _y = control.PointToClient(_mousePoint).Y;
            tip.Show(message, control, _x + xoffset, _y + yoffset);
            tip.Active = true;
        }
        /// <summary>
        /// 为控件提供Tooltip
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="tip">ToolTip</param>
        /// <param name="message">提示消息</param>
        /// <param name="xoffset">水平偏移量</param>
        /// <param name="yoffset">垂直偏移量</param>
        /// <param name="durationTime">保持提示的持续时间</param>
        public static void ShowTooltip(this Control control, ToolTip tip, string message, int xoffset, int yoffset, int durationTime)
        {
            Point _mousePoint = Control.MousePosition;
            int _x = control.PointToClient(_mousePoint).X;
            int _y = control.PointToClient(_mousePoint).Y;
            tip.Show(message, control, _x + xoffset, _y + yoffset, durationTime);
            tip.Active = true;
        }
    }

}