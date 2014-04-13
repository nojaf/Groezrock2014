using GalaSoft.MvvmLight;
using Groezrock2014.Models;
using Groezrock2014.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;
using GalaSoft.MvvmLight.Command;


namespace Groezrock2014.ViewModel
{
    public class BandViewModel:ViewModelBase
    {
        private INavigationService _navigationService;
        private IGroezrockService _groezrockService;

        /// <summary>
        /// The <see cref="CurrentBand" /> property's name.
        /// </summary>
        public const string CurrentBandPropertyName = "CurrentBand";

        private Band _currentBand = null;

        /// <summary>
        /// Sets and gets the CurrentBand property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Band CurrentBand
        {
            get
            {
                return _currentBand;
            }

            set
            {
                if (_currentBand == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentBandPropertyName);
                _currentBand = value;
                RaisePropertyChanged(CurrentBandPropertyName);
            }
        }

        public RelayCommand AddToCalendarCommand { get; set; }

        /// <summary>
        /// The <see cref="AppointmentNotAdded" /> property's name.
        /// </summary>
        public const string AppointmentNotAddedPropertyName = "AppointmentNotAdded";

        private bool _appointmentNotAdded = false;

        /// <summary>
        /// Sets and gets the AppointmentNotAdded property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool AppointmentNotAdded
        {
            get
            {
                return _appointmentNotAdded;
            }

            set
            {
                if (_appointmentNotAdded == value)
                {
                    return;
                }

                RaisePropertyChanging(AppointmentNotAddedPropertyName);
                _appointmentNotAdded = value;
                RaisePropertyChanged(AppointmentNotAddedPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="AddToMySchedule" /> property's name.
        /// </summary>
        public const string AddToMySchedulePropertyName = "AddToMySchedule";



        /// <summary>
        /// Sets and gets the AddToMySchedule property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool AddToMySchedule
        {
            get
            {
                if (CurrentBand == null) return false;

                return CurrentBand.AddToMySchedule;
            }

            set
            {
                if(CurrentBand == null) return;
                if (CurrentBand.AddToMySchedule == value)
                {
                    return;
                }

                RaisePropertyChanging(AddToMySchedulePropertyName);
                CurrentBand.AddToMySchedule = value;
                if (!IsInDesignMode)
                {
                    _groezrockService.Persist();
                }
                RaisePropertyChanged(AddToMySchedulePropertyName);
            }
        }


        public BandViewModel(INavigationService navigationService, IGroezrockService groezrockService)
        {
            _navigationService = navigationService;
            _groezrockService = groezrockService;
           

            if(IsInDesignMode)
            {
                CurrentBand = _groezrockService.SelectedBand;
            }
            else
            {
                InitCommands();
                InitEvents();
            }
        }

        private void InitEvents()
        {
            _navigationService.RootFrame.Navigated += RootFrame_Navigated;
        }

        async void RootFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.OriginalString.Contains("Band"))
            {
                string bandName = e.Uri.OriginalString.Split('=')[1];
                await _groezrockService.SetActiveBand(bandName);
                CurrentBand = _groezrockService.SelectedBand;
                LookForAppointment();
                AddToMySchedule = CurrentBand.AddToMySchedule;
                RaisePropertyChanged("AddToMySchedule");
            }
            else
            {
                CurrentBand = null;
            }
        }

        private void InitCommands()
        {
            AddToCalendarCommand = new RelayCommand(AddReminder);
        }



        private void AddReminder()
        {
            if (CurrentBand == null) return;

            SaveAppointmentTask saveAppointmentTask = new SaveAppointmentTask();
            saveAppointmentTask.StartTime = CurrentBand.Starts;
            saveAppointmentTask.EndTime = CurrentBand.Ends;
            saveAppointmentTask.Subject = CurrentBand.Name;
            saveAppointmentTask.Location = _groezrockService.GetStageFromBand(CurrentBand.Name)+ ", Groezrock";
            saveAppointmentTask.IsAllDayEvent = false;
            saveAppointmentTask.Reminder = Reminder.FifteenMinutes;
            saveAppointmentTask.AppointmentStatus = Microsoft.Phone.UserData.AppointmentStatus.Busy;
            saveAppointmentTask.Show();

            AppointmentNotAdded = false;
        }



        //public async void SetCurrentBand(string bandName)
        //{
        //    await _groezrockService.SetActiveBand(bandName);
        //    CurrentBand = _groezrockService.SelectedBand;
        //    LookForAppointment();
        //}

        private void LookForAppointment()
        {
            Appointments appts = new Appointments();

            //Identify the method that runs after the asynchronous search completes.
            appts.SearchCompleted += new EventHandler<AppointmentsSearchEventArgs>(Appointments_SearchCompleted);


            //Start the asynchronous search.
            appts.SearchAsync(CurrentBand.Starts, CurrentBand.Ends, 5, "Appointments "+CurrentBand.Name);
        }

        private void Appointments_SearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {
            AppointmentNotAdded = !e.Results.Any(x => x.Subject == CurrentBand.Name);
        }

        //public void ResetBand()
        //{
        //    CurrentBand = null;
        //}
    }
}
