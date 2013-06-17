using System.Text;
using System;
namespace ExampleAccelGyroSensor.Sensor
{
    /// <summary>
    /// Objekt zum Auswerten der Ergebnisse und Bereitstellung der gemessenen Werten.
    /// </summary>
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

        /// <summary>
        /// Reconstructs a short (signed 16-bit int) from bytes representing the high and low 8-bit blocks.
        /// </summary>
        /// <param name="byte1">High bits</param>
        /// <param name="byte2">Low bits</param>
        private static short ToShort(byte byte1, byte byte2)
        {
            return (short)((byte1 << 8) | (byte2 << 0));
        }

        public override string ToString()
        {
            return "Temp / deg C: " + ParseTemp(RawTemperature) +
                " \tAcceleration: X: " + GetAccelString(RawAccelerationX) + "\tY: " + GetAccelString(RawAccelerationY) + "\tZ: " + GetAccelString(RawAccelerationZ) + 
                " \t Gyro: X: " + GetGyroRateString(RawGyroX) + "\tY: " + GetGyroRateString(RawGyroY) + "\tZ: " + GetGyroRateString(RawGyroZ);
        }

        private string GetAccelString(short value)
        {
            double range = 0;
            switch (_accelRange)
            {
                case AccelConfig.Range.plusMinus02G:
                    range = 2;
                    break;
                case AccelConfig.Range.plusMinus04G:
                    range = 4;
                    break;
                case AccelConfig.Range.plusMinus08G:
                    range = 8;
                    break;
                case AccelConfig.Range.plusMinus16G:
                    range = 16;
                    break;
            }

            double g = ((double)value * (range / (double)short.MaxValue));

            string sign = g < 0 ? "" : "+";
            return sign + g.ToString("f2"); // 2 d.p.
        }

        private string GetGyroRateString(short value)
        {
            double range = 0;
            switch (_gyroRange)
            {
                case GyroConfig.Range.plusMinus0250Dps:
                    range = 250;
                    break;
                case GyroConfig.Range.plusMinus0500dps:
                    range = 500;
                    break;
                case GyroConfig.Range.plusMinus1000dps:
                    range = 1000;
                    break;
                case GyroConfig.Range.plusMinus2000dps:
                    range = 2000;
                    break;
            }

            int dps = (int)((double)value * (range / (double)short.MaxValue));

            return PadString(dps, 4);
        }

        private string PadString(int value, int paddedLength)
        {
            StringBuilder sb = new StringBuilder();

            // Add sign
            if (value < 0) sb.Append("-"); else sb.Append("+");
            
            // Add padding zeros
            string s = Math.Abs(value).ToString();
            for (int i = 0; i < paddedLength - s.Length; i++)
            {
                sb.Append("0");
            }
            
            sb.Append(s);
            return sb.ToString();
        }

        /// <summary>
        /// Returns the value as a percentage value, to keep the number small
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetPercentString(short value)
        {
            double d = (double)value;
            int result = (int)System.Math.Round((d / short.MaxValue) * 100);

            return PadString(result, 3);
        }

        /// <summary>
        /// Converts the raw temperature value to real units.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int ParseTemp(short value)
        {
            int celsius = (value + 11796) / 524;
            return celsius;
        }
    }
}
