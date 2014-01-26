using System;
using Microsoft.SPOT;
using AeroDataLogger.I2C;
using System.Threading;
using Microsoft.SPOT.Hardware;
using AeroDataLogger.Output;

namespace AeroDataLogger.Sensors.Barometer
{
    /// <summary>
    /// http://www.meas-spec.com/downloads/MS5611-01BA03.pdf
    /// </summary>
    public class MS5611Baro
    {
        private const byte MS5611_I2C_ADDRESS = 0x77;
        private const int I2C_TIMEOUT = 1000;
        private const int I2C_CLOCK = 100; // kHz

        private readonly I2CBus _i2cBus = I2CBus.GetInstance();
        private readonly I2CDevice.Configuration _i2cConfig = new I2CDevice.Configuration(MS5611_I2C_ADDRESS, I2C_CLOCK);
         
        private CalibrationData _calibrationData;

        public MS5611Baro()
        {
            Log.WriteLine("Initialising the MS5611 barometric sensor...");
            Initialise();
            Log.WriteLine("MS5611 Ready\n");
        }

        /// <summary>
        /// Converts the raw register values to the correct units.
        /// The variable names are consistent with those used in the datasheet (see page 7).
        /// </summary>
        public void ReadTemperatureAndPressure(out double temp, out double pressure)
        {
            UInt32 D1 = ReadRawPressure();
            UInt32 D2 = ReadRawTemperature();
            
            // Difference between current temperature, and the temperature used during factory calibration
            Int32 dT = (Int32)D2 - ((Int32)_calibrationData.C5_TRef << 8);
            Debug.Assert(dT >= -16776960 && dT <= 16776960);

            // Calculate temperature from raw value using correction coefficient C6 (20deg reference)
            Int64 TEMP = 2000 + ((dT * _calibrationData.C6_TempSens) >> 23);
            Debug.Assert(TEMP >= -4000 && TEMP <= 8500);

            // Correct factory pressure offset (OffT1) using current temperature
            Int64 OFF = ((Int64)_calibrationData.C2_OffT1 << 16) + (((Int64)dT * _calibrationData.C4_TCO) >> 7);
            Debug.Assert(OFF >= -8589672450 && OFF <= 12884705280);

            // Correct factory pressure sensor sensitivity (SensT1) using current temperature
            Int64 SENS = ((Int64)_calibrationData.C1_SensT1 << 15) + (((Int64)dT * _calibrationData.C3_TCS) >> 8);
            Debug.Assert(SENS >= -4294836225 && SENS <= 6442352640);

            // Higher order temperature correction
            if (TEMP < 2000) // if temperature lower than +20 Celsius...
            {
                Int32 T1 = 0;
                Int64 OFF1 = 0;
                Int64 SENS1 = 0;

                T1 = (Int32)System.Math.Pow(dT, 2) >> 31;
                OFF1 = (Int64)(5 * System.Math.Pow((TEMP - 2000), 2) / 2);
                SENS1 = (Int64)(5 * System.Math.Pow((TEMP - 2000), 2) / 4);

                if (TEMP < -1500) // if temperature lower than -15 Celsius...
                {
                    OFF1 = (Int64)(OFF1 + 7 * System.Math.Pow((TEMP + 1500), 2));
                    SENS1 = (Int64)(SENS1 + 11 * System.Math.Pow((TEMP + 1500), 2) / 2);
                }

                TEMP -= T1;
                OFF -= OFF1;
                SENS -= SENS1;
            }

            // Calculate pressure
            Int32 P = (Int32)((((D1 * SENS) >> 21) - OFF) >> 15);
            Debug.Assert(1000 <= P && P <= 120000);

            temp = (double)TEMP / 100;
            pressure = (double)P / 100;
        }

        private void Initialise()
        {
            Reset();
            ReadCalibrationData();
        }

        private void Reset()
        {
            const byte RESET_COMMAND = 0x1E;
            _i2cBus.Write(_i2cConfig, new byte[] { RESET_COMMAND }, I2C_TIMEOUT);
            Thread.Sleep(100); // allow reset to complete
        }

