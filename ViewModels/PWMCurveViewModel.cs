using FanController.Events;
using FanController.Models;
using FanController.ServiceContainers;
using FanController.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml;
using System.Xml.Serialization;

namespace FanController.ViewModels
{
    class PWMCurveViewModel : BindableBase
    {
        int previousTemperature = 0;

        private ObservableCollection<PointViewModel> _listOfPoints;
        public ObservableCollection<PointViewModel> listOfPoints
        {
            get { return _listOfPoints; }
            set { SetProperty(ref _listOfPoints, value); }
        }

        private PointCollection _polyline = new PointCollection();
        public PointCollection polyline
        {
            get { return _polyline; }
            set { SetProperty(ref _polyline, value); }
        }

        private List<RealPoint> function;
        private ObservableCollection<PointViewModel> _drawableFunction;
        public ObservableCollection<PointViewModel> drawableFunction { get { return _drawableFunction; } set { SetProperty(ref _drawableFunction, value); } }

        public double PanelX { get { return PointViewModel.sourceX; } set { PointViewModel.sourceX = value; } }
        public double PanelY { get { return PointViewModel.sourceY; } set { PointViewModel.sourceY = value; } }

        private List<int> counterList;

        public int width { get; set; } = 600;
        public int height { get; set; } = 300;

        private PointViewModel attachedPoint;


        private ICommand _previewMouseMove;
        public ICommand PreviewMouseMove
        {
            get
            {
                return _previewMouseMove ?? (_previewMouseMove = new DelegateCommand(MovePoint));
            }
        }

        public ICommand _removePoint;
        public ICommand removePoint
        {
            get
            {
                return _removePoint ?? (_removePoint = new DelegateCommand<object>(RemovePoint));
            }
        }

        public ICommand _addPoint;
        public ICommand addPoint
        {
            get
            {
                return _addPoint ?? (_addPoint = new DelegateCommand(AddPoint));
            }
        }

        private void AddPoint()
        {
            PointViewModel new_point = new PointViewModel(PanelX, PanelY, "B");
            listOfPoints.Add(new_point);
            new_point.PointDettached += (sender, args) => { SortPoints((PointViewModel)sender);  attachedPoint = null; };
            new_point.PointAttached += (sender, args) => attachedPoint = (PointViewModel)sender;
            SortPoints(new_point);
        }

        public void SortPoints(PointViewModel sender)
        {
            listOfPoints = new ObservableCollection<PointViewModel>(listOfPoints.OrderBy(x => x.RectX).ToList());
            bool position = false;
            foreach (PointViewModel point in listOfPoints)
            {
                if (point.id == sender.id) position = true;
                else if (position == false)
                {
                    if (point.PanelY < sender.PanelY) point.PanelY = sender.PanelY;
                }
                else
                {
                    if (point.PanelY > sender.PanelY) point.PanelY = sender.PanelY;
                }
                point.CalculateRealPosition();
            }
            //GenerateCurve();
            XmlSerializer xs = new XmlSerializer(typeof(List<RealPoint>));
            using (StreamWriter wr = new StreamWriter("settings.xml"))
            {
                List<RealPoint> listOfRealPoints = new List<RealPoint>();
                foreach(var x in _listOfPoints)
                {
                    listOfRealPoints.Add(x.realPoint);
                }
                xs.Serialize(wr, listOfRealPoints);
            }
            DrawPolyline();
        }

        public void DrawPolyline()
        {
            PointCollection newpolyline = new PointCollection();
            foreach (PointViewModel point in listOfPoints)
            {
                newpolyline.Add(new Point { X = point.PanelX, Y = point.PanelY });
                //Console.WriteLine(point.PanelX + " " + point.PanelY);
            }
            polyline = newpolyline;
            RaisePropertyChanged(nameof(polyline));
            GenerateCurve();
        }

        void GenerateCounterList()
        {
            counterList = new List<int>();
            foreach (var x in drawableFunction) counterList.Add(0);
        }

        private void RemovePoint(object xo)
        {
            int id = (int)xo;
            PointViewModel x = listOfPoints.FirstOrDefault(point => point.id == id);
            listOfPoints.Remove(x);
            DrawPolyline();
        }

        void MovePoint()
        {
            if (attachedPoint != null)
            {
                attachedPoint.Move();
            }
        }

