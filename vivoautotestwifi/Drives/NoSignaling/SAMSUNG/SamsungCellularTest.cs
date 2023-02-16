using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Samsung = CellularNetwork.Platform.Samsung.SamsungATBase;
using CMW100 = RF_SDK.Instrument.CMW100;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using ChannelToFreq = vivo_Auto_Test.MainTestQueue.ChannelToFreq;
using System.Xml;

namespace vivo_Auto_Test.Drives.NoSignaling.SAMSUNG
{
    public class SamsungCellularTest
    {
        Samsung samsung;
        CMW100 cmw100;
        Control.GlobalCommunicate globalcommunicate;
        //MyInstrumentControl.AT_Control aT_Control;
        System.IO.Ports.SerialPort aT_Control;
        RF_Switch.RFSwitchImp RFSwitchImp;


        public static int Samsung_port = 1;

        private bool GlodEnable;

        public bool glodEnable
        {
            get => GlodEnable;
            set { GlodEnable = value; }
        }

        private MuliBoxType muliboxType;

        public MuliBoxType muliBoxType
        {
            get => muliboxType;
            set { muliboxType = value; }
        }

        private double loss;

        public double Loss
        {
            get => loss;
            set { loss = value; }
        }

        private bool SubTx;

        public bool subTx
        {
            get => SubTx;
            set { SubTx = value; }
        }

        private bool DrxEnable;

        public bool drxEnable
        {
            get => DrxEnable;
            set { DrxEnable = value; }
        }

        private bool MIMO1Enable;

        public bool mimo1Enable
        {
            get => MIMO1Enable;
            set { MIMO1Enable = value; }
        }

        private bool MIMO2Enable;

        public bool mimo2Enable
        {
            get => MIMO2Enable;
            set { MIMO2Enable = value; }
        }

        private uint DPDT_AntState;

        public uint dpdt_AntState
        {
            get => DPDT_AntState;
            set { DPDT_AntState = value; }
        }

        public bool Tx2_Prx = false;

        public bool Tx2_Drx = false;

        public int SRS_State
        {
            get;
            set;
        }

        public string[] BandWithMIMO
        {
            get;
            set;
        }

        public bool DownAntRxIsTest { get; set; }

        public string[] SubTxBand
        {
            get;
            set;
        }

        public string[] BandWithTx2Rx = null;

        public Func<int, string, int> GetPowerClass;

        public bool RxLevel_IsChecked = false;//是否测试电平


        public bool MaxPower_IsChecked = false;//是否测试最大功率


        public bool FreqErr_IsChecked = false;//是否测试频率误差


        public bool Sensitivity_IsChecked = false;//是否测试灵敏度

        private void CMW100_LogWrite(string command)
        {
            Log.GetInstance().d("log", command.Replace("\r"," ").Replace("\n"," "));
        }

        private void CMW100_Write(string command)
        {
            globalcommunicate.Write(command);
        }

        private string CMW100_Query(string command)
        {
            return globalcommunicate.Query(command);
        }


        public SamsungCellularTest(bool isOnlyDut = false)
        {
            MyInstrumentControl.AT_Control.logWrite = CMW100_LogWrite;
            CMW100.logWrite = CMW100_LogWrite;
            Samsung.logWrite = CMW100_LogWrite;

            cmw100 = CMW100.CreateLandlordProxy();
            RF_SDK.Control.Write = CMW100_Write;
            RF_SDK.Control.Query = CMW100_Query;
            if (!isOnlyDut)
            {
                globalcommunicate = new Control.GlobalCommunicate();
                globalcommunicate.Connect();
                globalcommunicate.delayTime = 0;
                string InstrumentName = globalcommunicate.ConnectDeviceInfo.DeviceType;
            }
        }

        public void TestInit()
        {
            //aT_Control = MyInstrumentControl.AT_Control.CreateLandlordProxy();
            //aT_Control.Open((uint)Samsung_port);
            aT_Control = new System.IO.Ports.SerialPort();
            aT_Control.PortName = string.Format("COM{0}", Samsung_port);
            aT_Control.ReadTimeout = 5000;

            samsung = Samsung.CreateLandlordProxy();
            //samsung.Write = aT_Control.Write;
            samsung.Write = (string command) =>
            {
                string buff = null;
                for (int j = 0; j < 3; j++)
                {
                    try
                    {
                        //Open(portbuff);
                        string readline = null;
                        Log.GetInstance().d("Info", string.Format("-->{0}", command));
                        if (aT_Control.IsOpen == false)
                        {
                            aT_Control.Close();
                            aT_Control.Open();
                        }
                        try
                        {
                            aT_Control.WriteLine(command);
                        }
                        catch
                        {
                            throw new Exception("命令超时");
                        }
                        do
                        {
                            if (aT_Control.IsOpen == false)
                            {
                                aT_Control.Close();
                                aT_Control.Open();
                            }
                            readline = aT_Control.ReadLine();
                            buff = buff + readline;
                            if (readline.Contains("OK")) break;
                        } while (readline.Contains("OK") == false || readline.Contains("ERROR") == false);
                        Log.GetInstance().d("Info", string.Format("<--{0}", buff.Replace("\r", " ")));
                        aT_Control.Close();
                        j = 3;
                    }
                    catch (Exception ex)
                    {
                        Log.GetInstance().d("Info", ex.ToString());
                        //i = 0;
                        Log.GetInstance().d("Info", string.Format("{0}命令连续{1}次超时", command, j + 1));
                    }
                }
                return buff;
            };
        }


        public void TestEnd()
        {
            aT_Control.Close();
        }
        
        public Dictionary<string, double> SamsungNR_Test(int Band, Samsung.NR_BandWidth bandWidth, uint channel)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            int dlchannel = ChannelToFreq.GetChannelDL("NR_Sub6", Band, (int)channel);
            int ulchannel = ChannelToFreq.GetChannelUL("NR_Sub6", Band, (int)channel);

            double dlfreqMHz = NR_SetSamsung_CenterFreq(bandWidth, (uint)dlchannel);
            double ulfreqMHz = NR_SetSamsung_CenterFreq(bandWidth, (uint)ulchannel);
            Test_ini(Band, bandWidth, dlfreqMHz, ulfreqMHz);

            List<uint> ants = new List<uint>();

            if (DPDT_AntState != 3) ants = new List<uint>() { DPDT_AntState };
            else ants = new List<uint>() { 0, 1 };

            foreach (uint ant in ants)
            {
                samsung.Dut_NR_S_SYNC((uint)Band, Samsung.NR_SYNC_ModType._QPSK, dlfreqMHz, bandWidth, Samsung.NR_AntType.PRX);
                Dictionary<string, double> resultTx = new Dictionary<string, double>();
              
                string anttype = null;
                switch (ant)
                {
                    case 0:
                        anttype = "上天线";
                        break;
                    case 1:
                        anttype = "下天线";
                        break;
                    case 2:
                        anttype = "自由切换";
                        break;
                }
                //samsung.Dut_Dpdt_S_Switch("6", (int)ant);
                //samsung.Dut_NR_SRS_S_Switch(SRS_State);
                int[] FDD_Band_Array = new int[] { 1, 2, 3, 5, 7, 8, 20, 28, 66, 70, 71, 74 };
                int[] TDD_Band_Array = new int[] { 38, 41, 50, 51, 77, 78, 79 };
                //if (TDD_Band_Array.Contains(Band))
                //{
                //    dlfreqMHz = dlfreqMHz - 0.005;
                //}

                if (GlodEnable & MaxPower_IsChecked) //线损
                {
                    NR_CalibratePowerTestLogic(Samsung.TxType.Main, Band, ulfreqMHz, dlfreqMHz, bandWidth, anttype, (int)ant);
                }

                if (SubTx & MaxPower_IsChecked && SubTxBand.Contains(Band.ToString()))
                {
                    NR_CalibratePowerTestLogic(Samsung.TxType.Sub, Band, ulfreqMHz, dlfreqMHz, bandWidth, anttype, (int)ant);
                }

                if (MaxPower_IsChecked)
                {
                    NR_TX_Test(Band, bandWidth, ulfreqMHz, dlfreqMHz, ref resultTx, anttype, (int)ant);

                    //foreach (KeyValuePair<string, double> kv in resultTx)
                    //{
                    //    result.Add(kv.Key, kv.Value);
                    //}
                }

                if (DownAntRxIsTest == false && ant == 1)
                { }
                else
                {
                    if (GlodEnable & (RxLevel_IsChecked || Sensitivity_IsChecked)) //线损
                    {
                        NR_CalibrateRxLevelTestLogic(Band, bandWidth, dlfreqMHz, anttype);
                    }
                    if (RxLevel_IsChecked || Sensitivity_IsChecked)
                    {
                        NR_RX_Test(Band, bandWidth, dlfreqMHz, ref result, anttype);
                    }
                }
                if (MaxPower_IsChecked)
                {
                    foreach (KeyValuePair<string, double> kv in resultTx)
                    {
                        result.Add(anttype + "_" + kv.Key, kv.Value);
                    }
                }
            }
            samsung.Dut_NR_S_TestEnd();
            return result;
        }

        private void Test_ini(int Band, Samsung.NR_BandWidth bandWidth, double dlfreqMHz, double ulfreqMHz)
        {
            cmw100.CB_Ver_NR_S_PreSetting(Band, Convert.ToInt16(bandWidth.ToString().Replace("_", "").Replace("M", "")));
            int bandwidthindex = 100;

            switch (bandWidth)
            {
                case Samsung.NR_BandWidth._10M:
                    bandwidthindex = 10;
                    break;
                case Samsung.NR_BandWidth._20M:
                    bandwidthindex = 20;
                    break;
                case Samsung.NR_BandWidth._50M:
                    bandwidthindex = 50;
                    break;
                case Samsung.NR_BandWidth._100M:
                    bandwidthindex = 100;
                    break;
                case Samsung.NR_BandWidth._40M:
                    bandwidthindex = 40;
                    break;
            }
            cmw100.CB_Ver_NR_S_Init(CMW100.MCS_Type.QPSK, bandwidthindex, Band, ulfreqMHz, dlfreqMHz, loss, loss);
            cmw100.CB_Ver_GPRF_Set("ON");
        }

