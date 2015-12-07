using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace AcFunBlue.Common
{
    public class ListViewHelper
    {
        public static ScrollBar GetChildScrollBar(ListView view)
        {
            try
            {
                ScrollViewer scrollviewer = FindVisualChildByName<ScrollViewer>(view, "ScrollViewer");
                ScrollBar verticalScrollBar = FindVisualChildByName<ScrollBar>(scrollviewer, "VerticalScrollBar");
                return verticalScrollBar;
            }
            catch { return null; }
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if (controlName == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
                return null;
            }
            catch { return null; }
        }
    }
}
