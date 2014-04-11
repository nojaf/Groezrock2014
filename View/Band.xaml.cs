using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Groezrock2014.ViewModel;

namespace Groezrock2014.View
{
    public partial class Band : PhoneApplicationPage
    {
        private BandViewModel _vm;

        public Band()
        {
            InitializeComponent();
            _vm = this.DataContext as BandViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string bandName = NavigationContext.QueryString["bandName"];
            _vm.SetCurrentBand(bandName);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _vm.ResetBand();
            base.OnNavigatedFrom(e);
        }
    }
}