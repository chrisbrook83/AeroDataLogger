using System.Threading;
using AeroDataLogger.I2C;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using AeroDataLogger.Logging;

namespace AeroDataLogger.Sensors.AccelGyro
{
    /// <summary>
    /// Class for interfacing with MPU-6050 module.
    /// References:
    ///   http://invensense.com/mems/gyro/documents/PS-MPU-6000A.pdf
    ///   http://www.i2cdevlib.com/devices/mpu6050#registers
    /// </summary>
    public class MPU6050Device
    {
        //private I2CConnector _I2C;
        private I2CBus _I2CBus = I2CBus.GetInstance();

        private readonly GyroConfig.Range _gyroRange;
        private readonly AccelConfig.Range _accelRange;
        private const int _clockRateKhz = 100;
        private const int _timeout = 1000;
        private readonly I2CDevice.Configuration _i2cConfig = new I2CDevice.Configuration(MPU6050Registers.I2C_ADDRESS, _clockRateKhz);
        
        public MPU6050Device()
        {
            Log.WriteLine("Initialising the MPU-6050 accelerometer and gyro package...");
            Thread.Sleep(100);

            // Check connectivity
            byte[] data = new byte[] { new byte() };
            _I2CBus.ReadRegister(_i2cConfig, MPU6050Registers.WHO_AM_I, data, 1000);
            Debug.Assert((data[0] & 0x68) == 0x68);

            // PWR_MGMT_1 = Power Management 1 
            // 0xF9 = 11111001: Device Reset=true, Sleep=true, Cycle=true, Temp Sensor=On, Clock Select=PLL with X axis gyroscope reference
            // TODO: Given the reset, half of these values are probably ignored, and the defaults are used instead.
            _I2CBus.WriteRegister(_i2cConfig, MPU6050Registers.PWR_MGMT_1, 0xF9, _timeout);
            Thread.Sleep(100); // allow to reset

            // Exit sleep mode (CARE! Not sure why, but this resets the device too! - only set config after this line)
            // TODO: Figure out what's going on here. Perhaps the reset at the line above was taking some time, hence why the Scale settings (below) were ignored
            _I2CBus.WriteRegister(_i2cConfig, MPU6050Registers.PWR_MGMT_1, 0x01, _timeout);
            Thread.Sleep(100); // allow to reset

            // Gyro Scale
            _gyroRange = GyroConfig.Range.plusMinus0500dps;
            _I2CBus.WriteRegister(_i2cConfig, MPU6050Registers.GYRO_CONFIG, GyroConfig.Build(_gyroRange), _timeout);

            // Accelerometer Scale
            _accelRange = AccelConfig.Range.plusMinus08G;
            _I2CBus.WriteRegister(_i2cConfig, MPU6050Registers.ACCEL_CONFIG, AccelConfig.Build(_accelRange), _timeout);

            // Confirm I2C_MST_EN is disabled (required to connect to the HMC5883L)
            data = new byte[] { new byte() };
            _I2CBus.ReadRegister(_i2cConfig, 0x6A, data, 1000);
            Debug.Assert((data[0] & 0x10) == 0);

            // Turn on I2C Bypass (required to connect to the HMC5883L)
            _I2CBus.WriteRegister(_i2cConfig, 0x37, 0x02, 1000);
            data = new byte[] { new byte() };
            _I2CBus.ReadRegister(_i2cConfig, 0x37, data, 1000);
            Debug.Assert((data[0] & 0x02) == 0x02);

            Log.WriteLine("MPU-6050 Ready\n");
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
            //_I2C.Write(new byte[] { MPU6050Registers.ACCEL_XOUT_H });
            _I2CBus.Write(_i2cConfig, new byte[] { MPU6050Registers.ACCEL_XOUT_H }, _timeout);
            //_I2C.Read(registerList);
            _I2CBus.Read(_i2cConfig, registerList, _timeout);

            var result = AccelerationAndGyroDataBuilder.Build(registerList, _gyroRange, _accelRange);
            return result;
        }
    }
}
