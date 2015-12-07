using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using ACFUN.ViewModel;
using Microsoft.Phone.Controls;

namespace ACFUN.View
{
    public class ViewBase : PhoneApplicationPage
    {
        protected string GuidKey;

        /// <summary>
        /// 便于访问的一个VM的引用
        /// </summary>
        protected ViewModelBase ViewModel
        {
            get { return ViewModelLocator.Instance.DicVM[GuidKey]; }
        }

        /// <summary>
        /// vm的初始化，置入容器中，执行vm和m的绑定
        /// </summary>
        protected ViewBase()
        {
            GuidKey = Guid.NewGuid().ToString("N");
            var name = GetType().FullName.Replace(".View.", ".ViewModel.") + "Model";
            var o = Activator.CreateInstance(Type.GetType(name)) as ViewModelBase;

            o.BaseView = this;

            ViewModelLocator.Instance.DicVM.Add(GuidKey, o);
            DataContext = ViewModelLocator.Instance.DicVM[GuidKey];
            ViewModel.Init();
            Loaded += (s, e) => ViewModel.Load();
        }

        /// <summary>
        /// 隐藏显示大法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Content.Visibility = System.Windows.Visibility.Visible;
            ViewModel.NaviTo(e);
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 隐藏显示大法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Content.Visibility = System.Windows.Visibility.Collapsed;
            ViewModel.NaviFrom(e);
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 将vm的绑定移除
        /// </summary>
        ~ViewBase()
        {
            if (GuidKey != null && ViewModelLocator.Instance.DicVM.ContainsKey(GuidKey))
            {
                ViewModelLocator.Instance.DicVM.Remove(GuidKey);
            }
        }
    }
}
