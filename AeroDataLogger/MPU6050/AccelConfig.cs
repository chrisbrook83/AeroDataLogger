namespace AeroDataLogger.MPU6050
{
    public class AccelConfig
    {
        public enum Range : short
        {
            plusMinus02G = 2,
            plusMinus04G = 4,
            plusMinus08G = 8,
            plusMinus16G = 16
        }

        public static byte Build(Range Sensitivity)
        {
            // Structure:
            // Bit 7 = XA_ST: Setting this bit causes the X axis accelerometer to perform self test
            // Bit 6 = YA_ST: Setting this bit causes the Y axis accelerometer to perform self test
            // Bit 5 = ZA_ST: Setting this bit causes the Z axis accelerometer to perform self test
            // Bit 4:3 = AFS_SEL: 2-bit unsigned value. Selects the full scale range of accelerometer. 
            //  Options:
            //  0 = ± 2.0G
            //  1 = ± 4.0G
            //  2 = ± 8.0G
            //  3 = ± 16.0G
            // Bits 2:0 = ACCEL_HPF: 3-bit unsigned value. High pass filter settings.
            //  Options: 
            //  0 = Reset
            //  1 = On @ 5 Hz
            //  2 = On @ 2.5 Hz
            //  3 = On @ 1.25 Hz
            //  4 = On @ 0.63 Hz
            //  7 = Hold

            byte bits = 0x01; // 00000001 (no tests, option 0, HPF 5hz) 
            byte mask = 0x00;

            switch (Sensitivity)
            {
                case Range.plusMinus02G:
                    mask = 0x00; // 00000000
                    break;
                case Range.plusMinus04G:
                    mask = 0x08; // 00001000
                    break;
                case Range.plusMinus08G:
                    mask = 0x10; // 00010000
                    break;
                case Range.plusMinus16G:
                    mask = 0x18; // 00011000
                    break;
            }

            bits |= mask;
            return bits;
        }
    }
}
