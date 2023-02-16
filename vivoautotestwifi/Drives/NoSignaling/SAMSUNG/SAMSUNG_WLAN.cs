using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SamsungATClass = vivoautotestwifi.Drives.NoSignaling.SAMSUNG.SamsungATClass;

//WZT
namespace vivoautotestwifi.Drives.NoSignaling.SAMSUNG
{
    class SAMSUNG_WLAN
    {
        private int Samsung_port = 0;

        private SamsungATClass samsungATClass;

        public string adbresource = null;

        public SAMSUNG_WLAN(string adbResource, int port)
        {
            Log.GetInstance().d("SAMSUNG_WLAN", adbResource);
            Samsung_port = port;
            SamsungATClass.port = port;
            samsungATClass = new SamsungATClass();
            adbresource = adbResource;
        }

        public enum SamsungWlanBandWidth
        {
            b_g_a_n_20MHZ = 1,
            n_40MHZ = 2,
            ac_20MHZ = 4,
            ac_40MHZ = 5,
            ac_80MHZ = 3,
            ax_20MHZ = 6,
            ax_40MHZ = 7,
            ax_80MHZ = 8
        }

        public enum AntMode
        {
            MIMO = 0,
            SISOFir = 1,
            SISOSec = 2
        }

        public enum WlanBand
        {
            _2G4=1,
            _5G=2
        }

        public enum Argument
        {
            B = 0,
            G,
            A,
            N_2G4,
            N_5G,
            AC_20M,
            AC_40M,
            AC_80M,
            AX_20M,
            AX_40M,
            AX_80M
        }

        public enum Preamble
        {
            Prem_Short = 1,
            Prem_Long = 2,
            Prem_ShortGI = 3,
            Prem_LongGI = 4,
            Prem_0_8GI = 5,
            Prem_1_6GI = 6,
            Prem_3_2GI = 7
        }

        public void Open_WALN()
        {
            samsungATClass.Ini(adbresource);
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,1,1,0");
        }

        public void Close_WLAN()
        {
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,1,0,0");
            
         }

        public void DisposeAllResourse()
        {
            samsungATClass.Close();
            samsungATClass.CloseAdbServer();
        }

        private string[,] SamsungDataRate = new string[11, 16] {
            {"1","2","3","6","","","","","","","","","","","",""},
            {"4","5","7","8","10","12","13","14","","","","","","","",""},
            {"4","5","7","8","10","12","13","14","","","","","","","",""},
            {"15","16","17","18","19","20","21","22","","24","25","26","27","28","29","30"},//N2G4
            {"15","16","17","18","19","20","21","22","","24","25","26","27","28","29","30"},//N5G
            {"15","16","17","18","19","20","21","22","23","24","","","","","",""},//AC_20M
            {"15","16","17","18","19","20","21","22","23","24","","","","","",""},//AC_40M
            {"15","16","17","18","19","20","21","22","23","24","","","","","",""},//AC_80M
            {"15","16","17","18","19","20","21","22","23","24","25","26","","","",""},//AX_20M
            {"15","16","17","18","19","20","21","22","23","24","25","26","","","",""},//AX_40M
            {"15","16","17","18","19","20","21","22","23","24","25","26","","","",""}//AX_80M
        };


        public void SetBandWidth(SamsungWlanBandWidth BW)
        {
            samsungATClass.SamsungAT_Send(String.Format("AT+WIFIRF=0,0,7,{0}", ((uint)BW).ToString()));
        }

        public void SetAnt(AntMode antMode)
        {
            samsungATClass.SamsungAT_Send(String.Format("AT+WIFIRF=0,7,0,{0}", ((uint)antMode).ToString()));
        }

        public void SetBand(WlanBand wlanBand)
        {
            samsungATClass.SamsungAT_Send(String.Format("AT+WIFIRF=0,0,6,{0}", ((uint)wlanBand).ToString()));
        }

        public void SetChannel(uint channel)
        {
            Log.GetInstance().d("Samsung","channel="+ channel.ToString());
            samsungATClass.SamsungAT_Send(String.Format("AT+WIFIRF=0,0,0,{0}", channel.ToString()));
        }

        public void SetDataRate(Argument argument,uint DataRateIndex)
        {
            samsungATClass.SamsungAT_Send(String.Format("AT+WIFIRF=0,0,1,{0}", SamsungDataRate[(uint)argument, DataRateIndex]));
        }

        public void SetFormat(Preamble preamble)
        {
            samsungATClass.SamsungAT_Send(String.Format("AT+WIFIRF=0,0,2,{0}", ((uint)preamble).ToString()));
        }

        public void SetEnp(float expPower)
        {
            samsungATClass.SamsungAT_Send(string.Format("AT+WIFIRF=0,0,3,{0:F}", expPower.ToString()));
        }

		// author 何苹 Samsung WiFi MIMO
        public void SetOthorTx()
        {
            //samsungATClass.SamsungAT_Send("AT+WIFIRF=0,0,2,2");
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,0,5,100");
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,0,4,1024");
        }
        public void SetStartTx()
        {
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,2,1,0");
        }

        public void SetStopTx()
        {
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,2,0,0");
        }

        public void SetStartRx()
        {
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,3,1,0");
            System.Threading.Thread.Sleep(200);
        }

        public void SetStopRx()
        {
            samsungATClass.SamsungAT_Send("AT+WIFIRF=0,3,0,0");
        }

        public enum PacketType
        {
            errorPacket,
            goodPacket,
        }

        public Dictionary<PacketType,Int32> GetRxPacket()
        {
            SetStopRx();
            Dictionary<PacketType, Int32> PacketDic = new Dictionary<PacketType, Int32>();
            string goodPacket = samsungATClass.SamsungAT_Read("AT+WIFIRF=0,4,1,0");
            string errorPacket= samsungATClass.SamsungAT_Read("AT+WIFIRF=0,4,0,0");
            PacketDic.Add(PacketType.errorPacket, Convert.ToInt32(errorPacket));
            
            PacketDic.Add(PacketType.goodPacket, Convert.ToInt32(goodPacket));
            return PacketDic;
        }

        public int GetRxRssi()
        {
            string rssi = null;
            SetStopRx();
            rssi = samsungATClass.SamsungAT_Read("AT+WIFIRF=0,4,2,0");
            //System.Threading.Thread.Sleep(1000);
            //rssi = samsungATClass.SamsungAT_Read("AT+WIFIRF=0,4,2,0");
            //System.Threading.Thread.Sleep(1000);
            //rssi = samsungATClass.SamsungAT_Read("AT+WIFIRF=0,4,2,0");
            //Regex re = new Regex("(?<=\").*?(?=\")", RegexOptions.None);
            //MatchCollection mc = re.Matches(rssi);
            //rssi = mc[0].Value;
            return Convert.ToInt32(rssi);
        }
    }
}
