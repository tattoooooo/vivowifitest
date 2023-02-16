using FirstFloor.ModernUI.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using vivoautotestwifi.Control;

namespace vivoautotestwifi.CustomizeToast
{
    /// <summary>
    /// Interaction logic for ChoseDevice.xaml
    /// </summary>
    public partial class ChoseDevice : ModernDialog, INotifyPropertyChanged
    {

        List<CommunicateUtils.DeviceInfo> device = null;

        List<CommunicateUtils.DeviceInfo> Devices
        {
            set
            {
                this.device = value;
                NotifityProperty("Devices");
            }
            get
            {
                return this.device;
            }
        }

        public string id = null;

        public ChoseDevice(List<CommunicateUtils.DeviceInfo> device)
        {
            InitializeComponent();
            foreach (CommunicateUtils.DeviceInfo deviceInfo in device)
            {
                if (deviceInfo != null)
                {
                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = deviceInfo.DeviceType.Trim();
                    this.choose_list.Items.Add(new ComboBoxItem()
                    {
                        Content = deviceInfo.ID,
                        ToolTip = toolTip,
                    });
                }
            }
            this.Title = Properties.Resources.software_name;
            if (this.choose_list.Items.Count > 0)
            {
                this.choose_list.SelectedIndex = 0;
            }
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public void NotifityProperty(string name) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

       
        private void choose_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem comboBoxItem = (ComboBoxItem)comboBox.SelectedValue;
            if (comboBoxItem != null)
            {
                id = (string)comboBoxItem.Content;
            }

        }
    }
}
