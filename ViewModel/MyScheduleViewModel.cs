using GalaSoft.MvvmLight;
using Groezrock2014.Models;
using Groezrock2014.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.ViewModel
{
    public class MyScheduleViewModel:ViewModelBase
    {
        private IGroezrockService _service;
        private INavigationService _navigationService;

        /// <summary>
        /// The <see cref="MySchedule" /> property's name.
        /// </summary>
        public const string MySchedulePropertyName = "MySchedule";

        private Band[] _mySchedule = null;

        /// <summary>
        /// Sets and gets the BandsOnDayOne property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Band[] BandsOnDayOne
        {
            get
            {
                if(_mySchedule == null) return new Band[0];
                return _mySchedule.Where(b => b.Day == GroezrockConstants.DayOne.Day).OrderBy(x => x.Starts).ToArray();
            }
        }

        /// <summary>
        /// Sets and gets the BandsOnDayTwo property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Band[] BandsOnDayTwo
        {
            get
            {
                if(_mySchedule == null) return new Band[0];
                return _mySchedule.Where(b => b.Day == GroezrockConstants.DayTwo.Day).OrderBy(x => x.Starts).ToArray();
            }
        }

        public DateTime DayOne
        {
            get
            {
                return GroezrockConstants.DayOne;
            }
        }

        public DateTime DayTwo
        {
            get
            {
                return GroezrockConstants.DayTwo;
            }
        }

        public MyScheduleViewModel(IGroezrockService service, INavigationService navigationService)
        {
            _service = service;
            _navigationService = navigationService;

            InitData();
        }

        private void InitData()
        {
            _navigationService.RootFrame.Navigated += RootFrame_Navigated;
        }

        /// <summary>
        /// Has to be re-evaluted on each page request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void RootFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.OriginalString.Contains("MySchedule"))
            {
                _mySchedule = await _service.GetMySchedule();
                RaisePropertyChanged("BandsOnDayOne");
                RaisePropertyChanged("BandsOnDayTwo");
            }
        }

        
    }
}
