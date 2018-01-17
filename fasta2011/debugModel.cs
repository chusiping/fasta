//Edit Jarry 2011-05-16 Debug类的调用
//Edit Jarry 2012-01-29 
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms; //引入Debug类所在命名空间

namespace Zone
{
    public class DebugNew
    {
        public static void WriteLog(string Path, string DebugMsg)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Path, DebugMsg));
            //Debug.Listeners.Add(new TextWriterTraceListener("c:\\debug.log", "testdebug"));
            //将Debug类输出定向到控制台输出
            Debug.AutoFlush = true;
            //设置Debug为自动输出，即每次写入后都调用Listeners上调用Flush
            Debug.Indent();
            //设置缩进
            Debug.WriteLine(DebugMsg);
            //用Debug输出"Debug WriteLine()"
            //Console.WriteLine("Console.WriteLine()");
            //用Console输出"Console.WriteLine()"
            //用Debug输出"Debug WriteLine2()"
            Debug.Unindent();
            //取消缩进
            //Console.Read();
        }
        public static void WriteConSole(string DebugMsg)
        {
            Debug.Listeners.Add(new TextWriterTraceListener("c:\\debug.log", DebugMsg));
            //Debug.Listeners.Add(new TextWriterTraceListener("c:\\debug.log", "testdebug"));
            //将Debug类输出定向到控制台输出
            Debug.AutoFlush = true;
            //设置Debug为自动输出，即每次写入后都调用Listeners上调用Flush
            Debug.Indent();
            //设置缩进
            Debug.WriteLine(DebugMsg);
            //用Debug输出"Debug WriteLine()"
            Console.WriteLine("Console.WriteLine()");
            //用Console输出"Console.WriteLine()"
            //用Debug输出"Debug WriteLine2()"
            Debug.Unindent();
            //取消缩进
            Console.Read();
        }
        public static void WriteLog(string DebugMsg)
        {
            string Path = Application.UserAppDataPath+"\\3.log";
            WriteLog(Path, DebugMsg);
        }
    }
}