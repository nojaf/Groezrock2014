using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Groezrock2014.Models;
using Groezrock2014.Services;
using System;
using System.Threading.Tasks;
using System.Windows;


namespace Groezrock2014.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand<string> NavigateCommand { get; set; }

        public RelayCommand LoadAllCommand { get; set; }

        /// <summary>
        /// The <see cref="LoadAllProgress" /> property's name.
        /// </summary>
        public const string LoadAllProgressPropertyName = "LoadAllProgress";

        private int _loadAllProgress = 0;

        /// <summary>
        /// Sets and gets the LoadAllProgress property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int LoadAllProgress
        {
            get
            {
                return _loadAllProgress;
            }

            set
            {
                if (_loadAllProgress == value)
                {
                    return;
                }

                RaisePropertyChanging(LoadAllProgressPropertyName);
                _loadAllProgress = value;
                RaisePropertyChanged(LoadAllProgressPropertyName);
            }
        }

        private INavigationService _navigationService;
        private IGroezrockService _groezrockService;

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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navigation, IGroezrockService service)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

            }
            else
            {
                // Code runs "for real"
                _navigationService = navigation;
                _groezrockService = service;
                InitCommands();

            }
        }

        private void InitCommands()
        {
            NavigateCommand = new RelayCommand<string>(Navigate);
            LoadAllCommand = new RelayCommand(LoadAllData);
        }

        private async void LoadAllData()
        {
            MessageBoxResult result = MessageBox.Show("The download is approximitly 5Mb and could take a while. Do you want to continue?",
                                        "Work offline", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
            {
                Progress<int> loadAllProgress = new Progress<int>(ProgressUpdated);
                await _groezrockService.LoadAll(loadAllProgress);
            }
        }

        private void ProgressUpdated(int procent)
        {
            LoadAllProgress = procent;
        }

        private void Navigate(string path)
        {
            switch(path)
            {
                case "SCHEDULE":
                    _navigationService.Navigate("/View/Schedule.xaml");
                    break;
                case "MYSCHEDULE":
                    _navigationService.Navigate("/View/MySchedule.xaml");
                    break;
                case "ALLBANDS":
                    _navigationService.Navigate("/View/AllBands.xaml");
                    break;
                case "MAP":
                    _navigationService.Navigate("/View/Map.xaml");
                    break;
            }
        }
    }
}