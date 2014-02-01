using System;
using Microsoft.SPOT;
using System.IO;
using System.IO.Ports;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace AeroDataLogger.Output
{
    internal class RS232Writer : StreamWriter, ILogSink 
    {
        private static Stream GetSerialPort()
        {
            var serialPort = new SerialPort(SerialPorts.COM2, 115200, Parity.None, 8, StopBits.One);
            serialPort.Open();
            return serialPort;
        }

        public RS232Writer()
            : base(GetSerialPort())
        {
        }

        void ILogSink.WriteLine(string value)
        {
            StreamWriterExtensions.WriteAndFlushLine(this, value);
        }

        public void TryDispose()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            // Dispose SerialPort
            base.BaseStream.Close();
            base.BaseStream.Dispose();

            base.Dispose(disposing);
        }
    }

    internal static class StreamWriterExtensions
    {
        public static void WriteAndFlushLine(this StreamWriter streamWriter, string value)
        {
            streamWriter.WriteLine(value);
            streamWriter.Flush();
        }
    }
}
