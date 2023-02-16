using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Common.Models;
using WpfApp1.Extensions;

namespace WpfApp1.ViewModels
{
   public class MainViewModels :BindableBase
   {
        public MainViewModels(IRegionManager regioinManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            CreateMenuBar();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);

            GoBackCommand = new DelegateCommand(() =>
            {
                if (regionNavigationJournal != null && regionNavigationJournal.CanGoBack)
                    regionNavigationJournal.GoBack();
            });
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (regionNavigationJournal != null && regionNavigationJournal.CanGoForward)
                    regionNavigationJournal.GoForward();
            });

            this.regioinManager = regioinManager;
        }
        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }

        private readonly IRegionManager regioinManager;
        private IRegionNavigationJournal regionNavigationJournal;
        private ObservableCollection<MenuBar> menuBars;
        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }
        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Home",Title = "首页",NameSpace = "IndexView"});
            MenuBars.Add(new MenuBar() { Icon = "NotebookOutline",Title = "待办事项",NameSpace = "ToDoView"});
            MenuBars.Add(new MenuBar() { Icon = "NotebookPlus",Title = "备忘录",NameSpace = "MemoView"});
            MenuBars.Add(new MenuBar() { Icon = "Cog",Title = "设置",NameSpace = "SettingsView"});
        }
        private void Navigate(MenuBar obj)
        {
            if (obj == null || String.IsNullOrWhiteSpace(obj.NameSpace))
                return;
            regioinManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace,back=>
            {
                regionNavigationJournal = back.Context.NavigationService.Journal;
            });
        }
   }
}
