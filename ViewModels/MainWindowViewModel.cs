using FanController.Models;
using Prism.Mvvm;
using System;
using System.Timers;

namespace FanController.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Fan Controller";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

       
        public MainWindowViewModel()
        {

           
        }

        
    }
}
