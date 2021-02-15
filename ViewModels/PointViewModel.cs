using FanController.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using FanController.Models;

namespace FanController.ViewModels
{
    public class PointViewModel : BindableBase
    {
        static int count = 0;
        public int id { get; set; }
        string name;
        bool _captured = false;
        
        public bool captured
        {
            get { return _captured; }
            set
            {
                _captured = value;
                if (value == false) PointDettached(this, new EventArgs());
                else PointAttached(this, new EventArgs());
            }
        }

        private bool _visible;
        public bool visible
        {
            get { return _visible; }
            set { SetProperty(ref _visible, value); }
        }
        public readonly static double xstart = 0;
        public readonly static double ystart = 0;
        public static double xstop;
        public static double ystop;
        public ICommand _leftButtonDownCommand;
        public ICommand _leftButtonUpCommand;
        public ICommand _previewMouseMove;
        public ICommand _leftMouseButtonUp;

        private PointViewModel()
        {

        }

        public PointViewModel(double px, double py, string name)
        {
            PanelX = px;
            PanelY = py;
            this.name = name;
            count++;
            id = count;
            CalculateRealPosition();
        }
        public PointViewModel(RealPoint p, bool isLookup)
        {
            PanelX = ((xstop - xstart) / (100 - 0) * p.x - 0 * (xstop - xstart) / (100 - 0));
            PanelY = ((ystop - ystart) / (0 - 100) * p.y - 100 * (ystop - ystart) / (0 - 100));
            realPoint.x = p.x;
            realPoint.y = p.y;
            if (!isLookup)
            {
                count++;
                id = count;
            }
        }


        public delegate void EventHandler(object sender, EventArgs args);
        public event EventHandler<EventArgs> PointDettached = delegate { };
        public event EventHandler<EventArgs> PointAttached = delegate { };


        public void Move()
        {
            if (captured)
            {
                PanelX = sourceX;
                PanelY = sourceY;
                bool result = true;
                if (PanelX - 2 < xstart) { PanelX = PanelX + 5; result = false; }
                else if (PanelX + 2 > xstop) { PanelX = PanelX - 5; result = false; }
                else if (PanelY - 2 < ystart) { PanelY = PanelY + 5; result = false; }
                else if (PanelY + 2 > ystop) { PanelY = PanelY - 5; result = false; }
                //RectX = PanelX - 5.0;
                //RectY = PanelY - 5.0;
                if (!result) captured = false;
            }
        }

        public ICommand LeftMouseButtonUp
        {
            get
            {
                return _leftMouseButtonUp ?? (_leftMouseButtonUp = new DelegateCommand(GrabAndRelease));
            }
        }

        void GrabAndRelease()
        {
            if (captured)
            {
                captured = false;
                layer = 1;
            }
            else
            {

                captured = true;
                layer = 2;
            }
        }

        public ICommand LeftMouseButtonDown
        {
            get
            {
                return _leftButtonDownCommand ?? (_leftButtonDownCommand = new DelegateCommand(null));
            }
        }

        private double _panelX;
        private double _panelY;
        //private double _rectX;
        //private double _rectY;
        private int _layer = 1;

        public int layer
        {
            get { return _layer; }
            set { SetProperty(ref _layer, value); }
        }
        public static double sourceX { get; set; }
        public static double sourceY { get; set; }

        public double RectX
        {
            get { return _panelX - 5; }
        }

        public double RectY
        {
            get { return _panelY - 5; }
        }

        public double PanelX
        {
            get { return _panelX; }
            set
            {
                if (value.Equals(_panelX)) return;
                SetProperty(ref _panelX, value);
                RaisePropertyChanged("RectX");
            }
        }

        public double PanelY
        {
            get { return _panelY; }
            set
            {
                if (value.Equals(_panelY)) return;
                SetProperty(ref _panelY, value);
                RaisePropertyChanged("RectY");
            }
        }

        //Rzeczywiste wartości
        public RealPoint realPoint = new RealPoint();

        public void CalculateRealPosition()
        {
            realPoint.x = PanelX / xstop * 100;
            realPoint.y = (ystop - PanelY) / ystop * 100;
            //Console.WriteLine("Start X: " + PanelX + " Y: " + PanelY);
            //Console.WriteLine("Stop X: " + xstop + " Y: " + ystop);
            //Console.WriteLine("Wynik: " + realPoint.x + " " + realPoint.y);
        }

        
    }
}

