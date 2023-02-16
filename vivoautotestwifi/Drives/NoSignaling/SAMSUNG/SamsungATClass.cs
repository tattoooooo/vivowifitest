using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO.Ports;
     
//wzt
namespace vivoautotestwifi.Drives.NoSignaling.SAMSUNG
{
    class SamsungATClass
    {
        //SerialPort serialPort = new SerialPort();

        Process proc;
        public static int port=0;
        Socket socket; 
        string adbResource = null;
        int samsung_port = 0;
        static IPAddress iP = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipe = new IPEndPoint(iP, port);

        //public static int port = 0;//Samsung通信端口

        //public SamsungATClass(string adbresourcORcomport)
        //{

        //    adbResource = adbresourcORcomport;
        //    samsung_port = port;
        //    //serialPort.PortName = "COM" + adbresource;
        //    //serialPort.Open();
        //    OpenSamsungSdk();
        //    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    System.Threading.Thread.Sleep(1000);
        //    socket.Connect(ipe);
        //}

        public void Ini(string adbresourcORcomport)
        {
            Log.GetInstance().d("Ini", adbresourcORcomport);
            adbResource = adbresourcORcomport;
            samsung_port = port;
            //serialPort.PortName = "COM" + adbresource;
            //serialPort.Open();
            OpenSamsungSdk();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            System.Threading.Thread.Sleep(1000);

            socket.Connect(ipe);


        }

        private void OpenSamsungSdk()
        {
            proc = new Process();
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine("adb -s " + adbResource + " " + " forward tcp:" + samsung_port.ToString() + " " + "tcp:" + samsung_port.ToString() + " " + "&& adb shell atsmdk");

            Log.GetInstance().d("Samsung:", "adb" + " " + adbResource + " " + "-s" + " " + " forward tcp:" + samsung_port.ToString() + " " + "tcp:" + samsung_port.ToString() + " " + "&& adb shell atsmdk");
            Log.GetInstance().d("Samsung", "打开Samsung 测试 SDK");

            //proc.StandardInput.WriteLine("exit");
            //string outStr = proc.StandardOutput.ReadToEnd();
            //Console.WriteLine("error:" + outStr);
            //proc.WaitForExit();
            //proc.Close();
            //Log.GetInstance().d("Samsung", "错误： Samsung测试SDK关闭");
            //System.Threading.Thread.Sleep(5000);
        }

        public void Close()
        {
            socket.Close();
            proc.Kill();
        }

        public string SamsungAT_Send(string command)
        {
            string re = null;

            //string buff = string.Empty;
            //String readline = string.Empty;
            //serialPort.WriteLine(command);
            //do
            //{
            //    if (serialPort.IsOpen == false)
            //    {
            //        serialPort.Close();
            //        serialPort.Open();
            //    }
            //    readline = serialPort.ReadLine();
            //    buff = buff + readline;
            //    if (readline.Contains("OK")) break;
            //} while (readline.Contains("OK") == false || readline.Contains("ERROR") == false);
            //Log.GetInstance().d("Samsung", command);

            //OpenSamsungSdk();
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.Connect(ipe);
            
            //string sendStr = "AT+WIFIRF=0,1,1,0";
            Log.GetInstance().d("Samsung", command);
            byte[] sendBytes = Encoding.ASCII.GetBytes(command);

            byte[] buff = new byte[1024];
            int j = 0;


            socket.Send(sendBytes);

            j = socket.Receive(buff);


            string s = Encoding.ASCII.GetString(buff);
            if (s.Contains("OK"))
            {
                re = "Success";
            }
            else
            {
                re = "Failure";
            }
            Log.GetInstance().d("Samsung+open", re + "," + command + "=" + s);
            //socket.Disconnect(true);
            //socket.Close();
            //Close();



            return re;
        }

        public string SamsungAT_Read(string command)
        {
            //string buff = string.Empty;
            //String readline= string.Empty;
            //serialPort.WriteLine(command);
            //do
            //{
            //    if (serialPort.IsOpen == false)
            //    {
            //        serialPort.Close();
            //        serialPort.Open();
            //    }
            //    readline = serialPort.ReadLine();
            //    buff = buff + readline;
            //    if (readline.Contains("OK")) break;
            //} while (readline.Contains("OK") == false || readline.Contains("ERROR") == false);
            //Log.GetInstance().d("Samsung", command);
            string re = null;
            //OpenSamsungSdk();
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.Connect(ipe);
            //Log.GetInstance().d("Samsung", buff);
            byte[] sendBytes = Encoding.ASCII.GetBytes(command);
            byte[] buff = new byte[1024];
            int j = 0;
            //try
            //{
                socket.Send(sendBytes);
                System.Threading.Thread.Sleep(100);
                j = socket.Receive(buff);
            //}
            //catch(SocketException sex)
            //{

            //}
            
            
            
            string s = Encoding.ASCII.GetString(buff);
            if (s.Contains("OK"))
            {
                Log.GetInstance().d("Samsung", command + "命令发送成功");
            }
            else
            {
                Log.GetInstance().d("Samsung", command + "命令发送失败");
            }
            re = s.Split(',').Last();
            Log.GetInstance().d("Samsung", re);
            Log.GetInstance().d("Samsung", "s" + s);
            //socket.Close();
            //Close();
            return re;
        }

        public void exitAdbServer()
        {

        }

        
        public void CloseAdbServer()
        {
            proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine("adb Kill-server");
            Log.GetInstance().d("Samsung", "adb Kill-server");

            //proc.StandardInput.WriteLine("exit");
            //string outStr = proc.StandardOutput.ReadToEnd();
            //Console.WriteLine("error:" + outStr);
            //proc.WaitForExit();
            proc.Close();
            Log.GetInstance().d("Samsung", "错误： Samsung测试SDK关闭");
        }

    }
}
