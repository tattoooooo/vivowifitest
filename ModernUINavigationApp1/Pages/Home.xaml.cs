using System;
using System.Collections.Generic;
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

namespace ModernUINavigationApp1.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        String path = "";
        public Home()
        {
            InitializeComponent();
            UIConfig("Pages\\WiFiUIConfig.xml");
        }
        public void UIConfig(String path)
        {
            XmlDocument docs = new XmlDocument();
            docs.Load(path);
            foreach (XmlElement systeminformation in docs.GetElementsByTagName("SystemInformation"))
            {
                foreach (XmlElement items in systeminformation.GetElementsByTagName("WiFi_IC"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal};
                    stackpanel.Children.Add(new TextBlock() { Text = items.GetAttribute("name")});
                    ComboBox combobox = new ComboBox()
                    {
                        Items = { "3660", "3680", "3990", "6174", "MTK", "MTK New", "MT6635X", "6391", "6750" },
                        Background = new SolidColorBrush(Color.FromRgb(255,255,255))
                    };
                    combobox.SelectionChanged += SELECTIONCHANGED;
                    stackpanel.Children.Add(combobox);
                    SystemInformation.Children.Add(stackpanel);
                }
            }
        }

        private void SELECTIONCHANGED(object sender,SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Name == "WiFi_IC")
            {
               
            }
            else
            {
                ChildrenUIConfig("Pages\\WiFiUIConfig.xml");
            }
        }
        public void ChildrenUIConfig(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            foreach (XmlElement TestArgument in doc.GetElementsByTagName("TestArgument"))
            {
                foreach (XmlElement TestBand in TestArgument.GetElementsByTagName("TestBAND"))
                {
                    StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(31, 0, 0, 3) };
                    foreach (XmlElement Argument in TestBand.GetElementsByTagName("Argument"))
                    {
                        StackPanel ArgumentStackPanel = new StackPanel() { };
                        CheckBox argumentcheckbox = new CheckBox() { Content=Argument.GetAttribute("name"), Name = "_" + TestBand.GetAttribute("name") + "_" + Argument.GetAttribute("name"), Margin = new Thickness(3, 0, 0, 3), Style = Resources["ItemCheckStyle"] as Style };
                        argumentcheckbox.Checked += CHECKED;
                        argumentcheckbox.Unchecked += UNCHECKED;
                        ArgumentStackPanel.Children.Add(argumentcheckbox);
                        stackpanel.Children.Add(ArgumentStackPanel);
                        WrapPanel wrappanel = new WrapPanel() { Margin = new Thickness(31, 0, 0, 3) };
                        foreach (XmlElement rate in Argument.GetElementsByTagName("Rate"))
                        {
                            CheckBox checkBox = new CheckBox()
                            {
                                Content = rate.GetAttribute("name"),
                                Style = Resources["ItemCheckStyle"] as Style
                            };
                            checkBox.Checked += CHECKED;
                            checkBox.Unchecked += UNCHECKED;
                            wrappanel.Children.Add(checkBox);
                        }
                        stackpanel.Children.Add(wrappanel);
                    }
                    StackPanel ParentStackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                    StackPanel TestBandStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    CheckBox bandcheckedbox = new CheckBox() { Content = TestBand.GetAttribute("name").Replace("1", ""), Margin = new Thickness(3, 0, 0, 3), Style = Resources["ItemCheckStyle"] as Style };
                    bandcheckedbox.Checked += CHECKED;
                    bandcheckedbox.Unchecked += UNCHECKED;
                    TestBandStackPanel.Children.Add(bandcheckedbox);
                    ParentStackPanel.Children.Add(TestBandStackPanel);
                    ParentStackPanel.Children.Add(stackpanel);
                    TestInformation.Children.Add(ParentStackPanel);
                }
            }
        }
        private void CHECKED(object sender,RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Content.ToString())
            {
                case "2G4":
                case "5G":
                    foreach (object o in ((StackPanel)(((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1])).Children) //最外层(StackPanel) .Children就是ArgumentStackPanel跟Wrappanel
                    {
                        switch (o.GetType().ToString())
                        {
                            case "StackPanel":
                                ((CheckBox)((StackPanel)o).Children[0]).IsChecked = true;
                                break;
                            case "WrapPanel":

                                break;
                        } 
                    }
                    break;
                case "B":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        ((CheckBox)o).IsChecked = true;
                    }
                    break;
            }
        }
        private void UNCHECKED(object sender, RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Content.ToString())
            {
                case "B":
                    foreach (object o in ((WrapPanel)((StackPanel)((StackPanel)((CheckBox)sender).Parent).Parent).Children[1]).Children)
                    {
                        ((CheckBox)o).IsChecked = false;
                    }
                    break;
            }
        }
    }
}
