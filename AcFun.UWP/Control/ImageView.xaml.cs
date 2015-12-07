using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AcFun.UWP.Control
{
    public sealed partial class ImageView : UserControl
    {
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageView), new PropertyMetadata("", OnBlockTextChanged));
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        static void OnBlockTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var view = obj as ImageView;
            if (view != null)
            {
                view.Image.Source = (ImageSource) e.NewValue;
            }
        }

        public ImageView()
        {
            this.InitializeComponent();
        }
    }
}
