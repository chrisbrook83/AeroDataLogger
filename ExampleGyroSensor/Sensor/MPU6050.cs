using Microsoft.SPOT;
using ExampleAccelGyroSensor.I2C_Hardware;

namespace ExampleAccelGyroSensor.Sensor
{
    public class MPU6050
    {
        /// <summary>
        /// Klassen für die Verbindung über den I²C Bus
        /// </summary>
        private I2C_Connector _I2C;

        private GyroConfig.Range _gyroRange;
        private AccelConfig.Range _accelRange;
        
        /// <summary>
        /// Klasse mit dem Entsprechendn Adressen Initialisieren
        /// </summary>
        public MPU6050()
        {
            Debug.Print("Initialising the MPU-6050 Accelerometer and Gyro package...");
            int clockRateHz = 100;
            _I2C = new I2C_Connector(MPU6050_Registers.I2C_ADDRESS, clockRateHz);

            Initialize();

            // Test connectivity
            Debug.Print("Testing connectivity:");
            byte testResult = 0xFF;
            //CheckErrorStatus(_I2C.Read(MPU6050_Registers.WHO_AM_I, testResult));
            CheckErrorStatus(_I2C.Read(new byte(), new byte[] { MPU6050_Registers.WHO_AM_I }));
            Debug.Print("-----------------------------------------------------------------------");
        }
        
        /// <summary>
        /// Initialisiert den Sensor mit der Standard Adresse
        /// </summary>
        public void Initialize()
        {
            // See: http://www.i2cdevlib.com/devices/mpu6050#registers

            // PWR_MGMT_1 = Power Management 1 (11111001)
            // 0xF9 = Device Reset (1), Sleep (1), Cycle (1), nichts (1), Temperatur (1), Taktquelle festlegen (001)
            CheckErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.PWR_MGMT_1, 0xF9 }));
            
            // FullScaleGyroRange
            _gyroRange = GyroConfig.Range.plusMinus2000dps;
            CheckErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.GYRO_CONFIG, GyroConfig.Build(_gyroRange) })); 
            
            // FullScaleAccelRange
            _accelRange = AccelConfig.Range.plusMinus16G;
            CheckErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.ACCEL_CONFIG, AccelConfig.Build(_accelRange) }));
            
            // Schlafmodus beenden
            CheckErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.PWR_MGMT_1, 0x01 }));
        }

        /// <summary>
        /// Gibt ein Error Status über Debug aus
        /// </summary>
        /// <param name="error"></param>
        private void CheckErrorStatus(object error)
        {
            if (error != null)
            {
                if ((int)error == 0) 
                {
                    Debug.Print("Status: Error"); 
                }
                else 
                {
                    Debug.Print("Status: OK"); 
                }
            }
        }
        
        /// <summary>
        /// Ruft die Daten aus dem Sensor ab
        /// </summary>
        public AccelerationAndGyroData GetSensorData()
        {
            /*byte[] registerList = new byte[16];

            _I2C.Read(MPU6050_Registers.ACCEL_XOUT_H, registerList[0]);
            _I2C.Read(MPU6050_Registers.ACCEL_XOUT_L, registerList[1]);
            _I2C.Read(MPU6050_Registers.ACCEL_YOUT_H, registerList[2]);
            _I2C.Read(MPU6050_Registers.ACCEL_YOUT_L, registerList[3]);
            _I2C.Read(MPU6050_Registers.ACCEL_ZOUT_H, registerList[4]);
            _I2C.Read(MPU6050_Registers.ACCEL_ZOUT_L, registerList[5]);
            _I2C.Read(MPU6050_Registers.TEMP_OUT_H,   registerList[6]);
            _I2C.Read(MPU6050_Registers.TEMP_OUT_L,   registerList[7]);
            _I2C.Read(MPU6050_Registers.GYRO_XOUT_H,  registerList[8]);
            _I2C.Read(MPU6050_Registers.GYRO_XOUT_L,  registerList[9]);
            _I2C.Read(MPU6050_Registers.GYRO_YOUT_H,  registerList[10]);
            _I2C.Read(MPU6050_Registers.GYRO_YOUT_L,  registerList[11]);
            _I2C.Read(MPU6050_Registers.GYRO_ZOUT_H,  registerList[12]);
            _I2C.Read(MPU6050_Registers.GYRO_ZOUT_L,  registerList[13]);
            _I2C.Read(MPU6050_Registers.GYRO_CONFIG,  registerList[14]);
            _I2C.Read(MPU6050_Registers.ACCEL_CONFIG, registerList[15]);
            */
            byte[] registerList = new byte[14];

            registerList[0] = MPU6050_Registers.ACCEL_XOUT_H;
            registerList[1] = MPU6050_Registers.ACCEL_XOUT_L;
            registerList[2] = MPU6050_Registers.ACCEL_YOUT_H;
            registerList[3] = MPU6050_Registers.ACCEL_YOUT_L;
            registerList[4] = MPU6050_Registers.ACCEL_ZOUT_H;
            registerList[5] = MPU6050_Registers.ACCEL_ZOUT_L;
            registerList[6] = MPU6050_Registers.TEMP_OUT_H;
            registerList[7] = MPU6050_Registers.TEMP_OUT_L;
            registerList[8] = MPU6050_Registers.GYRO_XOUT_H;
            registerList[9] = MPU6050_Registers.GYRO_XOUT_L;
            registerList[10] = MPU6050_Registers.GYRO_YOUT_H;
            registerList[11] = MPU6050_Registers.GYRO_YOUT_L;
            registerList[12] = MPU6050_Registers.GYRO_ZOUT_H;
            registerList[13] = MPU6050_Registers.GYRO_ZOUT_L;

            _I2C.Write(new byte[] { MPU6050_Registers.ACCEL_XOUT_H });
            _I2C.Read(new byte(), registerList);

            return new AccelerationAndGyroData(registerList, _gyroRange, _accelRange);
        }
    }
}
