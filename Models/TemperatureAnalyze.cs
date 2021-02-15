using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FanController.Models
{
    internal static class TemperatureAnalyzer
    {
        static ISensor GPUTempSensor;
        public static float? GPUTemp
        {
            get { return GPUTempSensor.Value; }
        }

        static public void Initialize()
        {
            Computer myComputer = new Computer();
            myComputer.Open();
            myComputer.GPUEnabled = true;
            myComputer.CPUEnabled = true;
            foreach (var hardwareItem in myComputer.Hardware)
            {
                Console.WriteLine(hardwareItem.Name);
                if (hardwareItem.HardwareType == HardwareType.GpuNvidia)
                //if (hardwareItem.HardwareType == HardwareType.CPU)
                    {
                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            GPUTempSensor = sensor;
                        }
                    }
                }

            }

        }

        

        public static void Update()
        {
            if (GPUTempSensor != null)
            {
                GPUTempSensor.Hardware.Update();//This line refreshes the sensor values
                //Console.WriteLine("The current temperature is {0}", GPUTempSensor.Value);
            }
            else
            {
                //Console.WriteLine("Could not find the GPU Temperature Sensor.");
            }
        }
    }
}
