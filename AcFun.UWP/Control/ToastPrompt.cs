using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace AcFun.UWP.Control
{
    public class ToastPrompt : Windows.UI.Xaml.Controls.Control
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(string), typeof(ToastPrompt), new PropertyMetadata(default(string)));
        
        public ToastPrompt()
        {
            DefaultStyleKey = typeof(ToastPrompt);
        }

        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateManager.GoToState(this, "Show", true);
        }

        public static async void Show(string content, Action tapAction = null, int showtime = 2000)
        {
            try
            {
                Debug.WriteLine("ToastPrompt:" + content);
                if (content.Contains("主题参数出错"))
                {
                    return;
                }

                var toast = new ToastPrompt
                {
                    Content = content,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };

                if (tapAction != null)
                {
                    toast.Tapped += (s, e) => { tapAction(); };
                }

                var popUp = new Popup();
                //popUp.HorizontalOffset = Window.Current.Bounds.Width - 300;


                var grid = new Grid() { Width = 300 };
                grid.Children.Add(toast);
                toast.Margin = new Thickness(0, 30, 0, 0);
                popUp.Child = grid;

                //显示动画
                var cts = new CompositeTransform { TranslateX = -300 };
                grid.RenderTransform = cts;
                var sbShow = new Storyboard();
                var daShow = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(500),
                    To = 0,
                    EasingFunction = new ElasticEase
                    {
                        EasingMode = EasingMode.EaseOut,
                        Oscillations = 1,
                        Springiness = 10
                    }
                };
                Storyboard.SetTarget(daShow, cts);
                Storyboard.SetTargetProperty(daShow, "TranslateX");
                sbShow.Children.Add(daShow);
                popUp.IsOpen = true;
                sbShow.Begin();

                await Task.Delay(showtime);

                //隐藏动画
                var sbHide = new Storyboard();
                var daHide = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(200),
                    To = -300
                };
                Storyboard.SetTarget(daHide, cts);
                Storyboard.SetTargetProperty(daHide, "TranslateX");
                sbHide.Children.Add(daHide);
                sbHide.Begin();

                await Task.Delay(300);

                //清理
                grid.Children.Clear();
                popUp.Child = null;
                grid = null;
                toast = null;
                popUp.IsOpen = false;
                popUp = null;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
    }
}
