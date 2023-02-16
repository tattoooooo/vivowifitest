using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace vivoautotestwifi.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public static Home home = null;
        private String testItem;
        private Thread _mainTestThread;
        private const string TAG = "Home Page";
        private DateTime startTime;
        private Stopwatch testTimeSW = new Stopwatch();
        /// <summary>
        /// 测试中标志位
        /// </summary>
        private static bool isTesting = false;

        private delegate void controlOperation();
        public static Brush NormalBrush = null;
        public static Brush WarningBrush = null;
        public static Brush ErrorBrush = null;
        public static Brush DebugBrush = null;

        public static ObservableList<LogMessage> LogMessageItems { set; get; } = new ObservableList<LogMessage>(1024);
        public Home()
        {
            home = this;
            InitializeComponent();
            this.DataContext = this;
            InitLogColorBrush();
            Log.SetViewProxy(ViewLog);
        }

        [Obsolete]
        private void Button_Click(object sender , RoutedEventArgs e)
        {
            if (((System.Windows.Controls.Button)sender).Equals(run))
            {
                run.IsEnabled = false;
                Abort.IsEnabled = true;
                control.IsEnabled = true;
            }
            Button button = sender as Button;
            if (button == this.run)
            {
                testItem = "";
                _mainTestThread = new Thread(ThreadTestFunction);
                _mainTestThread.Start();
            }
            else if (button == this.control)
            {
                if (button.Content.Equals("Pause") || button.Content.Equals("暂停"))
                {
                    if (_mainTestThread.IsAlive && (_mainTestThread.ThreadState & System.Threading.ThreadState.Suspended) == 0)

                    {
                        testTimeSW.Stop(); //测试暂停停止测试耗时计时器
                        _mainTestThread.Suspend();
                        button.Content = Application.Current.FindResource("Restart").ToString();
                    }
                }
                else
                {
                    if (_mainTestThread.IsAlive)
                    {
                        testTimeSW.Start(); //测试继续重新启动测试耗时计时器
                        _mainTestThread.Resume();
                        button.Content = Application.Current.FindResource("Pause").ToString();
                    }
                }
            }
        }

        //主线程入口
        private void ThreadTestFunction()
        {
            bool isStart = false;
            startTime = DateTime.Now;
            testTimeSW.Restart();
            testTimeSW.Start();
            isTesting = true;
            if (CheckBoxDelegate(WiFi) == true)
            {
                testItem = "ni无线测试";
                List<string> instruments = new List<string> { "CMW500", "CMW270", "IQxel80", "IQxel160", "IQxelMW" };
                if (instruments.Contains(Pages.Wifi.WiFiConfigDong.wificonfigdong.GetSystemInformation()["Instrument"]))
                {
                    new Test().Wifi_Test();
                }
                else   //CMW100的测试方法
                { 
                    
                }
                isStart = true;
            }
        }

        private bool? CheckBoxDelegate(object controlName)
        {
            bool? ischecked = false;
            this.Dispatcher.Invoke((controlOperation)delegate ()
            {
                ischecked = ((CheckBox)((ListBoxItem)controlName).Content).IsChecked;
            });
            return ischecked;
        }

        private void InitLogColorBrush()
        {
            NormalBrush = new System.Windows.Media.SolidColorBrush(Color.FromRgb(30, 144, 255));
            WarningBrush = new System.Windows.Media.SolidColorBrush(Color.FromRgb(238, 201, 0));
            ErrorBrush = new System.Windows.Media.SolidColorBrush(Color.FromRgb(255, 0, 0));
            DebugBrush = new System.Windows.Media.SolidColorBrush(Color.FromRgb(147, 112, 219));
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="messages">日志Collection</param>
        public void ViewLog(Collection<LogMessage> messages)
        {
            lock (this)
            {
                this.Dispatcher.Invoke((Action)delegate ()
                {
                    //LogMessageItems.AddRange(messages);                    
                    foreach (LogMessage lm in messages)
                    {
                        LogMessageItems.Insert(0, lm);
                    }

                    if (LogMessageItems.Count > 99)
                    {
                        //controlOperation set = delegate ()
                        //{
                        //    LogMessageItems.AddRange(messages);
                        //    if (LogMessageItems.Count > 99)
                        //    {
                        //        int count = LogMessageItems.Count - 99;
                        //        LogMessageItems.RemoveRange(0, count);
                        //    }
                        //};
                        //this.Dispatcher.Invoke(set);
                        //修改 2021.5.31
                        int count = LogMessageItems.Count - 99;
                        for (int index = 0; index < count; index++)
                        {
                            try
                            {
                                LogMessageItems.Remove(LogMessageItems[100 + index]);
                            }
                            catch
                            {
                                //Log.GetInstance().d("Log", string.Format("InputLogCount={0} CurrentLogIndex={1}",count,index));
                            }
                        }

                    }
                });
            }
        }
    }
}
