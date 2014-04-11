using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Groezrock2014.Models;
using Groezrock2014.Services;


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
            InitData();
        }

        private async void InitData()
        {
            Schedules = await _groezrockService.GetSchedules();
        }

        private void InitCommands()
        {
            NavigateCommand = new RelayCommand<string>(Navigate);
        }

        private void Navigate(string path)
        {
            switch(path)
            {
                case "SCHEDULE":
                    _navigationService.Navigate("/View/Schedule.xaml");
                    break;
            }
        }
    }
}