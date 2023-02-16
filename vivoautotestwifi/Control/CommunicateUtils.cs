using System;
using System.Collections.Generic;
using System.Threading;
using NationalInstruments.VisaNS;

namespace vivoautotestwifi.Control
{
    public class CommunicateUtils
    {

      
        /// <summary>
        /// 获取设备的类型
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>设备类型</returns>
        public static  HardwareInterfaceType GetResourceType(string resource)
        {
            if (resource == null)
            {
                throw new Exception("Parameter is empty");
            }
            HardwareInterfaceType intType = HardwareInterfaceType.Gpib;
            short intNum = 0;
            ResourceManager.GetLocalManager().ParseResource(resource, out intType, out intNum);
            return intType;

        }

        

        public static List<DeviceInfo> FindPhoneSerialResources()
        {
            string[] resources = ResourceManager.GetLocalManager().FindResources("ASRL[0-9]*::?*INSTR");
            if (resources.Length == 0)
            {
                return null;
            }
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            foreach (string _id in resources)
            {
                DeviceInfo dev = new DeviceInfo
                {
                    ID = _id,
                    type = GetResourceType(_id)
                };
                MessageBasedSession messageBasedSession = null;
                try
                {
                    messageBasedSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(_id);
                    int i = 0;
                    messageBasedSession.Write("AT+BKMEID=0\n");

                    do
                    {
                        dev.DeviceType = messageBasedSession.ReadString(1024);
                        i += 1;
                        if (i > 5)
                        {
                            break;
                        }
                        Thread.Sleep(500);
                        if (dev.DeviceType == null)
                        {
                            continue;
                        }
                        if (dev.DeviceType.ToUpper().Trim().Contains("IMEI"))
                        {
                            break;
                        }
                        if (dev.DeviceType.ToUpper().Trim().Contains("MEID:"))
                        {
                            break;
                        }
                        if (dev.DeviceType.ToUpper().Trim().Contains("OK"))
                        {
                            break;
                        }
                    } while (true);

                }
                catch (Exception)
                {
                    
                    dev.DeviceType = "";
                }
                finally
                {
                    if (messageBasedSession != null)
                    {
                       // messageBasedSession.Dispose();
                    }
                }
               
                deviceInfos.Add(dev);
            }
            return deviceInfos;
        }


        #region FindResources 过滤关键字
        //"?*"
        //"GPIB?*"
        //"GPIB?*INSTR"
        //"GPIB?*INTFC"
        //"GPIB?*SERVANT"
        //"GPIB-VXI?*"
        //"GPIB-VXI?*INSTR"
        //"GPIB-VXI?*MEMACC"
        //"GPIB-VXI?*BACKPLANE"
        //"PXI?*INSTR"
        //"ASRL?*INSTR"
        //"VXI?*"
        //"VXI?*INSTR"
        //"VXI?*MEMACC"
        //"VXI?*BACKPLANE"
        //"VXI?*SERVANT"
        //"USB?*"
        //"FIREWIRE?*"
        #endregion
        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="needSerial">是否需要串口设备</param>
        public static List<DeviceInfo> FindResources(bool needSerial = true)
        {
            string[] resources = null;
            try
            {
                 resources = ResourceManager.GetLocalManager().FindResources("?*");
            }
            catch(Exception ex)
            {
                Log.GetInstance().d("CommunicateUtils", ex.ToString());
                return null;
            }
            if (resources.Length== 0)
            {
                return null;
            }
            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            foreach (string _id in resources){
                if (!needSerial)
                {
                    if (GetResourceType(_id) == HardwareInterfaceType.Serial)
                    {
                        continue;
                    }
                }
                DeviceInfo deviceInfo = new DeviceInfo
                {
                    ID = _id,
                    type = GetResourceType(_id)
                };
                
                MessageBasedSession messageBasedSession =null;
                try
                {
                    Session session = ResourceManager.GetLocalManager().Open(_id);
                    if (session is MessageBasedSession)
                    {
                        messageBasedSession = (MessageBasedSession)session;

                        if (deviceInfo.type == HardwareInterfaceType.Gpib)
                        {

                            deviceInfo.DeviceType = messageBasedSession.Query("*IDN?\n");
                        }
                        else
                        {
                            deviceInfo.DeviceType = messageBasedSession.Query("*IDN?\n");
                        }
                    }
                    else {
                        deviceInfo.DeviceType = "";
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstance()?.w("VISA Communication", ex.ToString());
                    deviceInfo.DeviceType = "";
                }
                finally
                {
                    if (messageBasedSession != null)
                    {
                        //messageBasedSession.Dispose();
                    }
                }
                deviceInfos.Add(deviceInfo);
            }
            return deviceInfos;
        }

        /// <summary>
        /// 查找NI端口
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<String> FindResources(String filter=null)
        {
            List<String> result = new List<String>();
            #region VisaNS
            try
            {
                String pattern;
                if (filter == null)
                    pattern = "(ASRL|GPIB|TCPIP|USB)?*";
                else
                    pattern = filter;
                string[] resources = ResourceManager.GetLocalManager().FindResources(pattern);
                if (resources.Length == 0)
                {
                    Log.GetInstance()?.d("CommunicateUtils", "There was no resource found on your system");
                }
                foreach (string s in resources)
                {
                    HardwareInterfaceType intType;
                    short intNum;
                    ResourceManager.GetLocalManager().ParseResource(s, out intType, out intNum);
                    result.Add(s);
                }
            }
            catch (VisaException)
            {
                // Don't do anything
            }
            catch (Exception ex)
            {
                Log.GetInstance()?.d("CommunicateUtils", ex.Message);
            }
            #endregion
            return result;
        }

        public class DeviceInfo
        {
            /// <summary>
            /// 设备连接ID，可用于NI连接
            /// </summary>
            public string ID {
                set;
                get;
            }
            /// <summary>
            /// 设备连接介质的类型，如USB GPID
            /// </summary>
            public HardwareInterfaceType type
            {
                set;
                get;
            }
            /// <summary>
            /// 终端设备的*IDN返回的字符串，如CMW500 8820C等
            /// </summary>
            public string DeviceType
            {
                set;
                get;
            }
        }
    }

    
}