        private void ReadCalibrationData()
        {
            const byte READ_PROM = 0xA6;
            byte[] commands = new byte[]
            {
                0xA0,
                0xA2,
                0xA4,
                0xA6,
                0xA8,
                0xAA,
                0xAC,
                0xAE
            };

            byte[] results = new byte[16];

            for (int i = 0; i < commands.Length; i++)
            {
                //_i2cBus.Write(_i2cConfig, new byte[] { commands[i] }, I2C_TIMEOUT);
                byte[] temp = new byte[2];
                //_i2cBus.Read(_i2cConfig, temp, I2C_TIMEOUT);

                _i2cBus.ReadRegister(_i2cConfig, commands[i], temp, I2C_TIMEOUT);

                Array.Copy(temp, 0, results, 2 * i, 2);
            }

            _calibrationData = new CalibrationData(results);
        }

        private UInt32 ReadRawTemperature()
        {
            // Uncorrected Temperature (D2)
            const byte CONV_SEQ_COMMAND_D2_OSR256 = 0x50;
            const byte CONV_SEQ_COMMAND_D2_OSR512 = 0x52;
            const byte CONV_SEQ_COMMAND_D2_OSR1024 = 0x54;
            const byte CONV_SEQ_COMMAND_D2_OSR2048 = 0x56;
            const byte CONV_SEQ_COMMAND_D2_OSR4096 = 0x58;

            _i2cBus.Write(_i2cConfig, new byte[] { CONV_SEQ_COMMAND_D2_OSR4096 }, I2C_TIMEOUT);
            Thread.Sleep(15); // wait for conversion (8.22ms)
            
            UInt32 temp = this.ReadAdcValue();
            if (temp == 0)
            {
                throw new InvalidOperationException("Conversion sequence was unsuccessful.");
            }

            return temp;
        }

        private UInt32 ReadRawPressure()
        {
            // Uncorrected Pressure (D1)
            const byte CONV_SEQ_COMMAND_D1_OSR256 = 0x40;
            const byte CONV_SEQ_COMMAND_D1_OSR512 = 0x42;
            const byte CONV_SEQ_COMMAND_D1_OSR1024 = 0x44;
            const byte CONV_SEQ_COMMAND_D1_OSR2048 = 0x46;
            const byte CONV_SEQ_COMMAND_D1_OSR4096 = 0x48;

            _i2cBus.Write(_i2cConfig, new byte[] { CONV_SEQ_COMMAND_D1_OSR4096 }, I2C_TIMEOUT);
            Thread.Sleep(15); // wait for conversion (8.22ms)

            UInt32 pressure = this.ReadAdcValue();
            if (pressure == 0)
            {
                throw new InvalidOperationException("Conversion sequence was unsuccessful.");
            }

            return pressure;
        }
        
        private UInt32 ReadAdcValue()
        {
            const byte ADC_READ_COMMAND = 0x00;
            _i2cBus.Write(_i2cConfig, new byte[] { ADC_READ_COMMAND }, I2C_TIMEOUT);

            byte[] result = new byte[3];
            _i2cBus.Read(_i2cConfig, result, I2C_TIMEOUT);

            UInt32 value = (UInt32)((result[0] << 16) | (result[1] << 8) | result[0]);
            return value;
        }

        private struct CalibrationData
        {
            public ushort FactoryData;
            public ushort C1_SensT1;
            public ushort C2_OffT1;
            public ushort C3_TCS;
            public ushort C4_TCO;
            public ushort C5_TRef;
            public ushort C6_TempSens;
            public ushort SerialCodeAndCRC;

            public CalibrationData(byte[] PromData)
            {
                if (PromData.Length != 16)
                {
                    throw new ArgumentException("PromData");
                }

                FactoryData = ToUShort(PromData[0], PromData[1]);
                C1_SensT1 = ToUShort(PromData[2], PromData[3]);
                C2_OffT1 = ToUShort(PromData[4], PromData[5]);
                C3_TCS = ToUShort(PromData[6], PromData[7]);
                C4_TCO = ToUShort(PromData[8], PromData[9]);
                C5_TRef = ToUShort(PromData[10], PromData[11]);
                C6_TempSens = ToUShort(PromData[12], PromData[13]);
                SerialCodeAndCRC = ToUShort(PromData[14], PromData[15]);
            }

            private static ushort ToUShort(byte byte1, byte byte2)
            {
                return (ushort)((byte1 << 8) | (byte2 << 0));
            }
        }
    }
}
