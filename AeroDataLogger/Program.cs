/*
 Developed from the code sample written by Johannes Paul Langner
 http://meineweltinmeinemkopf.blogspot.co.uk/2013/01/mpu6050-sensor-auslesen-mit-dem-netduino.html
 */

using System.Threading;
using AeroDataLogger.Sensors.AccelGyro;
using Microsoft.SPOT;

namespace AeroDataLogger
{
    public class Program
    {
        public static void Main()
        {
            MPU6050Device mpu6050 = new MPU6050Device();

            AccelerationAndGyroData sensorResult;
            while (true)
            {
                sensorResult = mpu6050.GetSensorData();
                Debug.Print(sensorResult.ToString());
                Thread.Sleep(100);
            }
        }
    }
}
