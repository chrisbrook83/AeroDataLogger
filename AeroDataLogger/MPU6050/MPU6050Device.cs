using AeroDataLogger.I2C;
using Microsoft.SPOT;

namespace AeroDataLogger.MPU6050
{
    /// <summary>
    /// Class for interfacing with MPU-6050 module.
    /// References:
    ///   http://invensense.com/mems/gyro/documents/PS-MPU-6000A.pdf
    ///   http://www.i2cdevlib.com/devices/mpu6050#registers
    /// </summary>
    public class MPU6050Device
    {
        private I2CConnector _I2C;

        private GyroConfig.Range _gyroRange;
        private AccelConfig.Range _accelRange;
        
        public MPU6050Device()
        {
            Debug.Print("Initialising the MPU-6050 Accelerometer and Gyro package...");
            const int clockRateKhz = 100;
            _I2C = new I2CConnector(MPU6050Registers.I2C_ADDRESS, clockRateKhz);

            // PWR_MGMT_1 = Power Management 1 
            // 0xF9 = 11111001: Device Reset=true, Sleep=true, Cycle=true, Temp Sensor=On, Clock Select=PLL with X axis gyroscope reference
            // TODO: Given the reset, half of these values are probably ignored, and the defaults are used instead.
            _I2C.Write(new byte[] { MPU6050Registers.PWR_MGMT_1, 0xF9 });

            // Exit sleep mode (CARE! Not sure why, but this resets the device too! - only set config after this line)
            // TODO: Figure out what's going on here. Perhaps the reset at the line above was taking some time, hence why the Scale settings (below) were ignored
            _I2C.Write(new byte[] { MPU6050Registers.PWR_MGMT_1, 0x01 });

            // Gyro Scale
            _gyroRange = GyroConfig.Range.plusMinus0500dps;
            _I2C.Write(new byte[] { MPU6050Registers.GYRO_CONFIG, GyroConfig.Build(_gyroRange) });

            // Accelerometer Scale
            _accelRange = AccelConfig.Range.plusMinus08G;
            _I2C.Write(new byte[] { MPU6050Registers.ACCEL_CONFIG, AccelConfig.Build(_accelRange) });

            Debug.Print("Done");
        }

        /// <summary>
        /// Read the sensor values from the relevant registers and parse.
        /// </summary>
        public AccelerationAndGyroData GetSensorData()
        {
            byte[] registerList = new byte[]
            {
                MPU6050Registers.ACCEL_XOUT_H,
                MPU6050Registers.ACCEL_XOUT_L,
                MPU6050Registers.ACCEL_YOUT_H,
                MPU6050Registers.ACCEL_YOUT_L,
                MPU6050Registers.ACCEL_ZOUT_H,
                MPU6050Registers.ACCEL_ZOUT_L,
                MPU6050Registers.TEMP_OUT_H,
                MPU6050Registers.TEMP_OUT_L,
                MPU6050Registers.GYRO_XOUT_H,
                MPU6050Registers.GYRO_XOUT_L,
                MPU6050Registers.GYRO_YOUT_H,
                MPU6050Registers.GYRO_YOUT_L,
                MPU6050Registers.GYRO_ZOUT_H,
                MPU6050Registers.GYRO_ZOUT_L
            };

            // Funky - you need to do this, otherwise you won't get anything back. See MPU-6050 datasheet for details.
            _I2C.Write(new byte[] { MPU6050Registers.ACCEL_XOUT_H });
            _I2C.Read(registerList);

            return new AccelerationAndGyroData(registerList, _gyroRange, _accelRange);
        }
    }
}