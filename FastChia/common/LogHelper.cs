using FastChia.config;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FastChia.common
{
    public class LogHelper
    {

        public static void ShowLog(string format, object arg0)
        {
            string str = String.Format(format, arg0);
            if (BaseConfig.DEBUG)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff") + "  " + str);
            }
            WriteLog(str);
        }

        public static void ShowLog(string format, params object[] arg)
        {
            string str = String.Format(format, arg);
            if (BaseConfig.DEBUG)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff") + "  " + str);
            }
            WriteLog(str);
        }

        public static void WriteLog(string msg)
        {
            string path = string.Empty;
            try
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            }
            catch (Exception)
            {
                path = @"c:\temp";
            }
            if (string.IsNullOrEmpty(path))
                path = @"c:\temp";
            try
            {
                //如果日志目录不存在,则创建该目录
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string logFileName = path + "\\程序日志_" + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
                StringBuilder logContents = new StringBuilder();
                logContents.AppendLine(msg);
                //当天的日志文件不存在则新建，否则追加内容
                StreamWriter sw = new StreamWriter(logFileName, true, Encoding.Unicode);
                sw.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff") + " " + logContents.ToString());
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
            }
        }

        public static void WriteLog(String fileName, string msg)
        {
            string path = string.Empty;
            try
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            }
            catch (Exception)
            {
                path = @"c:\temp";
            }
            if (string.IsNullOrEmpty(path))
                path = @"c:\temp";
            StreamWriter sw = null;
            try
            {
                if (BaseConfig.DEBUG)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff") + "  " + msg);
                }
                //如果日志目录不存在,则创建该目录
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string logFileName = path + "\\" + fileName + ".log";
                StringBuilder logContents = new StringBuilder();
                logContents.AppendLine(msg);
                //当天的日志文件不存在则新建，否则追加内容
                sw = new StreamWriter(logFileName, true, Encoding.Unicode);
                sw.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff") + " " + logContents.ToString());
            }
            catch (Exception)
            {
            } finally
            {
                if (null != sw)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }
    }
}
