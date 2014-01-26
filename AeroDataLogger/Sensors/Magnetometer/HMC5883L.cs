using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using AeroDataLogger.I2C;
using System.Threading;
using AeroDataLogger.Output;

namespace AeroDataLogger.Sensors.Magnetometer
{
    /// <summary>
    /// Code from : http://forums.netduino.com/index.php?/topic/2545-hmc5883l-magnetometer-netduino-code/
    /// Datasheet : http://dlnmh9ip6v2uc.cloudfront.net/datasheets/Sensors/Magneto/HMC5883L-FDS.pdf
    /// Datasheet : http://www51.honeywell.com/aero/common/documents/myaerospacecatalog-documents/Defense_Brochures-documents/HMC5883L_3-Axis_Digital_Compass_IC.pdf
    /// More Info : http://www.sparkfun.com/products/10494
    /// </summary>
    public class HMC5883L
    {
        private const byte HMC5883L_I2C_ADDRESS = 0x1E;
        private const byte ScaleRegister = 0x01;
        private const byte MeasurementRateRegister = 0x02;
        private const byte DataValue_StartingRegister = 0x03;
        private const byte IdentificationRegister_1 = 0x0A;
        private const byte IdentificationRegister_2 = 0x0B;
        private const byte IdentificationRegister_3 = 0x0C;

        private const byte IDENTIFICATION_REGISTER_A_VALUE = 0x48;
        private const byte IDENTIFICATION_REGISTER_B_VALUE = 0x34;
        private const byte IDENTIFICATION_REGISTER_C_VALUE = 0x33;
        private const byte CONTINUOUS_MEASUREMENT = 0x00;

        private const int I2C_TIMEOUT = 1000;
        private const int I2C_CLOCK = 100; // kHz

        private readonly I2CBus _i2cBus = I2CBus.GetInstance();
        private readonly I2CDevice.Configuration _i2cConfig = new I2CDevice.Configuration(HMC5883L_I2C_ADDRESS, I2C_CLOCK);

        public static double Scale { get; set; }

        public HMC5883L()
        {
            // WARNING: Because this chip is connected as a slave of the MPU-6050, that device 
            // must be configured properly for this device to be visible on the I2C bus.
            Log.WriteLine("Initialising the HMC5883L magnetometer...");
            Thread.Sleep(100);
            this.IsConnected();
            this.SetScale(1.3);
            this.SetContinuous();
            Log.WriteLine("HMC5883L Ready\n");
        }

        public bool IsConnected()
        {
            if (DeviceIdentifier()[0] != IDENTIFICATION_REGISTER_A_VALUE
                || DeviceIdentifier()[1] != IDENTIFICATION_REGISTER_B_VALUE
                || DeviceIdentifier()[2] != IDENTIFICATION_REGISTER_C_VALUE)
            {
                throw new Exception("Did not return Device ID 0x48/0x34/0x33 as expected.");
            }

            return true;
        }

        public byte[] DeviceIdentifier()
        {
            byte[] r1 = new byte[1];
            byte[] r2 = new byte[1]; 
            byte[] r3 = new byte[1];
            _i2cBus.ReadRegister(_i2cConfig, IdentificationRegister_1, r1, I2C_TIMEOUT);
            _i2cBus.ReadRegister(_i2cConfig, IdentificationRegister_2, r2, I2C_TIMEOUT);
            _i2cBus.ReadRegister(_i2cConfig, IdentificationRegister_3, r3, I2C_TIMEOUT);
            return new byte[] { r1[0], r2[0], r3[0] };
        }

        public void SetContinuous()
        {
            _i2cBus.WriteRegister(_i2cConfig, MeasurementRateRegister, CONTINUOUS_MEASUREMENT, I2C_TIMEOUT);
        }

        public void SetScale(double gauss)
        {
            byte regValue = 0x00;

            if (gauss == 0.88)
            {
                regValue = 0x00;
                Scale = 0.73;
            }
            else if (gauss == 1.3)
            {
                regValue = 0x01;
                Scale = 0.92;
            }
            else if (gauss == 1.9)
            {
                regValue = 0x02;
                Scale = 1.22;
            }
            else if (gauss == 2.5)
            {
                regValue = 0x03;
                Scale = 1.52;
            }
            else if (gauss == 4.0)
            {
                regValue = 0x04;
                Scale = 2.27;
            }
            else if (gauss == 4.7)
            {
                regValue = 0x05;
                Scale = 2.56;
            }
            else if (gauss == 5.6)
            {
                regValue = 0x06;
                Scale = 3.03;
            }
            else if (gauss == 8.1)
            {
                regValue = 0x07;
                Scale = 4.35;
            }
            else
            {
                throw new ArgumentException("Unknown gauss value");
            }

            regValue = (byte)(((int)regValue) << 5);

            _i2cBus.WriteRegister(_i2cConfig, ScaleRegister, regValue, I2C_TIMEOUT);
        }

        public RawData Raw
        {
            get
            {
                var r = new RawData();

                byte[] bytes = new byte[6];
                _i2cBus.ReadRegister(_i2cConfig, DataValue_StartingRegister, bytes, I2C_TIMEOUT);

                short xReading = (short)((bytes[0] << 8) | bytes[1]);
                short zReading = (short)((bytes[2] << 8) | bytes[3]);
                short yReading = (short)((bytes[4] << 8) | bytes[5]);
                
                r.X = xReading;
                r.Y = yReading;
                r.Z = zReading;

                return r;
            }
        }

        public ScaledData ScaledData
        {
            get
            {
                var s = new ScaledData();
                s.ScaledX = Scale * (double)this.Raw.X;
                s.ScaledY = Scale * (double)this.Raw.Y;
                s.ScaledZ = Scale * (double)this.Raw.Z;

                return s;
            }
        }

        public double GetHeading()
        {
            // TODO: not tilt compensated. Co-ordinate system may also be misaligned (sensor upside down)
            var raw = this.Raw;
            double heading = System.Math.Atan(((double)raw.Y) / ((double)raw.X)) * (360 / (2 * System.Math.PI));
            return heading;
        }
    }

    public struct RawData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    public struct ScaledData
    {
        public double ScaledX { get; set; }
        public double ScaledY { get; set; }
        public double ScaledZ { get; set; }
    }
}

