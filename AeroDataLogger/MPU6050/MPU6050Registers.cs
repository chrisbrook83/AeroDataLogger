namespace AeroDataLogger.MPU6050
{
    /// <summary>
    /// See: http://www.i2cdevlib.com/devices/mpu6050#registers
    /// Also: http://arduino.cc/playground/Main/MPU-6050
    /// </summary>
    public static class MPU6050Registers
    {
        public static byte I2C_ADDRESS = 0x69;      // AD0 = 1 (vs 0x68,1101000 for AD0 = 0) - pull up V setting

        public static byte GYRO_CONFIG = 0x1B;      // Read/Write
        
        public static byte ACCEL_CONFIG = 0x1C;     // Read/Write

        public static byte ACCEL_XOUT_H = 0x3B;     // Read only 
        public static byte ACCEL_XOUT_L = 0x3C;     // Read only 

        public static byte ACCEL_YOUT_H = 0x3D;     // Read only 
        public static byte ACCEL_YOUT_L = 0x3E;     // Read only 

        public static byte ACCEL_ZOUT_H = 0x3F;     // Read only 
        public static byte ACCEL_ZOUT_L = 0x40;     // Read only
        
        public static byte TEMP_OUT_H = 0x41;       // Read only 
        public static byte TEMP_OUT_L = 0x42;       // Read only 
        
        public static byte GYRO_XOUT_H = 0x43;      // Read only 
        public static byte GYRO_XOUT_L = 0x44;      // Read only 
        
        public static byte GYRO_YOUT_H = 0x45;      // Read only 
        public static byte GYRO_YOUT_L = 0x46;      // Read only 
        
        public static byte GYRO_ZOUT_H = 0x47;      // Read only 
        public static byte GYRO_ZOUT_L = 0x48;      // Read only 
        
        public static byte PWR_MGMT_1 = 0x6B;       // Read/Write
        public static byte PWR_MGMT_2 = 0x6C;       // Read/Write
        
        public static byte WHO_AM_I = 0x75;         // Read only
    }
}
