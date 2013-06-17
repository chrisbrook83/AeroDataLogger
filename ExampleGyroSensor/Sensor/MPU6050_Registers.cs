
namespace ExampleAccelGyroSensor.Sensor
{
    /// <summary>
    /// Adressen alle abgeleitet vom Beispiel Sketch.
    /// Siehe Seite http://arduino.cc/playground/Main/MPU-6050
    /// 
    /// See also: 
    /// http://www.i2cdevlib.com/devices/mpu6050#registers
    /// </summary>
    public static class MPU6050_Registers
    {
        /// <summary>
        /// MPU-6050 I2C Address.
        /// </summary>
        public static byte I2C_ADDRESS = 0x69; // AD0 = 1 (vs 1101000 for AD0 = 0)
        
        /// <summary>
        /// Address for the configuration of the gyro
        /// </summary>
        public static byte GYRO_CONFIG = 0x1B;    // Read/Write
        
        /// <summary>
        /// Address for the configuration of the acceleration sensor
        /// </summary>
        public static byte ACCEL_CONFIG = 0x1C;   // Read/Write
        
        /// <summary>
        /// Address for the X axis of the acceleration sensor part 1
        /// </summary>
        public static byte ACCEL_XOUT_H = 0x3B;       // Read only 
        
        /// <summary>
        /// Address for the X axis of the acceleration sensor part 2
        /// </summary>
        public static byte ACCEL_XOUT_L = 0x3C;       // Read only 
        
        /// <summary>
        /// Address for the Y axis of the acceleration sensor part 1
        /// </summary>
        public static byte ACCEL_YOUT_H = 0x3D;       // Read only 
        
        /// <summary>
        /// Address for the Y axis of the acceleration sensor part 2
        /// </summary>
        public static byte ACCEL_YOUT_L = 0x3E;       // Read only 
        
        /// <summary>
        /// Address for the Z axis of the acceleration sensor part 1
        /// </summary>
        public static byte ACCEL_ZOUT_H = 0x3F;       // Read only 
        
        /// <summary>
        /// Address for the Z axis of the acceleration sensor part 2
        /// </summary>
        public static byte ACCEL_ZOUT_L = 0x40;       // Read only
        
        /// <summary>
        /// Address for the temperature Part 1 (not yet tried)
        /// </summary>
        public static byte TEMP_OUT_H = 0x41;         // Read only 
        
        /// <summary>
        /// Address for the temperature Part 2 (not yet tried)
        /// </summary>
        public static byte TEMP_OUT_L = 0x42;         // Read only 
        
        /// <summary>
        /// Address for the X axis of the gyroscope Part 1
        /// </summary>
        public static byte GYRO_XOUT_H = 0x43;        // Read only 
        
        /// <summary>
        /// Address for the X axis of the gyroscope Part 2
        /// </summary>
        public static byte GYRO_XOUT_L = 0x44;        // Read only 
        
        /// <summary>
        /// Address for the Y axis of the gyroscope Part 1
        /// </summary>
        public static byte GYRO_YOUT_H = 0x45;        // Read only 
        
        /// <summary>
        /// Address for the Y axis of the gyroscope Part 2
        /// </summary>
        public static byte GYRO_YOUT_L = 0x46;        // Read only 
        
        /// <summary>
        /// Address for the Z axis of the gyroscope Part 1
        /// </summary>
        public static byte GYRO_ZOUT_H = 0x47;        // Read only 
        
        /// <summary>
        /// Address for the Z axis of the gyroscope Part 2
        /// </summary>
        public static byte GYRO_ZOUT_L = 0x48;        // Read only 
        
        /// <summary>
        /// Adresse für Power Management 1
        /// Ermöglicht die Einstellungen für den Power Modus und die Taktquelle zu bestimmen.
        /// </summary>
        public static byte PWR_MGMT_1 = 0x6B;         // Read/Write
        
        /// <summary>
        /// Adresse für Power Management 2
        /// Weitere Einstellungen.
        /// </summary>
        public static byte PWR_MGMT_2 = 0x6C;         // Read/Write
        
        /// <summary>
        /// Adresse für Eigene Identität bzw. Adresse prüfen.
        /// </summary>
        public static byte WHO_AM_I = 0x75;           // Read only
    }
}
