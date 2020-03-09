/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:RPARobot"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using RPARobot.Navigation;
using System;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance;
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            Instance = this;
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

            SimpleIoc.Default.Register<StartupViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<UserPreferencesViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<FlowViewModel>();
            SimpleIoc.Default.Register<TaskViewModel>();

            SimpleIoc.Default.Register(CreateNavigationService);
        }

        public static TaskFormViewModel CreateTaskFormViewModel(Models.ScheduledTask task = null) => new TaskFormViewModel(task);

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();

            navigationService.AddPage(nameof(Flow), new Uri("/RPARobot;component/Pages/FlowPage.xaml", UriKind.Relative));
            navigationService.AddPage(nameof(Task), new Uri("/RPARobot;component/Pages/TaskPage.xaml", UriKind.Relative));
            navigationService.AddPage(nameof(Log), new Uri("/RPARobot;component/Pages/LogPage.xaml", UriKind.Relative));
            navigationService.AddPage(nameof(UserPreferences), new Uri("/RPARobot;component/Pages/UserPreferencesPage.xaml", UriKind.Relative));

            return navigationService;
        }
        public StartupViewModel Startup
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartupViewModel>();
            }
        }


        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }


        public UserPreferencesViewModel UserPreferences
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UserPreferencesViewModel>();
            }
        }

        public RegisterViewModel Register
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RegisterViewModel>();
            }
        }

        public FlowViewModel Flow => ServiceLocator.Current.GetInstance<FlowViewModel>();
        public TaskViewModel Task => ServiceLocator.Current.GetInstance<TaskViewModel>();
        public ViewModelBase Log { get; set; }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}