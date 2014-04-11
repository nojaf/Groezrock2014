/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Groezrock2014"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Groezrock2014.Services;
using Microsoft.Practices.ServiceLocation;


namespace Groezrock2014.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ScheduleViewModel>();
            SimpleIoc.Default.Register<BandViewModel>();

            if(ViewModelBase.IsInDesignModeStatic)
            {
                if (!SimpleIoc.Default.IsRegistered<INavigationService>())
                {
                    SimpleIoc.Default.Register<INavigationService>(() =>
                    {
                        return new GroezrockNavigationService(App.RootFrame);
                    });
                }

                if(!SimpleIoc.Default.IsRegistered<IGroezrockService>())
                {
                    SimpleIoc.Default.Register<IGroezrockService>(() =>
                    {
                        return new DesignService();
                    });
                }
            }
            else
            {
                SimpleIoc.Default.Register<INavigationService>(() =>
                {
                    return new GroezrockNavigationService(App.RootFrame);
                });
                SimpleIoc.Default.Register<IGroezrockService>(() =>
                {
                    return new GroezrockService();
                });
            }


        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ScheduleViewModel Schedule
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ScheduleViewModel>();
            }
        }

        public BandViewModel Band
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BandViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}