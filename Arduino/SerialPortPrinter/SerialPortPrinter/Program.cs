using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortPrinter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Available Ports : ");
            foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                Console.WriteLine($" - {portName}");
            }
            Console.ReadLine();

        }
    }
}
