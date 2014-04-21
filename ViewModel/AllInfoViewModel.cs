using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Groezrock2014.Models;
using Groezrock2014.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.ViewModel
{
    public class AllInfoViewModel:ViewModelBase
    {
        private IGroezrockService _groezrockService;
        private INavigationService _navigationService;

        /// <summary>
        /// The <see cref="Info" /> property's name.
        /// </summary>
        public const string InfoPropertyName = "Info";

        private Info[] _info = null;

        /// <summary>
        /// Sets and gets the Info property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Info[] Info
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

        public RelayCommand<string> NavigateToInfoCommand { get; set; } 

        public AllInfoViewModel(IGroezrockService groezrockService, INavigationService navigationService)
        {
            _groezrockService = groezrockService;
            _navigationService = navigationService;

            InitData();
            InitCommands();
        }

        private void InitCommands()
        {
            NavigateToInfoCommand = new RelayCommand<string>(NavigateToInfo);
        }

        private void NavigateToInfo(string title)
        {
            _navigationService.Navigate("/View/Info.xaml?title=" + title);   
        }

        private async void InitData()
        {
            Info = await _groezrockService.GetAllInfo();
        }
    }
}
