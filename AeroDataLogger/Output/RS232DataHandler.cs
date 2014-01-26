using System;
using System.IO.Ports;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.IO;

namespace AeroDataLogger.Output
{
    internal class RS232DataHandler : IDisposable
    {
        SerialPort _port;

        public RS232DataHandler()
        {
            _port = new SerialPort(SerialPorts.COM2, 115200, Parity.None, 8, StopBits.One);
            _port.Open();
        }

        public void WriteLine(string value)
        {
            byte[] payload = System.Text.UTF8Encoding.UTF8.GetBytes(value);
            _port.Write(payload, 0, payload.Length);
        }
        
        public void OnDataAvailable(object sender, DataAvailableHandlerEventArgs args)
        {
            WriteLine(args.Data);
        }

        public void Dispose()
        {
            _port.Close();
            _port.Dispose();
        }
    }

    internal class DataAvailableHandlerEventArgs : EventArgs
    {
        public DataAvailableHandlerEventArgs(string data)
        {
            this.Data = data;
        }

        public string Data { get; private set; }
    }
}
