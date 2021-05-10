using System;
using System.IO;

namespace FastChia.config
{
    public class BaseConfig
    {
        // 日志debug开关
        public const bool DEBUG = true;

        public static string INFO_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");//配置

        public static string INFO_NODE = "basic";
    }
}
