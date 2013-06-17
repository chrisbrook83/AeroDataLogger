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
        public short Acceleration_X;

        /// <summary>
        /// Y-axis acceleration
        /// </summary>
        public short Acceleration_Y;
        
        /// <summary>
        /// Z -axis acceleration
        /// </summary>
        public short Acceleration_Z;
        
        /// <summary>
        /// Temperature
        /// </summary>
        public short Temperature;
        
        /// <summary>
        /// X-axis angular rate
        /// </summary>
        public short Gyro_X;
        
        /// <summary>
        /// Y-axis angular rate
        /// </summary>
        public short Gyro_Y;
        
        /// <summary>
        /// Z-axis angular rate
        /// </summary>
        public short Gyro_Z;

        public ushort Accel_Config;

        public ushort Gyro_Config;

        private GyroConfig.Range _gyroRange;
        private AccelConfig.Range _accelRange;

        /// <summary>
        /// Constructs the structure from a byte array of raw data.
        /// </summary>
        /// <param name="results"></param>
        public AccelerationAndGyroData(byte[] results, GyroConfig.Range gyroRange, AccelConfig.Range accelRange)
        {
            _gyroRange = gyroRange;
            _accelRange = accelRange;

            // Result for the acceleration sensor, merged together by bit shifting
            Acceleration_X = ToShort(results[0], results[1]);
            Acceleration_Y = ToShort(results[2], results[3]);
            Acceleration_Z = ToShort(results[4], results[5]);
            
            // Results for temperature (not tested)
            Temperature = ToShort(results[6], results[7]);

            // Result for the gyro sensor, merged together by bit shifting
            Gyro_X = ToShort(results[8], results[9]);
            Gyro_Y = ToShort(results[10], results[11]);
            Gyro_Z = ToShort(results[12], results[13]);

            // debug:
            Gyro_Config = new byte(); // results[14];
            Accel_Config = new byte(); // results[15];           
        }

        static short ToShort(byte byte1, byte byte2)
        {
            return (short)((byte1 << 8) | (byte2 << 0));
        }

        /// <summary>
        /// Überschreibe die ToString() Methode
        /// Gibt alle in Werte in einem String zurück
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Temp / deg C: " + ParseTemp(Temperature) +
                " \tAcceleration: X: " + GetAccelString(Acceleration_X) + "\tY:" + GetAccelString(Acceleration_Y) + "\tZ:" + GetAccelString(Acceleration_Z) + 
                " \t Gyro: X: " + GetGyroRateString(Gyro_X) + "\tY: " + GetGyroRateString(Gyro_Y) + "\tZ: " + GetGyroRateString(Gyro_Z) 
                + "\tGyro: " + ((int)Gyro_Config).ToString() + "\tAccel: " + ((int)Accel_Config).ToString();
        }

        private string GetAccelString(short value)
        {
            return value.ToString();
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

            double fudgeFactor = (2 / range); // why?! (determined through observation)
            double g = ((double)value * (range / (double)short.MaxValue) * fudgeFactor);

            return g.ToString("f2"); // 2 d.p.
        }

        private string GetGyroRateString(short value)
        {
            return value.ToString();
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

            double fudgeFactor = (250 / range); // why?! (determined through observation)
            int dps = (int)((double)value * (range / (double)short.MaxValue) * fudgeFactor);

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


        private int ParseTemp(short value)
        {
            int celsius = (value + 11796) / 524;
            return celsius;
        }
    }
}
