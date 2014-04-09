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
    public class ScheduleViewModel:ViewModelBase
    {


        /// <summary>
        /// The <see cref="Schedules" /> property's name.
        /// </summary>
        public const string SchedulesPropertyName = "Schedules";

        private Schedule[] _schedules = null;

        /// <summary>
        /// Sets and gets the Schedules property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Schedule[] Schedules
        {
            get
            {
                return _schedules;
            }

            set
            {
                if (_schedules == value)
                {
                    return;
                }

                RaisePropertyChanging(SchedulesPropertyName);
                _schedules = value;
                RaisePropertyChanged(SchedulesPropertyName);
            }
        }

        public RelayCommand<string> NavigateToBandCommand { get; set; }

        private IGroezrockService _groezrockService;
        private INavigationService _navigationService;
        public ScheduleViewModel(IGroezrockService groezrockService, INavigationService navigationService)
        {
            if(IsInDesignMode)
            {
                _groezrockService = new DesignService();
                InitData();
            }
            else
            {
                _groezrockService = groezrockService;
                _navigationService = navigationService;
            }

            InitData();
            InitCommands();
        }

        private void InitCommands()
        {
            NavigateToBandCommand = new RelayCommand<string>(NavigateToBand);
        }

        private void NavigateToBand(string bandName)
        {
            _groezrockService.SetActiveBand(bandName);
            _navigationService.Navigate("/Views/Band.xaml");
        }

        private async void InitData()
        {
            Schedules = await _groezrockService.GetSchedules();
        }
    }
}
