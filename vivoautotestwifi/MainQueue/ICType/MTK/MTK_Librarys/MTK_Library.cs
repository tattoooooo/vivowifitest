using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vivoautotestwifi.MainQueue.ICType.MTK.MTK_Librarys
{
   public partial class MTK_Library
    {
        const string METAAPP_DLL_PATH = @"lib\MTK\MT6635\MetaApp.dll";
        const string metacore = @"lib\MTK\MT6635\MetaCore.dll";

        #region MetaApp

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_ConnectTargetAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ConnectTargetAllInOne_r(int threadSlot, ref METAAPP_CONN_STTING_T pSetting, ref METAAPP_CONNCT_CNF_T pCnf,UInt32 arg_size);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_ApToModemAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ApToModemAllInOne_r(int threadSlot);
        
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_ModemToApAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ModemToApAllInOne_r(int threadSlot);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_DisConnectMeta_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_DisConnectMeta_r(int threadSlot, bool isPoweroff);
        
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_InitMetaLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_InitMetaLog(int meta_handle, MiniComLogInput_s sLogInput);
        
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_InitMetaLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_StartMetaLog(int meta_handle);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_StopLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_StopLog(int meta_handle, bool bAssert);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_RenameLogFiles", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_RenameLogFiles(ref string strPath, ref string strKeyPhase, ref string strReplacePhase);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_AddTimeForLogName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_AddTimeForLogName(ref string strInputName, ref string strOutputName, int iOutputStringSize);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_SwitchTestRat_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_SwitchTestRat_r(int threadSlot, METAAPP_TEST_RAT_E tRat);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_DisConnectMeta_Ex_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_DisConnectMeta_Ex_r(int threadSlot, METAAPP_DISCONNECT_META_T disconPara, UInt32 arg_size);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_DisConnectMeta_Ex_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ConnectAPOnlyAllInOne_r(int threadSlot, ref METAAPP_CONN_STTING_T pSetting, ref METAAPP_CONNCT_CNF_T pCnf, UInt32 arg_size);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_DisConnectMeta_Ex_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ConnectThinModemToMeta_r(int threadSlot, ref METAAPP_CONN_STTING_T pSetting, ref METAAPP_CONNCT_CNF_T pCnf, UInt32 arg_size);


        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_DisConnectMeta_Ex_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_DisConnectThinModemMeta_r(int threadSlot, METAAPP_DISCONNECT_META_T disconPara, UInt32 arg_size);

        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_DisConnectMeta_Ex_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_RegisterCallBackFor_Conn_Log_Display(int threadSlot, METAAPP_Conn_Log_Display_CallBackForMultiThread cb, IntPtr cbUserData);

        #endregion

        #region MetaCore
        /// <summary>
        /// Get META_DLL.dll api result infomation string.
        /// </summary>
        /// <param name="ErrCode">the return value by other META_xxx apis.</param>
        /// <returns>infromation string</returns>
        [DllImport(metacore, EntryPoint = "SP_META_GetErrorString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static string SP_META_GetErrorString(int ErrCode);

        /// <summary>
        /// Disconnect from ap meta stage, and shutdown device.
        /// </summary>
        /// <param name="aphandle"></param>
        /// <returns></returns>
        [DllImport(metacore, EntryPoint = "SP_META_DisconnectWithTarget_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_DisconnectWithTarget_r(int aphandle);

        /// <summary>
        /// Disconnect from ap meta stage ( kip device in meta mode.)
        /// </summary>
        /// <param name="aphandle"></param>
        /// <returns></returns>
        [DllImport(metacore, EntryPoint = "SP_META_DisconnectInMetaMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_DisconnectInMetaMode_r(int aphandle);

        [DllImport(metacore, EntryPoint = "SP_META_GetTargetVerInfoV2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_GetTargetVerInfoV2_r(int aphandle, ref VerInfo_V2_Cnf cnf, ref short token, IntPtr usrData);

        /// <summary>
        /// Backup all nvram content from nvdata partition to nvram(binregion) partition.
        /// This api only can be called in ap meta stage.
        /// </summary>
        /// <param name="aphandle"></param>
        /// <param name="ms_timeout"></param>
        /// <param name="req"></param>
        /// <param name="cnf"></param>
        /// <returns></returns>
        [DllImport(metacore, EntryPoint = "SP_META_SetCleanBootFlag_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_SetCleanBootFlag_r(int aphandle, uint ms_timeout, ref SetCleanBootFlag_REQ req, ref SetCleanBootFlag_CNF cnf);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_GetChipVersion_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_GetChipVersion_r(int mdhandle, int ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_readMCR32_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_readMCR32_r(int mdhandle, UInt32 ms_timeout, UInt32 offset, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_WriteNVRAM_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_WriteNVRAM_r(int mdhandle, UInt32 ms_timeout, ref NVRAM_ACCESS_STRUCT preq);        

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_GetChipCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_GetChipCapability_r(int mdhandle, UInt32 ms_timeout, ref CapBuffer CapBuffer,ref UInt32 CapBufferLength);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReadNVRAM_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_ReadNVRAM_r(int mdhandle, UInt32 ms_timeout, ref NVRAM_ACCESS_STRUCT preq);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_QueryConfig_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_QueryConfig_r(int mdhandle, UInt32 ms_timeout, ref ENUM_CFG_SRC_TYPE_T buffType);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setTestMode_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_QueryAntSwap_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_QueryAntSwap_r(int mdhandle, UInt32 ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_SetAntSwap_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_SetAntSwap_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_switchAntenna_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_switchAntenna_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WIFI_OPEN_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WIFI_OPEN_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setChannel_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setChannel_r(int mdhandle, UInt32 ms_timeout, int channelfreq);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setRate_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setRate_r(int mdhandle, UInt32 ms_timeout, UInt32 Rate);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setBandwidthEx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setBandwidthEx_r(int mdhandle, UInt32 ms_timeout, UInt32 nChBandwidth, UInt32 nDataBandwidth, UInt32 nPrimarySetting);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setGuardinterval_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setGuardinterval_r(int mdhandle, UInt32 ms_timeout, UInt32 inerval);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setBandwidth_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setBandwidth_r(int mdhandle, UInt32 ms_timeout, UInt32 BandWidth);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setModeSelect_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setModeSelect_r(int mdhandle, UInt32 ms_timeout, UInt32 Mode);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setPacketTxEx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setPacketTxEx_r(int mdhandle, UInt32 ms_timeout, ref WIFI_TX_PARAM_T txparam);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setStandBy_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setStandBy_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setRxTest_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setRxTest_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedErrorCount_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_ReceivedErrorCount_r(int mdhandle, UInt32 ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedOKCount_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_ReceivedOKCount_r(int mdhandle, UInt32 ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setNss_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setNss_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setTXPath_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setTXPath_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setRXPath_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setRXPath_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedRSSI_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_ReceivedRSSI_r(int mdhandle, UInt32 ms_timeout, ref short value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedRSSI1_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_ReceivedRSSI1_r(int mdhandle, UInt32 ms_timeout, ref short value);

        [DllImport(metacore, EntryPoint = "SP_META_WIFI_CLOSE_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WIFI_CLOSE_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_queryThermoInfo_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_queryThermoInfo_r(int mdhandle, UInt32 ms_timeout, ref Int32 pi4Enable, ref UInt32 pu4RawVal);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_getTemperatureSensorResult_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_getTemperatureSensorResult_r(int mdhandle, UInt32 ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setJMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_setJMode_r(int mdhandle, UInt32 ms_timeout, int nMode);
        //meta.h

        [DllImport(metacore, EntryPoint = "META_Customer_Func_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Customer_Func_r(int mdhandle, UInt32 ms_timeout, string data_in, int data_in_len, ref string data_out, int data_out_len);

        [DllImport(metacore, EntryPoint = "SP_META_Customer_Func_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_Customer_Func_r(int mdhandle, UInt32 ms_timeout, string data_in, int data_in_len, int type, byte dummy_in, ref byte dummy_out, ref string data_out, int data_out_len);

        [DllImport(metacore, EntryPoint = "SP_META_GetTargetBuildProp_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_GetTargetBuildProp_r(int mdhandle, ref BUILD_PROP_REQ_S pReq, ref BUILD_PROP_CNF_S pCnf);

        [DllImport(metacore, EntryPoint = "SP_META_CancelAllBlockingCall_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_CancelAllBlockingCall_r(int mdhandle);

        [DllImport(metacore, EntryPoint = "SP_META_NVRAM_GetRecLen", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_NVRAM_GetRecLen(string LID, ref int len);

        [DllImport(metacore, EntryPoint = "SP_META_NVRAM_Read_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_NVRAM_Read_r(int mdhandle, ref AP_FT_NVRAM_READ_REQ req, ref AP_FT_NVRAM_READ_CNF cnf, SP_META_NVRAM_Read_CNF cb, ref short token, ref IntPtr usrData);

        [DllImport(metacore, EntryPoint = "SP_META_NVRAM_Write_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_NVRAM_Write_r(int mdhandle, ref AP_FT_NVRAM_READ_REQ req, SP_META_NVRAM_Write_CNF cb, ref short token, ref IntPtr usrData);

        //[DllImport(metacore, EntryPoint = "SP_META_NVRAM_GetRecLen_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int SP_META_NVRAM_GetRecLen_r(int mdhandle, string LID, ref int len);

        //[DllImport(metacore, EntryPoint = "META_NVRAM_Write_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_NVRAM_Write_r(int mdhandle, ref AP_FT_NVRAM_WRITE_REQ req, AP_FT_NVRAM_WRITE_CNF cb, ref short token, ref IntPtr usrData);

        //[DllImport(metacore, EntryPoint = "SP_META_NVRAM_GetRecFieldValue_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int SP_META_NVRAM_GetRecFieldValue_r(int mdhandle,string LID,string field, string buf, int buf_len,ref IntPtr value, int value_len);

        //[DllImport(metacore, EntryPoint = "SP_META_NVRAM_SetRecFieldValue_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int SP_META_NVRAM_SetRecFieldValue_r(int mdhandle, string LID, string field, string buf, int buf_len, ref IntPtr value, int value_len);

       [DllImport(metacore, EntryPoint = "META_NVRAM_GetRecLen_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int  META_NVRAM_GetRecLen_r(int meta_handle, string LID, ref int len);


        [DllImport(metacore, EntryPoint = "META_NVRAM_Read_Ex_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int  META_NVRAM_Read_Ex_r( int meta_handle, UInt32 ms_timeout, ref FT_NVRAM_READ_REQ req,ref FT_NVRAM_READ_CNF cnf);

         [DllImport(metacore, EntryPoint = "META_NVRAM_GetRecFieldValue_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NVRAM_GetRecFieldValue_r(int meta_handle, string LID, string field, string buf, int buf_len, IntPtr value, int value_len);
       //[DllImport(metacore, EntryPoint = "META_NVRAM_Write_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       // public extern static int   META_NVRAM_Write_r(int meta_handle, ref FT_NVRAM_WRITE_REQ req, ref META_NVRAM_Write_CNF cb, ref short token, IntPtr usrData);





        /***************************************by wzt**********************************************/

        [DllImport(metacore, EntryPoint = "SP_META_Query_WCNDriver_Ready_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_Query_WCNDriver_Ready_r(int mdhandle, UInt32 ms_timeout, ref QUERY_WCNDRIVER_READY_CNF pcnf);

        [DllImport(metacore, EntryPoint = "SP_META_QueryIfFunctionSupportedByTarget_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_QueryIfFunctionSupportedByTarget_r(int mdhandle, UInt32 ms_timeout, ref string query_func_name);

        [DllImport(metacore, EntryPoint = "SP_META_BT_OPEN_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_BT_OPEN_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_BT_CLOSE_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_BT_CLOSE_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_BTPowerOn_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_BTPowerOn_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_BT_GetChipID_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_BT_GetChipID_r(int mdhandle, UInt32 ms_timeout, ref int pID);

        [DllImport(metacore, EntryPoint = "SP_META_BT_SendHCICommand_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_BT_SendHCICommand_r(int mdhandle, UInt32 ms_timeout, ref BT_HCI_COMMAND req, META_BT_HCI_CNF cb, IntPtr cb_arg, byte Cmpltcode);

        #region WiFi New

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_OpenAdapter_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_OpenAdapter_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_SetBandMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_SetBandMode_r(int mdhandle, UInt32 ms_timeout, int iBandMode,UInt32 iBandType, UInt32 reserved0, UInt32 reserved1);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_AntSwapCap_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_AntSwapCap_r(int mdhandle,UInt32 ms_timeout, UInt32 band,ref UInt32 support);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_AntSwapSet_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_AntSwapSet_r(int mdhandle, UInt32 ms_timeout, UInt32 band, UInt32 ant);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_CloseAdapter_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_CloseAdapter_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_SetTxPathv2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_SetTxPathv2_r(int mdhandle, UInt32 ms_timeout, byte TxPath, UInt32 band);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_SetRxPathv2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_SetRxPathv2_r(int mdhandle, UInt32 ms_timeout, byte RxPath, UInt32 band);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_DBDCSetChannel_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_DBDCSetChannel_r(int mdhandle, UInt32 ms_timeout, UInt32 band, UInt32 CenterChannel0, UInt32 CenterChannel1/*(for 160nc)*/,
            UInt32 SystemBW, UInt32 PerpacketBW, UInt32 PrimarySelect, UInt32 Reason, UInt32 ChannelBand, UInt32 NewFreq);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_ResetTxRxCounterBand_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_ResetTxRxCounterBand_r(int mdhandle, UInt32 ms_timeout, UInt32 band);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_DBDCSetTXContent_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_DBDCSetTXContent_r(int mdhandle, UInt32 ms_timeout, UInt32 band, UInt32 FC, UInt32 Dur,
           UInt32 SeqID, UInt32 RepeatRandomNormal, UInt32 TxLength, UInt32 PayloadLngth, ref string pSourceAddr, ref string pDestAddr, ref string pBSSID,
           ref string pPayloadContent, UInt32 pReserved0, UInt32 pReserved1, UInt32 pReserved2);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_SetTxPowerExt_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_SetTxPowerExt_r(int meta_handle, UInt32 ms_timeout, UInt32 TxPower, UInt32 BAND, UInt32 Channel, UInt32 ChannelBand, UInt32 WFSelect);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_SetRUSettings_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_SetRUSettings_r(int meta_handle, UInt32 ms_timeout, UInt32 BandIdx, UInt32 Seg0Count, UInt32 Seg1Count,ref RUSettings pRUSettingsSeg0, ref UInt32 plenSeg0, string pRUSettingsSeg1, ref UInt32 plenSeg1);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_DBDCStartTX_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_DBDCStartTX_r(int meta_handle, UInt32 ms_timeout, UInt32 Band, UInt32 PcaketCount, UInt32 Preamble, UInt32 Rate, UInt32 Power, UInt32 STBC, UInt32 LDPC, UInt32 iBF,
      UInt32 eBF, UInt32 WlanIdx, UInt32 AIFS, UInt32 isShortGI, UInt32 TXPath, UInt32 Nss, ref UInt32 pReserved0, ref UInt32 pReserved1, ref UInt32 pReserved2);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_GetAFactor_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_GetAFactor_r(int meta_handle, UInt32 ms_timeout, ref UInt32 FactorValue, ref UInt32 LDPCExtraSym, ref UInt32 PEDisamp, ref UInt32 TXPEValue, ref UInt32 LSIG);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_DBDCStopTX_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_DBDCStopTX_r(int meta_handle, UInt32 ms_timeout, UInt32 Band, ref UInt32 pReserved0, ref UInt32 pReserved1, ref UInt32 pReserved2);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_DBDCStartRXExt_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_DBDCStartRXExt_r(int meta_handle, UInt32 ms_timeout, UInt32 Band, UInt32 RxPath, ref byte pMAC, UInt32 StaID, UInt32 Preamble, UInt32 LTFGI);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_GetAXRXInfoAll_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_GetAXRXInfoAll_r(int meta_handle, UInt32 ms_timeout, UInt32 InTypeMask, UInt32 InBand,ref WIFI_RX_STASTIC_AX AXRxInfo);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_HQA_DBDCStopRX_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_WiFi_HQA_DBDCStopRX_r(int meta_handle, UInt32 ms_timeout, UInt32 Band);

        #endregion

        #region GPS

        [DllImport(metacore, EntryPoint = "SP_META_GPS_Open_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_GPS_Open_r(int meta_handle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_GPS_Close_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_GPS_Close_r(int meta_handle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_GPS_SendCommandMultiThread_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_GPS_SendCommandMultiThread_r(int meta_handle, UInt32 ms_timeout, ref GPS_CMD req, ref GPS_ACK_BUF cnf, META_GPS_SEND_COMMAND_CNF cb, IntPtr usrData);

        [DllImport(metacore, EntryPoint = "SP_META_GPS_SendCommand_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_GPS_SendCommand_r(int meta_handle, UInt32 ms_timeout, ref GPS_CMD req, ref GPS_ACK_BUF cnf, META_GPS_SEND_COMMAND_CNF cb);

        [DllImport(metacore, EntryPoint = "SP_META_GPS_SendCommand_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SP_META_GPS_SendCommand_Internal_r(int meta_handle, UInt32 ms_timeout, ref GPS_CMD req, ref GPS_ACK_BUF cnf, META_GPS_SEND_COMMAND_CNF cb, IntPtr usrData);

        #endregion


        [DllImport(metacore, EntryPoint = "META_MMRf_SetToolUsage_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_SetToolUsage_r(int meta_handle, UInt32 ms_timeout, MMRfTestCmdEtSetToolUsage[] req);

        [DllImport(metacore, EntryPoint = "META_QueryIfFunctionSupportedByTarget_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_QueryIfFunctionSupportedByTarget_r(int meta_handle, UInt32 ms_timeout, string query_func_name);

        [DllImport(metacore, EntryPoint = "META_ERf_ForceTasSwitch_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_ForceTasSwitch_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdTasCfg req);

        [DllImport(metacore, EntryPoint = "META_MMRf_ForceTasSwitch_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_ForceTasSwitch_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmdCfgTas req, ref MMRfTestResultCfgTas cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_ForceTasState_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_ForceTasState_V7_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmdForceTasStateV7 req, ref MMRfTestResultForceTasStateV7 cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_QueryTasVerifyList_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_QueryTasVerifyList_V7_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultQueryTasVerifyListV7 cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_GetTasStateCfg_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_GetTasStateCfg_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmdFGetTasStateCfg req, ref MMRfTestResultGetTasStateCfg cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_GetUtasStateCfg_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_GetUtasStateCfg_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmdFGetTasStateCfg req, ref MMRfTestCmdGetTasStateCfgV5Cnf cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_GetRfCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_GetRfCapability_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmdRfCapabilityReq req, UInt32 requestLength, ref MMRfTestCmdRfCapabilityCnf resp, UInt32 responseLength);



        [DllImport(metacore, EntryPoint = "META_MMRf_QueryCarKitMappingInfo_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_QueryCarKitMappingInfo_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmdQueryCarKitMappingInfoReq req, ref MMRfTestCmdQueryCarKitMappingInfoCnf cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_QueryCarKitMappingInfoV7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_QueryCarKitMappingInfoV7_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmdQueryCarKitMappingInfoReq req, ref MMRfTestCmdQueryCarKitMappingInfoCnfV7 cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_GetShareRouteInfo_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_GetShareRouteInfo_V7_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultShareRouteInfo cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_SetAfcSetting_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_SetAfcSetting_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultGetAfcSetting req);
        
        [DllImport(metacore, EntryPoint = "META_MMRf_GetAfcSetting_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_GetAfcSetting_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultGetAfcSetting cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_SetAfcSettingV2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_SetAfcSettingV2_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV2 req);
        [DllImport(metacore, EntryPoint = "META_MMRf_GetAfcSettingV2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_GetAfcSettingV2_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV2 cnf);

        [DllImport(metacore, EntryPoint = "META_MMRf_SetAfcSettingV3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_SetAfcSettingV3_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV3 req);

        [DllImport(metacore, EntryPoint = "META_MMRf_GetAfcSettingV3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_GetAfcSettingV3_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestResultGetAfcSettingV3 cnf);

        [DllImport(metacore, EntryPoint = "META_ERf_TxPusch_V2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int  META_ERf_TxPusch_V2_r( int meta_handle, uint ms_timeout, ref ERfTestCmdPuschTxV2 req, ref uint pSyncStatus );


        [DllImport(metacore, EntryPoint = "META_Rf_GetRFID_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_GetRFID_r(int meta_handle, UInt32 ms_timeout,ref RFMod_ID cnf);
        [DllImport(metacore, EntryPoint = " META_Rf_QueryMSCapabilityEx2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_QueryMSCapabilityEx2_r(int meta_handle, UInt32 ms_timeout,ref RfMsCapabilityEx2_S p_ms_cap);
        [DllImport(metacore, EntryPoint = "META_Rf_QueryMSCapabilityEx3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_QueryMSCapabilityEx3_r(int meta_handle, UInt32 ms_timeout,ref RfMsCapabilityEx3_REQ_S req,ref RfMsCapabilityEx3_S ms_cap);
        [DllImport(metacore, EntryPoint = "META_Rf_Stop_Ex_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]

       //META_Rf_NSFT_Start_V5_r(const int meta_handle, unsigned int ms_timeout, const  Rf_NSFT_REQ_V5_T* req);
        public extern static int META_Rf_Stop_Ex_r(int meta_handle, UInt32 ms_timeout);
        [DllImport(metacore, EntryPoint = "META_Util_QueryTargetOptionInfo_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Util_QueryTargetOptionInfo_r(int meta_handle, UInt32 ms_timeout, UInt32[] info);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetRFID_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GetRFID_r(int meta_handle, UInt32 ms_timeout,ref URfTestResultRFID cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_QueryTargetCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_QueryTargetCapability_r(int meta_handle, UInt32 ms_timeout, ref UMTS_MsCapabilityEx cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetRfCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GetRfCapability_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdGetRfCapabilityReq req, UInt32 requestLength, ref URfTestResultGetRfCapabilityCnf cnf, UInt32 responseLength);
        [DllImport(metacore, EntryPoint = "META_3Grf_UbinModeSetup_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_UbinModeSetup_r(int meta_handle, UInt32 ms_timeout, byte ubin_fdd_mode_init);
        [DllImport(metacore, EntryPoint = "META_3Grf_TestStop_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_TestStop_r(int meta_handle, UInt32 ms_timeout, ref URfTestResultParam cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetRssiV3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GetRssiV3_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdGetRssiV3 req, ref URfTestResultGetRssiV3 cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetRssiV5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_3Grf_GetRssiV5_r(int meta_handle, UInt32 ms_timeout, URfTestCmdGetRssiV5[] req, URfTestResultGetRssiV5[] cnf);
        public extern static int META_3Grf_GetRssiV5_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdGetRssiV5 req, ref URfTestResultGetRssiV3 cnf);


        [DllImport(metacore, EntryPoint = "META_3Grf_GainSelectFromPowerV7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_3Grf_GainSelectFromPowerV7_r(int meta_handle, UInt32 ms_timeout, URfTestCmdPwrToGainV7[] req, URfTestResultPwrToGainV7[] cnf);
        public extern static int META_3Grf_GainSelectFromPowerV7_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdPwrToGainV7 req, ref URfTestResultPwrToGainV3 cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetRssiV7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GetRssiV7_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdGetRssiV7 req, ref URfTestResultGetRssiV7 cnf);
        [DllImport(metacore, EntryPoint = "META_ERf_GetRfCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_GetRfCapability_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdRfCapabilityReq req, UInt32 requestLength, ref ERfTestCmdRfCapabilityCnf resp, UInt32 responseLength );
        [DllImport(metacore, EntryPoint = "META_ERf_QueryCaConfigTableV3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_QueryCaConfigTableV3_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdCaConfig_V3 resp);
        [DllImport(metacore, EntryPoint = "META_ERf_QueryCaConfigTableV5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_QueryCaConfigTableV5_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdCaConfig_V5 resp);
        [DllImport(metacore, EntryPoint = "META_MMRf_EN_QueryConfig_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_MMRf_EN_QueryConfig_V7_r(int meta_handle, UInt32 ms_timeout, ref MMRfTestCmd_EN_QueryConfigV7 req,ref MMRfTestResult_EN_QueryConfigV7 cnf);
        [DllImport(metacore, EntryPoint = "META_ERf_StopTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_StopTestMode_r(int meta_handle, UInt32 ms_timeout);
        [DllImport(metacore, EntryPoint = "META_ERf_TxPusch_V5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_TxPusch_V5_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmd_StartPuschTxCaV5_ReqParam req,ref UInt32 pSyncStatus );
        [DllImport(metacore, EntryPoint = "META_ERf_StartMixRx_CaMode_V3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_StartMixRx_CaMode_V3_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdMixRx_CaMode_V3 req);
        [DllImport(metacore, EntryPoint = "META_ERf_StartMixRx_CaMode_V5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_StartMixRx_CaMode_V5_r(int meta_handle, UInt32 ms_timeout, ref ERFTestCmd_StartMixRxCaV5_ReqParam req);
        [DllImport(metacore, EntryPoint = "META_ERf_ResetCounter_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_ResetCounter_r(int meta_handle, UInt32 ms_timeout);
        [DllImport(metacore, EntryPoint = "META_ERf_GetMixRxReport_CaMode_V3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_GetMixRxReport_CaMode_V3_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdGetMixRxRpt_CaMode_V2 resp);
        [DllImport(metacore, EntryPoint = "META_ERf_GetMixRxReport_CaMode_V5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_GetMixRxReport_CaMode_V5_r(int meta_handle, UInt32 ms_timeout, ref ERfTestResultGetMixRxRpt_CaMode_V5 resp);
        [DllImport(metacore, EntryPoint = "META_NRf_GetRfCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NRf_GetRfCapability_r(int meta_handle, UInt32 ms_timeout,ref NRfTestCmdRfCapabilityReq req, UInt32 requestLength,ref NRfTestCmdRfCapabilityCnf resp, UInt32 responseLength );
        [DllImport(metacore, EntryPoint = "META_NRf_GetPuschTxDLFreq_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NRf_GetPuschTxDLFreq_V7_r(int meta_handle, UInt32 ms_timeout, ref NRfTestCmd_GetDlFreqV7 req, ref NRfTestResult_GetDlFreqV7 dlFrequency );
        [DllImport(metacore, EntryPoint = "META_NRf_TestStop_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NRf_TestStop_V7_r(int meta_handle, UInt32 ms_timeout);
        [DllImport(metacore, EntryPoint = "META_NRf_TxPusch_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NRf_TxPusch_V7_r(int meta_handle, UInt32 ms_timeout,ref NRfTestCmd_StartPuschTxCaV7 req,ref UInt32 pSyncStatus);
        [DllImport(metacore, EntryPoint = "META_NRf_StartMixRx_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NRf_StartMixRx_V7_r(int meta_handle, UInt32 ms_timeout,ref NRfTestCmd_StartMixRxCaV7 req,ref UInt32 pSyncStatus );
        [DllImport(metacore, EntryPoint = "META_NRf_ResetCounter_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NRf_ResetCounter_r(int meta_handle, UInt32 ms_timeout);
        [DllImport(metacore, EntryPoint = "META_NRf_GetMixRxReport_V7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_NRf_GetMixRxReport_V7_r(int meta_handle, UInt32 ms_timeout,ref NRfTestResult_GetMixRxReportV7 cnf );


        [DllImport(metacore, EntryPoint = "META_Rf_NSFT_Start_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_NSFT_Start_r(int meta_handle, UInt32 ms_timeout, ref Rf_NSFT_REQ_T req);
        [DllImport(metacore, EntryPoint = "META_Rf_NSFT_Start_V5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_NSFT_Start_V5_r(int meta_handle, UInt32 ms_timeout, ref Rf_NSFT_REQ_V5_T req);
        //[DllImport(metacore, EntryPoint = "META_Rf_NSFT_ChangeSettings_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_Rf_NSFT_ChangeSettings_r(int meta_handle, UInt32 ms_timeout, const Rf_NSFT_REQ_T* req);
        //[DllImport(metacore, EntryPoint = "META_Rf_NSFT_ChangeSettings_V5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_Rf_NSFT_ChangeSettings_V5_r(int meta_handle, UInt32 ms_timeout, const Rf_NSFT_REQ_V5_T* req);
        [DllImport(metacore, EntryPoint = "META_Rf_NSFT_StartRxLevel_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_NSFT_StartRxLevel_r(int meta_handle, UInt32 ms_timeout);
        [DllImport(metacore, EntryPoint = "META_Rf_NSFT_GetRxLevel_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_NSFT_GetRxLevel_r(int meta_handle, UInt32 ms_timeout, ref UInt16 rx_level);
        [DllImport(metacore, EntryPoint = "META_Rf_NSFT_GetRxLevel_V5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_NSFT_GetRxLevel_V5_r(int meta_handle, UInt32 ms_timeout,ref UInt16[] rx_level);
        //[DllImport(metacore, EntryPoint = "META_Rf_NSFT_GetRxQual_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_Rf_NSFT_GetRxQual_r(int meta_handle, UInt32 ms_timeout, UInt16 ber_decile, string rx_qual);
        [DllImport(metacore, EntryPoint = "META_Rf_NSFT_ConfigSBER_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_NSFT_ConfigSBER_r(int meta_handle, UInt32 ms_timeout, UInt32 test_frame_count);
        [DllImport(metacore, EntryPoint = "META_Rf_NSFT_GetSBER_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_Rf_NSFT_GetSBER_r(int meta_handle, UInt32 ms_timeout, ref RF_NSFT_SBERResult_T sber_result);


       //======================wcda
        [DllImport(metacore, EntryPoint = "META_3Grf_SetTxPaDriftCompEnable_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_SetTxPaDriftCompEnable_r(int meta_handle, UInt32 ms_timeout, byte is_PaDrift);
        [DllImport(metacore, EntryPoint = "META_3Grf_NSFT_StartEx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_NSFT_StartEx_r(int meta_handle, UInt32 ms_timeout, ref UL1D_RF_NSFT_REQ_T req,ref UMTS_NSFTLinkStatusReport cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_Set_Initial_Cellpower_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_Set_Initial_Cellpower_r(int meta_handle, UInt32 ms_timeout, int confg_cell/*uints:qdbm*/);
        [DllImport(metacore, EntryPoint = "META_3Grf_NSFT_SetILPCStep_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_NSFT_SetILPCStep_r(int meta_handle, UInt32 ms_timeout, byte step);

        [DllImport(metacore, EntryPoint = "META_3Grf_NSFT_GetBitCountForSingleEndedBER_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_NSFT_GetBitCountForSingleEndedBER_r(int meta_handle, UInt32 ms_timeout, ref UL1D_RF_NSFT_GET_BIT_CNT_FOR_BER_CNF_T cnf);

        [DllImport(metacore, EntryPoint = "META_3Grf_Rssi_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_Rssi_r(int meta_handle, UInt32 ms_timeout,ref URfTestCmdRSSI req, ref URfTestResultRSSI cnf);

        [DllImport(metacore, EntryPoint = "META_3Grf_RxDRssi_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_RxDRssi_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdRSSI req, ref URfTestResultRSSIRxD cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_Lpm_Rssi_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_Lpm_Rssi_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdLPMRSSI req, ref URfTestResultRSSI cnf);

        [DllImport(metacore, EntryPoint = "META_3Grf_Lpm_RxDRssi_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_Lpm_RxDRssi_r(int meta_handle, UInt32 ms_timeout,ref URfTestCmdLPMRSSI req, ref URfTestResultRSSIRxD cnf);

        [DllImport(metacore, EntryPoint = "META_3Grf_ELNA_Rssi_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_ELNA_Rssi_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdELNARSSI req, ref URfTestResultELNARSSI cnf);

        [DllImport(metacore, EntryPoint = "META_3Grf_GainSelectFromPowerV3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GainSelectFromPowerV3_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdPwrToGainV3 req, ref URfTestResultPwrToGainV3 cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GainSelectFromPowerV5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GainSelectFromPowerV5_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdPwrToGainV5 req, ref URfTestResultPwrToGainV3 cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetCalInfoV3_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GetCalInfoV3_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdGetMdCalInfoV3 req, ref URfTestResultGetMdCalInfoV3 cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetCalInfoV5_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GetCalInfoV5_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdGetMdCalInfoV5 req, ref URfTestResultGetMdCalInfoV5 cnf);
        [DllImport(metacore, EntryPoint = "META_3Grf_GetCalInfoV7_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_3Grf_GetCalInfoV7_r(int meta_handle, UInt32 ms_timeout, ref URfTestCmdGetMdCalInfoV7 req, ref URfTestResultGetMdCalInfoV7 cnf);


        [DllImport(metacore, EntryPoint = "META_ERf_QueryCaConfigTable_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_QueryCaConfigTable_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdCaConfig resp);
        //[DllImport(metacore, EntryPoint = "META_ERf_QueryCaConfigTableV2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_ERf_QueryCaConfigTableV2_r(int meta_handle, UInt32 ms_timeout, ERfTestCmdCaConfig_V2* resp);
        [DllImport(metacore, EntryPoint = "META_ERf_StartTraditionalTxNSFT_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_StartTraditionalTxNSFT_r(int meta_handle, ref ERfTestCmdPuschTx req, UInt32 ms_timeout, ref UInt32 pSyncStatus );
        //[DllImport(metacore, EntryPoint = "META_ERf_TxPusch_V2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_ERf_TxPusch_V2_r(int meta_handle, UInt32 ms_timeout, const ERfTestCmdPuschTxV2* req, UInt32[] pSyncStatus );
        //[DllImport(metacore, EntryPoint = "META_ERf_TxCfgUpdate_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int META_ERf_TxCfgUpdate_r(int meta_handle, UInt32 ms_timeout, const ERfTestCmdTxCfgUpdt* req);
        [DllImport(metacore, EntryPoint = "META_ERf_StartMixRx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_StartMixRx_r(int meta_handle, ref ERfTestCmdMixRx req, UInt32 ms_timeout );
        [DllImport(metacore, EntryPoint = "META_ERf_SetLnaSrx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_SetLnaSrx_r(int meta_handle, ref ERfTestCmdSetRxCommonCfg req, UInt32 ms_timeout);
        [DllImport(metacore, EntryPoint = "META_ERf_GetMixRxReport_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int META_ERf_GetMixRxReport_r(int meta_handle, UInt32 ms_timeout, ref ERfTestCmdGetMixRxRpt resp);
        //[DllImport(metacore, EntryPoint = "SP_META_FM_SetMonoOrStereo_Blend", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public extern static int SP_META_FM_SetMonoOrStereo_Blend(UInt32 ms_timeout, FM_MONO_STEREO_BLEND_REQ_T* req);


        [DllImport(metacore, EntryPoint = "META_MMC2K_GetTargetGenType_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_MMC2K_GetTargetGenType_r(int metaHandle, ref UInt32 type);
       [DllImport(metacore, EntryPoint = "META_MMC2K_UbinModeSetup_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       extern static int META_MMC2K_UbinModeSetup_r(int metaHandle, uint msTimeout, byte mode);


       [DllImport(metacore, EntryPoint = "META_MMC2K_UbinModeSetup_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       extern static int META_MMC2K_QueryTargetCapability_r(int metaHandle, uint msTimeout, ref C2kMsCapability msCapability);

        [DllImport(metacore, EntryPoint = "META_MMC2K_NsftEnterTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_MMC2K_NsftEnterTestMode_r(int metaHandle, uint msTimeout, ref C2kNsftCmdTestMode req, ref C2kNsftResultStatus cnf);
       [DllImport(metacore, EntryPoint = "META_MMC2K_NsftExitTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_MMC2K_NsftExitTestMode_r(int metaHandle, uint msTimeout, ref C2kNsftCmdTestMode req,ref C2kNsftResultStatus cnf);
        [DllImport(metacore, EntryPoint = "META_MMC2K_NsftPowerUp_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_MMC2K_NsftPowerUp_r(int metaHandle, uint msTimeout, ref C2kNsftCmdPowerUp req, ref C2kNsftResultStatus cnf);
        [DllImport(metacore, EntryPoint = "META_MMC2K_NsftSetTxPower_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_MMC2K_NsftSetTxPower_r(int metaHandle, uint msTimeout, ref C2kNsftCmdSetTxPower req,ref C2kNsftResultStatus cnf);
        [DllImport(metacore, EntryPoint = "META_MMC2K_NsftGetFer_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_MMC2K_NsftGetFer_r(int metaHandle, uint msTimeout, ref C2kNsftCmdFer req,ref C2kNsftResultFer cnf);
        [DllImport(metacore, EntryPoint = "META_MMC2K_NsftGetRssi_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_MMC2K_NsftGetRssi_r(int metaHandle, uint msTimeout, ref C2kNsftCmdRssi req, ref C2kNsftResultRssi cnf);
        [DllImport(metacore, EntryPoint = "META_C2K_NsftSetTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_NsftSetTestMode_r(int metaHandle, uint msTimeout);
       [DllImport(metacore, EntryPoint = "META_C2K_NsftExitTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_NsftExitTestMode_r(int metaHandle, uint msTimeout);

       [DllImport(metacore, EntryPoint = "META_C2K_QueryTargetCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_QueryTargetCapability_r(int metaHandle, uint msTimeout,ref C2K_MS_CAPABILITY msCapability);
        [DllImport(metacore, EntryPoint = "META_C2K_PsPower_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_PsPower_r(int metaHandle, uint msTimeout, bool enable);
        [DllImport(metacore, EntryPoint = "META_C2K_NsftPowerUp_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_NsftPowerUp_r(int metaHandle, uint msTimeout, uint band, UInt16 channel, uint walshCode, uint rc, uint numFrames, bool afcEnable);
        [DllImport(metacore, EntryPoint = "META_C2K_NsftGetFer_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_NsftGetFer_r(int metaHandle, uint msTimeout, ref UInt32 badFrames,ref UInt32 totalFrames);
        [DllImport(metacore, EntryPoint = "META_C2K_NsftTxTrafficChannel_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_NsftTxTrafficChannel_r(int metaHandle, uint msTimeout, uint rc, uint powerCtrlMode, double txPower);
       [DllImport(metacore, EntryPoint = "META_C2K_NsftRssi_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int META_C2K_NsftRssi_r(int metaHandle, uint msTimeout, uint band, UInt16 channel, ref C2K_NSFT_RSSI_CNF cnf);


        //[DllImport(metacore, EntryPoint = "META_MMC2K_RegisterHandler_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] extern static int META_MMC2K_RegisterHandler_r(int meta_handle);
        [DllImport(metacore, EntryPoint = "AST_CAL_get_chipid_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int AST_CAL_get_chipid_r(int meta_handle, int ms_timeout, ref ushort BB_chipID, ref ushort RF_chipID);

        [DllImport(metacore, EntryPoint = "AST_CAL_get_capability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int AST_CAL_get_capability_r(int meta_handle, int ms_timeout, UInt16 capability, UInt16 band_support);

        [DllImport(metacore, EntryPoint = "AST_GetRfCapability_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int AST_GetRfCapability_r(int meta_handle, UInt32 ms_timeout,ref T_AST_RF_CAPABILITY_RESULT res, /* [OUT] the RF capability provided by L1 */        UInt32 res_length /* [IN] length of the input buffer */		);
        
       [DllImport(metacore, EntryPoint = "AST_CAL_QueryAPISupported_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int AST_CAL_QueryAPISupported_r(int meta_handle, int ms_timeout,     string func_name		);

       [DllImport(metacore, EntryPoint = "AST_UBIN_MODE_SETUP_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       extern static int AST_UBIN_MODE_SETUP_r(int meta_handle, UInt32 ms_timeout, ref T_AST_RFCAL_UBIN_MODE ubin_tdd_mode_init);
       [DllImport(metacore, EntryPoint = "AST_CAL_tl1_reset_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int AST_CAL_tl1_reset_r(int meta_handle, int ms_timeout);
       [DllImport(metacore, EntryPoint = "AST_NSFT_Start_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       extern static int AST_NSFT_Start_r(int meta_handle, int ms_timeout, 
           UInt16 dpch_freq,  /* UARFCN*/		
           UInt16  	cp_id,  /* CPID for the midamble selection*/		
           byte  		dl_timeslot,  /* DL Timeslot, support 1 TS only(0 for TS0,1 for TS1бн)*/		
           UInt32 		dl_code,  /* DL TS channel code bitmap										bit0 is C16-1                                         bit1 is C16-2                                         ...                                        bit 15 is C16-16                                         bit16 is C8-1                                         ...                                        bit23 is C8-8                                         bit24 is C4-1										...                                        bit27 is C4-4                                         bit28 is C2-1                                         bit29 is C2-2                                         bit30 is C1-1                                        bit31 is reserved.*/		
               byte     	ul_timeslot,// 0-6,UL Timeslot, support 1 TS only(0 for TS0,1 for TS1бн)		
                   UInt32    	ul_code, /* UL TS channel code bitmap										bit0 is C16-1                                         bit1 is C16-2                                         ...                                        bit 15 is C16-16                                         bit16 is C8-1                                         ...                                        bit23 is C8-8                                         bit24 is C4-1										...                                        bit27 is C4-4                                         bit28 is C2-1                                         bit29 is C2-2                                         bit30 is C1-1                                        bit31 is reserved.*/		
                       bool     			single_end_loop_enable, //switch for the single ended BER		
                           bool 			b_afc_dac_valid,//indicate if using the AFC DAC below		
                               UInt16    	afc_dac,    // AFC DAC set by META		
                               byte     	loopbackType,  // Loopback type  BER/BLER type, 0 for BER, 1 for BLER		
                                   bool     			b_bit_pattern_allzero,  // all 1 or all zero		
                                       byte	ul_dpdch_pwr,  // UL target power for the output loop power control 		
                                           byte		ul_ma
                                             	
                   );
        [DllImport(metacore, EntryPoint = "AST_NSFT_Stop_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]  
       extern static int AST_NSFT_Stop_r(int meta_handle, int ms_timeout);
        [DllImport(metacore, EntryPoint = "AST_NSFT_GetRscp_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
       extern static int AST_NSFT_GetRscp_r(int meta_handle, int ms_timeout,ref short rscp);
       [DllImport(metacore, EntryPoint = "AST_NSFT_GetBitCount_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       extern static int AST_NSFT_GetBitCount_r(int meta_handle, int ms_timeout, ref UInt32 total_bits, //total bits received between 2 GetBER commands		
           ref UInt32 error_bits //total error bits received between 2 GetBER commands		
               );


        #endregion
    }
}
