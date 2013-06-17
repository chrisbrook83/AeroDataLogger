using System;
using Microsoft.SPOT;

namespace ExampleAccelGyroSensor.Sensor
{
    public class GyroConfig
    {
        public enum Range : short
        {
            plusMinus0250Dps = 0,
            plusMinus0500dps = 1,
            plusMinus1000dps = 2,
            plusMinus2000dps = 3
        }

        public static byte Build(Range Sensitivity)
        {
            // Structure:
            // Bit 7 = XG_ST: Setting this bit causes the X axis gyroscope to perform self test
            // Bit 6 = YG_ST: Setting this bit causes the Y axis gyroscope to perform self test
            // Bit 5 = ZG_ST: Setting this bit causes the Z axis gyroscope to perform self test
            // Bit 4:3 = FS_SE: 2-bit unsigned value. Selects the full scale range of gyroscopes. 
            //  Options:
            //  0 = ± 250 °/s
            //  1 = ± 500 °/s
            //  2 = ± 1000 °/s
            //  3 = ± 2000 °/s
            // Bits 2:0 = Not used

            byte bits = 0x00; // 00000000 (no tests, option 0, set unused bits to false)
            byte mask = 0x00;

            switch (Sensitivity)
            {
                case Range.plusMinus0250Dps:
                    mask = 0x00; // 00000000
                    break;
                case Range.plusMinus0500dps:
                    mask = 0x08; // 00001000
                    break;
                case Range.plusMinus1000dps:
                    mask = 0x10; // 00010000
                    break;
                case Range.plusMinus2000dps:
                    mask = 0x18; // 00011000
                    break;
            }

            bits |= mask;
            return bits;
        }
    }
}
