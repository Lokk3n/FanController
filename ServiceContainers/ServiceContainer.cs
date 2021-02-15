using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanController.ServiceContainers
{
    public static class ServiceContainer
    {

        public static IRegionManager regionManager;
        public static IDialogService dialogService;
        public static IEventAggregator eventAggregator;

        public static void Initialize(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator)
        {
            ServiceContainer.regionManager = regionManager;
            ServiceContainer.dialogService = dialogService;
            ServiceContainer.eventAggregator = eventAggregator;
        }
    }
}
