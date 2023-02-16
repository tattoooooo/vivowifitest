using ExcelLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using vivoautotestwifi.MainQueue;

namespace vivoautotestwifi
{
   public class Test
    {
        Pages.Wifi.WiFiConfigDong wificonfigdong;
        MainQueue.WifiTest wifitest;
        public ExcelStream excelReport;
        public String path,savepath, Not_signalIC, DPDMode, Port, IQxel, chain;

        public Test()
        { }
        public void Wifi_Test()
        {
            Control.GlobalCommunicate globalcommunicate = new Control.GlobalCommunicate();
            globalcommunicate.Connect();                                                        //验证一下是否能连接NI_Max设备

            wifitest = new MainQueue.WifiTest();                                        
            excelReport = new ExcelStream();
            excelReport.CreateNewWorkBook(true);
            path = System.IO.Directory.GetCurrentDirectory()+ @"Report\非信令报表.xlsx";        //结果报表路劲位置
            excelReport.OpenWorkBook(path);
            wifitest.excelReport = excelReport;
            wificonfigdong = new Pages.Wifi.WiFiConfigDong().Get_WiFiConfigDong_Instance();     //获得单例实例
            #region 信令测试
            if (wificonfigdong.GetSystemInformation()["WiFi_IC"].Contains("Signaling"))
            {
                String signalIC = wificonfigdong.GetSystemInformation()["WiFi_IC"];
            }
            #endregion

            #region 非信令测试
            else
            {
                int column = 0;
                if (wificonfigdong.GetSystemInformation()["TestMode"].Contains("对位"))
                {
                    
                }
                #region Transmit测试
                else
                {
                    //判断测试数据有没有填写
                    if (wificonfigdong.GetSystemInformation()["WiFi_IC"] == "" ||
                        wificonfigdong.GetSystemInformation()["TestMode"] == "" ||
                        wificonfigdong.GetSystemInformation()["TestChain"] == "" ||
                        wificonfigdong.GetSystemInformation()["Limit_2G4"] == "" ||
                        wificonfigdong.GetSystemInformation()["Limit_5G"] == "" ||
                        //string.IsNullOrEmpty(Pages.WiFi.WiFiConfig.wificonfig.GetSystemInformation()["Port"]) ||
                        String.IsNullOrEmpty(wificonfigdong.GetSystemInformation()["IP"]))
                    {
                        MessageBox.Show("请检查是否遗漏配置IP参数!", "提示");
                    }

                    #region 标准文件
                    wifitest.CriteriasPath_2G4 = wificonfigdong.GetSystemInformation()["Limit_2G4"];
                    wifitest.CriteriasPath_2G4 = wificonfigdong.GetSystemInformation()["Limit_5G"];
                    #endregion

                    wifitest.TXbool = wificonfigdong.GetTestItem()["TX"];
                    wifitest.SENbool = wificonfigdong.GetTestItem()["SEN"];
                    wifitest.MILbool = wificonfigdong.GetTestItem()["MIL"];
                    wifitest.RSSIbool = wificonfigdong.GetTestItem()["RSSIbool"];
                    wifitest.WaterFall = wificonfigdong.GetTestItem()["WaterFall"];

                    wificonfigdong.GetSystemInformation().TryGetValue("WiFi_IC", out Not_signalIC);
                    DPDMode = wificonfigdong.GetSystemInformation()["DPDMode"];
                    Port = wificonfigdong.GetSystemInformation()["Port"];
                    Dictionary<String, String> LossDic = wificonfigdong.GetLoss();
                    List<String> RateChecked = wificonfigdong.GetRate();
                    Dictionary<String, String> ChannelAndFreq = wificonfigdong.GetArgumentChannel();
                    Dictionary<String, String> enpdic = wificonfigdong.GetRateAndENP();
                    IQxel = wificonfigdong.GetSystemInformation()["Instrument"].Replace("IQxel", "");
                    chain = wificonfigdong.GetSystemInformation()["TestChain"];
                    /*
                        wifitest.WiFiTransmit(
                        Pages.WiFi.WiFiConfig.wificonfig.GetSystemInformation()["IP"],
                        Pages.WiFi.WiFiConfig.wificonfig.GetSystemInformation()["Port"],
                        Pages.WiFi.WiFiConfig.wificonfig.GetLoss(),
                        Pages.WiFi.WiFiConfig.wificonfig.GetRate(),
                        Pages.WiFi.WiFiConfig.wificonfig.GetArgumentChannel(),
                        Pages.WiFi.WiFiConfig.wificonfig.GetRateAndENP(),
                        ic,
                        Pages.WiFi.WiFiConfig.wificonfig.GetSystemInformation()["Instrument"].Replace("IQxel",""),
                        0,Symbol
                    );
                     */
                }
                #endregion
            }
            #endregion
        }

       
    }
}
