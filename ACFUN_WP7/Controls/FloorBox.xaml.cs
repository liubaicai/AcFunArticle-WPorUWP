using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ACFUN.Controls
{
    public partial class FloorBox : UserControl
    {
        private PhoneApplicationPage _BasePage;

        public FloorBox()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
        }

        public void Reset(commentdetail detail)
        {
            try
            {
                floorlistbox.ScrollIntoView(detail);
            }
            catch { }
        }

        public void Open(PhoneApplicationPage basePage)
        {
            _BasePage = basePage;
            _BasePage.BackKeyPress += BasePage_BackKeyPress;
            OpenStory.Begin();
        }

        private void Close()
        {
            if (_BasePage != null)
            {
                _BasePage.BackKeyPress -= BasePage_BackKeyPress;
                OpenStory.Stop();
                CloseStory.Begin();
            }
            _BasePage = null;
        }

        private void BasePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Close();
        }

        private void floorlistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            floorlistbox.SelectedIndex = -1;
        }
    }
}
