using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using Zone;
using System.Reflection;

namespace fasta2011
{
    public class HotKey   
    {   
        //如果函数执行成功，返回值不为0。   
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。  2015-10-10 
        [DllImport("user32.dll", SetLastError = true)]   
        public static extern bool RegisterHotKey(   
            IntPtr hWnd,                //要定义热键的窗口的句柄   
            int id,                     //定义热键ID（不能与其它ID重复）              
            KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效   
            Keys vk                     //定义热键的内容   
            );   
  
        [DllImport("user32.dll", SetLastError = true)]   
        public static extern bool UnregisterHotKey(   
            IntPtr hWnd,                //要取消热键的窗口的句柄   
            int id                      //要取消热键的ID   
            );   
  
        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）   
        [Flags()]   
        public enum KeyModifiers   
        {   
            None = 0,   
            Alt = 1,   
            Ctrl = 2,   
            Shift = 4,   
            WindowsKey = 8   
        }     
    }

    public class CommonWay
    {
        #region 添加启动项到注册表
        public static void RegAdd()
        {
            try
            {
                string FullPathFile = Application.ExecutablePath;       //获取带全路径的本程序   
                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true).SetValue("Fast-Pro", FullPathFile);//将本程序加入到注册表的RUN中  
                LogAsyncWriter.Default.Info("添加启动项到注册表：CommonWay.RegAdd()", "hotkey.cs", "");
            }
            catch (Exception)
            {

            }
        }
        #endregion

       
    }
}
