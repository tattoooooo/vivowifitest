using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QC.QMSLPhone;
using System.Windows;
using System.Threading;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Management;
using Samsungwlan = vivoautotestwifi.Drives.NoSignaling.SAMSUNG.SAMSUNG_WLAN;
using vivoautotestwifi.MainQueue.ICType.MTK;
using vivoautotestwifi.MainQueue.waveforms;

namespace vivoautotestwifi.MainQueue
{
    class WiFi_Function
    {
        private string Testic = null;

        public string TestIc
        {
            get { return Testic; }
            set { Testic = value; }
        }

        #region 调用IQXEL仪器的dll
        [DllImport("lib\\IQxel\\IQmeasure.dll", EntryPoint = "LP_Init", CallingConvention = CallingConvention.Cdecl)]
        private extern static void LP_Init(int IQType, int TestMode);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_InitTester", CallingConvention = CallingConvention.Cdecl)]
        private extern static int IQXEL_LP_InitTester(IntPtr ipaddress, int instrumenttype);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_ScpiCommandSet", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_ScpiCommandSet(IntPtr ipaddress);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_ScpiCommandQuery", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_ScpiCommandQuery(char[] ipaddress, int maxlength, [Out, MarshalAs(UnmanagedType.LPArray)] char[] re, out IntPtr maxreturnlength);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_EnableVsgRF", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_EnableVsgRF(int enabled);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_EnableVsgRF", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_EnableVsgRFNxN(int enabled1, int enabled2, int enabled3, int enabled4);
      
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_SetVsg", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_SetVsg(double rfFreqHz, double rfPowerLeveldBm, int port, bool setGapPowerOff, double dFreqShiftHz);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_SetVsgModulation_SetPlayCondition", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_SetVsgModulation_SetPlayCondition(string modFileName, bool autoPlay, int loadInternalWaveform);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_SetVsgModulation", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_SetVsgModulation(string modFileName, int loadInternalWaveform);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_SetFrameCnt", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_SetFrameCnt(int FrameCount);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_Analyze80211n", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_Analyze80211n(string type, string mode, int enablePhaseCorr, int enableSymTimingCorr, int enableAmplitedeTracking, int decodePSDU, int enableFullPacketChannelEst, string referenceFile, int packetFormat, int FrequencyCorr, double prePowStartSec, double prePowStopSec);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_Analyze80211ac", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_Analyze80211ac(string mode, int enablePhaseCorr, int enableSymTimingCorr, int enableAmplitedeTracking, int decodePSDU, int enableFullPacketChannelEst, int FrequencyCorr, string referenceFile, int packetFormat, int numberOfPacketToAnalysis, double prePowStartSec, double prePowStopSec);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_GetScalarMeasurement", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_GetScalarMeasurement(string measurement, int index);
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_TxDone", CallingConvention = CallingConvention.Cdecl)]
        private extern static int LP_TxDone();
        [DllImport("lib\\IQxel\\IQmeasure.dll", CharSet = CharSet.Ansi, EntryPoint = "LP_ConOpen", CallingConvention = CallingConvention.Cdecl)]
        private extern static void LP_ConOpen();
        #endregion

        Phone qmslphone = new Phone();//调用高通的托管程序集

        private Int16 Phoneport = 0;

        #region 已经写好的代码（高通的控制测试方法，与Iqxel仪器的控制方法），无需再改


        public void setLossByIQName(string IQxel)
        {
            if (IQxel.Equals("MW"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;RFC:USE \"1\",RF1A"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RFC:USE \"1\",RF1A"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG12;RFC:USE \"1\",RF1B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA12;RFC:USE \"1\",RF1B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEMory:TABLe \"1\";MEMory:TABLe:STORe"));
            }
        }

