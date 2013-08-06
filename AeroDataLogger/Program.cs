/*
 Developed from the code sample written by Johannes Paul Langner
 http://meineweltinmeinemkopf.blogspot.co.uk/2013/01/mpu6050-sensor-auslesen-mit-dem-netduino.html
 */

using System.Threading;
using AeroDataLogger.Sensors.AccelGyro;
using AeroDataLogger.Sensors.Barometer;
using AeroDataLogger.Sensors.Magnetometer;
using Microsoft.SPOT;

namespace AeroDataLogger
{
    public class Program
    {
        public static void Main()
        {
            Run();

            //using (Log log = new Log())
            //{
            //    log.Write("Hello");
            //    log.Write("This is a test...");
            //    log.Write("Goodbye!");
            //}
        }

        private static void Run()
        {
            MPU6050Device mpu6050 = new MPU6050Device();  // initalise this before the compass!
            MS5611Baro baro = new MS5611Baro();
            HMC5883L compass = new HMC5883L();

            AccelerationAndGyroData sensorResult;
            RawData rawMagnetrometry;
            double temp = 0;
            double pressure = 0;

            while (true)
            {
                sensorResult = mpu6050.GetSensorData();
                baro.ReadTemperatureAndPressure(out temp, out pressure);
                rawMagnetrometry = compass.Raw;

                Debug.Print(sensorResult.ToString() 
                    + "\tT: " + temp.ToString("f2") + "\tP: " + pressure.ToString("f2") 
                    + "\tMx=" + rawMagnetrometry.X + " My= " + rawMagnetrometry.Y + " Mz= " + rawMagnetrometry.Z);

                Thread.Sleep(100);
            }
        }
    }
}
