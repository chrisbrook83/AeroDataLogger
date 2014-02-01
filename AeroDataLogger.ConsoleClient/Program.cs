using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;

namespace RS232Client
{
    public class Program
    {
        private const string ComPort = "COM3";

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("--- Aero Data Logger : Serial Console ---");
            Console.WriteLine("Listening on {0} - Press any key to exit", ComPort);
            Console.WriteLine("-----------------------------------------\n"); 
            Console.ForegroundColor = ConsoleColor.White;

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            var task = Task.Factory.StartNew(() => ListenOnComPort(ct));

            // Exit on key press
            Console.ReadKey();
            
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("---------------- Exiting ----------------");
            tokenSource.Cancel();
            Task.WaitAll(task);
            Thread.Sleep(2000);
        }

        private static void ListenOnComPort(CancellationToken cancellationToken)
        {
            using (SerialPort port = new SerialPort(ComPort, 115200, Parity.None, 8, StopBits.One))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    CheckAndReconnectPortIfRequired(port);

                    // Yield briefly, until checking if user has cancelled
                    Thread.Sleep(100);
                }

                port.DataReceived -= OnDataReceived;
                port.Close();
            }
        }

        private static void CheckAndReconnectPortIfRequired(SerialPort port)
        {
            if (!port.IsOpen)
            {
                while (!SerialPort.GetPortNames().Contains(ComPort))
                {
                    Thread.Sleep(500);
                }

                port.DataReceived += OnDataReceived;
                port.Open();
            }
        }

        static void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            int bytesToRead = serialPort.BytesToRead;
            byte[] readBuffer = new byte[serialPort.BytesToRead];
            
            int bytesRead = 0;
            do
            {
                bytesRead += serialPort.Read(readBuffer, bytesRead, readBuffer.Length - bytesRead);
            } 
            while (bytesRead < bytesToRead);

            string s = UTF8Encoding.UTF8.GetString(readBuffer);
            Console.Write(s);
        }
    }
}
