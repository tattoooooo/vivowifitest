using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using vivoautotestwifi.Pages;
using vivoautotestwifi;

namespace vivoautotestwifi
{
    /// <summary>
    /// 全局系统日志类
    /// </summary>
    public class Log
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// 单例模式：log对象.
        /// </summary>
        public static readonly Log log = new Log();
        /// <summary>
        /// log系统主线程
        /// </summary>
        private Thread LogThread;
        /// <summary>
        /// 日志消息队列(线程安全)
        /// </summary>
        private readonly ConcurrentQueue<LogMessage> LogQueue = new ConcurrentQueue<LogMessage>();
        /// <summary>
        /// 定义UI 代理方法
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <param name="level">日志等级</param>
        public delegate void WriteLog(Collection<LogMessage> messages);
        /// <summary>
        /// UI 打印Log方法
        /// </summary>
        private WriteLog WriteLogFunction;
        /// <summary>
        /// Log4Net日志实例
        /// </summary>
        private static readonly ILog logger;
        /// <summary>
        /// Log线程生命标志位
        /// </summary>
        private static Boolean IsLife = true;

        /// <summary>
        /// 静态构造器
        /// </summary>
        static Log()
        {
            logger = LogManager.GetLogger("appsLog");
        }

        /// <summary>
        /// 类构造器
        /// </summary>
        public Log()
        {
            LogThread = new Thread(Run);
            LogThread.IsBackground = true;
            LogThread.Start();
        }

        /// <summary>
        /// 创建窗体打印Log方法
        /// </summary>
        /// <param name="writeLogFunction">打印日志的方法</param>
        public static void SetViewProxy(WriteLog writeLogFunction)
        {
            if (log != null)
            {
                log.WriteLogFunction = writeLogFunction;
            }
        }

        /// <summary>
        /// 获取日志实例
        /// </summary>
        /// <returns></returns>
        public static Log GetInstance()
        {
            return log;
        }

        /// <summary>
        /// 添加一条消息
        /// </summary>
        /// <param name="message"></param>
        private void PushLog(LogMessage message)
        {
            this.LogQueue.Enqueue(message);
        }

        /// <summary>
        /// log线程执行方法体
        /// </summary>
        public void Run()
        {
            while (IsLife)
            {
                while (!LogQueue.IsEmpty)
                {
                    //modifed by wzt at 2021.7.27
                    //取出全部日志
                    //Collection<LogMessage> buff = new Collection<LogMessage>();
                    for (int i = 0; i < LogQueue.Count; i++)
                    {
                        if (LogQueue.TryDequeue(out LogMessage logMessage))
                        {
                            Collection<LogMessage> buff = new Collection<LogMessage>();
                            buff.Add(logMessage);
                            this.WriteLogFunction(buff);
                        }
                    }
                    //LogQueue.TryDequeue(out LogMessage logMessage);
                    //buff.Add(logMessage);
                    //显示
                    //this.WriteLogFunction(buff);
                }
                //Thread.Sleep(1000);
            }
        }


        #region 打印日志方法方法
        /// <summary>
        /// 打印一条 Debug  Log日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="args">格式化参数</param>
        /// <param name="format">格式化</param>
        public void d(string tag, string format, params object[] args)
        {
            string msg = String.Format(format, args);
            logger.DebugFormat("{0}    {1}", tag, msg);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("D");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(msg);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Debug));
                sb.Clear();
            }
        }

        /// <summary>
        /// 打印一条 Debug  Log日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="message">Log消息主体</param>
        public void d(string tag, string message)
        {
            logger.DebugFormat("{0}    {1}", tag, message);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("D");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(message);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Debug));
                sb.Clear();
            }
        }

        /// <summary>
        /// 打印一条Info log日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="format">格式化</param>
        /// <param name="arg">格式化参数</param>
        public void i(string tag, string format, params object[] arg)
        {
            string msg = string.Format(format, arg);
            logger.InfoFormat("{0}    {1}", tag, msg);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("I");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(msg);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Normal));
                sb.Clear();
            }
        }

        /// <summary>
        /// 打印一条Info log日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="message">Log消息主体</param>
        public void i(string tag, string message)
        {
            logger.InfoFormat("{0}    {1}", tag, message);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("I");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(message);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Normal));
                sb.Clear();
            }
        }

        /// <summary>
        ///  打印一条Error log日志
        /// </summary>
        /// <param name="tag">Log标签</param>
        /// <param name="format">格式化</param>
        /// <param name="args">格式化参数</param>
        public void e(string tag, string format, params object[] args)
        {
            string msg = string.Format(format, args);
            logger.ErrorFormat("{0}    {1}", tag, msg);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("E");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(msg);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Error));
                sb.Clear();

            }
        }

        /// <summary>
        ///  打印一条Error log日志
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        public void e(string tag, string message)
        {
            logger.ErrorFormat("{0}    {1}", tag, message);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("E");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(message);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Error));
                sb.Clear();

            }
        }

        /// <summary>
        ///  打印一条Error log日志
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="e"></param>
        public void e(string tag, Exception e)
        {
            logger.Error(tag, e);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("E");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(e?.ToString());
                PushLog(new LogMessage(sb.ToString(), LogLevel.Error));
                sb.Clear();
            }
        }

        /// <summary>
        ///  打印一条Warning log日志
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        public void w(string tag, string message)
        {
            logger.WarnFormat("{0}    {1}", tag, message);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("W");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(message);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Warning));
                sb.Clear();

            }
        }

        /// <summary>
        ///  打印一条Warning log日志
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="format">格式化</param>
        /// <param name="args">格式化参数</param>
        public void w(string tag, string format, params object[] args)
        {
            string msg = string.Format(format, args);
            logger.WarnFormat("{0}    {1}", tag, msg);
            if (WriteLogFunction != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetSystemTime());
                sb.Append("    ");
                sb.Append("W");
                sb.Append("    ");
                sb.Append(tag);
                sb.Append(" ");
                sb.Append(msg);
                PushLog(new LogMessage(sb.ToString(), LogLevel.Warning));
                sb.Clear();

            }
        }

        /// <summary>
        /// 返回标准系统时间
        /// </summary>
        /// <returns></returns>
        private static string GetSystemTime()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ");
        }
        #endregion

    }
}