using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using vivo_Auto_Test.MainTestQueue.WiFi.ICType.MTK.MTK_Librarys;
using static vivo_Auto_Test.MainTestQueue.WiFi.ICType.MTK.MTK_Librarys.MTK_Library;
using System.Windows;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace vivoautotestwifi.MainQueue.ICType.MTK
{
    /// <summary>
    /// 
    /// </summary>
    public class MT6635
    {
        public Dictionary<string, uint> preambleS = null;
        public Dictionary<string, uint> rateS = null;
        public Dictionary<string, uint> rateM = null;
        WIFI_RX_STASTIC_AX rxInfo = new WIFI_RX_STASTIC_AX();
        public Dictionary<string, string> selectPreamble = new Dictionary<string, string>();
        int ms = 0;

        MTK_Library mtk = new MTK_Library();

        private byte txPath = 3;
        private UInt32 band = 0;
        private UInt32 channel = 1;
        private UInt32 cBW = 0;
        private UInt32 dBW = 0;
        private UInt32 channelBand = 0;
        private UInt32 txPower = 44;
        private UInt32 preamble = 0;
        private UInt32 rate = 0;
        public UInt32 okData = 999999;
        public UInt32 nss = 1;
        public UInt32 FEC = 0;
        public UInt32 isShortGI = 0;
        public UInt32 swap = 0;
        public int isGetMcr = 0;

        public byte TxPath { get => txPath; set => txPath = value; }
        public uint Band { get => band; set => band = value; }
        public uint Channel { get => channel; set => channel = value; }
        public uint CBW { get => cBW; set => cBW = value; }
        public uint DBW { get => dBW; set => dBW = value; }
        public uint ChannelBand { get => channelBand; set => channelBand = value; }
        public uint TxPower { get => txPower; set => txPower = value; }
        public uint Preamble { get => preamble; set => preamble = value; }
        public uint Rate { get => rate; set => rate = value; }

        public MT6635()
        {
            createPreambleS();
            createRateS();
            createRateM();
            getConfigInfo();
        }

        public void runTxTest() {
            try
            {
                //openAdapter();
                //SetBandMode();
                setTxPathv2();
                setRxPathv2();
                AntSwapSet();
                DBDCSetChannel();
                DBDCSetTXContent();
                setTxPowerExt();
                if (this.preamble == 10)
                {
                    setRUSettings();
                }
                DBDCStartTX();
            }
            catch (Exception ex)
            {
                Log.GetInstance().d("MT6635", "参数设置异常" + ex.ToString());
                Log.GetInstance().d("MT6635", "参数重设！");
                DBDCStopTX();
                runTxTest();
            }
            finally
            {
                Log.GetInstance().d("MT6635", "数据发送中...");
            }
        }

        public void runRxTest()
        {
            try
            {
                //openAdapter();
                //SetBandMode();
                setTxPathv2();
                setRxPathv2();
                AntSwapSet();
                DBDCSetChannel();
                //ResetTxRxCounter();//封装rssi异常
                //DBDCStartRX();

                int mr = mtk.SP_META_WiFi_HQA_ResetTxRxCounterBand_r(5000, 1);
                byte pMac = 0;
                mr = mtk.SP_META_WiFi_HQA_DBDCStartRXExt_r(5000, Band, TxPath, ref pMac, 0, Preamble, 0);
            }
            catch (Exception ex)
            {
                Log.GetInstance().d("MT6635", "参数设置异常" + ex.ToString());
                Log.GetInstance().d("MT6635", "参数重设！");
                DBDCStopRX();
                runRxTest();
            }
            finally
            {
                Log.GetInstance().d("MT6635", "数据接收中...");
            }
        }

        #region 基本业务逻辑
        public void connetWithUSB(string logPATH)
        {
            string errMsg = null;
            MessageBox.Show("MT6635,进入meta模式，点击确定后 你将有1分钟时间连接USB！");
            ms = mtk.ConnetWithUSB(logPATH, out errMsg);
            if (ms != 0)
            {
                Log.GetInstance().d("MT6635", "连接失败！");
                connetWithUSB(logPATH);
                Log.GetInstance().d("MT6635", "尝试重新连接！");
            }
            else
            {
                Log.GetInstance().d("MT6635", "连接成功，进入meta模式！");
            }

            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"MainTestQueue\WiFi\config\iwpriv_L_64\OpenGetMCR.txt", Encoding.UTF8, true))
            {
                isGetMcr = Convert.ToInt32(sr.ReadToEnd().ToString());
            }
        }

        public void disconnect()
        {
            Thread.Sleep(100);
            Log.GetInstance().d("MT6635", "断开连接！");
            string errMsg = null;
            mtk.Disconnect(ref errMsg);
            Log.GetInstance().d("MT6635", "断开信息：" + errMsg);
        }

        public void openWiFi()
        {
            Thread.Sleep(100);
            ms = mtk.SP_META_WIFI_OPEN_r(30000);
            Log.GetInstance().d("MT6635", "开启WIFI -> SP_META_WIFI_OPEN_r()=" + ms + "");
        }

        public void openAdapter()
        {
            Thread.Sleep(100);
            ms = mtk.SP_META_WiFi_HQA_OpenAdapter_r(15000);
            Log.GetInstance().d("MT6635", "开启Adapter -> SP_META_WiFi_HQA_OpenAdapter_r(5000)=" + ms + "");
        }

        public void GetChipVersion()
        {
            Thread.Sleep(100);
            UInt32 value = 0;
            ms = mtk.SP_META_WiFi_GetChipVersion_r(18000, ref value);
            Log.GetInstance().d("MT6635", "GetChipVersion->SP_META_WiFi_GetChipVersion_r(18000,ref value" + value + ")=" + ms);
            if (value == 0x5921)
                Log.GetInstance().e("MT6635", "GetChipVersion-> 不支持此芯片！");
        }

        public void ReadMCR32()
        {
            Thread.Sleep(100);
            UInt32 u4Addr = 0;
            UInt32 u4Value = 0;
            ms = mtk.SP_META_WiFi_readMCR32_r(1200,u4Addr,ref u4Value);
            Log.GetInstance().d("MT6635", "ReadMCR32->SP_META_WiFi_readMCR32_r(1200,offset:"+u4Addr+ ",ref u4Value" + u4Value + ")=" + ms);
        }

        public void GetChipCapability()
        {
            Thread.Sleep(100);
            CapBuffer CapBuffer = new CapBuffer();
            CapBuffer.ext = new EXT_CAP();
            CapBuffer.phy = new PHY_CAP();
            CapBuffer.ext.ext_value = new EXT_Value();
            CapBuffer.phy.phy_value = new PHY_Value();
            UInt32 CapBufferLength = 0;
            ms =mtk.SP_META_WiFi_HQA_GetChipCapability_r(5000,ref CapBuffer,ref CapBufferLength);
            Log.GetInstance().d("MT6635", "GetChipCapability->SP_META_WiFi_HQA_GetChipCapability_r(5000,CapBuffer:"+CapBuffer.version+",ref CapBufferLength" + CapBufferLength + ")=" + ms);
        }

        public void QueryConfig()
        {
            Thread.Sleep(100);
            ENUM_CFG_SRC_TYPE_T bufType=ENUM_CFG_SRC_TYPE_T.CFG_SRC_TYPE_AUTO;
            ms = mtk.SP_META_WiFi_QueryConfig_r(5000, ref bufType);
            Log.GetInstance().d("MT6635", "QueryConfig->SP_META_WiFi_QueryConfig_r(5000,ref bufType" + bufType + ")=" + ms);
        }

        public void SetTestMode()
        {
            ms = mtk.SP_META_WiFi_setTestMode_r(5000);
            Log.GetInstance().d("MT6635", "SetTestMode -> SP_META_WiFi_setTestMode_r(5000)=" + ms + "");
        }

        public void SwitchAntenna()
        {
            ms = mtk.SP_META_WiFi_switchAntenna_r(5000, 0);
            Log.GetInstance().d("MT6635", "SwitchAntenna -> SP_META_WiFi_switchAntenna_r(5000,0)=" + ms + "");
        }

        public void AntSwapCap()
        {
            Thread.Sleep(100);
            ms = mtk.SP_META_WiFi_HQA_AntSwapCap_r(5000,band,ref swap);
            Log.GetInstance().d("MT6635", "AntSwapCap -> SP_META_WiFi_HQA_AntSwapCap_r(5000,band:"+band+",ref swap:"+swap+")="+ms);
        }

        public void AntSwapSet()
        {
            if (swap == 2)
            {
                Thread.Sleep(100);
                ms = mtk.SP_META_WiFi_HQA_AntSwapSet_r(5000, band, nss);
                Log.GetInstance().d("MT6635", "AntSwapSet -> SP_META_WiFi_HQA_AntSwapSet_r(5000,band:" + band + ",ant:" + nss + ")=" + ms);
            }
            else
            {
                Log.GetInstance().d("MT6635","AntSwapSet -> swap:"+swap+"!=2 不设置");
            }
        }

        public void SetIwpriv()
        {
            Process process= new Process();
            process.StartInfo.FileName = "adb_push_tools.bat";
            process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"MainTestQueue\WiFi\config\iwpriv_L_64\";
            process.Start();
        }

        public void GetMCR(string channel)
        {
            Process process = new Process();
            process.StartInfo.FileName = "TestEnd.bat";
            process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"MainTestQueue\WiFi\config\iwpriv_L_64\";
            process.Start();
            Thread.Sleep(100);
            string mcr = "";
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"MainTestQueue\WiFi\config\iwpriv_L_64\mcrinfo.txt", Encoding.UTF8, true))
            {
                mcr = sr.ReadToEnd().ToString();
            }
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"MainTestQueue\WiFi\config\iwpriv_L_64\mcrTestResult.txt", true, Encoding.UTF8))
            {
                sw.WriteLine(channel+":"+mcr+"\n");
                sw.Close();
            }
        }

        public void SetBandMode()
        {
            Thread.Sleep(100);
            ms = mtk.SP_META_WiFi_HQA_SetBandMode_r(5000,1,1,0,0);
            Log.GetInstance().d("MT6635", "SetBandMode -> SP_META_WiFi_HQA_SetBandMode_r(5000,1,1,0,0)=" + ms + "");
        }

        public void setTxPathv2()
        {
            Thread.Sleep(100);
            if (swap == 2)
            {
                ms = mtk.SP_META_WiFi_HQA_SetTxPathv2_r(5000, 1, band);
                Log.GetInstance().d("MT6635", "设置TxPathv2 -> SP_META_WiFi_HQA_SetTxPathv2_r（5000,TxPath:" + 1 + ",band:" + band + ")=" + ms + "");
            }
            else
            {
                ms = mtk.SP_META_WiFi_HQA_SetTxPathv2_r(5000, txPath, band);
                Log.GetInstance().d("MT6635", "设置TxPathv2 -> SP_META_WiFi_HQA_SetTxPathv2_r（5000,TxPath:" + txPath + ",band:" + band + ")=" + ms + "");
            }            
        }

        public void setRxPathv2()
        {
            Thread.Sleep(100);
            if (swap == 2)
            {
                ms = mtk.SP_META_WiFi_HQA_SetRxPathv2_r(5000, 1, band);
                Log.GetInstance().d("MT6635", "设置RxPathv2 -> SP_META_WiFi_HQA_SetRxPathv2_r（5000,RxPath:" + 1 + ",band:" + band + ")=" + ms + "");
            }
            else
            {
                ms = mtk.SP_META_WiFi_HQA_SetRxPathv2_r(5000, txPath, band);
                Log.GetInstance().d("MT6635", "设置RxPathv2 -> SP_META_WiFi_HQA_SetRxPathv2_r（5000,RxPath:" + txPath + ",band:" + band + ")=" + ms + "");
            }
        }

        public void DBDCSetChannel()
        {
            Thread.Sleep(100);
            ms =mtk.SP_META_WiFi_HQA_DBDCSetChannel_r(5000, band, channel,0,cBW, dBW,0, 0, channelBand, 0);
            Log.GetInstance().d("MT6635", "设置DBDCSetChannel -> SP_META_WiFi_HQA_DBDCSetChannel_r（5000,band:" + band + ",channel:" + channel + ",0,CBW:" + cBW + ",DBW:" + dBW + ",0,0,channelBand:" + channelBand + ",0)="+ms+"");
        }

        public void ResetTxRxCounter()
        {
            int mr = mtk.SP_META_WiFi_HQA_ResetTxRxCounterBand_r(5000, 1);
            //Log.GetInstance().d("MT6635", "ResetTxRxCounter -> SP_META_WiFi_HQA_ResetTxRxCounterBand_r（5000,band:" + 1 + ")=" + mr);
        }

        public void DBDCSetTXContent()
        {
            Thread.Sleep(100);
            string pSourceAddr = "";
            string pDestAddr = "";
            string pBSSID = "";
            string pPayloadContent = "";
            
            //txlenth=0
            ms=mtk.SP_META_WiFi_HQA_DBDCSetTXContent_r(5000, band, 0, 0, 0, 2, 1024, 1, ref pSourceAddr, ref pDestAddr, ref pBSSID, ref pPayloadContent, 0, 0, 0);
            Log.GetInstance().d("MT6635", "设置DBDCSetTXContent -> SP_META_WiFi_HQA_DBDCSetTXContent_r（5000,band:" + band + ",0,0,0,2,1024,1,ref pSourceAddr:"+ pSourceAddr + ",ref pDestAddr:" + pDestAddr + ",ref pBSSID:" + pBSSID + ",ref pPayloadContent:" + pPayloadContent + ",0,0,0)=" +ms+"");
        }

        public void setTxPowerExt()
        {
            Thread.Sleep(100);
            if (swap == 2)
            {
                ms = mtk.SP_META_WiFi_HQA_SetTxPowerExt_r(5000, txPower, band, channel, channelBand, 1);
                Log.GetInstance().d("MT6635", "设置TxPowerExt -> SP_META_WiFi_HQA_SetTxPowerExt_r（5000,TxPower:" + txPower + ",band:" + band + ",channel:" + channel + ",channelBand" + channelBand + "," + 1 + ")=" + ms + "");
            }
            else
            {
                ms = mtk.SP_META_WiFi_HQA_SetTxPowerExt_r(5000, txPower, band, channel, channelBand, txPath);
                Log.GetInstance().d("MT6635", "设置TxPowerExt -> SP_META_WiFi_HQA_SetTxPowerExt_r（5000,TxPower:" + txPower + ",band:" + band + ",channel:" + channel + ",channelBand" + channelBand + "," + txPath + ")=" + ms + "");
            }            
        }

        public void setRUSettings()
        {
            Dictionary<int, UInt32> Allocation=new Dictionary<int, UInt32>();
            Allocation.Add(13, 0xB0);
            Allocation.Add(14, 0xB8);
            Allocation.Add(15, 0xC0);

            Dictionary<int, UInt32> RUIndex = new Dictionary<int, UInt32>();
            RUIndex.Add(13, 61);
            RUIndex.Add(14, 65);
            RUIndex.Add(15, 67);


            RUSettings ruSettings = new RUSettings();
            UInt32 plenSeg0 = (UInt32)System.Runtime.InteropServices.Marshal.SizeOf(ruSettings);
            string pRUSettingsSeg1 = null;
            UInt32 plenSeg1 = 0;
                        
            ruSettings.Seg0Cat = this.CBW+13;//Category BW20 13 61 / BW40 14 65 /BW80 15 67
            ruSettings.Seg0Alloc = Allocation[(int)this.CBW + 13];//Allocation
            ruSettings.Seg0RUIdx = RUIndex[(int)this.CBW+13];//RU Index
            ruSettings.Seg0StalD = 0;//STA ID
            ruSettings.Seg0LDPC = this.FEC;//FEC
            ruSettings.Seg0Stream = 1;//不知道
            ruSettings.Seg0TxPwr = this.TxPower;
            ruSettings.Seg0Nss = this.nss;
            ruSettings.Seg0Rate = this.rate;
            ruSettings.Seg0Length = 1024;//Pkt len
            ruSettings.Seg0MUNss = this.nss;

            ms = mtk.SP_META_WiFi_HQA_SetRUSettings_r(5000,this.band,1,0,ref ruSettings,ref plenSeg0,pRUSettingsSeg1,ref plenSeg1);
            Log.GetInstance().d("MT6635", "设置setRUSettings -> SP_META_WiFi_HQA_SetRUSettings_r（5000,band:" + band + ",Seg0Count:1,Seg1Count:0,ruSettings:" + ruSettings+ " pRUSettingsSeg1:"+ pRUSettingsSeg1 + " plenSeg1:"+ plenSeg1+"=" + ms);
        }

        public void DBDCStartTX()
        {
            Thread.Sleep(100);
            UInt32 pReserved0 = 0;
            UInt32 pReserved1 = 0;
            UInt32 pReserved2 = 0;
            if (swap == 2)
            {
                ms = mtk.SP_META_WiFi_HQA_DBDCStartTX_r(5000, band, 0, preamble, rate, txPower, 0, FEC, 0, 0, 0, 10, isShortGI, 1, nss, ref pReserved0, ref pReserved1, ref pReserved2);
                Log.GetInstance().d("MT6635", "开始测试DBDCStartTX -> SP_META_WiFi_HQA_DBDCStartTX_r (5000, band:" + band + ", 0, preamble:" + preamble + ", rete:" + rate + ", TxPower:" + 1 + ", nss:"+nss+", LDPC:" + FEC + ", 0, 0, 0, 10, 0, TxPath:" + txPath + ", " + 1 + ", 0, 0, 0)=" + ms + "");
            }
            else
            {
                ms = mtk.SP_META_WiFi_HQA_DBDCStartTX_r(5000, band, 0, preamble, rate, txPower, 0, FEC, 0, 0, 0, 10, isShortGI, txPath, nss, ref pReserved0, ref pReserved1, ref pReserved2);
                Log.GetInstance().d("MT6635", "开始测试DBDCStartTX -> SP_META_WiFi_HQA_DBDCStartTX_r (5000, band:" + band + ", 0, preamble:" + preamble + ", rete:" + rate + ", TxPower:" + txPower + ",nss:"+nss+", LDPC:" + FEC + ", 0, 0, 0, 10, 0, TxPath:" + txPath + ", " + nss + ", 0, 0, 0)=" + ms + "");
            }
            
        }

        public void DBDCStopTX()
        {
            Thread.Sleep(100);
            UInt32 pReserved0 = 0;
            UInt32 pReserved1 = 0;
            UInt32 pReserved2 = 0;            
            ms=mtk.SP_META_WiFi_HQA_DBDCStopTX_r(5000, 0, ref pReserved0, ref pReserved1, ref pReserved2);
            Log.GetInstance().d("MT6635", "停止测试DBDCStopTX -> SP_META_WiFi_HQA_DBDCStopTX_r（5000,0,0,0,0)="+ms+"");
        }

        public void closeAdapter()
        {
            Thread.Sleep(100);
            ms =mtk.SP_META_WiFi_HQA_CloseAdapter_r(5000);
            Log.GetInstance().d("MT6635", "关闭Adapter -> SP_META_WiFi_HQA_CloseAdapter_r(5000)="+ms+"");
        }

        public void DBDCStartRX()
        {
            Thread.Sleep(100);
            byte pMac = 0;
            ms=mtk.SP_META_WiFi_HQA_DBDCStartRXExt_r(5000, band, txPath, ref pMac, 0, preamble, 0);
            Log.GetInstance().d("MT6635", "RX测试开始 -> SP_META_WiFi_HQA_DBDCStartRXExt_r(5000, band:" + band + ", txPath:" + txPath + ", ref pMac:"+pMac+", 0, preamble:" + preamble + ", 0)=" + ms + "");
        }

        public void getAFactor()
        {
            UInt32 FactorValue=0;
            UInt32 LDPCExtraSym=0;
            UInt32 PEDisamp=0;
            UInt32 TXPEValue=0;
            UInt32 LSIG=0;
            ms =mtk.SP_META_WiFi_HQA_GetAFactor_r(5000,ref FactorValue,ref LDPCExtraSym,ref PEDisamp,ref TXPEValue,ref LSIG);
            Log.GetInstance().d("MT6635", "getAFactor -> SP_META_WiFi_HQA_GetAFactor_r()=" + ms + " FactorValue:" + FactorValue);
        }

        public WIFI_RX_STASTIC_AX getRxInfo()
        {
            rxInfo.BandInfo = new WIFI_RX_BANDINFO_AX[1];
            rxInfo.AntInfo = new WIFI_RX_ANTINFO_AX[4];
            rxInfo.CommonInfo = new WIFI_RX_COMMONINFO_AX[1];
            rxInfo.UsrInfo = new WIFI_RX_USERINFO_AX[16];
            rxInfo.AccInfo = new WIFI_RX_ACCINFO();
            ms = mtk.SP_META_WiFi_HQA_GetAXRXInfoAll_r(5000, 0X0F, band, ref rxInfo);
            short rssi;
            if (swap == 2)
            {
                rssi = (short)(rxInfo.AntInfo[0].rssi);
            }
            else
            {
                rssi = (short)(rxInfo.AntInfo[nss].rssi);
            }

            Log.GetInstance().d("MT6635", "获取RX信息 -> SP_META_WiFi_HQA_GetAXRXInfoAll_r(5000, 0X0F,band:" + band + ", ref rxInfo)=" + ms + "---rssi0:"+ (short)(rxInfo.AntInfo[0].rssi)+"----rssi1:"+ (short)(rxInfo.AntInfo[1].rssi) + " ok:"+ rxInfo.AccInfo.DLL_RXOK+" err:"+ rxInfo.AccInfo.DLL_AllFCSErr);
            if (okData == rxInfo.AccInfo.DLL_RXOK)
            {
                Log.GetInstance().e("MT6635", "RX信息异常重设手机");
                DBDCStopRX();
                closeAdapter();
                openAdapter();
                SetBandMode();
            }
            return rxInfo;
        }

        public void DBDCStopRX()
        {
            Thread.Sleep(100);
            ms =mtk.SP_META_WiFi_HQA_DBDCStopRX_r(5000, band);
            Log.GetInstance().d("MT6635", "RX测试停止 -> SP_META_WiFi_HQA_DBDCStopRX_r(5000, band:" + band + ")=" + ms + "");
        }

        #endregion

        private void createPreambleS()
        {
            preambleS = new Dictionary<string, uint>();
            preambleS.Add("B",0);//CCK
            preambleS.Add("A", 1);//OFDM
            preambleS.Add("G", 1);//OFDM
            preambleS.Add("N", 2);//HT_MM
            preambleS.Add("AC", 4);//VHT
            preambleS.Add("AX", 8);//HE_SU
            preambleS.Add("AX_SU", 8);//HE_SU
            preambleS.Add("AX_ER", 9);//HE_ER
            preambleS.Add("AX_TB", 10);//HE_TB
            preambleS.Add("AX_MU", 11);//HE_MU
        }

        private void createRateS()
        {
            rateS = new Dictionary<string, uint>();
            rateS.Add("1Mbit", 0);
            rateS.Add("2Mbit", 1);
            rateS.Add("5", 2);
            rateS.Add("11Mbit", 3);

            rateS.Add("6Mbit", 0);
            rateS.Add("9Mbit", 1);
            rateS.Add("12Mbit", 2);
            rateS.Add("18Mbit", 3);
            rateS.Add("24Mbit", 4);
            rateS.Add("36Mbit", 5);
            rateS.Add("48Mbit", 6);
            rateS.Add("54Mbit", 7);

            rateS.Add("MCS0", 0);
            rateS.Add("MCS1", 1);
            rateS.Add("MCS2", 2);
            rateS.Add("MCS3", 3);
            rateS.Add("MCS4", 4);
            rateS.Add("MCS5", 5);
            rateS.Add("MCS6", 6);
            rateS.Add("MCS7", 7);
            rateS.Add("MCS8", 8);
            rateS.Add("MCS9", 9);
            rateS.Add("MCS10", 10);
            rateS.Add("MCS11", 11);
        }

        private void createRateM()
        {
            rateM = new Dictionary<string, uint>();
            rateM.Add("MCS0", 8);
            rateM.Add("MCS1", 9);
            rateM.Add("MCS2", 10);
            rateM.Add("MCS3", 11);
            rateM.Add("MCS4", 12);
            rateM.Add("MCS5", 13);
            rateM.Add("MCS6", 14);
            rateM.Add("MCS7", 15);
        }

        private void getConfigInfo()
        {
            string path = "MainTestQueue\\WiFi\\config\\PreambleType.xml";
            XmlDocument senDealTypeXml = new XmlDocument();
            senDealTypeXml.Load(path);            
            foreach (XmlElement element in senDealTypeXml.DocumentElement.GetElementsByTagName("preamble"))
            {
                foreach (XmlElement item in element.GetElementsByTagName("item"))
                {
                    if (item.InnerText.Equals("1"))
                    {
                        selectPreamble.Add(element.GetAttribute("name"),item.GetAttribute("name"));
                        break;
                    }
                }
            }
        }
    }
}
