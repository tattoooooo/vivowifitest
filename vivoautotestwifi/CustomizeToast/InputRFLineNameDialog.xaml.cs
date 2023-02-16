using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace vivo_Auto_Test.CustomizeToast
{
    /// <summary>
    /// Interaction logic for InputRFLineNameDialog.xaml
    /// </summary>
    public partial class InputRFLineNameDialog : ModernDialog,INotifyPropertyChanged
    {

        public string rfline { get; set; }

        public string RFLine {
            set {
                this.rfline = value;
                NotifyChange("RFLine");
            }
            get {
                return this.rfline;
            }
        }

        public InputRFLineNameDialog()
        {
            InitializeComponent();
            this._name.DataContext = this;
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChange(string name) {
            if (PropertyChanged != null) {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
