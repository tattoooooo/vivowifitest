using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelLibrary;

namespace vivoautotestwifi.MainQueue
{
   public class WifiTest
    {
        WiFi_Function wififunctiontest = new WiFi_Function();
        public ExcelStream excelReport;
        public string CriteriasPath_5G = null;
        public string CriteriasPath_2G4 = null;
        public short relport;
        public bool? TXbool = false;
        public bool? SENbool = false;
        public bool? MILbool = false;
        public bool? RSSIbool = false;
        public bool? WaterFall = false;
        private String IP = null;
        string samsungport = null;
        Dictionary<String, String> LossDicbuff = new Dictionary<String, String>();

        /// <summary>
        /// WiFi传导测试
        /// </summary>
        /// <param name="ip">仪器IP地址</param>
        /// <param name="port">手机端口</param>
        /// <param name="LossDic">线损字典</param>
        /// <param name="RateChecked">测试数率列表</param>
        /// <param name="ChannelAndFreq">信道字典</param>
        /// <param name="enpdic">期望功率字典</param>
        /// <param name="IC">WiFi芯片：3660、3688、3990、6174、6391</param>
        /// <param name="IQxel">update 2019 09 27 添加参数 - 仪器选择 another:hp </param>
        public void WiFiTransmit(String ip,String port, Dictionary<String,String> LossDic,List<String> RateChecked,Dictionary<String,String> ChannelAndFreq,Dictionary<String,String> enpdic,String IC,String IQxel,String chain,int column,int Symbol,String DPDMode )
        {
            List<String> reportNameList = new List<String>();
            int testCount = 0;
            LossDicbuff = LossDic;
            IP = ip;
            if (IC != "Samsung")
            {
                try
                {
                    relport = (short)Convert.ToInt32(port);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("port项出问题:"+ex);
                }
            }
            else
            {
                samsungport = port;
            }
            try 
            {
                if (TXbool == true || SENbool == true)
                {
                    ConnectAllInstrument(ip, port, IC, IQxel);
                }
                switch (chain)
                {
                    case "上天线":
                        Argument_AllItems_Test(IC, IQxel, RateChecked, ChannelAndFreq, enpdic, Symbol, testCount, DPDMode, ref reportNameList, "1", column);
                        break;
                    case "下天线":
                        Argument_AllItems_Test(IC, IQxel, RateChecked, ChannelAndFreq, enpdic, Symbol, testCount, DPDMode, ref reportNameList, "2", column);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
   

        //连接仪器还有手机的方法
        private void ConnectAllInstrument(String ip, String port, String IC, String IQxel)
        {
            wififunctiontest.ConnectIQxelIP(ip, IQxel); //连接仪器
            if (IC.Equals("Samsung"))
            {
                
            }
            else if (IC.Equals("MT6635X"))
            {

            }
            else                // 高通 || MTK 芯片
            {
                wififunctiontest.ConnectPhone((short)Convert.ToInt32(port), IC);
                wififunctiontest.PhoneIni(IC);
            }
        }

        /// <summary>
        /// 单个协议全指标测试方法
        /// </summary>
        /// <param name="IC">WiFi芯片</param>
        /// <param name="RateChecked">测试速率列表</param>
        /// <param name="ChannelAndFreq">测试信道字典</param>
        /// <param name="enpdic">期望功率字典</param>
        /// <param name="chain">天线编号：1：上天线，2：下天线</param>
        /// <param name="IQxel">仪器名</param>
        public void Argument_AllItems_Test(String IC, String IQxel, List<String> RateChecked, Dictionary<String, String> ChannelAndFreq, Dictionary<String, String> enpdic, int Symbol, int testCount, String DPDMode, ref List<String> reportNameList, String chain = "1", int column = 0)
        {
            List<String> argumentList = new List<String>();
            List<String> reportNameListt = new List<String>();
            int argumentchangeindex = 0;
            String argumentbuff = null;
            try
            {
                foreach (String rateitem in RateChecked)
                {
                    argumentList.Add(GetArgument(rateitem));
                }
                for (int i=0;i<RateChecked.Count;i++)
                {
                    int flag = RateChecked.Count();
                    String Ratestr = RateChecked[i];
                    String Argument = GetArgument(Ratestr);
                    String channelandfreq = null;
                    ChannelAndFreq.TryGetValue(Argument,out channelandfreq);
                    for (int channelindex = 0;channelindex < channelandfreq.Split(';')[0].Split(',').Length;channelindex++)
                    { 
                        
                    }
                }
            } catch (Exception e)
            {
                MessageBox.Show("报错信息:"+e.ToString());
            }
        }

        private String GetArgument(String rate)
        {
            String Argument = null;
            if (rate.Contains("2G4_B"))
            {
                Argument = "2G4_B";
            }
            else if (rate.Contains("2G4_G"))
            {
                Argument = "2G4_G";
            }
            else if (rate.Contains("2G4_N20M"))
            {
                Argument = "2G4_N20M";
            }
            else if (rate.Contains("2G4_N40M"))
            {
                Argument = "2G4_N40M";
            }
            else if (rate.Contains("2G4_AC20M"))
            {
                Argument = "2G4_AC20M";
            }
            else if (rate.Contains("2G4_AX20M"))
            {
                Argument = "2G4_AX20M";
            }
            else if (rate.Contains("2G4_AX40M"))
            {
                Argument = "2G4_AX40M";
            }
            else if (rate.Contains("5G1_A_"))
            {
                Argument = "5G1_A";
            }
            else if (rate.Contains("5G1_N20M"))
            {
                Argument = "5G1_N20M";
            }
            else if (rate.Contains("5G1_N40M"))
            {
                Argument = "5G1_N40M";
            }
            else if (rate.Contains("5G1_AC20M"))
            {
                Argument = "5G1_AC20M";
            }
            else if (rate.Contains("5G1_AC40M"))
            {
                Argument = "5G1_AC40M";
            }
            else if (rate.Contains("5G1_AC80M"))
            {
                Argument = "5G1_AC80M";
            }
            else if (rate.Contains("5G1_AX20M"))
            {
                Argument = "5G1_AX20M";
            }
            else if (rate.Contains("5G1_AX40M"))
            {
                Argument = "5G1_AX40M";
            }
            else if (rate.Contains("5G1_AX80M"))
            {
                Argument = "5G1_AX80M";
            }
            else if (rate.Contains("5G1_AX160M"))
            {
                Argument = "5G1_AX160M";
            }
            else if (rate.Contains("5G4_A_"))
            {
                Argument = "5G4_A";
            }
            else if (rate.Contains("5G4_N20M"))
            {
                Argument = "5G4_N20M";
            }
            else if (rate.Contains("5G4_N40M"))
            {
                Argument = "5G4_N40M";
            }
            else if (rate.Contains("5G4_AC20M"))
            {
                Argument = "5G4_AC20M";
            }
            else if (rate.Contains("5G4_AC40M"))
            {
                Argument = "5G4_AC40M";
            }
            else if (rate.Contains("5G4_AC80M"))
            {
                Argument = "5G4_AC80M";
            }
            else if (rate.Contains("5G4_AX20M"))
            {
                Argument = "5G4_AX20M";
            }
            else if (rate.Contains("5G4_AX40M"))
            {
                Argument = "5G4_AX40M";
            }
            else if (rate.Contains("5G4_AX80M"))
            {
                Argument = "5G4_AX80M";
            }
            else if (rate.Contains("5G4_AX160M"))
            {
                Argument = "5G4_AX160M";
            }
            else if (rate.Contains("5G8_A_"))
            {
                Argument = "5G8_A";
            }
            else if (rate.Contains("5G8_N20M"))
            {
                Argument = "5G8_N20M";
            }
            else if (rate.Contains("5G8_N40M"))
            {
                Argument = "5G8_N40M";
            }
            else if (rate.Contains("5G8_AC20M"))
            {
                Argument = "5G8_AC20M";
            }
            else if (rate.Contains("5G8_AC40M"))
            {
                Argument = "5G8_AC40M";
            }
            else if (rate.Contains("5G8_AC80M"))
            {
                Argument = "5G8_AC80M";
            }
            else if (rate.Contains("5G8_AX20M"))
            {
                Argument = "5G8_AX20M";
            }
            else if (rate.Contains("5G8_AX40M"))
            {
                Argument = "5G8_AX40M";
            }
            else if (rate.Contains("5G8_AX80M"))
            {
                Argument = "5G8_AX80M";
            }
            else if (rate.Contains("5G8_AX160M"))
            {
                Argument = "5G8_AX160M";
            }
            return Argument;
        }
    }
}
