using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Threading;
using NationalInstruments.VisaNS;
using vivoautotestwifi.CustomizeToast;
using vivoautotestwifi.Pages;

namespace vivoautotestwifi.Control
{
    public class GlobalCommunicate
    {
        //NI连接实例
        private MessageBasedSession messageBasedSession;

        private ResourceManager resourceManager;
        //延时时间
        public int delayTime { get; set; } = 1000;

        //CMW500命令结束符/分割符
        private const string RohdeSchwarzENDFLAG = "\n";

        //MT8820C命令结束符/分割符
        private const string ANRITSUENDFLAG = "\n";

        /// <summary>
        /// 是否已经结束
        /// </summary>
        public static bool isFinish = false;

        /// <summary>
        /// 连接设备的信息
        /// </summary>
        private CommunicateUtils.DeviceInfo connectDeviceInfo;

        public CommunicateUtils.DeviceInfo ConnectDeviceInfo
        {
            get { return this.connectDeviceInfo; }
        }

        public static Log log;

        public const string TAG = "GlobalCommunicate";

        public GlobalCommunicate()
        {
            resourceManager =  ResourceManager.GetLocalManager();
            log = Log.GetInstance();
        }


        /// <summary>
        /// 获取可连接的设备，优先返回8820C
        /// </summary>
        /// <returns></returns>
        private CommunicateUtils.DeviceInfo GetFastConnectDevice()
        {
            List<CommunicateUtils.DeviceInfo> device = CommunicateUtils.FindResources(false);
            if (device==null||device.Count == 0)
            {
                return null;
            }
      
            int devNum = 0;
            foreach (CommunicateUtils.DeviceInfo dev in device)
            {
                if (dev.DeviceType.Contains("ANRITSU"))
                {
                    devNum += 1;
                }
                if (dev.DeviceType.Contains("Rohde&Schwarz"))
                {
                    devNum += 1;
                }
                if (dev.DeviceType.Contains("Starpoint"))
                {
                    devNum += 1;
                }
            }
            if (devNum == 0) {
                return null;
            }
            if (devNum == 1)
            {
                foreach (CommunicateUtils.DeviceInfo dev in device)
                {
                    if (dev.DeviceType.Contains("ANRITSU"))
                    {
                        delayTime = 100;
                        return dev;
                    }
                }
                foreach (CommunicateUtils.DeviceInfo dev in device)
                {
                    if (dev.DeviceType.Contains("Rohde&Schwarz"))
                    {
                        delayTime = 2000;
                        return dev;
                    }
                }
                foreach (CommunicateUtils.DeviceInfo dev in device)
                {
                    if (dev.DeviceType.Contains("Starpoint"))
                    {
                        delayTime = 2000;
                        return dev;
                    }
                }
            }
            else if(devNum>1){
                List<CommunicateUtils.DeviceInfo> target = new List<CommunicateUtils.DeviceInfo>();
                foreach (CommunicateUtils.DeviceInfo dev in device)
                {
                    if (dev.DeviceType.Contains("ANRITSU"))
                    {
                        target.Add(dev);
                    }
                    if (dev.DeviceType.Contains("Rohde&Schwarz"))
                    {
                        target.Add(dev);
                    }
                    if (dev.DeviceType.Contains("Starpoint"))
                    {
                        target.Add(dev);
                    }
                }
                string id = null;
                Home.home.Dispatcher.Invoke(() =>
                {
                    ChoseDevice  choseDevice= new ChoseDevice(target);      //ChooseDevice是一个xaml页面,把这个弹框窗口委托给Home.xaml来做，展示在Home.xaml上
                    choseDevice.ShowDialog();
                    id = choseDevice.id;
                });
                if (id != null)
                {
                    foreach (CommunicateUtils.DeviceInfo dev in device)
                    {
                        if (dev.ID.Equals(id))
                        {
                            if (dev.DeviceType.Contains("ANRITSU"))
                            {
                                delayTime = 100;
                                //return dev;
                            }
                            else if (dev.DeviceType.Contains("Rohde&Schwarz"))
                            {
                                delayTime = 200;
                                //return dev;
                            }
                            else
                            {
                                delayTime = 200;
                            }
                            return dev;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 创建回话，连接到移动程控电源
        /// </summary>
        /// <returns>
        /// 0:连接失败，设备不存在
        /// -1：连接失败，参数错误
        /// 1：连接成功
        /// </returns>
        public int ConnectMobileCommunicationDCSource()
        {
            //搜索所有NI通讯设备
            List<CommunicateUtils.DeviceInfo> device = CommunicateUtils.FindResources(false);
            if (device.Count == 0)
            {
                return -1;
            }
            isFinish = false;
            try
            {
                //遍历设备*IDN返回的字符串
                foreach (CommunicateUtils.DeviceInfo dev in device)
                {
                    if (dev.DeviceType.Contains("HEWLETT-PACKARD") &&
                        (dev.DeviceType.Contains("66309B") || dev.DeviceType.Contains("66311D")))
                    {
                        delayTime = 0;
                        messageBasedSession = (MessageBasedSession) resourceManager.Open(dev.ID);
                        connectDeviceInfo = dev;
                        return 1;
                    }
                }
            }
            catch (VisaException ex)
            {
                log?.e("VISA Communication", ex.Message);
                connectDeviceInfo = null;
                return 0;
            }
            catch (ArgumentException ex)
            {
                log.e("VISA Communication",ex.Message);
                connectDeviceInfo = null;
                return -1;
            }
            catch (Exception ex)
            {
                connectDeviceInfo = null;
                throw ex;
            }
            return 0;
        }

        /// <summary>
        /// 创建回话，优先8820C 后CMW500
        /// </summary>
        /// <returns>
        /// 0：打开回话失败
        /// -1：不存在的设备或资源
        /// 1：连接成功 
        /// </returns>
        /// <exception cref="DllNotFoundException">The NI-VISA driver library cannot be found.  </exception>
        ///  <exception cref="EntryPointNotFoundException">A required operation in the NI-VISA driver library cannot be found.</exception>
        public int Connect()
        {
            connectDeviceInfo = GetFastConnectDevice();
            if (connectDeviceInfo == null)
            {                
                log?.i(TAG, "Not Found Device!!");
                return 0;
            }
            isFinish = false;
            try
            {
                messageBasedSession = (MessageBasedSession) resourceManager.Open(connectDeviceInfo.ID);
            }
            catch (VisaException ex)
            {
                log?.e("VISA Communication",ex.Message);
                connectDeviceInfo = null;
                return 0;
            }
            catch (ArgumentException ex)
            {
                log?.e("VISA Communication",ex.Message);
                connectDeviceInfo = null;
                return -1;
            }catch(Exception ex)
            {
                connectDeviceInfo = null;
                throw ex;
            }
            return 1;
        }

        private void SetSessionTimeout(int timeout) {
            if (messageBasedSession != null)
            {
                messageBasedSession.Timeout = timeout;
            }
        }

        public int Connect(String address)
        {
            isFinish = false;
            try
            {                
                messageBasedSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(address);
                delayTime = 200;
                messageBasedSession.Timeout = 3000;
            }
            catch (VisaException ex)
            {
                log?.e("VISA Communication",ex.Message);
                connectDeviceInfo = null;
                return 0;
            }
            catch (ArgumentException ex)
            {
                log?.e("VISA Communication",ex.Message);
                connectDeviceInfo = null;
                return -1;
            }
            catch (Exception ex)
            {
                connectDeviceInfo = null;
                throw ex;
            }
            return 1;
        }

        public void SetTimeout(int timeout) {
            if (messageBasedSession != null)
            {
                messageBasedSession.Timeout = timeout;
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns>
        /// 0：打开回话失败
        /// -1：不存在的设备或资源
        /// 1：连接成功 
        /// </returns>
        public int OpenAT()
        {
            List<CommunicateUtils.DeviceInfo> devices = CommunicateUtils.FindPhoneSerialResources();
            foreach (CommunicateUtils.DeviceInfo device in devices)
            {
                if (device.DeviceType == null )
                {
                    continue;
                }
                if ((!device.DeviceType.Contains("IMEI") && !device.DeviceType.Contains("MEID:") &&
                     !device.DeviceType.Contains("OK")))
                {
                    continue;
                }
                try
                {
                    messageBasedSession = (MessageBasedSession)resourceManager.Open(device.ID);
                    isFinish = false;
                    connectDeviceInfo = device;
                    delayTime = 50;
                    return 1;
                }
                catch (VisaException ex)
                {
                    connectDeviceInfo = null;
                    log?.d(TAG,"OpenAT Method: " + ex.Message);
                    return 0;
                }
                catch (ArgumentException ex)
                {
                    connectDeviceInfo = null;
                    log?.d(TAG,"OpenAT Method: " + ex.Message);
                    return -1;
                }
                catch (Exception ex)
                {
                    connectDeviceInfo = null;
                    //log?.PutMessage("OpenAT Method: " + ex.Message);
                    throw ex;
                }
            }
            return 0;
        }



        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="data">字符串</param>
        /// <param name="isDelay">是否做延时操作</param>
        public void Write(string data, bool isDelay = true)
        {           
            if (messageBasedSession == null)
            {
                return;
            }
            try
            {
                string value = null;
                messageBasedSession.Write(data + ANRITSUENDFLAG);
                //CommunicateUtils.DeviceInfo device = new CommunicateUtils.DeviceInfo();
                //value = messageBasedSession.Query("*OPC\n");
                //if (value.Contains("1"))
                //{
                //    isFinish = true;
                //}

                //打印Log
                log?.i(TAG, LogFormat(data));
                if (isDelay)
                {
                    Thread.Sleep(delayTime);                  
                }
            }
            catch (VisaException ex)
            {
                if (ex.ErrorCode == VisaStatusCode.ErrorTimeout)
                {
                    log?.w(TAG, ex.Message);
                    Write(data, isDelay);                  
                }
                else
                {
                    log?.e(TAG, ex);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private String LogFormat(String log)
        {
            if (log == null)
            {
                return "---> " ;
            }
            return String.Format("[--->{0}] {1}" ,messageBasedSession.ResourceName,log);
        }

        private  String LogInputFormat(String log)
        {
            if (log == null)
            {
                return  "<--- ";
            }
            return String.Format("[<---{0}] {1}", messageBasedSession.ResourceName, log);
        }

       
        /// <summary>
        /// 写入并读取数据
        /// </summary>
        /// <param name="data">向设备发送的数据</param>
        /// <param name="isDelay">是否延时</param>
        /// <param name="recursiveNum">递归次数</param>
        /// <returns> 从设备读取的字符串</returns>
        public string Query(string data,bool isDelay = true,int recursiveNum =0)
        {
            if (messageBasedSession == null)
            {
                return null;
            }
            string result = "";
            log?.i(TAG,LogFormat(data));
            try
            {              
                result = messageBasedSession.Query(data + ANRITSUENDFLAG);
                //messageBasedSession.Write(data + ANRITSUENDFLAG);
                log?.i(TAG,LogInputFormat(result.Trim()));
                if (isDelay)
                {
                    Thread.Sleep(delayTime);
                }
                //result = messageBasedSession.ReadString();
            }
            catch (VisaException ex)
            { 
                if (ex.ErrorCode == VisaStatusCode.ErrorTimeout)
                {
                    if (recursiveNum > 10) {
                        throw ex;
                    }
                    log?.w(TAG, ex.Message);
                    result = Query(data, isDelay,recursiveNum + 1);
                }
                else {
                    log?.e(TAG, ex);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        
		/// <summary>
        /// 写入并读取数据
        /// </summary>
        /// <param name="data">向设备发送的数据</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="isDelay">是否延时</param>
        /// <returns></returns>
        public string Query(string data, int timeout, bool isDelay = true)
        {
            if (messageBasedSession == null)
            {
                return null;
            }
            //string Value = "";
            //Value = messageBasedSession.Query(data);
            //if(Value.Contains("1"))
            //{
            //    isFinish = true;
            //}
                
            
            string result = "";
            int loop = 0;
            int loopcount = 10;
            messageBasedSession.Timeout = timeout; // NR某些读值命令，返回值较多，超时时间需加长
            if (data.Contains("*OPC?") == true)
            {
                loopcount = 30;  // OPC查询，循环次数加大到30次
            }
            // 两条命令合并的查询命令，把写入命令拆出来单独发，避免超时重发写命令导致的NR注册问题
            foreach (string cmd in data.Split(';'))
            {
                if (cmd.Contains("?"))
                {
                    do
                    {
                        try
                        {
                            log?.i(TAG, LogFormat(cmd));
                            result = messageBasedSession.Query(cmd + ANRITSUENDFLAG);
                            log?.i(TAG, LogInputFormat(result.Trim()));
                            if (isDelay)
                            {
                                Thread.Sleep(delayTime);
                            }
                            break;
                        }
                        catch (VisaException ex)
                        {
                            log?.d(TAG, "Query Method: " + ex.Message);
                            //messageBasedSession.Clear();
                            //if (!isFinish)
                            //{
                            //    result = Query(data, isDelay);
                            //}
                        }
                        catch (Exception ex)
                        {
                            //messageBasedSession.Clear();
                            throw ex;
                        }
                        loop++;
                    } while (loop < loopcount);
                }
                else
                {
                    Write(cmd);
                }
            }
            return result.Trim();  // 最终返回结果去掉多余的换行符
        }

        public string Read()
        {
            if (messageBasedSession == null)
            {
                return null;
            }
            return messageBasedSession.ReadString();
                
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        public void DisConnect()
        {
            if (messageBasedSession != null)
            {
                return;
            }
            //释放连接
            //messageBasedSession.Dispose();
        }
    }


}