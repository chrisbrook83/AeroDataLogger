/*
 Developed from the code sample written by Johannes Paul Langner
 http://meineweltinmeinemkopf.blogspot.co.uk/2013/01/mpu6050-sensor-auslesen-mit-dem-netduino.html
 */

using System.Threading;
using AeroDataLogger.Sensors.AccelGyro;
using AeroDataLogger.Sensors.Barometer;
using AeroDataLogger.Sensors.Magnetometer;
using Microsoft.SPOT;
using AeroDataLogger.Output;
using AeroDataLogger.Data;
using System;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace AeroDataLogger
{
    public class Program
    {
        /*
         TODO
         * Glitch on status button
         * Data files not being written / saved properly (keeps using same file)
         */

        public static void Main()
        {
            try
            {
                Run();
            }
            catch (System.Exception ex)
            {
                Log.WriteLine("UNHANDLED EXCEPTION! " + ex);
                Log.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }

        private static void Run()
        {
            Log.AttachLogSink(new RS232Writer()); // get this in early
            Log.WriteLine("\n--- Logging Initialisation ---");
            Log.AttachLogSink(new TextFileWriter("Logs", "Log"));

            Log.WriteLine("\n--- AeroDataLogger: Boot Sequence Start ---");

            MPU6050Device mpu6050 = new MPU6050Device();  // initalise this before the compass!
            MS5611Baro baro = new MS5611Baro();
            HMC5883L compass = new HMC5883L();

            Log.WriteLine("Initialisation successful! Waiting to begin recording...\n");
            Controller.SignalReady();
            while (!Controller.Recording) { Thread.Sleep(100); };

            Log.WriteLine("Starting data capture.");
            AccelerationAndGyroData inertialResult;
            RawData rawMagnetrometry;
            double temp = 0;
            double pressure = 0;

            using (var fileDataSink = new FileDataSink())
            {
                var dataSinks = new IDataSink[] { fileDataSink, new DebuggerDataSink() };
                DataHandler dataHandler = new DataHandler(dataSinks);
                dataHandler.WriteHeader(new[] { "Ax", "Ay", "Az", "Rx", "Ry", "Rz", "Mx", "My", "Mz", "P" });

                while (Controller.Recording)
                {
                    // Gather data from sensors
                    inertialResult = mpu6050.GetSensorData();
                    baro.ReadTemperatureAndPressure(out temp, out pressure);
                    rawMagnetrometry = compass.Raw;

                    // Emit data to handlers
                    dataHandler.HandleData(new object[] 
                                            {
                                                inertialResult.Ax,
                                                inertialResult.Ay,
                                                inertialResult.Az,
                                                inertialResult.Rx,
                                                inertialResult.Ry,
                                                inertialResult.Rz,
                                                rawMagnetrometry.X,
                                                rawMagnetrometry.Y,
                                                rawMagnetrometry.Z,
                                                pressure
                                            });

                    Thread.Sleep(100);
                }
            }

            Log.WriteLine("Data capture stopped.");
            Log.WriteLine("Exiting.\n\n");
            Log.Close();
        }

        private static class Controller
        {
            private static InterruptPort _button;

            private static bool _ready;

            public static bool Recording { get; private set; }

            static Controller()
            {
                _button = new InterruptPort(Pins.GPIO_PIN_D7, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
                _button.OnInterrupt += new NativeEventHandler(button_OnInterrupt);
                _ready = false;
                Recording = false; 
                StatusLed.Off();
            }

            public static void SignalReady()
            {
                StatusLed.On();
                _ready = true;
            }

            private static void button_OnInterrupt(uint data1, uint data2, DateTime time)
            {
                _button.Interrupt = Port.InterruptMode.InterruptNone;

                if (_ready)
                {
                    Recording = !Recording;
                    if (Recording)
                    {
                        StatusLed.Flash();
                    }
                    else
                    { 
                        StatusLed.Off();
                    }

                    Thread.Sleep(100); // glitch filtering
                    _button.Interrupt = Port.InterruptMode.InterruptEdgeHigh;
                }
            }
        }
    }
}
