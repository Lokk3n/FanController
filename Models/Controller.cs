using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace FanController.Models
{
    public class Controller
    {
        bool isConnected = false;
        string selectedPort;
        String[] ports;
        SerialPort port;

        public Controller()
        {
            //getAvailableComPorts();

            //foreach (string port in ports)
            //{
            //    Console.WriteLine(port);
            //}
            //Console.WriteLine("");

            //Console.Write("Choose a port: ");

            //selectedPort = Console.ReadLine();

            //Console.WriteLine("");

            selectedPort = "COM3";

            connectToArduino();
        }

        private void getAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
        }

        private void connectToArduino()
        {
            isConnected = true;
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
        }

        private void disconnectFromArduino()
        {
            isConnected = false;
            port.Close();
        }

        public void UpdateFanSpeed(int _percentage)
        {
            //float arduinoValue = _percentage / (100f / 79f);

            //Console.WriteLine("Sending: " + arduinoValue.ToString());

            string _send = "#UPDATE" + _percentage.ToString() + "\n";
            port.Write(_send);
            //Console.WriteLine("Sent: " + _send);
        }
    }
}
