using FastChia.common;
using FastChia.config;
using FastChia.view;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FastChia.model
{
    public class MainViewModel : NotificationObject
    {
        public static MainViewModel _mainViewModel;

        private static object singleton_Lock = new object(); // 锁同步

        private MainWindow _window;

        public MainViewModel(MainWindow window)
        {
            this._window = window;
        }

        public static MainViewModel GetModel(MainWindow window)
        {
            lock (singleton_Lock)
            {
                if (null == _mainViewModel)
                {
                    _mainViewModel = new MainViewModel(window);
                }
            }
            return _mainViewModel;
        }

        string _fpk;

        public string fpk
        {
            get
            {
                _fpk = INIHelper.Read(BaseConfig.INFO_NODE, "fpk", "", BaseConfig.INFO_PATH);
                return _fpk;
            }
            set
            {
                if (value.StartsWith("0x"))
                {
                    value = value.Substring(2, value.Length -2);
                }
                _fpk = value;
                INIHelper.Write(BaseConfig.INFO_NODE, "fpk", _fpk, BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("fpk");
            }
        }

        string _ppk;

        public string ppk
        {
            get
            {
                _ppk = INIHelper.Read(BaseConfig.INFO_NODE, "ppk", "", BaseConfig.INFO_PATH);
                return _ppk;
            }
            set
            {
                if (value.StartsWith("0x"))
                {
                    value = value.Substring(2, value.Length - 2);
                }
                _ppk = value;
                INIHelper.Write(BaseConfig.INFO_NODE, "ppk", _ppk, BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("ppk");
            }
        }

        public class FSize
        {

            public FSize(string name, string desc, Double size, Double maxSize)
            {
                this.name = name;
                this.desc = desc;
                this.size = size;
                this.maxSize = maxSize;
            }
            public string name { get; set; }

            public string desc { get; set; }

            public Double size { get; set; }

            public Double maxSize { get; set; }
        }

        List<FSize> _fList = new List<FSize>
        {
            new FSize("32", "K32 101.4GiB", 101.4, 239),
            new FSize("33", "K33 208.8GiB", 208.8, 521),
            new FSize("34", "K34 429.8GiB", 429.8, 1041),
            new FSize("35", "K35 884.1GiB", 884.1, 2175),
            new FSize("25", "K25 600Mb", 0.58, 1.8)
        };

        public List<FSize> fList
        {
            get
            {
                return _fList;
            }
        }

        // 选中农田大小
        String _bSize;

        public FSize bSize
        {
            get
            {
                _bSize = INIHelper.Read(BaseConfig.INFO_NODE, "bSize", "32", BaseConfig.INFO_PATH);
                FSize s = new FSize("", "", 0.00, 0.00);
                foreach (var a in _fList)
                {
                    if (a.name == _bSize)
                    {
                        s = a;
                    }
                }
                return s;
            }
            set
            {
                _bSize = value.name;
                INIHelper.Write(BaseConfig.INFO_NODE, "bSize", _bSize, BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("bSize");
            }
        }

        string _mSize;

        public string mSize
        {
            get
            {
                _mSize = INIHelper.Read(BaseConfig.INFO_NODE, "mSize", "4609", BaseConfig.INFO_PATH);
                return _mSize;
            }
            set
            {
                _mSize = value;
                INIHelper.Write(BaseConfig.INFO_NODE, "mSize", _mSize, BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("mSize");
            }
        }

        string _loopCount;

        public string loopCount
        {
            get
            {
                _loopCount = INIHelper.Read(BaseConfig.INFO_NODE, "loopCount", "1", BaseConfig.INFO_PATH);
                return _loopCount;
            }
            set
            {
                _loopCount = value;
                INIHelper.Write(BaseConfig.INFO_NODE, "loopCount", _loopCount, BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("loopCount");
            }
        }

        string _cpuCount;

        public string cpuCount
        {
            get
            {
                _cpuCount = INIHelper.Read(BaseConfig.INFO_NODE, "cpuCount", "-1", BaseConfig.INFO_PATH);
                if (_cpuCount.Equals("-1"))
                {
                    _cpuCount = Environment.ProcessorCount / 2 + "";
                    INIHelper.Write(BaseConfig.INFO_NODE, "cpuCount", _cpuCount, BaseConfig.INFO_PATH);
                }
                return _cpuCount;
            }
            set
            {
                _cpuCount = value;
                INIHelper.Write(BaseConfig.INFO_NODE, "cpuCount", _cpuCount, BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("cpuCount");
            }
        }

        private int _threadNum;

        public int threadNum
        {
            get
            {
                _threadNum = Convert.ToInt32(INIHelper.Read(BaseConfig.INFO_NODE, "threadNum", "2", BaseConfig.INFO_PATH));
                return _threadNum;
            }
            set
            {
                _threadNum = value;
                INIHelper.Write(BaseConfig.INFO_NODE, "threadNum", _threadNum.ToString(), BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("threadNum");
            }
        }

        private string _writeSpeed;

        public string writeSpeed
        {
            get
            {
                _writeSpeed = INIHelper.Read(BaseConfig.INFO_NODE, "writeSpeed", "190", BaseConfig.INFO_PATH);
                return _writeSpeed;
            }
            set
            {
                _writeSpeed = value;
                INIHelper.Write(BaseConfig.INFO_NODE, "writeSpeed", _writeSpeed, BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("writeSpeed");
            }
        }

        public int runingTaskNum
        {
            get
            {
                if (null == _dataModels)
                {
                    return 0;
                }
                int count = 0;
                foreach(var a in _dataModels)
                {
                    if (a.status == "进行中" || a.status == "转存中")
                    {
                        count += 1;
                    }
                }
                return count;
            }
        }

        private int _finalPlotNum = 0;

        public int finalPlotNum
        {
            get
            {
                return _finalPlotNum;
            }
            set
            {
                _finalPlotNum = value;
                this.RaisePropertyChanged("finalPlotNum");
            }
        }

        // 当前循环次数
        private int _plotLoopNum;

        public int plotLoopNum
        {
            get
            {
                return _plotLoopNum;
            }
            set
            {
                _plotLoopNum = value;
                this.RaisePropertyChanged("plotLoopNum");
            }
        }

        private bool _manual;

        public bool manual { 
            get
            {
                _manual = !Convert.ToBoolean(INIHelper.Read(BaseConfig.INFO_NODE, "autoNext", "False", BaseConfig.INFO_PATH));
                return _manual;
            }
            set
            {
                _manual = value;
                _autoNext = false;
                INIHelper.Write(BaseConfig.INFO_NODE, "autoNext", "False", BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("manual");
                this.RaisePropertyChanged("autoNext");
            }
        }

        private bool _autoNext;

        public bool autoNext
        {
            get
            {
                _autoNext = Convert.ToBoolean(INIHelper.Read(BaseConfig.INFO_NODE, "autoNext", "False", BaseConfig.INFO_PATH));
                return _autoNext;
            }
            set
            {
                _autoNext = value;
                _manual = false;
                INIHelper.Write(BaseConfig.INFO_NODE, "autoNext", "True", BaseConfig.INFO_PATH);
                this.RaisePropertyChanged("autoNext");
                this.RaisePropertyChanged("manual");
            }
        }

        private Visibility _showBt;

        public Visibility showBt
        {
            get
            {
                return _showBt;
            }
            set
            {
                _showBt = value;
                this.RaisePropertyChanged("showBt");
            }
        }

        ObservableCollection<string> _tempList = new ObservableCollection<string>();

        JArray _tempArr = new JArray();

        public ObservableCollection<string> tempList
        {
            get
            {
                _tempList = new ObservableCollection<string>();
                string str = INIHelper.Read(BaseConfig.INFO_NODE, "tempList", "[]", BaseConfig.INFO_PATH);

                if (!"".Equals(str))
                {
                    _tempArr = JArray.Parse(str);
                }

                foreach (JObject a in _tempArr)
                {
                    _tempList.Add(a.Value<string>("path"));
                }
                return _tempList;
            }
        }

        ObservableCollection<string> _saveList;

        public ObservableCollection<string> saveList
        {
            get
            {
                _saveList = new ObservableCollection<string>();
                string str = INIHelper.Read(BaseConfig.INFO_NODE, "saveList", "", BaseConfig.INFO_PATH);
                if (null != str && !"".Equals(str))
                {
                    string[] list = str.Split('|');
                    foreach (var a in list)
                    {
                        _saveList.Add(a);
                    }
                }
                return _saveList;
            }
        }

        private BaseCommand _delTempDir;

        public BaseCommand delTempDir
        {
            get
            {
                if (null == _delTempDir)
                {
                    _delTempDir = new BaseCommand(new Action<object>(o => {
                        string param = (string)o;
                        LogHelper.ShowLog(param);
                        foreach(JObject j in _tempArr)
                        {
                            if (j.Value<string>("path").Equals(param))
                            {
                                _tempArr.Remove(j);
                                break;
                            }
                        }
                        INIHelper.Write(BaseConfig.INFO_NODE, "tempList", Regex.Replace(_tempArr.ToString(), @"\s", ""), BaseConfig.INFO_PATH);
                        this.RaisePropertyChanged("tempList");
                    }));
                }
                return _delTempDir;
            }
        }

        private BaseCommand _delSaveDir;

        public BaseCommand delSaveDir
        {
            get
            {
                if (null == _delSaveDir)
                {
                    _delSaveDir = new BaseCommand(new Action<object>(o => {
                        string param = (string)o;
                        LogHelper.ShowLog(param);
                        _saveList.Remove(param);
                        string dirs = "";
                        for (int i = 0; i < _saveList.Count; i++)
                        {
                            string a = _saveList[i];
                            if (i == 0)
                            {
                                dirs = a;
                            }
                            else
                            {
                                dirs = dirs + '|' + a;
                            }
                        }
                        INIHelper.Write(BaseConfig.INFO_NODE, "saveList", dirs, BaseConfig.INFO_PATH);
                        this.RaisePropertyChanged("saveList");
                    }));
                }
                return _delSaveDir;
            }
        }

        private BaseCommand _delTask;

        public BaseCommand delTask
        {
            get
            {
                if (null == _delTask)
                {
                    _delTask = new BaseCommand(new Action<object>(o =>
                    {
                        string param = (string)o;
                        string tip = "是否删除该任务?执行会有一点延迟";
                        if (MessageBox.Show(tip, "删除任务", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            LogHelper.ShowLog("Delete Task PID: {0}", param);
                            Task.Run(() =>
                            {
                                foreach (var a in _dataModels)
                                {
                                    if (a.id == param)
                                    {
                                        try
                                        {
                                            Process p = Process.GetProcessById(Convert.ToInt32(a.id));
                                            p.Kill();
                                        }
                                        catch (Exception e)
                                        {
                                            LogHelper.ShowLog(e.Message);
                                        }
                                        Thread.Sleep(10 * 1000);
                                        try
                                        {
                                            deleteDirectory(a.path);
                                        }
                                        catch (Exception e)
                                        {
                                            LogHelper.ShowLog(e.Message);
                                            deleteDirectory(a.path);
                                        }
                                        if (a.status.Equals("异常"))
                                        {
                                            if (MessageBox.Show("异常任务是否重开?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                                            {
                                                autoStartTask(_fpk, _ppk, _bSize, _loopCount, _mSize, _threadNum, a.finalPath, a.path);
                                            }
                                        }
                                        ThreadPool.QueueUserWorkItem(delegate
                                        {
                                            SynchronizationContext.SetSynchronizationContext(new
                                                System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                                            SynchronizationContext.Current.Post(pl =>
                                            {
                                                _dataModels.Remove(a);
                                                if (_dataModels.Count < 1)
                                                {
                                                    showBt = Visibility.Visible;
                                                }
                                            }, null);
                                        });
                                        refreshList();
                                        this.RaisePropertyChanged("runingTaskNum");
                                        return;
                                    }
                                }
                            });
                        }
                    }));
                }
                return _delTask;
            }
        }
    
        private BaseCommand _showLog;

        public BaseCommand showLog
        {
            get
            {
                if (null == _showLog)
                {
                    _showLog = new BaseCommand(new Action<object>(o =>
                   {
                       string param = (string)o;
                       _window.Dispatcher.BeginInvoke(new Action(() =>
                       {
                           RealTimeLog timeLog = new RealTimeLog(param);
                           timeLog.Show();
                       }));
                   }));
                }
                return _showLog;
            }
        }

        // TODO 点击开始绘图后 弹出设置参数弹窗 倒计时结束后正式开始

        private BaseCommand _clickEven;

        public BaseCommand ClickEven
        {
            get
            {
                if (null == _clickEven)
                {
                    _clickEven = new BaseCommand(new Action<object>(o =>
                    {
                        string param = (string)o;
                        switch (param)
                        {
                            case "begin":
                                LogHelper.ShowLog("begin task");
                                if (MessageBox.Show("确认开始绘图?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                                {
                                    CreateNewTask(_fpk, _ppk, _bSize, _loopCount, _mSize, _threadNum);
                                }
                                break;
                            case "showKeys":
                                Dictionary<string, string> info = ShowChiaKeys();
                                if (null != info && info.Count > 0)
                                {
                                    ShowKeysDialog dialog = new ShowKeysDialog(_window.Top + 150, _window.Left + 110, info);
                                    if (dialog.ShowDialog().Value)
                                    {
                                        fpk = info["Farmerpublickey"];
                                        ppk = info["Poolpublickey"];
                                    }
                                }
                                break;
                            case "selectTempDir":
                                selectDir(1);
                                break;
                            case "selectSaveDir":
                                selectDir(2);
                                break;
                            case "initCf":
                                if (MessageBox.Show("是否初始化Chia配置文件？装过官方钱包则不用操作", "初始化", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                                {
                                    initChiaConfig();
                                }
                                break;
                            default:
                                LogHelper.ShowLog(param);
                                break;
                        }
                    }));
                }
                return _clickEven;
            }
        }

        /// <summary>
        /// 初始化Chia配置
        /// </summary>
        private void initChiaConfig()
        {
            try
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.FileName = "./chia/chia.exe";
                string args = "init";
                p.StartInfo.Arguments = args;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;//让窗体不显示
                p.Start();
                p.WaitForExit();
                p.Close();
                LogHelper.ShowLog("初始化Chia配置成功");
                MessageBox.Show("初始化成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception e)
            {
                LogHelper.ShowLog(e.Message);
                MessageBox.Show("初始化失败", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // p图路径下标
        private int pNum = 0;

        String regexStr = "ID: ";
        //String regexStr1 = "Progress: ";
        String regexStr2 = "Final File size: ";
        String regexStr3 = "Caught plotting error: ";
        String regexStr4 = "Renamed final file from ";
        String regexStr5 = "Starting plotting progress into temporary dirs: ";

        Dictionary<string, string> progressDic = new Dictionary<string, string> {
            { "Computing table 1", "1"},
            { "Computing table 2", "5"},
            { "Computing table 3", "10"},
            { "Computing table 4", "15"},
            { "Computing table 5", "20"},
            { "Computing table 6", "25"},
            { "Computing table 7", "30"},
            { "Compressing tables 1 and 2", "40"},
            { "Compressing tables 2 and 3", "50"},
            { "Compressing tables 3 and 4", "60"},
            { "Compressing tables 4 and 5", "70"},
            { "Compressing tables 5 and 6", "80"},
            { "Compressing tables 6 and 7", "90"},
            { "Starting to write C1 and C3 tables", "95"},
            { "Final File size:", "98"},
        };

        private void CreateNewTask(string fpk, string ppk, string k, string n, string b, int r)
        {
            dt.Interval = TimeSpan.FromSeconds(30);
            dt.Tick += new EventHandler(autoRefreshList);
            dt.Start();

            showBt = Visibility.Hidden;
            Task.Run(() =>
            {
                // 计算p图数量
                int threadCount = 0;

                // 实际p图数
                int realCount = 0;
                // 实际储存数
                int realSaveCount = 0;

                // 实际占用最大缓存
                Double maxSize = 0.00;
                // 最终大小
                double saveSize = 0.00;

                foreach (var f in _fList)
                {
                    if (f.name == k)
                    {
                        maxSize = f.maxSize;
                        saveSize = f.size;
                        break;
                    }
                }
                // 每个硬盘可用p图数
                List<DirInfo> dirTempArr = new List<DirInfo>();

                // 存储数
                List<DirInfo> dirSaveArr = new List<DirInfo>();


                foreach (JObject jObject in _tempArr)
                {
                    string dir = jObject.Value<string>("path");
                    int preCount = jObject.Value<int>("count");

                    Double freeSize = GetDriveInfoDetail(dir);
                    // 四舍六入五取整
                    int freeCount = (int)Math.Round(freeSize / maxSize);

                    if (freeCount <= 0)
                    {
                        // TODO 容量不够提示
                        return;
                    }

                    if (preCount != 0 && freeCount > preCount)
                    {
                        freeCount = preCount;
                    }
                    
                    DirInfo info = new DirInfo(dir, freeCount);
                    dirTempArr.Add(info);
                    threadCount += freeCount;
                    LogHelper.ShowLog("计算线程数: {0}", threadCount);
                }
                
                foreach (var dir in _saveList)
                {
                    Double freeSize = GetDriveInfoDetail(dir);
                    int freeCount = (int)Math.Floor(freeSize / (saveSize * Convert.ToDouble(n)));
                    if (freeCount <= 0)
                    {
                        // TODO 容量不够提示
                        LogHelper.ShowLog("容量不足: {0}", dir);
                        return;
                    }
                    DirInfo info = new DirInfo(dir, freeCount);
                    dirSaveArr.Add(info);
                    realSaveCount += freeCount;
                    LogHelper.ShowLog("计算储存数: {0}", realSaveCount);
                }

                // 实际存储数小于线程p图数 取存储数
                if (realSaveCount < threadCount)
                {
                    LogHelper.ShowLog("实际存储数小于线程数");
                    threadCount = realSaveCount;
                }

                if (threadCount > Convert.ToInt32(_cpuCount))
                {
                    realCount = Convert.ToInt32(_cpuCount);
                }
                else
                {
                    realCount = threadCount;
                }

                int tempIndex = 0;
                int saveIndex = 0;

                // 最终路径
                string d = "";
                // 临时路径
                string t = "";
                _plotLoopNum = 1;
                this.RaisePropertyChanged("plotLoopNum");
                for (int q = 0; q < realCount; q++)
                {
                    d = dirSaveArr[saveIndex].name;
                    t = dirTempArr[tempIndex].name + "\\" + pNum;
                    pNum += 1;
                    ProcessDataModel model = new ProcessDataModel();
                    if (null == _dataModels)
                    {
                        _dataModels = new ObservableCollection<ProcessDataModel>();

                    }
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        SynchronizationContext.SetSynchronizationContext(new
                            System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                        SynchronizationContext.Current.Post(pl =>
                        {
                            _dataModels.Add(model);
                        }, null);
                    });
                    this.RaisePropertyChanged("runingTaskNum");
                    model.progress = "1%";
                    model.status = "进行中";
                    model.path = t;
                    model.finalPath = d;
                    model.loop = 1;
                    model.startTime = DateTime.Now;
                    model.showDelBt = Visibility.Visible;
                    Task.Run(() =>
                    {
                        Process p = new Process();
                        p.StartInfo = new ProcessStartInfo();
                        p.StartInfo.FileName = "./chia/chia.exe";
                        string args = "plots create -f " + fpk + " -p " + ppk + " -k " + k + " -n " + n + " -b " + b + " -d " + d + " -t " + t + " -r " + r;
                        if (k.Equals("25"))
                        {
                            args += " --override-k";
                        }
                        p.StartInfo.Arguments = args;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;//让窗体不显示
                        p.Start();
                        LogHelper.ShowLog("process id: {0}", p.Id);
                        model.id = p.Id.ToString();
                        StreamReader reader = p.StandardOutput;//截取输出流
                        string line = reader.ReadLine();//每次读取一行
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();//每次读取一行
                            if (null != line && !"".Equals(line))
                            {
                                if (null == model.name || "".Equals(model.name))
                                {
                                    LogHelper.ShowLog("PID {1} info: {0}", line, p.Id);
                                } else
                                {
                                    LogHelper.WriteLog(model.name, line);
                                }

                                if (Regex.IsMatch(line, regexStr))
                                {
                                    String name = line.Split(':')[1];
                                    LogHelper.ShowLog("PID {1} find ID: {0}", name, p.Id);
                                    model.name = name;
                                }
                                //else if (Regex.IsMatch(line, regexStr1))
                                //{
                                //    String progress = line.Split(':')[1];
                                //    LogHelper.ShowLog("PID {1} Progress: {0}", progress, p.Id);
                                //    model.progress = progress + "%";
                                //}
                                else if (Regex.IsMatch(line, regexStr2))
                                {
                                    String finalSize = line.Split(':')[1];
                                    model.status = "转存中";
                                } else if (Regex.IsMatch(line, regexStr3))
                                {
                                    String msg = line.Split(':')[1];
                                    LogHelper.ShowLog("Task Throws Exception!!! MsgInfo: {0}", msg);
                                    model.status = "异常";
                                } else if (Regex.IsMatch(line, regexStr4))
                                {
                                    model.progress = "100%";
                                    model.status = "完成";
                                    model.endTime = DateTime.Now;
                                    model.showDelBt = Visibility.Hidden;
                                    finalPlotNum = finalPlotNum + 1;
                                    // 开启自动接续
                                    if (_autoNext && model.loop == Convert.ToInt32(n))
                                    {
                                        LogHelper.ShowLog(model.name + " 已完成 自动接续");
                                        autoStartTask(_fpk, _ppk, _bSize, _loopCount, _mSize, _threadNum, model.finalPath, model.path);
                                    }
                                } else if (Regex.IsMatch(line, regexStr5))
                                {
                                    if (!model.status.Equals("进行中"))
                                    {
                                        // 新一轮开始
                                        model.status = "进行中";
                                        model.progress = "1%";
                                        model.endTime = default(DateTime);
                                        model.showDelBt = Visibility.Visible;
                                        model.loop = model.loop + 1;
                                        // 判断是否全部循环完毕
                                        int temCount = 0;
                                        foreach (var m in _dataModels)
                                        {
                                            if (m.loop > _plotLoopNum)
                                            {
                                                temCount += 1;
                                            }
                                        }
                                        if (temCount == _dataModels.Count)
                                        {
                                            _finalPlotNum = 0;
                                            _plotLoopNum += 1;
                                            this.RaisePropertyChanged("plotLoopNum");
                                            this.RaisePropertyChanged("finalPlotNum");
                                        }
                                    }
                                }
                                // 判断进度
                                if (line.Length < 40 && progressDic.ContainsKey(line))
                                {
                                    model.progress = progressDic[line] + "%";
                                }
                                refreshList();
                                this.RaisePropertyChanged("runingTaskNum");
                            }
                        }
                        LogHelper.ShowLog("Task Finish!!!!!!!!!");
                        p.WaitForExit();
                        p.Close();
                        // 判断所有任务是否全部完成
                        int taskFinalNum = 0;
                        foreach(var o in _dataModels)
                        {
                            if (o.status.Equals("完成") || o.status.Equals("异常"))
                            {
                                taskFinalNum += 1;
                            }
                        }
                        if (taskFinalNum == _dataModels.Count)
                        {
                            dt.Stop();
                            if (MessageBox.Show("是否开始新一轮Plot?", "任务完成", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                Task.Run(() =>
                               {
                                   CreateNewTask(_fpk, _ppk, _bSize, _loopCount, _mSize, _threadNum);
                               });
                            }
                            else
                            {
                                showBt = Visibility.Visible;
                            }
                        }
                    });
                    // 计算路径
                    dirSaveArr[saveIndex].count = dirSaveArr[saveIndex].count - 1;
                    dirTempArr[tempIndex].count = dirTempArr[tempIndex].count - 1;
                    if (dirSaveArr[saveIndex].count <= 0)
                    {
                        saveIndex += 1;
                    }
                    if (dirTempArr[tempIndex].count <= 0)
                    {
                        tempIndex += 1;
                    }
                    double finalSaveSize = saveSize * 1024; // 单位m
                    int sleepTime = (int)Math.Floor(finalSaveSize / Convert.ToDouble(_writeSpeed));
                    Thread.Sleep(sleepTime * 1000);
                }
            });
        }

        private void autoStartTask(string fpk, string ppk, string k, string n, string b, int r, string d, string t)
        {
            // TODO 自动接续 强制循环为1
            // 实际占用最大缓存
            Double maxSize = 0.00;
            // 最终大小
            double saveSize = 0.00;

            foreach (var f in _fList)
            {
                if (f.name == k)
                {
                    maxSize = f.maxSize;
                    saveSize = f.size;
                    break;
                }
            }

            // 验证磁盘空间
            Double freeSize = GetDriveInfoDetail(t);
            if (freeSize < maxSize)
            {
                // TODO 容量不够提示
                LogHelper.ShowLog("缓存目录容量不足无法接续");
                return;
            }

            // 验证最终磁盘空间 还要计算进未完成任务的
            Double freeSaveSize = GetDriveInfoDetail(d);

            // 即将要完成任务需要的容量
            Double futrueSaveSize = 0.00;

            foreach(var m in _dataModels)
            {
                if (m.finalPath.Equals(d) && m.status.Equals("进行中"))
                {
                    futrueSaveSize = futrueSaveSize + (saveSize * Convert.ToDouble(n));
                }
            }

            freeSaveSize = freeSaveSize - futrueSaveSize;

            if (freeSaveSize < saveSize)
            {
                bool findDir = false;
                // TODO 容量不够提示 换盘
                LogHelper.ShowLog("最终目录 " + d +" 容量不足无法接续 剩余容量：{0} 需要容量{1}", freeSaveSize, saveSize);

                foreach (var dir in _saveList)
                {
                    if (dir != d)
                    {
                        freeSaveSize = GetDriveInfoDetail(dir);
                        if (freeSaveSize > saveSize)
                        {
                            futrueSaveSize = 0.00;
                            // 需要加上新选中硬盘进行中的任务
                            foreach (var m in _dataModels)
                            {
                                if (m.finalPath.Equals(dir) && m.status.Equals("进行中"))
                                {
                                    futrueSaveSize = futrueSaveSize + (saveSize * Convert.ToDouble(n));
                                }
                            }
                            freeSaveSize = freeSaveSize - futrueSaveSize;
                            if (freeSaveSize > saveSize)
                            {
                                LogHelper.ShowLog("找到符合容量的最终盘 从 {0} 切换到 {1}", d, dir);
                                findDir = true;
                                d = dir;
                                break;
                            }
                        }
                    }
                }
                if (!findDir)
                {
                    LogHelper.ShowLog("未找到可用容量的最终盘");
                    return;
                }
            }

            ProcessDataModel model = new ProcessDataModel();
            if (null == _dataModels)
            {
                _dataModels = new ObservableCollection<ProcessDataModel>();

            }
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                    System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(pl =>
                {
                    _dataModels.Add(model);
                }, null);
            });
            this.RaisePropertyChanged("runingTaskNum");
            model.progress = "1%";
            model.status = "进行中";
            model.path = t;
            model.finalPath = d;
            model.loop = 1;
            model.showDelBt = Visibility.Visible;
            model.startTime = DateTime.Now;
            Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.FileName = "./chia/chia.exe";
                string args = "plots create -f " + fpk + " -p " + ppk + " -k " + k + " -n " + n + " -b " + b + " -d " + d + " -t " + t + " -r " + r;
                if (k.Equals("25"))
                {
                    args += " --override-k";
                }
                p.StartInfo.Arguments = args;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;//让窗体不显示
                p.Start();
                LogHelper.ShowLog("process id: {0}", p.Id);
                model.id = p.Id.ToString();
                StreamReader reader = p.StandardOutput;//截取输出流
                string line = reader.ReadLine();//每次读取一行
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();//每次读取一行
                    if (null != line && !"".Equals(line))
                    {
                        if (null == model.name || "".Equals(model.name))
                        {
                            LogHelper.ShowLog("PID {1} info: {0}", line, p.Id);
                        }
                        else
                        {
                            LogHelper.WriteLog(model.name, line);
                        }

                        if (Regex.IsMatch(line, regexStr))
                        {
                            String name = line.Split(':')[1];
                            LogHelper.ShowLog("PID {1} find ID: {0}", name, p.Id);
                            model.name = name;
                        }
                        //else if (Regex.IsMatch(line, regexStr1))
                        //{
                        //    String progress = line.Split(':')[1];
                        //    LogHelper.ShowLog("PID {1} Progress: {0}", progress, p.Id);
                        //    model.progress = progress + "%";
                        //}
                        else if (Regex.IsMatch(line, regexStr2))
                        {
                            String finalSize = line.Split(':')[1];
                            model.status = "转存中";
                        }
                        else if (Regex.IsMatch(line, regexStr3))
                        {
                            String msg = line.Split(':')[1];
                            LogHelper.ShowLog("Task Throws Exception!!! MsgInfo: {0}", msg);
                            model.status = "异常";
                        }
                        else if (Regex.IsMatch(line, regexStr4))
                        {
                            model.progress = "100%";
                            model.status = "完成";
                            model.endTime = DateTime.Now;
                            model.showDelBt = Visibility.Hidden;
                            finalPlotNum = finalPlotNum + 1;
                            // 开启自动接续
                            if (_autoNext && model.loop == Convert.ToInt32(n))
                            {
                                LogHelper.ShowLog("自动接续");
                                autoStartTask(_fpk, _ppk, _bSize, _loopCount, _mSize, _threadNum, model.finalPath, model.path);
                            }
                        }
                        else if (Regex.IsMatch(line, regexStr5))
                        {
                            if (!model.status.Equals("进行中"))
                            {
                                // 新一轮开始
                                model.status = "进行中";
                                model.progress = "1%";
                                model.endTime = default(DateTime);
                                model.showDelBt = Visibility.Visible;
                                model.loop = model.loop + 1;
                                // 判断是否全部循环完毕
                                int temCount = 0;
                                foreach (var m in _dataModels)
                                {
                                    if (m.loop > _plotLoopNum)
                                    {
                                        temCount += 1;
                                    }
                                }
                                if (temCount == _dataModels.Count)
                                {
                                    _finalPlotNum = 0;
                                    _plotLoopNum += 1;
                                    this.RaisePropertyChanged("plotLoopNum");
                                    this.RaisePropertyChanged("finalPlotNum");
                                }
                            }
                        }
                        // 判断进度
                        if (line.Length < 40 && progressDic.ContainsKey(line))
                        {
                            model.progress = progressDic[line] + "%";
                        }
                        refreshList();
                        this.RaisePropertyChanged("runingTaskNum");
                    }
                }
                LogHelper.ShowLog("Task Finish!!!!!!!!!");
                p.WaitForExit();
                p.Close();
                // 判断所有任务是否全部完成
                int taskFinalNum = 0;
                foreach (var o in _dataModels)
                {
                    if (o.status.Equals("完成") || o.status.Equals("异常"))
                    {
                        taskFinalNum += 1;
                    }
                }
                if (taskFinalNum == _dataModels.Count)
                {
                    if (MessageBox.Show("是否开始新一轮Plot?", "任务完成", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        // TODO 开始
                        Task.Run(() =>
                        {
                            CreateNewTask(_fpk, _ppk, _bSize, _loopCount, _mSize, _threadNum);
                        });
                    }
                    else
                    {
                        // TODO 关闭
                        showBt = Visibility.Visible;
                    }
                }
            });
        }

        private void deleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
        }

        private System.Windows.Threading.DispatcherTimer dt = new DispatcherTimer();

        private static object lockObj = new object();//定义锁

        private void refreshList()
        {
            lock (lockObj) {
                Action action1 = () =>
                {
                    _window.TaskList.ItemsSource = null;
                    _window.TaskList.ItemsSource = _dataModels;
                };
                _window.TaskList.Dispatcher.BeginInvoke(action1);
            }
        }

        private void autoRefreshList(object sender, EventArgs e)
        {
            refreshList();
        }

        private Dictionary<string, string> ShowChiaKeys()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.FileName = "./chia/chia.exe";
                p.StartInfo.Arguments = "keys show";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;//让窗体不显示
                p.Start();
                StreamReader reader = p.StandardOutput;//截取输出流
                string line = reader.ReadLine();//每次读取一行
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();//每次读取一行
                    if (null != line && !"".Equals(line))
                    {
                        LogHelper.ShowLog(line);
                        string[] str = line.Split(':');
                        string title = str[0].Replace(" ", "");
                        int startIndex = title.LastIndexOf('(');
                        if (startIndex != -1)
                        {
                            title = title.Substring(0, startIndex);
                        }
                        string desc = str[1];
                        dic.Add(title, desc.Trim());
                    }
                }
                LogHelper.ShowLog("show finish!!!!!!!!!");
                p.WaitForExit();
                p.Close();
            } catch (Exception e)
            {
                LogHelper.ShowLog("error", e.Message);
                MessageBox.Show("读取公钥失败，请手动填写", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return dic;
        }

        private void selectDir(int type)
        {
            if (type == 1)
            {
                SelectDirDialog dirDialog = new SelectDirDialog(_window.Top + 150, _window.Left + 110);
                if (dirDialog.ShowDialog().Value)
                {
                    this.RaisePropertyChanged("tempList");
                }
            } else
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                CommonFileDialogResult result = dialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                {
                    var dir = dialog.FileName;
                    LogHelper.ShowLog("文件夹名字：{0}", dir);
                    if (null != dir)
                    {
                        if (!_saveList.Contains(dir))
                        {
                            string dirs = INIHelper.Read(BaseConfig.INFO_NODE, "saveList", "", BaseConfig.INFO_PATH);
                            if ("".Equals(dirs))
                            {
                                dirs = dir;
                            }
                            else
                            {
                                dirs = dirs + "|" + dir;
                            }
                            INIHelper.Write(BaseConfig.INFO_NODE, "saveList", dirs, BaseConfig.INFO_PATH);
                            this.RaisePropertyChanged("saveList");
                        }
                    }
                }
            }
        }

        private Double GetDriveInfoDetail(string driveName)
        {
            LogHelper.ShowLog("查询 " + driveName + "容量");
            long gb = 1024 * 1024 * 1024;

            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_LogicalDisk WHERE DeviceID = '{0}'", driveName.Substring(0, 2)));

            ManagementObjectSearcher managerSearch = new ManagementObjectSearcher(wqlObjectQuery);

            Double size = 0.00;
            foreach (ManagementObject mobj in managerSearch.Get())
            {
                // 本地或远程
                LogHelper.ShowLog("Description: " + mobj["Description"]);
                // 磁盘格式
                LogHelper.ShowLog("File system: " + mobj["FileSystem"]);
                // 可用空间
                size = Convert.ToDouble(mobj["FreeSpace"]) / gb;
                LogHelper.ShowLog("Free disk space: " + size);
                //总空间
                LogHelper.ShowLog("Size: " + Convert.ToDouble(mobj["Size"]) / gb);
            }
            return size;
        }

        private ObservableCollection<ProcessDataModel> _dataModels;

        public ObservableCollection<ProcessDataModel> dataModels
        {
            get
            {
                if (null == _dataModels)
                {
                    _dataModels = new ObservableCollection<ProcessDataModel>();
                }
                return _dataModels;
            }
        }
    }
}

