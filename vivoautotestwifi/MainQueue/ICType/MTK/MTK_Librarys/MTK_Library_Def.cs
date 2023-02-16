    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vivoautotestwifi.MainQueue.ICType.MTK.MTK_Librarys
{
    public partial class MTK_Library
    {
        public int ConnetWithUSB(String logSavePath, out string ErrorMsg)
        {
            //SPMETA_DLL_PATH = metacore;
            ErrorMsg = string.Empty;
            try
            {
                //int iRet = 0;
                int m_iStop = 0;
                int m_nDUTID = 0;

                //string logSavePath = @"D:\\logSavePath"; //注意:LOGSAVEPATH一定要是已经存在的文件夹,不然连接不上Meta
                string BROMPID = "VID_0E8D&PID_2007";
                string PRELOADERPID = "VID_0E8D&PID_2000 VID_22D9&PID_0006";
                string KENERLPID =
                    "VID_0E8D&PID_2040&MI_02 VID_0E8D&PID_2007 VID_0E8D&PID_2006&MI_02 VID_0BB4&PID_0005&MI_02 VID_1004&PID_6000 VID_22D9&PID_0006 VID_0E8D&PID_202D&MI_01 VID_0E8D&PID_2026&MI_02 VID_0E8D&PID_200E&MI_01";

                if (!Directory.Exists(logSavePath))
                    Directory.CreateDirectory(logSavePath);

                METAAPP_HIGH_LEVEL_RESULT result;

                METAAPP_CONN_STTING_T m_Setting = new METAAPP_CONN_STTING_T();

                METAAPP_CONNCT_CNF_T pCnf = new METAAPP_CONNCT_CNF_T();

                /*****common setting******/
                m_Setting.autoScanPort = true; //是否自动扫口

                m_Setting.stopFlag = Marshal.AllocHGlobal(Marshal.SizeOf(m_iStop)); //stop标志，用于上层tool 控制底层stop的操作
                Marshal.StructureToPtr(m_iStop, m_Setting.stopFlag, true);


                m_Setting.uTimeOutMs =
                    120000; //scan port timeout，preloader handshake，connect to SP/Modem meta 的timeout 设定，单位是ms
                m_Setting.connectType = METAAPP_CONN_TYPE_E.METAAPP_CONN_CONNCET_BY_USB; //connect 方式的设定UART, USB, WIFI
                m_Setting.connectMode = METAAPP_CONN_MODE_E.METAAPP_CONN_BOOT_META_MODE;
                //METAAPP_CONN_MODE_E.METAAPP_CONN_ALREADY_IN_META_MODE; //三种模式，正常boot mode， already in meta mode, ATM mode


                /*****Boot Setting*******/
                //m_Setting.bootSetting.authHandle = IntPtr.Zero; //security boot 相关设定，可以不设置, 当前MetaApp是不支持security boot
                //m_Setting.bootSetting.scertHandle = IntPtr.Zero; //security boot 相关设定，可以不设置, 当前MetaApp是不支持security boot
                m_Setting.bootSetting.bEnableAdbDevice = false; //是否要开启adb口，如要开kernel + adb，那么这里设置为true
                m_Setting.bootSetting.uPortNumber = 0; //preloader comport, 如果不选择AutoScan，那么这里一要设定好对应的poreloader comport
                m_Setting.bootSetting.uMDMode = 0xc000; //选择modem mode, 决定哪个modem将被boot，新平台只有一个modem，这里设置默认值
                /****选择modem mode****/
                // 6771/6763/6765/6762/6739, 这里固定配置为0xc000即可
                // 6738/6750/6755/6757/6797, 这里固定配置0u
                // 6735/6737/6753 这里要根据具体的modem index，modem type 来计算
                // world phone boot up configuration
                // 0	-	7	Bits: modem_type
                // 8	-	13 Bits: modem index
                // 14 -	15 Bits: reserved
                /*
                MDIndex bit map table(8-13 Bits)
                1(H)   -   1   -   1   -   1   -   1   -   1   -   1   -   1(L)
                                   MD6     MD5     MD4     MD3     MD2     MD1
                Example: MDIndex = 16(0001 0000) will enable: MD5
                详细参考MultiATE code： CATEMainUI::CalcWorldphoneMDMode
                */




                /*****AP Setting******/
                m_Setting.connectSetting.kernelComPort = 0; //如果选择AutoScan，那么这里可以不设置，否则一定要设置好对应的kernel 端口
                m_Setting.connectSetting.bDbFileFromDUT =
                    true; //是否从DUT中导出db 文件，true 即为从DUT中导出，导出的db文件将保存在C:\DbPath\线程号, Modem db file 默认导出的是odb
                m_Setting.connectSetting.pApDbFilePath =
                    @""; //如果bDbFileFromDUT 设置为false，那么这里这里要附上详细的file fullname路径,否则可无需设置
                m_Setting.connectSetting.pMdDbFilePath =
                    @""; //如果bDbFileFromDUT 设置为false，那么这里这里要附上详细的modem1 file fullname路径,否则可无需设置
                m_Setting.connectSetting.pMdDbFilePath1 =
                    @""; //这个主要是运用在有两个modem 的老平台项目,即区分LWG,LTG modem db，新的平台可以无视这个配置


                /****Log Setting******/
                m_Setting.logSetting.enableDllLog =
                    true; //是否要打印dll log，true的话会打印Metacore dll log（即过去的Spmeta/modem meta dll log）
                m_Setting.logSetting.enableUartLog = false; //这个功能目前是无效的，无法通过USB端口吐出UART log，无需配置
                m_Setting.logSetting.enablePcUsbDriverLog = false; //这个功能目前无效，无需配置
                m_Setting.logSetting.iMDLoggEnable = 1; //抓取modem log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.iMobileLogEnable = 1; //抓取mobile log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.IP =
                    @""; //"192.168.43.108";  //wifi 通信下要抓取modem、mobile log 就需要设定通信的ip 地址，目前这个地址在 SW 端是固定的。
                m_Setting.logSetting.logSavePath = logSavePath; ////log要保存的路径，运行生成的所有log都会保存在这个路径下面对应的线程号里
                m_Setting.logSetting.pcbSn = "PCB_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");  //这个一般设置SN+time，目的是为了保证所有的log都是相同的名字
                m_Setting.logSetting.iConnsysLogEnable = 0;

                /****Filter Setting******/
                m_Setting.filterSetting.brom = BROMPID; //brom 方式做preloader handshake，那么需要配置，设定preloader filter即可
                m_Setting.filterSetting.preloader = PRELOADERPID; //
                m_Setting.filterSetting.kernel = KENERLPID; //

                //这个API会连接AP meta，并且会进行AP db 的匹配check
                int len = Marshal.SizeOf(m_Setting);

                int len1 = Marshal.SizeOf(m_Setting.autoScanPort);//4
                int len2 = Marshal.SizeOf(m_Setting.stopFlag); //4
                //UInt32 len3 = Marshal.SizeOf();/////4?
                //int len4 = Marshal.SizeOf(m_Setting.connectMode);///////4?
                int len5 = Marshal.SizeOf(m_Setting.uTimeOutMs); //4
                int len6 = Marshal.SizeOf(m_Setting.bootSetting); //12
                int len7 = Marshal.SizeOf(m_Setting.connectSetting);//20
                int len8 = Marshal.SizeOf(m_Setting.logSetting); //C#432   C++436
                int len9 = Marshal.SizeOf(m_Setting.filterSetting); //12


                result = METAAPP_ConnectTargetAllInOne_r(m_nDUTID, ref m_Setting, ref pCnf, (UInt32)Marshal.SizeOf(m_Setting));
                if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                {
                    ErrorMsg = "Connect AP meta OK.";
                }
                else
                {
                    ErrorMsg = "Connect AP meta Failed " + Convert.ToInt16(result);
                    Log.GetInstance().i("MT6635", ErrorMsg);
                    return -1;
                }
                Log.GetInstance().i("MT6635", ErrorMsg);
                m_hMeta = pCnf.hMDHandle;
                //LogUtils.WriteLog(String.Format("DutID = {0}, MetaHandle = {1}.", m_nDutID, m_hMeta));

                //ap to modem
                //result = METAAPP_ApToModemAllInOne_r(m_nDUTID);
                //if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                //{
                //    ErrorMsg = "AP to Modem Meta OK.";
                //    Log.GetInstance().i("MT6635", ErrorMsg);
                //}
                //else
                //{
                //    ErrorMsg = "AP to Modem Meta Failed" + Convert.ToInt16(result);
                //    Log.GetInstance().i("MT6635", ErrorMsg);
                //    return -1;
                //}

                //UInt32 value = 999, u4Addr = 0, u4value = 0;
                //int ms = 0;

                //ms = SP_META_WIFI_OPEN_r(m_hMeta, (UInt32)(15000));
                //ms = SP_META_WiFi_GetChipVersion_r(m_hMeta, 18000, ref value);
                //ms = SP_META_WiFi_readMCR32_r(m_hMeta, 1200, u4Addr, ref u4value);
                //m_wifiChipVersion = u4value & 0xFFFF;
                //if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
                //{
                //}
                //else
                //{

                //}
                //if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
                //{
                //    NVRAM_ACCESS_STRUCT wifinvram = new NVRAM_ACCESS_STRUCT();

                //    byte[] pszBufferRaw = new byte[512];
                //    pszBufferRaw[0] = (0x6628 >> 8) & 0x00ff;
                //    pszBufferRaw[1] = 0x6628 & 0x00ff;

                //    wifinvram.data = System.Text.Encoding.Default.GetString(pszBufferRaw);
                //    wifinvram.dataLen = 2;
                //    wifinvram.dataOffset = 0x3c;
                //    SP_META_WiFi_WriteNVRAM_r(m_hMeta, 1200, ref wifinvram);
                //}
                //ENUM_CFG_SRC_TYPE_T buffType = new ENUM_CFG_SRC_TYPE_T();
                //ms = SP_META_WiFi_QueryConfig_r(m_hMeta, 5000, ref buffType);
                //ms = SP_META_WiFi_setTestMode_r(m_hMeta, 5000);
                //ms = SP_META_WiFi_switchAntenna_r(m_hMeta, 5000, 0);

                return 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = "Connect Meta Failed throw an exception" + ex.Message;
                Log.GetInstance().i("MT6635", ErrorMsg);
                return -1;
                throw ex;
            }
        }


        /// <summary>
        /// 使用wifi连接手机进入Meta模式
        /// </summary>
        /// <returns></returns>
        public int ConnectWithWifi(String logSavePath, int a, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            try
            {
                //int iRet = 0;
                int m_iStop = 0;
                int m_nDUTID = 0;

                //string LOGSAVEPATH = @"D:\\logSavePath"; //注意:LOGSAVEPATH一定要是已经存在的文件夹,不然连接不上Meta
                string BROMPID = "VID_0E8D&PID_2007";
                string PRELOADERPID = "VID_0E8D&PID_2000 VID_22D9&PID_0006";
                string KENERLPID =
                    "VID_0E8D&PID_2040&MI_02 VID_0E8D&PID_2007 VID_0E8D&PID_2006&MI_02 VID_0BB4&PID_0005&MI_02 VID_1004&PID_6000 VID_22D9&PID_0006 VID_0E8D&PID_202D&MI_01 VID_0E8D&PID_2026&MI_02 VID_0E8D&PID_200E&MI_01";
                if (!Directory.Exists(logSavePath))
                    Directory.CreateDirectory(logSavePath);
                METAAPP_HIGH_LEVEL_RESULT result;
                METAAPP_CONN_STTING_T m_Setting = new METAAPP_CONN_STTING_T();
                METAAPP_CONNCT_CNF_T pCnf = new METAAPP_CONNCT_CNF_T();
                /*****common setting******/
                m_Setting.autoScanPort = false; //是否自动扫口

                m_Setting.stopFlag = Marshal.AllocHGlobal(Marshal.SizeOf(m_iStop)); //stop标志，用于上层tool 控制底层stop的操作
                Marshal.StructureToPtr(m_iStop, m_Setting.stopFlag, true);


                m_Setting.uTimeOutMs =
                    30000; //scan port timeout，preloader handshake，connect to SP/Modem meta 的timeout 设定，单位是ms
                m_Setting.connectType = METAAPP_CONN_TYPE_E.METAAPP_CONN_CONNCET_BY_WIFI; //connect 方式的设定UART, USB, WIFI
                m_Setting.connectMode =
                    METAAPP_CONN_MODE_E.METAAPP_CONN_ATM_MODE; //三种模式，正常boot mode， already in meta mode, ATM mode


                /*****Boot Setting*******/
                //m_Setting.bootSetting.authHandle = IntPtr.Zero; //security boot 相关设定，可以不设置, 当前MetaApp是不支持security boot
                //m_Setting.bootSetting.scertHandle = IntPtr.Zero; //security boot 相关设定，可以不设置, 当前MetaApp是不支持security boot
                m_Setting.bootSetting.bEnableAdbDevice = false; //是否要开启adb口，如要开kernel + adb，那么这里设置为true
                m_Setting.bootSetting.uPortNumber = 0; //preloader comport, 如果不选择AutoScan，那么这里一要设定好对应的poreloader comport
                m_Setting.bootSetting.uMDMode = 0xc000; //选择modem mode, 决定哪个modem将被boot，新平台只有一个modem，这里设置默认值
                /****选择modem mode****/
                // 6771/6763/6765/6762/6739, 这里固定配置为0xc000即可
                // 6738/6750/6755/6757/6797, 这里固定配置0u
                // 6735/6737/6753 这里要根据具体的modem index，modem type 来计算
                // world phone boot up configuration
                // 0	-	7	Bits: modem_type
                // 8	-	13 Bits: modem index
                // 14 -	15 Bits: reserved
                /*
                MDIndex bit map table(8-13 Bits)
                1(H)   -   1   -   1   -   1   -   1   -   1   -   1   -   1(L)
                                   MD6     MD5     MD4     MD3     MD2     MD1
                Example: MDIndex = 16(0001 0000) will enable: MD5
                详细参考MultiATE code： CATEMainUI::CalcWorldphoneMDMode
                */




                /*****AP Setting******/
                m_Setting.connectSetting.kernelComPort = 512; //如果选择AutoScan，那么这里可以不设置，否则一定要设置好对应的kernel 端口
                m_Setting.connectSetting.bDbFileFromDUT =
                    true; //是否从DUT中导出db 文件，true 即为从DUT中导出，导出的db文件将保存在C:\DbPath\线程号, Modem db file 默认导出的是odb
                m_Setting.connectSetting.pApDbFilePath =
                    @""; //如果bDbFileFromDUT 设置为false，那么这里这里要附上详细的file fullname路径,否则可无需设置
                m_Setting.connectSetting.pMdDbFilePath =
                    @""; //如果bDbFileFromDUT 设置为false，那么这里这里要附上详细的modem1 file fullname路径,否则可无需设置
                m_Setting.connectSetting.pMdDbFilePath1 =
                    @""; //这个主要是运用在有两个modem 的老平台项目,即区分LWG,LTG modem db，新的平台可以无视这个配置


                /****Log Setting******/
                m_Setting.logSetting.enableDllLog =
                    true; //是否要打印dll log，true的话会打印Metacore dll log（即过去的Spmeta/modem meta dll log）
                m_Setting.logSetting.enableUartLog = false; //这个功能目前是无效的，无法通过USB端口吐出UART log，无需配置
                m_Setting.logSetting.enablePcUsbDriverLog = false; //这个功能目前无效，无需配置
                m_Setting.logSetting.iMDLoggEnable = 1; //抓取modem log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.iMobileLogEnable = 1; //抓取mobile log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.IP =
                    @""; //"192.168.43.108";  //wifi 通信下要抓取modem、mobile log 就需要设定通信的ip 地址，目前这个地址在 SW 端是固定的。
                m_Setting.logSetting.logSavePath = logSavePath; ////log要保存的路径，运行生成的所有log都会保存在这个路径下面对应的线程号里
                m_Setting.logSetting.pcbSn = "PCB_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"); //这个一般设置SN+time，目的是为了保证所有的log都是相同的名字


                /****Filter Setting******/
                m_Setting.filterSetting.brom = BROMPID; //brom 方式做preloader handshake，那么需要配置，设定preloader filter即可
                m_Setting.filterSetting.preloader = PRELOADERPID; //
                m_Setting.filterSetting.kernel = KENERLPID; //



                //这个API会连接AP meta，并且会进行AP db 的匹配check

                result = METAAPP_ConnectTargetAllInOne_r(m_nDUTID, ref m_Setting, ref pCnf, (UInt32)Marshal.SizeOf(m_Setting));
                if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                {
                    ErrorMsg = "Connect AP meta OK.\r\n";
                }
                else
                {
                    ErrorMsg = "Connect AP meta Failed ," + Convert.ToInt16(result);
                    //return -1;
                }

                m_hMeta = pCnf.hMDHandle;
                //LogUtils.WriteLog(String.Format("DutID = {0}, MetaHandle = {1}.", m_nDutID, m_hMeta));

                //ap to modem
                result = METAAPP_ApToModemAllInOne_r(0);
                if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                {
                    ErrorMsg = "AP to Modem Meta OK.\r\n";
                }
                else
                {
                    ErrorMsg = "AP to Modem Meta Failed," + Convert.ToInt16(result);
                    return -1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = "Connect Meta Failed throw an exception" + ex.Message;
                return -1;
                throw ex;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect(ref string errMsg)
        {
            METAAPP_HIGH_LEVEL_RESULT result;
            METAAPP_DISCONNECT_META_T disinfo = new METAAPP_DISCONNECT_META_T();
            METAAPP_DISCONNECT_META_PARA_E disenum = METAAPP_DISCONNECT_META_PARA_E.METAAPP_Disconnect_Poweroff;
            disinfo.eDisconPara = disenum;
            disinfo.bDoBackupNv = false;   //备份NV,如果有写校准参数，为true,否则false
            try
            {

                result = METAAPP_DisConnectMeta_Ex_r(m_nDutID, disinfo, (UInt32)Marshal.SizeOf(disinfo));
                if (result != METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS &&
                    result != METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_CLEAN_BOOT_FAIL)
                    //LogUtils.WriteLog(String.Format("Disconnect fail({0}).", result));

                    m_nDutID = 0;
                m_hMeta = 0;


                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                return false;
            }
        }


        public int META_Customer_Func_r(UInt32 ms_timeout, ref string data)
        {
            string data_in = data;
            int errMsgIndex = 0;
            errMsgIndex = META_Customer_Func_r(m_hMeta, ms_timeout, data, data.Length, ref data, 1024);
            return errMsgIndex;
        }

        public int SP_META_Customer_Func_r(UInt32 ms_timeout, ref string data, int type, ref byte dummy)
        {
            string data_in = data;
            byte dummy_in = dummy;
            int errMsgIndex = 0;
            errMsgIndex = SP_META_Customer_Func_r(m_hMeta, ms_timeout, data_in, data_in.Length, type, dummy_in, ref dummy, ref data, 1024);

            return errMsgIndex;
        }

        public int SP_META_GetTargetBuildProp_r(ref BUILD_PROP_REQ_S pReq, ref BUILD_PROP_CNF_S pCnf)
        {
            int errMsgIndex = 0;
            errMsgIndex = SP_META_GetTargetBuildProp_r(m_hMeta, ref pReq, ref pCnf);
            return errMsgIndex;
        }

        public int SP_META_CancelAllBlockingCall_r()
        {
            int errMsgIndex = 0;
            errMsgIndex = SP_META_CancelAllBlockingCall_r(m_hMeta);
            return errMsgIndex;
        }

        public int SP_META_NVRAM_GetRecLen(string LID, int len)
        {
            int errMsgIndex = 0;
            errMsgIndex = SP_META_NVRAM_GetRecLen(LID, ref len);
            return errMsgIndex;
        }

        public int SP_META_NVRAM_Read_r(ref AP_FT_NVRAM_READ_REQ req, ref AP_FT_NVRAM_READ_CNF cnf, SP_META_NVRAM_Read_CNF cb, ref short token, ref IntPtr usrData)
        {
            int errMsgIndex = 0;
            errMsgIndex = SP_META_NVRAM_Read_r(m_hMeta, ref req, ref cnf, cb, ref token, ref usrData);
            return errMsgIndex;
        }

        public int SP_META_NVRAM_Write_r(ref AP_FT_NVRAM_READ_REQ req, SP_META_NVRAM_Write_CNF cb, ref short token, ref IntPtr usrData)
        {
            int errMsgIndex = 0;
            errMsgIndex = SP_META_NVRAM_Write_r(m_hMeta, ref req, cb, ref token, ref usrData);
            return errMsgIndex;
        }

        //public int SP_META_NVRAM_GetRecLen_r(string LID, int len)
        //{
        //    int errMsgIndex = 0;
        //    errMsgIndex = SP_META_NVRAM_GetRecLen_r(m_hMeta,LID, ref len);
        //    return errMsgIndex;
        //}

        //public int META_NVRAM_Write_r(ref AP_FT_NVRAM_WRITE_REQ req, AP_FT_NVRAM_WRITE_CNF cb, ref short token, ref IntPtr usrData)
        //{

        //    int errMsgIndex = 0;
        //    errMsgIndex = META_NVRAM_Write_r(m_hMeta,ref req, cb,ref token,ref usrData);
        //    return errMsgIndex;
        //}

        //public int SP_META_NVRAM_GetRecFieldValue_r(string LID, string field, string buf, int buf_len, ref IntPtr value, int value_len)
        //{
        //    int errMsgIndex = 0;
        //    errMsgIndex = SP_META_NVRAM_GetRecFieldValue_r(m_hMeta,LID,field,buf,buf_len,ref value,value_len);
        //    return errMsgIndex;
        //}

        //public int SP_META_NVRAM_SetRecFieldValue_r(string LID, string field, string buf, int buf_len, ref IntPtr value, int value_len)
        //{
        //    int errMsgIndex = 0;
        //    errMsgIndex = SP_META_NVRAM_SetRecFieldValue_r(m_hMeta, LID, field, buf, buf_len, ref value, value_len);
        //    return errMsgIndex;
        //}


        /***********************by wzt*********************************/

        #region BT
        public int SP_META_Query_WCNDriver_Ready_r(UInt32 ms_timeout, ref QUERY_WCNDRIVER_READY_CNF pcnf)
        {
            return SP_META_Query_WCNDriver_Ready_r(m_hMeta, ms_timeout, ref pcnf);
        }

        public int SP_META_QueryIfFunctionSupportedByTarget_r(UInt32 ms_timeout, ref string query_func_name)
        {
            return SP_META_QueryIfFunctionSupportedByTarget_r(m_hMeta, ms_timeout, ref query_func_name);
        }


        public int SP_META_BT_OPEN_r(UInt32 ms_timeout)
        {
            return SP_META_BT_OPEN_r(m_hMeta, ms_timeout);
        }

        public int SP_META_BT_CLOSE_r(UInt32 ms_timeout)
        {
            return SP_META_BT_CLOSE_r(m_hMeta, ms_timeout);
        }

        public int SP_META_BTPowerOn_r(UInt32 ms_timeout)
        {
            return SP_META_BTPowerOn_r(m_hMeta, ms_timeout);
        }

        public int SP_META_BT_GetChipID_r(UInt32 ms_timeout, ref int pid)
        {
            return SP_META_BT_GetChipID_r(m_hMeta, ms_timeout, ref pid);
        }

        public int SP_META_BT_SendHCICommand_r(UInt32 ms_timeout, ref BT_HCI_COMMAND req, META_BT_HCI_CNF cb, IntPtr cb_arg, byte Cmpltcode)
        {
            return SP_META_BT_SendHCICommand_r(m_hMeta, ms_timeout, ref req, cb, cb_arg, Cmpltcode);
        }
        #endregion

        #region WiFi

        public int SP_META_WIFI_OPEN_r(UInt32 ms_timeout)
        {
            return SP_META_WIFI_OPEN_r(m_hMeta, ms_timeout);
        }

        public int SP_META_WIFI_CLOSE_r(UInt32 ms_timeout)
        {
            return SP_META_WIFI_CLOSE_r(m_hMeta, ms_timeout);
        }

        public int SP_META_WiFi_GetChipVersion_r(int ms_timeout, ref UInt32 value)
        {
            return SP_META_WiFi_GetChipVersion_r(m_hMeta, ms_timeout, ref value);
        }

        public int SP_META_WiFi_readMCR32_r(UInt32 ms_timeout, UInt32 offset, ref UInt32 value)
        {
            return SP_META_WiFi_readMCR32_r(m_hMeta, ms_timeout, offset, ref value);
        }

        public int SP_META_WiFi_HQA_GetChipCapability_r(UInt32 ms_timeout, ref CapBuffer CapBuffer, ref UInt32 CapBufferLength)
        {
            return SP_META_WiFi_HQA_GetChipCapability_r(m_hMeta,ms_timeout,ref CapBuffer,ref CapBufferLength);
        }

        public int SP_META_WiFi_WriteNVRAM_r(UInt32 mstimeout, ref NVRAM_ACCESS_STRUCT preq)
        {
            return SP_META_WiFi_WriteNVRAM_r(m_hMeta, mstimeout, ref preq);
        }

        public int SP_META_WiFi_ReadNVRAM_r(UInt32 mstimeout, ref NVRAM_ACCESS_STRUCT preq)
        {
            return SP_META_WiFi_ReadNVRAM_r(m_hMeta, mstimeout, ref preq);
        }

        public int SP_META_WiFi_setTestMode_r(UInt32 ms_timeout)
        {
            return SP_META_WiFi_setTestMode_r(m_hMeta, ms_timeout);
        }

        public int SP_META_WiFi_QueryAntSwap_r(UInt32 ms_timeout, ref UInt32 value)
        {
            return SP_META_WiFi_QueryAntSwap_r(m_hMeta, ms_timeout, ref value);
        }

        public int SP_META_WiFi_SetAntSwap_r(UInt32 ms_timeout, UInt32 value)
        {
            return SP_META_WiFi_SetAntSwap_r(m_hMeta, ms_timeout, value);
        }

        public int SP_META_WiFi_switchAntenna_r(UInt32 ms_timeout, UInt32 value)
        {
            return SP_META_WiFi_switchAntenna_r(m_hMeta, ms_timeout, value);
        }

        public int SP_META_WiFi_QueryConfig_r(UInt32 ms_timeout, ref ENUM_CFG_SRC_TYPE_T buffType)
        {
            return SP_META_WiFi_QueryConfig_r(m_hMeta, ms_timeout, ref buffType);
        }

        public int SP_META_WiFi_queryThermoInfo_r(UInt32 ms_timeout, ref Int32 pi4Enable, ref UInt32 pu4RawVal)
        {
            return SP_META_WiFi_queryThermoInfo_r(m_hMeta, ms_timeout, ref pi4Enable, ref pu4RawVal);
        }

        public int SP_META_WiFi_getTemperatureSensorResult_r(UInt32 ms_timeout, ref UInt32 value)
        {
            return SP_META_WiFi_getTemperatureSensorResult_r(m_hMeta, ms_timeout, ref value);
        }

        public int SP_META_WiFi_setNss_r(UInt32 ms_timeout, UInt32 value)
        {
            return SP_META_WiFi_setNss_r(m_hMeta, ms_timeout, value);
        }

        public int SP_META_WiFi_setJMode_r(UInt32 ms_timeout, int nMode)
        {
            return SP_META_WiFi_setJMode_r(m_hMeta, ms_timeout, nMode);
        }

        public int SP_META_WiFi_setBandwidthEx_r(UInt32 ms_timeout, UInt32 nChBandwidth, UInt32 nDataBandwidth, UInt32 nPrimarySetting)
        {
            return SP_META_WiFi_setBandwidthEx_r(m_hMeta, ms_timeout, nChBandwidth, nDataBandwidth, nPrimarySetting);
        }


        public int SP_META_WiFi_setChannel_r(UInt32 ms_timeout, int channelfreq)
        {
            return SP_META_WiFi_setChannel_r(m_hMeta, ms_timeout, channelfreq);
        }

        public int SP_META_WiFi_setRate_r(UInt32 ms_timeout, UInt32 Rate)
        {
            return SP_META_WiFi_setRate_r(m_hMeta, ms_timeout, Rate);
        }

        public int SP_META_WiFi_setGuardinterval_r(UInt32 ms_timeout, UInt32 inerval)
        {
            return SP_META_WiFi_setGuardinterval_r(m_hMeta, ms_timeout, inerval);
        }

        public int SP_META_WiFi_setModeSelect_r(UInt32 ms_timeout, UInt32 inerval)
        {
            return SP_META_WiFi_setModeSelect_r(m_hMeta, ms_timeout, inerval);
        }

        public int SP_META_WiFi_setTXPath_r(UInt32 ms_timeout, UInt32 value)
        {
            return SP_META_WiFi_setTXPath_r(m_hMeta, ms_timeout, value);
        }

        public int SP_META_WiFi_setPacketTxEx_r(UInt32 ms_timeout, ref WIFI_TX_PARAM_T txparam)
        {
            return SP_META_WiFi_setPacketTxEx_r(m_hMeta, ms_timeout, ref txparam);
        }

        public int SP_META_WiFi_setRXPath_r(UInt32 ms_timeout, UInt32 value)
        {
            return SP_META_WiFi_setRXPath_r(m_hMeta, ms_timeout, value);
        }

        public int SP_META_WiFi_setRxTest_r(UInt32 ms_timeout)
        {
            return SP_META_WiFi_setRxTest_r(m_hMeta, ms_timeout);
        }

        public int SP_META_WiFi_ReceivedErrorCount_r(UInt32 ms_timeout, ref UInt32 value)
        {
            return SP_META_WiFi_ReceivedErrorCount_r(m_hMeta, ms_timeout, ref value);
        }

        public int SP_META_WiFi_ReceivedOKCount_r(UInt32 ms_timeout, ref UInt32 value)
        {
            return SP_META_WiFi_ReceivedOKCount_r(m_hMeta, ms_timeout, ref value);
        }

        public int SP_META_WiFi_ReceivedRSSI_r(UInt32 ms_timeout, ref short value)
        {
            return SP_META_WiFi_ReceivedRSSI_r(m_hMeta, ms_timeout, ref value);
        }

        public int SP_META_WiFi_ReceivedRSSI1_r(UInt32 ms_timeout, ref short value)
        {
            return SP_META_WiFi_ReceivedRSSI1_r(m_hMeta, ms_timeout, ref value);
        }

        public int SP_META_WiFi_setStandBy_r(UInt32 ms_timeout)
        {
            return SP_META_WiFi_setStandBy_r(m_hMeta, ms_timeout);
        }

        public int SP_META_WiFi_HQA_OpenAdapter_r(UInt32 ms_timeout)
        {
            return SP_META_WiFi_HQA_OpenAdapter_r(m_hMeta, ms_timeout);
        }

        public int SP_META_WiFi_HQA_SetBandMode_r(UInt32 ms_timeout, int iBandMode, UInt32 iBandType, UInt32 reserved0, UInt32 reserved1)
        {
            return SP_META_WiFi_HQA_SetBandMode_r(m_hMeta,ms_timeout, iBandMode, iBandType, reserved0, reserved1);
        }

        public int SP_META_WiFi_HQA_AntSwapCap_r(UInt32 ms_timeout, UInt32 band, ref UInt32 support)
        {
            return SP_META_WiFi_HQA_AntSwapCap_r(m_hMeta,ms_timeout,band,ref support);
        }

        public int SP_META_WiFi_HQA_AntSwapSet_r(UInt32 ms_timeout, UInt32 band, UInt32 ant)
        {
            return SP_META_WiFi_HQA_AntSwapSet_r(m_hMeta, ms_timeout, band, ant);
        }

        public int SP_META_WiFi_HQA_CloseAdapter_r(UInt32 ms_timeout)
        {
            return SP_META_WiFi_HQA_CloseAdapter_r(m_hMeta, ms_timeout);
        }

        public int SP_META_WiFi_HQA_SetTxPathv2_r(UInt32 ms_timeout, byte TxPath, UInt32 band)
        {
            return SP_META_WiFi_HQA_SetTxPathv2_r(m_hMeta, ms_timeout, TxPath, band);
        }

        public int SP_META_WiFi_HQA_SetRxPathv2_r(UInt32 ms_timeout, byte RxPath, UInt32 band)
        {
            return SP_META_WiFi_HQA_SetRxPathv2_r(m_hMeta, ms_timeout, RxPath, band);
        }

        public int SP_META_WiFi_HQA_DBDCSetChannel_r(UInt32 ms_timeout, UInt32 band,
                 UInt32 CenterChannel0, UInt32 CenterChannel1/*(for 160nc)*/,
                 UInt32 SystemBW, UInt32 PerpacketBW, UInt32 PrimarySelect,
                 UInt32 Reason, UInt32 ChannelBand, UInt32 NewFreq)
        {
            return SP_META_WiFi_HQA_DBDCSetChannel_r(m_hMeta, ms_timeout, band, CenterChannel0, CenterChannel1/*(for 160nc)*/,
            SystemBW, PerpacketBW, PrimarySelect, Reason, ChannelBand, NewFreq);
        }

        public int SP_META_WiFi_HQA_ResetTxRxCounterBand_r(UInt32 ms_timeout,UInt32 band)
        {
            return SP_META_WiFi_HQA_ResetTxRxCounterBand_r(m_hMeta,ms_timeout,band);
        }

        public int SP_META_WiFi_HQA_DBDCSetTXContent_r(UInt32 ms_timeout, UInt32 band, UInt32 FC, UInt32 Dur,
           UInt32 SeqID, UInt32 RepeatRandomNormal, UInt32 TxLength, UInt32 PayloadLngth, ref string pSourceAddr, ref string pDestAddr, ref string pBSSID,
           ref string pPayloadContent, UInt32 pReserved0, UInt32 pReserved1, UInt32 pReserved2)
        {
            return SP_META_WiFi_HQA_DBDCSetTXContent_r(m_hMeta, ms_timeout, band, FC, Dur,
           SeqID, RepeatRandomNormal, TxLength, PayloadLngth, ref pSourceAddr, ref pDestAddr, ref pBSSID,
           ref pPayloadContent, pReserved0, pReserved1, pReserved2);
        }

        public int SP_META_WiFi_HQA_SetTxPowerExt_r(UInt32 ms_timeout, UInt32 TxPower, UInt32 BAND, UInt32 Channel, UInt32 ChannelBand, UInt32 WFSelect)
        {
            return SP_META_WiFi_HQA_SetTxPowerExt_r(m_hMeta, ms_timeout, TxPower, BAND, Channel, ChannelBand, WFSelect);
        }

        public int SP_META_WiFi_HQA_SetRUSettings_r(UInt32 ms_timeout, UInt32 BandIdx, UInt32 Seg0Count, UInt32 Seg1Count,ref RUSettings pRUSettingsSeg0, ref UInt32 plenSeg0, string pRUSettingsSeg1, ref UInt32 plenSeg1)
        {
            return SP_META_WiFi_HQA_SetRUSettings_r(m_hMeta, ms_timeout, BandIdx, Seg0Count, Seg1Count,ref pRUSettingsSeg0, ref plenSeg0, pRUSettingsSeg1, ref plenSeg1);
        }

        public int SP_META_WiFi_HQA_DBDCStartTX_r(UInt32 ms_timeout, UInt32 Band,
                 UInt32 PcaketCount, UInt32 Preamble, UInt32 Rate,
                 UInt32 Power, UInt32 STBC, UInt32 LDPC, UInt32 iBF,
                 UInt32 eBF, UInt32 WlanIdx, UInt32 AIFS, UInt32 isShortGI,
                 UInt32 TXPath, UInt32 Nss, ref UInt32 pReserved0,
                 ref UInt32 pReserved1, ref UInt32 pReserved2)
        {
            return SP_META_WiFi_HQA_DBDCStartTX_r(m_hMeta, ms_timeout, Band, PcaketCount, Preamble, Rate, Power, STBC, LDPC, iBF,
                    eBF, WlanIdx, AIFS, isShortGI, TXPath, Nss, ref pReserved0, ref pReserved1, ref pReserved2);
        }

        public int SP_META_WiFi_HQA_DBDCStopTX_r(UInt32 ms_timeout, UInt32 Band, ref UInt32 pReserved0, ref UInt32 pReserved1, ref UInt32 pReserved2)
        {
            return SP_META_WiFi_HQA_DBDCStopTX_r(m_hMeta, ms_timeout, Band, ref pReserved0, ref pReserved1, ref pReserved2);
        }

        public int SP_META_WiFi_HQA_DBDCStartRXExt_r(UInt32 ms_timeout, UInt32 Band, UInt32 RxPath, ref byte pMAC, UInt32 StaID, UInt32 Preamble, UInt32 LTFGI)
        {
            return SP_META_WiFi_HQA_DBDCStartRXExt_r(m_hMeta, ms_timeout, Band, RxPath, ref pMAC, StaID, Preamble, LTFGI);
        }

        public int SP_META_WiFi_HQA_GetAFactor_r(UInt32 ms_timeout, ref UInt32 FactorValue, ref UInt32 LDPCExtraSym, ref UInt32 PEDisamp, ref UInt32 TXPEValue, ref UInt32 LSIG)
        {
            return SP_META_WiFi_HQA_GetAFactor_r(m_hMeta, ms_timeout, ref FactorValue, ref LDPCExtraSym, ref PEDisamp, ref TXPEValue, ref LSIG);
        }

        public int SP_META_WiFi_HQA_GetAXRXInfoAll_r(UInt32 ms_timeout, UInt32 InTypeMask, UInt32 InBand, ref WIFI_RX_STASTIC_AX AXRxInfo)
        {
            return SP_META_WiFi_HQA_GetAXRXInfoAll_r(m_hMeta, ms_timeout, InTypeMask, InBand,ref AXRxInfo);
        }

        public int SP_META_WiFi_HQA_DBDCStopRX_r(UInt32 ms_timeout, UInt32 Band)
        {
            return SP_META_WiFi_HQA_DBDCStopRX_r(m_hMeta, ms_timeout, Band);
        }

        public int SP_META_GPS_Open_r(UInt32 ms_timeout)
        {
            return SP_META_GPS_Open_r(m_hMeta, ms_timeout);
        }

        public int SP_META_GPS_SendCommandMultiThread_r(UInt32 ms_timeout, ref GPS_CMD req, ref GPS_ACK_BUF cnf, META_GPS_SEND_COMMAND_CNF cb, IntPtr usrData)
        {
            return SP_META_GPS_SendCommandMultiThread_r(m_hMeta, ms_timeout, ref req, ref cnf, cb, usrData);
        }

        public int SP_META_GPS_SendCommand_r(UInt32 ms_timeout, ref GPS_CMD req, ref GPS_ACK_BUF cnf, META_GPS_SEND_COMMAND_CNF cb)
        {
            return SP_META_GPS_SendCommand_r(m_hMeta, ms_timeout, ref req, ref cnf, cb);
        }

        public int SP_META_GPS_SendCommand_Internal_r(UInt32 ms_timeout, ref GPS_CMD req, ref GPS_ACK_BUF cnf, META_GPS_SEND_COMMAND_CNF cb, IntPtr usrData)
        {
            return SP_META_GPS_SendCommand_Internal_r(m_hMeta, ms_timeout, ref req, ref cnf, cb, usrData);
        }

        public int META_MMRf_SetToolUsage_r(UInt32 ms_timeout, MMRfTestCmdEtSetToolUsage[] req)
        {
            return META_MMRf_SetToolUsage_r(m_hMeta, ms_timeout, req);
        }

        public int META_QueryIfFunctionSupportedByTarget_r(UInt32 ms_timeout, string query_func_name)
        {
            return META_QueryIfFunctionSupportedByTarget_r(m_hMeta, ms_timeout, query_func_name);
        }

        public int META_ERf_ForceTasSwitch_r(UInt32 ms_timeout, ref ERfTestCmdTasCfg req)
        {
            return META_ERf_ForceTasSwitch_r(m_hMeta, ms_timeout,ref req);
        }

        public int META_MMRf_ForceTasSwitch_r(UInt32 ms_timeout,ref MMRfTestCmdCfgTas req, ref MMRfTestResultCfgTas cnf)
        {
            return META_MMRf_ForceTasSwitch_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }

        public int META_MMRf_QueryTasVerifyList_V7_r(UInt32 ms_timeout, ref MMRfTestResultQueryTasVerifyListV7 cnf)
        {
            return META_MMRf_QueryTasVerifyList_V7_r(m_hMeta, ms_timeout, ref cnf);
        }

        public int META_MMRf_GetTasStateCfg_r(UInt32 ms_timeout, ref MMRfTestCmdFGetTasStateCfg req,ref MMRfTestResultGetTasStateCfg cnf)
        {
            return META_MMRf_GetTasStateCfg_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }

        public int META_MMRf_GetUtasStateCfg_r(UInt32 ms_timeout, ref MMRfTestCmdFGetTasStateCfg req, ref MMRfTestCmdGetTasStateCfgV5Cnf cnf)
        {
            return META_MMRf_GetUtasStateCfg_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }

        public int META_MMRf_ForceTasState_V7_r(UInt32 ms_timeout, ref MMRfTestCmdForceTasStateV7 req, ref MMRfTestResultForceTasStateV7 cnf)
        {
            return META_MMRf_ForceTasState_V7_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_MMRf_QueryCarKitMappingInfo_r(UInt32 ms_timeout, ref MMRfTestCmdQueryCarKitMappingInfoReq req, ref MMRfTestCmdQueryCarKitMappingInfoCnf cnf)
        {
            return META_MMRf_QueryCarKitMappingInfo_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_MMRf_QueryCarKitMappingInfoV7_r(UInt32 ms_timeout, ref MMRfTestCmdQueryCarKitMappingInfoReq req, ref MMRfTestCmdQueryCarKitMappingInfoCnfV7 cnf)
        {
            return META_MMRf_QueryCarKitMappingInfoV7_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_MMRf_GetShareRouteInfo_V7_r(UInt32 ms_timeout, ref MMRfTestResultShareRouteInfo cnf)
        {
            return META_MMRf_GetShareRouteInfo_V7_r(m_hMeta, ms_timeout, ref cnf);
        }
        public int META_MMRf_SetAfcSetting_r(UInt32 ms_timeout, ref MMRfTestResultGetAfcSetting req)
        {
            return META_MMRf_SetAfcSetting_r(m_hMeta, ms_timeout, ref req);
        }
        public int META_MMRf_GetAfcSetting_r(UInt32 ms_timeout, ref MMRfTestResultGetAfcSetting cnf)
        {
            return META_MMRf_GetAfcSetting_r(m_hMeta, ms_timeout, ref cnf);
        }
        public int META_MMRf_SetAfcSettingV2_r(UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV2 req)
        {
            return META_MMRf_SetAfcSettingV2_r(m_hMeta, ms_timeout, ref req);
        }
        public int META_MMRf_GetAfcSettingV2_r(UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV2 cnf)
        {
            return META_MMRf_GetAfcSettingV2_r(m_hMeta, ms_timeout, ref cnf);
        }
        public int META_MMRf_SetAfcSettingV3_r(UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV3 req)
        {
            return META_MMRf_SetAfcSettingV3_r(m_hMeta, ms_timeout, ref req);
        }
        public int META_MMRf_GetAfcSettingV3_r(UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV3 cnf)
        {
            return META_MMRf_GetAfcSettingV3_r(m_hMeta, ms_timeout, ref cnf);
        }

        #endregion

        public int META_Rf_GetRFID_r(UInt32 ms_timeout, ref RFMod_ID cnf)
        {
            return META_Rf_GetRFID_r(m_hMeta, ms_timeout, ref cnf);
        }

        public int META_Rf_QueryMSCapabilityEx2_r(UInt32 ms_timeout, ref RfMsCapabilityEx2_S p_ms_cap)
        {
            return META_Rf_QueryMSCapabilityEx2_r(m_hMeta, ms_timeout, ref p_ms_cap);
        }

        public int META_Rf_QueryMSCapabilityEx3_r(UInt32 ms_timeout, ref RfMsCapabilityEx3_REQ_S req, ref RfMsCapabilityEx3_S ms_cap)
        {
            return META_Rf_QueryMSCapabilityEx3_r(m_hMeta, ms_timeout, ref req, ref ms_cap);
        }

        public int META_Rf_Stop_Ex_r(UInt32 ms_timeout)
        {
            return META_Rf_Stop_Ex_r(m_hMeta, ms_timeout);
        }

        public int META_Rf_NSFT_Start_V5_r(UInt32 ms_timeout, ref Rf_NSFT_REQ_V5_T req)
        {
            return META_Rf_NSFT_Start_V5_r(m_hMeta, ms_timeout, ref req);
        }

        public int META_Rf_NSFT_Start_r(UInt32 ms_timeout, ref Rf_NSFT_REQ_T req)
        {
            return META_Rf_NSFT_Start_r(m_hMeta, ms_timeout, ref req);
        }

        public int META_Rf_NSFT_StartRxLevel_r(UInt32 ms_timeout)
        {
            return META_Rf_NSFT_StartRxLevel_r(m_hMeta, ms_timeout);
        }

        public int META_Rf_NSFT_GetRxLevel_V5_r(UInt32 ms_timeout, ref UInt16[] rx_levels)
        {
            return META_Rf_NSFT_GetRxLevel_V5_r(m_hMeta, ms_timeout, ref rx_levels);
        }

        public int META_Rf_NSFT_GetRxLevel_r(UInt32 ms_timeout, ref UInt16 rx_level)
        {
            return META_Rf_NSFT_GetRxLevel_r(m_hMeta, ms_timeout, ref rx_level);
        }

        public int META_Rf_NSFT_ConfigSBER_r(UInt32 ms_timeout, UInt32 test_frame_count)
        {
            return META_Rf_NSFT_ConfigSBER_r(m_hMeta, ms_timeout, test_frame_count);
        }

        public int META_Rf_NSFT_GetSBER_r(UInt32 ms_timeout, ref RF_NSFT_SBERResult_T sber_result)
        {
            return META_Rf_NSFT_GetSBER_r(m_hMeta, ms_timeout, ref sber_result);
        }

        public int META_MMRf_GetRfCapability_r(UInt32 ms_timeout, ref MMRfTestCmdRfCapabilityReq req, UInt32 requestLength, ref MMRfTestCmdRfCapabilityCnf resp, UInt32 responseLength)
        {
            return META_MMRf_GetRfCapability_r(m_hMeta, ms_timeout, ref req, requestLength, ref resp, responseLength);
        }

        public int META_NRf_GetRfCapability_r(UInt32 ms_timeout, ref NRfTestCmdRfCapabilityReq req, UInt32 requestLength, ref NRfTestCmdRfCapabilityCnf resp, UInt32 responseLength)
        {
            return META_NRf_GetRfCapability_r(m_hMeta, ms_timeout, ref req, requestLength, ref  resp, responseLength);
        }

        public int META_NRf_TestStop_V7_r(UInt32 ms_timeout)
        {
            return META_NRf_TestStop_V7_r(m_hMeta, ms_timeout);
        }

        public int META_NRf_TxPusch_V7_r(UInt32 ms_timeout, ref NRfTestCmd_StartPuschTxCaV7 req, ref UInt32 pSyncStatus)
        {
            return META_NRf_TxPusch_V7_r(m_hMeta, ms_timeout, ref  req, ref pSyncStatus);
        }

        public int META_NRf_StartMixRx_V7_r(UInt32 ms_timeout, ref NRfTestCmd_StartMixRxCaV7 req, ref UInt32 pSyncStatus)
        {
            return META_NRf_StartMixRx_V7_r(m_hMeta, ms_timeout, ref req, ref pSyncStatus);
        }

        public int META_NRf_ResetCounter_r(UInt32 ms_timeout)
        {
            return META_NRf_ResetCounter_r(m_hMeta, ms_timeout);
        }

        public int META_NRf_GetMixRxReport_V7_r(UInt32 ms_timeout, ref NRfTestResult_GetMixRxReportV7 cnf)
        {
            return META_NRf_GetMixRxReport_V7_r(m_hMeta, ms_timeout, ref cnf);
        }

        public int META_MMC2K_GetTargetGenType_r(ref UInt32 type)
        {
            return META_MMC2K_GetTargetGenType_r(m_hMeta, ref type);
        }

        public int META_MMC2K_UbinModeSetup_r(uint msTimeout, byte mode)
        {
            return META_MMC2K_UbinModeSetup_r(m_hMeta, msTimeout, mode);
        }

        public int META_MMC2K_QueryTargetCapability_r(uint msTimeout, ref C2kMsCapability msCapability)
        {
            return META_MMC2K_QueryTargetCapability_r(m_hMeta, msTimeout, ref msCapability);
        }

        public int META_MMC2K_NsftEnterTestMode_r(uint msTimeout, ref C2kNsftCmdTestMode req, ref C2kNsftResultStatus cnf)
        {
            return META_MMC2K_NsftEnterTestMode_r(m_hMeta, msTimeout, ref req, ref cnf);
        }

        public int META_MMC2K_NsftExitTestMode_r(uint msTimeout, ref C2kNsftCmdTestMode req, ref C2kNsftResultStatus cnf)
        {
            return META_MMC2K_NsftExitTestMode_r(m_hMeta, msTimeout, ref req, ref cnf);
        }

        public int META_MMC2K_NsftPowerUp_r(uint msTimeout, ref C2kNsftCmdPowerUp req, ref C2kNsftResultStatus cnf)
        {
            return META_MMC2K_NsftPowerUp_r(m_hMeta, msTimeout, ref req, ref cnf);
        }

        public int META_MMC2K_NsftSetTxPower_r(uint msTimeout, ref C2kNsftCmdSetTxPower req, ref C2kNsftResultStatus cnf)
        {
            return META_MMC2K_NsftSetTxPower_r(m_hMeta, msTimeout, ref req, ref  cnf);
        }

        public int META_MMC2K_NsftGetFer_r(uint msTimeout, ref C2kNsftCmdFer req, ref C2kNsftResultFer cnf)
        {
            return META_MMC2K_NsftGetFer_r(m_hMeta, msTimeout, ref req, ref cnf);

        }

        public int META_MMC2K_NsftGetRssi_r(uint msTimeout, ref C2kNsftCmdRssi req, ref C2kNsftResultRssi cnf)
        {
            return META_MMC2K_NsftGetRssi_r(m_hMeta, msTimeout, ref req, ref cnf);
        }



        public int META_C2K_NsftSetTestMode_r(uint msTimeout)
        {
            return META_C2K_NsftSetTestMode_r(m_hMeta, msTimeout);
        }

        public int META_C2K_NsftExitTestMode_r(uint msTimeout)
        {
            return META_C2K_NsftExitTestMode_r(m_hMeta, msTimeout);
        }

        public int META_C2K_QueryTargetCapability_r(uint msTimeout, ref C2K_MS_CAPABILITY msCapability)
        {
            return META_C2K_QueryTargetCapability_r(m_hMeta, msTimeout, ref msCapability);
        }

        public int META_C2K_PsPower_r(uint msTimeout, bool enable)
        {
            return META_C2K_PsPower_r(m_hMeta, msTimeout, enable);
        }

        public int META_C2K_NsftPowerUp_r(uint msTimeout, uint band, UInt16 channel, uint walshCode, uint rc, uint numFrames, bool afcEnable)
        {
            return META_C2K_NsftPowerUp_r(m_hMeta, msTimeout, band, channel, walshCode, rc, numFrames, afcEnable);
        }

        public int META_C2K_NsftTxTrafficChannel_r(uint msTimeout, uint rc, uint powerCtrlMode, double txPower)
        {
            return META_C2K_NsftTxTrafficChannel_r(m_hMeta, msTimeout, rc, powerCtrlMode, txPower);
        }

        public int META_C2K_NsftGetFer_r(uint msTimeout, ref UInt32 badFrames, ref UInt32 totalFrames)
        {
            return META_C2K_NsftGetFer_r(m_hMeta, msTimeout, ref badFrames, ref totalFrames);
        }

        public int META_C2K_NsftRssi_r(uint msTimeout, uint band, UInt16 channel, ref C2K_NSFT_RSSI_CNF cnf)
        {
            return META_C2K_NsftRssi_r(m_hMeta, msTimeout, band, channel, ref  cnf);
        }

        public int AST_CAL_get_chipid_r(int msTimeout, ref ushort BB_chipID, ref ushort RF_chipID)
        {
            return AST_CAL_get_chipid_r(m_hMeta, msTimeout, ref BB_chipID, ref RF_chipID);
        }

        public int AST_CAL_get_capability_r(int ms_timeout, UInt16 capability, UInt16 band_support)
        {
            return AST_CAL_get_capability_r(m_hMeta, ms_timeout, capability, band_support);
        }
        public int AST_GetRfCapability_r(UInt32 ms_timeout, ref T_AST_RF_CAPABILITY_RESULT res, /* [OUT] the RF capability provided by L1 */        UInt32 res_length /* [IN] length of the input buffer */		)
        {
            return AST_GetRfCapability_r(m_hMeta, ms_timeout, ref res, res_length);
        }
        public int AST_CAL_QueryAPISupported_r(int ms_timeout, string func_name)
        {
            return AST_CAL_QueryAPISupported_r(m_hMeta, ms_timeout, func_name);
        }
        public int AST_UBIN_MODE_SETUP_r(UInt32 ms_timeout, ref T_AST_RFCAL_UBIN_MODE ubin_tdd_mode_init)
        {
            return AST_UBIN_MODE_SETUP_r(m_hMeta, ms_timeout, ref ubin_tdd_mode_init);
        }
        public int AST_CAL_tl1_reset_r(int ms_timeout)
        {
            return AST_CAL_tl1_reset_r(m_hMeta, ms_timeout);
        }
        public int AST_NSFT_Start_r(int ms_timeout,
           UInt16 dpch_freq,  /* UARFCN*/
           UInt16 cp_id,  /* CPID for the midamble selection*/
           byte dl_timeslot,  /* DL Timeslot, support 1 TS only(0 for TS0,1 for TS1бн)*/
           UInt32 dl_code,  /* DL TS channel code bitmap										bit0 is C16-1                                         bit1 is C16-2                                         ...                                        bit 15 is C16-16                                         bit16 is C8-1                                         ...                                        bit23 is C8-8                                         bit24 is C4-1										...                                        bit27 is C4-4                                         bit28 is C2-1                                         bit29 is C2-2                                         bit30 is C1-1                                        bit31 is reserved.*/
               byte ul_timeslot,// 0-6,UL Timeslot, support 1 TS only(0 for TS0,1 for TS1бн)		
                   UInt32 ul_code, /* UL TS channel code bitmap										bit0 is C16-1                                         bit1 is C16-2                                         ...                                        bit 15 is C16-16                                         bit16 is C8-1                                         ...                                        bit23 is C8-8                                         bit24 is C4-1										...                                        bit27 is C4-4                                         bit28 is C2-1                                         bit29 is C2-2                                         bit30 is C1-1                                        bit31 is reserved.*/
                       bool single_end_loop_enable, //switch for the single ended BER		
                           bool b_afc_dac_valid,//indicate if using the AFC DAC below		
                               UInt16 afc_dac,    // AFC DAC set by META		
                               byte loopbackType,  // Loopback type  BER/BLER type, 0 for BER, 1 for BLER		
                                   bool b_bit_pattern_allzero,  // all 1 or all zero		
                                       byte ul_dpdch_pwr,  // UL target power for the output loop power control 		
                                           byte ul_ma)
        {
            return AST_NSFT_Start_r(m_hMeta, ms_timeout, dpch_freq,  /* UARFCN*/
                cp_id,  /* CPID for the midamble selection*/
                    dl_timeslot,  /* DL Timeslot, support 1 TS only(0 for TS0,1 for TS1бн)*/
                    dl_code,  /* DL TS channel code bitmap										bit0 is C16-1                                         bit1 is C16-2                                         ...                                        bit 15 is C16-16                                         bit16 is C8-1                                         ...                                        bit23 is C8-8                                         bit24 is C4-1										...                                        bit27 is C4-4                                         bit28 is C2-1                                         bit29 is C2-2                                         bit30 is C1-1                                        bit31 is reserved.*/
                        ul_timeslot,// 0-6,UL Timeslot, support 1 TS only(0 for TS0,1 for TS1бн)		
                        ul_code, /* UL TS channel code bitmap										bit0 is C16-1                                         bit1 is C16-2                                         ...                                        bit 15 is C16-16                                         bit16 is C8-1                                         ...                                        bit23 is C8-8                                         bit24 is C4-1										...                                        bit27 is C4-4                                         bit28 is C2-1                                         bit29 is C2-2                                         bit30 is C1-1                                        bit31 is reserved.*/
                                        single_end_loop_enable, //switch for the single ended BER		
                                        b_afc_dac_valid,//indicate if using the AFC DAC below		
                                    afc_dac,    // AFC DAC set by META		
                                        loopbackType,  // Loopback type  BER/BLER type, 0 for BER, 1 for BLER		
                                                    b_bit_pattern_allzero,  // all 1 or all zero		
                                        ul_dpdch_pwr,  // UL target power for the output loop power control 		
                                                ul_ma);
        }

        public int AST_NSFT_Stop_r(int ms_timeout)
        {
            return AST_NSFT_Stop_r(m_hMeta, ms_timeout);
        }
        public int AST_NSFT_GetRscp_r(int ms_timeout, ref short rscp)
        {
            return AST_NSFT_GetRscp_r(m_hMeta, ms_timeout, ref rscp);
        }
        public int AST_NSFT_GetBitCount_r(int ms_timeout, ref UInt32 total_bits, ref UInt32 error_bits)
        {
            return AST_NSFT_GetBitCount_r(m_hMeta, ms_timeout, ref total_bits, ref error_bits);
        }
        public int META_3Grf_SetTxPaDriftCompEnable_r(UInt32 ms_timeout, byte is_PaDrift)
        {
            return META_3Grf_SetTxPaDriftCompEnable_r(m_hMeta, ms_timeout, is_PaDrift);
        }
        public int META_3Grf_NSFT_StartEx_r(UInt32 ms_timeout, ref UL1D_RF_NSFT_REQ_T req, ref UMTS_NSFTLinkStatusReport cnf)
        {
            return META_3Grf_NSFT_StartEx_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_NSFT_SetILPCStep_r(UInt32 ms_timeout, byte step)
        {
            return META_3Grf_NSFT_SetILPCStep_r(m_hMeta, ms_timeout, step);
        }
        public int META_3Grf_NSFT_GetBitCountForSingleEndedBER_r(UInt32 ms_timeout, ref UL1D_RF_NSFT_GET_BIT_CNT_FOR_BER_CNF_T cnf)
        {
            return META_3Grf_NSFT_GetBitCountForSingleEndedBER_r(m_hMeta, ms_timeout, ref cnf);
        }
        public int META_3Grf_Rssi_r(UInt32 ms_timeout, ref URfTestCmdRSSI req, ref URfTestResultRSSI cnf)
        {
            return META_3Grf_Rssi_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_RxDRssi_r(UInt32 ms_timeout, ref URfTestCmdRSSI req, ref URfTestResultRSSIRxD cnf)
        {
            return META_3Grf_RxDRssi_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_Lpm_Rssi_r(UInt32 ms_timeout, ref URfTestCmdLPMRSSI req, ref URfTestResultRSSI cnf)
        {
            return META_3Grf_Lpm_Rssi_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_ELNA_Rssi_r(UInt32 ms_timeout, ref URfTestCmdELNARSSI req, ref URfTestResultELNARSSI cnf)
        {
            return META_3Grf_ELNA_Rssi_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GainSelectFromPowerV3_r(UInt32 ms_timeout, ref URfTestCmdPwrToGainV3 req, ref URfTestResultPwrToGainV3 cnf)
        {
            return META_3Grf_GainSelectFromPowerV3_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GainSelectFromPowerV5_r(UInt32 ms_timeout, ref URfTestCmdPwrToGainV5 req, ref URfTestResultPwrToGainV3 cnf)
        {
            return META_3Grf_GainSelectFromPowerV5_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GetRFID_r(UInt32 ms_timeout, ref URfTestResultRFID cnf)
        {
            return META_3Grf_GetRFID_r(m_hMeta, ms_timeout, ref cnf);
        }
        public int META_3Grf_QueryTargetCapability_r(UInt32 ms_timeout, ref UMTS_MsCapabilityEx cnf)
        {
            return META_3Grf_QueryTargetCapability_r(m_hMeta, ms_timeout, ref cnf);
        }
        public int META_3Grf_GetRfCapability_r(UInt32 ms_timeout, ref URfTestCmdGetRfCapabilityReq req, UInt32 requestLength, ref URfTestResultGetRfCapabilityCnf cnf, UInt32 responseLength)
        {
            return META_3Grf_GetRfCapability_r(m_hMeta, ms_timeout, ref req, requestLength, ref cnf, responseLength);
        }
        public int META_3Grf_UbinModeSetup_r(UInt32 ms_timeout, byte ubin_fdd_mode_init)
        {
            return META_3Grf_UbinModeSetup_r(m_hMeta, ms_timeout, ubin_fdd_mode_init);
        }
        public int META_3Grf_TestStop_r(UInt32 ms_timeout, ref URfTestResultParam cnf)
        {
            return META_3Grf_TestStop_r(m_hMeta, ms_timeout, ref cnf);
        }
        public int META_3Grf_GetRssiV3_r(UInt32 ms_timeout, ref URfTestCmdGetRssiV3 req, ref URfTestResultGetRssiV3 cnf)
        {
            return META_3Grf_GetRssiV3_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GetRssiV5_r(UInt32 ms_timeout, ref URfTestCmdGetRssiV5 req, ref URfTestResultGetRssiV3 cnf)
        {
            return META_3Grf_GetRssiV5_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GainSelectFromPowerV7_r(UInt32 ms_timeout, ref URfTestCmdPwrToGainV7 req, ref URfTestResultPwrToGainV3 cnf)
        {
            return META_3Grf_GainSelectFromPowerV7_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GetRssiV7_r(UInt32 ms_timeout, ref URfTestCmdGetRssiV7 req, ref URfTestResultGetRssiV7 cnf)
        {
            return META_3Grf_GetRssiV7_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_Set_Initial_Cellpower_r(UInt32 ms_timeout, int confg_cell)
        {
            return META_3Grf_Set_Initial_Cellpower_r(m_hMeta, ms_timeout, confg_cell);
        }
        public int META_3Grf_Lpm_RxDRssi_r(UInt32 ms_timeout, ref URfTestCmdLPMRSSI req, ref URfTestResultRSSIRxD cnf)
        {
            return META_3Grf_Lpm_RxDRssi_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_ERf_GetRfCapability_r(UInt32 ms_timeout, ref ERfTestCmdRfCapabilityReq req, UInt32 requestLength, ref ERfTestCmdRfCapabilityCnf resp, UInt32 responseLength)
        {
            return META_ERf_GetRfCapability_r(m_hMeta, ms_timeout, ref req, requestLength, ref resp, responseLength);
        }
        public int META_ERf_QueryCaConfigTableV3_r(UInt32 ms_timeout, ref ERfTestCmdCaConfig_V3 resp)
        {
            return META_ERf_QueryCaConfigTableV3_r(m_hMeta, ms_timeout, ref resp);
        }
        public int META_ERf_QueryCaConfigTableV5_r(UInt32 ms_timeout, ref ERfTestCmdCaConfig_V5 resp)
        {
            return META_ERf_QueryCaConfigTableV5_r(m_hMeta, ms_timeout, ref resp);
        }
        public int META_MMRf_EN_QueryConfig_V7_r(UInt32 ms_timeout, ref MMRfTestCmd_EN_QueryConfigV7 req, ref MMRfTestResult_EN_QueryConfigV7 cnf)
        {
            return META_MMRf_EN_QueryConfig_V7_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_ERf_StopTestMode_r(UInt32 ms_timeout)
        {
            return META_ERf_StopTestMode_r(m_hMeta, ms_timeout);
        }
        public int META_ERf_TxPusch_V5_r(UInt32 ms_timeout, ref ERfTestCmd_StartPuschTxCaV5_ReqParam req, ref UInt32 pSyncStatus)
        {
            return META_ERf_TxPusch_V5_r(m_hMeta, ms_timeout, ref req, ref pSyncStatus);
        }
        public int META_ERf_StartMixRx_CaMode_V3_r(UInt32 ms_timeout, ref ERfTestCmdMixRx_CaMode_V3 req)
        {
            return META_ERf_StartMixRx_CaMode_V3_r(m_hMeta, ms_timeout, ref req);
        }
        public int META_ERf_StartMixRx_CaMode_V5_r(UInt32 ms_timeout, ref ERFTestCmd_StartMixRxCaV5_ReqParam req)
        {
            return META_ERf_StartMixRx_CaMode_V5_r(m_hMeta, ms_timeout, ref req);
        }
        public int META_ERf_ResetCounter_r(UInt32 ms_timeout)
        {
            return META_ERf_ResetCounter_r(m_hMeta, ms_timeout);
        }
        public int META_ERf_GetMixRxReport_CaMode_V3_r(UInt32 ms_timeout, ref ERfTestCmdGetMixRxRpt_CaMode_V2 resp)
        {
            return META_ERf_GetMixRxReport_CaMode_V3_r(m_hMeta, ms_timeout, ref resp);
        }
        public int META_ERf_GetMixRxReport_CaMode_V5_r(UInt32 ms_timeout, ref ERfTestResultGetMixRxRpt_CaMode_V5 resp)
        {
            return META_ERf_GetMixRxReport_CaMode_V5_r(m_hMeta, ms_timeout, ref resp);
        }

        public int META_ERf_TxPusch_V2_r(uint ms_timeout, ref ERfTestCmdPuschTxV2 req, ref uint pSyncStatus)
        {
            return META_ERf_TxPusch_V2_r(m_hMeta, ms_timeout, ref req, ref pSyncStatus);
        }

        public int META_NRf_GetPuschTxDLFreq_V7_r(UInt32 ms_timeout, ref NRfTestCmd_GetDlFreqV7 req, ref NRfTestResult_GetDlFreqV7 dlFrequency)
        {
            return META_NRf_GetPuschTxDLFreq_V7_r(m_hMeta, ms_timeout, ref req, ref dlFrequency);
        }
        public int META_3Grf_GetCalInfoV3_r(UInt32 ms_timeout, ref URfTestCmdGetMdCalInfoV3 req, ref URfTestResultGetMdCalInfoV3 cnf)
        {
            return META_3Grf_GetCalInfoV3_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GetCalInfoV5_r(UInt32 ms_timeout, ref URfTestCmdGetMdCalInfoV5 req, ref URfTestResultGetMdCalInfoV5 cnf)
        {
            return META_3Grf_GetCalInfoV5_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }
        public int META_3Grf_GetCalInfoV7_r(UInt32 ms_timeout, ref URfTestCmdGetMdCalInfoV7 req, ref URfTestResultGetMdCalInfoV7 cnf)
        {
            return META_3Grf_GetCalInfoV7_r(m_hMeta, ms_timeout, ref req, ref cnf);
        }

        public int META_ERf_StartTraditionalTxNSFT_r(ref ERfTestCmdPuschTx req, UInt32 ms_timeout, ref UInt32 pSyncStatus)
        {
            return META_ERf_StartTraditionalTxNSFT_r(m_hMeta, ref req, ms_timeout, ref pSyncStatus);
        }

        public int META_ERf_QueryCaConfigTable_r(UInt32 ms_timeout, ref ERfTestCmdCaConfig resp)
        {
            return META_ERf_QueryCaConfigTable_r(m_hMeta, ms_timeout, ref resp);
        }

        public int META_ERf_SetLnaSrx_r(ref ERfTestCmdSetRxCommonCfg req, UInt32 ms_timeout)
        {
            return META_ERf_SetLnaSrx_r(m_hMeta, ref req, ms_timeout);
        }

        public int META_ERf_StartMixRx_r(ref ERfTestCmdMixRx req, UInt32 ms_timeout)
        {
            return META_ERf_StartMixRx_r(m_hMeta, ref req, ms_timeout);
        }

        public int META_ERf_GetMixRxReport_r(UInt32 ms_timeout, ref ERfTestCmdGetMixRxRpt resp)
        {
            return META_ERf_GetMixRxReport_r(m_hMeta, ms_timeout, ref resp);
        }

        public int META_NVRAM_GetRecLen_r(string LID, ref int len)
        {
            return META_NVRAM_GetRecLen_r(m_hMeta, LID, ref len);
        }

        public int META_NVRAM_Read_Ex_r( UInt32 ms_timeout, ref FT_NVRAM_READ_REQ req, ref FT_NVRAM_READ_CNF cnf)
        {
            return META_NVRAM_Read_Ex_r(m_hMeta, ms_timeout, ref  req, ref  cnf);
        }

        public int META_NVRAM_GetRecFieldValue_r( string LID, string field, string buf, int buf_len, IntPtr value, int value_len)
        {
            return META_NVRAM_GetRecFieldValue_r(m_hMeta, LID, field, buf, buf_len, value, value_len);
        }
    }
}
