using System;
using Microsoft.SPOT;
using System.Text;

namespace AeroDataLogger.Sensors.AccelGyro
{
    internal static class AccelerationAndGyroDataBuilder
    {
        private static StringBuilder _sb = new StringBuilder();

        public static AccelerationAndGyroData Build(byte[] results, GyroConfig.Range gyroRange, AccelConfig.Range accelRange)
        {
            // Result for the acceleration sensor, merged together by bit shifting
            var RawAccelerationX = ToShort(results[0], results[1]);
            var RawAccelerationY = ToShort(results[2], results[3]);
            var RawAccelerationZ = ToShort(results[4], results[5]);
            
            // Results for temperature (not tested)
            var RawTemperature = ToShort(results[6], results[7]);

            // Result for the gyro sensor, merged together by bit shifting
            var RawGyroX = ToShort(results[8], results[9]);
            var RawGyroY = ToShort(results[10], results[11]);
            var RawGyroZ = ToShort(results[12], results[13]);

            // Now convert to sensible numbers
            var Temp = ParseTemp(RawTemperature);
            var Ax = ConvertRawValueToUnits(RawAccelerationX, (short)accelRange);
            var Ay = ConvertRawValueToUnits(RawAccelerationY, (short)accelRange);
            var Az = ConvertRawValueToUnits(RawAccelerationZ, (short)accelRange);
            var Rx = ConvertRawValueToUnits(RawGyroX, (short)gyroRange);
            var Ry = ConvertRawValueToUnits(RawGyroY, (short)gyroRange);
            var Rz = ConvertRawValueToUnits(RawGyroZ, (short)gyroRange);

            return new AccelerationAndGyroData(Ax, Ay, Az, Temp, Rx, Ry, Rz); 
        }

        public static string ConvertValueToString(double value, int integerDigits, int decimalPlaces)
        {
            _sb.Clear();

            // Add sign
            string sign = value < 0 ? "-" : "+";
            _sb.Append(sign);

            // Add padding zeros
            double absValue = System.Math.Abs(value);
            string integerPart = ((int)absValue).ToString();
            for (int i = 0; i < integerDigits - integerPart.Length; i++)
            {
                _sb.Append("0");
            }

            // Add value (to X dp)
            string format = "f" + decimalPlaces.ToString();
            _sb.Append(absValue.ToString(format));
            
            return _sb.ToString();
        }

        /// <summary>
        /// Reconstructs a short (signed 16-bit int) from bytes representing the high and low 8-bit blocks.
        /// </summary>
        /// <param name="byte1">High bits</param>
        /// <param name="byte2">Low bits</param>
        private static short ToShort(byte byte1, byte byte2)
        {
            return (short)((byte1 << 8) | (byte2 << 0));
        }
        
        /// <summary>
        /// Scales a raw data value to actual units.
        /// </summary>
        /// <param name="sensorValue">The raw data value (signed 16-bit integer: -32.7k to +32.7k)</param>
        /// <param name="maxValueInUnits">The maximum positive value on the scale.</param>
        /// <returns></returns>
        private static double ConvertRawValueToUnits(short sensorValue, short maxValueInUnits)
        {
            return sensorValue * ((double)maxValueInUnits / short.MaxValue);
        }

        /// <summary>
        /// Converts the raw temperature value to real units.
        /// </summary>
        /// <param name="value">The raw data value (signed 16-bit integer: -32.7k to +32.7k)</param>
        /// <returns>Temp in degrees C</returns>
        private static double ParseTemp(short value)
        {
            double celsius = (value + 11796.0) / 524.0;
            return celsius;
        }
    }
}
