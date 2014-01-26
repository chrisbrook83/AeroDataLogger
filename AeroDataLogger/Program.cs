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

namespace AeroDataLogger
{
    public class Program
    {
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
            Log.WriteLine("\n--- Logging Initialisation ---");
            Log.AttachLogSink(new RS232Writer());
            Log.AttachLogSink(new TextFileWriter("Logs", "Log"));

            Log.WriteLine("\n--- AeroDataLogger: Boot Sequence Start ---");

            MPU6050Device mpu6050 = new MPU6050Device();  // initalise this before the compass!
            MS5611Baro baro = new MS5611Baro();
            HMC5883L compass = new HMC5883L();

            Log.WriteLine("Initialisation successful!\n");

            AccelerationAndGyroData inertialResult;
            RawData rawMagnetrometry;
            double temp = 0;
            double pressure = 0;

            var dataSinks = new IDataSink[] { new FileDataSink(), new DebuggerDataSink() };
            DataHandler dataHandler = new DataHandler(dataSinks);

            Log.WriteLine("Starting data capture...");
            dataHandler.WriteHeader(new [] { "Ax", "Ay", "Az", "Rx", "Ry", "Rz", "Mx", "My", "Mz", "P" });

            while (true)
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
    }
}
