using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanController.Models
{
    class FanSpeedAnalyzer
    {
        public ISensor FanSensor;
        public float? FanSpeed
        {
            get
            {
                return FanSensor != null ? FanSensor.Value : 0;
            }
        }
        public FanSpeedAnalyzer(int id)
        {
            Computer myComputer = new Computer();
            myComputer.Open();
            myComputer.MainboardEnabled = true;
            foreach (var hardware in myComputer.Hardware)
            {
                hardware.Update();

                foreach (var subhardware in hardware.SubHardware)
                {
                    // This will be in the SuperIO
                    subhardware.Update();
                    if (subhardware.Sensors.Length > 0) // Index out of bounds check
                    {
                        foreach (var sensor in subhardware.Sensors)
                        {
                            // Look for the main fan sensor
                            if (sensor.SensorType == SensorType.Fan && sensor.Name.Equals("Fan #" + id, StringComparison.OrdinalIgnoreCase))
                            //if (sensor.SensorType == SensorType.Fan)
                            {
                                //Console.WriteLine(sensor.Name);
                                FanSensor = sensor;
                                //Console.WriteLine("CPU Fan Speed:" + Convert.ToString((int)(float)sensor.Value) + " RPM");
                            }
                        }
                    }
                }
            }
        }



        public void Update()
        {
            if (FanSensor != null)
            {
                FanSensor.Hardware.Update();//This line refreshes the sensor values
                //Console.WriteLine("Fan Speed:" + Convert.ToString((int)(float)FanSensor.Value) + " RPM");
            }
            else
            {
                //Console.WriteLine("Could not find the speed Sensor.");
            }
        }
    }
}
