using System.Windows.Media;

namespace vivoautotestwifi.Pages
{
    /// <summary>
    /// Log消息
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Log消息
        /// </summary>
        public string Message { set; get; }
        /// <summary>
        /// Log 等级
        /// </summary>
        public LogLevel Level { set; get; }
        /// <summary>
        /// Log 画布
        /// </summary>
        public Brush Brush
        {
            get
            {
                if (this.Level == LogLevel.Normal)
                {
                    //Brush必须在主线程初始化，否则出现异常
                    //ArgumentException: 必须在与 DependencyObject 相同的线程上创建 DependencySource。
                    return Home.NormalBrush;
                }
                else if (this.Level == LogLevel.Warning)
                {
                    return Home.WarningBrush;
                }
                else if (this.Level == LogLevel.Error)
                {
                    return Home.ErrorBrush;
                }
                else if (this.Level == LogLevel.Debug)
                {
                    return Home.DebugBrush;
                }
                else
                {
                    return Home.NormalBrush;
                }
            }
        }

        public LogMessage(string message, LogLevel level)
        {
            this.Message = message;
            this.Level = level;
        }

        public LogMessage() { }
    }

    /// <summary>
    /// Log等级枚举
    /// </summary>
    public enum LogLevel
    {
        Normal = 0,
        Warning = 1,
        Error = 2,
        Debug = 3,
    }
}
