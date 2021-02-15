using FanController.Events;
using FanController.Models;
using FanController.ServiceContainers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace FanController.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        IEventAggregator _ea;
        private static Timer aTimer;
        private int _voltagePWM;
        private int _lockedTemperature;
        private FanSpeedAnalyzer FanAnalyzer = new FanSpeedAnalyzer(3);
        private Controller SpeedController = new Controller();
        private bool enableTimer = true;
        public int temperature { get { return (int)TemperatureAnalyzer.GPUTemp; } }
        public int voltagePWM { get { return _voltagePWM; } private set { SetProperty(ref _voltagePWM, value); } }
        public int lockedTemperature { get { return _lockedTemperature; } private set { SetProperty(ref _lockedTemperature, value); } }
        public int fanSpeed { get { return (int)FanAnalyzer.FanSpeed; } }

        public SettingsViewModel(IEventAggregator ea)
        {
            _ea = ea;
            ServiceContainer.eventAggregator.GetEvent<VoltagePWMMessage>().Subscribe((int voltage) => { SpeedController.UpdateFanSpeed(voltage); voltagePWM = voltage; });
            ServiceContainer.eventAggregator.GetEvent<LockedTemperatureMessage>().Subscribe((int lockedTemp) => lockedTemperature = lockedTemp);
            ServiceContainer.eventAggregator.GetEvent<EditingCurve>().Subscribe((bool enableTimerMessage) => enableTimer = enableTimerMessage);
            TemperatureAnalyzer.Initialize();
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            TemperatureAnalyzer.Update();
            FanAnalyzer.Update();
            RaisePropertyChanged(nameof(temperature));
            RaisePropertyChanged(nameof(fanSpeed));

            if(!enableTimer) ServiceContainer.eventAggregator.GetEvent<TemperatureMessage>().Publish(temperature);
        }
    }
}
