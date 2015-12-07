using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using ACFUN.View;

namespace ACFUN.ViewModel
{
    internal interface IViewModel
    {
        /// <summary>
        /// 初始化时调用
        /// </summary>
        void Init();

        /// <summary>
        /// 页面加载完毕调用,应将大部分逻辑放于此处
        /// </summary>
        void Load();

        void NaviTo(NavigationEventArgs e);
        void NaviFrom(NavigationEventArgs e);
    }

    /// <summary>
    /// 继承INotifyPropertyChanged实现绑定通知更新，实现IViewModel的方法
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IViewModel
    {
        public ViewBase BaseView { get; set; }

        public virtual void Init()
        {

        }

        public virtual void Load()
        {

        }

        public virtual void NaviTo(NavigationEventArgs e)
        {

        }

        public virtual void NaviFrom(NavigationEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}