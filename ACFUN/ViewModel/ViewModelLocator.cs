using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACFUN.ViewModel
{
    public class ViewModelLocator
    {
        /// <summary>
        /// 每个MVVM的共用容器
        /// </summary>
        public Dictionary<string, ViewModelBase> DicVM = new Dictionary<string, ViewModelBase>();

        private static ViewModelLocator _instance;
        public static ViewModelLocator Instance
        {
            get { return _instance ?? (_instance = Application.Current.Resources["Locator"] as ViewModelLocator); }
        }
    }
}
