using System;
using System.Text;
namespace AeroDataLogger.Sensors.AccelGyro
{
    public struct AccelerationAndGyroData
    {
        /// <summary>
        /// X-axis acceleration
        /// </summary>
        public short RawAccelerationX;

        /// <summary>
        /// Y-axis acceleration
        /// </summary>
        public short RawAccelerationY;
        
        /// <summary>
        /// Z -axis acceleration
        /// </summary>
        public short RawAccelerationZ;
        
        /// <summary>
        /// Temperature
        /// </summary>
        public short RawTemperature;
        
        /// <summary>
        /// X-axis angular rate
        /// </summary>
        public short RawGyroX;
        
        /// <summary>
        /// Y-axis angular rate
        /// </summary>
        public short RawGyroY;
        
        /// <summary>
        /// Z-axis angular rate
        /// </summary>
        public short RawGyroZ;

        private GyroConfig.Range _gyroRange;

        private AccelConfig.Range _accelRange;

        /// <summary>
        /// Constructs the structure from a byte array of raw data.
        /// </summary>
        /// <param name="results"></param>
        public AccelerationAndGyroData(byte[] results, GyroConfig.Range gyroRange, AccelConfig.Range accelRange)
        {
            // Store these so we can scale the raw numbers to actual units.
            _gyroRange = gyroRange;
            _accelRange = accelRange;

            // Result for the acceleration sensor, merged together by bit shifting
            RawAccelerationX = ToShort(results[0], results[1]);
            RawAccelerationY = ToShort(results[2], results[3]);
            RawAccelerationZ = ToShort(results[4], results[5]);
            
            // Results for temperature (not tested)
            RawTemperature = ToShort(results[6], results[7]);

            // Result for the gyro sensor, merged together by bit shifting
            RawGyroX = ToShort(results[8], results[9]);
            RawGyroY = ToShort(results[10], results[11]);
            RawGyroZ = ToShort(results[12], results[13]);         
        }

        public override string ToString()
        {
            var temp = ConvertValueToString(ParseTemp(RawTemperature), 2, 1);
            var aX = ConvertValueToString(ConvertRawValueToUnits(RawAccelerationX, (short)_accelRange), 1, 3);
            var aY = ConvertValueToString(ConvertRawValueToUnits(RawAccelerationY, (short)_accelRange), 1, 3);
            var aZ = ConvertValueToString(ConvertRawValueToUnits(RawAccelerationZ, (short)_accelRange), 1, 3);
            var rX = ConvertValueToString(ConvertRawValueToUnits(RawGyroX, (short)_gyroRange), 3, 1);
            var rY = ConvertValueToString(ConvertRawValueToUnits(RawGyroY, (short)_gyroRange), 3, 1);
            var rZ = ConvertValueToString(ConvertRawValueToUnits(RawGyroZ, (short)_gyroRange), 3, 1);

            return "Temp: " + temp + "°C" + 
                "\tAccel: " + aX + "X\t " + aY + "Y\t " + aZ + "Z" +
                "\tGyro: " + rX + "X\t " + rY + "Y\t " + rZ + "Z";
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

        private static string ConvertValueToString(double value, int integerDigits, int decimalPlaces)
        {
            StringBuilder sb = new StringBuilder();

            // Add sign
            string sign = value < 0 ? "-" : "+";
            sb.Append(sign);

            // Add padding zeros
            double absValue = Math.Abs(value);
            string integerPart = ((int)absValue).ToString();
            for (int i = 0; i < integerDigits - integerPart.Length; i++)
            {
                sb.Append("0");
            }
            
            // Add value (to X dp)
            string format = "f" + decimalPlaces.ToString();
            sb.Append(absValue.ToString(format));

            return sb.ToString();
        }
    }
}
