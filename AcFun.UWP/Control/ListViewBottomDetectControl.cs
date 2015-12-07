using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using AcFun.UWP.Control.LinqToTree;

namespace AcFun.UWP.Control
{
    public class ListViewBottomDetectControl : ListView
    {
        public event EventHandler BottomArrived;
        public event EventHandler OnTopReached;

        private string ItemStyle =
                @"
                          <Style x:Key='ListViewItemStyle1' TargetType='ListViewItem' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <Setter Property='HorizontalContentAlignment' Value='Stretch'/>
                                <Setter Property='Margin' Value='0'/>
                                <Setter Property='Padding' Value='0'/>
                           </Style>
                        ";

        //private string ItemsPanelStyle =
        //    @"
        //        <ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
        //            <VirtualizingStackPanel/>
        //        </ItemsPanelTemplate>
        //    ";

        public ScrollBar VerticalScrollBar { get; set; }

        public ScrollViewer ScrollViewer { get; set; }

        public ListViewBottomDetectControl()
        {
            this.SelectionMode = ListViewSelectionMode.None;
            this.Loaded += ListViewBottomDetectControl_Loaded;
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.ItemContainerStyle = XamlReader.Load(ItemStyle) as Style;
            this.ShowsScrollingPlaceholders = false;
            // this.ItemsPanel = XamlReader.Load(ItemsPanelStyle) as ItemsPanelTemplate;

            this.VerticalAlignment = VerticalAlignment.Top;

            AddItemContainerTransition();
        }

        private void AddItemContainerTransition()
        {
            TransitionCollection collection = new TransitionCollection();
            EntranceThemeTransition transition = new EntranceThemeTransition();
            collection.Add(transition);
            this.ItemContainerTransitions = collection;
        }

        void ListViewBottomDetectControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Loaded -= ListViewBottomDetectControl_Loaded;

            if (OnTopReached != null || BottomArrived != null)
            {
                FindScrollViewControlAndRegisterEvents();
            }
        }

        #region ScrollViewer 实现的底部检测

        private void FindScrollViewControlAndRegisterEvents()
        {
            var svs = this.Descendants<ScrollViewer>().ToList();

            ScrollViewer = svs.FirstOrDefault() as ScrollViewer;
            if (ScrollViewer != null)
            {
                ScrollViewer.ViewChanged += ScrollViewerChanged;
            }
        }

        private void ScrollViewerChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ScrollViewer.VerticalOffset == 0)
            {
                OnTopReached?.Invoke(this, new EventArgs());
            }
            else if (ScrollViewer.VerticalOffset == ScrollViewer.ScrollableHeight)
            {
                BottomArrived?.Invoke(this, new EventArgs());
            }
        }

        #endregion
    }
}