        public PWMCurveViewModel()
        {
            ServiceContainer.eventAggregator.GetEvent<TemperatureMessage>().Subscribe((int temp) =>
            {

                ServiceContainer.eventAggregator.GetEvent<EditingCurve>().Publish(true);
                lastSetTemperature = AnalyzeTemperatureChange(temp);

                ServiceContainer.eventAggregator.GetEvent<EditingCurve>().Publish(false);
                //ServiceContainer.eventAggregator.GetEvent<VoltagePWMMessage>().Publish((int)drawableFunction[temp].realPoint.y);
                ServiceContainer.eventAggregator.GetEvent<VoltagePWMMessage>().Publish((int)drawableFunction[lastSetTemperature].realPoint.y);
                ServiceContainer.eventAggregator.GetEvent<LockedTemperatureMessage>().Publish(lastSetTemperature);
                drawableFunction[temp].visible = true;
                if(temp != previousTemperature) drawableFunction[previousTemperature].visible = false;
                previousTemperature = temp;
            });

            PointViewModel.ystop = height;
            PointViewModel.xstop = width;
            try
            {
                using (StreamReader rd = new StreamReader("settings.xml"))
                {
                    List<RealPoint> listOfRealPoints;
                    XmlSerializer xs = new XmlSerializer(typeof(List<RealPoint>));
                    listOfRealPoints = xs.Deserialize(rd) as List<RealPoint>;

                    _listOfPoints = new ObservableCollection<PointViewModel>();
                    foreach (var x in listOfRealPoints)
                    {
                        _listOfPoints.Add(new PointViewModel(x, false));
                    }
                }
            }
            catch
            {
                _listOfPoints = new ObservableCollection<PointViewModel> { new PointViewModel(100, 100, "A"),
            new PointViewModel(150, 150, "B"),
            new PointViewModel(200, 200, "C"),
            new PointViewModel(250, 150, "B"),
            new PointViewModel(300, 200, "C")};
            }
            foreach (PointViewModel p in _listOfPoints)
            {
                p.PointDettached += (sender, args) => { SortPoints((PointViewModel)sender); };
                p.PointAttached += (sender, args) => { attachedPoint = (PointViewModel)sender; };


                SortPoints(p);
            }
        }


        int lastSetTemperature = 20;
        private int AnalyzeTemperatureChange(int temperature)
        {
            if (temperature == lastSetTemperature)
            {
                for (int i = 0; i < counterList.Count(); ++i)
                    counterList[i] = 0;
            }
            else if(temperature > lastSetTemperature)
            {
                for (int i = lastSetTemperature+1; i <= temperature; ++i)
                    ++counterList[i];
                for (int i = 0; i <= lastSetTemperature; ++i)
                    counterList[i] = 0;
            }
            else if(temperature < lastSetTemperature - 3)
            {
                for (int i = lastSetTemperature-1; i >= temperature; --i)
                    ++counterList[i];
                for (int i = lastSetTemperature; i < counterList.Count(); ++i)
                    counterList[i] = 0;
            }
            int temp = lastSetTemperature;
            for (int i = 0; i < counterList.Count(); ++i)
            {
                if (counterList[i] >= 5)
                {
                    temp = i;
                    if (temperature < lastSetTemperature) break;
                }

            }
            return temp;
        }


        private void GenerateCurve()
        {
            ServiceContainer.eventAggregator.GetEvent<EditingCurve>().Publish(true);
            if (!listOfPoints.Any())
            {
                return;
            }
            List<RealPoint> function = new List<RealPoint>();
            this.drawableFunction = new ObservableCollection<PointViewModel>();
            for (int i = 0; i < 101; ++i)
            {
                function.Add(new RealPoint() { x = i });
            }

            List<RealPoint> listOfRealPoints = new List<RealPoint>();
            foreach (PointViewModel point in listOfPoints)
            {
                listOfRealPoints.Add(point.realPoint);
                //Console.WriteLine(point.realPoint.x);
            }

            var pairs = listOfRealPoints.Pairwise();
            foreach (RealPoint point in function)
            {
                if (point.x < listOfRealPoints[0].x)
                {
                    point.y = listOfRealPoints[0].y;
                    this.drawableFunction.Add(new PointViewModel(point, true));
                }
                //Console.WriteLine(pair.Item1.x + " " + pair.Item2.x);
                else if (point.x > listOfRealPoints.Last().x)
                {
                    point.y = listOfRealPoints.Last().y;
                    this.drawableFunction.Add(new PointViewModel(point, true));
                }
                else foreach (var pair in pairs)
                    {
                        if (pair.Item1.x <= point.x && pair.Item2.x >= point.x)
                        {
                            //Console.WriteLine(pair.Item1.x + " < " + point.x + " < " + pair.Item2.x);
                            point.y = ((pair.Item2.y - pair.Item1.y) / (pair.Item2.x - pair.Item1.x) * (point.x - pair.Item1.x)) + pair.Item1.y;

                            this.drawableFunction.Add(new PointViewModel(point, true));
                            break;
                        }
                    }
            }
            this.function = function;

            GenerateCounterList();
            ServiceContainer.eventAggregator.GetEvent<EditingCurve>().Publish(false);
        }
    }
}