        /// <summary>
        /// 设置线损
        /// </summary>
        /// <param name="Freq">设置线损的频率MHz</param>
        /// <param name="Loss">线损值</param>
        /// <param name="waittime">延时</param>
        /// <param name="testtype">true为MIMO测试（使用IQxel160）的线损，false为非MIMO测试（使用IQxel80）的线损</param>
        public void SetIQxelLoss(string Freq, string Loss, string waittime, bool testtype)
        {
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEM:TABLE \"1\"; MEM:TABLE:DEFINE \"FREQ,LOSS\""));
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEM:TABLE \"1\";MEMory:TABLe:INSert:POINt" + " " + Freq + " " + "MHz," + Loss));

            if (testtype == true)
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG2;RFC:USE \"1\",RF1B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG2;RFC:USE \"1\",RF2B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA2;RFC:USE \"1\",RF1B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA2;RFC:USE \"1\",RF2B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEMory:TABLe \"1\";MEMory:TABLe:STORe"));
                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF1B,VSA12"));
                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF2,VSA11"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG:DEF:ADD VSG1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG:DEF:ADD VSG12"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA:DEF:ADD VSA1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA:DEF:ADD VSA12"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT:DEF:ADD ROUT1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT:DEF:ADD ROUT12"));
            }
            else
            {
                //非MIMO测试
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;RFC:STAT  ON,RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RFC:STAT  ON,RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RFC:STAT  ON,RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;RFC:STAT  ON,RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME" + " " + waittime));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME 0.01"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF1,VSA1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEMory:TABLe \"1\";MEMory:TABLe:STORe"));
            }
        }

        public void SetLoss(bool testtype = false)
        {
            if (testtype == true)
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG2;RFC:USE \"1\",RF1B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG2;RFC:USE \"1\",RF2B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA2;RFC:USE \"1\",RF1B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA2;RFC:USE \"1\",RF2B"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEMory:TABLe \"1\";MEMory:TABLe:STORe"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG:DEF:ADD VSG1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG:DEF:ADD VSG12"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA:DEF:ADD VSA1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA:DEF:ADD VSA12"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT:DEF:ADD ROUT1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT:DEF:ADD ROUT12"));
            }
            else
            {
                //非MIMO测试
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;RFC:STAT  ON,RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RFC:STAT  ON,RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RFC:STAT  ON,RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;RFC:STAT  ON,RF2"));
                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME" + " " + waittime));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME 0.01"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF1,VSA1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF2"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEMory:TABLe \"1\";MEMory:TABLe:STORe"));
            }
        }


        /// <summary>
        /// 点击 RX Paly 按钮
        /// author 何苹
        /// </summary>
        /// <param name="waveformName">波形文件</param>
        public void ClickRXPaly(string waveformName)
        {
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL ;wave:exec off;WLIST:WSEG1:DATA \"" + waveformName + "\";wlist:wseg1:save;WLIST:COUNT:ENABLE WSEG1;WAVE:EXEC ON, WSEG1"));

            //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;"));
            //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("wave:exec off;"));
            //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("WLIST:WSEG1:DATA " + waveformName + ";"));
            //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("wlist:wseg1:save;"));
            //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("WLIST:COUNT:ENABLE WSEG1;"));
            //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("WAVE:EXEC ON,WSEG1"));
        }

        string enppower = null;

        /// <summary>
        /// 设置IQxel仪器配置
        /// </summary>
        /// <param name="BAND">频段：2G4、5G</param>
        /// <param name="channel">信道</param>
        /// <param name="STAN">DSSS、OFDM</param>
        /// <param name="ENP">期望功率</param>
        /// <param name="channelfre">信道频率误差（MHZ）</param>
        public void SetIQxelConfig(string BAND, string channel, string STAN, string ENP, string channelfre, string Argument,string IQxel,string chain)
        {
            Log.GetInstance().d("数据", "band:"+ BAND + " channel:"+ channel + " STAN:"+ STAN + " ENP:"+ ENP + " channelfre:"+ channelfre + " argument:"+ Argument);
            enppower = ENP;
            Channel = Convert.ToInt32(channel);

            if (chain.Equals("3"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL;SRAT 160000000"));

                if (IQxel.Equals("MW"))
                {
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:UAS ON"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:MIMO EVMSignal"));
                }
            }
            else
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;SRAT 160000000"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;SRAT 160000000"));//add 
            }

            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:chan:BAND" + " " + BAND));
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:CHAN:IND1" + " " + channel));
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:STAN" + " " + STAN));
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:ENP" + " " + ENP));
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;FREQ:cent" + " " + channelfre));

            if (chain.Equals("3"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL MVSAALL"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL;CAPT:TIME 0.02"));
            }
            else
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                Thread.Sleep(2000);
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1 ;RLEVel:AUTO"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME 0.02"));
            }


            if (Argument.Contains("AC")||Argument.Contains("AX"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:CEST DATA"));
            }
            else
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:CEST LTF"));
            }

            if (Argument.Contains("AX40M") || Argument.Contains("AX20M")|| Argument.Contains("AX80M"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:SPEC:HLIM:TYPE 11AX"));                
            }
            else
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:SPEC:HLIM:TYPE AUTO"));
            }



            //if (Argument.Contains("2G4_B") == false)
            //{
            //    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME 0.01"));
            //}


        }

        public void SetTBPram(string Argument,string mcsNum)
        {
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:MCS:user "+mcsNum));
            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:COD:user LDPC"));
            if (Argument.Contains("AX20M"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:RUIN:user 61"));
            }
            else if (Argument.Contains("AX40M"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:RUIN:user 65"));
            }
            else if (Argument.Contains("AX80M"))
            {
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:OFDM:RUIN:user 67"));
            }
        }

        /// <summary>
        /// 3660和3680手机配置
        /// </summary>
        /// <param name="channel">信道</param>
        /// <param name="bandwidth">带宽</param>
        /// <param name="ratebit">速率</param>
        /// <param name="agreement">协议</param>
        /// <param name="enp">期望功率</param>
        public void SetQRCTConfig(uint channel, string bandwidth, uint ratebit, string agreement, double enp)
        {
            try
            {
                //TX测试是“6”，RX测试是“4”
                qmslphone.FTM_WLAN_GEN6_ENABLE_CHAINS(QC.QMSLPhone.Phone.WLAN_Gen6_PHY_ChainSelect.PHY_CHAIN_SEL_T0_ON);
                //设置信道带宽
                QC.QMSLPhone.Phone.WLAN_Gen6_ChannelBondingState BandWidthType = new Phone.WLAN_Gen6_ChannelBondingState();
                //qmslphone.FTM_WLAN_GEN6_ENABLE_DPD(0);
                switch (bandwidth)
                {
                    case "20M":
                        BandWidthType = QC.QMSLPhone.Phone.WLAN_Gen6_ChannelBondingState.none;
                        break;
                    case "40M":
                        BandWidthType = QC.QMSLPhone.Phone.WLAN_Gen6_ChannelBondingState.primaryHigh;
                        channel = channel + 2;
                        break;
                    case "80M":
                        BandWidthType = QC.QMSLPhone.Phone.WLAN_Gen6_ChannelBondingState.BW80_20_40GHigh_40_80High;
                        channel = channel + 6;
                        break;
                }
                qmslphone.FTM_WLAN_GEN6_SET_CHANNEL_V2(channel, BandWidthType);
                uint Agreement = 0;
                switch (agreement)
                {
                    case "11b":
                        Agreement = 0;//QC.QMSLPhone.Phone.WLAN_Gen6_PHYDBG_PreambleRate.PREAMBLE_LONGB_11B;
                        break;
                    case "11a/g":
                        Agreement = 1;//QC.QMSLPhone.Phone.WLAN_Gen6_PHYDBG_PreambleRate.PREAMBLE_OFDM_11G;
                        break;
                    case "11n":
                        Agreement = 2;//QC.QMSLPhone.Phone.WLAN_Gen6_PHYDBG_PreambleRate.PREAMBLE_MIXED_11N;
                        break;
                    case "11ac":
                        Agreement = 3;//QC.QMSLPhone.Phone.WLAN_Gen6_PHYDBG_PreambleRate.PREAMBLE_MIXED_11N;
                        break;
                }
                qmslphone.FTM_WLAN_GEN6_SET_TX_FRAME_V2(0, 756, 2, 165, 200, 1, ratebit, Agreement);
                qmslphone.FTM_WLAN_GEN6_SET_CLOSED_LOOP_POWER(enp);
                qmslphone.FTM_WLAN_GEN6_CLOSE_TPC_LOOP_V2(2);//1:SCPC，2:CLPC;  Changed 2 to 1 at 2019.08.26 by wzt
                qmslphone.FTM_WLAN_GEN6_SET_PWR_INDEX_SOURCE(2);
                qmslphone.FTM_WLAN_GEN6_TX_PKT_START_STOP(true);
            }
            catch (PhoneException pex)
            {
                MessageBox.Show("错误", pex.ToString());
                return;
            }
        }

        public void stop6391or6750(string phyId)
        {
            try
            {
                qmslphone.FTM_WLAN_TLV2_Create(10);
                qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));
                qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopTx"), System.Text.Encoding.Default.GetBytes("1"));
                qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("needReport"), System.Text.Encoding.Default.GetBytes("1"));
                qmslphone.FTM_WLAN_TLV2_Complete();
            }
            catch(PhoneException pe)
            {
                Log.GetInstance().d("6391", "stop6391失败");

            }
        }

        /// <summary>
        /// qc6174 3990 设置测试参数
        /// </summary>
        /// <param name="type">6174、3990、6391</param>
        /// <param name="channel">信道 使用频率表示，单位MHz</param>
        /// <param name="tpcm">是否使用期望功率 “1”表示使用，“2表示不使用”</param>
        /// <param name="rateBitIndex">速率</param>
        /// <param name="wlanmode">协议，包含带宽</param>
        /// <param name="chain">天线 “1”表示使用天线1，“2”表示使用天线2，“3”表示MIMO</param>
        /// <param name="txpower">使用期望功率时，设定的期望功率值</param>
        public void SetQRCTConfig(string type, string channel, string tpcm, string rateBitIndex, string wlanmode, string chain, string txpower, bool RXorTX,string phyId="1",string rateBW="")
        {
            string DPDValue = null;
            Log.GetInstance().d("手机参数数据", "type："+ type+ "\nchannel："+ channel + "\nrateBitIndex：" + rateBitIndex + "\nwlanmode：" + wlanmode + "\nchain：" + chain + "\ntxpower：" + txpower + "\n6391phyId：" + phyId + "\nrateBW：" + rateBW);
            if (type == "6174")
            {
                if (RXorTX == false)
                {
                    qmslphone.FTM_WLAN_TLV_Create(1);
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("txMode"), System.Text.Encoding.Default.GetBytes("3"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("tpcm"), System.Text.Encoding.Default.GetBytes(tpcm));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("rateBitIndex0"), System.Text.Encoding.Default.GetBytes(rateBitIndex));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//rx tx
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("txChain0"), System.Text.Encoding.Default.GetBytes(chain));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("pktLen0"), System.Text.Encoding.Default.GetBytes("1500"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("shortGuard"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("numPackets"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("txPattern"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("scgg"), System.Text.Encoding.Default.GetBytes("1"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("ramblerOff"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("aaifsn"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("broadcast"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("24"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("txPower0"), System.Text.Encoding.Default.GetBytes(txpower));
                    qmslphone.FTM_WLAN_TLV2_Complete();
                }
                else
                {
                    qmslphone.FTM_WLAN_TLV_Create(2);
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//rx tx
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("rxMode"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("rateMask"), System.Text.Encoding.Default.GetBytes(rateBitIndex));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("rxChain"), System.Text.Encoding.Default.GetBytes(chain));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("enANI"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("0"));
                    qmslphone.FTM_WLAN_TLV2_Complete();
                }
                qmslphone.FTM_WLAN_TLV_Complete();
            }
            else if (type == "3990")
            {
                if (RXorTX == false)
                {
                A: try
                    {
                        qmslphone.FTM_WLAN_TLV2_Create(1);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopTx"), System.Text.Encoding.Default.GetBytes("1"));//add                         
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txMode"), System.Text.Encoding.Default.GetBytes("3"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx                        
                        if (enppower.Equals("0"))
                        {
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("tpcm"), System.Text.Encoding.Default.GetBytes(tpcm));
                        }
                        else
                        {
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("tpcm"), System.Text.Encoding.Default.GetBytes("0"));
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txPower0"), System.Text.Encoding.Default.GetBytes(txpower));
                        }
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBitIndex0"), System.Text.Encoding.Default.GetBytes(rateBitIndex));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txChain0"), System.Text.Encoding.Default.GetBytes(chain));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("pktLen0"), System.Text.Encoding.Default.GetBytes("1500"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("shortGuard"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("numPackets"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txPattern"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("scramblerOff"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("agg"), System.Text.Encoding.Default.GetBytes("1"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("aifsn"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("broadcast"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("28"));
                        qmslphone.FTM_WLAN_TLV2_Complete();
                        Thread.Sleep(50);

                    }
                    catch (PhoneException pex)
                    {
                        Log.GetInstance().d("3990", "参数设置失败，重新设置");
                        qmslphone.DisconnectToServer();
                        ConnectPhone(Phoneport, "3990");
                        PhoneIni("3990");
                        goto A;
                    }
                }
                else
                {

                B: try
                    {
                        //qmslphone.FTM_WLAN_TLV2_Create(169);
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyRFMode"), System.Text.Encoding.Default.GetBytes("0"));
                        //qmslphone.FTM_WLAN_TLV2_Complete();
                        //qmslphone.FTM_WLAN_TLV2_Create(117);
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//rx tx
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxMode"), System.Text.Encoding.Default.GetBytes("0"));
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateMask"), System.Text.Encoding.Default.GetBytes(rateBitIndex));
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxChain"), System.Text.Encoding.Default.GetBytes(chain.ToString()));
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("enANI"), System.Text.Encoding.Default.GetBytes("0"));
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("0"));
                        //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("48"));
                        //qmslphone.FTM_WLAN_TLV2_Complete();
                        qmslphone.FTM_WLAN_TLV2_Create(2);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopTx"), System.Text.Encoding.Default.GetBytes("1"));//add  
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxMode"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateMask"), System.Text.Encoding.Default.GetBytes(rateBitIndex));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxChain"), System.Text.Encoding.Default.GetBytes(chain));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("enANI"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("48"));
                        qmslphone.FTM_WLAN_TLV2_Complete();
                    }
                    catch
                    {
                        Log.GetInstance().d("3990", "参数设置失败，重新设置");
                        qmslphone.DisconnectToServer();
                        ConnectPhone(Phoneport, "3990");
                        PhoneIni("3990");
                        goto B;
                    }
                }
            }
            else  if (type == "6391")
            {
                if (RXorTX == false)
                {
                    //通过配置文件获取DPDmode配置，1：开DPD 0：关DPD
                    XmlDocument DPDInfo = new XmlDocument();
                    DPDInfo.Load(@"MainTestQueue\\WiFi\\config\\6391DPDSetting.xml");
                    foreach (XmlElement DPDSetting in DPDInfo.DocumentElement.GetElementsByTagName("item"))
                    {
                        if ("DPDSetting" == DPDSetting.GetAttribute("name"))
                        {
                            DPDValue = DPDSetting.InnerText;
                        }
                    }
                A: try
                    {
                        bool f = qmslphone.IsPhoneConnected();

                        qmslphone.FTM_WLAN_TLV2_Create(169);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyRFMode"), System.Text.Encoding.Default.GetBytes("1"));
                        qmslphone.FTM_WLAN_TLV2_Complete();
                        Thread.Sleep(50);

                        qmslphone.FTM_WLAN_TLV2_Create(114);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));//变                         
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txMode"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                        qmslphone.FTM_WLAN_TLV2_Complete();
                        Thread.Sleep(50);

                        qmslphone.FTM_WLAN_TLV2_Create(114);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wifiStandard"), System.Text.Encoding.Default.GetBytes("2"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));//变                         
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txMode"), System.Text.Encoding.Default.GetBytes("3"));//
                        if (!txpower.Equals("0"))
                        {
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("tpcm"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txPower0"), System.Text.Encoding.Default.GetBytes(txpower));//
                        }
                        else
                        {
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("tpcm"), System.Text.Encoding.Default.GetBytes(tpcm));//
                        }
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//                        
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("gainIdx"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("dacGain"), System.Text.Encoding.Default.GetBytes("0"));
                        //MAC 3
                        if (phyId.Equals("0"))
                        {
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("txStation"), 0XD0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("rxStation"), 0XE0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("bssid"), 0XF0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                        }
                        else
                        {
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("txStation"), 0XB0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("rxStation"), 0XA0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("bssid"), 0XC0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                        }

                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBitIndex0"), System.Text.Encoding.Default.GetBytes(rateBitIndex));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBw"), System.Text.Encoding.Default.GetBytes(rateBW));//变
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("nss"), System.Text.Encoding.Default.GetBytes(chain.Equals("3") ? "2" : "1"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("enANI"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("scramblerOff"), System.Text.Encoding.Default.GetBytes("1"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("aifsn"), System.Text.Encoding.Default.GetBytes("0"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("gl"), System.Text.Encoding.Default.GetBytes("2"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("agg"), System.Text.Encoding.Default.GetBytes("0"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("dutyCycle"), System.Text.Encoding.Default.GetBytes("10"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("pktLen0"), System.Text.Encoding.Default.GetBytes("1500"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("0"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txChain0"), System.Text.Encoding.Default.GetBytes(chain));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("broadcast"), System.Text.Encoding.Default.GetBytes("1"));//                     
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("shortGuard"), System.Text.Encoding.Default.GetBytes("0"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("numPackets"), System.Text.Encoding.Default.GetBytes("0"));//
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txPattern"), System.Text.Encoding.Default.GetBytes("0"));//
                        if(DPDValue=="1")//获取配置文件对DPDmode的设置，1：flags=28表示勾选DPDmode 0：flags=24，表示不勾选DPDmode add by lxy 2021.6.30
                        {
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("28"));// 此处开发需要修改为28，使用DPDmode，lxy2021.6.29
                        }
                        else
                        {
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("24"));
                        }      
                        qmslphone.FTM_WLAN_TLV2_Complete();
                        Log.GetInstance().d("6391", "参数设置完成");
                    }
                    catch (PhoneException pex)
                    {
                        Log.GetInstance().d("6391", "参数设置失败，重新设置");
                        qmslphone.DisconnectToServer();
                        ConnectPhone(Phoneport, "6391");
                        PhoneIni("6391");
                        goto A;
                    }
                }
                else
                {

                B: try
                    {
                        qmslphone.FTM_WLAN_TLV2_Create(169);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyRFMode"), System.Text.Encoding.Default.GetBytes("1"));//add               
                        qmslphone.FTM_WLAN_TLV2_Complete();

                        qmslphone.FTM_WLAN_TLV2_Create(117);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxMode"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBitIndex0"), System.Text.Encoding.Default.GetBytes(rateBitIndex));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBw"), System.Text.Encoding.Default.GetBytes(rateBW));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("nss"), System.Text.Encoding.Default.GetBytes(chain.Equals("3") ? "2" : "1"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wifiStandard"), System.Text.Encoding.Default.GetBytes("2"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("1"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("enANI"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxChain"), System.Text.Encoding.Default.GetBytes(chain));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("48"));
                        qmslphone.FTM_WLAN_TLV2_Complete();
                    }
                    catch(Exception ex)
                    {
                        Log.GetInstance().d("6391"+ex.ToString(), "参数设置失败，重新设置");
                        qmslphone.DisconnectToServer();
                        ConnectPhone(Phoneport, "6391");
                        PhoneIni("6391");
                        goto B;
                    }

                }
            }
            else if (type == "6750") //by wzt at 2021.04.20
            {
                if (RXorTX == false)
                {
                A: try
                    {
                        //bool f = qmslphone.IsPhoneConnected();


                        for (int loop = 0; loop < 2; loop++) //6750发射需要先开关一次然后再开一次才能正常发射 by wzt at 2021.04.20
                        {
                            qmslphone.FTM_WLAN_TLV2_Create(169);
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyRFMode"), System.Text.Encoding.Default.GetBytes("0"));
                            qmslphone.FTM_WLAN_TLV2_Complete();
                            Thread.Sleep(50);

                            qmslphone.FTM_WLAN_TLV2_Create(114);
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes("0"));//变   //6750不支持DSB，所以PHY_ID=0,RFmode=0 by wzt at 2021.04.20               
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txMode"), System.Text.Encoding.Default.GetBytes("0"));
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                            qmslphone.FTM_WLAN_TLV2_Complete();
                            Thread.Sleep(50);

                            qmslphone.FTM_WLAN_TLV2_Create(114);
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wifiStandard"), System.Text.Encoding.Default.GetBytes("2"));
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));//变                         
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txMode"), System.Text.Encoding.Default.GetBytes("3"));//
                            if (!txpower.Equals("0"))
                            {
                                qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("tpcm"), System.Text.Encoding.Default.GetBytes("0"));//
                                qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txPower0"), System.Text.Encoding.Default.GetBytes(txpower));//
                            }
                            else
                            {
                                qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("tpcm"), System.Text.Encoding.Default.GetBytes(tpcm));//
                            }
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//                        
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("gainIdx"), System.Text.Encoding.Default.GetBytes("0"));
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("dacGain"), System.Text.Encoding.Default.GetBytes("0"));
                            //MAC 3

                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("txStation"), 0XE0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("rxStation"), 0XD0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);
                            qmslphone.FTM_WLAN_TLV2_AddMAC(System.Text.Encoding.Default.GetBytes("bssid"), 0XF0, 0XC0, 0XC0, 0XC0, 0X00, 0X00);



                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBitIndex0"), System.Text.Encoding.Default.GetBytes(rateBitIndex));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBw"), System.Text.Encoding.Default.GetBytes(rateBW));//变
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("nss"), System.Text.Encoding.Default.GetBytes(chain.Equals("3") ? "2" : "1"));
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("enANI"), System.Text.Encoding.Default.GetBytes("0"));
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("scramblerOff"), System.Text.Encoding.Default.GetBytes("1"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("aifsn"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("gl"), System.Text.Encoding.Default.GetBytes("2"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("agg"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("dutyCycle"), System.Text.Encoding.Default.GetBytes("50")); //6750是50 6391是10
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("pktLen0"), System.Text.Encoding.Default.GetBytes("1500"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txChain0"), System.Text.Encoding.Default.GetBytes(chain));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("broadcast"), System.Text.Encoding.Default.GetBytes("1"));//                     
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("shortGuard"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("numPackets"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("txPattern"), System.Text.Encoding.Default.GetBytes("0"));//
                            qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("28")); //6750是28 6391是24
                            qmslphone.FTM_WLAN_TLV2_Complete();
                            Log.GetInstance().d("6750", "参数设置完成");
                            if (loop == 0)
                            {
                                stop6391or6750("0");
                            }
                        }
                    }
                    catch (PhoneException pex)
                    {
                        Log.GetInstance().d("6750", "参数设置失败，重新设置");
                        qmslphone.DisconnectToServer();
                        ConnectPhone(Phoneport, "6750");
                        PhoneIni("6750");
                        goto A;
                    }
                }
                else
                {

                B: try
                    {
                        qmslphone.FTM_WLAN_TLV2_Create(169);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyRFMode"), System.Text.Encoding.Default.GetBytes("0"));//add               
                        qmslphone.FTM_WLAN_TLV2_Complete();

                        qmslphone.FTM_WLAN_TLV2_Create(117);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("channel"), System.Text.Encoding.Default.GetBytes(channel));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxMode"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBitIndex0"), System.Text.Encoding.Default.GetBytes(rateBitIndex));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rateBw"), System.Text.Encoding.Default.GetBytes(rateBW));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("nss"), System.Text.Encoding.Default.GetBytes(chain.Equals("3") ? "2" : "1"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("bandwidth"), System.Text.Encoding.Default.GetBytes("0"));//rx tx
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wifiStandard"), System.Text.Encoding.Default.GetBytes("2"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("wlanMode"), System.Text.Encoding.Default.GetBytes(wlanmode));//rx tx//gai
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("antenna"), System.Text.Encoding.Default.GetBytes("1"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("enANI"), System.Text.Encoding.Default.GetBytes("0"));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("rxChain"), System.Text.Encoding.Default.GetBytes(chain));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("flags"), System.Text.Encoding.Default.GetBytes("48"));
                        qmslphone.FTM_WLAN_TLV2_Complete();
                    }
                    catch
                    {
                        Log.GetInstance().d("6750", "参数设置失败，重新设置");
                        qmslphone.DisconnectToServer();
                        ConnectPhone(Phoneport, "6750");
                        PhoneIni("6750");
                        goto B;
                    }
                }
            }
        }
        
        /// <summary>
        /// iqxel仪器开始测试方法
        /// </summary>
        /// <param name="testtype">true为MIMO测试，false为非MIMO测试</param>
        /// <param name="iQxelDataList">手机参数设置数据</param>
        /// <param name="phoneDataList">仪器参数设置数据</param>
        public void IqxelTestSig(bool testtype,string RateBits,string IQxel,string Agreement,List<string> phoneDataList, List<string> iQxelDataList)
        {
            int count = 0;
            if (testtype == false) {
                //add 开启RF 6391
                if (IQxel.Equals("MW"))
                {
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("ROUT1;PORT:RES RF1A,VSA1"));
                }
                else
                {
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("ROUT1; PORT: RES RF1, VSA1"));
                }
            }//改 author 何苹 Samsung WiFi MIMO                              
            else { int temp = LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL MVSAALL")); }//加 author 何苹 Samsung WiFi MIMO
            for (int textindex = 0; textindex < 6; textindex++)
            {
                if (testtype == true)
                {
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL;CAPT:TIME 0.01"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL;RLEVel:AUTO"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1"));
                    if (IQxel.Equals("MW"))
                    {
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL ;MVSGALL:INST:COUN 1;MVSAALL:INST:COUN 1;init"));
                    }
                    else
                    {
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL ;init"));
                    }
                    
                }
                else
                {
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RLEVel:AUTO"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1; WIFI; HSET: ALL VSA1"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;init"));
                    //if (RateBits.Contains("48") || RateBits.Contains("54"))
                    //{
                    //    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1; RLEV" + " " + enppower));
                    //}
                    //else
                    //{
                    //}
                }
																							  
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("WIFI"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:pow 0, 1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:txq 0, 1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:ccdf 0, 1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:ramp 0, 1"));
                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:spec 0, 1"));

                Thread.Sleep(500);

                //检查输出值是否满足要求 若不满足将执行textindex-1
                if (textindex == 5)
                {
                    IntPtr relenght = new IntPtr();
                    char[] re = new char[512];
                    string buff = null;
                    double evm = 0;
                    double power = 0;

                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1"));
                    switch (IQxel)
                    {
                        case "80":
                        case "MW":
                            if (testtype)
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL ;MVSGALL:INST:COUN 1;MVSAALL:INST:COUN 1;init"));
                            else
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;init"));
                            break;
                        case "160":
                            if(testtype)
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSAALL ;init"));
                            else
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1 ;init"));
                            break;                        
                    }                    
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("WIFI"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:pow 0, 1"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:txq 0, 1"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:ccdf 0, 1"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:ramp 0, 1"));
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("calc:spec 0, 1"));
                    Thread.Sleep(1000);

                    if (Agreement.Contains("2G4_B"))
                    {
                        LP_ScpiCommandQuery("FETCH:TXQUALITY:DSSS:AVER?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);//buff.Split(',')[1]=EVM  buff.Split(',')[4]=中心频率误差  buff.Split(',')[3]=码片时钟频率误差
                    }
                    else
                    {
                        LP_ScpiCommandQuery("FETC:TXQ:OFDM?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                    }
                    try
                    {
                        evm = Convert.ToDouble(buff.Split(',')[1]);
                    }
                    catch(Exception ex)
                    {
                        evm = 99999999;
                    }

                    LP_ScpiCommandQuery("FETCH:POWER:AVER?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);
                    try
                    {
                        power = Convert.ToDouble(buff.Split(',')[1]);
                    }
                    catch (Exception ex)
                    {
                        power = 99999999;
                    }

                    Log.GetInstance().d("数据", "power、evm值,power:" + power + " evm:" + evm);
                    if ((evm > 100|| power<5) &&count <10)
                    {
                        Log.GetInstance().d("数据异常","power、evm值异常"+ count+" 次,power:"+power+" evm:"+evm);
                        if (IQxel.Equals("MW"))
                        {
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RLEVel:AUTO"));
                        }
                        //MW处理
                        if(iQxelDataList.Count != 0 && phoneDataList.Count != 0)
                        {
                            switch (phoneDataList[0])
                            {
                                case "6391":
                                    if (count >= 5)
                                    {
                                        Log.GetInstance().d(phoneDataList[0], "power、evm值异常，重连手机并设置仪器参数 power: " + power + " evm:" + evm);
                                        ConnectPhone(Phoneport, phoneDataList[0]);
                                        PhoneIni(phoneDataList[0]);
                                        SetIQxelConfig(iQxelDataList[0], iQxelDataList[1], iQxelDataList[2], iQxelDataList[3], iQxelDataList[4], iQxelDataList[5], iQxelDataList[6], iQxelDataList[7]);
                                    }
                                    SetQRCTConfig(phoneDataList[0], phoneDataList[1], "5", phoneDataList[2], phoneDataList[3], phoneDataList[4], phoneDataList[5], false, phoneDataList[6], phoneDataList[7]);
                                    Thread.Sleep(200);
                                    break;
                            }
                        }
                        textindex = textindex - 1;
                        count++;
                    }
                }
            }
            //Thread.Sleep(2000);
        }

        /// <summary>
        /// 获取发射测试结果
        /// </summary>
        /// <param name="IQxel">设备，true：Iqxel160，false：Iqxel80  改  name：IQxel 仪器参数80,160，MW</param>
        /// <param name="ArgumentType">协议字符串</param>
        /// <param name="RateBits">速率字符串</param>
        /// <returns>返回测试结果</returns>
        public Dictionary<string, string> GetIQxelValue(string IQxel, string ArgumentType, string RateBits,string CriteriasFilePath,string chain="1",List<string> phoneDataList=null, List<string> iQxelDataList = null )
        {            
            Dictionary<string, string> ResultDic = new Dictionary<string, string>();
            char[] re = new char[512];
            List<string> criterias = new List<string>(), sfllist = new List<string>();
            List<double> PowerList = new List<double>(), EvmList = new List<double>(), FreqErrList = new List<double>(),
                ClockErr = new List<double>(), LolList = new List<double>(), RampOnList = new List<double>(), RampOffList = new List<double>();
            List<double>[] SPFarray = new List<double>[8], SFLarray = new List<double>[4];
            List<double>[] SPFarraySIGN2 = new List<double>[8];
            bool sflchecked = false;//是否测试SFL(频谱发射模板)
            bool rampchecked = false;//是否测试Ramp（上升沿、下降沿）
            IntPtr relenght = new IntPtr();
            string buff = null;

            #region//获取测试标准
            XmlDocument doc = new XmlDocument();
            //if (instrument == true)      改
            //{
            //    //改
            //    doc.Load(CriteriasFilePath);
            //}
            //else
            //{
            //    doc.Load(CriteriasFilePath);
            //}
            doc.Load(CriteriasFilePath);
            XmlElement Criterias = doc.DocumentElement;
            XmlNodeList Agreement = Criterias.GetElementsByTagName("Agreement");
            foreach (XmlNode agreement in Agreement)
            {
                if (((XmlElement)agreement).GetAttribute("name").ToString() == ArgumentType)
                {
                    XmlNodeList ratebitS = ((XmlElement)agreement).GetElementsByTagName("ratebit");
                    foreach (XmlNode ratebit in ratebitS)
                    {
                        if (((XmlElement)ratebit).GetAttribute("name").ToString() == RateBits)
                        {
                            XmlNodeList limits = ((XmlElement)ratebit).GetElementsByTagName("limits");
                            foreach (XmlElement limit in limits)
                            {
                                criterias.Add(((XmlElement)limit).InnerText);
                            }
                        }
                    }
                }
            }
            #endregion
            for (int i = 0; i < 8; i++)
            {
                if (SPFarray[i] == null&&SPFarraySIGN2[i]==null)//加
                {
                    SPFarray[i] = new List<double>();
                    SPFarraySIGN2[i] = new List<double>();//加
                    if (i < 4)
                    {
                        SFLarray[i] = new List<double>();
                        SPFarraySIGN2[i] = new List<double>();//加
                    }
                }
            }
            //List<string> sfllimits = new List<string>();
            //for (int TestCount = 0; TestCount < 1; TestCount++)
            {
                int temp=LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL MVSAALL"));//加 author 何苹 Samsung WiFi MIMO
                IqxelTestSig(chain.Equals("3"), RateBits,IQxel, ArgumentType, phoneDataList, iQxelDataList);

                if (ArgumentType == "2G4_B")
                {
                    #region B协议
                    LP_ScpiCommandQuery("FETCH:POWER:AVER?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);
                    PowerList.Add(Convert.ToDouble(buff.Split(',')[1]));
                    //EVM、FREQ、CLOCKERR、LOL指标测试
                    LP_ScpiCommandQuery("FETCH:TXQUALITY:DSSS:AVER?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);//buff.Split(',')[1]=EVM  buff.Split(',')[4]=中心频率误差  buff.Split(',')[3]=码片时钟频率误差
                    EvmList.Add(Convert.ToDouble(buff.Split(',')[1]));
                    FreqErrList.Add(Convert.ToDouble(buff.Split(',')[4]) / 1000);
                    ClockErr.Add(Convert.ToDouble(buff.Split(',')[3]));
                    LolList.Add(Convert.ToDouble(buff.Split(',')[7]));
                    //发射频谱
                    LP_ScpiCommandQuery("FETC:SPEC:SIGN1:MARG?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);
                    //LP_ScpiCommandQuery("FETC:SPEC:SIGN1:MARG:OFR?".ToCharArray(), 128, re, out relenght);
                    //string freq = new string(re);
                    for (int i = 1; i <= 4; i++)
                    {
                        try
                        {
                            SPFarray[i - 1].Add(Convert.ToDouble(buff.Split(',')[i]));
                        }
                        catch
                        {
                            SPFarray[i - 1].Add(999999);
                        }

                    }
                    //RAMP ON指标测试
                    rampchecked = true;
                    LP_ScpiCommandQuery("FETC:RAMP:ON:TRIS?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);
                    if (Convert.ToDouble(buff.Split(',')[1]) < 1000)
                    {
                        RampOnList.Add(Convert.ToDouble(buff.Split(',')[1]) * 1000000);
                    }
                    if (RampOnList.Count == 0)
                    {
                        RampOnList.Add(999);
                    }
                    //RAMP OFF指标测试
                    LP_ScpiCommandQuery("FETC:RAMP:OFF:TRIS?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);
                    if (Convert.ToDouble(buff.Split(',')[1]) < 1000)
                    {
                        RampOffList.Add(Convert.ToDouble(buff.Split(',')[1]) * 1000000);
                    }
                    if (RampOffList.Count == 0)
                    {
                        RampOffList.Add(999);
                    }
                    #endregion
                }
                else
                {
                    #region 其他协议
                    

                    LP_ScpiCommandQuery("FETCH:POWER:AVER?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);
                    double resultSignal = 0.0;
                    try
                    {
                        resultSignal = Convert.ToDouble(buff.Split(',')[1]);
                    }
                    catch
                    {
                        resultSignal = 999999;
                    }
                    PowerList.Add(resultSignal);
				   
					


                    //加 author 何苹 信道2 获取值（power） Samsung WiFi MIMO
                    if (chain.Equals("3"))
                    {
                        LP_ScpiCommandQuery("FETCH:POWER:SIGN2:AVER?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                        resultSignal = 0.0;
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[1]);
                        }
                        catch
                        {
                            resultSignal = 999999;
                        }
                        PowerList.Add(resultSignal);
                    }

                    //加 author 何苹 Samsung WiFi MIMO 获取通道1，2值 EVM 、FrequencyError 、Symbol Clock Error 、LOLeakage值
                    if (chain.Equals("3"))
                    {
                        //FETC:OFDM:EVM:ALL? 获取所有EVM值
                        double resultSignal2 = 0.0;

                        LP_ScpiCommandQuery("FETC:OFDM:EVM:ALL?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[1]);
                        }
                        catch
                        {
                            resultSignal = 999999;
                        }
                        try
                        {
                            resultSignal2 = Convert.ToDouble(buff.Split(',')[2]);
                        }
                        catch
                        {
                            resultSignal2 = 999999;
                        }
                        if (resultSignal == resultSignal2)
                        {
                            EvmList.Add(resultSignal2);
                            EvmList.Add(resultSignal);
                        }
                        else
                        {
                            if (resultSignal == 999999)
                            {
                                EvmList.Add(resultSignal2);
                                EvmList.Add(resultSignal2);
                            }
                            else if (resultSignal2 == 999999)
                            {
                                EvmList.Add(resultSignal);
                                EvmList.Add(resultSignal);
                            }
                            else
                            {
                                EvmList.Add(resultSignal);
                                EvmList.Add(resultSignal2);
                            }
                        }
                        //FETC:OFDM:FERR:ALL? 获取所有FrequencyError
                        LP_ScpiCommandQuery("FETC:OFDM:FERR:ALL?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[1]);
                        }
                        catch
                        {
                            resultSignal = 999999000;
                        }
                        try
                        {
                            resultSignal2 = Convert.ToDouble(buff.Split(',')[2]);
                        }
                        catch
                        {
                            resultSignal2 = 999999000;
                        }
                        FreqErrList.Add(resultSignal / 1000);
                        FreqErrList.Add(resultSignal2 / 1000);

                        //FETC:OFDM:SCER:ALL? Symbol Clock Error值
                        LP_ScpiCommandQuery("FETC:OFDM:SCER:ALL?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[1]);
                        }
                        catch
                        {
                            resultSignal = 999999;
                        }
                        try
                        {
                            resultSignal2 = Convert.ToDouble(buff.Split(',')[2]);
                        }
                        catch
                        {
                            resultSignal2 = 999999;
                        }
                        ClockErr.Add(resultSignal);
                        ClockErr.Add(resultSignal2);

                        //FETC:OFDM:LOL:ALL? LOLeakage值
                        LP_ScpiCommandQuery("FETC:OFDM:LOL:ALL?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[1]);
                        }
                        catch
                        {
                            resultSignal = 999999;
                        }
                        try
                        {
                            resultSignal2 = Convert.ToDouble(buff.Split(',')[2]);
                        }
                        catch
                        {
                            resultSignal2 = 999999;
                        }
                        LolList.Add(resultSignal);
                        LolList.Add(resultSignal2);
					 
																			
                    }
                    else //获取EVM 、FrequencyError 、Symbol Clock Error 、LOLeakage值 Composite列
                    {
                        LP_ScpiCommandQuery("FETC:TXQ:OFDM?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[1]);
                        }
                        catch
                        {
                            resultSignal = 999999;
                        }
                        EvmList.Add(resultSignal);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[3]);
                        }
                        catch
                        {
                            resultSignal = 999999000;
                        }
                        FreqErrList.Add(resultSignal / 1000);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[4]);
                        }
                        catch
                        {
                            resultSignal = 999999;
                        }
                        ClockErr.Add(resultSignal);
                        try
                        {
                            resultSignal = Convert.ToDouble(buff.Split(',')[5]);
                        }
                        catch
                        {
                            resultSignal = 999999;
                        }
                        LolList.Add(resultSignal);
                    }                    

                    //信道1 获取值（Freq Marg）
                    LP_ScpiCommandQuery("FETC:SPEC:SIGN1:MARG?".ToCharArray(), 128, re, out relenght);
                    buff = new string(re);
                    for (int i = 1; i <= 8; i++)
                    {
                        try
                        {
                            SPFarray[i - 1].Add(Convert.ToDouble(buff.Split(',')[i]));
                        }
                        catch
                        {
                            SPFarray[i - 1].Add(999999);
                        }
                    }

                    //加 author 何苹 信道2 获取值（Freq Marg） Samsung WiFi MIMO
                    if (chain.Equals("3"))
                    {
                        LP_ScpiCommandQuery("FETC:SPEC:SIGN2:MARG?".ToCharArray(), 128, re, out relenght);
                        buff = new string(re);
                        for (int i = 1; i <= 8; i++)
                        {
                            try
                            {
                                SPFarraySIGN2[i - 1].Add(Convert.ToDouble(buff.Split(',')[i]));
                            }
                            catch
                            {
                                SPFarraySIGN2[i - 1].Add(999999);
                            }
                        }
                    }

                    char[] sflre = new char[10240];
                    sflchecked = true;
                    LP_ScpiCommandQuery("FETC:OFDM:SFL:CHECK?".ToCharArray(), 10240, sflre, out relenght);
                    sfllist.Add(new string(sflre));
                    #region//废弃代码
                    //string llimitstr = null, hlimitstr = null;
                    //LP_ScpiCommandQuery("FETCh:OFDM:SFLatness:LLIMit?".ToCharArray(), 10240, re, out relenght);
                    //llimitstr = new string(re);
                    //LP_ScpiCommandQuery("FETCh:OFDM:SFLatness:HLIMit?".ToCharArray(), 10240, re, out relenght);
                    //hlimitstr = new string(re);
                    //double limit = 999;
                    //for (int limitchangeindex = 1; limitchangeindex < llimitstr.Split(',').Length; limitchangeindex++)
                    //{
                    //    int limitindex = 0;
                    //    SFLarray[limitindex] = new List<double>();
                    //    if (Convert.ToDouble(llimitstr.Split(',')[limitchangeindex]) < 999999)
                    //    {
                    //        if (limit == 999)
                    //        {
                    //            sfllimits.Add(llimitstr.Split(',')[limitchangeindex] + "," + hlimitstr.Split(',')[limitchangeindex]);
                    //        }
                    //        if (limit != 999 && limit != Convert.ToDouble(llimitstr.Split(',')[limitchangeindex]))
                    //        {
                    //            limitindex += 1;
                    //            SFLarray[limitindex] = new List<double>();
                    //            sfllimits.Add(llimitstr.Split(',')[limitchangeindex] + "," + hlimitstr.Split(',')[limitchangeindex]);
                    //        }
                    //        SFLarray[limitindex].Add(Convert.ToDouble(buff.Split(',')[limitchangeindex]));
                    //        limit = Convert.ToDouble(llimitstr.Split(',')[limitchangeindex]);
                    //    }
                    //    else
                    //    {
                    //        if (sfllimits != null)
                    //        {
                    //            limitindex += 1;
                    //            if (limitindex == 4) break;
                    //        }
                    //    }
                    //}
                    #endregion
                    #endregion
                }
            }

            #region //判断测试结果并输出到字典中
            double result = 0.0;
            int low = 0;
            result = PowerList[low];
            Log.GetInstance().d("Power", result.ToString());
            if (result <= Convert.ToDouble(criterias[0].Split(',')[1]) && result >= Convert.ToDouble(criterias[0].Split(',')[0]))
            {
                ResultDic.Add("Power", result.ToString("f2"));
            }
            else
            {
                ResultDic.Add("Power", result.ToString("f2") + ",FAIL");
            }
            result = EvmList[low];
            if (result <= Convert.ToDouble(criterias[1]))
            {
                ResultDic.Add("EVM", result.ToString("f2"));
            }
            else
            {
                ResultDic.Add("EVM", result.ToString("f2") + ",FAIL");
            }
            result = FreqErrList[low];
            if (result <= Convert.ToDouble(criterias[2].Split(',')[1]) && result >= Convert.ToDouble(criterias[2].Split(',')[0]))
            {
                ResultDic.Add("FreqErr", result.ToString("f2"));
            }
            else
            {
                ResultDic.Add("FreqErr", result.ToString("f2") + ",FAIL");
            }
            result = ClockErr[low];
            if (result <= Convert.ToDouble(criterias[3].Split(',')[1]) && result >= Convert.ToDouble(criterias[3].Split(',')[0]))
            {
                ResultDic.Add("ClockErr", result.ToString("f2"));
            }
            else
            {
                ResultDic.Add("ClockErr", result.ToString("f2") + ",FAIL");
            }
            result = LolList[low];
            if (result < Convert.ToDouble(criterias[4]))
            {
                ResultDic.Add("LOL", result.ToString("f2"));
            }
            else
            {
                ResultDic.Add("LOL", result.ToString("f2") + ",FAIL");
            }
            if (rampchecked == true)
            {
                result = RampOnList.Average();
                if (result < Convert.ToDouble(criterias[5]))
                {
                    ResultDic.Add("RampON", result.ToString("f2"));
                }
                else
                {
                    ResultDic.Add("RampON", result.ToString("f2") + ",FAIL");
                }

                result = RampOffList.Average();
                if (result < Convert.ToDouble(criterias[6]))
                {
                    ResultDic.Add("RampOFF", result.ToString("f2"));
                }
                else
                {
                    ResultDic.Add("RampOFF", result.ToString("f2") + ",FAIL");
                }
            }

            LP_ScpiCommandQuery("FETC:SPEC:SIGN1:MARG:OFR?".ToCharArray(), 128, re, out relenght);
            string freq = new string(re);
            for (int i = 0; i < SPFarray.Length; i++)
            {
                if (ArgumentType.Contains("80M"))
                {
                    if (i == 0 || i == 1 || i == SPFarray.Length-1 || i == SPFarray.Length-2) continue;
                }
                if (SPFarray[i] != null && SPFarray[i].Count != 0)
                {
                    result = SPFarray[i].Average();
                    if (result >= 0)
                    {
                        try
                        {
                            ResultDic.Add("FC_SIGN1_" + i.ToString() + "_" + Convert.ToDouble(freq.Split(',')[i+1]).ToString("f2"), result.ToString("f2"));
                        }
                        catch
                        {
                            ResultDic.Add("FC_SIGN1_" + i.ToString() + "_" +"999999", result.ToString("f2"));
                        }
                    }
                    else
                    {
                        try
                        {
                            try
                            {
                                ResultDic.Add("FC_SIGN1_" + i.ToString() + "_" + Convert.ToDouble(freq.Split(',')[i+1]).ToString("f2"), result.ToString("f2") + "FAIL");
                            }
                            catch
                            {
                                ResultDic.Add("FC_SIGN1_" + i.ToString() + "_" +"999999", result.ToString("f2") + "FAIL");
                            }
                        }
                        catch
                        {
                            ResultDic.Add("FC_SIGN1_" + i.ToString() + "_" + "999999", result.ToString("f2") + "FAIL");
                        }
                    }
                }
            }

            if (sflchecked == true)
            {
                foreach (string sfl in sfllist)
                {
                    if (sfl.Contains("1"))
                    {

                        if (ResultDic.Keys.Contains("SFL"))
                        {
                            ResultDic["SFL"] = "FAIL";
                        }
                        else
                        {
                            ResultDic.Add("SFL", "FAIL");
                        }
                        break;
                    }
                    if (ResultDic.Keys.Contains("SFL"))
                    {
                        ResultDic["SFL"] = "PASS";
                    }
                    else
                    {
                        ResultDic.Add("SFL", "PASS");
                    }
                }
                #region//废弃代码
                //for (int i = 0; i < 4; i++)
                //{
                //    if (Convert.ToDouble(sfllimits[i].Split(',')[0]) < SFLarray[i].Min() && Convert.ToDouble(sfllimits[i].Split(',')[1]) > SFLarray[i].Max())
                //    {
                //        if (Math.Abs(Convert.ToDouble(sfllimits[i].Split(',')[0]) - SFLarray[i].Min()) > Math.Abs(Convert.ToDouble(sfllimits[i].Split(',')[1]) - SFLarray[i].Max()))
                //        {
                //            ResultDic.Add("SFL" + i.ToString(), SFLarray[i].Max().ToString());
                //        }
                //        else
                //        {
                //            ResultDic.Add("SFL" + i.ToString(), SFLarray[i].Min().ToString());
                //        }
                //    }
                //    else
                //    {
                //        if (Convert.ToDouble(sfllimits[i].Split(',')[0]) > SFLarray[i].Min() && Convert.ToDouble(sfllimits[i].Split(',')[1]) > SFLarray[i].Max())
                //        {
                //            ResultDic.Add("SFL" + i.ToString(), SFLarray[i].Min().ToString() + ",FAIL");
                //        }
                //        else if (Convert.ToDouble(sfllimits[i].Split(',')[0]) < SFLarray[i].Min() && Convert.ToDouble(sfllimits[i].Split(',')[1]) < SFLarray[i].Max())
                //        {
                //            ResultDic.Add("SFL" + i.ToString(), SFLarray[i].Max().ToString() + ",FAIL");
                //        }
                //        else
                //        {
                //            ResultDic.Add("SFL" + i.ToString(), SFLarray[i].Min().ToString() + ",FAIL");
                //        }
                //    }
                //}
                #endregion
            }
            //if (m_hMeta.md != 0)
            //{
            //    MTK_StopTest();
            //}
            #endregion

            //加 author 何苹 Samsung WiFi MIMO 将另一通道值加入字典
            if (chain.Equals("3"))
            {
                low = 1;
                result = PowerList[low];
                if (result <= Convert.ToDouble(criterias[0].Split(',')[1]) && result >= Convert.ToDouble(criterias[0].Split(',')[0]))
                {
                    ResultDic.Add("Power2", result.ToString("f2"));
                }
                else
                {
                    ResultDic.Add("Power2", result.ToString("f2") + ",FAIL");
                }
                result = EvmList[low];
                if (result <= Convert.ToDouble(criterias[1]))
                {
                    ResultDic.Add("EVM2", result.ToString("f2"));
                }
                else
                {
                    ResultDic.Add("EVM2", result.ToString("f2") + ",FAIL");
                }
                result = FreqErrList[low];
                if (result <= Convert.ToDouble(criterias[2].Split(',')[1]) && result >= Convert.ToDouble(criterias[2].Split(',')[0]))
                {
                    ResultDic.Add("FreqErr2", result.ToString("f2"));
                }
                else
                {
                    ResultDic.Add("FreqErr2", result.ToString("f2") + ",FAIL");
                }
                result = ClockErr[low];
                if (result <= Convert.ToDouble(criterias[3].Split(',')[1]) && result >= Convert.ToDouble(criterias[3].Split(',')[0]))
                {
                    ResultDic.Add("ClockErr2", result.ToString("f2"));
                }
                else
                {
                    ResultDic.Add("ClockErr2", result.ToString("f2") + ",FAIL");
                }
                result = LolList[low];
                if (result < Convert.ToDouble(criterias[4]))
                {
                    ResultDic.Add("LOL2", result.ToString("f2"));
                }
                else
                {
                    ResultDic.Add("LOL2", result.ToString("f2") + ",FAIL");
                }


                LP_ScpiCommandQuery("FETC:SPEC:SIGN2:MARG:OFR?".ToCharArray(), 128, re, out relenght);
                freq = new string(re);
                for (int i = 0; i < SPFarraySIGN2.Length; i++)
                {
                    if (ArgumentType.Contains("80M"))
                    {
                        if (i == 0 || i == 1 || i == SPFarraySIGN2.Length - 1 || i == SPFarraySIGN2.Length - 2) continue;
                    }
                    if (SPFarraySIGN2[i] != null && SPFarraySIGN2[i].Count != 0)
                    {
                        result = SPFarraySIGN2[i].Average();
                        if (result >= 0)
                        {
                            try
                            {
                                ResultDic.Add("FC_SIGN2_" + i.ToString() + "_" + Convert.ToDouble(freq.Split(',')[i + 1]).ToString("f2"), result.ToString("f2"));
                            }
                            catch
                            {
                                ResultDic.Add("FC_SIGN2_" + i.ToString() + "_" + "999999", result.ToString("f2"));
                            }
                        }
                        else
                        {
                            try
                            {
                                try
                                {
                                    ResultDic.Add("FC_SIGN2_" + i.ToString() + "_" + Convert.ToDouble(freq.Split(',')[i + 1]).ToString("f2"), result.ToString("f2") + "FAIL");
                                }
                                catch
                                {
                                    ResultDic.Add("FC_SIGN2_" + i.ToString() + "_" + "999999", result.ToString("f2") + "FAIL");
                                }
                            }
                            catch
                            {
                                ResultDic.Add("FC_SIGN2_" + i.ToString() + "_" + "999999", result.ToString("f2") + "FAIL");
                            }
                        }
                    }
                }

                if (sflchecked == true)
                {
                    foreach (string sfl in sfllist)
                    {
                        if (sfl.Contains("1"))
                        {

                            if (ResultDic.Keys.Contains("SFL2"))
                            {
                                ResultDic["SFL2"] = "FAIL";
                            }
                            else
                            {
                                ResultDic.Add("SFL2", "FAIL");
                            }
                            break;
                        }
                        if (ResultDic.Keys.Contains("SFL2"))
                        {
                            ResultDic["SFL2"] = "PASS";
                        }
                        else
                        {
                            ResultDic.Add("SFL2", "PASS");
                        }
                    }                   
                }
            }

            return ResultDic;
        }
        
        
        /// <summary>
        /// 连接IQxel仪器
        /// </summary>
        /// <param name="ip">IQxel仪器地址</param>
        /// <param name="IQxel">仪器名 MW项目</param>
        public void ConnectIQxelIP(string ip,string IQxel)
        {
            int count = 0;
            int iQConType = 0;
            A: try
            {
                LP_Init(1, 0);
                switch (IQxel)
                {
                    case "160":iQConType = 1;
                        break;
                    case "MW":iQConType = 0;
                        break;

                }
                int temp=IQXEL_LP_InitTester(Marshal.StringToHGlobalAnsi(ip), iQConType);              
            }
            catch (Exception ex)
            {
                count++;
                if (count >= 3)
                {
                    MessageBox.Show("请检查IP地址或仪器与电脑是否正确连接\n" + ex.ToString(), "错误");
                }
                else
                {
                    goto A;
                }
            }
        }

        /// <summary>
        /// 连接手机
        /// </summary>
        /// <param name="PhonePort">高通手机串口</param>
        public void ConnectPhone(Int16 PhonePort,string ic)
        {
            string errmsg = null;
            Phoneport = PhonePort;
            try
            {
                if (ic !="MTK" && ic !="AT" && ic !="MTK New" && ic!="Samsung")
                {
                    qmslphone.DisconnectToServer();
                    qmslphone.ConnectToServer(PhonePort, Phone.TargetType.QLIB_TARGET_TYPE_MSM_MDM);
                    Log.GetInstance().d("Quammon", "连接成功");
                    if (Directory.Exists("Log\\QMSL") == false)//如果不存在就创建file文件夹
                    {
                        Directory.CreateDirectory("Log\\QMSL");
                    }
                    qmslphone.SetLogFlags();
                    qmslphone.StartLogging("Log\\QMSL\\" + " " + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".txt");
                }
                else if (ic == "AT")
                {
                    OpenPort("COM" + PhonePort.ToString());
                }
                else if (ic == "MTK")
                {
                    Connect();
                }
                else if (ic == "MTK New")
                {
                    ConnetWithUSB(@"C:\MyMETA", out errmsg);
                }
                //else if (ic == "Samsung")
                //{
                //   OpenSamsungPort("COM" + PhonePort.ToString());
                //}
            }
            catch (PhoneException pex)
            {
                MessageBox.Show(pex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
         
        /// <summary>
        /// 初始化手机(连接手机串口之后)
        /// </summary>
        /// <param name="type">3990、3980、6174、3980、3990</param>
        public void PhoneIni(string IC)
        {
            try
            {
                switch (IC)
                {
                    case "3660":
                        qmslphone.FTM_WLAN_GEN6_START(3660);
                        break;
                    case "3680":
                        qmslphone.FTM_WLAN_GEN6_START(3680);
                        break;
                    case "6174":
                        qmslphone.FTM_WLAN_Atheros_LoadDUT(System.Text.Encoding.Default.GetBytes("qc6174"), System.Text.Encoding.Default.GetBytes(System.Windows.Forms.Application.StartupPath + "\\boardData\\bdwlan30.bin"), 5, 62);
                        break;
                    case "3990":
                        qmslphone.FTM_WLAN_Atheros_LoadDUT(System.Text.Encoding.Default.GetBytes("qc6180"), System.Text.Encoding.Default.GetBytes(""), 5, 64);
                        break;
                    case "3980":
                        break;
                    case "6750":
                    case "6391":
                        qmslphone.FTM_WLAN_SetModuleType(Phone.WLAN_moduleType.Atheros);
                        qmslphone.FTM_WLAN_Atheros_LoadDUT(System.Text.Encoding.Default.GetBytes("qc6180"), System.Text.Encoding.Default.GetBytes(System.Windows.Forms.Application.StartupPath + "\\boardData\\bdwlan30.bin"), 5, 62);

                        qmslphone.FTM_WLAN_TLV2_Create(200);
                        if (IC == "6750")
                        {
                            qmslphone.FTM_WLAN_TLV2_Complete();
                            qmslphone.FTM_WLAN_Atheros_LoadDUT(System.Text.Encoding.Default.GetBytes("qc6390"), System.Text.Encoding.Default.GetBytes(System.Windows.Forms.Application.StartupPath + "\\boardData\\bdwlan30.bin"), 5, 62);
                        }
                        if (IC == "6391")
                        {
                            qmslphone.FTM_WLAN_TLV2_Complete();
                            qmslphone.FTM_WLAN_Atheros_LoadDUT(System.Text.Encoding.Default.GetBytes("ipq807x"), System.Text.Encoding.Default.GetBytes(""), 5, 62);
                        }
                        //byte[] reframebyte = new byte[100];
                        //qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.Default.GetBytes("ver_info"), reframebyte);
                        //string buff = System.Text.Encoding.ASCII.GetString(reframebyte);
                        break;
                    case "AT":
                        IniDut();
                        break;
                }
            }
            catch (PhoneException pex)
            {
                MessageBox.Show(pex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 打开MIMO(IQxel160)测试端口
        /// </summary>
        /// <param name="TXorRX">true为发射端口，false为接收端口</param>
        public void MimoOpenIQxelWiFiTestPort(bool TXorRX,string IQxel)
        {
            switch (IQxel)
            {
                case "160":
                    if (TXorRX == true)
                    {
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF1B,VSA12"));
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF2,VSA11"));
                    }
                    else
                    {
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF1B,OFF"));
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF2B,OFF"));
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF1,OFF"));
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF2,OFF"));

                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF1B,VSG12"));
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF2,VSG11"));
                        //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF1B,VSA12"));
                        //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI"));
                    }
                    break;
                case "MW":
                    if (TXorRX == true)
                    {
                        //MROUT2; PORT: RES RF1B, VSA12
                        //MROUT1;PORT:RES RF1A,VSA11
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF1B,VSA12"));
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF1A,VSA11"));
                    }
                    else
                    {
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT2;PORT:RES RF1B,VSG12"));
                        LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF1A,VSG11"));
                    }
                    break;
            }
        }

        int Channel = 0;
        /// <summary>
        /// 接收测试
        /// </summary>
        /// <param name="testtype">测试类型：0为灵敏度测试，1为最大接收电平测试</param>
        /// <param name="IC">芯片</param>
        /// <param name="Argument">协议</param>
        /// <param name="RateStr">速率</param>
        /// <param name="channelfreqMHz"></param>
        /// <param name="rfPowerLeveldBm">仪器发射功率</param>
        /// <param name="RatebitIndex">QRC速率索引</param>
        /// <param name="chain">天线</param>
        /// <param name="port">仪器端口</param>
        /// <returns>灵敏度返回最小接受功率，最大接收电平返回手机接收报数</returns>
        public string SetIQxelVsg(string CriteriasFilePath, Vsg_Type testtype, string IC, string Argument, string RateStr, double channelfreqMHz, double rfPowerLeveldBm, string RatebitIndex, string IQxel, string chain, int Symbol, ICType.MTK.MT6635 mT6635 = null,int port = 2)
        {
            bool mtkParaSeted = false;//判断MTK是否已经设置参数
            int TXDONECOUNT = 999;       
            string phyId="";
            string result = null;
            Dictionary<string, string> ResultLimitDic = new Dictionary<string, string>();
            Dictionary<string, string> ResultDic = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(CriteriasFilePath);
            foreach (XmlElement xmlArgument in doc.DocumentElement.GetElementsByTagName("Agreement"))
            {
                if (Argument == xmlArgument.GetAttribute("name"))
                {
                    foreach (XmlElement xmlRate in xmlArgument.GetElementsByTagName("ratebit"))
                    {
                        if (RateStr == xmlRate.GetAttribute("name"))
                        {
                            foreach (XmlElement xmlLimits in xmlRate.GetElementsByTagName("limits"))
                            {
                                ResultLimitDic.Add(Argument + "-" + RateStr + "-" + xmlLimits.GetAttribute("name"), xmlLimits.InnerText);
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            string path = "MainTestQueue\\WiFi\\config\\SenDealType.xml";
            XmlDocument senDealTypeXml = new XmlDocument();
            senDealTypeXml.Load(path);
            int deal_type = 0;
            Dictionary<string, string> typeResult = new Dictionary<string, string>();
            foreach (XmlElement element in senDealTypeXml.DocumentElement.GetElementsByTagName("item"))
            {
                switch (element.GetAttribute("name"))
                {
                    case "deal_type":
                        deal_type =Convert.ToInt32(element.InnerText);
                        break;
                    case "type0":
                        typeResult.Add("type0",element.InnerText);
                        break;
                    case "type1":
                        typeResult.Add("type1", element.InnerText);
                        break;
                    case "type2":
                        typeResult.Add("type2", element.InnerText);
                        break;
                }
            }

            string waveformName = null, wlanmode = null,rateBW = "";
            byte[] reframebyte = new byte[100];
            uint GetFrameCount = 0, LimitCount = 900;
            int sleepTime = 2000;
            #region 波形文件
            if (IC == "3990")
            {
                #region 3990
                if (Argument == "2G4_B")
                {
                    wlanmode = "4";
                    switch (RateStr)
                    {
                        case "1Mbit/s":
                            RatebitIndex = "0";
                            sleepTime = 11000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_DSSS-1.iqvsg";
                            break;
                        case "2Mbit/s":
                            RatebitIndex = "1";
                            sleepTime = 8000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_DSSS-2L.iqvsg";
                            break;
                        case "5.5Mbit/s":
                            RatebitIndex = "2";
                            sleepTime = 5000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_CCK-5_5L.iqvsg";
                            break;
                        case "11Mbit/s":
                            RatebitIndex = "3";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_CCK-11L.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_G" || Argument == "5G1_A" || Argument == "5G4_A" || Argument == "5G8_A")
                {
                    wlanmode = "0";
                    switch (RateStr)
                    {
                        case "6Mbit/s":
                            RatebitIndex = "7";
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-6.iqvsg";
                            break;
                        case "9Mbit/s":
                            RatebitIndex = "8";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-9.iqvsg";
                            break;
                        case "12Mbit/s":
                            RatebitIndex = "9";
                            sleepTime = 3000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-12.iqvsg";
                            break;
                        case "18Mbit/s":
                            RatebitIndex = "10";
                            sleepTime = 3000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-18.iqvsg";
                            break;
                        case "24Mbit/s":
                            RatebitIndex = "11";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-24.iqvsg";
                            break;
                        case "36Mbit/s":
                            RatebitIndex = "12";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-36.iqvsg";
                            break;
                        case "48Mbit/s":
                            RatebitIndex = "13";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-48.iqvsg";
                            break;
                        case "54Mbit/s":
                            RatebitIndex = "14";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-54.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_N20M" || Argument == "5G1_N20M" || Argument == "5G4_N20M" || Argument == "5G8_N20M")
                {
                    wlanmode = "1";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "15";
                            sleepTime = 7000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "16";
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "17";
                            sleepTime = 5000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "18";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "19";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "20";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "21";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "22";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS7_LDPC.iqvsg";
                            break;

                        //add MCS8-MCS15 author 何苹 高通 WiFi MIMO
                        case "MCS8":
                            RatebitIndex = "61";
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "62";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS9_LDPC.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "63";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS10_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "64";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS11_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            RatebitIndex = "65";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS12_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            RatebitIndex = "66";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS13_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            RatebitIndex = "67";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS14_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            RatebitIndex = "68";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS15_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_N40M" || Argument == "5G1_N40M" || Argument == "5G4_N40M" || Argument == "5G8_N40M")
                {
                    wlanmode = "2";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "23";
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "24";
                            sleepTime = 5000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "25";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "26";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "27";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "28";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "29";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "30";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS7_LDPC.iqvsg";
                            break;

                        //add MCS8-MCS15 author 何苹 高通 WiFi MIMO
                        case "MCS8":
                            RatebitIndex = "69";
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "70";
                            sleepTime = 5000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS9_LDPC.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "71";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS10_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "72";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS11_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            RatebitIndex = "73";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS12_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            RatebitIndex = "74";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS13_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            RatebitIndex = "75";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS14_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            RatebitIndex = "76";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS15_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AC20M" || Argument == "5G4_AC20M" || Argument == "5G8_AC20M")
                {
                    wlanmode = "5";

                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "31";
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "32";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "33";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "34";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "35";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "36";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "37";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "38";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "39";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS8_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AC40M" || Argument == "5G4_AC40M" || Argument == "5G8_AC40M")
                {
                    wlanmode = "6";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "41";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "42";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "43";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "44";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "45";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "46";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "47";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "48";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "49";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "50";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS9_LDPC.iqvsg";
                            break;

                        //add MCS10-MCS19 author 何苹 高通 WiFi MIMO
                        case "MCS10":
                            RatebitIndex = "87";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "88";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            RatebitIndex = "89";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            RatebitIndex = "90";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            RatebitIndex = "91";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            RatebitIndex = "92";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS16":
                            RatebitIndex = "93";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS17":
                            RatebitIndex = "94";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS18":
                            RatebitIndex = "95";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS19":
                            RatebitIndex = "96";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS9_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AC80M" || Argument == "5G4_AC80M" || Argument == "5G8_AC80M")
                {
                    wlanmode = "8";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "51";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "52";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "53";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "54";
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "55";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "56";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "57";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "58";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "59";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "60";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS9_LDPC.iqvsg";
                            break;

                        //add MCS8-MCS15 author 何苹 高通 WiFi MIMO
                        case "MCS10":
                            RatebitIndex = "97";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "98";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            RatebitIndex = "99";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            RatebitIndex = "100";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            RatebitIndex = "101";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            RatebitIndex = "102";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS16":
                            RatebitIndex = "103";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS17":
                            RatebitIndex = "104";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS18":
                            RatebitIndex = "105";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS19":
                            RatebitIndex = "106";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS9_LDPC.iqvsg";
                            break;
                    }
                }

                #endregion
            }
            else if (IC.Equals("6391") || IC.Equals("6750"))
            {
                #region 6391
                //2019 10 07 未修改波形文件（可不修改）、wlanmode已修改、RatebitIndex已修改 6391
                if (Argument == "2G4_B")
                {
                    wlanmode = "0";
                    rateBW = "0";
                    switch (RateStr)
                    {
                        case "1Mbit/s":
                            sleepTime = 11000;
                            RatebitIndex = "0";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_DSSS-1.iqvsg";
                            break;
                        case "2Mbit/s":
                            sleepTime = 8000;
                            RatebitIndex = "1";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_DSSS-2L.iqvsg";
                            break;
                        case "5.5Mbit/s":
                            sleepTime = 3000;
                            RatebitIndex = "2";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_CCK-5_5L.iqvsg";
                            break;
                        case "11Mbit/s":
                            sleepTime = 2000;
                            RatebitIndex = "3";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_CCK-11L.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_G" || Argument == "5G1_A" || Argument == "5G4_A" || Argument == "5G8_A")
                {
                    wlanmode = "0";
                    rateBW = "1";
                    switch (RateStr)
                    {
                        case "6Mbit/s":
                            sleepTime = 2000;
                            RatebitIndex = "10";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-6.iqvsg";
                            break;
                        case "9Mbit/s":
                            sleepTime = 2000;
                            RatebitIndex = "11";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-9.iqvsg";
                            break;
                        case "12Mbit/s":
                            sleepTime = 1500;
                            RatebitIndex = "12";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-12.iqvsg";
                            break;
                        case "18Mbit/s":
                            sleepTime = 1500;
                            RatebitIndex = "13";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-18.iqvsg";
                            break;
                        case "24Mbit/s":
                            sleepTime = 1500;
                            RatebitIndex = "14";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-24.iqvsg";
                            break;
                        case "36Mbit/s":
                            sleepTime = 1500;
                            RatebitIndex = "15";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-36.iqvsg";
                            break;
                        case "48Mbit/s":
                            sleepTime = 1500;
                            RatebitIndex = "16";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-48.iqvsg";
                            break;
                        case "54Mbit/s":
                            sleepTime = 1500;
                            RatebitIndex = "17";
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-54.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_N20M" || Argument == "5G1_N20M" || Argument == "5G4_N20M" || Argument == "5G8_N20M")
                {
                    wlanmode = "0";
                    rateBW = "2";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 4000;
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 2000;
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 2000;
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS10_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            sleepTime = 2000;
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS11_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            sleepTime = 2000;
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS12_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            sleepTime = 2000;
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS13_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            sleepTime = 2000;
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS14_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            sleepTime = 2000;
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS15_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS7_LDPC.iqvsg";
                            break;

                    }
                }
                else if (Argument == "2G4_N40M" || Argument == "5G1_N40M" || Argument == "5G4_N40M" || Argument == "5G8_N40M")
                {
                    wlanmode = "1";
                    rateBW = "3";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 2000;
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 2000;
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 2000;
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS10_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            sleepTime = 2000;
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS11_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            sleepTime = 2000;
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS12_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            sleepTime = 1500;
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS13_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            sleepTime = 1500;
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS14_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            sleepTime = 1500;
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS15_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS7_LDPC.iqvsg";
                            break;
                    }
                }

                else if (Argument == "5G1_AC20M" || Argument == "5G4_AC20M" || Argument == "5G8_AC20M")
                {
                    wlanmode = "0";
                    rateBW = "4";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 4000;
                            RatebitIndex = "20";
                            if(chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS0_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 3000;
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS1_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 3000;
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS2_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            sleepTime = 2500;
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS3_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS4_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS5_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS6_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS7_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS8_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AC40M" || Argument == "5G4_AC40M" || Argument == "5G8_AC40M")
                {
                    wlanmode = "1";
                    rateBW = "5";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS0_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS1_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS2_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS3_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS4_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS5_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS6_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS7_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS9_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AC80M" || Argument == "5G4_AC80M" || Argument == "5G8_AC80M")
                {
                    wlanmode = "12";
                    rateBW = "6";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS0_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS1_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS2_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS3_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS4_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS5_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS6_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS7_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS9_LDPC.iqvsg";
                            break;
                    }
                }
                
                //2019 10 07 已修改波形文件、wlanmode未修改、RatebitIndex未修改 6193
                else if (Argument == "2G4_AX20M" || Argument == "5G1_AX20M" || Argument == "5G4_AX20M" || Argument == "5G8_AX20M")
                {
                    wlanmode = "0";
                    rateBW = "8";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 3000;
                            RatebitIndex = "20";
                            if(chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS0.iqvsg"; 
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS0.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 3000;
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS1.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS1.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 3000;
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS2.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS2.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS3.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS3.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS4.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS4.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS5.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS5.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS6.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS6.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS7.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS7.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS8.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS8.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS9.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS9.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "30";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS10.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS10.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "31";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS11.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS11.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_AX40M" || Argument == "5G1_AX40M" || Argument == "5G4_AX40M" || Argument == "5G8_AX40M")
                {
                    wlanmode = "1";
                    rateBW = "9";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 3000;
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS0.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS0.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 3000;
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS1.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS1.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 3000;
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS2.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS2.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS3.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS3.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS4.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS4.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS5.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS5.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS6.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS6.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS7.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS7.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS8.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS8.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS9.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS9.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "30";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS10.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS10.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "31";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS11.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS11.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AX80M" || Argument == "5G4_AX80M" || Argument == "5G8_AX80M")
                {
                    wlanmode = "12";
                    rateBW = "10";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS0.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS0.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS1.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS1.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS2.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS2.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS3.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS3.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS4.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS4.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS5.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS5.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS6.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS6.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS7.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS7.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS8.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS8.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS9.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS9.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "30";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS10.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS10.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "31";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS11.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS11.iqvsg";
                            break;
                    }
                }
                #endregion
            }
            else
            {
                #region 3660 3680 MTK
                if (Argument == "2G4_B")
                {
                    wlanmode = "4";
                    switch (RateStr)
                    {
                        case "1Mbit/s":
                            sleepTime = 11000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_DSSS-1.iqvsg";
                            break;
                        case "2Mbit/s":
                            sleepTime = 9000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_DSSS-2L.iqvsg";
                            break;
                        case "5.5Mbit/s":
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_CCK-5_5L.iqvsg";
                            break;
                        case "11Mbit/s":
                            sleepTime = 4000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\WiFi_CCK-11L.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_G" || Argument == "5G1_A" || Argument == "5G4_A" || Argument == "5G8_A")
                {
                    wlanmode = "1";
                    switch (RateStr)
                    {
                        case "6Mbit/s":
                            sleepTime = 6000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-6.iqvsg";
                            break;
                        case "9Mbit/s":
                            sleepTime = 5000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-9.iqvsg";
                            break;
                        case "12Mbit/s":
                            sleepTime = 3000;
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-12.iqvsg";
                            break;
                        case "18Mbit/s":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-18.iqvsg";
                            break;
                        case "24Mbit/s":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-24.iqvsg";
                            break;
                        case "36Mbit/s":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-36.iqvsg";
                            break;
                        case "48Mbit/s":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-48.iqvsg";
                            break;
                        case "54Mbit/s":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\WiFi_OFDM-54.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_N20M" || Argument == "5G1_N20M" || Argument == "5G4_N20M" || Argument == "5G8_N20M")
                {
                    wlanmode = "1";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 5000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 4000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 3000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS10_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS11_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS12_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS13_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS14_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS15_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\WiFi_HT20_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS9_LDPC.iqvsg";
                            break;
                        case "MCS10":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS10_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS11_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS12_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS13_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS14_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M MIMO\\WiFi_HT20_MCS15_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_N40M" || Argument == "5G1_N40M" || Argument == "5G4_N40M" || Argument == "5G8_N40M")
                {
                    wlanmode = "2";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 6000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 5000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 4000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS10_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            sleepTime = 3500;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS11_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            sleepTime = 3000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS12_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS13_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS14_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS15_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\WiFi_HT40_MCS7_LDPC.iqvsg";
                            break;
                        //Samsung WiFi MIMO
                        case "MCS9":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS9_LDPC.iqvsg";
                            break;
                        case "MCS10":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS10_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS11_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS12_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS13_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS14_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M MIMO\\WiFi_HT40_MCS15_LDPC.iqvsg";
                            break;
                    }
                }

                else if (Argument == "5G1_AC20M" || Argument == "5G4_AC20M" || Argument == "5G8_AC20M")
                {
                    wlanmode = "5";

                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 6000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS0_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 5000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS1_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 4000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS2_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            sleepTime = 3000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS3_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS4_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS5_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS6_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS7_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M MIMO\\WiFi_11AC_VHT20_S2_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\WiFi_11AC_VHT20_S1_MCS8_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AC40M" || Argument == "5G4_AC40M" || Argument == "5G8_AC40M")
                {
                    wlanmode = "6";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 6000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS0_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 5000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS1_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 4000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS2_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            sleepTime = 3000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS3_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS4_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS5_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS6_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS7_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\WiFi_11AC_VHT40_S1_MCS9_LDPC.iqvsg";
                            break;

                        //add hp samsung wifi mimo
                        case "MCS10":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS16":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS17":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS18":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS19":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M MIMO\\WiFi_11AC_VHT40_S2_MCS9_LDPC.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AC80M" || Argument == "5G4_AC80M" || Argument == "5G8_AC80M")
                {
                    wlanmode = "8";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 6000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS0_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 5000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS1_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 4000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS2_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS3":
                            sleepTime = 3000;
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS3_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS4":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS4_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS5":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS5_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS6":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS6_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS7":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS7_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS8":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS8_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS9":
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS9_LDPC.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\WiFi_11AC_VHT80_S1_MCS9_LDPC.iqvsg";
                            break;
                        case "MCS10":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS0_LDPC.iqvsg";
                            break;
                        case "MCS11":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS1_LDPC.iqvsg";
                            break;
                        case "MCS12":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS2_LDPC.iqvsg";
                            break;
                        case "MCS13":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS3_LDPC.iqvsg";
                            break;
                        case "MCS14":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS4_LDPC.iqvsg";
                            break;
                        case "MCS15":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS5_LDPC.iqvsg";
                            break;
                        case "MCS16":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS6_LDPC.iqvsg";
                            break;
                        case "MCS17":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS7_LDPC.iqvsg";
                            break;
                        case "MCS18":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS8_LDPC.iqvsg";
                            break;
                        case "MCS19":
                            waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M MIMO\\WiFi_11AC_VHT80_S2_MCS9_LDPC.iqvsg";
                            break;
                    }
                }

                else if (Argument == "2G4_AX20M" || Argument == "5G1_AX20M" || Argument == "5G4_AX20M" || Argument == "5G8_AX20M")
                {
                    wlanmode = "0";
                    rateBW = "8";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 3000;
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS0.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS0.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 3000;
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS1.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS1.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 3000;
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS2.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS2.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS3.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS3.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS4.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS4.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS5.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS5.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS6.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS6.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS7.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS7.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS8.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS8.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS9.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS9.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "30";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS10.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS10.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "31";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M MIMO\\WiFi_11AX_HE20_S2_MCS11.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX20M\\WiFi_11AX_HE20_S1_MCS11.iqvsg";
                            break;
                    }
                }
                else if (Argument == "2G4_AX40M" || Argument == "5G1_AX40M" || Argument == "5G4_AX40M" || Argument == "5G8_AX40M")
                {
                    wlanmode = "1";
                    rateBW = "9";
                    switch (RateStr)
                    {
                        case "MCS0":
                            sleepTime = 3000;
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS0.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS0.iqvsg";
                            break;
                        case "MCS1":
                            sleepTime = 3000;
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS1.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS1.iqvsg";
                            break;
                        case "MCS2":
                            sleepTime = 3000;
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS2.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS2.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS3.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS3.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS4.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS4.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS5.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS5.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS6.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS6.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS7.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS7.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS8.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS8.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS9.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS9.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "30";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS10.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS10.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "31";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M MIMO\\WiFi_11AX_HE40_S2_MCS11.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX40M\\WiFi_11AX_HE40_S1_MCS11.iqvsg";
                            break;
                    }
                }
                else if (Argument == "5G1_AX80M" || Argument == "5G4_AX80M" || Argument == "5G8_AX80M")
                {
                    wlanmode = "12";
                    rateBW = "10";
                    switch (RateStr)
                    {
                        case "MCS0":
                            RatebitIndex = "20";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS0.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS0.iqvsg";
                            break;
                        case "MCS1":
                            RatebitIndex = "21";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS1.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS1.iqvsg";
                            break;
                        case "MCS2":
                            RatebitIndex = "22";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS2.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS2.iqvsg";
                            break;
                        case "MCS3":
                            RatebitIndex = "23";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS3.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS3.iqvsg";
                            break;
                        case "MCS4":
                            RatebitIndex = "24";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS4.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS4.iqvsg";
                            break;
                        case "MCS5":
                            RatebitIndex = "25";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS5.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS5.iqvsg";
                            break;
                        case "MCS6":
                            RatebitIndex = "26";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS6.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS6.iqvsg";
                            break;
                        case "MCS7":
                            RatebitIndex = "27";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS7.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS7.iqvsg";
                            break;
                        case "MCS8":
                            RatebitIndex = "28";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS8.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS8.iqvsg";
                            break;
                        case "MCS9":
                            RatebitIndex = "29";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS9.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS9.iqvsg";
                            break;
                        case "MCS10":
                            RatebitIndex = "30";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS10.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS10.iqvsg";
                            break;
                        case "MCS11":
                            RatebitIndex = "31";
                            if (chain.Equals("3"))
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M MIMO\\WiFi_11AX_HE80_S2_MCS11.iqvsg";
                            else
                                waveformName = "MainTestQueue\\WiFi\\waveforms\\AX80M\\WiFi_11AX_HE80_S1_MCS11.iqvsg";
                            break;
                    }
                }
                #endregion
            }

            WaveFromIC waveFromIC = WiFiIQWaveForm.WaveFromICs.Where(wf=>wf.ICtype.Equals(IC)).First();
            WaveFormInfo waveformInfo = waveFromIC.WaveFormInfos.Where(
                wfs=>wfs.IsLDPC==waveFromIC.IsLDPC&&
                wfs.IsMIMO==chain.Equals("3")&&
                Argument.Contains(wfs.Argument)&&
                wfs.Rate.Equals(RateStr)).First();

            waveformName = waveformInfo.WaveformName;
            wlanmode = waveformInfo.Wlanmode;
            rateBW = waveformInfo.RateBW;
            RatebitIndex = waveformInfo.RatebitIndex;
            sleepTime = waveformInfo.SleepTime;

            #endregion

            if (IQxel.Equals("80"))
            {
                if (Argument == "2G4_B")
                {
                    waveformName = "MainTestQueue\\WiFi\\waveforms\\2G4_B\\" + waveformName;
                }
                else if (Argument == "2G4_G" || Argument == "5G1_A" || Argument == "5G4_A" || Argument == "5G8_A")
                {
                    waveformName = "MainTestQueue\\WiFi\\waveforms\\G_A\\" + waveformName;
                }
                else if (Argument == "2G4_N20M" || Argument == "5G1_N20M" || Argument == "5G4_N20M" || Argument == "5G8_N20M")
                {
                    waveformName = "MainTestQueue\\WiFi\\waveforms\\N20M\\" + waveformName;
                }
                else if (Argument == "2G4_N40M" || Argument == "5G1_N40M" || Argument == "5G4_N40M" || Argument == "5G8_N40M")
                {
                    waveformName = "MainTestQueue\\WiFi\\waveforms\\N40M\\" + waveformName;
                }
                else if (Argument == "2G4_AC20M" || Argument == "5G1_AC20M"|| Argument == "5G4_AC20M" || Argument == "5G8_AC20M")
                {
                    waveformName = "MainTestQueue\\WiFi\\waveforms\\AC20M\\" + waveformName;
                }
                else if (Argument == "5G1_AC40M" || Argument == "5G4_AC40M" || Argument == "5G8_AC40M")
                {
                    waveformName = "MainTestQueue\\WiFi\\waveforms\\AC40M\\" + waveformName;
                }
                else if (Argument == "5G1_AC80M" || Argument == "5G4_AC80M" || Argument == "5G8_AC80M")
                {
                    waveformName = "MainTestQueue\\WiFi\\waveforms\\AC80M\\" + waveformName;
                }
            }
            #region 测试
            if (IC == "3990"||IC.Equals("6391") || IC.Equals("6750"))//modified by wzt at 2021.04.23
            {
                ConnectPhone(Phoneport, IC);
            }
            if (Argument == "2G4_B")
            {
                LimitCount = 920;
            }
            else
            {
                LimitCount = 900;
            }
            //LP_EnableVsgRF(0);
            //LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
            //LP_SetVsgModulation_SetPlayCondition(waveformName, false, 0);
            //LP_EnableVsgRF(1);
            switch (testtype)
            {
                case Vsg_Type.Sensitivity:
                    #region 灵敏度测试
                    rfPowerLeveldBm = Convert.ToDouble(ResultLimitDic[Argument + "-" + RateStr + "-" + "最小接收灵敏度"]);
                    if (deal_type == 1&& Convert.ToDouble(typeResult["type1"])!=0)
                    {
                        rfPowerLeveldBm = Convert.ToDouble(typeResult["type1"]);
                        using (StreamReader sr = new StreamReader("MainTestQueue\\WiFi\\config\\SenT1Info", Encoding.Default, true))
                        {
                            string data = sr.ReadLine();
                            if (data != null && !data.Equals(""))
                            {
                                string testInfo = data.Split('%')[1];
                                string[] itemInfo = testInfo.Split('\t');
                                if (itemInfo[0].Split(':')[1].Equals(IC) && itemInfo[1].Split(':')[1].Equals(Argument) && itemInfo[2].Split(':')[1].Equals(RateStr))
                                {
                                    rfPowerLeveldBm = Convert.ToDouble(data.Split('%')[2])+3.5;                                    
                                }                                
                            }
                        }
                    }                    
                    //add hp samsung wifi mimo
                    switch (IQxel)
                    {
                        case "80":
                            LP_EnableVsgRF(0);
                            LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                            LP_SetVsgModulation_SetPlayCondition(waveformName, false, 0);
                            LP_EnableVsgRF(1);
                            Thread.Sleep(500);
                            break;
                        case "160":
                            //MVSGALL;POW:lev -30 add
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;POW:lev " + rfPowerLeveldBm + "")); //add
                            waveformName = "user/" + waveformName;      //add  
                            //MVSGALL; add
                            //WAVE:LOAD "/user/WiFi_HT20_MCS9.iqvsg"   add
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;WAVE:LOAD \"" + waveformName + "\""));
                            //WLIS: COUN 1000 add
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;WLIS:COUN 1000"));
                            //CHAN1;WIFI; HSET:ALL MVSGALL add
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL MVSGALL"));
                            break;
                        case "MW":
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("ROUT1;PORT:RES RF1A,VSG1"));
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;POW:lev " + rfPowerLeveldBm + "")); //add
                            waveformName = "user/" + waveformName;      //add  
                            //if (Argument.Contains("N20M") || Argument.Contains("N40M"))          //上面改了这个没用啦！
                            //    waveformName = waveformName.Split('.').First().Replace("_LDPC", "") + "_LDPC.iqvsg";
                            //MVSGALL; add
                            //WAVE:LOAD "/user/WiFi_HT20_MCS9.iqvsg"   add
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WAVE:LOAD \"" + waveformName + "\""));
                            //WLIS: COUN 1000 add
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WLIS:COUN 1000"));
                            //CHAN1;WIFI; HSET:ALL MVSGALL add
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;HSET:ALL VSG1"));

                            //LP_EnableVsgRF(0);
                            //LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                            //LP_SetVsgModulation_SetPlayCondition(waveformName, false, 0);
                            //LP_EnableVsgRF(1);
                            break;
                    }

                    int testindex = 0;
                    sleepTime += 1000;

                    if (deal_type == 1)
                    {
                        double step = 2;
                        double lastLevel = rfPowerLeveldBm;
                        int count = 0;
                        do
                        {
                            int i = 0;
                            uint maxCount = 0;
                            while (i < 2)
                            {
                                //add hp samsung wifi mimo
                                setRXLevel(channelfreqMHz, rfPowerLeveldBm, IQxel, port);
                                setPhoneParam(IC, Argument, RateStr, channelfreqMHz, RatebitIndex, chain, mT6635, ref mtkParaSeted, ref phyId, wlanmode, rateBW);
                                //add hp samsung wifi mimo
                                TXDONECOUNT = setWaveform(IQxel, TXDONECOUNT, waveformName, sleepTime);
                                Thread.Sleep(500);
                                GetFrameCount = getFrameCount(IC, reframebyte, IQxel, waveformName, sleepTime, Argument, RateStr, chain, phyId, mT6635);

                                if (GetFrameCount > maxCount)
                                {
                                    maxCount = GetFrameCount;
                                }
                                i++;
                            }
                            GetFrameCount = maxCount;

                            Log.GetInstance().i("WiFi", "发包数：1000，收包数：" + GetFrameCount.ToString() + "，仪器发射功率：" + rfPowerLeveldBm.ToString());

                            if (GetFrameCount > LimitCount && GetFrameCount < LimitCount + 20 && count > 3)
                            {
                                break;
                            }
                            else if (GetFrameCount < LimitCount)
                            {
                                if (count < 2)
                                {
                                    continue;
                                }
                                rfPowerLeveldBm = rfPowerLeveldBm + step;
                                if (lastLevel < rfPowerLeveldBm)
                                {
                                    rfPowerLeveldBm = rfPowerLeveldBm - step;
                                    continue;
                                }
                                lastLevel = rfPowerLeveldBm;
                                if (step > 0.2 && count > 2)
                                {
                                    step = step / 2;
                                }
                            }
                            else
                            {
                                rfPowerLeveldBm = rfPowerLeveldBm - step;
                            }
                            count++;
                        } while (true);
                    }
                    else if (deal_type == 2)
                    {
                        double low = Convert.ToDouble(typeResult["type2"].Split(':').First());//-120
                        double high = Convert.ToDouble(typeResult["type2"].Split(':').Last());//-70
                        do
                        {
                            rfPowerLeveldBm = (low + high) / 2;

                            //add hp samsung wifi mimo
                            setRXLevel(channelfreqMHz, rfPowerLeveldBm, IQxel, port);
                            setPhoneParam(IC, Argument, RateStr, channelfreqMHz, RatebitIndex, chain, mT6635, ref mtkParaSeted, ref phyId, wlanmode, rateBW);
                            //add hp samsung wifi mimo
                            TXDONECOUNT = setWaveform(IQxel, TXDONECOUNT, waveformName, sleepTime);
                            Thread.Sleep(500);
                            GetFrameCount = getFrameCount(IC, reframebyte, IQxel, waveformName, sleepTime, Argument, RateStr, chain, phyId, mT6635);
                            Log.GetInstance().i("WiFi", "发包数：1000，收包数：" + GetFrameCount.ToString() + "，仪器发射功率：" + rfPowerLeveldBm.ToString() + " " + high.ToString() + " " + low.ToString() + " " + LimitCount.ToString());

                            int i = 0;
                            while (GetFrameCount == 0 && i < 2)
                            {
                                //add hp samsung wifi mimo
                                setRXLevel(channelfreqMHz, rfPowerLeveldBm, IQxel, port);
                                setPhoneParam(IC, Argument, RateStr, channelfreqMHz, RatebitIndex, chain, mT6635, ref mtkParaSeted, ref phyId, wlanmode, rateBW);
                                //add hp samsung wifi mimo
                                TXDONECOUNT = setWaveform(IQxel, TXDONECOUNT, waveformName, sleepTime);
                                Thread.Sleep(500);
                                GetFrameCount = getFrameCount(IC, reframebyte, IQxel, waveformName, sleepTime, Argument, RateStr, chain, phyId, mT6635);
                                Log.GetInstance().i("WiFi", "发包数：1000，收包数：" + GetFrameCount.ToString() + "，仪器发射功率：" + rfPowerLeveldBm.ToString() + " " + high.ToString() + " " + low.ToString() + " " + LimitCount.ToString());
                                i++;
                            }

                            if (GetFrameCount < LimitCount)
                            {
                                high = rfPowerLeveldBm;
                            }
                            else if (GetFrameCount > LimitCount)
                            {
                                low = rfPowerLeveldBm;
                            }
                        } while (Math.Abs(low - high) > 0.5);
                    }
                    else
                    {
                        do
                        {
                            rfPowerLeveldBm -= 0.5;
                        A:
                            //add hp samsung wifi mimo
                            setRXLevel(channelfreqMHz, rfPowerLeveldBm, IQxel, port);
                            setPhoneParam(IC, Argument, RateStr, channelfreqMHz, RatebitIndex, chain, mT6635, ref mtkParaSeted, ref phyId, wlanmode, rateBW);
                            //add hp samsung wifi mimo
                            TXDONECOUNT = setWaveform(IQxel, TXDONECOUNT, waveformName, sleepTime);
                            GetFrameCount = getFrameCount(IC, reframebyte, IQxel, waveformName, sleepTime, Argument, RateStr, chain, phyId, mT6635);

                            Log.GetInstance().i("WiFi", "发包数：1000，收包数：" + GetFrameCount.ToString() + "，仪器发射功率：" + rfPowerLeveldBm.ToString());
                            if (GetFrameCount > 1100)
                            {
                                MessageBox.Show("仪器发包数量为1000，DUT收包数量为" + GetFrameCount + "\n请检查测试环境是否异常\n确定无异常后请再点击确定或取消继续测试");
                                goto A;
                            }
                            if (testindex > 1)
                            {
                                if (GetFrameCount >= LimitCount)
                                {
                                    break;
                                }
                            }
                            if (GetFrameCount < LimitCount)
                            {
                                rfPowerLeveldBm += 0.7;
                                testindex++;
                            }
                            //else if (GetFrameCount >= LimitCount && GetFrameCount < 1050)
                            //{
                            //    rfPowerLeveldBm -= 0.7;
                            //}
                        } while (true);
                    }

                    //rfPowerLeveldBm = rfPowerLeveldBm - 0.3;
                    if (rfPowerLeveldBm > Convert.ToDouble(ResultLimitDic[Argument + "-" + RateStr + "-" + "最小接收灵敏度"]))
                    {
                        result = rfPowerLeveldBm.ToString() + ",FAIL";
                    }
                    else
                    {
                        result = rfPowerLeveldBm.ToString();
                    }

                    using (StreamWriter sw = new StreamWriter("MainTestQueue\\WiFi\\config\\SenT1Info", false, Encoding.Default))
                    {
                        sw.Write(string.Format("%IC:{0}\tArgument:{1}\tRateStr:{2}%\t{3}",IC, Argument, RateStr,result));
                    }
                    #endregion
                    break;
                case Vsg_Type.MaxInputLevel:
                case Vsg_Type.RSSI:
                    #region 最大接收电平和RSSI测试
                    int loopindex = 1;
                    List<string> powerlist = new List<string>();

                    rfPowerLeveldBm = Convert.ToDouble(ResultLimitDic[Argument + "-" + RateStr + "-" + "最小接收灵敏度"]);
                    
                    switch (testtype)
                    {
                        case Vsg_Type.MaxInputLevel:
                            rfPowerLeveldBm = Convert.ToDouble(ResultLimitDic[Argument + "-" + RateStr + "-" + "最大输入电平"]);//改 author 何苹 Samsung WiFi MIMO 
                            break;
                        case Vsg_Type.RSSI:
                            string powersstr=null;
                            using (StreamReader sr = new StreamReader("MainTestQueue\\WiFi\\RSSI_Power", Encoding.Default, true))
                            {
                                powersstr = sr.ReadLine();
                            }
                            if (powersstr.Contains(":"))
                            {
                                loopindex = powersstr.Split(':').Length;
                                powerlist = powersstr.Split(':').ToList();
                            }
                            else if (powersstr.Contains(","))
                            {

                            }
                            else if (powersstr.Contains(",") == false || powersstr.Contains(":") == false)
                            {
                                powerlist.Add(powersstr);
                            }   
                            break;
                    }
                    result = null;
                    for (int rssiloopindex = 0; rssiloopindex < loopindex; rssiloopindex++)
                    {
                        #region 设置仪器和手机参数
                        if (testtype == Vsg_Type.RSSI)
                        {
                            rfPowerLeveldBm = Convert.ToDouble(powerlist[rssiloopindex]);
                        }
                        B: if (IQxel.Equals(80)) { LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0); }
                        switch (IC)
                        {
                            case "6750"://add by wzt at 2021.04.23
                            case "6391":
                                #region 处理phyId 2.4G 与 5G问题
                                phyId = IC=="6750"?"0":"1";//modifed by wzt at 2021.04.23
                                if (Argument.Contains("5G"))
                                {
                                    phyId = "0";
                                }
                                #endregion
                                SetQRCTConfig(IC, channelfreqMHz.ToString(), "5", RatebitIndex, wlanmode, chain, "0", true, phyId, rateBW);
                                break;
                            case "3990":
                                #region 处理phyId 2.4G 与 5G问题
                                phyId = "1";
                                if (Argument.Contains("5G"))
                                {
                                    phyId = "0";
                                }
                                #endregion
                                SetQRCTConfig(IC, channelfreqMHz.ToString(), "5", RatebitIndex, wlanmode, chain, "0", true, phyId);
                                break;
                            case "3660":
                            case "3680":
                                qmslphone.FTM_WLAN_GEN6_ENABLE_CHAINS(Phone.WLAN_Gen6_PHY_ChainSelect.PHY_CHAIN_SEL_T0_R0_ON);
                                break;
                            case "MTK":
                                string bw = null;
                                if (Argument.Contains("20M") || (Argument.Contains("20M") || Argument.Contains("40M") || Argument.Contains("80M")) == false)
                                {
                                    bw = "20M";
                                }
                                else if (Argument.Contains("40M"))
                                {
                                    bw = "40M";
                                }
                                else if (Argument.Contains("80M"))
                                {
                                    bw = "80M";
                                }
                                MTK_SetParam((int)channelfreqMHz, bw);
                                MTK_SetRxMode();
                                break;
                            case "MTK New":
                                string bw_new = null;
                                if (Argument.Contains("20M") || (Argument.Contains("20M") || Argument.Contains("40M") || Argument.Contains("80M")) == false)
                                {
                                    bw_new = "20M";
                                }
                                else if (Argument.Contains("40M"))
                                {
                                    bw_new = "40M";
                                }
                                else if (Argument.Contains("80M"))
                                {
                                    bw_new = "80M";
                                }
                                MTK_SetParam((int)channelfreqMHz, bw_new);
                                MTK_SetRxMode(chain);
                                break;
                            case "MT6635X":
                                mT6635.runRxTest();
                                Thread.Sleep(1000);
                                break;
                            case "Samsung":
                               // SetSamsungATParam(Argument, (uint)Convert.ToInt32(channelandfreq.Split(';')[0].Split(',')[channelindex]), GetRate(IC, Argument, Ratestr).Split(',')[1], Convert.ToInt32(chain), enpowerstr);
                                SetSamsungATStopRx();
                                SetSamsungATStartRx();
                                break;
                        }
                        #endregion
                        #region 控制仪器发射
                        switch (IQxel)
                        {
                            case "80":
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("ROUT1;PORT:RES RF1,VSG1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WAVE:EXEC OFF"));
                                //Thread.Sleep(500);
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;FREQ:cent "+(channelfreqMHz * 1000000)));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;POW:lev "+rfPowerLeveldBm));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1; WAVE:LOAD \"/user/"+waveformName+"\""));

                                LP_EnableVsgRF(0);
                                LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                                LP_SetVsgModulation_SetPlayCondition(waveformName, false, 0);
                                LP_EnableVsgRF(1);

                                if (testtype == Vsg_Type.RSSI)
                                {
                                    LP_SetFrameCnt(100000);
                                }
                                else
                                {
                                    LP_SetFrameCnt(1000);
                                    do
                                    {
                                        TXDONECOUNT = LP_TxDone();
                                    } while (TXDONECOUNT != 0);
                                }

                                //LP_Init(1, 0);
                                //IQXEL_LP_InitTester(Marshal.StringToHGlobalAnsi("192.168.100.254"), 0);
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEM:TABLE \"1\"; MEM:TABLE:DEFINE \"FREQ,LOSS\""));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEM:TABLE \"1\";MEMory:TABLe:INSert:POINt" + " " + 2400.0000 + " " + "MHz," + 0.8));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;RFC:STAT  ON,RF1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RFC:STAT  ON,RF1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;RFC:STAT  ON,RF2"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;RFC:STAT  ON,RF2"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME" + " " + 1));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME 0.01"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MROUT1;PORT:RES RF1,VSA1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSG1;RFC:USE \"1\",RF2"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSA1;RFC:USE \"1\",RF2"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MEMory:TABLe \"1\";MEMory:TABLe:STORe"));



                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;SRAT 160000000"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;SRAT 160000000"));//add 

                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:chan:BAND" + " " + "2G4"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:CHAN:IND1" + " " + 1));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:STAN" + " " + "DSSS"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:ENP" + " " + 0));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;FREQ:cent" + " " + 2412));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                                //Thread.Sleep(2000);
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL VSA1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSA1;CAPT:TIME 0.02"));

                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;CONF:SPEC:HLIM:TYPE AUTO"));


                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("ROUT1;PORT:RES RF1,VSG1"));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WAVE:EXEC OFF"));
                                //Thread.Sleep(500);
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;FREQ:cent " + (channelfreqMHz * 1000000)));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;POW:lev " + rfPowerLeveldBm));
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1; WAVE:LOAD \"/user/" + waveformName + "\""));

                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WAVE:EXEC ON"));

                                break;
                            case "160":
                                //MVSGALL;POW:lev -30 add
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;POW:lev " + rfPowerLeveldBm + "")); //add
                                waveformName = "user/" + waveformName;      //add  
                                //MVSGALL; add
                                //WAVE:LOAD "/user/WiFi_HT20_MCS9.iqvsg"   add
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;WAVE:LOAD \"" + waveformName + "\""));
                                //WLIS: COUN 1000 add
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;WLIS:COUN 10000"));
                                //CHAN1;WIFI; HSET:ALL MVSGALL add
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI; HSET:ALL MVSGALL"));

                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;POW:lev " + rfPowerLeveldBm + "")); //add

                                ClickRXPaly(waveformName); //add
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1; WAVE:EXEC OFF;WLIST:COUNT:DISABLE WSEG1")); //add
                                Thread.Sleep(1000);
                                break;
                            case "MW":
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("ROUT1;PORT:RES RF1A,VSG1"));
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;POW:lev " + rfPowerLeveldBm + "")); //add
                                waveformName = "user/" + waveformName;      //add  
                                //MVSGALL; add
                                //WAVE:LOAD "/user/WiFi_HT20_MCS9.iqvsg"   add
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WAVE:LOAD \"" + waveformName + "\""));
                                //WLIS: COUN 1000 add
                                if (testtype == Vsg_Type.RSSI)
                                {
                                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;POW:STAT 1"));
                                }
                                else
                                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WLIS:COUN 1000"));

                                //CHAN1;WIFI; HSET:ALL MVSGALL add
                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("CHAN1;WIFI;HSET:ALL VSG1"));

                                LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;POW:lev " + rfPowerLeveldBm + "")); //add
                                //LP_EnableVsgRF(0);
                                //LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                                //LP_EnableVsgRF(1);

                                if (testtype == Vsg_Type.RSSI)
                                {
                                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WAVE:EXEC ON"));
                                }
                                else
                                {
                                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1 ;wave:exec off;WLIST:WSEG1:DATA \"" + waveformName + "\";wlist:wseg1:save;WLIST:COUNT:ENABLE WSEG1;WAVE:EXEC ON, WSEG1"));
                                }
                                
                                Log.GetInstance().d("MW", "sleep(ms)：" + sleepTime);
                                Thread.Sleep(2000);
                                //LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1 ;WAVE:EXEC OFF;WLIST:COUNT:DISABLE WSEG1")); //add
                                break;
                        }
                        #endregion
                        #region 获取测试值
                        short rssi = 0;
                        short rssi1 = 0;
                        switch (IC)
                        {
                            case "3990":
                                switch (testtype)
                                {
                                    case Vsg_Type.MaxInputLevel:
                                        #region
                                        qmslphone.FTM_WLAN_TLV2_Create(11);
                                        qmslphone.FTM_WLAN_TLV2_Complete();
                                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);

                                        if (System.Text.Encoding.ASCII.GetString(reframebyte) == "" || System.Text.Encoding.ASCII.GetString(reframebyte) == null)
                                        {
                                            GetFrameCount = 0;
                                        }
                                        else
                                        {
                                            GetFrameCount = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(reframebyte));
                                        }

                                        do
                                        {
                                            Thread.Sleep(1000);
                                            TXDONECOUNT = LP_TxDone();
                                            //Log.GetInstance().PutMessage("LP_TxDone()=" + TXDONECOUNT.ToString());
                                        } while (TXDONECOUNT != 0);

                                        qmslphone.FTM_WLAN_TLV2_Create(11);
                                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopRx"), System.Text.Encoding.Default.GetBytes("1"));
                                        qmslphone.FTM_WLAN_TLV2_Complete();
                                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);

                                        if (string.IsNullOrEmpty(System.Text.Encoding.ASCII.GetString(reframebyte)))
                                        {
                                            GetFrameCount = 0;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                GetFrameCount = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(reframebyte));
                                            }
                                            catch
                                            {
                                                GetFrameCount = 0;
                                            }
                                        }
                                        #endregion
                                        break;
                                    case Vsg_Type.RSSI:
                                        qmslphone.FTM_WLAN_TLV2_Create(11);
                                        qmslphone.FTM_WLAN_TLV2_Complete();
                                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("rssi"), reframebyte);
                                        rssi = Convert.ToInt16(System.Text.Encoding.ASCII.GetString(reframebyte));                                        
                                        break;
                                }
                                break;
                            case "6750"://add by wzt at 2021.04.23
                            case "6391":
                                switch (testtype)
                                {
                                    case Vsg_Type.RSSI:
                                        qmslphone.FTM_WLAN_TLV2_Create(118);
                                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));
                                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopRx"), System.Text.Encoding.Default.GetBytes("1"));
                                        qmslphone.FTM_WLAN_TLV2_Complete();
                                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);
                                        int goodPackets = Convert.ToInt16(System.Text.Encoding.ASCII.GetString(reframebyte));
                                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("rssi"), reframebyte);
                                        rssi = Convert.ToInt16(System.Text.Encoding.ASCII.GetString(reframebyte));
                                        Log.GetInstance().d("6391", "RSSI:" + rssi + " goodPackets:" + goodPackets);
                                        break;
                                }
                                break;
                            case "3660":
                            case "3680":
                                switch (testtype)
                                {
                                    case Vsg_Type.MaxInputLevel:
                                        //qmslphone.FTM_WLAN_GEN6_ENABLE_CHAINS(Phone.WLAN_Gen6_PHY_ChainSelect.PHY_CHAIN_SEL_T0_ON);
                                        GetFrameCount = qmslphone.FTM_WLAN_GEN6_GET_RX_PACKET_COUNTS();
                                        qmslphone.FTM_WLAN_GEN6_RESET_RX_PACKET_STATISTICS();
                                        break;
                                    case Vsg_Type.RSSI:
                                        rssi=qmslphone.FTM_WLAN_GEN6_GET_RX_RSSI_VAL()[0];
                                        break;
                                } 
                                break;
                            case "MTK":
                                switch (testtype)
                                {
                                    case Vsg_Type.MaxInputLevel:
                                        GetFrameCount = (UInt32)(MTK_RxTest());
                                        break;
                                    case Vsg_Type.RSSI:
                                        SP_META_WiFi_ReceivedRSSI_r(m_hMeta.md, 1200, ref rssi);
                                        break;
                                }
                                
                                break;
                            case "MTK New":
                                switch (testtype)
                                {
                                    case Vsg_Type.MaxInputLevel:
                                        GetFrameCount = (UInt32)(MTK_RxTest());
                                        break;
                                    case Vsg_Type.RSSI:
                                        SP_META_WiFi_ReceivedRSSI_r_New(m_hMeta_New, 1200, ref rssi);
                                        SP_META_WiFi_ReceivedRSSI1_r_New(m_hMeta_New, 1200, ref rssi1);
                                        break;
                                }
                                break;
                            case "MT6635X":
                                switch (testtype)
                                {
                                    case Vsg_Type.MaxInputLevel:
                                        GetFrameCount = mT6635.getRxInfo().AccInfo.DLL_RXOK;
                                        mT6635.DBDCStopRX();
                                        break;
                                    case Vsg_Type.RSSI:
                                        Thread.Sleep(500);
                                        ICType.MTK.MTK_Librarys.MTK_Library.WIFI_RX_STASTIC_AX rX_STASTIC_AX = mT6635.getRxInfo();
                                        //int count = 0;
                                        //while ((short)rX_STASTIC_AX.AntInfo[mT6635.nss].rssi < -100&&count++<30)
                                        //{
                                        //    Thread.Sleep(1000);
                                        //    rX_STASTIC_AX = mT6635.getRxInfo();
                                        //}
                                        List<short> rssiList = new List<short>();
                                        rssiList.Add((short)(rX_STASTIC_AX.AntInfo[mT6635.nss].rssi));
                                        for (int testCount = 0; testCount < 5; testCount++)
                                        {
                                            Thread.Sleep(1000);
                                            rX_STASTIC_AX = mT6635.getRxInfo();                                            
                                            rssiList.Add((short)(rX_STASTIC_AX.AntInfo[mT6635.swap==2? 0:mT6635.nss].rssi));
                                        }
                                        rssiList.Sort((x, y) => (Math.Abs(y+rfPowerLeveldBm)).CompareTo(Math.Abs(x+rfPowerLeveldBm)));
                                        rssi=(from n in rssiList group n by n into g orderby g.Count() descending select g).First().Key;
                                        //rssi1 = (short)(rX_STASTIC_AX.AntInfo[1].rssi);
                                        mT6635.DBDCStopRX();
                                        break;
                                }
                                break;
                            case "Samsung":
                                switch (testtype)
                                {
                                    case Vsg_Type.MaxInputLevel:
                                        GetFrameCount = (uint)GetSamsungGoodPacket();
                                        SetSamsungATStopRx();
                                        break;
                                    case Vsg_Type.RSSI:
                                        rssi = (short)GetSamsungRSSI();
                                        SetSamsungATStopRx();
                                        break;
                                }
                                break;
                        }

                        if (IQxel.Equals("MW")&& testtype== Vsg_Type.RSSI)
                            LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;WAVE:EXEC OFF")); //add
                        #endregion
                        #region 处理测试值
                        switch (testtype)
                        {
                            case Vsg_Type.MaxInputLevel:
                                //Log.GetInstance().PutMessage("发包数：1000，收包数：" + GetFrameCount.ToString() + "仪器发射功率：" + rfPowerLeveldBm.ToString());
                                if (GetFrameCount > 1050)
                                {
                                    MessageBox.Show("仪器发包数量为1000，DUT收包数量为" + GetFrameCount + "\n请检查测试环境是否异常\n确定无异常后请再点击确定或取消继续测试");
                                    goto B;
                                }
                                if (GetFrameCount > LimitCount)
                                {
                                    result = GetFrameCount.ToString();
                                }
                                else
                                {
                                    result = GetFrameCount.ToString() + ",FAIL";
                                }
                                break;
                            case Vsg_Type.RSSI:
                                if (loopindex > 1)
                                {
                                    if (result != null || result != "")
                                    {
                                        if (System.Math.Abs(Convert.ToDouble(powerlist[rssiloopindex]) - Convert.ToDouble(rssi)) > 2)
                                        {
                                            result = result + "," + powerlist[rssiloopindex] + ":" + rssi.ToString()+" "+rssi1.ToString() + ":FAIL";
                                        }
                                        else
                                        {
                                            result = result + "," + powerlist[rssiloopindex] + ":" + rssi.ToString()+" "+rssi1.ToString();
                                        }
                                    }
                                    else
                                    {
                                        result = powerlist[rssiloopindex] + ":" + rssi.ToString()+" "+rssi1.ToString()+" "+rssi1.ToString();
                                    }
                                }
                                else
                                {
                                    if (IC == "MTK New")
                                    {
                                        result = rssi.ToString() + "," + rssi1.ToString();
                                    }
                                    else if (IC == "MT6635X")
                                    {
                                        //result = powerlist[rssiloopindex] + ":rssi0 " + rssi.ToString() + "&" + "rssi1 " + rssi1.ToString();
                                        result = powerlist[rssiloopindex] + ":rssi " + rssi.ToString();
                                    }
                                    else
                                    {
                                        result = powerlist[rssiloopindex] + ":" + rssi.ToString();
                                    }
                                }
                                break;
                        }
                        #endregion
                    }

                    #endregion
                    break;
                case Vsg_Type.WaterFall:
                    #region WaterFall测试 
                    //modified by lxy 2021.6.10
                    uint ToltalPackets = 0;
                    uint GoodPackects = 0;
                    uint ErrorPackects = 0;
                    string PERValue = null;                 
                    string waterFallValue = null;                        
                    //仪器参数设置
                    rfPowerLeveldBm = -9;
                    LP_EnableVsgRF(0);//0：OFF 1：RF1 2：RF2      
                    //自动切换上下天线时，仪器VSG口设置为RF2口
                    if (Symbol==1&&chain=="2")
                    {
                        LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, 3, true, 0.0);
                        LP_EnableVsgRF(2);
                    }
                    else
                    {                          
                       LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                        LP_EnableVsgRF(1);//RF1  
                    }                    
                    LP_SetVsgModulation_SetPlayCondition(waveformName, false, 0);
                  do
                    {
                        rfPowerLeveldBm -= 1;
                        A: LP_EnableVsgRF(0);//OFF                   
                        if (Symbol == 1 && chain == "2")
                        {                           
                           LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, 3, true, 0.0);
                           LP_EnableVsgRF(2);                           
                        }
                        else
                        {
                            LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                            LP_EnableVsgRF(1);//RF1                                 
                        }
                    //手机参数设置
                    switch (IC)
                        {
                            case "3990":
                                SetQRCTConfig(IC, channelfreqMHz.ToString(), "5", RatebitIndex, wlanmode, chain, "0", true);
                                break;
                            case "3660":
                            case "3680":
                                qmslphone.FTM_WLAN_GEN6_ENABLE_CHAINS(Phone.WLAN_Gen6_PHY_ChainSelect.PHY_CHAIN_SEL_T0_R0_ON);
                                break;
                            case "6391":
                            case "6750":
                                #region 处理phyId 2.4G 与 5G问题
                                phyId = "1";
                                if (Argument.Contains("5G"))
                                {
                                    phyId = "0";
                                }
                                #endregion
                                SetQRCTConfig(IC, channelfreqMHz.ToString(), "5", RatebitIndex, wlanmode, chain, "0", true, phyId, rateBW);
                                break;
                            case "MTK":
                                string bw = null;
                                if (Argument.Contains("20M") || (Argument.Contains("20M") || Argument.Contains("40M") || Argument.Contains("80M")) == false)
                                {
                                    bw = "20M";
                                }
                                else if (Argument.Contains("40M"))
                                {
                                    bw = "40M";
                                }
                                else if (Argument.Contains("80M"))
                                {
                                    bw = "80M";
                                }
                                MTK_SetParam((int)channelfreqMHz, bw);
                                MTK_SetRxMode(chain);
                                mtkParaSeted = true;
                                break;
                            case "MTK New":
                                string bw_new = null;
                                if (Argument.Contains("20M") || (Argument.Contains("20M") || Argument.Contains("40M") || Argument.Contains("80M")) == false)
                                {
                                    bw_new = "20M";
                                }
                                else if (Argument.Contains("40M"))
                                {
                                    bw_new = "40M";
                                }
                                else if (Argument.Contains("80M"))
                                {
                                    bw_new = "80M";
                                }
                                MTK_SetParam((int)channelfreqMHz, bw_new, Argument);
                                MTK_SetRxMode(chain);
                                mtkParaSeted = true;
                                break;
                            case "MT6635X":
                                mT6635 = new MT6635();
                                mT6635.runRxTest();
                                Thread.Sleep(1000);
                                break;
                            case "AT":
                                SetATRxParam(Argument, channelfreqMHz.ToString(), RateStr);
                                break;
                            case "Samsung":
                                SetSamsungATParam(Argument, (uint)Channel, RateStr, Convert.ToInt32(chain));
                                SetSamsungATStartRx();
                                break;
                        } 
                        LP_SetFrameCnt(500);//仪器发包数 修改 lxy                       
                        Thread.Sleep(sleepTime);
                        //获取测试结果
                        switch (IC)
                        {
                            case "3990":
                                #region 3990   
                                try
                                {
                                    qmslphone.FTM_WLAN_TLV2_Create(118);
                                    qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));
                                    qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopRx"), System.Text.Encoding.Default.GetBytes("1"));
                                    qmslphone.FTM_WLAN_TLV2_Complete();
                                    qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("totalPkt"), reframebyte);//add by lxy 2021.5.19 增加错误包的百分比，需要得出总的收包数
                                    ToltalPackets = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(reframebyte));
                                    qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);
                                    //GoodPackects = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(reframebyte));
                                    //qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);

                                    if (string.IsNullOrEmpty(System.Text.Encoding.ASCII.GetString(reframebyte)))
                                    {
                                        GetFrameCount = 0;
                                    }
                                    else
                                    {
                                        string data = null;
                                        try
                                        {
                                            data = System.Text.Encoding.ASCII.GetString(reframebyte);
                                            data = new string((from c in data.ToCharArray() where !char.IsControl(c) select c).ToArray());
                                        }
                                        catch (PhoneException ex)
                                        {
                                            GoodPackects = 0;

                                        }
                                        GoodPackects = Convert.ToUInt32(data);
                                        double Per6391 = (Convert.ToDouble(ToltalPackets) - Convert.ToDouble(GoodPackects)) / Convert.ToDouble(ToltalPackets) * 100;
                                        PERValue = Per6391.ToString("f2");
                                    }
                                }
                                catch (PhoneException ex)
                                {
                                    Log.GetInstance().d("手机参数设置异常", "正在重新设置");
                                    ConnectPhone(Phoneport, IC);
                                    PhoneIni(IC);
                                    goto A;
                                }
                                #endregion
                                break;
                            case "3660":
                            case "3680":
                                GetFrameCount = qmslphone.FTM_WLAN_GEN6_GET_RX_PACKET_COUNTS();
                                qmslphone.FTM_WLAN_GEN6_RESET_RX_PACKET_STATISTICS();
                                break;
                            case "6391":
                            case "6750":
                                try
                                   {
                                        phyId = "1";
                                        if (Argument.Contains("5G"))
                                        {
                                            phyId = "0";
                                        }
                                        qmslphone.FTM_WLAN_TLV2_Create(118);
                                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));
                                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopRx"), System.Text.Encoding.Default.GetBytes("1"));
                                        qmslphone.FTM_WLAN_TLV2_Complete();
                                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("totalPkt"), reframebyte);//add by lxy 2021.5.19 增加错误包的百分比，需要得出总的收包数
                                        ToltalPackets = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(reframebyte));
                                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);
                                        if (string.IsNullOrEmpty(System.Text.Encoding.ASCII.GetString(reframebyte)))
                                        {
                                            GoodPackects = 0;
                                        }
                                        else
                                        {
                                            string data = null;
                                            try
                                            {
                                                data = System.Text.Encoding.ASCII.GetString(reframebyte);
                                                data = new string((from c in data.ToCharArray() where !char.IsControl(c) select c).ToArray());
                                            }
                                            catch (PhoneException ex)
                                            {
                                                GoodPackects = 0;                                           
                                            }
                                            GoodPackects = Convert.ToUInt32(data);
                                            double Per6391 = (Convert.ToDouble(ToltalPackets) - Convert.ToDouble(GoodPackects)) / Convert.ToDouble(ToltalPackets) * 100;
                                            PERValue = Per6391.ToString("f2");
                                        }
                                    }
                                    catch(PhoneException ex)
                                    {
                                        Log.GetInstance().d("接收参数设置异常", "正在重新设置");
                                        ConnectPhone(Phoneport, IC);
                                        PhoneIni(IC);
                                        goto A;
                                    }                           
                                break;
                            case "MTK":
                                GoodPackects = Convert.ToUInt32(MTK_RxTest_New().Split(';')[0]);//add by lxy 2021.5.21
                                ErrorPackects = Convert.ToUInt32(MTK_RxTest_New().Split(';')[1]);
                                ToltalPackets = GoodPackects + ErrorPackects;
                                double PerMTK = (Convert.ToDouble(ToltalPackets) - Convert.ToDouble(GetFrameCount) / Convert.ToDouble(ToltalPackets)) * 100;
                                PERValue = PerMTK.ToString("f2");
                                break;
                            case "MTK New":
                                GoodPackects = Convert.ToUInt32(MTK_RxTest_New().Split(';')[0]);//add by lxy 2021.5.21
                                ErrorPackects = Convert.ToUInt32(MTK_RxTest_New().Split(';')[1]);
                                ToltalPackets = GoodPackects + ErrorPackects;
                                double PerMTKNew = (Convert.ToDouble(ToltalPackets) - Convert.ToDouble(GetFrameCount) / Convert.ToDouble(ToltalPackets)) * 100;
                                PERValue = PerMTKNew.ToString("f2");
                                break;
                            case "MT6635X":
                                ICType.MTK.MTK_Librarys.MTK_Library.WIFI_RX_STASTIC_AX rX_STASTIC_AX = mT6635.getRxInfo();
                                GoodPackects =Convert.ToUInt32( rX_STASTIC_AX.AccInfo.DLL_RXOK);
                                if (GoodPackects > 1000000)
                                {
                                    GoodPackects = 0;
                                }
                                mT6635.okData = Convert.ToUInt32(GoodPackects);
                                float Per = rX_STASTIC_AX.AccInfo.DLL_PER*100;//转为百分比
                                PERValue = Per.ToString("f2");
                                ErrorPackects = rX_STASTIC_AX.AccInfo.DLL_AllFCSErr;
                                ToltalPackets = GoodPackects+ErrorPackects;
                                mT6635.DBDCStopRX();
                                break;
                            case "AT":
                                GetFrameCount = Convert.ToUInt32(GetATRxFrameCount());
                                break;
                            case "Samsung":
                                SetSamsungATStopRx();
                                GoodPackects = (uint)GetSamsungGoodPacket();
                                ErrorPackects = (uint)GetSamsungErrorPacket();
                                if (GoodPackects == 0)
                                {
                                    Log.GetInstance().d("收到包数为0", "正在重新测试");
                                    SetSamsungATParam(Argument, (uint)Channel, RateStr, Convert.ToInt32(chain));
                                    SetSamsungATStartRx();
                                    LP_SetFrameCnt(500);
                                    Thread.Sleep(sleepTime);
                                    SetSamsungATStopRx();
                                    GoodPackects = (uint)GetSamsungGoodPacket();
                                    ErrorPackects = (uint)GetSamsungErrorPacket();
                                }                                                          
                                 ToltalPackets = GoodPackects + ErrorPackects;
                                double perSamsung = (Convert.ToDouble(ErrorPackects)) / Convert.ToDouble(ToltalPackets) * 100;
                                PERValue = perSamsung.ToString("f2");
                                break;
                        }
                        if (waterFallValue != null)
                        {
                            waterFallValue = waterFallValue + ";" + rfPowerLeveldBm.ToString() + ":" + GoodPackects.ToString() + ":" + PERValue;
                        }
                        else
                        {
                            waterFallValue = rfPowerLeveldBm.ToString() + ":" + GoodPackects.ToString() + ":" + PERValue;//add by lxy 2021.5.19                                                    
                        }
                        Log.GetInstance().d("WiFi", "发包数：500，收包总数："+ToltalPackets.ToString()+"收到好包个数：" + GoodPackects.ToString() + "，仪器发射功率：" + rfPowerLeveldBm.ToString() + " 错误包百分比：" + PERValue);
                        if (GoodPackects > 550)
                        {
                            Log.GetInstance().d("WaterFall", "仪器发包数量为500，DUT收包数量为" + GoodPackects.ToString() + "\n请检查测试环境是否异常\n确定无异常后请再点击确定或取消继续测试");
                            Thread.Sleep(100);
                           // MessageBox.Show("仪器发包数量为500，DUT收包数量为" + GoodPackects.ToString() + "\n请检查测试环境是否异常\n确定无异常后请再点击确定或取消继续测试");
                            goto A;
                        }
                        if (rfPowerLeveldBm <= -100)//发射功率每个信道每个速率都从-10到-100
                        {
                            break;
                        }
                    } while (true);
                    result = waterFallValue;
                    #endregion
                    break;
            }
            #endregion
            return result;
          
        }

        private uint getFrameCount(string IC, byte[] reframebyte, string IQxel, string waveformName, int sleepTime, string Argument, string RateStr, string chain, string phyId = "", MT6635 mT6635 = null)
        {
            uint GetFrameCount = 0;
            int TXDONECOUNT = 999;
            switch (IC)
            {
                //获取接收包数
                case "6391":
                case "6750"://modified by wzt at 2021.04.23
                //6391 获取包
                PhoneE:
                    try
                    {
                        qmslphone.FTM_WLAN_TLV2_Create(118);
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("phyId"), System.Text.Encoding.Default.GetBytes(phyId));
                        qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopRx"), System.Text.Encoding.Default.GetBytes("1"));
                        qmslphone.FTM_WLAN_TLV2_Complete();
                        qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);
                    }
                    catch (PhoneException pe)
                    {
                        Log.GetInstance().d("PhoneException", pe.Message + "手机参数异常重新设置中...");
                        ConnectPhone(Phoneport, "6391");
                        PhoneIni("6391");
                        goto PhoneE;
                    }

                    if (string.IsNullOrEmpty(System.Text.Encoding.ASCII.GetString(reframebyte)))
                    {
                        GetFrameCount = 0;
                    }
                    else
                    {
                        try
                        {
                            GetFrameCount = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(reframebyte));
                        }
                        catch (Exception e)
                        {
                            GetFrameCount = 0;
                        }
                    }
                    break;
                case "3990":
                    #region 3990
                    qmslphone.FTM_WLAN_TLV2_Create(11);
                    qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopRx"), System.Text.Encoding.Default.GetBytes("1"));
                    qmslphone.FTM_WLAN_TLV2_Complete();
                    qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);

                    //SetQRCTConfig(IC, channelfreqMHz.ToString(), "5", RatebitIndex, wlanmode, chain, "0", true);
                    //LP_SetFrameCnt(1000);
                    //do
                    //{
                    //    Thread.Sleep(1000);
                    //    TXDONECOUNT = LP_TxDone();
                    //    Log.GetInstance().PutMessage("LP_TxDone()=" + TXDONECOUNT.ToString());
                    //} while (TXDONECOUNT != 0);
                    //qmslphone.FTM_WLAN_TLV2_Create(11);
                    //qmslphone.FTM_WLAN_TLV2_AddParam(System.Text.Encoding.Default.GetBytes("stopRx"), System.Text.Encoding.Default.GetBytes("1"));
                    //qmslphone.FTM_WLAN_TLV2_Complete();
                    //qmslphone.FTM_WLAN_TLV2_GetRspParam(System.Text.Encoding.ASCII.GetBytes("goodPackets"), reframebyte);

                    if (string.IsNullOrEmpty(System.Text.Encoding.ASCII.GetString(reframebyte)))
                    {
                        GetFrameCount = 0;
                    }
                    else
                    {
                        try
                        {
                            GetFrameCount = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(reframebyte));
                        }
                        catch
                        {
                            GetFrameCount = 0;
                        }
                    }
                    #endregion
                    break;
                case "3660":
                case "3680":
                    GetFrameCount = qmslphone.FTM_WLAN_GEN6_GET_RX_PACKET_COUNTS();
                    qmslphone.FTM_WLAN_GEN6_RESET_RX_PACKET_STATISTICS();
                    break;
                case "MTK":
                    GetFrameCount = (UInt32)(MTK_RxTest());
                    break;
                case "MTK New":
                    GetFrameCount = (UInt32)(MTK_RxTest());
                    break;
                case "MT6635X":
                    GetFrameCount = mT6635.getRxInfo().AccInfo.DLL_RXOK;
                    if (GetFrameCount > 1000000)
                    {
                        GetFrameCount = 0;
                    }
                    mT6635.okData = GetFrameCount;
                    mT6635.DBDCStopRX();
                    break;
                case "AT":
                    GetFrameCount = Convert.ToUInt32(GetATRxFrameCount());
                    break;
                case "Samsung":
                    SetSamsungATStopRx();
                    GetFrameCount = (uint)GetSamsungGoodPacket();
                    if (GetFrameCount == 0)
                    {
                        SetSamsungATParam(Argument, (uint)Channel, RateStr, Convert.ToInt32(chain));
                        SetSamsungATStartRx();

                        setWaveform(IQxel, TXDONECOUNT, waveformName, sleepTime);

                        SetSamsungATStopRx();
                        GetFrameCount = (uint)GetSamsungGoodPacket();
                    }
                    break;
                default:
                    break;
            }
            return GetFrameCount;
        }

        private int setWaveform(string IQxel, int TXDONECOUNT, string waveformName, int sleepTime)
        {
            switch (IQxel)
            {
                case "80":
                    LP_SetFrameCnt(1000);
                    do
                    {
                        Thread.Sleep(1000);
                        TXDONECOUNT = LP_TxDone();
                        //Log.GetInstance().PutMessage("LP_TxDone()=" + TXDONECOUNT.ToString());
                    } while (TXDONECOUNT != 0);
                    break;
                case "160":
                    //api使用失败 使用命令方式
                    ClickRXPaly(waveformName); //add
                    Log.GetInstance().d("160", "sleep(ms)：" + sleepTime);
                    Thread.Sleep(sleepTime);
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1; WAVE:EXEC OFF;WLIST:COUNT:DISABLE WSEG1")); //add
                    break;
                case "MW":
                    //LP_SetFrameCnt(1000);
                    //do
                    //{
                    //    Thread.Sleep(1000);
                    //    TXDONECOUNT = LP_TxDone();
                    //    Log.GetInstance().PutMessage("LP_TxDone()=" + TXDONECOUNT.ToString());
                    //} while (TXDONECOUNT != 0);
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1 ;wave:exec off;WLIST:WSEG1:DATA \"" + waveformName + "\";wlist:wseg1:save;WLIST:COUNT:ENABLE WSEG1;WAVE:EXEC ON, WSEG1"));
                    Log.GetInstance().d("MW", "sleep(ms)：" + sleepTime);
                    Thread.Sleep(sleepTime);
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1 ;WAVE:EXEC OFF;WLIST:COUNT:DISABLE WSEG1")); //add
                    break;
            }
            return TXDONECOUNT;
        }

        private void setPhoneParam(string IC, string Argument, string RateStr, double channelfreqMHz, string RatebitIndex, string chain, MT6635 mT6635, ref bool mtkParaSeted, ref string phyId, string wlanmode, string rateBW)
        {
            switch (IC)
            {
                //设置手机参数
                case "6750"://modified by wzt at 2021.04.23
                case "6391":
                    #region 处理phyId 2.4G 与 5G问题
                    phyId = IC=="6750"?"0":"1";
                    if (Argument.Contains("5G"))
                    {
                        phyId = "0";
                    }
                    #endregion
                    SetQRCTConfig(IC, channelfreqMHz.ToString(), "5", RatebitIndex, wlanmode, chain, "0", true, phyId, rateBW);
                    break;
                case "3990":
                    SetQRCTConfig(IC, channelfreqMHz.ToString(), "5", RatebitIndex, wlanmode, chain, "0", true);
                    break;
                case "3660":
                case "3680":
                    qmslphone.FTM_WLAN_GEN6_ENABLE_CHAINS(Phone.WLAN_Gen6_PHY_ChainSelect.PHY_CHAIN_SEL_T0_R0_ON);
                    break;
                case "MTK":
                    string bw = null;
                    if (Argument.Contains("20M") || (Argument.Contains("20M") || Argument.Contains("40M") || Argument.Contains("80M")) == false)
                    {
                        bw = "20M";
                    }
                    else if (Argument.Contains("40M"))
                    {
                        bw = "40M";
                    }
                    else if (Argument.Contains("80M"))
                    {
                        bw = "80M";
                    }
                    MTK_SetParam((int)channelfreqMHz, bw);
                    MTK_SetRxMode(chain);
                    mtkParaSeted = true;
                    break;
                case "MTK New":
                    string bw_new = null;
                    if (Argument.Contains("20M") || (Argument.Contains("20M") || Argument.Contains("40M") || Argument.Contains("80M")) == false)
                    {
                        bw_new = "20M";
                    }
                    else if (Argument.Contains("40M"))
                    {
                        bw_new = "40M";
                    }
                    else if (Argument.Contains("80M"))
                    {
                        bw_new = "80M";
                    }
                    MTK_SetParam((int)channelfreqMHz, bw_new, Argument);
                    MTK_SetRxMode(chain);
                    mtkParaSeted = true;
                    break;
                case "MT6635X":
                    mT6635.runRxTest();
                    break;
                case "AT":
                    SetATRxParam(Argument, channelfreqMHz.ToString(), RateStr);
                    break;
                case "Samsung":
                    SetSamsungATParam(Argument, (uint)Channel, RateStr, Convert.ToInt32(chain));
                    SetSamsungATStopRx();
                    SetSamsungATStartRx();
                    break;
            }
        }

        private static void setRXLevel(double channelfreqMHz, double rfPowerLeveldBm, string IQxel, int port)
        {
            switch (IQxel)
            {
                case "80":
                    LP_EnableVsgRF(0);
                    LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                    LP_EnableVsgRF(1);
                    break;
                case "160":
                    //MVSGALL;POW:lev -30 add
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("MVSGALL;POW:lev " + rfPowerLeveldBm + "")); //add
                    break;
                case "MW":
                    LP_ScpiCommandSet(Marshal.StringToHGlobalAnsi("VSG1;POW:lev " + rfPowerLeveldBm + "")); //add
                                                                                                            //LP_EnableVsgRF(0);
                                                                                                            //LP_SetVsg(channelfreqMHz * 1000000, rfPowerLeveldBm, port, true, 0.0);
                                                                                                            //LP_EnableVsgRF(1);
                    break;
            }
        }

        /// <summary>
        /// IQxel仪器发送数据包
        /// </summary>
        /// <param name="count">数据包数量</param>
        public void SetFrameCount(int count)
        {
            LP_SetFrameCnt(count);
        }

        /// <summary>
        /// 初始化WiFi芯片
        /// </summary>
        /// <param name="IC">WiFi芯片</param>
        public void IniIC(string IC)
        {
            switch (IC)
            {
                case "3660":
                    qmslphone.FTM_WLAN_GEN6_START(3660);
                    break;
                case "3680":
                    qmslphone.FTM_WLAN_GEN6_START(3680);
                    break;
                case "6174":
                    qmslphone.FTM_WLAN_Atheros_LoadDUT
                        (
                        System.Text.Encoding.Default.GetBytes("qc6174"),
                        System.Text.Encoding.Default.GetBytes(System.Windows.Forms.Application.StartupPath + "\\boardData\\bdwlan30.bin"),
                        5,
                        62);
                    break;
                case "3980":
                    //需要加
                    break;
                case "3990":
                    qmslphone.FTM_WLAN_Atheros_LoadDUT(System.Text.Encoding.Default.GetBytes("qc6180"), System.Text.Encoding.Default.GetBytes(""), 5, 64);
                    break;
                case "MTK":
                    break;
            }
        }



        #region MTK代码
        
        #region MTK old
        const int MAX_PATH = 260;
        const int STOP_VAL = 9876;
        const string SPBROM_DLL_PATH = @"lib\MTK\SPBootMode.dll";
        const string SPMETA_DLL_PATH = @"lib\MTK\SPMETA_DLL.dll";
        const string MDMETA_DLL_PATH = @"lib\MTK\META_DLL.dll";
        const string WRAPPER_DLL_PATH = @"lib\MTK\SP_META_Wrapper.dll";

        ////////////////////////////////////////////////////////////////
        enum WRAPPER_BOOT_TYPE
        {
            BROM = 0,
            PRELODER,
            UNKNOWN
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct WRAPPER_BOOT_STTING_T
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool BypassBootPreloader;    // if bypass borom/preloader handshake
            [MarshalAs(UnmanagedType.U1)]
            public bool mdLogging;      // if enable modem log after modem bootup
            [MarshalAs(UnmanagedType.U1)]
            public bool uartLogDisable; // if disable uart log during boot procedure
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr authHandle;   // void *, for security feature
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr certHandle;   // void *, for security feature
            [MarshalAs(UnmanagedType.U1)]
            public bool useUsb;         // if connect by usb cable
            [MarshalAs(UnmanagedType.U1)]
            public bool enAdb;          // enable adb interface after kernel boot up
            public uint comPort;        // brom or preloader com port for handshake, if specify BypassFindPort ignore this value.
            public WRAPPER_BOOT_TYPE bootType;  // handshake by brom or preloader,  if specify BypassFindPort true, this value ignored.
            public ushort mdMode;       // specify the init modem type. 0 means use default type. For mt6763/6739/6771 etc, set it to 0.
        };

        enum WRAPPER_CONNECT_TYPE_E
        {
            USB = 0,
            WIFI,
            NUM
        };

        enum WRAPPER_CONNECT_MODE_E
        {
            META = 0,
            ATM,
            NUM
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct WRAPPER_CONNECT_AP_SETTING_T
        {
            public WRAPPER_CONNECT_TYPE_E connType;  // specify connect method between, by usb calbe or wifi
            public WRAPPER_CONNECT_MODE_E connMode;  // ATM(Android Test Mode) is in Normal mode but also enable meta service, should do more setting automatically.
            public int comPort;             // specify kernel port, if specify BypassFindPort ignore this value.
            [MarshalAs(UnmanagedType.LPStr)]
            public string pApDbFilePath;    // char *, specify ap nvram db file path, "" means get the file from device.
            [MarshalAs(UnmanagedType.LPStr)]
            public string pMdDbFilePath;    // char *, specify modem nvram db file path, "" means get the file from device.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string metaLogDir;       // char [MAX_PATH], spbootmode.dll/meta.dll/spmeta_dll.dll log directory
        };

        enum SP_FILTER_TYPE_E
        {
            WHITE_LIST = 0,
            BLACK_LIST,
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SP_COM_FILTER_LIST_S
        {
            public uint count;          // filter string number
            public SP_FILTER_TYPE_E type;  // white list means only the VID/PID in the list can be used.
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr list;         // char **, filter string list
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr iface;    // bool *, out value, can always set to false
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct WRAPPER_FILTER_SETTING_T
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.SysUInt)]
            public IntPtr[] pFilter; // SP_COM_FILTER_LIST_S *, 1st [0] is brom/preloader 
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct WRAPPER_CONNCT_SETTING_T
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool BypassFindPort; // if get brom/preloder/kernel com port automatically
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr stopFlag;     // int *, value STOP_VAL means stop and return some long time span api.
            public uint m_uTimeOutMs;   // connect timeout
            [MarshalAs(UnmanagedType.U1)]
            public bool enableLog;      // if enable meta dll log

            public WRAPPER_BOOT_STTING_T bootCfg;
            public WRAPPER_CONNECT_AP_SETTING_T connCfg;
            public WRAPPER_FILTER_SETTING_T filterCfg;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct WRAPPER_HANDLE_T
        {
            public int md;  // modem meta handle
            public int ap;  // ap meta handle
        };

        public enum WRAPPER_TEST_RAT_E
        {
            T_GSM = 0,
            T_TDS,
            T_WCDMA,
            T_LTE,
            T_C2K,
            T_EXIT_C2K
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct WRAPPER_CONNCT_SETTING_WO_BOOT_T
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool BypassFindPort; // if get brom/preloder/kernel com port automatically
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr stopFlag;     // int *, value STOP_VAL means stop and return some long time span api.
            public uint m_uTimeOutMs;   // connect timeout
            [MarshalAs(UnmanagedType.U1)]
            public bool enableLog;      // if enable meta dll log

            public WRAPPER_CONNECT_AP_SETTING_T m_sConnectSetting;
            public WRAPPER_FILTER_SETTING_T m_sFilterSetting;
        };

        /// <summary>
        /// Wrapper api for enter meta mode expediently.
        /// </summary>
        /// <param name="nDutId">the device id(from 0), for multi-device scene.</param>
        /// <param name="pSetting"></param>
        /// <param name="pCnf">ap meta and modem meta handle.</param>
        /// <returns>0: suncess; others: fail.</returns>
        [DllImport(WRAPPER_DLL_PATH, EntryPoint = "SP_META_Wrapper_ConnectTargetAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int Wrapper_ConnectTarget(int nDutId, ref WRAPPER_CONNCT_SETTING_T pSetting, ref WRAPPER_HANDLE_T pCnf);

        /// <summary>
        /// Wrapper api for enter meta mode expediently.
        /// The device had in meta mode.
        /// </summary>
        /// <param name="nDutId"></param>
        /// <param name="pSetting"></param>
        /// <param name="pCnf"></param>
        /// <returns></returns>
        [DllImport(WRAPPER_DLL_PATH, EntryPoint = "SP_META_Wrapper_ConnectAPAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int Wrapper_ConnectReadyTarget(int nDutId, ref WRAPPER_CONNCT_SETTING_WO_BOOT_T pSetting, ref WRAPPER_HANDLE_T pCnf);

        [DllImport(WRAPPER_DLL_PATH, EntryPoint = "SP_META_Wrapper_ApToModemAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int Wrapper_ApToModem(int nDutId);

        [DllImport(WRAPPER_DLL_PATH, EntryPoint = "SP_META_Wrapper_ModemToApAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int Wrapper_ModemToAp(int nDutId);

        [DllImport(WRAPPER_DLL_PATH, EntryPoint = "SP_META_Wrapper_SwitchTestRat_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int Wrapper_SwitchTestRat(int nDutId, WRAPPER_TEST_RAT_E rat);


        ////////////////////////////////////////////////////////////////

      

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct VerInfo_V2_Cnf
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BB_CHIP;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string ECO_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string SW_TIME;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string DSP_FW;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string DSP_PATCH;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string SW_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string HW_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string MELODY_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BUILD_DISP_ID;
            public byte status;
        };
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct SetCleanBootFlag_REQ
        {
            public int Notused;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BackupTime;   // unsigned char [64]
        };

        [StructLayout(LayoutKind.Sequential)]
        struct SetCleanBootFlag_CNF
        {
            int drv_status;
        };
        

        ////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get META_DLL.dll api result infomation string.
        /// </summary>
        /// <param name="ErrCode">the return value by other META_xxx apis.</param>
        /// <returns>infromation string</returns>
        [DllImport(MDMETA_DLL_PATH, EntryPoint = "META_GetErrorString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static string META_GetErrorString(int ErrCode);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct VerInfo_Cnf
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BB_CHIP;    /**< BaseBand chip version */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string ECO_VER;     /**< ECO version */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string DSP_FW;     /**< DSP firmware version */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string DSP_PATCH;  /**< DSP patch version */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string SW_VER;     /**< S/W version */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string HW_VER;     /**< H/W board version */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string MELODY_VER; /**< Melody version */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string SW_FLAVOR;  /**< Build flavor information*/
        };

        [DllImport(MDMETA_DLL_PATH, EntryPoint = "META_GetTargetVerInfoEx2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int META_GetTargetVerInfoEx2_r(int mdhandle, int ms_timeout, ref VerInfo_Cnf cnf);



        UInt32 m_wifiChipVersion = 0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NVRAM_ACCESS_STRUCT
        {
            public UInt32 dataLen;
            public UInt32 dataOffset; /*set as zero for whole region */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string data;
        }

        enum ENUM_CFG_SRC_TYPE_T
        {
            CFG_SRC_TYPE_EEPROM,    //cfg data is queried/set from/to EEPROM
            CFG_SRC_TYPE_NVRAM,     //cfg data is queried/set from/to NVRAM
            CFG_SRC_TYPE_BOTH,      //cfg data is queried/set from/to NVRAM, and E2PROM presents too
            CFG_SRC_TYPE_AUTO
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct WIFI_TX_PARAM_T
        {
            public UInt32 bufSize;
            public UInt32 bLongPreamble;
            public UInt32 txRate;
            public UInt32 pktCount;
            public UInt32 pktInterval;
            public UInt32 bGainControl;
            public UInt32 gainControl;
            public UInt32 bTrackAlc;
            public UInt32 bTargetAlc;
            public UInt32 targetAlcValue;
            public UInt32 txAntenna;

        }

        /// <summary>
        /// Get META_DLL.dll api result infomation string.
        /// </summary>
        /// <param name="ErrCode">the return value by other META_xxx apis.</param>
        /// <returns>infromation string</returns>
        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_GetErrorString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static string SP_META_GetErrorString(int ErrCode);

        /// <summary>
        /// Disconnect from ap meta stage, and shutdown device.
        /// </summary>
        /// <param name="aphandle"></param>
        /// <returns></returns>
        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_DisconnectWithTarget_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_DisconnectWithTarget_r(int aphandle);

        /// <summary>
        /// Disconnect from ap meta stage ( kip device in meta mode.)
        /// </summary>
        /// <param name="aphandle"></param>
        /// <returns></returns>
        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_DisconnectInMetaMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_DisconnectInMetaMode_r(int aphandle);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_GetTargetVerInfoV2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_GetTargetVerInfoV2_r(int aphandle, ref VerInfo_V2_Cnf cnf, ref short token, IntPtr usrData);

        /// <summary>
        /// Backup all nvram content from nvdata partition to nvram(binregion) partition.
        /// This api only can be called in ap meta stage.
        /// </summary>
        /// <param name="aphandle"></param>
        /// <param name="ms_timeout"></param>
        /// <param name="req"></param>
        /// <param name="cnf"></param>
        /// <returns></returns>
        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_SetCleanBootFlag_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_SetCleanBootFlag_r(int aphandle, uint ms_timeout, ref SetCleanBootFlag_REQ req, ref SetCleanBootFlag_CNF cnf);


        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_GetChipVersion_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_GetChipVersion_r(int mdhandle, int ms_timeout, ref UInt32 value);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_readMCR32_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_readMCR32_r(int mdhandle, UInt32 ms_timeout, UInt32 offset, ref UInt32 value);

        

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_WriteNVRAM_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_WriteNVRAM_r(int mdhandle, UInt32 ms_timeout, ref NVRAM_ACCESS_STRUCT preq);

      
        

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_QueryConfig_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_QueryConfig_r(int mdhandle, UInt32 ms_timeout, ref ENUM_CFG_SRC_TYPE_T buffType);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setTestMode_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_switchAntenna_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_switchAntenna_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WIFI_OPEN_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WIFI_OPEN_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setChannel_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setChannel_r(int mdhandle, UInt32 ms_timeout, int channelfreq);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_META_WiFi_setRate_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_META_WiFi_setRate_r(int mdhandle, UInt32 ms_timeout, UInt32 Rate);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setBandwidthEx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setBandwidthEx_r(int mdhandle, UInt32 ms_timeout, UInt32 nChBandwidth, UInt32 nDataBandwidth, UInt32 nPrimarySetting);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setGuardinterval_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setGuardinterval_r(int mdhandle, UInt32 ms_timeout, UInt32 inerval);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setBandwidth_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setBandwidth_r(int mdhandle, UInt32 ms_timeout, UInt32 BandWidth);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setModeSelect_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setModeSelect_r(int mdhandle, UInt32 ms_timeout, UInt32 Mode);

       

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setPacketTxEx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setPacketTxEx_r(int mdhandle, UInt32 ms_timeout, ref WIFI_TX_PARAM_T txparam);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setStandBy_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setStandBy_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_setRxTest_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setRxTest_r(int mdhandle, UInt32 ms_timeout);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_ReceivedErrorCount_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_ReceivedErrorCount_r(int mdhandle, UInt32 ms_timeout,ref UInt32 value);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_ReceivedOKCount_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_ReceivedOKCount_r(int mdhandle, UInt32 ms_timeout, ref UInt32 value);

        [DllImport(SPMETA_DLL_PATH, EntryPoint = "SP_META_WiFi_ReceivedRSSI_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_ReceivedRSSI_r(int mdhandle, UInt32 ms_timeout, ref short value);



        int m_nDutID;
        WRAPPER_HANDLE_T m_hMeta;
        int m_nStopFlag;
        
        public bool Connect()
        {
            this.m_nDutID = 0;
            this.m_hMeta.ap = 0;
            this.m_hMeta.md = 0;
            this.m_nStopFlag = 0;

            int result;
            WRAPPER_CONNCT_SETTING_T req;

            m_nDutID = 0;

            req = new WRAPPER_CONNCT_SETTING_T();
            req.BypassFindPort = false;
            req.stopFlag = Marshal.AllocHGlobal(Marshal.SizeOf(m_nStopFlag));
            Marshal.StructureToPtr(m_nStopFlag, req.stopFlag, true);
            req.m_uTimeOutMs = 20000;
            req.enableLog = true;
            req.bootCfg.BypassBootPreloader = false;
            req.bootCfg.mdLogging = false;
            req.bootCfg.uartLogDisable = true;
            req.bootCfg.authHandle = IntPtr.Zero;
            req.bootCfg.certHandle = IntPtr.Zero;
            req.bootCfg.useUsb = true;
            req.bootCfg.enAdb = false;
            req.bootCfg.comPort = 0;
            req.bootCfg.bootType = WRAPPER_BOOT_TYPE.BROM;
            req.bootCfg.mdMode = 0;
            req.connCfg.connType = WRAPPER_CONNECT_TYPE_E.USB;
            req.connCfg.connMode = WRAPPER_CONNECT_MODE_E.META;
            req.connCfg.comPort = 0;
            req.connCfg.pApDbFilePath = @"";
            req.connCfg.pMdDbFilePath = @"";
            req.connCfg.metaLogDir = @"C:\MyMETA";
            req.filterCfg.pFilter = new IntPtr[2];
            {
                // bootloader filter
                int count = 4;
                bool bIf;
                SP_COM_FILTER_LIST_S filterBoot;
                IntPtr[] filter;

                filter = new IntPtr[count];
                filter[0] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_0003"); // index 0 is for brom
                filter[1] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_2000");
                filter[2] = Marshal.StringToHGlobalAnsi(@"VID_0525&PID_A4A7");
                filter[3] = Marshal.StringToHGlobalAnsi(@"VID_1004&PID_6000");

                filterBoot = new SP_COM_FILTER_LIST_S();
                filterBoot.count = (uint)count;
                filterBoot.type = SP_FILTER_TYPE_E.WHITE_LIST;
                filterBoot.list = Marshal.AllocHGlobal(Marshal.SizeOf(IntPtr.Zero) * count);
                Marshal.Copy(filter, 0, filterBoot.list, count);

                bIf = false;
                filterBoot.iface = Marshal.AllocHGlobal(Marshal.SizeOf(IntPtr.Zero));
                Marshal.StructureToPtr(bIf, filterBoot.iface, true);

                req.filterCfg.pFilter[0] = Marshal.AllocHGlobal(Marshal.SizeOf(filterBoot));
                Marshal.StructureToPtr(filterBoot, req.filterCfg.pFilter[0], true);
            }
            {
                // kernel filter
                int count = 8;
                bool bIf;
                SP_COM_FILTER_LIST_S filterKernel;
                IntPtr[] filter;

                filter = new IntPtr[count];
                filter[0] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_2007");
                filter[1] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_2006&MI_02");
                filter[2] = Marshal.StringToHGlobalAnsi(@"VID_0BB4&PID_0005&MI_02");
                filter[3] = Marshal.StringToHGlobalAnsi(@"VID_1004&PID_6000");
                filter[4] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_202D&MI_01");
                filter[5] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_200E&MI_01");
                filter[6] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_2026&MI_02");
                filter[7] = Marshal.StringToHGlobalAnsi(@"VID_0E8D&PID_7101&MI_00");

                filterKernel = new SP_COM_FILTER_LIST_S();
                filterKernel.count = (uint)count;
                filterKernel.type = SP_FILTER_TYPE_E.WHITE_LIST;
                filterKernel.list = Marshal.AllocHGlobal(Marshal.SizeOf(IntPtr.Zero) * count);
                Marshal.Copy(filter, 0, filterKernel.list, count);

                bIf = false;
                filterKernel.iface = Marshal.AllocHGlobal(Marshal.SizeOf(IntPtr.Zero));
                Marshal.StructureToPtr(bIf, filterKernel.iface, true);

                req.filterCfg.pFilter[1] = Marshal.AllocHGlobal(Marshal.SizeOf(filterKernel));
                Marshal.StructureToPtr(filterKernel, req.filterCfg.pFilter[1], true);
            }
            Log.GetInstance().d("MTK WiFi", "正在进入Meta模式，请连上手机");

            Thread currentthread = Thread.CurrentThread;
            new Task(() => FindSerialPort(currentthread)).Start();

            result = Wrapper_ConnectTarget(m_nDutID, ref req, ref m_hMeta);

        



            UInt32 value = 999, u4Addr = 0, u4value = 0;
            int ms = 0;

            ms = SP_META_WIFI_OPEN_r(m_hMeta.md, (UInt32)(15000));
            Log.GetInstance().d("MTK WiFi", "SP_META_WIFI_OPEN_r=" + ms.ToString());
            ms = SP_META_WiFi_GetChipVersion_r(m_hMeta.md, 18000, ref value);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_GetChipVersion_r=" + ms.ToString());
            ms = SP_META_WiFi_readMCR32_r(m_hMeta.md, 1200, u4Addr, ref u4value);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_readMCR32_r=" + ms.ToString());
            m_wifiChipVersion = u4value & 0xFFFF;
            if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
            {
            }
            else
            {

            }
            if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
            {
                NVRAM_ACCESS_STRUCT wifinvram = new NVRAM_ACCESS_STRUCT();

                byte[] pszBufferRaw = new byte[512];
                pszBufferRaw[0] = (0x6628 >> 8) & 0x00ff;
                pszBufferRaw[1] = 0x6628 & 0x00ff;

                wifinvram.data = System.Text.Encoding.Default.GetString(pszBufferRaw);
                wifinvram.dataLen = 2;
                wifinvram.dataOffset = 0x3c;
                SP_META_WiFi_WriteNVRAM_r(m_hMeta.md, 1200, ref wifinvram);
            }
            ENUM_CFG_SRC_TYPE_T buffType = new ENUM_CFG_SRC_TYPE_T();
            ms = SP_META_WiFi_QueryConfig_r(m_hMeta.md, 5000, ref buffType);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_QueryConfig_r=" + ms.ToString());
            ms = SP_META_WiFi_setTestMode_r(m_hMeta.md, 5000);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setTestMode_r=" + ms.ToString());
            ms = SP_META_WiFi_switchAntenna_r(m_hMeta.md, 5000, 0);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_switchAntenna_r=" + ms.ToString());
            
            if (result != 0)
            {
                //Console.WriteLine("Connect fail.");
                return false;
            }

            //Console.WriteLine("Connect ok. DutID = {0}, APHandle = {1}, MDHandle = {2}.", m_nDutID, m_hMeta.ap, m_hMeta.md);

            return true;
        }


        /// <summary>
        /// 枚举win32 api
        /// </summary>
        public enum HardwareEnum
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity,//all device
        }

        /// <summary>
        /// WMI取硬件信息
        /// </summary>
        /// <param name="hardType"></param>
        /// <param name="propKey"></param>
        /// <returns></returns>
        private string[] MulGetHardwareInfo(HardwareEnum hardType, string propKey)
        {

            List<string> strs = new List<string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties[propKey].Value.ToString().Contains("COM"))
                        {
                            strs.Add(hardInfo.Properties[propKey].Value.ToString());
                        }

                    }
                    searcher.Dispose();
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            { strs = null; }
        }


        private void FindSerialPort(Thread thread)
        {
            bool threadsuspend = false;
            //通过WMI获取COM端口
            do
            {
                string[] ss = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity, "Name");
                foreach (string com in ss)
                {
                    if (com.Contains("PreLoader"))
                    {
                        thread.Suspend();
                        threadsuspend = true;
                        Thread.Sleep(5000);
                        thread.Resume();
                        break;
                    }
                }
            } while (false);
        }

        public bool SwitchAp2Modem()
        {
            int result;

            //Console.WriteLine("Switch ap to modem ...");
            result = Wrapper_ApToModem(m_nDutID);
            if (result != 0)
            {
                //Console.WriteLine("Switch fail.");
                return false;
            }

            //Console.WriteLine("Switch ok.");
            return true;
        }

        public bool SwitchModemRfRat(WRAPPER_TEST_RAT_E rat)
        {
            int result;

            //Console.WriteLine("Switch modem RF rat to {0} ...", rat);
            result = Wrapper_SwitchTestRat(m_nDutID, rat);
            if (result != 0)
            {
                //Console.WriteLine("Switch fail.");
                return false;
            }

            //Console.WriteLine("Switch ok.");
            return true;
        }

        public bool SwitchModem2AP()
        {
            int result;

            //Console.WriteLine("Switch modem to ap ...");
            result = Wrapper_ModemToAp(m_nDutID);
            if (result != 0)
            {
                //Console.WriteLine("Switch fail({0}:{1}).", result, SP_META_GetErrorString(result));
                return false;
            }

            //Console.WriteLine("Switch ok.");
            return true;
        }

        public bool Disconnect()
        {
            int result;

            //Console.WriteLine("Disconnect ...");
            result = SP_META_DisconnectWithTarget_r(m_hMeta.ap);
            if (result != 0)
                //Console.WriteLine("Disconnect fail({0}:{1}).", result, SP_META_GetErrorString(result));

            m_nDutID = 0;
            m_hMeta.ap = 0;
            m_hMeta.md = 0;

            return true;
        }
        public bool GetAndroidVer()
        {
            int result;
            VerInfo_V2_Cnf cnf = new VerInfo_V2_Cnf();
            short token = 0;

            //Console.WriteLine("Get target version infomation ...");
            result = SP_META_GetTargetVerInfoV2_r(m_hMeta.ap, ref cnf, ref token, IntPtr.Zero);
            if (result != 0)
            {
                //Console.WriteLine("Get target version infomation fail({0}:{1})", result, SP_META_GetErrorString(result));
                return false;
            }

            //Console.WriteLine(" ver.token = {0}", token);
            //Console.WriteLine(" ver.status = {0}", cnf.status);
            //Console.WriteLine(" ver.BB_CHIP = {0}", cnf.BB_CHIP);
            //Console.WriteLine(" ver.ECO_VER = {0}", cnf.ECO_VER);
            //Console.WriteLine(" ver.SW_TIME = {0}", cnf.SW_TIME);
            //Console.WriteLine(" ver.DSP_FW = {0}", cnf.DSP_FW);
            //Console.WriteLine(" ver.DSP_PATCH = {0}", cnf.DSP_PATCH);
            //Console.WriteLine(" ver.SW_VER = {0}", cnf.SW_VER);
            //Console.WriteLine(" ver.HW_VER = {0}", cnf.HW_VER);
            //Console.WriteLine(" ver.MELODY_VER = {0}", cnf.MELODY_VER);
            //Console.WriteLine(" ver.BUILD_DISP_ID = {0}", cnf.BUILD_DISP_ID);
            return true;
        }

        public bool BackUpNvram()
        {
            int result;
            SetCleanBootFlag_REQ req = new SetCleanBootFlag_REQ();
            SetCleanBootFlag_CNF cnf = new SetCleanBootFlag_CNF();

            req.Notused = 0;

            Console.WriteLine("Backup nvram ...");
            result = SP_META_SetCleanBootFlag_r(m_hMeta.ap, 15000, ref req, ref cnf);
            if (result != 0)
            {
                //Console.WriteLine("Backup nvram fail({0}:{1})", result, SP_META_GetErrorString(result));
                return false;
            }

            //Console.WriteLine("Backup nvram {0}.", SP_META_GetErrorString(0));
            return true;
        }

        public bool GetModemVer()
        {
            int result;
            VerInfo_Cnf cnf = new VerInfo_Cnf();

            result = META_GetTargetVerInfoEx2_r(m_hMeta.md, 5000, ref cnf);
            if (result != 0)
            {
                //Console.WriteLine("Get modem version fail({0}:{1}).", result, META_GetErrorString(result));
                return false;
            }
            //Console.WriteLine("Get modem version {0}", META_GetErrorString(0));
            //Console.WriteLine(" ver.BB_CHIP = {0}", cnf.BB_CHIP);
            //Console.WriteLine(" ver.BB_CHIP = {0}", cnf.BB_CHIP);
            //Console.WriteLine(" ver.ECO_VER = {0}", cnf.ECO_VER);
            //Console.WriteLine(" ver.DSP_FW = {0}", cnf.DSP_FW);
            //Console.WriteLine(" ver.DSP_PATCH = {0}", cnf.DSP_PATCH);
            //Console.WriteLine(" ver.SW_VER = {0}", cnf.SW_VER);
            //Console.WriteLine(" ver.HW_VER = {0}", cnf.HW_VER);
            //Console.WriteLine(" ver.MELODY_VER = {0}", cnf.MELODY_VER);
            //Console.WriteLine(" ver.SW_FLAVOR = {0}", cnf.SW_FLAVOR);

            return true;
        }

        public void MTK_SetParam(int channelfreqMHz,string Bandwidth,string Argument="")
        {
            //UInt32 value = 999, u4Addr = 0, u4value = 0;
            int ms = 0;

            //ms = SP_META_WIFI_OPEN_r(m_hMeta.md, (UInt32)(15000));
            //Log.GetInstance().d("MTK WiFi", "SP_META_WIFI_OPEN_r=" + ms.ToString());
            //ms = SP_META_WiFi_GetChipVersion_r(m_hMeta.md, 18000, ref value);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_GetChipVersion_r=" + ms.ToString());
            //ms = SP_META_WiFi_readMCR32_r(m_hMeta.md, 1200, u4Addr, ref u4value);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_readMCR32_r=" + ms.ToString());
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
            //    SP_META_WiFi_WriteNVRAM_r(m_hMeta.md, 1200, ref wifinvram);
            //}
            //ENUM_CFG_SRC_TYPE_T buffType = new ENUM_CFG_SRC_TYPE_T();
            //ms = SP_META_WiFi_QueryConfig_r(m_hMeta.md, 5000, ref buffType);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_QueryConfig_r=" + ms.ToString());
            //ms = SP_META_WiFi_setTestMode_r(m_hMeta.md, 5000);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setTestMode_r=" + ms.ToString());
            //ms = SP_META_WiFi_switchAntenna_r(m_hMeta.md, 5000, 0);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_switchAntenna_r=" + ms.ToString());
            if (Testic == "MTK")
            {
                ms = SP_META_WiFi_setChannel_r(m_hMeta.md, 1200, channelfreqMHz * 1000);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setChannel_r(" + (channelfreqMHz * 1000).ToString() + ")= " + ms.ToString());
                if (Argument.Contains("AC"))
                {
                    ms = SP_META_WiFi_setBandwidthEx_r(m_hMeta.md, 1200, GetBandWidth(Bandwidth, Argument), GetBandWidth(Bandwidth, Argument), 0);//20M为 0
                    Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setBandwidthEx_r(" + Bandwidth + ")= " + ms.ToString());
                }
                else
                {
                    ms = SP_META_WiFi_setBandwidth_r(m_hMeta.md, 1200, GetBandWidth(Bandwidth, Argument));//20M为 0
                    Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setBandwidth_r(" + Bandwidth + ")= " + ms.ToString());
                }
                
            }
            else if(Testic == "MTK New")
            {
                Log.GetInstance().d("MTK New", "设置信道");
                ms = SP_META_WiFi_setChannel_r_New(m_hMeta_New, 1200, channelfreqMHz * 1000);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setChannel_r_New(" + (channelfreqMHz * 1000).ToString() + ")= " + ms.ToString());
                Log.GetInstance().d("MTK New", "带宽");
                if (Argument.Contains("AC"))
                {
                    ms = SP_META_WiFi_setBandwidthEx_r_New(m_hMeta_New, 1200, GetBandWidth(Bandwidth, Argument), GetBandWidth(Bandwidth, Argument), 0);//20M为 0
                    Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setBandwidthEx_r_New(" + Bandwidth + ")= " + ms.ToString());
                }
                else
                {
                    ms = SP_META_WiFi_setBandwidth_r_New(m_hMeta_New, 1200, GetBandWidth(Bandwidth, Argument));//20M为 0
                    Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setBandwidth_r_New(" + Bandwidth + ")= " + ms.ToString());
                }                
            }
        }

        public void MTK_SetTxParam(UInt32 bLongPreamble,double enppower,string Ratestr,string chain="1")
        {
            int ms = 1;

            if (Testic == "MTK New")
            {
                ms = SP_META_WiFi_setNss_r(m_hMeta_New, 1200, 1);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setNss_r=" + ms.ToString());
                ms = SP_META_WiFi_setTXPath_r(m_hMeta_New, 1200, Convert.ToUInt16(chain));
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setTXPath_r=" + ms.ToString());

                 
                WIFI_TX_PARAM_T wifitxparam = new WIFI_TX_PARAM_T();
                wifitxparam.bufSize = 1024;
                wifitxparam.bLongPreamble = bLongPreamble;//green
                                                          //MessageBox.Show("bLongPreamble=" + bLongPreamble.ToString());
                wifitxparam.txRate = GetRate(Ratestr);//1M速率为 0x00000000
                wifitxparam.pktCount = 0;
                wifitxparam.pktInterval = 800;
                //if (Ratestr.Contains("MCS")|| Ratestr.Contains("36")|| Ratestr.Contains("54")|| Ratestr.Contains("48"))
                //{
                //    wifitxparam.bGainControl = 2;
                //}
                //else
                //{
                    wifitxparam.bGainControl = 1;
                //}

                wifitxparam.gainControl = (UInt32)((enppower) * 2);//等于目标功率的2倍       
                //MessageBox.Show("enppower=" + (enppower*2).ToString());
                wifitxparam.txAntenna = 0;//只有一个WiFi天线

                SP_META_WiFi_setStandBy_r_New(m_hMeta_New, 1200);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setStandBy_r_New=" + ms.ToString());

                ms = SP_META_WiFi_setPacketTxEx_r_New(m_hMeta_New, 1200, ref wifitxparam);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setPacketTxEx_r_New=" + ms.ToString());
                //Thread.Sleep(1000);
                
            }
            else
            {
                WIFI_TX_PARAM_T wifitxparam = new WIFI_TX_PARAM_T();
                wifitxparam.bufSize = 1024;
                wifitxparam.bLongPreamble = bLongPreamble;//green
                                                          //MessageBox.Show("bLongPreamble=" + bLongPreamble.ToString());
                wifitxparam.txRate = GetRate(Ratestr);//1M速率为 0x00000000
                wifitxparam.pktCount = 0;
                wifitxparam.pktInterval = 800;
                //if (Ratestr.Contains("MCS"))
                //{
                //    wifitxparam.bGainControl = 0;
                //}
                //else
                //{
                wifitxparam.bGainControl = 1;
                //}

                wifitxparam.gainControl = (UInt32)((enppower) * 2);//等于目标功率的2倍
                                                                       //MessageBox.Show("enppower=" + (enppower*2).ToString());
                wifitxparam.txAntenna = 0;//只有一个WiFi天线
                Log.GetInstance().d("MTK WiFi", "wifitxparam \tPreamble:" + wifitxparam.bLongPreamble+ "\t txRate:"+ wifitxparam.txRate);
                ms = SP_META_WiFi_setPacketTxEx_r(m_hMeta.md, 1200, ref wifitxparam);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setPacketTxEx_r=" + ms.ToString());
                //Thread.Sleep(1000);
                //ms = SP_META_WiFi_setStandBy_r(m_hMeta.md, 1200);
                //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setPacketTxEx_r=" + ms.ToString());
            }
        }

        public void MTK_SetRxMode(string chain="1")
        {
            int ms = 2;
            if (Testic == "MTK New")
            {
                ms = SP_META_WiFi_setNss_r(m_hMeta_New, 1200, 2);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setNss_r=" + ms.ToString());
                ms = SP_META_WiFi_setRXPath_r(m_hMeta_New, 1200, 0x00030000);
                //if (chain == "1")
                //{
                //    ms = SP_META_WiFi_setRXPath_r(m_hMeta_New, 1200, 0x00010000);
                //}
                //else
                //{
                //    ms = SP_META_WiFi_setRXPath_r(m_hMeta_New, 1200, 0x00020000);
                //}
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setTXPath_r=" + ms.ToString());
                SP_META_WiFi_setStandBy_r_New(m_hMeta_New, 1200);
                //Log.GetInstance().d("MTK WIFI", "SP_META_WiFi_setStandBy_r(+" + m_hMeta_New.ToString() + ",1200)");
                SP_META_WiFi_setRxTest_r_New(m_hMeta_New, 1200);
                Log.GetInstance().d("MTK WIFI", "SP_META_WiFi_setRxTest_r(" + m_hMeta_New.ToString() + ",1200)");
            }
            else
            {
                SP_META_WiFi_setStandBy_r(m_hMeta.md, 1200);
                Log.GetInstance().d("MTK WIFI", "SP_META_WiFi_setStandBy_r(+" + m_hMeta.md.ToString() + ",1200)");
                SP_META_WiFi_setRxTest_r(m_hMeta.md, 1200);
                Log.GetInstance().d("MTK WIFI", "SP_META_WiFi_setRxTest_r(+" + m_hMeta.md.ToString() + ",1200)");
            }
        }

        public double MTK_RxTest()
        {
            UInt32 OKCOUNT = 0, ERRORCOUNT = 0;
            if (Testic == "MTK New")
            {
                SP_META_WiFi_ReceivedOKCount_r_New(m_hMeta_New, 1200, ref OKCOUNT);
                SP_META_WiFi_ReceivedErrorCount_r_New(m_hMeta_New, 1200, ref ERRORCOUNT);
                SP_META_WiFi_setStandBy_r_New(m_hMeta_New, 1200);
            }
            else
            {
                SP_META_WiFi_ReceivedOKCount_r(m_hMeta.md, 1200, ref OKCOUNT);
                SP_META_WiFi_ReceivedErrorCount_r(m_hMeta.md, 1200, ref ERRORCOUNT);
                SP_META_WiFi_setStandBy_r(m_hMeta.md, 1200);
            }
            return OKCOUNT;
        }

        public string MTK_RxTest_New()
        {
            UInt32 OKCOUNT = 0, ERRORCOUNT = 0;
            if (Testic == "MTK New")
            {
                SP_META_WiFi_ReceivedOKCount_r_New(m_hMeta_New, 1200, ref OKCOUNT);
                SP_META_WiFi_ReceivedErrorCount_r_New(m_hMeta_New, 1200, ref ERRORCOUNT);
                SP_META_WiFi_setStandBy_r_New(m_hMeta_New, 1200);
            }
            else
            {
                SP_META_WiFi_ReceivedOKCount_r(m_hMeta.md, 1200, ref OKCOUNT);
                SP_META_WiFi_ReceivedErrorCount_r(m_hMeta.md, 1200, ref ERRORCOUNT);
                SP_META_WiFi_setStandBy_r(m_hMeta.md, 1200);
            }
            return OKCOUNT+";"+ERRORCOUNT;
        }
        public void MTK_StopTest()
        {
            SP_META_WiFi_setStandBy_r(m_hMeta.md, 1200);
        }

        public void MTK_StopTest_New()
        {
            SP_META_WiFi_setStandBy_r_New(m_hMeta_New, 1200);
        }

        public void WifiMTKInit()
        {
            UInt32 value = 999, u4Addr = 0, u4value = 0;
            int ms = SP_META_WIFI_OPEN_r(m_hMeta.md, 15000);
            Log.GetInstance().d("MTK WiFi","SP_META_WIFI_OPEN_r="+ms.ToString());
            ms= SP_META_WiFi_GetChipVersion_r(m_hMeta.md, 18000, ref value);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_GetChipVersion_r=" + ms.ToString());
            ms=SP_META_WiFi_readMCR32_r(m_hMeta.md, 1200, u4Addr, ref u4value);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_readMCR32_r=" + ms.ToString());
            m_wifiChipVersion = u4value & 0xFFFF;
            if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
            {
            }
            else
            {

            }
            if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
            {
                NVRAM_ACCESS_STRUCT wifinvram = new NVRAM_ACCESS_STRUCT();

                byte[] pszBufferRaw = new byte[512];
                pszBufferRaw[0] = (0x6628 >> 8) & 0x00ff;
                pszBufferRaw[1] = 0x6628 & 0x00ff;

                wifinvram.data = System.Text.Encoding.Default.GetString(pszBufferRaw);
                wifinvram.dataLen = 2;
                wifinvram.dataOffset = 0x3c;
                SP_META_WiFi_WriteNVRAM_r(m_hMeta.md, 1200, ref wifinvram);
            }
            //ENUM_CFG_SRC_TYPE_T buffType = new ENUM_CFG_SRC_TYPE_T();
            //ms= SP_META_WiFi_QueryConfig_r(m_hMeta.md, 5000, ref buffType);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_QueryConfig_r=" + ms.ToString());
            //ms = SP_META_WiFi_setTestMode_r(m_hMeta.md, 5000);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setTestMode_r=" + ms.ToString());
            //ms = SP_META_WiFi_switchAntenna_r(m_hMeta.md, 5000, 0);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_switchAntenna_r=" + ms.ToString());
            //ms = SP_META_WiFi_setChannel_r(m_hMeta.md, 1200, 2412000);
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setChannel_r=" + ms.ToString());
            //ms = SP_META_WiFi_setBandwidth_r(m_hMeta.md, 1200, GetBandWidth("20M"));//20M为 0
            //Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setBandwidth_r=" + ms.ToString());

            WIFI_TX_PARAM_T wifitxparam = new WIFI_TX_PARAM_T();
            wifitxparam.bufSize = 1024;
            wifitxparam.bLongPreamble = 3;//green
            wifitxparam.txRate = GetRate("1M");//1M速率为 0x00000000
            wifitxparam.pktCount = 0;
            wifitxparam.pktInterval = 50;
            wifitxparam.bGainControl = 1;
            wifitxparam.gainControl = 42;//等于目标功率的2倍
            wifitxparam.txAntenna = 0;//只有一个WiFi天线
            ms = SP_META_WiFi_setPacketTxEx_r(m_hMeta.md, 1200, ref wifitxparam);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setPacketTxEx_r=" + ms.ToString());
            //MessageBox.Show("DUT设置完毕，请手动查看仪器测试值是否正常，之后按任意键继续");
            ms = SP_META_WiFi_setStandBy_r(m_hMeta.md, 1200);
            Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setPacketTxEx_r=" + ms.ToString());
        }

        private UInt32 GetRate(string Ratestr)
        {
            UInt32 rate = 0;
            switch (Ratestr)
            {
                case "MCS0":
                    rate = 0x80000000;
                    break;
                case "MCS1":
                    rate = 0x80000001;
                    break;
                case "MCS2":
                    rate = 0x80000002;
                    break;
                case "MCS3":
                    rate = 0x80000003;
                    break;
                case "MCS4":
                    rate = 0x80000004;
                    break;
                case "MCS5":
                    rate = 0x80000005;
                    break;
                case "MCS6":
                    rate = 0x80000006;
                    break;
                case "MCS7":
                    rate = 0x80000007;
                    break;
                case "MCS8":
                    rate = 0x80000008;
                    break;
                case "MCS9":
                    rate = 0x80000009;
                    break;
                case "MCS10":
                    rate = 0x8000000A;
                    break;
                case "MCS11":
                    rate = 0x8000000B;
                    break;
                case "MCS12":
                    rate = 0x8000000C;
                    break;
                case "MCS13":
                    rate = 0x8000000D;
                    break;
                case "MCS14":
                    rate = 0x8000000E;
                    break;
                case "MCS15":
                    rate = 0x8000000F;
                    break;
                case "MCS32":
                    rate = 0x80000010;
                    break;
                case "1Mbit/s":
                    rate = 0x00000000;
                    break;
                case "2Mbit/s":
                    rate = 0x00000001;
                    break;
                case "5.5Mbit/s":
                    rate = 0x00000002;
                    break;
                case "11Mbit/s":
                    rate = 0x00000003;
                    break;
                case "6Mbit/s":
                    rate = 0x00000004;
                    break;
                case "9Mbit/s":
                    rate = 0x00000005;
                    break;
                case "12Mbit/s":
                    rate = 0x00000006;
                    break;
                case "18Mbit/s":
                    rate = 0x00000007;
                    break;
                case "24Mbit/s":
                    rate = 0x00000008;
                    break;
                case "36Mbit/s":
                    rate = 0x00000009;
                    break;
                case "48Mbit/s":
                    rate = 0x0000000A;
                    break;
                case "54Mbit/s":
                    rate = 0x0000000B;
                    break;
            }

            return rate;
        }

        private UInt32 GetPreamble(string Preamblestr)
        {
            UInt32 preamble = 0;
            switch (Preamblestr)
            {
                case "Normal":
                    preamble = 0;
                    break;
                case "Short":
                    preamble = 1;
                    break;
                case "Mixed":
                    preamble = 2;
                    break;
                case "Green":
                    preamble = 3;
                    break;
                case "AC":
                    preamble = 4;
                    break;
            }

            return preamble;
        }

        public UInt32 GetBandWidth(string bandwidthstr,string Argument)
        {
            UInt32 bandwidth = 0;
            if (Argument.Contains("AC") == false)
            {
                switch (bandwidthstr)
                {
                    case "20M":
                        bandwidth = 0;
                        break;
                    case "40M":
                        bandwidth = 1;
                        break;
                    case "U20":
                        bandwidth = 2;
                        break;
                    case "U40":
                        bandwidth = 3;
                        break;
                    case "80M"://尝试性的输入80M带宽的代码，不一定有效，如果能够测试出来，也不确定是否真的就是80M的值
                        bandwidth = 4;
                        break;
                    default:
                        bandwidth = 999;
                        break;

                }
            }
            else
            {
                switch (bandwidthstr)
                {
                    case "20M":
                        bandwidth = 0;
                        break;
                    case "40M":
                        bandwidth = 1;
                        break;
                    case "80M":
                        bandwidth = 2;
                        break;
                    case "160M":
                        bandwidth = 3;
                        break;
                    default:
                        bandwidth = 999;
                        break;

                }
            }
            return bandwidth;
        }

       

        #endregion
 
        #region New

        const string METAAPP_DLL_PATH = @"lib\MTK\New\MetaApp.dll";
        const string metacore = @"lib\MTK\New\MetaCore.dll";

        const int MAX_DUT_INIT = 16;
        const int MAX_BARCODE_SIZE = 64;
        const int MAX_TIME_SIZE = 32;
        const int MOBILE_LOG_SOCKET_PORT = 10119;
        const int MODEM_LOG_SOCKET_PORT = 10001;

        public enum METAAPP_CONN_TYPE_E
        {
            METAAPP_CONN_CONNCET_BY_UART = 0,
            METAAPP_CONN_CONNCET_BY_USB = 1,
            METAAPP_CONN_CONNCET_BY_WIFI = 2,
            METAAPP_CONN_CONNCET_NUM = METAAPP_CONN_CONNCET_BY_WIFI + 1
        }

        /// <summary>
        /// 连接结果
        /// </summary>
        public enum METAAPP_HIGH_LEVEL_RESULT
        {
            METAAPP_HIGH_LEVEL_SUCCESS = 0,
            METAAPP_HIGH_LEVEL_FAIL = 1,
            METAAPP_HIGH_LEVEL_HANDLE_ALLOC_FAIL = 2,
            METAAPP_HIGH_LEVEL_OPEN_DLL_LOG_FAIL = 3,
            METAAPP_HIGH_LEVEL_GET_PRELOAD_PORT_FAIL = 4,
            METAAPP_HIGH_LEVEL_BOOT_PRELOAD_FAIL = 5,
            METAAPP_HIGH_LEVEL_GET_KERNEL_PORT = 6,
            METAAPP_HIGH_LEVEL_CONNECT_AP_META_FAIL = 7,
            METAAPP_HIGH_LEVEL_GET_AP_VERSION_FAIL = 8,
            METAAPP_HIGH_LEVEL_LOAD_AP_DATABASE_FROM_DUT_FAIL = 9,
            METAAPP_HIGH_LEVEL_LOAD_MD_DATABASE_FROM_DUT_FAIL = 10,
            METAAPP_HIGH_LEVEL_INIT_AP_DATABASE_FAIL = 11,
            METAAPP_HIGH_LEVEL_INIT_MINICOMLOGGER_FAIL = 12,
            METAAPP_HIGH_LEVEL_OPEN_MODEM_LOG_FAIL = 13,
            METAAPP_HIGH_LEVEL_AP_DATABASE_NOT_MATCH = 14,
            METAAPP_HIGH_LEVEL_MD_DATABASE_NOT_MATCH = 15,

            METAAPP_HIGH_LEVEL_REBOOT_MODEM_FAIL = 51,
            METAAPP_HIGH_LEVEL_GET_SP_MODEM_INFO_FAIL = 52,
            METAAPP_HIGH_LEVEL_DISCON_AP_TO_MODEM_FAIL = 53,
            METAAPP_HIGH_LEVEL_GET_MODEM_VERSION_FAIL = 54,
            METAAPP_HIGH_LEVEL_INIT_MODEM_DATABASE_FAIL = 55,
            METAAPP_HIGH_LEVEL_SET_MMC2K_INFO_FAIL = 56,

            METAAPP_HIGH_LEVEL_CONNECT_AP_FAIL = 101,
            METAAPP_HIGH_LEVEL_C2K_INIT_FAIL = 102,
            METAAPP_HIGH_LEVEL_SWTICH_TO_C2K_FAIL = 103,
            METAAPP_HIGH_LEVEL_EXIT_C2K_FAIL = 104,

            METAAPP_HIGH_LEVEL_MD_STATUS_NOT_CORRENT_FAIL = 120,

            METAAPP_HIGH_LEVEL_CLEAN_BOOT_FAIL = 150,
            METAAPP_HIGH_LEVEL_EXIT_META_FAIL = 151,


            METAAPP_HIGH_LEVEL_END = 65536
        }

        /// <summary>
        /// 选择连接的模式
        /// </summary>
        public enum METAAPP_CONN_MODE_E
        {
            METAAPP_CONN_BOOT_META_MODE = 0,                      // boot process
            METAAPP_CONN_ALREADY_IN_META_MODE = 1,                // already in meta mode(skip boot preloader)
            METAAPP_CONN_ATM_MODE = 2,                            // ATM mode
            METAAPP_CONN_MODE_NUM = METAAPP_CONN_ATM_MODE + 1
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct METAAPP_CONN_BOOT_STTING_T
        {
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr authHandle;               // void *, for security feature
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr scertHandle;              // void *, for security feature

            [MarshalAs(UnmanagedType.U1)]
            public bool bEnableAdbDevice;
            public uint uPortNumber;
            public ushort uMDMode;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct METAAPP_CONN_AP_SETTING_T
        {
            public int kernelComPort;     //kernel com port
            [MarshalAs(UnmanagedType.U1)]
            public bool bDbFileFromDUT;     //true: database from DUT,false: database transmit by user
            [MarshalAs(UnmanagedType.LPStr)]
            public string pApDbFilePath;    // char *, specify ap nvram db file path, "" means get the file from device.
            [MarshalAs(UnmanagedType.LPStr)]
            public string pMdDbFilePath;    // char *, specify modem nvram db file path, "" means get the file from device. MD database path for WG\LWG\LWTG\LWCTG...database file
            [MarshalAs(UnmanagedType.LPStr)]
            public string pMdDbFilePath1;    // char *, specify modem nvram db file path, "" means get the file from device. MD database path for TG\LTG....database file
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct METAAPP_CONN_FILTER_SETTING_T
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string brom;
            [MarshalAs(UnmanagedType.LPStr)]
            public string preloader;
            [MarshalAs(UnmanagedType.LPStr)]
            public string kernel;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct METAAPP_CONN_LOG_SETTING_T
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool enableDllLog;                            // if bypass borom/preloader handshake
            [MarshalAs(UnmanagedType.U1)]
            public bool enableUartLog;                           // if enable modem log after modem bootup
            [MarshalAs(UnmanagedType.U1)]
            public bool enablePcUsbDriverLog;                    // if enable modem log after modem bootup
            public uint iMDLoggEnable;                           //0 disable; 1:ELT Port output 2:Modem log in SD card
            public uint iMobileLogEnable;                        //0 disable; 1:ELT Port output 2:Saving in SD card
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string IP;                                    //char IP[64];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_BARCODE_SIZE + MAX_TIME_SIZE)]
            public string pcbSn;                                 //char pcbSn[MAX_BARCODE_SIZE + MAX_TIME_SIZE];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string logSavePath;                           //char logSavePath[MAX_PATH];
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct METAAPP_CONN_STTING_T
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool autoScanPort;
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr stopFlag;                  //int *

            public METAAPP_CONN_TYPE_E connectType;
            public METAAPP_CONN_MODE_E connectMode;
            public uint uTimeOutMs;
            public METAAPP_CONN_BOOT_STTING_T bootSetting;
            public METAAPP_CONN_AP_SETTING_T connectSetting;
            public METAAPP_CONN_LOG_SETTING_T logSetting;
            public METAAPP_CONN_FILTER_SETTING_T filterSetting;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct METAAPP_CONNCT_CNF_T
        {
            public int hMDHandle;
            public uint uPreloaderPortNumber;
            public uint uKernelPortNumber;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct LogConfigure_s
        {
            public int iMDLoggEnable;                    //0 disable; 1:ELT Port output 2:Modem log in SD card
            [MarshalAs(UnmanagedType.U1)]
            public bool bSDMDLoggEnable;
            public int iMobileLogEnable;                 //0 disable; 1:ELT Port output 2:Saving in SD card
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_BARCODE_SIZE + MAX_TIME_SIZE)]
            public string pLogName;                      //char pcbSn[MAX_BARCODE_SIZE + MAX_TIME_SIZE];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string pLogPath;                      //char logSavePath[MAX_PATH];
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct LogWifiCon_s
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string IP;  //char IP[64];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct MiniComLogInput_s
        {
            public LogConfigure_s sLogConfigure;
            public uint nConnectType;
            public LogWifiCon_s sWifiCon;
            public uint m_uKernelCom;
            public uint m_uDebugCom;
        }


        //METAAPP_HIGH_LEVEL_RESULT METAAPPAPI  METAAPP_ConnectTargetAllInOne_r(const int threadSlot, METAAPP_CONN_STTING_T *pSetting, METAAPP_CONNCT_CNF_T* pCnf);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_ConnectTargetAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ConnectTargetAllInOne_r(int threadSlot, ref METAAPP_CONN_STTING_T pSetting, ref METAAPP_CONNCT_CNF_T pCnf);

        //METAAPP_HIGH_LEVEL_RESULT METAAPPAPI  METAAPP_ApToModemAllInOne_r(const int threadSlot);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_ApToModemAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ApToModemAllInOne_r(int threadSlot);


        //METAAPP_HIGH_LEVEL_RESULT METAAPPAPI  METAAPP_ModemToApAllInOne_r(const int threadSlot);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_ModemToApAllInOne_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_ModemToApAllInOne_r(int threadSlot);

        //METAAPP_HIGH_LEVEL_RESULT METAAPPAPI  METAAPP_SwitchTestRat_r(const int threadSlot, METAAPP_TEST_RAT_E tRat);
        //[DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_SwitchTestRat_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_SwitchTestRat_r(int threadSlot, METAAPP_TEST_RAT_E tRat);

        //METAAPP_HIGH_LEVEL_RESULT METAAPPAPI  METAAPP_DisConnectMeta_r(const int threadSlot, bool isPoweroff);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "METAAPP_DisConnectMeta_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT METAAPP_DisConnectMeta_r(int threadSlot, bool isPoweroff);



        //modem/mobile log function

        //META_RESULT METAAPPAPI MetaApp_InitMetaLog(const int meta_handle, MiniComLogInput_s sLogInput);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_InitMetaLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_InitMetaLog(int meta_handle, MiniComLogInput_s sLogInput);


        //META_RESULT METAAPPAPI MetaApp_StartMetaLog(const int meta_handle);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_InitMetaLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_StartMetaLog(int meta_handle);


        //META_RESULT METAAPPAPI MetaApp_StopLog(const int meta_handle, bool bAssert);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_StopLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_StopLog(int meta_handle, bool bAssert);

        //META_RESULT METAAPPAPI MetaApp_RenameLogFiles(char* strPath, const char* strKeyPhase, const char* strReplacePhase);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_RenameLogFiles", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_RenameLogFiles(ref string strPath, ref string strKeyPhase, ref string strReplacePhase);

        //META_RESULT METAAPPAPI MetaApp_AddTimeForLogName(char* strInputName, char* strOutputName, int iOutputStringSize);
        [DllImport(METAAPP_DLL_PATH, EntryPoint = "MetaApp_AddTimeForLogName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static METAAPP_HIGH_LEVEL_RESULT MetaApp_AddTimeForLogName(ref string strInputName, ref string strOutputName, int iOutputStringSize);


       


        int m_nDutID_New = 0;
        int m_hMeta_New = 0;
        int m_nStopFlag_New = 0;

        public int ConnetWithUSB(String logSavePath, out string ErrorMsg)
        {
            Log.GetInstance().d("MTK New", "手机开始进入META状态");
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
                    30000; //scan port timeout，preloader handshake，connect to SP/Modem meta 的timeout 设定，单位是ms
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
                m_Setting.logSetting.iMDLoggEnable = 0; //抓取modem log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.iMobileLogEnable = 0; //抓取mobile log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.IP =
                    @""; //"192.168.43.108";  //wifi 通信下要抓取modem、mobile log 就需要设定通信的ip 地址，目前这个地址在 SW 端是固定的。
                m_Setting.logSetting.logSavePath = logSavePath; ////log要保存的路径，运行生成的所有log都会保存在这个路径下面对应的线程号里
                m_Setting.logSetting.pcbSn = "PCB_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");  //这个一般设置SN+time，目的是为了保证所有的log都是相同的名字


                /****Filter Setting******/
                m_Setting.filterSetting.brom = BROMPID; //brom 方式做preloader handshake，那么需要配置，设定preloader filter即可
                m_Setting.filterSetting.preloader = PRELOADERPID; //
                m_Setting.filterSetting.kernel = KENERLPID; //



                //这个API会连接AP meta，并且会进行AP db 的匹配check
                result = METAAPP_ConnectTargetAllInOne_r(m_nDUTID, ref m_Setting, ref pCnf);
                if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                {
                    ErrorMsg = "Connect AP meta OK.";
                    Log.GetInstance().d("MTK New", "手机进入META状态\n" + ErrorMsg);
                }
                else
                {
                    ErrorMsg = "Connect AP meta Failed " + Convert.ToInt16(result);
                    Log.GetInstance().d("MTK New", "手机进入META状态FAIL\n" + ErrorMsg);
                    MessageBox.Show(ErrorMsg, "");
                }

                m_hMeta_New = pCnf.hMDHandle;
                //LogUtils.WriteLog(String.Format("DutID = {0}, MetaHandle = {1}.", m_nDutID, m_hMeta));

                //ap to modem
                //result = METAAPP_ApToModemAllInOne_r(0);
                //if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                //{
                //    ErrorMsg = "AP to Modem Meta OK.";
                //    Log.GetInstance().d("MTK New", "手机进入META状态结束\n" + ErrorMsg);
                //}
                //else
                //{
                //    ErrorMsg = "AP to Modem Meta Failed" + Convert.ToInt16(result);
                //    Log.GetInstance().d("MTK New", "手机进入META状态结束\n" + ErrorMsg);
                //    return -1;
                //}



                UInt32 value = 999, u4Addr = 0, u4value = 0;
                int ms = 0;

                ms = SP_META_WIFI_OPEN_r_New(m_hMeta_New, (UInt32)(15000));
                Log.GetInstance().d("MTK WiFi", "SP_META_WIFI_OPEN_r=" + ms.ToString());
                ms = SP_META_WiFi_GetChipVersion_r_New(m_hMeta_New, 18000, ref value);
                ms = SP_META_WiFi_readMCR32_r_New(m_hMeta_New, 1200, u4Addr, ref u4value);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_readMCR32_r=" + ms.ToString());
                m_wifiChipVersion = u4value & 0xFFFF;
                if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
                {
                }
                else
                {

                }
                if ((0x6620 == m_wifiChipVersion) | (0x6630 == m_wifiChipVersion))
                {
                    NVRAM_ACCESS_STRUCT wifinvram = new NVRAM_ACCESS_STRUCT();

                    byte[] pszBufferRaw = new byte[512];
                    pszBufferRaw[0] = (0x6628 >> 8) & 0x00ff;
                    pszBufferRaw[1] = 0x6628 & 0x00ff;

                    wifinvram.data = System.Text.Encoding.Default.GetString(pszBufferRaw);
                    wifinvram.dataLen = 2;
                    wifinvram.dataOffset = 0x3c;
                    SP_META_WiFi_WriteNVRAM_r(m_hMeta.md, 1200, ref wifinvram);
                }
                ENUM_CFG_SRC_TYPE_T buffType = new ENUM_CFG_SRC_TYPE_T();
                ms = SP_META_WiFi_QueryConfig_r_New(m_hMeta_New, 5000, ref buffType);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_QueryConfig_r=" + ms.ToString());
                ms = SP_META_WiFi_setTestMode_r_New(m_hMeta_New, 5000);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_setTestMode_r=" + ms.ToString());
                ms = SP_META_WiFi_switchAntenna_r_New(m_hMeta_New, 5000, 0);
                Log.GetInstance().d("MTK WiFi", "SP_META_WiFi_switchAntenna_r=" + ms.ToString());

                return 0;
            }
            catch (Exception ex)
            {
                Log.GetInstance().d("MTK New", ex.ToString());
                ErrorMsg = "Connect Meta Failed throw an exception" + ex.Message;
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
                m_Setting.bootSetting.authHandle = IntPtr.Zero; //security boot 相关设定，可以不设置, 当前MetaApp是不支持security boot
                m_Setting.bootSetting.scertHandle = IntPtr.Zero; //security boot 相关设定，可以不设置, 当前MetaApp是不支持security boot
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
                m_Setting.logSetting.iMDLoggEnable = 0; //抓取modem log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.iMobileLogEnable = 0; //抓取mobile log 到PC上，0：disable，1：USB logging to pc, 2: SD
                m_Setting.logSetting.IP =
                    @""; //"192.168.43.108";  //wifi 通信下要抓取modem、mobile log 就需要设定通信的ip 地址，目前这个地址在 SW 端是固定的。
                m_Setting.logSetting.logSavePath = logSavePath; ////log要保存的路径，运行生成的所有log都会保存在这个路径下面对应的线程号里
                m_Setting.logSetting.pcbSn = "PCB_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"); //这个一般设置SN+time，目的是为了保证所有的log都是相同的名字


                /****Filter Setting******/
                m_Setting.filterSetting.brom = BROMPID; //brom 方式做preloader handshake，那么需要配置，设定preloader filter即可
                m_Setting.filterSetting.preloader = PRELOADERPID; //
                m_Setting.filterSetting.kernel = KENERLPID; //



                //这个API会连接AP meta，并且会进行AP db 的匹配check

                result = METAAPP_ConnectTargetAllInOne_r(m_nDUTID, ref m_Setting, ref pCnf);
                if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                {
                    ErrorMsg = "Connect AP meta OK.\r\n";
                }
                else
                {
                    ErrorMsg = "Connect AP meta Failed ," + Convert.ToInt16(result);
                    //return -1;
                }

                m_hMeta_New = pCnf.hMDHandle;
                //LogUtils.WriteLog(String.Format("DutID = {0}, MetaHandle = {1}.", m_nDutID, m_hMeta));

                //ap to modem
                //result = METAAPP_ApToModemAllInOne_r(0);
                //if (result == METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS)
                //{
                //    ErrorMsg = "AP to Modem Meta OK.\r\n";
                //}
                //else
                //{
                //    ErrorMsg = "AP to Modem Meta Failed," + Convert.ToInt16(result);
                //    return -1;
                //}

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
        public bool Disconnect_New()
        {
            METAAPP_HIGH_LEVEL_RESULT result;
            bool bPowerOff = true;
            try
            {
                result = METAAPP_DisConnectMeta_r(m_nDutID, bPowerOff);
                if (result != METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_SUCCESS &&
                    result != METAAPP_HIGH_LEVEL_RESULT.METAAPP_HIGH_LEVEL_CLEAN_BOOT_FAIL)
                    //LogUtils.WriteLog(String.Format("Disconnect fail({0}).", result));

                    m_nDutID_New = 0;
                m_hMeta_New = 0;


                return true;
            }
            catch (Exception ex)
            {
                //LogUtils.WriteLog("Disconnect fail.exception:" + ex);
                return false;
            }
        }



        /// <summary>
        /// Get META_DLL.dll api result infomation string.
        /// </summary>
        /// <param name="ErrCode">the return value by other META_xxx apis.</param>
        /// <returns>infromation string</returns>
        [DllImport(metacore, EntryPoint = "SP_META_GetErrorString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static string SP_META_GetErrorString_New(int ErrCode);

        /// <summary>
        /// Disconnect from ap meta stage, and shutdown device.
        /// </summary>
        /// <param name="aphandle"></param>
        /// <returns></returns>
        [DllImport(metacore, EntryPoint = "SP_META_DisconnectWithTarget_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_DisconnectWithTarget_r_New(int aphandle);

        /// <summary>
        /// Disconnect from ap meta stage ( kip device in meta mode.)
        /// </summary>
        /// <param name="aphandle"></param>
        /// <returns></returns>
        [DllImport(metacore, EntryPoint = "SP_META_DisconnectInMetaMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_DisconnectInMetaMode_r_New(int aphandle);

        [DllImport(metacore, EntryPoint = "SP_META_GetTargetVerInfoV2_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_GetTargetVerInfoV2_r_New(int aphandle, ref VerInfo_V2_Cnf cnf, ref short token, IntPtr usrData);

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
        extern static int SP_META_SetCleanBootFlag_r_New(int aphandle, uint ms_timeout, ref SetCleanBootFlag_REQ req, ref SetCleanBootFlag_CNF cnf);
        
        [DllImport(metacore, EntryPoint = "SP_META_WiFi_GetChipVersion_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_GetChipVersion_r_New(int mdhandle, int ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_readMCR32_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_readMCR32_r_New(int mdhandle, UInt32 ms_timeout, UInt32 offset, ref UInt32 value);
        
        [DllImport(metacore, EntryPoint = "SP_META_WiFi_WriteNVRAM_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_WriteNVRAM_r_New(int mdhandle, UInt32 ms_timeout, ref NVRAM_ACCESS_STRUCT preq);
        
        [DllImport(metacore, EntryPoint = "SP_META_WiFi_QueryConfig_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_QueryConfig_r_New(int mdhandle, UInt32 ms_timeout, ref ENUM_CFG_SRC_TYPE_T buffType);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setTestMode_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setTestMode_r_New(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_switchAntenna_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_switchAntenna_r_New(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WIFI_OPEN_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WIFI_OPEN_r_New(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setChannel_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setChannel_r_New(int mdhandle, UInt32 ms_timeout, int channelfreq);

        [DllImport(metacore, EntryPoint = "SP_META_META_WiFi_setRate_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_META_WiFi_setRate_r_New(int mdhandle, UInt32 ms_timeout, UInt32 Rate);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setBandwidthEx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setBandwidthEx_r_New(int mdhandle, UInt32 ms_timeout, UInt32 nChBandwidth, UInt32 nDataBandwidth, UInt32 nPrimarySetting);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setGuardinterval_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setGuardinterval_r_New(int mdhandle, UInt32 ms_timeout, UInt32 inerval);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setBandwidth_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setBandwidth_r_New(int mdhandle, UInt32 ms_timeout, UInt32 BandWidth);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setModeSelect_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setModeSelect_r_New(int mdhandle, UInt32 ms_timeout, UInt32 Mode);
        
        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setPacketTxEx_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setPacketTxEx_r_New(int mdhandle, UInt32 ms_timeout, ref WIFI_TX_PARAM_T txparam);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setStandBy_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setStandBy_r_New(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setRxTest_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setRxTest_r_New(int mdhandle, UInt32 ms_timeout);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedErrorCount_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_ReceivedErrorCount_r_New(int mdhandle, UInt32 ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedOKCount_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_ReceivedOKCount_r_New(int mdhandle, UInt32 ms_timeout, ref UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setNss_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setNss_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setTXPath_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setTXPath_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_setRXPath_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_setRXPath_r(int mdhandle, UInt32 ms_timeout, UInt32 value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedRSSI_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_ReceivedRSSI_r_New(int mdhandle, UInt32 ms_timeout, ref short value);

        [DllImport(metacore, EntryPoint = "SP_META_WiFi_ReceivedRSSI1_r", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        extern static int SP_META_WiFi_ReceivedRSSI1_r_New(int mdhandle, UInt32 ms_timeout, ref short value);
        

        #endregion



        #endregion

        #region MTK AT

        private Control.SerialPortControl.MySerialport port;
       
        public void OpenPort(string portName)
        {
            port = new Control.SerialPortControl.MySerialport();
            port.PortName = portName;
            Log.GetInstance().d("WiFi AT", "PortName=" + portName);
            port.Open();
            
        }

        public void IniDut()
        {
            //Log.GetInstance().d("WiFi AT", "AT+BLWLAN=1即将发送");
            //port.WriteLine("AT+BLWLAN=1");
            //Log.GetInstance().d("WiFi AT", "AT+BLWLAN=1发送成功");
        }

        public void SetATTxParam(string Argument,string freq,string Rate,string enp)
        {
            string command = null;
            
            switch (Argument)
            {
                case "2G4_B":
                case "2G4_G":
                    command = "AT+BKWLAN=2," + Convert.ToDouble(freq).ToString("f2") + "," + Convert.ToDouble(Rate.Split('M')[0]).ToString("f2") + "," + Convert.ToDouble(enp).ToString("f2");
                    break;
                case "5G1_A":
                case "5G4_A":
                case "5G8_A":
                    command = "AT+BKWLAN=3," + Convert.ToDouble(freq).ToString("f2") + "," + Convert.ToDouble( Rate.Split('M')[0]).ToString("f2") + "," + Convert.ToDouble(enp).ToString("f2");
                    break;
                case "2G4_N20M":
                case "2G4_N40M":
                case "5G1_N20M":
                case "5G1_N40M":
                case "5G4_N20M":
                case "5G4_N40M":
                case "5G8_N20M":
                case "5G8_N40M":
                    Log.GetInstance().d("AT", Rate);
                    command = "AT+BKWLAN=3," + Convert.ToDouble(freq).ToString("f2") + "," +Convert.ToDouble(N_RateTransformation(Argument, Rate)).ToString("f2") + "," +Convert.ToDouble(enp).ToString("f2");
                    break;
            }
            //Log.GetInstance().d("AT", command);
            port.WriteLine(command);
            port.WriteLine(command);
        }

        private string N_RateTransformation(string Argument,string McsRate)
        {
            string rate = null;
            if (Argument.Contains("20M"))
            {
                switch (McsRate)
                {
                    case "MCS0":
                        rate = "6.5";
                        break;
                    case "MCS1":
                        rate = "13";
                        break;
                    case "MCS2":
                        rate = "19.5";
                        break;
                    case "MCS3":
                        rate = "26";
                        break;
                    case "MCS4":
                        rate = "39";
                        break;
                    case "MCS5":
                        rate = "52";
                        break;
                    case "MCS6":
                        rate = "58.5";
                        break;
                    case "MCS7":
                        rate = "65";
                        break;
                }
            }
            else if (Argument.Contains("40M"))
            {
                switch (McsRate.Split(',')[1])
                {
                    case "MCS0":
                        rate = "13.5";
                        break;
                    case "MCS1":
                        rate = "27";
                        break;
                    case "MCS2":
                        rate = "40.5";
                        break;
                    case "MCS3":
                        rate = "54";
                        break;
                    case "MCS4":
                        rate = "81";
                        break;
                    case "MCS5":
                        rate = "108";
                        break;
                    case "MCS6":
                        rate = "121.5";
                        break;
                    case "MCS7":
                        rate = "135";
                        break;
                }
            }
            return rate;
        }

        public void SetATRxParam(string Argument, string freq, string Rate)
        {
            string command = null;

            switch (Argument)
            {
                case "2G4_B":
                case "2G4_G":
                    command = "AT+BKWLAN=5," + freq + "," + N_RateTransformation(Argument, Rate);
                    break;
                case "2G4_N20M":
                case "2G4_N40M":
                case "5G1_A":
                case "5G4_A":
                case "5G8_A":
                case "5G1_N20M":
                case "5G1_N40M":
                case "5G4_N20M":
                case "5G4_N40M":
                case "5G8_N20M":
                case "5G8_N40M":
                    command = "AT+BKWLAN=5," + freq + "," + N_RateTransformation(Argument, Rate);
                    break;
            }
            //Log.GetInstance().d("AT", command);
            port.WriteLine(command);
        }

        public string GetATRxFrameCount()
        {
            //Log.GetInstance().d("AT", "AT+BKWLAN=6");
            string buff= port.Query("AT+BKWLAN=6");
            //Log.GetInstance().d("AT", buff);
            return buff;
        }

        public void ClosePort()
        {
            port.Close();
        }

        public int GetFrameCount()
        {

            return 0;
        }

        private string GetATArgument(string Argument)
        {
            return "0";
        }

        private string GetATRate(string Rate)
        {
            return "0";
        }

        #endregion

        #region Samsung
        /// <summary>
        /// 连接手机
        /// </summary>
        /// <param name="PhonePort">高通手机串口 Samsung手机设备号</param>
        /// <param name="ic">设备名</param>
        public void ConnectPhone(string adbPort, string ic)
        {
          
            Log.GetInstance().d("ConnectPhone", adbPort);
            try
            {
                if (ic == "Samsung")
                {
                  
                    int ipport = 0;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@"XmlConfig\SystemConfig.xml");
                    foreach (XmlElement xe in doc.GetElementsByTagName("page"))
                    {
                        foreach (XmlElement xec in xe.GetElementsByTagName("wifi"))
                        {
                            foreach (XmlElement xecc in xec.GetElementsByTagName("SamsungWlanPort"))
                            {
                                ipport= Convert.ToInt32(xecc.GetAttribute("port"));
                            }
                        }
                    }
                    OpenSamsungPort(adbPort, ipport);
                }
            }
            catch (PhoneException pex)
            {
                MessageBox.Show(pex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        private Samsungwlan samsungwlan;
        /// <summary>
        /// 打开Samsung串口
        /// </summary>
        /// <param name="adbResource">设备号</param>
        /// <param name="socketport">SamsungWlanPort（三星WLAN端口）</param>		 
        public void OpenSamsungPort(string adbResource, int socketport)
        {

            if (samsungwlan == null)
            {
                samsungwlan = new Samsungwlan(adbResource, socketport);
            }
            samsungwlan.adbresource = adbResource;
            samsungwlan.Open_WALN();
        }

        public void CloseSamsungWlan()
        {
            samsungwlan.Close_WLAN();
        }

        public void CloseAdbServer()
        {
            samsungwlan.DisposeAllResourse();
        }

        /// <summary>
        /// 设置Samsung AT命令
        /// </summary>
        /// <param name="Argument"></param>
        /// <param name="channel"></param>
        /// <param name="Rate"></param>
        /// <param name="SamsungChain">天线编号：1：上天线，2：下天线，3：MIMO测试双天线</param>
        /// <param name="enp"></param>
        public void SetSamsungATParam(string Argument, uint channel, string Rate,int SamsungChain, string enp="RX")
        {
            try					  
            { 
				Samsungwlan.SamsungWlanBandWidth bw = Samsungwlan.SamsungWlanBandWidth.b_g_a_n_20MHZ;
                Samsungwlan.AntMode antMode = Samsungwlan.AntMode.SISOFir;
                Samsungwlan.WlanBand wlanBand = Samsungwlan.WlanBand._2G4;
                Samsungwlan.Argument samsungargument = Samsungwlan.Argument.B;
                uint rateindex = 0;

                bw = Samsungwlan.SamsungWlanBandWidth.b_g_a_n_20MHZ;
                if (SamsungChain == 1)
                {
                    antMode = Samsungwlan.AntMode.SISOFir;
                }
                else if (SamsungChain == 2)
                {
                    antMode = Samsungwlan.AntMode.SISOSec;
                }
				//author 何苹 WiFi MIMO
                else if (SamsungChain == 3)
                {
                    antMode = Samsungwlan.AntMode.MIMO;
                }						 
                samsungargument = Samsungwlan.Argument.A;
                switch (Rate)
                {
                    case "1Mbit/s":
                        rateindex = 0;
                        break;
                    case "2Mbit/s":
                        rateindex = 1;
                        break;
                    case "5_5Mbit/s":
                        rateindex = 2;
                        break;
                    case "11Mbit/s":
                        rateindex = 3;
                        break;
                    case "6Mbit/s":
                        rateindex = 0;
                        break;
                    case "9Mbit/s":
                        rateindex = 1;
                        break;
                    case "12Mbit/s":
                        rateindex = 2;
                        break;
                    case "18Mbit/s":
                        rateindex = 3;
                        break;
                    case "24Mbit/s":
                        rateindex = 4;
                        break;
                    case "36Mbit/s":
                        rateindex = 5;
                        break;
                    case "48Mbit/s":
                        rateindex = 6;
                        break;
                    case "54Mbit/s":
                        rateindex = 7;
                        break;
                    case "MCS0":
                        rateindex = 0;
                        break;
                    case "MCS1":
                        rateindex = 1;
                        break;
                    case "MCS2":
                        rateindex = 2;
                        break;
                    case "MCS3":
                        rateindex = 3;
                        break;
                    case "MCS4":
                        rateindex = 4;
                        break;
                    case "MCS5":
                        rateindex = 5;
                        break;
                    case "MCS6":
                        rateindex = 6;
                        break;
                    case "MCS7":
                        rateindex = 7;
                        break;
                    case "MCS8":
                        rateindex = 8;
                        break;
                    case "MCS9":
                        rateindex = 9;
                        break;
					case "MCS10":			 
                        rateindex = 10;
                        break;
                    case "MCS11":
                        rateindex = 11;
                        break;
                    case "MCS12":
                        rateindex = 12;
                        break;
                    case "MCS13":
                        rateindex = 13;
                        break;
                    case "MCS14":
                        rateindex = 14;
                        break;
                    case "MCS15":
                        rateindex = 15;
                        break;
                    case "MCS16":
                        rateindex = 16;
                        break;
                    case "MCS17":
                        rateindex = 17;
                        break;
                    case "MCS18":
                        rateindex = 18;
                        break;
                    case "MCS19":
                        rateindex = 19;
                        break;		  					  
                }
                if ((Argument.Contains("AX") && rateindex > 11) || (Argument.Contains("AC") && rateindex > 9))
                {
                    rateindex = rateindex - 10;
                }

                if (Argument.Contains("5G"))
                {
                    wlanBand = Samsungwlan.WlanBand._5G;
                }
                else
                {
                    wlanBand = Samsungwlan.WlanBand._2G4;
                }
                Log.GetInstance().d("Samsung", Argument);
                
                switch (Argument)
                {
                    case "2G4_B":
                        bw = Samsungwlan.SamsungWlanBandWidth.b_g_a_n_20MHZ;
                        samsungargument = Samsungwlan.Argument.B;
                        break;
                    case "2G4_G":
                        bw = Samsungwlan.SamsungWlanBandWidth.b_g_a_n_20MHZ;
                        samsungargument = Samsungwlan.Argument.G;
                        break;
                    case "5G1_A":
                    case "5G4_A":
                    case "5G8_A":
                        bw = Samsungwlan.SamsungWlanBandWidth.b_g_a_n_20MHZ;
                        samsungargument = Samsungwlan.Argument.A;
                        break;
                    case "2G4_N20M":
                    case "2G4_N40M": //add hp samsung 
                    case "5G1_N20M":
                    case "5G1_N40M":
                    case "5G4_N20M":
                    case "5G4_N40M":
                    case "5G8_N20M":
                    case "5G8_N40M":
                        if (Argument.Contains("40M"))
                        {
                            bw = Samsungwlan.SamsungWlanBandWidth.n_40MHZ;
                        }
                        else
                        {
                            bw = Samsungwlan.SamsungWlanBandWidth.b_g_a_n_20MHZ;
                        }
                        if (Argument.Contains("5G"))
                        {
                            samsungargument = Samsungwlan.Argument.N_5G;
                        }
                        else
                        {
                            samsungargument = Samsungwlan.Argument.N_2G4;
                        }
                        break;
                    case "2G4_AC20M":
                    case "5G4_AC20M":
                    case "5G1_AC20M":
                    case "5G8_AC20M":
                    case "5G4_AC40M":
                    case "5G1_AC40M":
                    case "5G8_AC40M":
                    case "5G4_AC80M":
                    case "5G1_AC80M":
                    case "5G8_AC80M":
                        if (Argument.Contains("40M"))
                        {
                            bw = Samsungwlan.SamsungWlanBandWidth.ac_40MHZ;
                            samsungargument = Samsungwlan.Argument.AC_40M;
                        }
                        else if (Argument.Contains("20M"))
                        {
                            bw = Samsungwlan.SamsungWlanBandWidth.ac_20MHZ;
                            samsungargument = Samsungwlan.Argument.AC_20M;
                        }
                        else if (Argument.Contains("80M"))
                        {
                            bw = Samsungwlan.SamsungWlanBandWidth.ac_80MHZ;
                            samsungargument = Samsungwlan.Argument.AC_80M;
                        }
                        break;

                    case "2G4_AX20M":
                    case "5G4_AX20M":
                    case "5G1_AX20M":
                    case "5G8_AX20M":
                        bw = Samsungwlan.SamsungWlanBandWidth.ax_20MHZ;
                        samsungargument = Samsungwlan.Argument.AX_20M;
                        break;
                    case "2G4_AX40M":
                    case "5G4_AX40M":
                    case "5G1_AX40M":
                    case "5G8_AX40M":
                        bw = Samsungwlan.SamsungWlanBandWidth.ax_40MHZ;
                        samsungargument = Samsungwlan.Argument.AX_40M;
                        break;
                    case "5G4_AX80M":
                    case "5G1_AX80M":
                    case "5G8_AX80M":
                        bw = Samsungwlan.SamsungWlanBandWidth.ax_80MHZ;
                        samsungargument = Samsungwlan.Argument.AX_80M;
                        break;
                }
                Log.GetInstance().d("Samsung", bw.ToString());
                samsungwlan.SetBandWidth(bw);
                samsungwlan.SetAnt(antMode);
                samsungwlan.SetBand(wlanBand);
                samsungwlan.SetChannel(channel);              
                samsungwlan.SetDataRate(samsungargument, rateindex);                              
                if (enp != "RX")
                {
                    samsungwlan.SetEnp((float)Convert.ToDouble(enp));
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }

            
        }


        public void SetSamsungATOtherTX() {
            samsungwlan.SetOthorTx();
        }	 
        public void SetSamsungATStartTx()
        {
            samsungwlan.SetStartTx();
        }

        public void SetSamsungATStopTx()
        {
            samsungwlan.SetStopTx();
        }

        public void SetSamsungATStartRx()
        {
            samsungwlan.SetStartRx();
        }

        public void SetSamsungATStopRx()
        {
            samsungwlan.SetStopRx();
        }

        public int GetSamsungGoodPacket()
        {
            Dictionary<Samsungwlan.PacketType, int> packetdic = new Dictionary<Samsungwlan.PacketType, int>();
            packetdic=samsungwlan.GetRxPacket();
            Log.GetInstance().d("Samsung", "发报数1000" + "收包数" + packetdic[Samsungwlan.PacketType.goodPacket].ToString());
            return packetdic[Samsungwlan.PacketType.goodPacket];
        }

        public int GetSamsungErrorPacket()
        {
            Dictionary<Samsungwlan.PacketType, int> packetdic = new Dictionary<Samsungwlan.PacketType, int>();
            packetdic = samsungwlan.GetRxPacket();
            Log.GetInstance().d("Samsung", "发报数1000" + "错误包数" + packetdic[Samsungwlan.PacketType.errorPacket].ToString());
            return packetdic[Samsungwlan.PacketType.errorPacket];
        }

        public int GetSamsungRSSI()
        {
            return samsungwlan.GetRxRssi();
        }
        #endregion
    }

    public enum Vsg_Type
    {
        Sensitivity,
        MaxInputLevel,
        RSSI,
        WaterFall
    }

}
#endregion