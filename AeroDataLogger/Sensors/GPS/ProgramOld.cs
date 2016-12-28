using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.IO.Ports;
using System.Text;
using GPS.Structures;

namespace GPS
{
    /// <summary>
    /// TODO: this used to be in a separate project - need to integrate with rest of solution
    /// </summary>
    public class ProgramOld
    {
        private const string ComPort = "COM1";

        public static void MainOld()
        {
            try
            {
                //Debug.Print(new LatLong("4717.112671", "N").ToString());
                //Debug.Print(new LatLong("4717.112671", "S").ToString());
                //Debug.Print(new LatLong("00833.914843", "E").ToString());
                //Debug.Print(new LatLong("00833.914843", "W").ToString());
                GPS();
            }
            catch (Exception ex)
            {
                Debug.Print("UNHANDLED EXCEPTION! " + ex);
                Debug.Print("Stack Trace: " + ex.StackTrace);
            }
        }

        private static void GPS()
        {
            using (SerialPort port = new SerialPort(ComPort, 9600, Parity.None, 8, StopBits.One))
            {
                port.DataReceived += OnDataReceived;
                port.Open();

                while (true)
                {
                    // spin until exit
                    Thread.Sleep(100);
                }

                //port.DataReceived -= OnDataReceived;
                //port.Close();
            }
        }

        private static StringBuilder _sb = new StringBuilder();
        private static object _lock = new object();

        private static void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort serialPort = (SerialPort)sender;
                int bytesToRead = serialPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    byte[] readBuffer = new byte[serialPort.BytesToRead];
                    int bytesRead = 0;
                    do
                    {
                        bytesRead += serialPort.Read(readBuffer, bytesRead, readBuffer.Length - bytesRead);
                    }
                    while (bytesRead < bytesToRead);

                    for (int i = 0; i < readBuffer.Length; i++)
                    {
                        char c = (char)readBuffer[i];
                        if (c == '$' && _sb.Length != 0)
                        {
                            ParseNmeaFrame(_sb.ToString().Trim());
                            _sb.Clear();
                        }

                        _sb.Append(c.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("UNHANDLED EXCEPTION in handler! " + ex);
                Debug.Print("Stack Trace: " + ex.StackTrace);
            }
        }

        private static void ParseNmeaFrame(string frame)
        {
            var checksum = CalculateChecksum(frame);

            string[] fields = frame.Split(',');
            if (!IsChecksumOk(fields, checksum))
            {
                Debug.Print("Discarding corrupt message");
                return;
            }

            if (fields[0] != "$GPRMC")
            {
                return;
            }

            Debug.Print(frame);
            var gpgll = new GPRMC(fields);
            Debug.Print(gpgll.ToString());
        }

        private static string CalculateChecksum(string sentence)
        {
            int checksum = 0;
            var chars = sentence.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (i == 0 && c != '$')
                {
                    // Discard if the start of the sentence was missing
                    return null;
                }
                else if (i == 1)
                {
                    // Grab the first character after the '$'...
                    checksum = (byte)c;
                }
                else if (c == '*')
                {
                    // ...then loop through each char until the '*'...
                    break;
                }
                else
                {
                    // ...XOR'ing as we go.
                    checksum ^= (byte)c;
                }
            }

            // Return the checksum, formatted as a two-character hexadecimal
            return checksum.ToString("X2");
        }

        private static bool IsChecksumOk(string[] fields, string calculatedChecksum)
        {
            // Check the last field is a checksum
            var checkSum = fields[fields.Length - 1].Split('*');
            if (checkSum.Length == 2)
            {
                var checkSumOnMessage = checkSum[1];
                if (checkSumOnMessage == calculatedChecksum)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
