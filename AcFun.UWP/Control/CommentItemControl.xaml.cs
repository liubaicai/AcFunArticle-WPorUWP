using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AcFun.UWP.Helper;
using AcFun.UWP.Model;
using AcFun.UWP.Module;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AcFun.UWP.Control
{
    public sealed partial class CommentItemControl : UserControl
    {
        private CommentBindingItemModel Model { get; set; }

        public CommentItemControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += CommentItemControl_DataContextChanged;
        }

        private void CommentItemControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue!=null)
            {
                var data = args.NewValue as CommentBindingModel;
                Model = data.CommentContentList[data.CommentId];
                UserLogo.Source = string.IsNullOrEmpty(Model.UserImg)
                    ? new BitmapImage(new Uri("ms-appx:///Assets/avatar.jpg", UriKind.RelativeOrAbsolute))
                    : new BitmapImage(new Uri(Model.UserImg));
                CommentCount.Text = Model.Count + "";
                if (Model.NameRed - 1 >= 0)
                {
                    CommentName.Foreground = new SolidColorBrush(Color.FromArgb(255, 238, 0, 62));
                }
                CommentName.Text = Model.UserName;
                CommentTime.Text = Model.PostDate;
                CommentContent.Text = Model.ContentShow;
                UserLogoFrame.Opacity = Model.AvatarFrame;
                if (Model.ParentId > 0 && data.CommentContentList.ContainsKey(Model.ParentId))
                {
                    pContent.Visibility = Visibility.Visible;
                    var pmodel = data.CommentContentList[Model.ParentId];
                    pCommentName.Text = pmodel.UserName;
                    pCommentCount.Text = ((int)pmodel.Count).ToString();
                    pCommentContent.Text = pmodel.ContentShow;
                    if (pmodel.ParentId > 0&& data.CommentContentList.ContainsKey(pmodel.ParentId))
                    {
                        ppContent.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ppContent.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    pContent.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void QuoteLink_OnClick(object sender, RoutedEventArgs e)
        {
            if (Model != null)
            {
                Comment.Instance.SetQuote(Model.CommentId, (int)Model.Count);
            }
        }
    }
}
