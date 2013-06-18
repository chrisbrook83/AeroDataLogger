/*
 Developed from the code sample written by Johannes Paul Langner
 http://meineweltinmeinemkopf.blogspot.co.uk/2013/01/mpu6050-sensor-auslesen-mit-dem-netduino.html
 */

using System.Threading;
using AeroDataLogger.Sensors.AccelGyro;
using Microsoft.SPOT;
using AeroDataLogger.Sensors.Barometer;
using System;

namespace AeroDataLogger
{
    public class Program
    {
        public static void Main()
        {
            MS5611Baro baro = new MS5611Baro();
            while (true)
            {
                double temp;
                double pressure;
                baro.ReadTemperatureAndPressure(out temp, out pressure);
                Debug.Print("T: " + temp.ToString() + " P: " + pressure.ToString());
                Thread.Sleep(200);
            }
        }

        private static void RunGyro()
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
