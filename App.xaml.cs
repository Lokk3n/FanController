using CommonServiceLocator;
using FanController.ServiceContainers;
using FanController.ViewModels;
using FanController.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Windows;

namespace FanController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        
        protected override Window CreateShell()
        {
            //var x = 

            IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            IDialogService dialogService = ServiceLocator.Current.GetInstance<IDialogService>();
            IEventAggregator eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            ServiceContainer.Initialize(regionManager, dialogService, eventAggregator);


            regionManager.RegisterViewWithRegion("ContentRegion", typeof(PWMCurve));
            regionManager.RegisterViewWithRegion("ContentRegion2", typeof(SettingsView));
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<PWMCurve, PWMCurveViewModel>();
            ViewModelLocationProvider.Register<SettingsView, SettingsViewModel>();
        }

        
    }
}
