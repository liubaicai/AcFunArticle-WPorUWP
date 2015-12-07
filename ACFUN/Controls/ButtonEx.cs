using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ACFUN.Controls
{
    public class ButtonEx : Button
    {
        public ButtonEx()
        {
            this.SetValue(ButtonEx.StyleProperty, Application.Current.Resources["buttonExStyle"]);
        }

        #region Fields

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground",
            typeof(Brush),
            typeof(ButtonEx),
            new PropertyMetadata(new SolidColorBrush(Colors.White), null));

        public static readonly DependencyProperty PressedForegroundProperty =
            DependencyProperty.Register("PressedForeground",
            typeof(Brush),
            typeof(ButtonEx),
            new PropertyMetadata(new SolidColorBrush(Colors.Black), null));

        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.Register("PressedBorderBrush",
            typeof(Brush),
            typeof(ButtonEx),
            new PropertyMetadata(new SolidColorBrush(Colors.Black), null));

        public static readonly DependencyProperty InvisibleMarginProperty =
            DependencyProperty.Register("InvisibleMargin",
            typeof(Thickness),
            typeof(ButtonEx),
            new PropertyMetadata(new Thickness(12), null));

        #endregion

        #region Properties

        public Brush PressedBackground
        {
            set { SetValue(PressedBackgroundProperty, value); }
            get { return (Brush)GetValue(PressedBackgroundProperty); }
        }

        public Brush PressedForeground
        {
            set { SetValue(PressedForegroundProperty, value); }
            get { return (Brush)GetValue(PressedForegroundProperty); }
        }

        public Brush PressedBorderBrush
        {
            set { SetValue(PressedBorderBrushProperty, value); }
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
        }

        public Thickness InvisibleMargin
        {
            set { SetValue(InvisibleMarginProperty, value); }
            get { return (Thickness)GetValue(InvisibleMarginProperty); }
        }
        #endregion
    }
}
