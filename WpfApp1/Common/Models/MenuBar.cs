using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common.Models
{
    /// <summary>
    /// 系统导航菜单实体类
    /// </summary>
   public class MenuBar :BindableBase
    {
        private String icon;
        public String Icon
        {
            get { return icon; }
            set { icon = value; }
        }
        private String title;
        public String Title
        {
            get { return title; }
            set { title = value; }
        }
        private String nameSpace;
        public String NameSpace
        {
            get { return nameSpace; }
            set { nameSpace = value; }
        }

        private System.Collections.IEnumerable menuBars;

        public System.Collections.IEnumerable MenuBars { get => menuBars; set => SetProperty(ref menuBars, value); }
    }
}
