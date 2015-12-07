using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace ACFUN.Controls
{
    public partial class SortBox : UserControl
    {
        public event EventHandler<SortSelectEventArgs> SortSelectCompleted;

        private PhoneApplicationFrame frame;

        public SortBox()
        {
            InitializeComponent();
            this.Loaded += SortBox_Loaded;
            frame = Application.Current.RootVisual as PhoneApplicationFrame;
        }

        void SortBox_Loaded(object sender, RoutedEventArgs e)
        {
            var sort = Settings.GetValueOrDefault<int>("Sort", 1);
            switch (sort)
            {
                case 1:
                    rb1.IsChecked = true;
                    rb2.IsChecked = false;
                    rb3.IsChecked = false;
                    rb4.IsChecked = false;
                    break;
                case 2:
                    rb1.IsChecked = false;
                    rb2.IsChecked = true;
                    rb3.IsChecked = false;
                    rb4.IsChecked = false;
                    break;
                case 3:
                    rb1.IsChecked = false;
                    rb2.IsChecked = false;
                    rb3.IsChecked = true;
                    rb4.IsChecked = false;
                    break;
                case 4:
                    rb1.IsChecked = false;
                    rb2.IsChecked = false;
                    rb3.IsChecked = false;
                    rb4.IsChecked = true;
                    break;
            }

            frame.BackKeyPress += frame_BackKeyPress;
        }

        void frame_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SortSelectCompleted != null)
            {
                SortSelectCompleted.Invoke(this, new SortSelectEventArgs(false));
            }
        }

        private void rb1_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddOrUpdateValue("Sort", 1);
            rb1.IsChecked = true;
            rb2.IsChecked = false;
            rb3.IsChecked = false;
            rb4.IsChecked = false;

            if (SortSelectCompleted != null)
            {
                SortSelectCompleted.Invoke(this, new SortSelectEventArgs(true));
            }
        }

        private void rb2_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddOrUpdateValue("Sort", 2);
            rb1.IsChecked = false;
            rb2.IsChecked = true;
            rb3.IsChecked = false;
            rb4.IsChecked = false;

            if (SortSelectCompleted != null)
            {
                SortSelectCompleted.Invoke(this, new SortSelectEventArgs(true));
            }
        }

        private void rb3_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddOrUpdateValue("Sort", 3);
            rb1.IsChecked = false;
            rb2.IsChecked = false;
            rb3.IsChecked = true;
            rb4.IsChecked = false;

            if (SortSelectCompleted != null)
            {
                SortSelectCompleted.Invoke(this, new SortSelectEventArgs(true));
            }
        }

        private void rb4_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddOrUpdateValue("Sort", 4);
            rb1.IsChecked = false;
            rb2.IsChecked = false;
            rb3.IsChecked = false;
            rb4.IsChecked = true;

            if (SortSelectCompleted != null)
            {
                SortSelectCompleted.Invoke(this, new SortSelectEventArgs(true));
            }
        }
    }

    public class SortSelectEventArgs : EventArgs
    {
        public bool IsChanged { get; set; }

        public SortSelectEventArgs(bool IsChanged)
        {
            this.IsChanged = IsChanged;
        }
    }
}
