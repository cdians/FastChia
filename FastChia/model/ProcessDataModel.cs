using System;
using System.Windows;

namespace FastChia.model
{
    public class ProcessDataModel
    {
        public ProcessDataModel()
        {

        }

        public ProcessDataModel(String id, String name, String msg, String progress, String status, String path, String finalPath, int loop, Visibility showDelBt)
        {
            this.id = id;
            this.name = name;
            this.msg = msg;
            this.progress = progress;
            this.status = status;
            this.path = path;
            this.finalPath = finalPath;
            this.loop = loop;
            this.showDelBt = showDelBt;

        }

        public String id { get; set; }

        public String name { get; set; }
        public String msg { get; set; }

        public String progress { get; set; }

        public String status { get; set; }

        public String path { get; set; }

        public String finalPath { get; set; }

        public int loop { get; set; }

        public Visibility showDelBt { get; set; }

        public DateTime startTime { get; set; }

        public DateTime endTime { get; set; }

        public string time
        {
            get
            {
                if (default(DateTime) == startTime)
                {
                    return "0秒";
                }
                else if (default(DateTime) != startTime && default(DateTime) == endTime)
                {
                    DateTime local = DateTime.Now;
                    TimeSpan ts = local.Subtract(startTime);
                    string timespan = ts.Hours.ToString() + "小时"
                    + ts.Minutes.ToString() + "分钟"
                    + ts.Seconds.ToString() + "秒";
                    return timespan;
                }
                else if (default(DateTime) != startTime && default(DateTime) != endTime)
                {
                    TimeSpan ts = endTime.Subtract(startTime);
                    string timespan = ts.Hours.ToString() + "小时"
                    + ts.Minutes.ToString() + "分钟"
                    + ts.Seconds.ToString() + "秒";
                    return timespan;
                }
                else
                {
                    return "";
                }
            }
        }

    }
}
