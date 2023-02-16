using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace vivoautotestwifi.Control
{
    class SerialPortControl
    {
        /// <summary>
        /// 获取当前电脑上的所有串口号
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllPort()
        {
            return System.IO.Ports.SerialPort.GetPortNames().ToList();
        }
        
        /// <summary>
        /// 自定义串口类
        /// </summary>
        public class MySerialport
        {
            string portName = null;

            public string PortName
            {
                get { return portName; }
                set { portName = value; }
            }

            SerialPort serialport;

            /// <summary>
            /// 打开端口
            /// </summary>
            public void Open()
            {
                serialport = new SerialPort();
                serialport.PortName = portName;
                serialport.Open();
            }

            /// <summary>
            /// 写入命令并换行
            /// </summary>
            /// <param name="command"></param>
            public void WriteLine(string command)
            {
                string buff = null;
                Log.GetInstance().d("AT", "→" + command);
                serialport.WriteLine(command);
                do
                {
                    //serialport.ReadLine();
                    buff = serialport.ReadLine();
                    Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadLine();
                    //Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadLine();
                    //Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadLine();
                    //Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadChar().ToString();
                } while (buff.Contains("OK")==false);
            }

            /// <summary>
            /// 写入命令并换行，增加设置超时时间，防止异常时死循环
            /// </summary>
            /// <param name="command"></param>
            /// <param name="timeout">超时时间,单位ms</param>
            public void WriteLine(string command, int timeout)
            {
                string buff = null;
                Log.GetInstance().d("AT", "→" + command);
                serialport.WriteLine(command);
                DateTime start = DateTime.Now;
                do
                {
                    //serialport.ReadLine();
                    buff = serialport.ReadLine();
                    Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadLine();
                    //Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadLine();
                    //Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadLine();
                    //Log.GetInstance().d("AT", "←" + buff);
                    //buff = serialport.ReadChar().ToString();
                } while (buff.Contains("OK") == false && DateTime.Now.Subtract(start).TotalMilliseconds < timeout);
            }

            /// <summary>
            /// 写入一行命令并读取一行数据
            /// </summary>
            /// <param name="command"></param>
            /// <returns></returns>
            public string Query(string command)
            {
                string buff = null;
                Log.GetInstance().d("AT", "→" + command);
                serialport.WriteLine(command);
                buff = serialport.ReadLine();
                Log.GetInstance().d("AT", "←" + buff);
                buff = serialport.ReadLine();
                Log.GetInstance().d("AT", "←" + buff);
                buff = serialport.ReadLine();
                Log.GetInstance().d("AT", "←" + buff);
                return buff;
            }

            public void WriteHex(string hexStr)
            {
                Log.GetInstance().d("Serial", "→" + hexStr);
                string[] ArrhexStr = hexStr.Split(' ');
                byte[] buff = new byte[ArrhexStr.Length];
                for (int i = 0; i < ArrhexStr.Length; i++)
                {
                    byte bt = Convert.ToByte(ArrhexStr[i], 16);
                    buff[i] = bt;
                }
                serialport.Write(buff, 0, ArrhexStr.Length);
                // TODO
                string response = serialport.ReadExisting();
                string resHexStr = string.Empty;
                for (int i = 0; i < response.Length; i++)
                {
                    resHexStr += Convert.ToString(response[i], 16).ToUpper() + " ";
                }
                resHexStr = resHexStr.TrimEnd();
                Log.GetInstance().d("Serial", "←" + resHexStr);
                //byte[] res_buff = new byte[serialport.ReadBufferSize];
                //MemoryStream ms = new MemoryStream();
                //int len = serialport.Read(res_buff, 0, res_buff.Length);
                //ms.Write(res_buff, 0, len);
                //res_buff = ms.ToArray();
                //string resHexStr = string.Empty;
                //for (int i = 0; i < res_buff.Length; i++)
                //{
                //    resHexStr += Convert.ToString(res_buff[i], 16) + " ";
                //}
                //resHexStr = resHexStr.TrimEnd();
                //Log.GetInstance().d("Serial", "←" + resHexStr);
            }

            public void Close()
            {
                serialport.Close();
            }
        }
        
        
    }
}
