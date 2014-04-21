using GalaSoft.MvvmLight;
using Groezrock2014.Messages;
using Groezrock2014.Models;
using Groezrock2014.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.ViewModel
{
    public class InfoViewModel:ViewModelBase
    {
        private IGroezrockService _groezrockService;
        private INavigationService _navigationService;

        /// <summary>
        /// The <see cref="Info" /> property's name.
        /// </summary>
        public const string InfoPropertyName = "Info";

        private Info _info = null;

        /// <summary>
        /// Sets and gets the Info property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Info Info
        {
            get
            {
                return _info;
            }

            set
            {
                if (_info == value)
                {
                    return;
                }

                RaisePropertyChanging(InfoPropertyName);
                _info = value;
                RaisePropertyChanged(InfoPropertyName);
            }
        }

        public InfoViewModel(IGroezrockService groezrockService, INavigationService navigationService)
        {
            _groezrockService = groezrockService;
            _navigationService = navigationService;

            _navigationService.RootFrame.Navigated += RootFrame_Navigated;

            if(IsInDesignMode)
            {
                InitDesignData();
            }
        }

        private async void InitDesignData()
        {
            Info = await _groezrockService.GetInfo("design");
        }

        async void RootFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if(e.Uri.OriginalString.Contains("View/Info.xaml"))
            {
                string param = e.Uri.OriginalString.Split('=')[1];
                Info = await _groezrockService.GetInfo(param);
                MessengerInstance.Send<HtmlInfoMessage>(new HtmlInfoMessage(Info.HtmlContent));
            }
            else
            {
                if (Info != null) Info = null;
            }
        }
    }
}
