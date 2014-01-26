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
using AeroDataLogger.Logging;

namespace AeroDataLogger
{
    public class Program
    {
        public static void Main()
        {
            Run();
        }

        private delegate void DataAvailableHandler(object sender, DataAvailableHandlerEventArgs args);

        private static event DataAvailableHandler DataAvailable = delegate { };

        private static void Run()
        {
            Log.WriteLine("\n--- AeroDataLogger Startup ---");
            Thread.Sleep(2000);

            MPU6050Device mpu6050 = new MPU6050Device();  // initalise this before the compass!
            MS5611Baro baro = new MS5611Baro();
            HMC5883L compass = new HMC5883L();

            Log.WriteLine("Initialisation successful!\n");

            AccelerationAndGyroData inertialResult;
            RawData rawMagnetrometry;
            double temp = 0;
            double pressure = 0;

            using (TextFileWriter dataLog = new TextFileWriter("Data", "Raw"))
            {
                Log.WriteLine("Starting data capture...");

                // Write header
                dataLog.WriteLine("Ax\tAy\tAz\tRx\tRy\tRz\tMx\tMy\tMz\tP");

                while (true)
                {
                    inertialResult = mpu6050.GetSensorData();
                    baro.ReadTemperatureAndPressure(out temp, out pressure);
                    rawMagnetrometry = compass.Raw;

                    string message = inertialResult.ToString()
                        + "\tT: " + temp.ToString("f2") + "\tP: " + pressure.ToString("f2")
                        + "\tMx=" + rawMagnetrometry.X + " My= " + rawMagnetrometry.Y + " Mz= " + rawMagnetrometry.Z + "\n";

                    DataAvailable(null, new DataAvailableHandlerEventArgs(message));

                    Debug.Print(message);
                    dataLog.Write(new object[] 
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
}
