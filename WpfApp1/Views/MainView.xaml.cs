﻿using Prism.Regions;
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
using System.Windows.Shapes;
using WpfApp1.Extensions;

namespace WpfApp1.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IRegionManager regionManager)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.MainViewModels(regionManager);
            menuBar.SelectionChanged += (s, e) => {
                drawerHost.IsLeftDrawerOpen = false;
            };
        }
    }
}
