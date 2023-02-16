using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace vivoautotestwifi.Pages.Wifi
{
    /// <summary>
    /// WiFiConfigDong.xaml 的交互逻辑
    /// </summary>
    public partial class WiFiConfigDong : UserControl
    {
        public static WiFiConfigDong wificonfigdong;
        String testype = null;
        FirstFloor.ModernUI.Windows.Controls.ModernDialog dlg;
        private readonly object mylock = new object();

        public WiFiConfigDong()
        {
            InitializeComponent();
            wificonfigdong = this;
            XmlDocument docs = new XmlDocument();
            docs.Load("XmlConfig\\SystemConfig.xml");
            testype = ((XmlElement)((XmlElement)((XmlElement)docs.GetElementsByTagName("page")[0]).GetElementsByTagName("wifi")[0]).GetElementsByTagName("wifipageconfig")[0]).GetAttribute("Testtype");   //Testtype=1
            if (testype != "0")
            {
                UI_Config(@"Pages\Wifi\WiFiConfigDong.xml"); // !=0 非信令测试 
            }
        }

        public WiFiConfigDong Get_WiFiConfigDong_Instance()
        {
            if (wificonfigdong == null)
            {
                lock (mylock)
                {
                    if (wificonfigdong == null)
                    {
                        wificonfigdong = new WiFiConfigDong();
                    }
                }
            }
            return wificonfigdong;
        }

        private void UI_Config(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            #region TestInformation
            //foreach (XmlElement testinformation in doc.GetElementsByTagName("TestInformation"))
            //{
            //    foreach (XmlElement items in testinformation.GetElementsByTagName("Item"))
            //    {
            //        int width = 0;
            //        switch (items.GetAttribute("name"))
            //        {
            //            case "Tester":
            //                width = 50;
            //                break;
            //            case "ProjectName":
            //                width = 80;
            //                break;
            //            case "ProjectStage":
            //                width = 80;
            //                break;
            //            case "HardwareVersion":
            //                width = 110;
            //                break;
            //            case "SoftwareVersion":
            //                width = 110;
            //                break;
            //        }
            //        StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5), HorizontalAlignment = HorizontalAlignment.Left };
            //        stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = width, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Left });
            //        stackpanel.Children.Add(new TextBox()
            //        {
            //            Name = items.GetAttribute("name"),
            //            Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            //            Width = 100,
            //            Text = "1",
            //            Style = Resources["TextBoxStyle1"] as Style
            //        });
            //        TestInformation.Children.Add(stackpanel);
            //    }
            //}
            #endregion
            foreach (XmlElement systeminformation in doc.GetElementsByTagName("SystemInformation"))
            {
                foreach (XmlElement items in systeminformation.GetElementsByTagName("WiFi_IC"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 50, FontWeight = FontWeights.Bold });
                    ComboBox combobox = new ComboBox()
                    {
                        //Name = items.GetAttribute("name").Replace(" ", "_"),
                        Name = "WiFi_IC",
                        Items = { "3660", "3680", "3990", "6174", "MTK", "MTK New", "MT6635X", "6391", "6750",/* "3660 Signaling", "3680 Signaling", "39xx Signaling", "MTK Signaling" */"Signaling", "AT", "Samsung", "NI WTS" },
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 60,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    combobox.SelectionChanged += SELECTIONCHANGED;                          //这里Combobox子选项点击后下面页面加载更新 line1035
                    stackpanel.Children.Add(combobox);
                    SystemInformation.Children.Add(stackpanel);
                }
                foreach (XmlElement items in systeminformation.GetElementsByTagName("TestMode"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 60, FontWeight = FontWeights.Bold });
                    ComboBox combobox = new ComboBox()
                    {
                        //Name = items.GetAttribute("name"),
                        Name = "TestMode",
                        Items = { "Transmit", "ACSI", "对位", "RSSI", "耦合灵敏度" },
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 80,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    combobox.SelectionChanged += SELECTIONCHANGED;
                    stackpanel.Children.Add(combobox);
                    SystemInformation.Children.Add(stackpanel);
                }

                foreach (XmlElement items in systeminformation.GetElementsByTagName("Instrument"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 60, FontWeight = FontWeights.Bold });
                    ComboBox combobox = new ComboBox()
                    {
                        //Name = items.GetAttribute("name"),
                        Name = "Instrument",
                        Items = { "CMW270", "CMW500", "CMW100", "IQxel80", "IQxel160", "IQxelMW" },
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 80,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    //combobox.SelectionChanged += SELECTIONCHANGED;
                    stackpanel.Children.Add(combobox);
                    SystemInformation.Children.Add(stackpanel);
                }

                foreach (XmlElement items in systeminformation.GetElementsByTagName("IsAutoControl"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 60, FontWeight = FontWeights.Bold });
                    ComboBox combobox = new ComboBox()
                    {
                        //Name = items.GetAttribute("name"),
                        Name = "IsAutoControl",
                        Items = { "USB", "None" },
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 80,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    //combobox.SelectionChanged += SELECTIONCHANGED;
                    stackpanel.Children.Add(combobox);
                    SystemInformation.Children.Add(stackpanel);
                }

                foreach (XmlElement items in systeminformation.GetElementsByTagName("TestChain"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 60, FontWeight = FontWeights.Bold });
                    ComboBox combobox = new ComboBox()
                    {
                        //Name = items.GetAttribute("name"),
                        Name = "TestChain",
                        Items = { "上天线", "下天线", "MIMO" },
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 70,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    stackpanel.Children.Add(combobox);
                    SystemInformation.Children.Add(stackpanel);
                }
                foreach (XmlElement items in systeminformation.GetElementsByTagName("Port"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 30, FontWeight = FontWeights.Bold });
                    ComboBox combobox = new ComboBox()
                    {
                        //Name = items.GetAttribute("name"),
                        Name = "Port",
                        Items = { "FindPort", "Samsung_FindPort" },
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 90,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    combobox.SelectionChanged += SELECTIONCHANGED;
                    stackpanel.Children.Add(combobox);
                    SystemInformation.Children.Add(stackpanel);
                }
                foreach (XmlElement items in systeminformation.GetElementsByTagName("WaveFile"))
                {
                    StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackPanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 60, FontWeight = FontWeights.Bold });
                    ComboBox comboBox = new ComboBox()
                    {
                        Name = "WaveFile",
                        Items = { "Normal", "LDPC" },
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 90,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    comboBox.SelectionChanged += SELECTIONCHANGED;
                    stackPanel.Children.Add(comboBox);
                    SystemInformation.Children.Add(stackPanel);
                }

                foreach (XmlElement items in systeminformation.GetElementsByTagName("DPDMode"))
                {
                    StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    TextBlock textBlock = new TextBlock() { Text = items.GetAttribute("name"), Width = 65, FontWeight = FontWeights.Bold };
                    stackPanel.Children.Add(textBlock);
                    ComboBox comboBox = new ComboBox()
                    {
                        Name = "DPDMode",
                        Items = { "ON", "OFF" },
                        // Text = "OFF",
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 90,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    comboBox.SelectionChanged += SELECTIONCHANGED;
                    stackPanel.Children.Add(comboBox);
                    SystemInformation.Children.Add(stackPanel);
                }
                ////界面添加仪器选择项
                //foreach (XmlElement items in systeminformation.GetElementsByTagName("IQxel"))
                //{
                //    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                //    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 35, FontWeight = FontWeights.Bold });
                //    ComboBox combobox = new ComboBox()
                //    {
                //        Name = "IQxel",
                //        Items = { "80", "160", "MW" },
                //        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                //        Width = 60,
                //        Style = Resources["ComboBoxStyle1"] as Style
                //    };
                //    stackpanel.Children.Add(combobox);
                //    SystemInformation.Children.Add(stackpanel);
                //}

                foreach (XmlElement items in systeminformation.GetElementsByTagName("Limits"))
                {
                    StackPanel stackpanel = new StackPanel() { Name = "Limits", Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    //stackpanel.Children.Add(new TextBlock() { /*Text = items.GetAttribute("name"),*/ Width = 15, FontWeight = FontWeights.Bold });

                    Button button_2G4 = new Button()
                    {
                        Name = "Limit_2G4",
                        Content = "请选择2.4G测试标准",
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 80,
                        Style = Resources["ComboBoxStyle1"] as Style,
                    };

                    Button button_5G = new Button()
                    {
                        Name = "Limit_5G",
                        Content = "请选择5G测试标准",
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 80,
                        Style = Resources["ComboBoxStyle1"] as Style
                    };
                    button_2G4.Click += Button_Click;
                    button_5G.Click += Button_Click;
                    button_2G4.MouseEnter += MOUSEENTER;
                    button_5G.MouseEnter += MOUSEENTER;
                    button_2G4.MouseLeave += MOUSELEAVE;
                    button_5G.MouseLeave += MOUSELEAVE;

                    stackpanel.Children.Add(button_2G4);
                    stackpanel.Children.Add(button_5G);
                    SystemInformation.Children.Add(stackpanel);
                }
                foreach (XmlElement items in systeminformation.GetElementsByTagName("Item"))
                {
                    double width = 0.0;
                    if (items.GetAttribute("name") == "IP")
                    {
                        width = 120;
                    }
                    else
                    {
                        width = 30;
                    }
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 0, 0, 5) };
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name"), Width = 20, FontWeight = FontWeights.Bold });
                    stackpanel.Children.Add(new TextBox() { Name = items.GetAttribute("name"), Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Width = width, Text = "192.168.100.253", Style = Resources["TextBoxStyle1"] as Style });
                    SystemInformation.Children.Add(stackpanel);
                }
            }
        }
        private void ChildrenUIConfig(String path)
        {
            Loss.Children.Clear();
            Channel.Children.Clear();
            BandChecked.Children.Clear();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            foreach (XmlElement TestArgument in doc.GetElementsByTagName("TestArgument"))
            {
                foreach (XmlElement TestBand in TestArgument.GetElementsByTagName("TestBAND"))          //2G4
                {
                    #region //添加线损

                    StackPanel lossstackpannel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    lossstackpannel.Children.Add(new TextBlock()
                    {
                        Text = TestBand.GetAttribute("name"),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(0, 0, 0, 0),
                        Height = 23,
                        //Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        FontWeight = FontWeights.Bold
                    });
                    lossstackpannel.Children.Add(new TextBox()
                    {
                        Name = "_" + TestBand.GetAttribute("name") + "_Loss",
                        Width = 50,
                        Height = 23,
                        Text = TestBand.GetAttribute("text"),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Style = Resources["TextBoxStyle1"] as Style,
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255))
                    });
                    Loss.Children.Add(lossstackpannel);
                    #endregion
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(31, 0, 0, 3) };
                    WrapPanel ChannelWrapPanel = new WrapPanel();                                       //信道ChannelWrapPanel容器
                    foreach (XmlElement Argument in TestBand.GetElementsByTagName("Argument"))          //B,G
                    {
                        #region//添加信道
                        string channelstr = null, channelfreqstr = null;
                        switch (Argument.GetAttribute("name"))
                        {
                            case "B":
                            case "G":
                            case "N20M":
                                if (TestBand.GetAttribute("name").Contains("5G"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "36,48,64,149,157,165";
                                    channelfreqstr = "5180,5240,5320,5745,5785,5825";
                                }
                                else if (TestBand.GetAttribute("name").Contains("2G4"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "1,6,13";
                                    channelfreqstr = "2412,2437,2472";
                                }
                                break;
                            case "N40M":
                                if (TestBand.GetAttribute("name").Contains("5G"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "38,54,62,151,159";
                                    channelfreqstr = "5190,5270,5310,5755,5795";
                                }
                                else if (TestBand.GetAttribute("name").Contains("2G4"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "1,6,13";
                                    channelfreqstr = "2412,2437,2472";
                                }
                                break;
                            case "A":
                            case "AC20M":
                                if (TestBand.GetAttribute("name").Contains("5G"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "36,48,64,149,157,165";//36,48,64,149,157,165
                                    channelfreqstr = "5180,5240,5320,5745,5785,5825";//5180,5240,5320,5745,5785,5825
                                }
                                else if (TestBand.GetAttribute("name").Contains("2G4"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "1,6,13";
                                    channelfreqstr = "2412,2437,2472";
                                }
                                break;
                            case "AC40M":
                                //channelstr = Argument.GetAttribute("Channelstr");
                                //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                channelstr = "38,54,62,151,159";
                                channelfreqstr = "5190,5270,5310,5755,5795";
                                break;
                            case "AC80M":
                                //channelstr = Argument.GetAttribute("Channelstr");
                                //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                channelstr = "42,58,155";
                                channelfreqstr = "5210,5290,5775";
                                break;
                            case "AX20M":
                                if (TestBand.GetAttribute("name").Contains("5G"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "36,48,64,149,157,165";//36,48,64,149,157,165
                                    channelfreqstr = "5180,5240,5320,5745,5785,5825";//5180,5240,5320,5745,5785,5825
                                }
                                else
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "1,6,13";
                                    channelfreqstr = "2412,2437,2472";
                                }
                                break;
                            case "AX40M":
                                if (TestBand.GetAttribute("name").Contains("5G"))
                                {
                                    //channelstr = Argument.GetAttribute("Channelstr");
                                    //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                    channelstr = "38,54,62,151,159";
                                    channelfreqstr = "5190,5270,5310,5755,5795";
                                }
                                else
                                {
                                    channelstr = "1,6,13";
                                    channelfreqstr = "2412,2437,2472";
                                }

                                break;
                            case "AX80M":
                                channelstr = "42,58,155";
                                channelfreqstr = "5210,5290,5775";
                                //channelstr = Argument.GetAttribute("Channelstr");
                                //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                break;
                            case "AX160M":
                                channelstr = "50";
                                channelfreqstr = "5250";
                                //channelstr = Argument.GetAttribute("Channelstr");
                                //channelfreqstr = Argument.GetAttribute("ChannelFreqstr");
                                break;
                        }

                        StackPanel channelstackpanel = new StackPanel()
                        {
                            Orientation = Orientation.Horizontal,
                            Name = "_" + TestBand.GetAttribute("name").Replace(".", "_") + "_" + Argument.GetAttribute("name") + "_ChannelContain",
                        };

                        channelstackpanel.Children.Add(new TextBlock()                      //2G4 Label
                        {
                            Text = TestBand.GetAttribute("name").Replace(".", "_") + "_" + Argument.GetAttribute("name"),
                            Margin = new Thickness(10, 4, 0, 0),
                            Width = 80,
                            //Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                            FontWeight = FontWeights.Bold
                        });
                        TextBox channeltextbox = new TextBox()                              //1,6,13输入框
                        {
                            Name = "_" + TestBand.GetAttribute("name").Replace(".", "_") + "_" + Argument.GetAttribute("name") + "_CH",
                            Width = 140,
                            Margin = new Thickness(1, 2, 0, 2),
                            Text = channelstr,
                            Style = Resources["TextBoxStyle1"] as Style,
                            Background = new SolidColorBrush(Color.FromRgb(255, 255, 255))
                        };
                        channeltextbox.TextChanged += TEXTCHANGED;
                        channelstackpanel.Children.Add(channeltextbox);

                        channelstackpanel.Children.Add(new TextBlock()                      //Freq Label
                        {
                            Text = "Freq",
                            Margin = new Thickness(10, 4, 0, 0),
                            Width = 30,
                            //Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                            FontWeight = FontWeights.Bold
                        });
                        channelstackpanel.Children.Add(new TextBox()                        //2412,2437输入框
                        {
                            Name = "_" + TestBand.GetAttribute("name").Replace(".", "_") + "_" + Argument.GetAttribute("name") + "_CH",
                            Width = 200,
                            Margin = new Thickness(1, 2, 0, 2),
                            IsEnabled = false,
                            Text = channelfreqstr,
                            Style = Resources["TextBoxStyle1"] as Style,
                            Background = new SolidColorBrush(Color.FromRgb(255, 255, 255))
                        });
                        ChannelWrapPanel.Children.Add(channelstackpanel);
                        #endregion

                        StackPanel ArgumentStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                        //B协议CheckBox
                        CheckBox argumentcheckedbox = new CheckBox() { Content = Argument.GetAttribute("name"), Name = "_" + TestBand.GetAttribute("name") + "_" + Argument.GetAttribute("name"), Margin = new Thickness(3, 0, 0, 3), Style = Resources["ItemCheckStyle"] as Style };
                        argumentcheckedbox.Checked += CHECKED;
                        argumentcheckedbox.Unchecked += UNCHECKED;
                        ArgumentStackPanel.Children.Add(argumentcheckedbox);
                        #region 废弃代码
                        //加入信道输入框
                        //ChannelStackPanel.Children.Add(new TextBlock() { Text = "CH",Margin=new Thickness(10,4,0,0) });
                        //ChannelStackPanel.Children.Add(new TextBox()
                        //{
                        //    Name = "_" + TestBand.GetAttribute("name").Replace(".", "_") + "_" + Argument.GetAttribute("name") + "_CH",
                        //    Width = 135,
                        //    Margin=new Thickness(1,2,0,2)
                        //});
                        //加入信道频率输入框
                        //ChannelStackPanel.Children.Add(new TextBlock() { Text = "Freq", Margin = new Thickness(10, 4, 0, 0) });
                        //ChannelStackPanel.Children.Add(new TextBox()
                        //{
                        //    Name = "_" + TestBand.GetAttribute("name").Replace(".", "_") + "_" + Argument.GetAttribute("name") + "_Freq",
                        //    Width = 135,
                        //    Margin = new Thickness(1, 2, 0, 2)
                        //});
                        #endregion
                        stackpanel.Children.Add(ArgumentStackPanel);
                        WrapPanel wrappanel = new WrapPanel() { Margin = new Thickness(31, 0, 0, 3) };  //信道rate容器
                        foreach (XmlElement rate in Argument.GetElementsByTagName("Rate"))              //1Mbit/s
                        {
                            CheckBox checkbox = new CheckBox()
                            {
                                Content = rate.GetAttribute("name"),
                                Margin = new Thickness(3, 3, 0, 3),
                                Width = 80,
                                Name = "_" + TestBand.GetAttribute("name").Replace(".", "_") + "_"
                                        + Argument.GetAttribute("name") + "_"
                                        + rate.GetAttribute("name").Replace("/", "_").Replace(".", "_"),
                                Style = Resources["ItemCheckStyle"] as Style
                            };
                            checkbox.Checked += CHECKED;
                            checkbox.Unchecked += UNCHECKED;
                            wrappanel.Children.Add(checkbox);
                        }
                        stackpanel.Children.Add(wrappanel);//B+1Mbit/s....
                    }
                    StackPanel ParentStackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                    StackPanel TestBandStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    //2G4
                    CheckBox bandcheckedbox = new CheckBox() { Content = TestBand.GetAttribute("name").Replace("1", ""), Margin = new Thickness(3, 0, 0, 3), Style = Resources["ItemCheckStyle"] as Style };
                    bandcheckedbox.Checked += CHECKED;
                    bandcheckedbox.Unchecked += UNCHECKED;

                    TestBandStackPanel.Children.Add(bandcheckedbox);        //2G4
                    ParentStackPanel.Children.Add(TestBandStackPanel);      //B+1Mbit/s....
                    ParentStackPanel.Children.Add(stackpanel);

                    BandChecked.Children.Add(ParentStackPanel);     //Test Rate 下的
                    Channel.Children.Add(ChannelWrapPanel);         //Channel 下的
                }

                StackPanel Itemstackpanel = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 0, 0) };
                foreach (XmlElement TestItem in TestArgument.GetElementsByTagName("TestItem"))
                {
                    foreach (XmlElement Item in TestItem.GetElementsByTagName("Item"))
                    {
                        Itemstackpanel.Children.Add(new CheckBox() { Content = Item.GetAttribute("name"), Style = Resources["ItemCheckStyle"] as Style });
                    }
                }
                TestItemContain.Children.Clear();
                StackPanel ItemParentStackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                ItemParentStackPanel.Children.Add(Itemstackpanel);
                TestItemContain.Children.Add(ItemParentStackPanel);
            }
            State.Children.Clear();
            if (path.Contains("Signaling"))
                foreach (XmlElement state in ((XmlElement)doc.GetElementsByTagName("State")[0]).GetElementsByTagName("Item"))
                {
                    State.Children.Add(new CheckBox() { Content = state.InnerText, Margin = new Thickness(3, 5, 0, 0) });
                }
        }

        public Dictionary<String, String> GetSystemInformation()
        {
            Dictionary<String, String> information = new Dictionary<String, String>();
            this.Dispatcher.Invoke(()=> 
            {
                foreach (object o in SystemInformation.Children)
                {
                    if (((StackPanel)o).Children[1].GetType() == typeof(ComboBox))
                    {
                        if (((ComboBox)((StackPanel)o).Children[1]).Name == "WiFi_IC")
                        {
                            if (((ComboBox)((StackPanel)o).Children[1]).Name == "WiFi_IC")
                            {
                                try
                                {   //字典
                                    information.Add("WiFi_IC", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());           //WiFi_IC是key,例如选择3660，那么3660是value
                                }
                                catch
                                {
                                    //MessageBox.Show("WiFi_IC is null.Plesse reselect", "Hint");
                                    information.Add("WiFi_IC", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "TestMode")
                            {
                                try
                                {
                                    information.Add("TestMode", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestMode is null.Plesse reselect", "Hint");
                                    information.Add("TestMode", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "TestChain")
                            {
                                try
                                {
                                    information.Add("TestChain", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestChain is null.Plesse reselect", "Hint");
                                    information.Add("TestChain", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "IQxel")
                            {
                                try
                                {
                                    information.Add("IQxel", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestChain is null.Plesse reselect", "Hint");
                                    information.Add("IQxel", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "Instrument")
                            {
                                try
                                {
                                    information.Add("Instrument", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestChain is null.Plesse reselect", "Hint");
                                    information.Add("Instrument", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "IsAutoControl")
                            {
                                try
                                {
                                    information.Add("IsAutoControl", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    information.Add("IsAutoControl", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "Port")
                            {
                                try
                                {
                                    information.Add("Port", ((ComboBox)((StackPanel)o).Children[1]).Text.ToString().Replace("COM", ""));
                                }
                                catch
                                {
                                    //MessageBox.Show("PA and LNA is null.Plesse reselect", "Hint");
                                    information.Add("Port", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "WaveFile")
                            {
                                try
                                {
                                    information.Add("WaveFile", ((ComboBox)((StackPanel)o).Children[1]).Text.ToString());
                                }
                                catch
                                {
                                    information.Add("WaveFile", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "DPDMode")
                            {
                                try
                                {
                                    information.Add("DPDMode", ((ComboBox)((StackPanel)o).Children[1]).Text.ToString());
                                }
                                catch
                                {
                                    information.Add("DPDMode", "");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (((StackPanel)o).Children[1].GetType() == typeof(ComboBox))
                        {
                            if (((ComboBox)((StackPanel)o).Children[1]).Name == "WiFi_IC")
                            {
                                try
                                {   //字典
                                    information.Add("WiFi_IC", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());           //WiFi_IC是key,例如选择3660，那么3660是value
                                }
                                catch
                                {
                                    //MessageBox.Show("WiFi_IC is null.Plesse reselect", "Hint");
                                    information.Add("WiFi_IC", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "TestMode")
                            {
                                try
                                {
                                    information.Add("TestMode", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestMode is null.Plesse reselect", "Hint");
                                    information.Add("TestMode", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "TestChain")
                            {
                                try
                                {
                                    information.Add("TestChain", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestChain is null.Plesse reselect", "Hint");
                                    information.Add("TestChain", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "IQxel")
                            {
                                try
                                {
                                    information.Add("IQxel", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestChain is null.Plesse reselect", "Hint");
                                    information.Add("IQxel", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "Instrument")
                            {
                                try
                                {
                                    information.Add("Instrument", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    //MessageBox.Show("TestChain is null.Plesse reselect", "Hint");
                                    information.Add("Instrument", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "IsAutoControl")
                            {
                                try
                                {
                                    information.Add("IsAutoControl", ((ComboBox)((StackPanel)o).Children[1]).SelectedValue.ToString());
                                }
                                catch
                                {
                                    information.Add("IsAutoControl", "");
                                }
                            }

                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "Port")
                            {
                                try
                                {
                                    information.Add("Port", ((ComboBox)((StackPanel)o).Children[1]).Text.ToString().Replace("COM", ""));
                                }
                                catch
                                {
                                    //MessageBox.Show("PA and LNA is null.Plesse reselect", "Hint");
                                    information.Add("Port", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "WaveFile")
                            {
                                try
                                {
                                    information.Add("WaveFile", ((ComboBox)((StackPanel)o).Children[1]).Text.ToString());
                                }
                                catch
                                {
                                    information.Add("WaveFile", "");
                                }
                            }
                            else if (((ComboBox)((StackPanel)o).Children[1]).Name == "DPDMode")
                            {
                                try
                                {
                                    information.Add("DPDMode", ((ComboBox)((StackPanel)o).Children[1]).Text.ToString());
                                }
                                catch
                                {
                                    information.Add("DPDMode", "");
                                }
                            }
                        }
                        else if (((StackPanel)o).Children[1].GetType() == typeof(TextBox))
                        {
                            information.Add(((TextBox)((StackPanel)o).Children[1]).Name, ((TextBox)((StackPanel)o).Children[1]).Text);
                        }
                        else if (((StackPanel)o).Children[1].GetType() == typeof(Button))
                        {
                            information.Add(((Button)((StackPanel)o).Children[0]).Name, ((Button)((StackPanel)o).Children[0]).Content.ToString());
                            information.Add(((Button)((StackPanel)o).Children[1]).Name, ((Button)((StackPanel)o).Children[1]).Content.ToString());
                        }
                    }
                }
            });
            return information;
        }
        /// <summary>
        /// 具体测试项
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool?> GetTestItem()
        {
            Dictionary<string, bool?> TestItemDic = new Dictionary<string, bool?>();
            this.Dispatcher.Invoke(()=>
            {
                foreach (object o in ((StackPanel)((StackPanel)TestItemContain.Children[0]).Children[0]).Children)
                {
                    if (((CheckBox)o).Content.ToString() == "TxItems")
                    {
                        TestItemDic.Add("TX", ((CheckBox)o).IsChecked);
                    }
                    else if (((CheckBox)o).Content.ToString() == "Sensitivity")
                    {
                        TestItemDic.Add("SEN", ((CheckBox)o).IsChecked);
                    }
                    else if (((CheckBox)o).Content.ToString() == "MaxInputLevel")
                    {
                        TestItemDic.Add("MIL", ((CheckBox)o).IsChecked);
                    }
                    else if (((CheckBox)o).Content.ToString().Contains("RSSI"))
                    {
                        TestItemDic.Add("RSSI", ((CheckBox)o).IsChecked);
                    }
                    else if (((CheckBox)o).Content.ToString().Contains("WaterFall"))
                    {
                        TestItemDic.Add("WaterFall", ((CheckBox)o).IsChecked);
                    }
                }
            });
            return TestItemDic;
        }

        public Dictionary<string, string> GetLoss()
        {
            Dictionary<string, string> LossDic = new Dictionary<string, string>();
            this.Dispatcher.Invoke(()=>
            {
                foreach (object o in Loss.Children)
                {
                    foreach (object oc in ((StackPanel)o).Children)
                    {
                        if (oc.GetType() == typeof(TextBox))
                        {
                            LossDic.Add(((TextBox)oc).Name.Replace("_", ""), ((TextBox)oc).Text);
                        }
                    }
                }
            });
            return LossDic;
        }

        public Dictionary<string, string> GetArgumentChannel()
        {
            Dictionary<string, string> ChannelDic = new Dictionary<string, string>();
            this.Dispatcher.Invoke(()=>
            {
                foreach (object op in Channel.Children)
                {
                    if (op.GetType() == typeof(WrapPanel))
                    {
                        foreach (object o in ((WrapPanel)op).Children)
                        {
                            foreach (string name in GetRate())
                            {
                                if (ChannelDic.Keys.Contains(name.Split('_')[1] + "_" + name.Split('_')[2])) continue;
                                if (((StackPanel)o).Name == "_" + name.Split('_')[1] + "_" + name.Split('_')[2] + "_ChannelContain")
                                {
                                    ChannelDic.Add(name.Split('_')[1] + "_" + name.Split('_')[2],
                                        ((TextBox)((StackPanel)o).Children[1]).Text + ";" + ((TextBox)((StackPanel)o).Children[3]).Text);
                                }
                            }
                        }
                    }
                }
            });
            return ChannelDic;
        }

        public List<string> GetRate()
        {
            List<string> RateList = new List<string>();
            this.Dispatcher.Invoke(()=>
            {
                foreach (object c in BandChecked.Children)
                {
                    foreach (object o in ((StackPanel)c).Children)
                    {
                        foreach (object oc in ((StackPanel)o).Children)
                        {
                            if (oc.GetType() == typeof(WrapPanel))
                            {
                                if (oc.GetType() == typeof(WrapPanel))
                                {
                                    foreach (object occ in ((WrapPanel)oc).Children)
                                    {
                                        if (((CheckBox)occ).IsChecked == true)
                                        {
                                            RateList.Add(((CheckBox)occ).Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
            return RateList;
        }

        public Dictionary<string, string> GetRateAndENP()
        {
            Dictionary<string, string> RateAndEnpDic = new Dictionary<string, string>();
            this.Dispatcher.Invoke(()=>
            {
                foreach (object o in ENP.Children)
                {
                    foreach (object oc in ((StackPanel)o).Children)
                    {
                        if (((StackPanel)oc).Name != "")
                        {
                            if (((TextBox)((StackPanel)oc).Children[1]).Text != "" && ((TextBox)((StackPanel)oc).Children[1]).Text != null)
                            {
                                RateAndEnpDic.Add(((TextBox)((StackPanel)oc).Children[1]).Name, ((TextBox)((StackPanel)oc).Children[1]).Text);
                            }
                            else
                            {
                                RateAndEnpDic.Add(((TextBox)((StackPanel)oc).Children[1]).Name, "0");

                            }
                        }
                    }
                }
            });
            return RateAndEnpDic;
        }

        string openlocalFilePath = null;
        private void CHECKED(object sender, RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Content.ToString())
            {
                case "2G4":
                case "5G":
                    foreach (object o in ((StackPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        if (o.GetType() == typeof(StackPanel))
                        {
                            ((CheckBox)((StackPanel)o).Children[0]).IsChecked = true;
                        }
                    }
                    break;
                case "B":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
                case "G":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[3]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
                case "N20M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[5]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[3]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    break;
                case "N40M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[7]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[5]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    break;
                case "A":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
                case "AC20M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[9]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[7]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    break;
                case "AC40M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[9]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
                case "AC80M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[11]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
                case "AX20M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[11]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }

                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[13]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    break;
                case "AX40M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[13]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }

                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[15]).Children)
                        {
                            ((CheckBox)o).IsChecked = true;
                        }
                    }
                    break;
                case "AX80M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[17]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
                case "AX160M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[19]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
                case "WaterFall":
                    //Dictionary<string, bool?> itemdic = GetTestItem();
                    //if (itemdic["TX"] == true || itemdic["SEN"] == true)
                    //{
                    //    ((CheckBox)sender).IsChecked = false;
                    //}
                    break;
                default:
                    #region ENP输入框
                    string value = null;
                    XmlDocument doc = new XmlDocument();
                    if (openlocalFilePath != null)
                    {
                        doc.Load(openlocalFilePath);
                    }
                    else
                    {
                        doc.Load("Pages\\WiFi\\WiFiUIConfig.xml");
                    }

                    foreach (XmlElement TestArgument in doc.GetElementsByTagName("TestArgument"))
                    {
                        foreach (XmlElement TestBand in TestArgument.GetElementsByTagName("TestBAND"))
                        {
                            foreach (XmlElement argument in TestBand.GetElementsByTagName("Argument"))
                            {
                                foreach (XmlElement Rate in argument.GetElementsByTagName("Rate"))
                                {
                                    string key = ((CheckBox)sender).Name;
                                    if (key.Split('_')[1] == TestBand.GetAttribute("name") && key.Split('_')[2] == argument.GetAttribute("name")
                                   && (key.Split('_')[3] == Rate.GetAttribute("name").Replace("/s", "") || (key.Contains("5_5") && Rate.GetAttribute("name") == "5.5Mbit/s")))
                                    {
                                        value = Rate.GetAttribute("ENP");
                                    }
                                }
                            }
                        }
                    }

                    foreach (object o in ENP.Children)
                    {
                        if (((StackPanel)o).Name == "EnpContian_" + ((CheckBox)sender).Name.Split('_')[1] + "_" + ((CheckBox)sender).Name.Split('_')[2])
                        {
                            StackPanel sstackpanel = new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Name = ((CheckBox)sender).Name + "_ENP_Contian"
                            };
                            sstackpanel.Children.Add(new TextBlock()
                            {
                                Text = ((CheckBox)sender).Name.Split('_')[3] == "5" ? "5.5Mbit/s" : ((CheckBox)sender).Name.Split('_')[3],
                                Margin = new Thickness(0, 0, 2, 0),
                                Width = 37,
                                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                            });
                            sstackpanel.Children.Add(new TextBox()
                            {
                                Name = ((CheckBox)sender).Name + "_ENP",
                                Width = 70,
                                Text = value,
                                Style = Resources["TextBoxStyle1"] as Style
                            });
                            ((StackPanel)o).Children.Add(sstackpanel);
                            return;
                        }
                    }
                    StackPanel enpstackpanel = new StackPanel()
                    {
                        Name = "EnpContian_" + ((CheckBox)sender).Name.Split('_')[1] + "_" + ((CheckBox)sender).Name.Split('_')[2],
                        Orientation = Orientation.Vertical,
                        Width = 87,
                        Margin = new Thickness(5, 0, 5, 5)
                    };
                    #region //增加标签
                    StackPanel sssstackpanel = new StackPanel()
                    {
                        Background = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };
                    sssstackpanel.Children.Add(new TextBlock
                    {
                        Text = ((CheckBox)sender).Name.Split('_')[1] + " " + ((CheckBox)sender).Name.Split('_')[2],
                        Margin = new Thickness(10, 0, 0, 0),
                        Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        FontWeight = FontWeights.Bold
                    });
                    #endregion
                    StackPanel stackpanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Name = ((CheckBox)sender).Name + "_ENP_Contian"
                    };
                    stackpanel.Children.Add(new TextBlock()
                    {
                        Text = ((CheckBox)sender).Name.Split('_')[3] == "5" ? "5.5Mbit/s" : ((CheckBox)sender).Name.Split('_')[3],
                        Margin = new Thickness(0, 0, 2, 0),
                        Width = 37,
                        Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                    });
                    stackpanel.Children.Add(new TextBox()
                    {
                        Name = ((CheckBox)sender).Name + "_ENP",
                        Width = 70,
                        Text = value,
                        Style = Resources["TextBoxStyle1"] as Style
                    });
                    enpstackpanel.Children.Add(sssstackpanel);
                    enpstackpanel.Children.Add(stackpanel);
                    ENP.Children.Add(enpstackpanel);

                    #endregion
                    break;
            }
        }

        private void UNCHECKED(object sender, RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Content.ToString())
            {
                case "2G4":
                case "5G":
                    foreach (object o in ((StackPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        if (o.GetType() == typeof(StackPanel))
                        {
                            ((CheckBox)((StackPanel)o).Children[0]).IsChecked = false;
                        }
                    }
                    break;
                case "B":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
                case "G":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[3]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
                case "N20M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[5]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[3]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    break;
                case "N40M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[7]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[5]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    break;
                case "A":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
                case "AC20M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[9]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[7]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    break;
                case "AC40M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[9]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
                case "AC80M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[11]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
                case "AX20M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[11]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }

                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[13]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    break;
                case "AX40M":
                    if (((CheckBox)sender).Name.Contains("2G4"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[13]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }

                    }
                    else if (((CheckBox)sender).Name.Contains("5G"))
                    {
                        foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[15]).Children)
                        {
                            ((CheckBox)o).IsChecked = false;
                        }
                    }
                    break;
                case "AX80M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[17]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
                case "AX160M":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[19]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
                default:
                    #region//取消ENP输入框
                    foreach (object o in ENP.Children)
                    {
                        if (((StackPanel)o).Name == "EnpContian_" + ((CheckBox)sender).Name.Split('_')[1] + "_" + ((CheckBox)sender).Name.Split('_')[2])
                        {
                            foreach (object oc in ((StackPanel)o).Children)
                            {
                                if (((StackPanel)oc).Name == ((CheckBox)sender).Name + "_ENP_Contian")
                                {
                                    ((StackPanel)o).Children.Remove(((StackPanel)oc));
                                    if (((StackPanel)o).Children.Count < 2)
                                    {
                                        ENP.Children.Remove((StackPanel)o);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    #endregion
                    break;
            }
        }

        private void TEXTCHANGED(object sender, TextChangedEventArgs e )
        {
            if (((TextBox)sender).Name.Contains("2G4"))
            {
                try
                {
                    if (((TextBox)sender).Text != "")
                    {
                        string[] channelarray = ((TextBox)sender).Text.Split(',');
                        string freqstringTextBox = null;
                        int i = 0;
                        foreach (string str in channelarray)
                        {
                            if (str != "" && str != null)
                            {
                                string core = null;
                                if (i != 0)
                                {
                                    core = ",";
                                }
                                freqstringTextBox = freqstringTextBox + core + (2407 + Convert.ToInt16(str) * 5).ToString();
                            }
                            i++;
                        }
                        ((TextBox)((StackPanel)((TextBox)sender).Parent).Children[3]).Text = freqstringTextBox;

                    }
                    else
                    {
                        ((TextBox)((StackPanel)((TextBox)sender).Parent).Children[3]).Text = null;
                    }
                }
                catch
                {
                    MessageBox.Show("输入有误请重新输入\nps：信道间隔符号只能是英文逗号", "提示");
                    return;
                }
            }
            else
            {
                try
                {
                    if (((TextBox)sender).Text != "")
                    {
                        string[] channelarray = ((TextBox)sender).Text.Split(',');
                        string freqstringTextBox = null;
                        int i = 0;
                        foreach (string str in channelarray)
                        {
                            if (str != "")
                            {
                                string core = null;
                                if (i != 0)
                                {
                                    core = ",";
                                }
                                //if(((TextBox)sender).Name.Contains("5G1_N40M")|| ((TextBox)sender).Name.Contains("5G1_AC40M")||((TextBox)sender).Name.Contains("5G1_AX40M"))
                                //{
                                //    freqstringTextBox = freqstringTextBox + core +(5170 +10 +(Convert.ToInt16(str) - 34) * 5).ToString();
                                //}
                                //else
                                //{
                                freqstringTextBox = freqstringTextBox + core + (5170 + (Convert.ToInt16(str) - 34) * 5).ToString();//测试组反馈，5G40M频率不加10,20211123
                                                                                                                                   //}

                            }
                            i++;
                        }
                        ((TextBox)((StackPanel)((TextBox)sender).Parent).Children[3]).Text = freqstringTextBox;
                    }
                    else
                    {
                        ((TextBox)((StackPanel)((TextBox)sender).Parent).Children[3]).Text = null;
                    }
                }
                catch
                {
                    MessageBox.Show("输入有误请重新输入\nps：信道间隔符号只能是英文逗号", "提示");
                    return;
                }
            }
        }

        bool icSelect = false;
        private void SELECTIONCHANGED(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Name == "WiFi_IC")
            {
                if (((ComboBox)sender).SelectedItem.ToString().Contains("Signaling"))
                {
                    if (testype == "0")            //根据不同的xml配置文件加载不同的内容
                    {
                        ChildrenUIConfig("Pages\\WiFi\\WiFiSignalingUIConfig.xml");
                    }
                    else
                    {
                        ChildrenUIConfig("Pages\\WiFi\\WiFiUIConfig.xml");  //line 317
                    }
                }
                else
                {
                    ChildrenUIConfig("Pages\\WiFi\\WiFiUIConfig.xml");
                }
#pragma warning disable CS0252 // 可能非有意的引用比较；若要获取值比较，请将左边强制转换为类型“string”
#pragma warning disable CS0252 // 可能非有意的引用比较；若要获取值比较，请将左边强制转换为类型“string”
#pragma warning disable CS0252 // 可能非有意的引用比较；若要获取值比较，请将左边强制转换为类型“string”
                if (((ComboBox)sender).SelectedValue == "3990" || ((ComboBox)sender).SelectedValue == "3980" || ((ComboBox)sender).SelectedValue == "6174")
#pragma warning restore CS0252 // 可能非有意的引用比较；若要获取值比较，请将左边强制转换为类型“string”
#pragma warning restore CS0252 // 可能非有意的引用比较；若要获取值比较，请将左边强制转换为类型“string”
#pragma warning restore CS0252 // 可能非有意的引用比较；若要获取值比较，请将左边强制转换为类型“string”
                {
                    // ENP.Visibility = Visibility.Hidden;
                }
                else
                {
                    //ENP.Visibility = Visibility.Visible;
                }
                icSelect = true;
                string ic = ((ComboBox)sender).SelectedItem.ToString();
                try
                {
                    Pages.Wifi.WaveFromIC waveFromIC = Pages.Wifi.WiFiIQWaveForm.WaveFromICs.Where(wf => wf.ICtype.Equals(ic)).First();
                    ((ComboBox)((StackPanel)SystemInformation.Children[5]).Children[1]).SelectedIndex = waveFromIC.IsLDPC == false ? 0 : 1;
                    ((ComboBox)((StackPanel)SystemInformation.Children[6]).Children[1]).Text = "OFF";

                }
                catch { }

                icSelect = false;
            }
            if (((ComboBox)sender).Name == "Port")
            {
                if (((ComboBox)sender).SelectedItem == null || ((ComboBox)sender).SelectedItem.ToString() == "FindPort")
                {
                    ((ComboBox)sender).Items.Clear();

                    List<string> portnamelist = new List<string>();
                    portnamelist = System.IO.Ports.SerialPort.GetPortNames().ToList();
                    foreach (string com in portnamelist)
                    {
                        if (((ComboBox)sender).Items.Contains(com) == false)
                        {
                            ((ComboBox)sender).Items.Add(com);
                        }
                    }
                    if (((ComboBox)sender).Items.Contains("FindPort") == false)
                    {
                        ((ComboBox)sender).Items.Add("FindPort");
                    }
                    if (((ComboBox)sender).Items.Contains("Samsung_FindPort") == false)
                    {
                        ((ComboBox)sender).Items.Add("Samsung_FindPort");
                    }
                }
                else if (((ComboBox)sender).SelectedItem.ToString() == "Samsung_FindPort")
                {
                    ((ComboBox)sender).Items.Clear();

                    List<string> devices = new List<string>(AdbSetMobelONOFF("devices").Replace("List of devices attached", "").Replace("device", "").Replace(" ", "").Replace("\t", "").Split('\n'));//读取手机adb设备编号


                    //List<string> portnamelist = new List<string>();
                    //portnamelist = System.IO.Ports.SerialPort.GetPortNames().ToList();
                    foreach (string device in devices)
                    {
                        if (((ComboBox)sender).Items.Contains(device) == false)
                        {
                            ((ComboBox)sender).Items.Add(device);
                        }
                    }
                    if (((ComboBox)sender).Items.Contains("FindPort") == false)
                    {
                        ((ComboBox)sender).Items.Add("FindPort");
                    }
                    if (((ComboBox)sender).Items.Contains("Samsung_FindPort") == false)
                    {
                        ((ComboBox)sender).Items.Add("Samsung_FindPort");
                    }
                }
            }
            if (((ComboBox)sender).Name == "TestMode")
            {
                if (((ComboBox)sender).SelectedItem.ToString() == "对位")
                {
                    if (SystemInformation.Children[SystemInformation.Children.Count - 1].GetType() != typeof(CheckBox) ||
                        (SystemInformation.Children[SystemInformation.Children.Count - 1].GetType() == typeof(CheckBox) &&
                        ((CheckBox)SystemInformation.Children[SystemInformation.Children.Count - 1]).Content.ToString() != "金机校位"))
                    {
                        SystemInformation.Children.Add(new CheckBox() { Content = "金机校位", Margin = new Thickness(5, 0, 0, 0), Visibility = Visibility.Hidden });
                    }
                }
            }
            if (((ComboBox)sender).Name == "WaveFile" && !icSelect)
            {
                string ic = ((ComboBox)((StackPanel)SystemInformation.Children[0]).Children[1]).Text;
                for (int i = 0; i < Pages.Wifi.WiFiIQWaveForm.WaveFromICs.Count(); i++)
                {
                    if (Pages.Wifi.WiFiIQWaveForm.WaveFromICs[i].ICtype.Equals(ic))
                    {
                        Pages.Wifi.WiFiIQWaveForm.WaveFromICs[i].IsLDPC = ((ComboBox)sender).SelectedItem.ToString().Equals("LDPC") ? true : false;
                        Pages.Wifi.WiFiIQWaveForm.SaveWaveFromInfo();
                        break;
                    }
                }
            }

            if (((ComboBox)sender).Name == "DPDmode")
            {

                if (((ComboBox)sender).SelectedItem.ToString() == "ON")
                {
                    ((ComboBox)sender).Items.Add("ON");

                }
                if (((ComboBox)sender).SelectedItem.ToString() == "OFF")
                {
                    ((ComboBox)sender).Items.Add("OFF");
                }
            }
        }
        private string AdbSetMobelONOFF(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                //return "";
            }
            StringBuilder sb = new StringBuilder();
            System.Diagnostics.Process p = null;
            try
            {
                p = new System.Diagnostics.Process();

                // 设置属性  
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = Directory.GetCurrentDirectory() + @"\lib\PhoneControls\adb.exe";
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.StartInfo.Arguments = str;
                p.Start();

                StreamReader readerout = p.StandardOutput;
                string line = string.Empty;
                while (!readerout.EndOfStream)
                {
                    line = readerout.ReadLine();
                    //Console.WriteLine(line);  
                    //将得到的结果写入到excle中  
                    sb.Append(line + "\n");
                }
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常:" + ex.ToString());
            }
            if (p != null)
            {
                p.Close();
            }
            return sb.ToString();
        }

        private void Button_Click(object sender , RoutedEventArgs e)
        {
            String path = null;
            switch (((Button)sender).Name)
            {
                case "Limit_2G4":
                    path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Pages\Wifi\Wi-Fi传导标准文件\2G4";
                    OpenFileDialog openfiledialog = new OpenFileDialog();
                    openfiledialog.Reset();
                    openfiledialog.InitialDirectory = path;
                    openfiledialog.FileName = "User.xml"; // Default file name
                    openfiledialog.DefaultExt = ".xml"; // Default file extension
                    openfiledialog.Filter = "Text documents (.xml)|*.xml"; // Filter files by extension
                    openfiledialog.RestoreDirectory = false;
                    openfiledialog.Title = "请选择线损文件";

                    bool? result = openfiledialog.ShowDialog();
                    if (result == true)
                    {
                        ((Button)sender).Content = openfiledialog.FileName.ToString();
                    }
                    break;
                case "Limit_5G":
                    path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"MainTestQueue\wiFi\Wi-Fi传导标准文件\5G";//设置默认路径
                    OpenFileDialog openfiledialog_5G = new OpenFileDialog();
                    openfiledialog_5G.Reset();
                    openfiledialog_5G.InitialDirectory = path;
                    openfiledialog_5G.FileName = "User.xml"; // Default file name
                    openfiledialog_5G.DefaultExt = ".xml"; // Default file extension
                    openfiledialog_5G.Filter = "Text documents (.xml)|*.xml"; // Filter files by extension
                    openfiledialog_5G.RestoreDirectory = true;

                    bool? result_5G = openfiledialog_5G.ShowDialog();
                    if (result_5G == true)
                    {
                        ((Button)sender).Content = openfiledialog_5G.FileName.ToString();
                    }
                    break;
            }
        }

        private void MOUSEENTER(object sender, MouseEventArgs e)
        {
            if (sender.GetType() == typeof(Button))
            {
                if (((Button)sender).Name == "Limit_2G4" || ((Button)sender).Name == "Limit_5G")
                {
                    PopupText.Text = ((Button)sender).Content.ToString();
                    popup.PlacementTarget = ((Button)sender);
                    popup.IsOpen = true;
                }
                else if (((Button)sender).Content.ToString() == "ENP Power")
                {
                    PopupText.Text = "如果测试信道中存在多个频段范围如 5.1G,5.4G,5.8G；请依次输入对应频段的期望功率，用英文逗号分开，如果与前一个频段的期望功率相同允许只输入一个数值";
                    popup.PlacementTarget = ((Button)sender);
                    popup.IsOpen = true;
                }
                else if (((Button)sender).Content.ToString() == "Channel")
                {
                    PopupText.Text = "请依次输入信道，用英文逗号分开";
                    popup.PlacementTarget = ((Button)sender);
                    popup.IsOpen = true;
                }
                else if (((Button)sender).Content.ToString() == "State")
                {
                    PopupText.Text = "仅供测试WiFi干扰灵敏度";
                    popup.PlacementTarget = ((Button)sender);
                    popup.IsOpen = true;
                }
            }
        }

        private void MOUSELEAVE(object sender , MouseEventArgs e)
        {
            do
            {
                popup.IsOpen = false;
            } while (popup.IsOpen == true);
        }
    }
}