        public Dictionary<string, double> SamsungNR_TestOnlyDut(int Band, Samsung.NR_BandWidth bandWidth, uint channel, Func<string, bool> isContinue)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "XmlConfig\\SystemConfig.xml");
            List<string> LTE_RBP_L_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("LTE_RBP_L_Channels")[0]).InnerText.Split(',').ToList();
            List<string> LTE_RBP_H_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("LTE_RBP_H_Channels")[0]).InnerText.Split(',').ToList();
            List<string> NR_RBP_L_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("NR_RBP_L_Channels")[0]).InnerText.Split(',').ToList();
            List<string> NR_RBP_H_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("NR_RBP_H_Channels")[0]).InnerText.Split(',').ToList();


            Dictionary<string, double> result = new Dictionary<string, double>();

            int dlchannel = ChannelToFreq.GetChannelDL("NR_Sub6", Band, (int)channel);
            int ulchannel = ChannelToFreq.GetChannelUL("NR_Sub6", Band, (int)channel);

            double dlfreqMHz = NR_SetSamsung_CenterFreq(bandWidth, (uint)dlchannel);
            double ulfreqMHz = NR_SetSamsung_CenterFreq(bandWidth, (uint)ulchannel);
            int bandwidthindex = 100;

            switch (bandWidth)
            {
                case Samsung.NR_BandWidth._10M:
                    bandwidthindex = 10;
                    break;
                case Samsung.NR_BandWidth._20M:
                    bandwidthindex = 20;
                    break;
                case Samsung.NR_BandWidth._50M:
                    bandwidthindex = 50;
                    break;
                case Samsung.NR_BandWidth._100M:
                    bandwidthindex = 100;
                    break;
                case Samsung.NR_BandWidth._40M:
                    bandwidthindex = 40;
                    break;
            }
            List<uint> ants = new List<uint>();

            if (DPDT_AntState != 3) ants = new List<uint>() { DPDT_AntState };
            else ants = new List<uint>() { 0, 1 };

            foreach (uint ant in ants)
            {
               
                Dictionary<string, double> resultTx = new Dictionary<string, double>();
          
                string anttype = null;
                switch (ant)
                {
                    case 0:
                        anttype = "上天线";
                        break;
                    case 1:
                        anttype = "下天线";
                        break;
                    case 2:
                        anttype = "自由切换";
                        break;
                }
                samsung.Dut_Dpdt_S_Switch("6", (int)ant);

                Func<string, string, string> GetInfo = (txtype, rbstart) => { return string.Format("5G NR N{0} {1} {2} {3} {4}Tx RBconfig=1/{5}配置完成，请检查CCM", Band, bandWidth, channel,anttype, txtype, rbstart); };
                if (MaxPower_IsChecked)
                {
                    uint rbs_dft = 0,rbs_cp=0;
                    //NR_TX_Test(Band, bandWidth, ulfreqMHz, dlfreqMHz, ref resultTx, anttype);
                    if (NR_RBP_L_Channel_List.Contains(channel.ToString()))
                    {
                        rbs_dft = 1;
                        rbs_cp = 0;
                    }
                    if (NR_RBP_H_Channel_List.Contains(channel.ToString()))
                    {
                        rbs_dft = 271;
                        rbs_cp = 272;
                    }
                    samsung.Dut_NR_S_NoSYNC_SetBand((uint)Band);
                    //modefied by wzt at 2021.07.30
                    //samsung.Dut_NR_S_SendTx(Samsung.TxType.Main,Samsung.NR_ModType._QPSK,
                    //                               ulfreqMHz,
                    //                               bandWidth,
                    //                               Samsung.SCS_Type._30KHz,
                    //                               rbs_dft,
                    //                               Samsung.OFDM_Type.DFT_S_OFDM,
                    //                               GetPowerClass(Band, "NR"));
                    result.Add(GetInfo(nameof(Samsung.TxType.Main), rbs_dft.ToString()), double.NaN);
                    if (isContinue(GetInfo(nameof(Samsung.TxType.Main), rbs_dft.ToString())) == false) return result;
                    if (SubTx & SubTxBand.Contains(Band.ToString()))
                    {
                        //modefied by wzt at 2021.07.30
                        //samsung.Dut_NR_S_SendTx(Samsung.TxType.Sub,
                        //                           Samsung.NR_ModType._QPSK,
                        //                           ulfreqMHz,
                        //                           bandWidth,
                        //                           Samsung.SCS_Type._30KHz,
                        //                           rbs_dft,
                        //                           Samsung.OFDM_Type.DFT_S_OFDM,
                        //                           GetPowerClass(Band, "NR"));
                        result.Add(GetInfo(nameof(Samsung.TxType.Sub), rbs_dft.ToString()), double.NaN);
                        if (isContinue(GetInfo(nameof(Samsung.TxType.Sub), rbs_dft.ToString())) == false) return result;
                    }
                    //modefied by wzt at 2021.07.30
                    //samsung.Dut_NR_S_SendTx(Samsung.TxType.Main, Samsung.NR_ModType._QPSK,
                    //                              ulfreqMHz,
                    //                              bandWidth,
                    //                              Samsung.SCS_Type._30KHz,
                    //                              rbs_cp,
                    //                              Samsung.OFDM_Type.CP_OFDM,
                    //                              GetPowerClass(Band, "NR"));
                    result.Add(GetInfo(nameof(Samsung.TxType.Main), rbs_cp.ToString()), double.NaN);
                    if (isContinue(GetInfo(nameof(Samsung.TxType.Main), rbs_cp.ToString())) == false) return result;

                    if (SubTx & SubTxBand.Contains(Band.ToString()))
                    {
                        //modefied by wzt at 2021.07.30
                        //samsung.Dut_NR_S_SendTx(Samsung.TxType.Sub,
                        //                           Samsung.NR_ModType._QPSK,
                        //                           ulfreqMHz,
                        //                           bandWidth,
                        //                           Samsung.SCS_Type._30KHz,
                        //                           rbs_cp,
                        //                           Samsung.OFDM_Type.CP_OFDM,
                        //                           GetPowerClass(Band, "NR"));
                        result.Add(GetInfo(nameof(Samsung.TxType.Sub), rbs_cp.ToString()), double.NaN);
                        if (isContinue(GetInfo(nameof(Samsung.TxType.Sub), rbs_cp.ToString())) == false) return result;
                    }
                }
              
                if (MaxPower_IsChecked)
                {
                    foreach (KeyValuePair<string, double> kv in resultTx)
                    {
                        result.Add(anttype + "_" + kv.Key, kv.Value);
                    }
                }

                //samsung.Dut_NR_S_TestEnd();
            }


            return result;
        }

        private void NR_CalibrateRxLevelTestLogic(int band, Samsung.NR_BandWidth bandWidth, double freqMHz,string anttype)
        {
            Dictionary<Samsung.NR_AntType, int> prefectPORT = new Dictionary<Samsung.NR_AntType, int>();
            Dictionary<string, double> temp = new Dictionary<string, double>();
            switch (muliboxType)
            {
                case MuliBoxType.COM1:
                case MuliBoxType.COM2:
                case MuliBoxType.COM3:
                case MuliBoxType.COM4:
                case MuliBoxType.COM5:
                case MuliBoxType.COM6:
                case MuliBoxType.COM7:
                case MuliBoxType.COM8:
                    //cmw100.CMW100_GPRF_SelectPort((int)muliboxType);
                    cmw100.CMW100_GPRF_Level(-50);
                    bool S = samsung.Dut_NR_S_RxRSSITest(band, bandWidth, Samsung.NR_AntType.ALL, freqMHz, out temp);
                    //Thread.Sleep(1000);
                    //samsung.Dut_NR_S_RxRSSITest(band,bandWidth, Samsung.NR_AntType.ALL, freqMHz, out temp);
                    //Thread.Sleep(1000);
                    //samsung.Dut_NR_S_RxRSSITest(band,bandWidth, Samsung.NR_AntType.ALL, freqMHz, out temp);
                    if (temp[Samsung.NR_AntType.PRX.ToString() + "_RSSI"] > -95) prefectPORT.Add(Samsung.NR_AntType.PRX, (int)muliboxType);
                    if (drxEnable == true && temp[Samsung.NR_AntType.DRX.ToString() + "_RSSI"] > -98) prefectPORT.Add(Samsung.NR_AntType.DRX, (int)muliboxType);
                    if (MIMO1Enable == true && BandWithMIMO.Contains(band.ToString()) && temp[Samsung.NR_AntType.MIMO1.ToString() + "_RSSI"] > -98) prefectPORT.Add(Samsung.NR_AntType.MIMO1, (int)muliboxType);
                    if (MIMO2Enable == true && BandWithMIMO.Contains(band.ToString()) && temp[Samsung.NR_AntType.MIMO2.ToString() + "_RSSI"] > -98) prefectPORT.Add(Samsung.NR_AntType.MIMO2, (int)muliboxType);
                    if (MIMO2Enable == true && BandWithTx2Rx.Contains(band.ToString()) && temp[Samsung.NR_AntType.Tx2_PRX.ToString() + "_RSSI"] > -98) prefectPORT.Add(Samsung.NR_AntType.Tx2_PRX, (int)muliboxType);
                    if (MIMO2Enable == true && BandWithTx2Rx.Contains(band.ToString()) && temp[Samsung.NR_AntType.Tx2_DRX.ToString() + "_RSSI"] > -98) prefectPORT.Add(Samsung.NR_AntType.Tx2_DRX, (int)muliboxType);
                    break;
                case MuliBoxType.TESCOM16:
                    cmw100.CMW100_GPRF_Level(-50);
                    prefectPORT = NR_TestComRx(band, freqMHz, bandWidth);
                    break;
                default:
                    cmw100.CMW100_GPRF_Level(-50);
                    MuliBox.SwitchPerfectAnt(ref RFSwitchImp, muliboxType, (rfport) =>
                    {
                        cmw100.CMW100_GPRF_SelectPort(Convert.ToInt16(rfport));
                    }, 
                    () =>
                    {
                        samsung.Dut_NR_S_RxRSSITest(band, bandWidth, Samsung.NR_AntType.ALL, freqMHz, out temp);
                        return temp;
                    });

                    List<double> PRX_rscplist = new List<double>();
                    List<double> DRX_rscplist = new List<double>();
                    List<double> MIMO1_rscplist = new List<double>();
                    List<double> MIMO2_rscplist = new List<double>();
                    List<double> Tx2DRX_rscplist = new List<double>();
                    List<double> Tx2PRX_rscplist = new List<double>();
                    
                    if (PRX_rscplist.Max() > -98) prefectPORT.Add(Samsung.NR_AntType.PRX, PRX_rscplist.IndexOf(PRX_rscplist.Max()) + 1);
                    if (DrxEnable && DRX_rscplist.Max() > -98) prefectPORT.Add(Samsung.NR_AntType.DRX, DRX_rscplist.IndexOf(DRX_rscplist.Max()) + 1);
                    if (MIMO1Enable && BandWithMIMO.Contains(band.ToString()) && MIMO1_rscplist.Max() > -98) prefectPORT.Add(Samsung.NR_AntType.MIMO1, MIMO1_rscplist.IndexOf(MIMO1_rscplist.Max()) + 1);
                    if (MIMO2Enable && BandWithMIMO.Contains(band.ToString()) && MIMO2_rscplist.Max() > -98) prefectPORT.Add(Samsung.NR_AntType.MIMO2, MIMO2_rscplist.IndexOf(MIMO2_rscplist.Max()) + 1);
                    if (Tx2_Prx & BandWithTx2Rx.Contains(band.ToString()) && Tx2PRX_rscplist.Max() > -98)
                    {
                        Log.GetInstance().d("Debug", Tx2PRX_rscplist.ToString());
                        prefectPORT.Add(Samsung.NR_AntType.Tx2_PRX, Tx2PRX_rscplist.IndexOf(Tx2PRX_rscplist.Max()) + 1);
                    }

                    if (Tx2_Drx & BandWithTx2Rx.Contains(band.ToString()) && Tx2DRX_rscplist.Max() > -98) prefectPORT.Add(Samsung.NR_AntType.Tx2_DRX, Tx2DRX_rscplist.IndexOf(Tx2DRX_rscplist.Max()) + 1);

                    //throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
                    break;
            }

            foreach (KeyValuePair<Samsung.NR_AntType, int> kv in prefectPORT)
            {
                NR_CalibrateLevel(band, kv, loss, freqMHz, bandWidth, anttype);
            }
        }
        
        private void NR_CalibratePowerTestLogic(Samsung.TxType txType, int Band,double ulfreqMHz,double dlfreqMHz,Samsung.NR_BandWidth bandWidth,string anttype,int antnum)
        {
            double losstempmian = 0.0;
            TxTestSumsungNR(txType, Band, ulfreqMHz, dlfreqMHz, bandWidth, antnum);

            if (GlodEnable)
            {
                int port = 0;
                switch (muliboxType)
                {
                    case MuliBoxType.COM1:
                    case MuliBoxType.COM2:
                    case MuliBoxType.COM3:
                    case MuliBoxType.COM4:
                    case MuliBoxType.COM5:
                    case MuliBoxType.COM6:
                    case MuliBoxType.COM7:
                    case MuliBoxType.COM8:
                        port = (int)muliboxType;
                        break;
                    case MuliBoxType.TESCOM16:
                        port = NR_TestComTx();
                        break;
                    default:
                        port = MuliBox.SwitchPerfectAnt(ref RFSwitchImp, muliBoxType, (rfport) => { cmw100.CMW100_SelectMeasPort(rfport); }, () =>
                        {
                            Dictionary<string, double> result = new Dictionary<string, double>();
                            cmw100.CB_Ver_NR_S_Tx_Test(CMW100.MCS_Type.QPSK, out result);
                            return result["MaxPow"];
                        });
                        //throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
                        break;
                }

                losstempmian = NR_CalibratePower(port, loss, Band, bandWidth, ulfreqMHz, dlfreqMHz, txType, anttype, antnum);

                if (losstempmian == -1) NR_CalibratePowerTestLogic(txType, Band, ulfreqMHz, dlfreqMHz, bandWidth, anttype, antnum);
            }
        }

        private void TxTestSumsungNR(Samsung.TxType txType, int Band, double ulfreqMHz, double dlfreqMHz, Samsung.NR_BandWidth bandWidth,int antnum)
        {
             cmw100.CMW100_GPRF_Level(-50);
            //samsung.Dut_NR_S_TestEnd();
            //samsung.Dut_NR_S_SYNC((uint)Band, Samsung.NR_SYNC_ModType._QPSK, dlfreqMHz, bandWidth, Samsung.NR_AntType.PRX);
            //samsung.Dut_Dpdt_S_Switch("6", antnum);
            //modefied by wzt at 2021.07.30
            samsung.Dut_NR_S_SendTx(Samsung.NR_SYNC_ModType._QPSK,dlfreqMHz,bandWidth, Samsung.NR_AntType.PRX,2/*tx测试只需要测试主集*/,
                    txType,
                    Samsung.NR_ModType._QPSK,
                    ulfreqMHz,
                    Samsung.SCS_Type._30KHz,
                    1,
                    Samsung.OFDM_Type.DFT_S_OFDM,
                    GetPowerClass(Band, "NR"));
            samsung.Dut_Dpdt_S_Switch("6", antnum);

        }

        private int NR_TestComTx()
        {
            List<double> powerlist = new List<double>();
            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                cmw100.CMW100_SelectMeasPort(cmw100_portindex);
                Dictionary<string, double> result = new Dictionary<string, double>();
                cmw100.CB_Ver_NR_S_Tx_Test(CMW100.MCS_Type.QPSK, out result);
                powerlist.Add(result["MaxPow"]);
            }
            Log.GetInstance().d("TxPowerList", JsonConvert.SerializeObject(powerlist));
            int prefectport = powerlist.IndexOf(powerlist.Max())+1;
            SwitchRFPort(prefectport);
            return prefectport;
        }
        
        private Dictionary<Samsung.NR_AntType,int> NR_TestComRx(int band, double freqMHz,Samsung.NR_BandWidth bandWidth)
        {
            List<double> PRX_rscplist = new List<double>();
            List<double> DRX_rscplist = new List<double>();
            List<double> MIMO1_rscplist = new List<double>();
            List<double> MIMO2_rscplist = new List<double>();
            List<double> Tx2DRX_rscplist = new List<double>();
            List<double> Tx2PRX_rscplist = new List<double>();

            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                cmw100.CMW100_GPRF_SelectPort(cmw100_portindex);
                Dictionary<string, double> result = new Dictionary<string, double>();
                List<Samsung.NR_AntType> anttype_list = new List<Samsung.NR_AntType>() { Samsung.NR_AntType.PRX };
                if (drxEnable) anttype_list.Add(Samsung.NR_AntType.DRX);
                if (MIMO1Enable & BandWithMIMO.Contains(band.ToString())) anttype_list.Add(Samsung.NR_AntType.MIMO1);
                if (MIMO2Enable & BandWithMIMO.Contains(band.ToString())) anttype_list.Add(Samsung.NR_AntType.MIMO2);
                if (Tx2_Prx & BandWithTx2Rx.Contains(band.ToString())) anttype_list.Add(Samsung.NR_AntType.Tx2_PRX);
                if (Tx2_Drx & BandWithTx2Rx.Contains(band.ToString())) anttype_list.Add(Samsung.NR_AntType.Tx2_DRX);
                Log.GetInstance().d("Debug", JsonConvert.SerializeObject(anttype_list));
                foreach (Samsung.NR_AntType nR_AntType in anttype_list)
                {
                    samsung.Dut_NR_S_RxRSSITest(band, bandWidth, nR_AntType, freqMHz, out result);
                    if (nR_AntType== Samsung.NR_AntType.PRX) PRX_rscplist.Add(result[Samsung.NR_AntType.PRX + "_RSSI"]);
                    if (nR_AntType == Samsung.NR_AntType.DRX) DRX_rscplist.Add(result[Samsung.NR_AntType.DRX + "_RSSI"]);
                    if (nR_AntType == Samsung.NR_AntType.MIMO1) MIMO1_rscplist.Add(result[Samsung.NR_AntType.MIMO1 + "_RSSI"]);
                    if (nR_AntType == Samsung.NR_AntType.MIMO2) MIMO2_rscplist.Add(result[Samsung.NR_AntType.MIMO2 + "_RSSI"]);
                    if (nR_AntType == Samsung.NR_AntType.Tx2_PRX)
                    {
                        Log.GetInstance().d("Debug", result[Samsung.NR_AntType.Tx2_PRX + "_RSSI"].ToString());
                        Tx2PRX_rscplist.Add(result[Samsung.NR_AntType.Tx2_PRX + "_RSSI"]);
                    }
                    if (nR_AntType == Samsung.NR_AntType.Tx2_DRX) Tx2DRX_rscplist.Add(result[Samsung.NR_AntType.Tx2_DRX + "_RSSI"]);
                }
            }
            Log.GetInstance().d("debug", JsonConvert.SerializeObject(PRX_rscplist));
            Log.GetInstance().d("debug", JsonConvert.SerializeObject(DRX_rscplist));
            Log.GetInstance().d("debug", JsonConvert.SerializeObject(MIMO1_rscplist));
            Log.GetInstance().d("debug", JsonConvert.SerializeObject(MIMO2_rscplist));
            Dictionary<Samsung.NR_AntType, int> prefectPort = new Dictionary<Samsung.NR_AntType, int>();
            if (PRX_rscplist.Max() > -98) prefectPort.Add(Samsung.NR_AntType.PRX, PRX_rscplist.IndexOf(PRX_rscplist.Max())+1);
            if (DrxEnable && DRX_rscplist.Max() > -98) prefectPort.Add(Samsung.NR_AntType.DRX, DRX_rscplist.IndexOf(DRX_rscplist.Max())+1);
            if (MIMO1Enable&&BandWithMIMO.Contains(band.ToString()) && MIMO1_rscplist.Max() > -98) prefectPort.Add(Samsung.NR_AntType.MIMO1, MIMO1_rscplist.IndexOf(MIMO1_rscplist.Max())+1);
            if (MIMO2Enable&&BandWithMIMO.Contains(band.ToString()) && MIMO2_rscplist.Max() > -98) prefectPort.Add(Samsung.NR_AntType.MIMO2, MIMO2_rscplist.IndexOf(MIMO2_rscplist.Max())+1);
            if (Tx2_Prx & BandWithTx2Rx.Contains(band.ToString()) && Tx2PRX_rscplist.Max() > -98)
            {
                Log.GetInstance().d("Debug", Tx2PRX_rscplist.ToString());
                prefectPort.Add(Samsung.NR_AntType.Tx2_PRX, Tx2PRX_rscplist.IndexOf(Tx2PRX_rscplist.Max()) + 1);
            }

            if (Tx2_Drx & BandWithTx2Rx.Contains(band.ToString())&& Tx2DRX_rscplist.Max()>-98) prefectPort.Add(Samsung.NR_AntType.Tx2_DRX, Tx2DRX_rscplist.IndexOf(Tx2DRX_rscplist.Max()) + 1);
            return prefectPort;
        }

        private void SwitchRFPort(int rfport)
        {
            switch (muliboxType)
            {
                case MuliBoxType.COM1:
                case MuliBoxType.COM2:
                case MuliBoxType.COM3:
                case MuliBoxType.COM4:
                case MuliBoxType.COM5:
                case MuliBoxType.COM6:
                case MuliBoxType.COM7:
                case MuliBoxType.COM8:
                case MuliBoxType.TESCOM16:
                    cmw100.CMW100_GPRF_SelectPort(rfport);
                    break;
                default:
                    RFSwitchImp.Open();
                    RFSwitchImp.Switch(rfport);
                    RFSwitchImp.Close();
                    break;
            }
        }

        private KeyValuePair<Samsung.NR_AntType, double> NR_CalibrateLevel(int band, KeyValuePair<Samsung.NR_AntType, int> prefectport, double startloss, double freqMHz, Samsung.NR_BandWidth bandWidth,string anttype)
        {

            SwitchRFPort(prefectport.Value);
            double ulloss = 0.0;
            double rscp = 0.0;
            int loop = 10;

            do
            {
                cmw100.CMW100_GPRF_Level(-50);
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.NR, ref ulloss, ref startloss);
                cmw100.CMW100_GPRF_Level(-50);
                Dictionary<string, double> result = new Dictionary<string, double>();
                int testloopindex = 10;
                do
                {
                    Thread.Sleep(200);
                    samsung.Dut_NR_S_RxRSSITest(band, bandWidth, prefectport.Key, freqMHz, out result);
                    testloopindex--;
                    if (testloopindex == 0 & result.Count == 0) break;
                } while (result.Count == 0);

                if (testloopindex == 0)
                {
                    throw new Exception(string.Format("NR的{0}接收电平无法测量", freqMHz));
                }
                rscp = result[prefectport.Key.ToString() + "_RSSI"];
                
                Log.GetInstance().d(prefectport.Key.ToString(), rscp.ToString());
                if (rscp >= -49.5 || rscp <= -50.5)
                {
                    startloss = startloss - (rscp + 50);
                    if (startloss > 38)
                    {
                        Log.GetInstance().d("Samsung NR", string.Format("NR的 {0} 接收路损大于 {1}", freqMHz, startloss));
                        startloss = 25;
                    }
                    else if (startloss < 0)
                    {
                        Log.GetInstance().d("Samsung NR", string.Format("NR的 {0} 接收路损小于 {1}", freqMHz, 0));
                        startloss = 25;
                    }
                    
                }
                loop--;
                if (loop == 0)
                {
                    if (startloss > 38)
                    {
                        startloss = 38;
                    }
                    else if (startloss < 0)
                    {
                        startloss = 25;
                    }
                    break;
                }
            } while (rscp > -49.5 || rscp < -50.5);

            KeyValuePair<Samsung.NR_AntType, double> prefectLoss = new KeyValuePair<Samsung.NR_AntType, double>(prefectport.Key, startloss);

            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                sw.WriteLine(string.Format("%NR\tAntState:{0}\tDownLink\tCMW100_PORT:{1}\tBand:{2}\tBandWidth:{3}\tFreqMHz:{4}%\t{5}", anttype, prefectport.Value, band, bandWidth.ToString(), freqMHz, JsonConvert.SerializeObject(prefectLoss)));
            }

            return prefectLoss;
        }

        private double NR_CalibratePower(int port, double startloss, int band, Samsung.NR_BandWidth bandWidth, double freqMHz, double dlfreqMhz, Samsung.TxType txType, string anttype, int antumn)
        {
            double dlloss = 25;

            Dictionary<string, double> result = new Dictionary<string, double>();
            int loop = 10;
            double remarkpower = (double)GetPowerClass(band, "NR");
            
            SwitchRFPort(port);
            cmw100.CMW100_SelectMeasPort(port);

            do
            {
                TxTestSumsungNR(txType, band, freqMHz, dlfreqMhz, bandWidth, antumn);

                cmw100.CB_Ver_Set_Cellpower(-50);
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.NR, ref startloss, ref dlloss);
                Thread.Sleep(200);
                cmw100.CB_Ver_NR_S_Tx_Test(CMW100.MCS_Type.QPSK, out result);

                if (result["MaxPow"] > remarkpower + 0.5 || result["MaxPow"] < remarkpower - 0.5)
                {
                    startloss = startloss - (result["MaxPow"] - remarkpower);
                }
                Log.GetInstance().d("Tip", string.Format("Loss={0} MaxPower={1} Loop={2}", startloss, result["MaxPow"], loop));

                loop--;
                if (startloss < 0 || startloss > 60)
                {
                    System.Windows.MessageBox.Show("线损>60或线损<0", "提示");
                    return -1;
                }
                if (loop == 0) return startloss;
            } while (result["MaxPow"] > remarkpower + 0.5 || result["MaxPow"] < remarkpower - 0.5);
            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                sw.WriteLine(string.Format("%NR\tAntState:{0}\tUpLink:{1}\tCMW100_PORT:{2}\tBand:{3}\tBandWidth:{4}\tFreqMHz:{5}%\t{6}", anttype, txType, port, band, bandWidth.ToString(), freqMHz, startloss));
            }
            return startloss;
        }

        private void NR_RX_Test(int Band, Samsung.NR_BandWidth bandWidth, double freqMHz, ref Dictionary<string, double> result, string anttype)
        {
            int rePort = 0;
            double reloss = 0.0;
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    //if (readline == null) continue;
                    if (readline == "" || readline == null) break;
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');
                    //double s = Convert.ToDouble(testinfos[4].Split(':')[1]);
                    if (readline.Contains("DownLink")
                        && readline.Contains(string.Format("Band:{0}", Band))
                        && readline.Contains(string.Format("BandWidth:{0}", bandWidth.ToString()))
                        && readline.Contains("NR") && readline.Contains(anttype)
                        && Convert.ToDouble(testinfos[6].Split(':')[1]) == freqMHz)
                    {
                        Dictionary<string, double> resultRx = new Dictionary<string, double>();

                        rePort = Convert.ToInt16(testinfos[3].Split(':')[1]);
                        SwitchRFPort(rePort);

                        KeyValuePair<Samsung.NR_AntType, double> prefectloss = JsonConvert.DeserializeObject<KeyValuePair<Samsung.NR_AntType, double>>(readarray.Last());
                        reloss = prefectloss.Value;
                        double temploss = 0.0;

                        cmw100.CMW100_GPRF_Level(-50);
                        cmw100.CB_Ver_Set_Loss(CMW100.Technology.NR, ref temploss, ref reloss);
                        cmw100.CMW100_GPRF_Level(-50);
                        if (RxLevel_IsChecked)
                        {
                            samsung.Dut_NR_S_RxRSSITest(Band, bandWidth, prefectloss.Key, freqMHz, out resultRx);

                            foreach (KeyValuePair<string, double> kv in resultRx)
                            {
                                result.Add(kv.Key + "_" + anttype + "RSSI", kv.Value);
                            }
                        }
                        if (Sensitivity_IsChecked)
                        {
                            bool failed = false;
                            double power = -50;
                            double powerstart = -50;
                            double powerend = -100;
                            do
                            {
                                Log.GetInstance().d("Tip", anttype);
                                if (power > -50)
                                {
                                    power = 999999;
                                    break;
                                }
                                
                                SwitchRFPort(rePort);
                                cmw100.CB_Ver_Set_Loss(CMW100.Technology.NR, ref temploss, ref reloss);

                                Log.GetInstance().d("Samsung", power.ToString());
                                cmw100.CMW100_GPRF_Level(power);
                                samsung.Dut_NR_S_RxTest(Band, freqMHz, bandWidth, prefectloss.Key, out resultRx);
                                if (resultRx.ToList()[0].Value >= 95)
                                {
                                    if (failed == true) break;
                                    powerstart = power;
                                    if (Math.Abs(power - powerend) < 1 || power <= -100)
                                    {
                                        power = power - 1;
                                    }
                                    else
                                    {
                                        power = (power + powerend) / 2;
                                    }
                                }
                                else
                                {
                                    power = power + 1;
                                    failed = true;
                                }
                            } while (true);
                            foreach (KeyValuePair<string, double> kv in resultRx)
                            {
                                result.Add(prefectloss.Key + "_" + anttype + "_Sen".ToString(), power);
                            }
                        }
                    }
                } while (true);
            }
        }

        private void NR_TX_Test(int Band, Samsung.NR_BandWidth bandWidth, double ulfreqMHz, double dlfreqMhz, ref Dictionary<string, double> result, string anttype,int antnum)
        {
            int rePort = 0;
            double reloss = 0.0;

            Samsung.TxType txType = Samsung.TxType.Main;
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    Log.GetInstance().d("Samsung", readline);
                    if (readline == null || readline == "") break;
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');
                    if (readline.Contains("UpLink")
                        && readline.Contains("NR") && readline.Contains(anttype)
                        && readline.Contains(string.Format("Band:{0}", Band))
                        && readline.Contains(string.Format("BandWidth:{0}", bandWidth.ToString()))
                        && Convert.ToDouble(testinfos[6].Split(':')[1]) == ulfreqMHz)
                    {
                        txType = testinfos[2].Contains("Main") ? Samsung.TxType.Main : Samsung.TxType.Sub;
                        rePort = Convert.ToInt16(testinfos[3].Split(':')[1]);
                        reloss = Convert.ToDouble(readarray.Last().Replace("\t", ""));
                        double temploss = 25;
                        Dictionary<string, double> resultTx = new Dictionary<string, double>();
                        SwitchRFPort(rePort);
                        
                        //Test_ini(Band, bandWidth, dlfreqMhz, ulfreqMHz);

                        cmw100.CMW100_SelectMeasPort(rePort);									 
                        cmw100.CB_Ver_Set_Loss(CMW100.Technology.NR, ref reloss, ref temploss);
                        uint loop = 10;
                        cmw100.CB_Ver_Set_Cellpower(-50);
                        do
                        {
                            Log.GetInstance().d("INFO", string.Format("第{0}次测试功率", 10 - loop+1));
                            TxTestSumsungNR(txType, Band, ulfreqMHz, dlfreqMhz, bandWidth, antnum);
                            Thread.Sleep(200);
                            cmw100.CB_Ver_NR_S_Tx_Test(CMW100.MCS_Type.QPSK, out resultTx);
                            loop--;
                            if (loop == 0) break;
                           
                        } while (resultTx["MaxPow"] < (double)GetPowerClass(Band,"NR")-3 || resultTx["MaxPow"] > (double)GetPowerClass(Band, "NR") +3);

                        foreach (KeyValuePair<string, double> kv in resultTx)
                        {
                            result.Add(txType.ToString() + string.Format("_{0}_", anttype) + kv.Key.ToString(), kv.Value);
                        }
                    }
                } while (true);
            }
        }
        
        private double NR_SetSamsung_CenterFreq(Samsung.NR_BandWidth BandWidth, uint channel, double offset = 0.0)
        {
            double freqMHz = 0.0;

            if (channel >= 600000)
            {
                freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
            }
            else
            {
                freqMHz = ((int)channel) * 0.005 + 0 + offset;
            }

            switch (BandWidth)
            {
                case Samsung.NR_BandWidth._10M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 9.54;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 9.54;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;

                case Samsung.NR_BandWidth._20M:
                    if (channel >= 600000)
                    {
                       
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 9.18 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 9.18 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._50M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 23.94 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 23.94 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._100M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 49.14 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 49.14 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._40M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 49.14 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 49.14 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
            }
            return freqMHz;
        }
        
        public Dictionary<string,double> SamsungLTE_Test(int Band,int channel, Samsung.LTE_BandWidth bandWidth= Samsung.LTE_BandWidth._10MHz)
        {
            
            float ULfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelUL_FREQ("LTE", Band, channel);
            float DLfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelDL_FREQ("LTE", Band, channel);

            Dictionary<string, double> result = new Dictionary<string, double>();
            LTE_ini(Band, ULfreqMHz, DLfreqMHz);
            samsung.Dut_LTE_S_TestEnd();
            //switch (bandWidth)
            //{
            //    case Samsung.LTE_BandWidth._1p4MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._3MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._5MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._10MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._15MHz:
            //        //bandwidthindex = 100;
            //        break;
            //    case Samsung.LTE_BandWidth._20MHz:
            //        //bandwidthindex = 100;
            //        break;
            //}
            List<uint> ants = new List<uint>();
            if (DPDT_AntState != 3) ants = new List<uint>() { DPDT_AntState };
            else ants = new List<uint>() { 0, 1 };
            foreach (uint ant in ants)
            {
                Dictionary<string, double> resultTx = new Dictionary<string, double>();
                Dictionary<string, double> resultRx = new Dictionary<string, double>();
                //DPDT_AntState = ant;
                string anttype = null;
                switch (ant)
                {
                    case 0:
                        anttype = "上天线";
                        break;
                    case 1:
                        anttype = "下天线";
                        break;
                    case 2:
                        anttype = "自由切换";
                        break;
                }

                samsung.Dut_Dpdt_S_Switch("2", (int)ant);
                samsung.Dut_LTE_S_SYNC(Samsung.LTE_AntType.PRX, Band, (float)DLfreqMHz);

                if (GlodEnable)
                {
                    //cmw100.CB_Ver_FDDLTE_S_Init(Band, ULfreqMHz, 25, 1, 0, DLfreqMHz, 25, CMW100.MCS_Type.QPSK, RF_SDK.bandWidth.BW_10MHz);
                    LTE_CalibratePowerTestLogic(Band, ULfreqMHz, DLfreqMHz, bandWidth, ant);
                }
                
                LTE_TX_Test(Band, bandWidth, ULfreqMHz, DLfreqMHz,(int)ant, ref resultTx);
                if (ant == 0)
                {
                    if (GlodEnable)
                    {
                        LTE_CalibrateRxLevelTestLogic(Band, bandWidth, DLfreqMHz);
                    }

                    LTE_RX_Test(Band, bandWidth, DLfreqMHz, ref resultRx);
                    foreach (KeyValuePair<string, double> kv in resultRx)
                    {
                        result.Add(kv.Key, kv.Value);
                    }
                }
                foreach (KeyValuePair<string, double> kv in resultTx)
                {
                    result.Add(kv.Key, kv.Value);
                }
            }
            samsung.Dut_LTE_S_TestEnd();
            return result;
        }

        private void LTE_ini(int Band, float ULfreqMHz, float DLfreqMHz)
        {
            if (Band > 32)
            {
                cmw100.CB_Ver_TDDLTE_S_PreSetting();
                cmw100.CB_Ver_FDDLTE_S_Init(Band, ULfreqMHz, loss, 1, 0, DLfreqMHz, loss, CMW100.MCS_Type.QPSK, /*(RF_SDK.bandWidth)bandWidth*/RF_SDK.bandWidth.BW_10MHz);
            }

            else {

                cmw100.CB_Ver_FDDLTE_S_PreSetting();
                cmw100.CB_Ver_FDDLTE_S_Init(Band, ULfreqMHz, loss, 1, 0, DLfreqMHz, loss, CMW100.MCS_Type.QPSK, /*(RF_SDK.bandWidth)bandWidth*/RF_SDK.bandWidth.BW_10MHz);
            }
            
            cmw100.CB_Ver_GPRF_Set("ON");
        }

        public Dictionary<string, double> SamsungLTE_TestOnlyDut(Func<string, bool> isContinue,int Band, int channel, Samsung.LTE_BandWidth bandWidth = Samsung.LTE_BandWidth._10MHz)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "XmlConfig\\SystemConfig.xml");
            List<string> LTE_RBP_L_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("LTE_RBP_L_Channels")[0]).InnerText.Split(',').ToList();
            List<string> LTE_RBP_H_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("LTE_RBP_H_Channels")[0]).InnerText.Split(',').ToList();
            List<string> NR_RBP_L_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("NR_RBP_L_Channels")[0]).InnerText.Split(',').ToList();
            List<string> NR_RBP_H_Channel_List = ((XmlElement)((XmlElement)((XmlElement)doc.DocumentElement.GetElementsByTagName("NoSignaling")[0]).GetElementsByTagName("CCM_INTERFER")[0]).GetElementsByTagName("NR_RBP_H_Channels")[0]).InnerText.Split(',').ToList();


            Dictionary<string, double> resultTx = new Dictionary<string, double>();
            Dictionary<string, double> resultRx = new Dictionary<string, double>();
            float ULfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelUL_FREQ("LTE", Band, channel);
            float DLfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelDL_FREQ("LTE", Band, channel);

            Dictionary<string, double> result = new Dictionary<string, double>();
    
            List<uint> ants = new List<uint>();
            if (DPDT_AntState != 3) ants = new List<uint>() { DPDT_AntState };
            else ants = new List<uint>() { 0, 1 };
            foreach (uint ant in ants)
            {
                //DPDT_AntState = ant;
                string anttype = null;
                switch (ant)
                {
                    case 0:
                        anttype = "上天线";
                        break;
                    case 1:
                        anttype = "下天线";
                        break;
                    case 2:
                        anttype = "自由切换";
                        break;
                }

                samsung.Dut_Dpdt_S_Switch("2", (int)ant);

                Func<string, string, string> GetInfo = (txtype, rbstart) => { return string.Format("LTE Band{0} {1} {2} {3} {4}Tx RBconfig=1/{5}配置完成，请检查CCM", Band, bandWidth, channel, anttype, txtype, rbstart); };

                //LTE_TX_Test(Band, bandWidth, ULfreqMHz, DLfreqMHz, ref resultTx);
                uint rbs = 0;
                if (LTE_RBP_L_Channel_List.Contains(channel.ToString()))
                {
                    rbs = 0;
                }
                if (LTE_RBP_H_Channel_List.Contains(channel.ToString()))
                {
                    switch (bandWidth)
                    {
                        case Samsung.LTE_BandWidth._1p4MHz:
                            rbs = 5;
                            break;
                        case Samsung.LTE_BandWidth._3MHz:
                            break;
                        case Samsung.LTE_BandWidth._5MHz:
                            rbs = 24;
                            break;
                        case Samsung.LTE_BandWidth._10MHz:
                            rbs = 49;
                            break;
                        case Samsung.LTE_BandWidth._15MHz:
                            rbs = 74;
                            break;
                        case Samsung.LTE_BandWidth._20MHz:
                            rbs = 99;
                            break;
                    }

                }
                samsung.Dut_LTE_S_NoSYNC_SetBand(Band);
                samsung.Dut_LTE_S_SendTx(Samsung.LTE_MOD_Type._QPSK,
                                                        bandWidth,
                                                        ULfreqMHz,
                                                        1,
                                                        rbs,
                                                        GetPowerClass(Band, "LTE"));
                result.Add(GetInfo(nameof(Samsung.TxType.Main), rbs.ToString()), double.NaN);
                if (isContinue(GetInfo(nameof(Samsung.TxType.Main), rbs.ToString())) == false) return result;
                //foreach (KeyValuePair<string, double> kv in resultTx)
                //{
                //    result.Add(kv.Key, kv.Value);
                //}
            }
            samsung.Dut_LTE_S_TestEnd();
            return result;
        }

        public Dictionary<string, double> SamsungTDD_LTE_Test(int Band, int channel, Samsung.LTE_BandWidth bandWidth = Samsung.LTE_BandWidth._10MHz)
        {
            Dictionary<string, double> resultTx = new Dictionary<string, double>();
            Dictionary<string, double> resultRx = new Dictionary<string, double>();

            float ULfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelUL_FREQ("LTE", Band, channel);
            float DLfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelDL_FREQ("LTE", Band, channel);

            Dictionary<string, double> result = new Dictionary<string, double>();
            cmw100.CB_Ver_TDDLTE_S_PreSetting();


            cmw100.CB_Ver_TDDLTE_S_Init(Band, ULfreqMHz, loss, 1, 0, DLfreqMHz, loss, CMW100.MCS_Type.QPSK,/* (RF_SDK.bandWidth)bandWidth*/RF_SDK.bandWidth.BW_10MHz);

            
            cmw100.CB_Ver_GPRF_Set("ON"); //有可能换成

            samsung.Dut_LTE_S_TestEnd();

            //switch (bandWidth)
            //{
            //    case Samsung.LTE_BandWidth._1p4MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._3MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._5MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._10MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._15MHz:
            //        //bandwidthindex = 100;
            //        break;
            //    case Samsung.LTE_BandWidth._20MHz:
            //        //bandwidthindex = 100;
            //        break;
            //}
            //samsung.Dut_Dpdt_S_Switch("2", (int)DPDT_AntState);
            samsung.Dut_LTE_S_SYNC(Samsung.LTE_AntType.PRX, Band, (float)DLfreqMHz);

            if (GlodEnable)
            {
                LTE_CalibratePowerTestLogic(Band, ULfreqMHz, DLfreqMHz, bandWidth,DPDT_AntState);
            }
            ////LTE_TX_Test(Band, bandWidth, ULfreqMHz, DLfreqMHz, ref resultTx);

            if (GlodEnable)
            {
                LTE_CalibrateRxLevelTestLogic(Band, bandWidth, DLfreqMHz);
            }

            LTE_RX_Test(Band, bandWidth, DLfreqMHz, ref resultRx);
            foreach (KeyValuePair<string, double> kv in resultRx)
            {
                result.Add(kv.Key, kv.Value);
            }

            foreach (KeyValuePair<string, double> kv in resultTx)
            {
                result.Add(kv.Key, kv.Value);
            }
            samsung.Dut_LTE_S_TestEnd();
            return result;
        }

        public Dictionary<string, double> SamsungFDD_LTE_Test(int Band, int channel, Samsung.LTE_BandWidth bandWidth = Samsung.LTE_BandWidth._10MHz)
        {
            Dictionary<string, double> resultTx = new Dictionary<string, double>();
            Dictionary<string, double> resultRx = new Dictionary<string, double>();
            float ULfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelUL_FREQ("LTE", Band, channel);
            float DLfreqMHz = (float)MainTestQueue.ChannelToFreq.GetChannelDL_FREQ("LTE", Band, channel);

            Dictionary<string, double> result = new Dictionary<string, double>();
            cmw100.CB_Ver_FDDLTE_S_PreSetting();

            cmw100.CB_Ver_FDDLTE_S_Init(Band, ULfreqMHz, loss, 1, 0, DLfreqMHz, loss, CMW100.MCS_Type.QPSK, /*(RF_SDK.bandWidth)bandWidth*/RF_SDK.bandWidth.BW_10MHz);
            cmw100.CB_Ver_GPRF_Set("ON");
            samsung.Dut_LTE_S_TestEnd();
            //switch (bandWidth)
            //{
            //    case Samsung.LTE_BandWidth._1p4MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._3MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._5MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._10MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._15MHz:
            //        //bandwidthindex = 100;
            //        break;
            //    case Samsung.LTE_BandWidth._20MHz:
            //        //bandwidthindex = 100;
            //        break;
            //}
            List<uint> ants = new List<uint>();
            if (DPDT_AntState != 3) ants = new List<uint>() { DPDT_AntState };
            else ants = new List<uint>() { 0, 1 };
            foreach (uint ant in ants)
            {
                DPDT_AntState = ant;
                string anttype = null;
                switch (ant)
                {
                    case 0:
                        anttype = "上天线";
                        break;
                    case 1:
                        anttype = "下天线";
                        break;
                    case 2:
                        anttype = "自由切换";
                        break;
                }

                samsung.Dut_Dpdt_S_Switch("2", (int)ant);
                samsung.Dut_LTE_S_SYNC(Samsung.LTE_AntType.PRX, Band, (float)DLfreqMHz);

                if (GlodEnable)
                {
                    //cmw100.CB_Ver_FDDLTE_S_Init(Band, ULfreqMHz, 25, 1, 0, DLfreqMHz, 25, CMW100.MCS_Type.QPSK, RF_SDK.bandWidth.BW_10MHz);
                    LTE_CalibratePowerTestLogic(Band, ULfreqMHz, DLfreqMHz, bandWidth, ant);
                }
                LTE_TX_Test(Band, bandWidth, ULfreqMHz, DLfreqMHz,(int)ant, ref resultTx);
                if(ant==0)
                { 
                if (GlodEnable)
                {
                    LTE_CalibrateRxLevelTestLogic(Band, bandWidth, DLfreqMHz);
                }

                LTE_RX_Test(Band, bandWidth, DLfreqMHz, ref resultRx);
                foreach (KeyValuePair<string, double> kv in resultRx)
                {
                    result.Add(kv.Key, kv.Value);
                }
                }
                foreach (KeyValuePair<string, double> kv in resultTx)
                {
                    result.Add(kv.Key, kv.Value);
                }
            }
            samsung.Dut_LTE_S_TestEnd();
            return result;
        }


        private void LTE_CalibrateRxLevelTestLogic(int band, Samsung.LTE_BandWidth bandWidth, double DLfreqMHz)
        {
            string rssistr = null;
            string berstr = null;    
            Dictionary<Samsung.LTE_AntType, int> prefectPORT = new Dictionary<Samsung.LTE_AntType, int>();
            switch (muliboxType)
            {
                case MuliBoxType.COM1:
                case MuliBoxType.COM2:
                case MuliBoxType.COM3:
                case MuliBoxType.COM4:
                case MuliBoxType.COM5:
                case MuliBoxType.COM6:
                case MuliBoxType.COM7:
                case MuliBoxType.COM8:
                    //cmw100.CMW100_GPRF_SelectPort((int)muliboxType);
                    Dictionary<string, double> temp = new Dictionary<string, double>();
                    cmw100.CMW100_GPRF_Level(-70);
                    bool S = samsung.Dut_LTE_S_RSSITest(band, bandWidth,Samsung.LTE_AntType.PRX,(float)DLfreqMHz, ref rssistr,ref berstr);
                    if(rssistr=="") Log.GetInstance().d("Samsung","同步失败");
                    if (Convert.ToDouble(rssistr) > -95) prefectPORT.Add(Samsung.LTE_AntType.PRX, (int)muliboxType);
                    Thread.Sleep(1000);
                    samsung.Dut_LTE_S_RSSITest(band, bandWidth, Samsung.LTE_AntType.PRX, (float)DLfreqMHz, ref rssistr, ref berstr);
                    if (drxEnable == true && Convert.ToDouble(rssistr) > -98) prefectPORT.Add(Samsung.LTE_AntType.DRX, (int)muliboxType);
                    samsung.Dut_LTE_S_RSSITest(band, bandWidth, Samsung.LTE_AntType.PRX, (float)DLfreqMHz, ref rssistr, ref berstr);
                    if (BandWithMIMO.Contains(band.ToString()) && drxEnable == true && Convert.ToDouble(rssistr) > -98) prefectPORT.Add(Samsung.LTE_AntType.MIMO1, (int)muliboxType);
                    samsung.Dut_LTE_S_RSSITest(band, bandWidth, Samsung.LTE_AntType.PRX, (float)DLfreqMHz, ref rssistr, ref berstr);
                    if (BandWithMIMO.Contains(band.ToString()) && drxEnable == true && Convert.ToDouble(rssistr) > -98) prefectPORT.Add(Samsung.LTE_AntType.MIMO2, (int)muliboxType);
                    break;
                case MuliBoxType.TESCOM16:
                    cmw100.CMW100_GPRF_Level(-70);
                    prefectPORT = LTE_TestComRx(band, DLfreqMHz, bandWidth);
                    break;
                default:
                    throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
            }

            foreach (KeyValuePair<Samsung.LTE_AntType, int> kv in prefectPORT)
            {
                LTE_CalibrateLevel(kv, band, loss, DLfreqMHz, bandWidth);
            }
        }

        private void LTE_RX_Test(int Band, Samsung.LTE_BandWidth bandWidth, double DLfreqMHz, ref Dictionary<string, double> result)
        {
            string rssistr = null;
            string berstr = null;
            int rePort = 0;
            double reloss = 0.0;
            samsung.Dut_Dpdt_S_Switch("2", 0);
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    //if (readline == null) continue;
                    if (readline == "" || readline == null) break;
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');
                    //double s = Convert.ToDouble(testinfos[4].Split(':')[1]);
                    if (readline.Contains("DownLink") 
                        && readline.Contains("LTE") 
                        && Convert.ToDouble(testinfos[4].Split(':')[1]) == DLfreqMHz
                        && readline.Contains("Band"+Band.ToString())
                        && readline.Contains(string.Format("BandWidth:{0}",bandWidth.ToString())))
                    {
                        KeyValuePair<Samsung.LTE_AntType, double> prefectloss = JsonConvert.DeserializeObject<KeyValuePair<Samsung.LTE_AntType, double>>(readarray.Last());

                        //Dictionary<string, double> resultRx = new Dictionary<string, double>();
                        rePort = Convert.ToInt16(testinfos[2].Split(':')[1]);
                        //cmw100.CMW100_CloseAllPort();
                        
                       
                        reloss = prefectloss.Value;
                        double temploss = 0.0;

                        if (prefectloss.Key == Samsung.LTE_AntType.DRX)
                        {
                            string str01 = "drx";
                        }
                        else
                        {
                            string str02 = "prx";
                        }
                       
                        cmw100.CMW100_GPRF_Level(-70);
                        cmw100.CB_Ver_Set_Loss(CMW100.Technology.LTE, ref temploss, ref reloss);
                        cmw100.CMW100_GPRF_Level(-70);

                        //cmw100.CMW100_GPRF_SelectPort(18);
                        //do
                        //{
                        //    samsung.Dut_LTE_S_TestEnd();
                        //} while (!samsung.Dut_LTE_S_SYNC(prefectloss.Key, Band, (float)DLfreqMHz));
                        cmw100.CMW100_GPRF_SelectPort(rePort);

                        //Thread.Sleep(1000);
                        rssistr = null;
                        berstr = null;
                        //samsung.Dut_LTE_S_RSSITest(Band, bandWidth, prefectloss.Key, (float)DLfreqMHz, ref rssistr, ref berstr);
                        //Thread.Sleep(200);
                        samsung.Dut_LTE_S_RSSITest(Band, bandWidth, prefectloss.Key, (float)DLfreqMHz, ref rssistr, ref berstr);
                        //foreach (KeyValuePair<string, double> kv in resultRx)
                        //{
                        string key = prefectloss.Key.ToString() + "_RSCP";
                        //if (result.Keys.Contains(key)) key = key + "001";
                        result.Add(key, Convert.ToDouble(rssistr)); 
                        //}

                        bool failed = false;
                        double power = -92;
                        double powerstep = 2;
                        A: do
                        {
                            if (power > -45)
                            {
                                power = 999999;
                                break;
                            }

                            Log.GetInstance().d("Samsung", power.ToString());
                            cmw100.CMW100_GPRF_Level(power);
                            samsung.Dut_LTE_S_TestEnd();
                            samsung.Dut_LTE_S_SYNC(prefectloss.Key, Band, (float)DLfreqMHz);
                            //samsung.Dut_LTE_S_RSSITest(bandWidth, prefectloss.Key, (float)DLfreqMHz, ref rssistr, ref berstr);
                            //samsung.Dut_LTE_S_RSSITest(Band, bandWidth, prefectloss.Key, (float)DLfreqMHz, ref rssistr, ref berstr);
                            //Thread.Sleep(200);
                            samsung.Dut_LTE_S_RSSITest(Band, bandWidth, prefectloss.Key, (float)DLfreqMHz, ref rssistr, ref berstr);
                          C:try
                            {
                                if (Convert.ToDouble(berstr) >= 95)
                                {
                                    if (failed == true) break;
                                    power = power - powerstep;
                                }
                                else
                                {
                                    power = power + powerstep;
                                    failed = true;
                                    powerstep = 0.5;
                                }
                            } catch
                            {
                                samsung.Dut_LTE_S_TestEnd();
                                samsung.Dut_LTE_S_SYNC(prefectloss.Key, Band, (float)DLfreqMHz);
                                samsung.Dut_LTE_S_RSSITest(Band, bandWidth, prefectloss.Key, (float)DLfreqMHz, ref rssistr, ref berstr);
                                Log.GetInstance().d("Samsung", "接收电平为0");
                                goto C;
                                //throw new Exception("接收电平异常，导致路损<0,请保存数据后重启手机重新开始测试");
                            }
                            
                        } while (true);
                        if (power == 999999)
                        {
                            samsung.Dut_LTE_S_TestEnd();
                            samsung.Dut_LTE_S_SYNC(prefectloss.Key, Band, (float)DLfreqMHz);
                            power = -92;
                            goto A;
                        }
                        //foreach (KeyValuePair<string, double> kv in resultRx)
                        //{
                        key = prefectloss.Key + "_Sen";
                        //if (result.Keys.Contains(key)) key = key + "001";
                        result.Add(key, power);
                        //}
                    }
                } while (true);
            }
        }

        private void LTE_TX_Test(int Band, Samsung.LTE_BandWidth bandWidth, double ULfreqMHz, double DLfreqMHz,int ant, ref Dictionary<string, double> result)
        {
            int rePort = 0;
            double reloss = 0.0;

            Samsung.TxType txType = Samsung.TxType.Main;
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    if (readline == null || readline == "") break;
                    Log.GetInstance().d("Info", readline);
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');
                    if (readline.Contains("UpLink")
                        && readline.Contains("LTE")
                        && Convert.ToDouble(testinfos[4].Split(':')[1]) == ULfreqMHz
                        && readline.Contains("Band" + Band.ToString())
                        && readline.Contains(string.Format("BandWidth:{0}", bandWidth.ToString()))
                        && readline.Contains(string.Format("AntState:{0}",ant)))
                    {
                        int antstate = 0;
                        int.TryParse(testinfos[5].Split(':')[1], out antstate);
                        int classpower = GetPowerClass(Band, "LTE");
                        rePort = Convert.ToInt16(testinfos[2].Split(':')[1]);
                        reloss = Convert.ToDouble(readarray.Last().Replace("\t", ""));
                        Dictionary<string, double> resultTx = null;
                        uint loop = 10;
                        bool teststate = false;
                        do
                        {
                            resultTx = new Dictionary<string, double>();
                            
                            LTE_ini(Band, (float)ULfreqMHz, (float)DLfreqMHz);

                            cmw100.CB_Ver_Set_Cellpower(-50);
                            cmw100.CMW100_SelectMeasPort(rePort, CMW100.Technology.LTE);
                            samsung.Dut_Dpdt_S_Switch("2", antstate);
                            cmw100.CB_Ver_Set_Loss(CMW100.Technology.LTE, ref reloss, ref loss);
                            do
                            {

                               
                                samsung.Dut_LTE_S_TestEnd();
                                samsung.Dut_LTE_S_SYNC(Samsung.LTE_AntType.PRX, Band, (float)DLfreqMHz);
                                samsung.Dut_Dpdt_S_Switch("2", antstate);
                                samsung.Dut_LTE_S_SendTx(Samsung.LTE_MOD_Type._QPSK,
                                                        bandWidth,
                                                        ULfreqMHz,
                                                        1,
                                                        0,
                                                        classpower);
                                teststate = cmw100.CB_Ver_TDDLTE_S_Tx_Test(out resultTx);
                            } while (teststate == false);

                            loop--;
                            if (loop == 0&&(resultTx == null | resultTx.Count == 0))
                            {
                                resultTx.Add("MaxPow", 999999);
                                resultTx.Add("FreqErr", 999999);
                                break;
                            }
                        } while (teststate==false|resultTx["MaxPow"] < classpower-3 | resultTx["MaxPow"]>classpower+3);
                        foreach (KeyValuePair<string, double> kv in resultTx)
                        {
                            string keystr = (antstate == 0 ? "上天线" : "下天线") + "_" + txType.ToString() + "_" + kv.Key.ToString();
                            Log.GetInstance().d("System", keystr);
                            Log.GetInstance().d("System", JsonConvert.SerializeObject(result));
                            //if (result.Keys.Contains(keystr)) keystr = keystr + "01";
                            result.Add(keystr, kv.Value);
                        }
                    }
                } while (true);
            }
        }

        private void LTE_CalibratePowerTestLogic(int Band, double ULfreqMHz,double DLfreqMHz, Samsung.LTE_BandWidth bandWidth,uint DPDT_AntState=3)
        {
            double losstempmian = 0.0;
            //samsung.Dut_LTE_S_TestEnd();
            


            for (int AntState = 0; AntState <= 1; AntState++)
            {
              antswitch:
                LTE_ini(Band, (float)ULfreqMHz, (float)DLfreqMHz);
               
                samsung.Dut_LTE_S_SYNC(Samsung.LTE_AntType.PRX, Band, (float)DLfreqMHz);
                samsung.Dut_LTE_S_SendTx(
                        Samsung.LTE_MOD_Type._QPSK,
                        bandWidth,
                        ULfreqMHz,
                        1,
                        0,
                        GetPowerClass(Band, "LTE"));
                if (DPDT_AntState == 0 && AntState == 1) break;
                else if (DPDT_AntState == 1) AntState = 1;

                samsung.Dut_Dpdt_S_Switch("2", (int)AntState);
                
                int port = 0;
                switch (muliboxType)
                {
                    case MuliBoxType.COM1:
                    case MuliBoxType.COM2:
                    case MuliBoxType.COM3:
                    case MuliBoxType.COM4:
                    case MuliBoxType.COM5:
                    case MuliBoxType.COM6:
                    case MuliBoxType.COM7:
                    case MuliBoxType.COM8:
                        port = (int)muliboxType;
                        break;
                    case MuliBoxType.TESCOM16:
                        port = LTE_TestComTx();
                        break;
                    default:
                        throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
                }
                losstempmian = LTE_CalibratePower(port, Band, loss, bandWidth, ULfreqMHz, Samsung.TxType.Main, AntState);
                if (losstempmian == -1) goto antswitch;
            }
            //samsung.Dut_Dpdt_S_Switch("2", (int)DPDT_AntState);
            //if (GlodEnable)
            //{
            //    int port = 0;
            //    switch (muliboxType)
            //    {
            //        case MuliBoxType.COM1:
            //        case MuliBoxType.COM2:
            //        case MuliBoxType.COM3:
            //        case MuliBoxType.COM4:
            //        case MuliBoxType.COM5:
            //        case MuliBoxType.COM6:
            //        case MuliBoxType.COM7:
            //        case MuliBoxType.COM8:
            //            port = (int)muliboxType;
            //            break;
            //        case MuliBoxType.TESCOM16:
            //            port = LTE_TestComTx();
            //            break;
            //        default:
            //            throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
            //    }
            //    losstempmian = LTE_CalibratePower(port,Band, loss,bandWidth, ULfreqMHz, Samsung.TxType.Main);
            //    if (losstempmian == -1) LTE_CalibratePowerTestLogic(Band, ULfreqMHz, DLfreqMHz, bandWidth);
            //}
        }

        private int LTE_TestComTx()
        {
            List<double> powerlist = new List<double>();
            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                mytest:
                cmw100.CMW100_SelectMeasPort(cmw100_portindex, CMW100.Technology.LTE);
                Dictionary<string, double> result = new Dictionary<string, double>();
                bool teststate = cmw100.CB_Ver_TDDLTE_S_Tx_Test(out result);
                //if (teststate == false)
                //{
                //    resultTx.Add("MaxPow", 999999);
                //    resultTx.Add("FreqErr", 999999);
                //}
                if (teststate == false) goto mytest;
                powerlist.Add(result["MaxPow"]);
            }
            int prefectport = powerlist.IndexOf(powerlist.Max()) + 1;
            cmw100.CMW100_SelectMeasPort(prefectport,CMW100.Technology.LTE);
            return prefectport;
        }

        private Dictionary<Samsung.LTE_AntType, int> LTE_TestComRx(int band, double freqMHz, Samsung.LTE_BandWidth bandWidth)
        {
            string rssistr = null;
            string berstr = null;

            List<double> PRX_rscplist = new List<double>();
            List<double> DRX_rscplist = new List<double>();
            List<double> MIMO1_rscplist = new List<double>();
            List<double> MIMO2_rscplist = new List<double>();
            samsung.Dut_Dpdt_S_Switch("2", 0);
            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                cmw100.CMW100_GPRF_SelectPort(cmw100_portindex);
                Dictionary<string, double> result = new Dictionary<string, double>();
                Thread.Sleep(1000);
                bool state = samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.PRX, (float)freqMHz, ref rssistr, ref berstr);
                if (state == false)
                {
                    rssistr = "-1000";
                    Log.GetInstance().d("System", "PRX Dut_LTE_S_RSSITest同步失败");
                }
                PRX_rscplist.Add(Convert.ToDouble(rssistr));
                Thread.Sleep(1000);
                state = samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.DRX, (float)freqMHz, ref rssistr, ref berstr);
                if (state == false)
                {
                    rssistr = "-1000";
                    Log.GetInstance().d("System", "DRX Dut_LTE_S_RSSITest同步失败");
                }
                DRX_rscplist.Add(Convert.ToDouble(rssistr));
                Thread.Sleep(1000);
                if (BandWithMIMO.Contains(band.ToString()))
                {

                    //cmw100.CMW100_GPRF_SelectPort(18);
                    //do
                    //{
                    //    samsung.Dut_LTE_S_TestEnd();
                    //} while (!samsung.Dut_LTE_S_SYNC(Samsung.LTE_AntType.MIMO1, band, (float)freqMHz));
                    //cmw100.CMW100_GPRF_SelectPort(cmw100_portindex);

                    state = samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.MIMO1, (float)freqMHz, ref rssistr, ref berstr);
                    if (state == false)
                    {
                        rssistr = "-1000";
                        Log.GetInstance().d("System", "MIMO1 Dut_LTE_S_RSSITest同步失败");
                    }
                    MIMO1_rscplist.Add(Convert.ToDouble(rssistr));
                    Thread.Sleep(1000);
                    state = samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.MIMO2, (float)freqMHz, ref rssistr, ref berstr);
                    if (state == false)
                    {
                        rssistr = "-1000";
                        Log.GetInstance().d("System", "MIMO2 Dut_LTE_S_RSSITest同步失败");
                    }
                    MIMO2_rscplist.Add(Convert.ToDouble(rssistr));
                }
            }
            Dictionary<Samsung.LTE_AntType, int> prefectPort = new Dictionary<Samsung.LTE_AntType, int>();
           /* if (PRX_rscplist.Max() > -98)*/ prefectPort.Add(Samsung.LTE_AntType.PRX, PRX_rscplist.IndexOf(PRX_rscplist.Max()) + 1);
            if (DrxEnable /*&& DRX_rscplist.Max() > -98*/) prefectPort.Add(Samsung.LTE_AntType.DRX, DRX_rscplist.IndexOf(DRX_rscplist.Max()) + 1);
            if (BandWithMIMO.Contains(band.ToString()))
            {
                if (MIMO1Enable /*&& DRX_rscplist.Max() > -98*/) prefectPort.Add(Samsung.LTE_AntType.MIMO1, MIMO1_rscplist.IndexOf(MIMO1_rscplist.Max()) + 1);
                if (MIMO2Enable /*&& DRX_rscplist.Max() > -98*/) prefectPort.Add(Samsung.LTE_AntType.MIMO2, MIMO2_rscplist.IndexOf(MIMO2_rscplist.Max()) + 1);
            }
            return prefectPort;
        }

        private KeyValuePair<Samsung.LTE_AntType, double> LTE_CalibrateLevel(KeyValuePair<Samsung.LTE_AntType, int> prefectport,int band, double startloss, double freqMHz, Samsung.LTE_BandWidth bandWidth)
        {
            string rssistr = null;
            string berstr = null;
            //cmw100.CMW100_CloseAllPort();
            samsung.Dut_Dpdt_S_Switch("2", 0);

            double ulloss = 0.0;
            double rscp = 0.0;
            int loop = 10;
            do
            {
                //switch (prefectport.Key)
                //{
                //    case Samsung.NR_AntType.PRX:
                //        cmw100.CB_Ver_Set_Loss(CMW100.Technology.NR, ref ulloss, ref startloss);
                //        break;
                //    case Samsung.NR_AntType.DRX:
                //        cmw100.CB_Ver_Set_Loss(CMW100.RxType.DRX, CMW100.Technology.NR, ref ulloss, ref startloss);
                //        break;
                //    case Samsung.NR_AntType.MIMO1:
                //        cmw100.CB_Ver_Set_Loss(CMW100.RxType.MIMO2, CMW100.Technology.NR, ref ulloss, ref startloss);
                //        break;
                //    case Samsung.NR_AntType.MIMO2:
                //        cmw100.CB_Ver_Set_Loss(CMW100.RxType.MIMO3, CMW100.Technology.NR, ref ulloss, ref startloss);
                //        break;
                //    default:
                //        throw new Exception(string.Format("CMW100不支持{0}这种RX类型", prefectport.Key.ToString()));
                //}
                cmw100.CMW100_GPRF_Level(-70);
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.NR, ref ulloss, ref startloss);
                cmw100.CMW100_GPRF_Level(-70);

                //cmw100.CMW100_GPRF_SelectPort(18);
                //do
                //{
                //    samsung.Dut_LTE_S_TestEnd();
                //} while (!samsung.Dut_LTE_S_SYNC(prefectport.Key, band, (float)freqMHz));
                cmw100.CMW100_GPRF_SelectPort(prefectport.Value);

                switch (prefectport.Key)
                {
                    case Samsung.LTE_AntType.PRX:
                        samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.PRX, (float)freqMHz, ref rssistr, ref berstr);
                        rscp = Convert.ToDouble(rssistr);
                        break;
                    case Samsung.LTE_AntType.DRX:
                        samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.DRX, (float)freqMHz, ref rssistr, ref berstr);
                        rscp = Convert.ToDouble(rssistr);
                        break;
                    case Samsung.LTE_AntType.MIMO1:
                        samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.MIMO1, (float)freqMHz, ref rssistr, ref berstr);
                        rscp = Convert.ToDouble(rssistr);
                        break;
                    case Samsung.LTE_AntType.MIMO2:
                        samsung.Dut_LTE_S_RSSITest(band,bandWidth, Samsung.LTE_AntType.MIMO2, (float)freqMHz, ref rssistr, ref berstr);
                        rscp = Convert.ToDouble(rssistr);
                        break;
                    default:
                        throw new Exception(string.Format("Samsung LTE不支持{0}这种RX类型", prefectport.Key.ToString()));
                }
                Log.GetInstance().d(prefectport.Key.ToString(), rscp.ToString());
                if (rscp >= -69.5 || rscp <= -70.5)
                {
                    startloss = startloss - (rscp + 70);
                }
                loop--;
                if (startloss > 46) { startloss = 35; }
                if (loop == 0) break;
            } while (rscp > -69.5 || rscp < -70.5);

            KeyValuePair<Samsung.LTE_AntType, double> prefectLoss = new KeyValuePair<Samsung.LTE_AntType, double>(prefectport.Key, startloss);

            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                string str = string.Format("%LTE Band{0}\tDownLink \tCMW100_PORT:{1}\tBandWidth:{2}\tFreqMHz:{3}%\t{4}",band, prefectport.Value,bandWidth.ToString(), freqMHz, JsonConvert.SerializeObject(prefectLoss));
                Log.GetInstance().d("Samsung LTE", str);
                sw.WriteLine(str);
            }

            return prefectLoss;

        }

        private double LTE_CalibratePower(int port,int band, double startloss,Samsung.LTE_BandWidth lTE_BandWidth, double freqMHz, Samsung.TxType txType,int antState)
        {
            cmw100.CMW100_SelectMeasPort(port,CMW100.Technology.LTE);
            Dictionary<string, double> result = new Dictionary<string, double>();
            int loop = 10;

            double remarkpower = (double)GetPowerClass(band, "LTE");

            do
            {
                Log.GetInstance().d("Samsung", string.Format("AntState={0}", antState));
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.LTE, ref startloss, ref loss);
                cmw100.CB_Ver_TDDLTE_S_Tx_Test(out result);
                Log.GetInstance().d("CMW", string.Format("MAXPOWER={0}", result["MaxPow"]));
                if (result["MaxPow"] > remarkpower + 0.5 || result["MaxPow"] < remarkpower - 0.5)
                {
                    startloss = startloss - (result["MaxPow"] - remarkpower);
                }
                loop--;
                if (loop == 0 || startloss > 60) return -1;
            } while (result["MaxPow"] > remarkpower+0.5 || result["MaxPow"] < remarkpower-0.5);
            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                sw.WriteLine(string.Format("%LTE Band{0}\tUpLink:{1}\tCMW100_PORT:{2}\tBandWidth:{3}\tFreqMHz:{4}\tAntState:{5}%\t{6}",band, txType, port, lTE_BandWidth.ToString(), freqMHz, antState, startloss));
            }
            return startloss;
        }

        private double LTE_SetSamsung_CenterFreq(Samsung.NR_BandWidth BandWidth, uint channel, double offset = 0.0)
        {
            double freqMHz = 0.0;
            switch (BandWidth)
            {
                case Samsung.NR_BandWidth._20M:
                    if (channel >= 600000)
                    {

                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 9.18 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 9.18 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._50M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 23.94 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 23.94 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._100M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 49.14 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 49.14 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
            }
            return freqMHz;
        }
        
        public Dictionary<string,double> SamsungGSM_Test(int Band, int channel)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            double ULfreqMHz = MainTestQueue.ChannelToFreq.GetChannelUL_FREQ("GSM", Band, channel);
            double DLfreqMHz = MainTestQueue.ChannelToFreq.GetChannelDL_FREQ("GSM", Band, channel);

            Samsung.GSM_Band gSM_Band = Samsung.GSM_Band.GSM1800;
            CMW100.GSM_Band C_gSM_Band = CMW100.GSM_Band.GSM900;
            switch (Band)
            {
                case 900:
                    gSM_Band = Samsung.GSM_Band.GSM900;
                    C_gSM_Band = CMW100.GSM_Band.GSM900;
                    break;
                case 1800:
                    gSM_Band = Samsung.GSM_Band.GSM1800;
                    C_gSM_Band = CMW100.GSM_Band.GSM1800;
                    break;
                case 1900:
                    gSM_Band = Samsung.GSM_Band.GSM1900;
                    C_gSM_Band = CMW100.GSM_Band.GSM1900;
                    break;
                case 850:
                    gSM_Band = Samsung.GSM_Band.GSM850;
                    C_gSM_Band = CMW100.GSM_Band.GSM850;
                    break;
            }

            cmw100.CB_Ver_GSM_S_PreSetting();
            
            cmw100.CB_Ver_GSM_S_Init(C_gSM_Band, (uint)channel, 25, 25, CMW100.GSM_MCS_Type.TSC0, DLfreqMHz);
            cmw100.CB_Ver_GPRF_Set("ON");

            List<uint> ants = new List<uint>();

            if (DPDT_AntState != 3) ants = new List<uint>() { DPDT_AntState };
            else ants = new List<uint>() { 0, 1 };

            foreach (uint ant in ants)
            {
                string anttype = null;
                switch (ant)
                {
                    case 0:
                        anttype = "上天线";
                        break;
                    case 1:
                        anttype = "下天线";
                        break;
                    case 2:
                        anttype = "自由切换";
                        break;
                }

                samsung.Dut_Dpdt_S_Switch("0", (int)ant);
                samsung.Dut_GSM_S_SYNC(gSM_Band, channel, -70);

                if (GlodEnable)
                {
                    GSM_CalibrateRxLevelTestLogic(gSM_Band, channel, DLfreqMHz);
                }

                GSM_RX_Test(gSM_Band, channel, DLfreqMHz, ref result);
                if (GlodEnable)
                {
                    cmw100.CB_Ver_GSM_S_Init(C_gSM_Band, (uint)channel, 25, 25, CMW100.GSM_MCS_Type.TSC0, DLfreqMHz);
                    GSM_CalibratePowerTestLogic(gSM_Band, channel, ULfreqMHz, DLfreqMHz, -70, ant);
                }

                GSM_TX_Test(gSM_Band, channel, ULfreqMHz, DLfreqMHz, ref result);
            }
            return result;
        }

        public Dictionary<string, double> SamsungGSM_TestOnlyDut(int Band, int channel, Func<string, bool> isContinue)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            double ULfreqMHz = MainTestQueue.ChannelToFreq.GetChannelUL_FREQ("GSM", Band, channel);
            double DLfreqMHz = MainTestQueue.ChannelToFreq.GetChannelDL_FREQ("GSM", Band, channel);

            Samsung.GSM_Band gSM_Band = Samsung.GSM_Band.GSM1800;
            CMW100.GSM_Band C_gSM_Band = CMW100.GSM_Band.GSM900;
            switch (Band)
            {
                case 900:
                    gSM_Band = Samsung.GSM_Band.GSM900;
                    C_gSM_Band = CMW100.GSM_Band.GSM900;
                    break;
                case 1800:
                    gSM_Band = Samsung.GSM_Band.GSM1800;
                    C_gSM_Band = CMW100.GSM_Band.GSM1800;
                    break;
                case 1900:
                    gSM_Band = Samsung.GSM_Band.GSM1900;
                    C_gSM_Band = CMW100.GSM_Band.GSM1900;
                    break;
                case 850:
                    gSM_Band = Samsung.GSM_Band.GSM850;
                    C_gSM_Band = CMW100.GSM_Band.GSM850;
                    break;
            }

            List<uint> ants = new List<uint>();

            if (DPDT_AntState != 3) ants = new List<uint>() { DPDT_AntState };
            else ants = new List<uint>() { 0, 1 };

            foreach (uint ant in ants)
            {
                string anttype = null;
                switch (ant)
                {
                    case 0:
                        anttype = "上天线";
                        break;
                    case 1:
                        anttype = "下天线";
                        break;
                    case 2:
                        anttype = "自由切换";
                        break;
                }
                samsung.Dut_GSM_S_NoSYNC_Ini();

                samsung.Dut_Dpdt_S_Switch("0", (int)ant);

                samsung.Dut_GSM_S_SendPower(gSM_Band, Band == 1800 || Band == 1900 ? 0 : 5, (uint)channel);
                string info = string.Format("GSM {0} {1} {2}Tx 配置完成，请检查CCM", gSM_Band.ToString(), channel, anttype);
                result.Add(info, double.NaN);
                if (!isContinue(info)) return result;
            }

            return result;
        }

        private void GSM_CalibrateRxLevelTestLogic(Samsung.GSM_Band Band,int channel, double DLfreqMHz)
        {
            float rssi = 0;
            float ber = 0;
            Dictionary<Samsung.GSM_AntType, int> prefectPORT = new Dictionary<Samsung.GSM_AntType, int>();
            switch (muliboxType)
            {
                case MuliBoxType.COM1:
                case MuliBoxType.COM2:
                case MuliBoxType.COM3:
                case MuliBoxType.COM4:
                case MuliBoxType.COM5:
                case MuliBoxType.COM6:
                case MuliBoxType.COM7:
                case MuliBoxType.COM8:
                    //cmw100.CMW100_GPRF_SelectPort((int)muliboxType);
                    Dictionary<string, double> temp = new Dictionary<string, double>();
                    cmw100.CMW100_GPRF_Level(-70);
                    bool S = samsung.Dut_GSM_S_RxTest(Band, (uint)channel, Samsung.GSM_ModType._GSMK, -70, Samsung.GSM_AntType.Prx, ref rssi, ref ber);
                    if (rssi > -95) prefectPORT.Add(Samsung.GSM_AntType.Prx, (int)muliboxType);
                    Thread.Sleep(1000);
                    samsung.Dut_GSM_S_RxTest(Band, (uint)channel, Samsung.GSM_ModType._GSMK, -70, Samsung.GSM_AntType.Drx, ref rssi, ref ber);
                    if (drxEnable == true && rssi > -98) prefectPORT.Add(Samsung.GSM_AntType.Drx, (int)muliboxType);
                    break;
                case MuliBoxType.TESCOM16:
                    cmw100.CMW100_GPRF_Level(-70);
                    prefectPORT = GSM_TestComRx(Band, channel,-70,DLfreqMHz);
                    break;
                default:
                    throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
            }

            foreach (KeyValuePair<Samsung.GSM_AntType, int> kv in prefectPORT)
            {
                GSM_CalibrateLevel(Band, channel, -70,kv,25, DLfreqMHz);
            }
        }

        private void GSM_RX_Test(Samsung.GSM_Band Band, int channel, double DLfreqMHz, ref Dictionary<string, double> result)
        {
            string rssistr = null;
            string berstr = null;
            int rePort = 0;
            double reloss = 0.0;
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    //if (readline == null) continue;
                    if (readline == "" || readline == null) break;
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');
                    double s = Convert.ToDouble(testinfos[3].Split(':')[1]);
                    
                    if (readline.Contains("DownLink") && readline.Contains("GSM") && Convert.ToDouble(Convert.ToDouble(testinfos[3].Split(':')[1]).ToString("f2")) == Convert.ToDouble(DLfreqMHz.ToString("f2")))
                    {
                        
                        rePort = Convert.ToInt16(testinfos[2].Split(':')[1]);
                        //cmw100.CMW100_CloseAllPort();
                        cmw100.CMW100_GPRF_SelectPort(rePort);
                        KeyValuePair<Samsung.GSM_AntType, double> prefectloss = JsonConvert.DeserializeObject<KeyValuePair<Samsung.GSM_AntType, double>>(readarray.Last());
                        reloss = prefectloss.Value;
                        double temploss = 0.0;

                        if (prefectloss.Key == Samsung.GSM_AntType.Drx)
                        {
                            string str01 = "drx";
                        }
                        else
                        {
                            string str02 = "prx";
                        }
                        cmw100.CMW100_GPRF_Level(-70);
                        cmw100.CB_Ver_Set_Loss(CMW100.Technology.GSM, ref temploss, ref reloss);
                        cmw100.CMW100_GPRF_Level(-70);
                        //Thread.Sleep(1000);
                        float rssi = 0, ber = 0;
                        samsung.Dut_GSM_S_RxTest(Band, (uint)channel, Samsung.GSM_ModType._GSMK, -70, prefectloss.Key, ref rssi, ref ber);
                        result.Add(prefectloss.Key + "_RSCP", (double)rssi);

                        bool failed = false;
                        double power = -100;
                        A: do
                        {
                            if (power > -45)
                            {
                                power = 999999;
                                break;
                            }

                            Log.GetInstance().d("Samsung", power.ToString());
                            cmw100.CMW100_GPRF_Level(power);
                            samsung.Dut_GSM_S_TestEnd();
                            samsung.Dut_GSM_S_SYNC(Band, channel, (float)power);
                            samsung.Dut_GSM_S_RxTest(Band, (uint)channel, Samsung.GSM_ModType._GSMK, -70, prefectloss.Key, ref rssi, ref ber);
                            Log.GetInstance().d("BER", ber.ToString());
                            if (ber <= 2.439)
                            {
                                if (failed == true) break;
                                power = power - 1.5;
                            }
                            else
                            {
                                power = power + 0.5;
                                failed = true;
                            }
                        } while (true);
                        if (power == 999999)
                        {
                            samsung.Dut_NR_S_TestEnd();
                            samsung.Dut_GSM_S_SYNC(Band, channel, -105);
                            power = -105;
                            goto A;
                        }
                        result.Add(prefectloss.Key + "_Sen".ToString(), power);
                    }
                } while (true);
            }
        }

        private void GSM_TX_Test(Samsung.GSM_Band Band,int channel, double ULfreqMHz, double DLfreqMHz, ref Dictionary<string, double> result)
        {
            int rePort = 0;
            double reloss = 0.0;

            Samsung.TxType txType = Samsung.TxType.Main;
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    if (readline == null || readline == "") break;
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');

                    Log.GetInstance().d("wifi", testinfos[3].Split(':')[1] + "=" + ULfreqMHz.ToString());

                    if (readline.Contains("UpLink") && readline.Contains("GSM") && Convert.ToDouble(Convert.ToDouble(testinfos[3].Split(':')[1]).ToString("f2")) == Convert.ToDouble(ULfreqMHz.ToString("f2")))
                    {
                        int antstate = Convert.ToInt16(testinfos[4].Split(':')[1]);
                        rePort = Convert.ToInt16(testinfos[2].Split(':')[1]);
                        reloss = Convert.ToDouble(readarray.Last().Replace("\t", ""));
                        double temploss = 25;
                        Dictionary<string, double> resultTx = new Dictionary<string, double>();
                        cmw100.CMW100_SelectMeasPort(rePort,CMW100.Technology.GSM);
                        cmw100.CB_Ver_Set_Loss(CMW100.Technology.GSM, ref reloss, ref temploss);
                        uint loop = 10;
                        do
                        {
                            samsung.Dut_GSM_S_TestEnd();
                            samsung.Dut_GSM_S_SYNC(Band,channel,-70);
                            samsung.Dut_GSM_S_SendPower(Band, Band == Samsung.GSM_Band.GSM1800 || Band == Samsung.GSM_Band.GSM1900 ? 0 : 5,(uint)channel);
                            cmw100.CB_Ver_GSM_S_Tx_Test(out resultTx);
                            loop--;
                            if (loop == 0) break;
                        } while (resultTx["TxPow"] < 0);
                        foreach (KeyValuePair<string, double> kv in resultTx)
                        {
                            result.Add((antstate == 0 ? "下天线" : "上天线") + "_" + txType.ToString() + "_" + kv.Key.ToString(), kv.Value);
                        }
                    }
                } while (true);

            }
        }

        private void GSM_CalibratePowerTestLogic(Samsung.GSM_Band Band,int channel,double ULfreqMHz, double DLfreqMHz, double dlpower,uint antstate)
        {
            double losstempmian = 0.0;
            for (int antstateindex = 0; antstateindex <= 1; antstateindex++)
            {
                if (antstate == 0 && antstateindex == 1) break;
                else if (antstate == 1) antstateindex = 1;
                antswitch_gsm:
                samsung.Dut_GSM_S_TestEnd();
                samsung.Dut_GSM_S_SYNC(Band, channel, (float)dlpower);
                samsung.Dut_GSM_S_SendPower(
                        Band,
                        Band == Samsung.GSM_Band.GSM1800 || Band == Samsung.GSM_Band.GSM1900 ? 0 : 5,
                        (uint)channel);
                int port = 0;
                switch (muliboxType)
                {
                    case MuliBoxType.COM1:
                    case MuliBoxType.COM2:
                    case MuliBoxType.COM3:
                    case MuliBoxType.COM4:
                    case MuliBoxType.COM5:
                    case MuliBoxType.COM6:
                    case MuliBoxType.COM7:
                    case MuliBoxType.COM8:
                        port = (int)muliboxType;
                        break;
                    case MuliBoxType.TESCOM16:
                        do
                        {
                            port = GSM_TestComTx();
                        } while (port == -1);

                        break;
                    default:
                        throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
                }
                losstempmian = GSM_CalibratePower(Band, port, loss, ULfreqMHz, Samsung.TxType.Main, antstateindex);
                if (losstempmian == -1) goto antswitch_gsm;
            }

            //if (GlodEnable)
            //{
            //    int port = 0;
            //    switch (muliboxType)
            //    {
            //        case MuliBoxType.COM1:
            //        case MuliBoxType.COM2:
            //        case MuliBoxType.COM3:
            //        case MuliBoxType.COM4:
            //        case MuliBoxType.COM5:
            //        case MuliBoxType.COM6:
            //        case MuliBoxType.COM7:
            //        case MuliBoxType.COM8:
            //            port = (int)muliboxType;
            //            break;
            //        case MuliBoxType.TESCOM16:
            //            do
            //            {
            //                port = GSM_TestComTx();
            //            } while (port == -1);

            //            break;
            //        default:
            //            throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
            //    }
            //    losstempmian = GSM_CalibratePower(Band, port, loss, ULfreqMHz, Samsung.TxType.Main);
            //    if (losstempmian == -1) GSM_CalibratePowerTestLogic(Band, channel,ULfreqMHz, DLfreqMHz, dlpower);
            //}
        }

        private int GSM_TestComTx()
        {
            List<double> powerlist = new List<double>();
            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                try
                {
                    cmw100.CMW100_SelectMeasPort(cmw100_portindex,CMW100.Technology.GSM);
                    Dictionary<string, double> result = new Dictionary<string, double>();
                    cmw100.CB_Ver_GSM_S_Tx_Test(out result);
                    if (result == null) return -1;
                    powerlist.Add(result["TxPow"]);
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            int prefectport = powerlist.IndexOf(powerlist.Max()) + 1;
            cmw100.CMW100_SelectMeasPort(prefectport, CMW100.Technology.GSM);
            return prefectport;
        }

        private Dictionary<Samsung.GSM_AntType, int> GSM_TestComRx(Samsung.GSM_Band band,int channel,double dlpower, double freqMHz)
        {
            float rssi = 0;
            float ber = 0;

            List<double> PRX_rscplist = new List<double>();
            List<double> DRX_rscplist = new List<double>();
            List<double> MIMO1_rscplist = new List<double>();
            List<double> MIMO2_rscplist = new List<double>();

            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                cmw100.CMW100_GPRF_SelectPort(cmw100_portindex);
                Dictionary<string, double> result = new Dictionary<string, double>();
                Thread.Sleep(1000);
                samsung.Dut_GSM_S_RxTest(band, (uint)channel, Samsung.GSM_ModType._GSMK, (float)dlpower, Samsung.GSM_AntType.Prx, ref rssi, ref ber);
                PRX_rscplist.Add(Convert.ToDouble(rssi));
                Thread.Sleep(1000);
                samsung.Dut_GSM_S_RxTest(band, (uint)channel, Samsung.GSM_ModType._GSMK, (float)dlpower, Samsung.GSM_AntType.Prx, ref rssi, ref ber);
                DRX_rscplist.Add(Convert.ToDouble(rssi));
            }

            Dictionary<Samsung.GSM_AntType, int> prefectPort = new Dictionary<Samsung.GSM_AntType, int>();
            if (PRX_rscplist.Max() > -98) prefectPort.Add(Samsung.GSM_AntType.Prx, PRX_rscplist.IndexOf(PRX_rscplist.Max()) + 1);
            if (DrxEnable && DRX_rscplist.Max() > -98) prefectPort.Add(Samsung.GSM_AntType.Drx, DRX_rscplist.IndexOf(DRX_rscplist.Max()) + 1);


            return prefectPort;
        }

        private KeyValuePair<Samsung.GSM_AntType, double> GSM_CalibrateLevel(
            Samsung.GSM_Band band,
            int channel,
            double dlpower, 
            KeyValuePair<Samsung.GSM_AntType, int> prefectport, 
            double startloss, 
            double freqMHz)
        {
            float rssi = 0;
            float ber = 0;
            //cmw100.CMW100_CloseAllPort();
            cmw100.CMW100_GPRF_SelectPort(prefectport.Value);

            double ulloss = 0.0;
            double rscp = 0.0;
            int loop = 10;
            do
            {

                cmw100.CMW100_GPRF_Level(-70);
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.GSM, ref ulloss, ref startloss);
                cmw100.CMW100_GPRF_Level(-70);

                switch (prefectport.Key)
                {
                    case Samsung.GSM_AntType.Prx:
                        samsung.Dut_GSM_S_RxTest(band,(uint)channel,Samsung.GSM_ModType._GSMK, (float)dlpower,Samsung.GSM_AntType.Prx, ref rssi, ref ber);
                        rscp = rssi;
                        break;
                    case Samsung.GSM_AntType.Drx:
                        samsung.Dut_GSM_S_RxTest(band, (uint)channel, Samsung.GSM_ModType._GSMK, (float)dlpower, Samsung.GSM_AntType.Prx, ref rssi, ref ber);
                        rscp = rssi;
                        break;
                    default:
                        throw new Exception(string.Format("Samsung GSM不支持{0}这种RX类型", prefectport.Key.ToString()));
                }
                Log.GetInstance().d(prefectport.Key.ToString(), rscp.ToString());
                if (rscp >= -69.5 || rscp <= -70.5)
                {
                    startloss = startloss - (rscp + 70);
                }
                loop--;
                if (loop == 0 || (startloss < 0 || startloss > 60))
                {
                    samsung.Dut_GSM_S_TestEnd();
                    samsung.Dut_GSM_S_SYNC(band, channel, (float)dlpower);
                    return GSM_CalibrateLevel(band, channel, dlpower, prefectport, 25, freqMHz);
                    
                }
            } while (rscp > -69.5 || rscp < -70.5);

            KeyValuePair<Samsung.GSM_AntType, double> prefectLoss = new KeyValuePair<Samsung.GSM_AntType, double>(prefectport.Key, startloss);

            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                sw.WriteLine(string.Format("%GSM\tDownLink \tCMW100_PORT:{0}\t FreqMHz:{1}%\t{2}", prefectport.Value, freqMHz, JsonConvert.SerializeObject(prefectLoss)));
            }

            return prefectLoss;

        }

        private double GSM_CalibratePower(Samsung.GSM_Band band, int port, double startloss, double freqMHz, Samsung.TxType txType,int antstate)
        {
            double dlloss = 25;
            cmw100.CMW100_SelectMeasPort(port, CMW100.Technology.GSM);
            Dictionary<string, double> result = new Dictionary<string, double>();
            int loop = 10;
            double enppower = 32.5;
            if (band == Samsung.GSM_Band.GSM850 || band == Samsung.GSM_Band.GSM900) enppower = 32.5;
            else enppower = 29.5;
            do
            {
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.GSM, ref startloss, ref dlloss);
                cmw100.CB_Ver_GSM_S_Tx_Test(out result);
                if (result["TxPow"] > enppower+0.5 || result["TxPow"] < enppower-0.5)
                {
                    startloss = startloss - (result["TxPow"] - enppower);
                }
                loop--;
                if (loop == 0 || startloss > 60) return -1;
            } while (result["TxPow"] > enppower + 0.5 || result["TxPow"] < enppower - 0.5);
            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                sw.WriteLine(string.Format("%GSM\tUpLink:{0}\tCMW100_PORT:{1}\tFreqMHz:{2}\tAntState:{3}%\t{4}", txType, port, freqMHz, antstate, startloss));
            }
            return startloss;
        }

        private double GSM_SetSamsung_CenterFreq(Samsung.NR_BandWidth BandWidth, uint channel, double offset = 0.0)
        {
            double freqMHz = 0.0;
            switch (BandWidth)
            {
                case Samsung.NR_BandWidth._20M:
                    if (channel >= 600000)
                    {

                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 9.18 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 9.18 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._50M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 23.94 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 23.94 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._100M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 49.14 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 49.14 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
            }
            return freqMHz;
        }
        
        public Dictionary<string,double> SamsungCDMA_Test(int Band, int channel)
        {
            double ULfreqMHz = MainTestQueue.ChannelToFreq.GetChannelUL_FREQ("CDMA", Band, channel);
            double DLfreqMHz = MainTestQueue.ChannelToFreq.GetChannelDL_FREQ("CDMA", Band, channel);

            Dictionary<string, double> result = new Dictionary<string, double>();
            cmw100.CB_Ver_CDMA_S_PreSetting();
            cmw100.CB_Ver_CDMA_S_Init(ULfreqMHz, channel, 25, DLfreqMHz,25);
            cmw100.CB_Ver_GPRF_Set("ON");
           
            //switch (bandWidth)
            //{
            //    case Samsung.LTE_BandWidth._1p4MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._3MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._5MHz:
            //        //bandwidthindex = 20;
            //        break;
            //    case Samsung.LTE_BandWidth._10MHz:
            //        //bandwidthindex = 50;
            //        break;
            //    case Samsung.LTE_BandWidth._15MHz:
            //        //bandwidthindex = 100;
            //        break;
            //    case Samsung.LTE_BandWidth._20MHz:
            //        //bandwidthindex = 100;
            //        break;
            //}


            samsung.Dut_Dpdt_S_Switch("4", (int)DPDT_AntState);
            samsung.Dut_CDMA_S_SYNC(Band, channel);

            if (GlodEnable)
            {
                CDMA_CalibrateRxLevelTestLogic(channel, DLfreqMHz);
            }

            CDMA_RX_Test(Band, channel, DLfreqMHz, ref result);
            if (GlodEnable)
            {
                CDMA_CalibratePowerTestLogic(Band, channel);
            }
            CDMA_TX_Test(Band, channel,ref result);

            return result;
        }

        private void CDMA_CalibrateRxLevelTestLogic(int channel, double DLfreqMHz)
        {
            float ber = 0;
            Dictionary<Samsung.CDMA_AntType, int> prefectPORT = new Dictionary<Samsung.CDMA_AntType, int>();
            float rssi_PRX = 0;
            float rssi_DRX = 0;
            switch (muliboxType)
            {
                case MuliBoxType.COM1:
                case MuliBoxType.COM2:
                case MuliBoxType.COM3:
                case MuliBoxType.COM4:
                case MuliBoxType.COM5:
                case MuliBoxType.COM6:
                case MuliBoxType.COM7:
                case MuliBoxType.COM8:
                    //cmw100.CMW100_GPRF_SelectPort((int)muliboxType);
                    Dictionary<string, double> temp = new Dictionary<string, double>();
                    cmw100.CMW100_GPRF_Level(-70);
                    bool S = samsung.Dut_CDMA_S_RSSITest(channel, ref rssi_PRX, ref rssi_DRX, ref ber);
                    if (rssi_PRX > -95) prefectPORT.Add(Samsung.CDMA_AntType.PRX, (int)muliboxType);
                    if (drxEnable == true && rssi_PRX > -98) prefectPORT.Add(Samsung.CDMA_AntType.DRX, (int)muliboxType);
                    break;
                case MuliBoxType.TESCOM16:
                    cmw100.CMW100_GPRF_Level(-70);
                    prefectPORT = CDMA_TestComRx(channel);
                    break;
                default:
                    throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
            }

            foreach (KeyValuePair<Samsung.CDMA_AntType, int> kv in prefectPORT)
            {
                CDMA_CalibrateLevel(channel, kv, 25);
            }
        }

        private void CDMA_RX_Test(int band, int channel, double DLfreqMHz, ref Dictionary<string, double> result)
        {
            float rssi_PRX = 0, rssi_DRX=0;
            string berstr = null;
            int rePort = 0;
            double reloss = 0.0;
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    //if (readline == null) continue;
                    if (readline == "" || readline == null) break;
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');
                    double s = Convert.ToDouble(testinfos[3].Split(':')[1]);
                    if (readline.Contains("DownLink") && readline.Contains("C2K") && Convert.ToInt32(testinfos[3].Split(':')[1]) == channel)
                    {
                        rePort = Convert.ToInt16(testinfos[2].Split(':')[1]);
                        //cmw100.CMW100_CloseAllPort();
                        cmw100.CMW100_GPRF_SelectPort(rePort);
                        KeyValuePair<Samsung.CDMA_AntType, double> prefectloss = JsonConvert.DeserializeObject<KeyValuePair<Samsung.CDMA_AntType, double>>(readarray.Last());
                        reloss = prefectloss.Value;
                        double temploss = 0.0;

                        if (prefectloss.Key == Samsung.CDMA_AntType.DRX)
                        {
                            string str01 = "drx";
                        }
                        else
                        {
                            string str02 = "prx";
                        }
                        cmw100.CMW100_GPRF_Level(-70);
                        cmw100.CB_Ver_Set_Loss(CMW100.Technology.C2K, ref temploss, ref reloss);
                        cmw100.CMW100_GPRF_Level(-70);
                        Thread.Sleep(1000);
                        float rssi = 0, ber = 0;
                        samsung.Dut_CDMA_S_RSSITest(channel, ref rssi_PRX, ref rssi_DRX, ref ber);
                        result.Add(prefectloss.Key + "_RSCP", (double)rssi);

                        bool failed = false;
                        double power = -105;
                        A: do
                        {
                            if (power > -45)
                            {
                                power = 999999;
                                break;
                            }

                            Log.GetInstance().d("Samsung", power.ToString());
                            cmw100.CMW100_GPRF_Level(power);
                            samsung.Dut_CDMA_S_TestEnd();
                            samsung.Dut_CDMA_S_SYNC(band, channel);
                            samsung.Dut_CDMA_S_RSSITest(channel, ref rssi_PRX, ref rssi_DRX, ref ber);
                            if (rssi >= 95)
                            {
                                if (failed == true) break;
                                power = power - 0.5;
                            }
                            else
                            {
                                power = power + 0.5;
                                failed = true;
                            }
                        } while (true);
                        if (power == 999999)
                        {
                            samsung.Dut_CDMA_S_TestEnd();
                            samsung.Dut_CDMA_S_SYNC(band,channel);
                            power = -105;
                            goto A;
                        }
                        result.Add(prefectloss.Key + "_Sen".ToString(), power);


                    }
                } while (true);
            }
        }

        private void CDMA_TX_Test(int band,int channel, ref Dictionary<string, double> result)
        {
            int rePort = 0;
            double reloss = 0.0;

            Samsung.TxType txType = Samsung.TxType.Main;
            using (StreamReader sr = new StreamReader("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", Encoding.Default, true))
            {
                string readline = null;
                do
                {
                    readline = sr.ReadLine();
                    if (readline == null || readline == "") break;
                    string[] readarray = readline.Split('%');
                    string[] testinfos = readarray[readarray.Count() - 2].Split('\t');
                    if (readline.Contains("UpLink") && readline.Contains("C2K") && Convert.ToInt16(testinfos[3].Split(':')[1]) == channel)
                    {
                        int antstate = 0;
                        int.TryParse(testinfos[3].Split(':')[1], out antstate);
                        samsung.Dut_Dpdt_S_Switch("0", antstate);
                        rePort = Convert.ToInt16(testinfos[2].Split(':')[1]);
                        reloss = Convert.ToDouble(readarray.Last().Replace("\t", ""));
                        double temploss = 25;
                        Dictionary<string, double> resultTx = new Dictionary<string, double>();
                        cmw100.CMW100_SelectMeasPort(rePort, CMW100.Technology.GSM);
                        cmw100.CB_Ver_Set_Loss(CMW100.Technology.C2K, ref reloss, ref temploss);
                        uint loop = 10;
                        do
                        {
                            samsung.Dut_CDMA_S_TestEnd();
                            samsung.Dut_CDMA_S_SYNC(band,channel);
                            samsung.Dut_CDMA_S_SendPower(channel, 23);

                            cmw100.CB_Ver_CDMA_S_Tx_Test(out resultTx);
                            loop--;
                            if (loop == 0) break;
                        } while (resultTx["MaxPow"] < 0);
                        foreach (KeyValuePair<string, double> kv in resultTx)
                        {
                            result.Add(antstate==0?"上天线":"下天线"+"_"+txType.ToString() + "_" + kv.Key.ToString(), kv.Value);
                        }
                    }
                } while (true);

            }
        }

        private void CDMA_CalibratePowerTestLogic(int band, int channel)
        {
            double losstempmian = 0.0;
            samsung.Dut_CDMA_S_TestEnd();
            samsung.Dut_CDMA_S_SYNC(band,channel);
            samsung.Dut_CDMA_S_SendPower(channel,23);

            if (GlodEnable)
            {
                int port = 0;
                switch (muliboxType)
                {
                    case MuliBoxType.COM1:
                    case MuliBoxType.COM2:
                    case MuliBoxType.COM3:
                    case MuliBoxType.COM4:
                    case MuliBoxType.COM5:
                    case MuliBoxType.COM6:
                    case MuliBoxType.COM7:
                    case MuliBoxType.COM8:
                        port = (int)muliboxType;
                        break;
                    case MuliBoxType.TESCOM16:
                        port = CDMA_TestComTx();
                        break;
                    default:
                        throw new Exception(string.Format("不支持{0}类型的耦合箱端口", muliboxType));
                }
                losstempmian = CDMA_CalibratePower(port, loss, channel, Samsung.TxType.Main);
                if (losstempmian == -1) CDMA_CalibratePowerTestLogic(band,channel);
            }
        }

        private int CDMA_TestComTx()
        {
            List<double> powerlist = new List<double>();
            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                cmw100.CMW100_SelectMeasPort(cmw100_portindex,CMW100.Technology.C2K);
                Dictionary<string, double> result = new Dictionary<string, double>();
                cmw100.CB_Ver_CDMA_S_Tx_Test(out result);
                powerlist.Add(result["MaxPow"]);
            }
            int prefectport = powerlist.IndexOf(powerlist.Max()) + 1;
            cmw100.CMW100_SelectMeasPort(prefectport, CMW100.Technology.C2K);
            return prefectport;
        }

        private Dictionary<Samsung.CDMA_AntType, int> CDMA_TestComRx( int channel)
        {
            float rssi_PRX = 0, rssi_DRX=0;
            float ber = 0;

            List<double> PRX_rscplist = new List<double>();
            List<double> DRX_rscplist = new List<double>();
            List<double> MIMO1_rscplist = new List<double>();
            List<double> MIMO2_rscplist = new List<double>();

            for (int cmw100_portindex = 1; cmw100_portindex <= 8; cmw100_portindex++)
            {
                cmw100.CMW100_GPRF_SelectPort(cmw100_portindex);
                Dictionary<string, double> result = new Dictionary<string, double>();
                Thread.Sleep(1000);
                samsung.Dut_CDMA_S_RSSITest(channel,  ref rssi_PRX,ref rssi_DRX, ref ber);
                PRX_rscplist.Add(Convert.ToDouble(rssi_PRX));
                DRX_rscplist.Add(Convert.ToDouble(rssi_DRX));
            }

            Dictionary<Samsung.CDMA_AntType, int> prefectPort = new Dictionary<Samsung.CDMA_AntType, int>();
            if (PRX_rscplist.Max() > -98) prefectPort.Add(Samsung.CDMA_AntType.PRX, PRX_rscplist.IndexOf(PRX_rscplist.Max()) + 1);
            if (DrxEnable && DRX_rscplist.Max() > -98) prefectPort.Add(Samsung.CDMA_AntType.DRX, DRX_rscplist.IndexOf(DRX_rscplist.Max()) + 1);


            return prefectPort;
        }

        private KeyValuePair<Samsung.CDMA_AntType, double> CDMA_CalibrateLevel(
            int channel,
            KeyValuePair<Samsung.CDMA_AntType, int> prefectport,
            double startloss)
        {

            float ber = 0;
            //cmw100.CMW100_CloseAllPort();
            cmw100.CMW100_GPRF_SelectPort(prefectport.Value);

            double ulloss = 0.0;
            double rscp = 0.0;
            int loop = 10;
            do
            {

                cmw100.CMW100_GPRF_Level(-70);
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.C2K, ref ulloss, ref startloss);
                cmw100.CMW100_GPRF_Level(-70);

                float rssi_PRX = 0;
                float rssi_DRX = 0;
                switch (prefectport.Key)
                {
                    case Samsung.CDMA_AntType.PRX:
                        samsung.Dut_CDMA_S_RSSITest(channel, ref rssi_PRX, ref rssi_DRX, ref ber);
                        rscp = rssi_PRX;
                        break;
                    case Samsung.CDMA_AntType.DRX:
                        samsung.Dut_CDMA_S_RSSITest(channel, ref rssi_PRX, ref rssi_DRX, ref ber);
                        rscp = rssi_DRX;
                        break;
                    default:
                        throw new Exception(string.Format("Samsung GSM不支持{0}这种RX类型", prefectport.Key.ToString()));
                }
                Log.GetInstance().d(prefectport.Key.ToString(), rscp.ToString());
                if (rscp <= -69.5 || rscp >= -70.5)
                {
                    startloss = startloss - (rscp + 70);
                }
                loop--;
                if (loop == 0) break;
            } while (rscp > -69.5 || rscp < -70.5);

            KeyValuePair<Samsung.CDMA_AntType, double> prefectLoss = new KeyValuePair<Samsung.CDMA_AntType, double>(prefectport.Key, startloss);

            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                sw.WriteLine(string.Format("%C2K\tDownLink\tCMW100_PORT:{0}\tFreqMHz:{1}%\t{2}", prefectport.Value, channel, JsonConvert.SerializeObject(prefectLoss)));
            }

            return prefectLoss;

        }

        private double CDMA_CalibratePower(int port, double startloss, int channel, Samsung.TxType txType)
        {
            double dlloss = 25;
            cmw100.CMW100_SelectMeasPort(port, CMW100.Technology.C2K);
            Dictionary<string, double> result = new Dictionary<string, double>();
            int loop = 10;
            do
            {
                cmw100.CB_Ver_Set_Loss(CMW100.Technology.C2K, ref startloss, ref dlloss);
                cmw100.CB_Ver_CDMA_S_Tx_Test(out result);
                if (result["MaxPow"] > 23 || result["MaxPow"] < 22)
                {
                    startloss = startloss - (result["MaxPow"] - 22.5);
                }
                loop--;
                if (loop == 0 || startloss > 60) return -1;
            } while (result["MaxPow"] > 23 || result["MaxPow"] < 22);
            using (StreamWriter sw = new StreamWriter("MainTestQueue\\MultiAntennaCounterPosition\\线损.vat", true, Encoding.Default))
            {
                sw.WriteLine(string.Format("%C2K\tUpLink:{0}\tCMW100_PORT:{1}\tFreqMHz:{2}%\t{3}", txType, port, channel, startloss));
            }
            return startloss;
        }

        private double CDMA_SetSamsung_CenterFreq(Samsung.NR_BandWidth BandWidth, uint channel, double offset = 0.0)
        {
            double freqMHz = 0.0;
            switch (BandWidth)
            {
                case Samsung.NR_BandWidth._20M:
                    if (channel >= 600000)
                    {

                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 9.18 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 9.18 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._50M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 23.94 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 23.94 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
                case Samsung.NR_BandWidth._100M:
                    if (channel >= 600000)
                    {
                        //freqMHz = 3000 + (channel - 600000) * 0.015 + 49.14 + offset;
                        freqMHz = 3000 + (channel - 600000) * 0.015 + 0 + offset;
                    }
                    else
                    {
                        //freqMHz = ((int)channel) * 0.005 + 49.14 + offset;
                        freqMHz = ((int)channel) * 0.005 + 0 + offset;
                    }
                    break;
            }
            return freqMHz;
        }
    }
}
