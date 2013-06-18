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
            MPU6050Device mpu6050 = new MPU6050Device();
            MS5611Baro baro = new MS5611Baro();

            AccelerationAndGyroData sensorResult = new AccelerationAndGyroData();
            double temp = 0;
            double pressure = 0;
            
            while (true)
            {        
                sensorResult = mpu6050.GetSensorData();
                baro.ReadTemperatureAndPressure(out temp, out pressure);

                Debug.Print(sensorResult.ToString() + "\tT: " + temp.ToString("f2") + "\tP: " + pressure.ToString("f2"));
                
                Thread.Sleep(200);
            }
        }
    }
}
