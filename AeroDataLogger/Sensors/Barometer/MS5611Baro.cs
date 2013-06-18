using System;
using Microsoft.SPOT;
using AeroDataLogger.I2C;
using System.Threading;

namespace AeroDataLogger.Sensors.Barometer
{
    /// <summary>
    /// http://www.meas-spec.com/downloads/MS5611-01BA03.pdf
    /// </summary>
    public class MS5611Baro
    {
        private const byte I2CAddress = 0x77;
        private I2CConnector _I2CConnector;
        CalibrationData _calibrationData;

        public MS5611Baro()
        {
            _I2CConnector = new I2CConnector(I2CAddress, 100);
            Initialise();
        }


        public void ReadTemperatureAndPressure(out double temp, out double pressure)
        {
            UInt32 d1Pres = ReadRawPressure();
            UInt32 d2Temp = ReadRawTemperature();
            Int32 dT = (Int32)(d2Temp - (_calibrationData.TRef * System.Math.Pow(2,8)));
            temp = (2000 + dT * ((double)_calibrationData.TempSens / System.Math.Pow(2, 23))) / 100.0;

            Int64 offset = (Int64)(_calibrationData.OffT1 * System.Math.Pow(2, 16) + (_calibrationData.TCO * dT) / System.Math.Pow(2, 7));
            Int64 sens = (Int64)(_calibrationData.SensT1 * System.Math.Pow(2, 15) + (_calibrationData.TCS * dT) / System.Math.Pow(2, 8));
            pressure = (d1Pres * sens / System.Math.Pow(2, 21) - offset) / System.Math.Pow(2, 15) / 100;
        }

        private UInt32 ReadRawTemperature()
        {
            // Temperature (D2)
            const byte CONV_SEQ_COMMAND_D2_OSR256 = 0x50;
            const byte CONV_SEQ_COMMAND_D2_OSR512 = 0x52;
            const byte CONV_SEQ_COMMAND_D2_OSR1024 = 0x54;
            const byte CONV_SEQ_COMMAND_D2_OSR2048 = 0x56;
            const byte CONV_SEQ_COMMAND_D2_OSR4096 = 0x58;

            _I2CConnector.Write(new byte[] { CONV_SEQ_COMMAND_D2_OSR256 });
            Thread.Sleep(100);
            UInt32 temp = this.ReadAdcValue();
            return temp;
        }

        private UInt32 ReadRawPressure()
        {
            // Pressure (D1)
            const byte CONV_SEQ_COMMAND_D1_OSR256 = 0x50;
            const byte CONV_SEQ_COMMAND_D1_OSR512 = 0x52;
            const byte CONV_SEQ_COMMAND_D1_OSR1024 = 0x54;
            const byte CONV_SEQ_COMMAND_D1_OSR2048 = 0x56;
            const byte CONV_SEQ_COMMAND_D1_OSR4096 = 0x58;

            _I2CConnector.Write(new byte[] { CONV_SEQ_COMMAND_D1_OSR256 });
            Thread.Sleep(100);
            UInt32 pressure = this.ReadAdcValue();
            return pressure;
        }

        private void Initialise()
        {
            Reset();
            ReadCalibrationData();
        }

        private void Reset()
        {
            const byte RESET_COMMAND = 0x1E;
            _I2CConnector.Write(new byte[] { RESET_COMMAND });
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
                _I2CConnector.Write(new byte[] { commands[i] });
                byte[] temp = new byte[2];
                _I2CConnector.Read(temp);
                Array.Copy(temp, 0, results, 2 * i, 2);
            }

            _calibrationData = new CalibrationData(results);
        }
        
        private UInt32 ReadAdcValue()
        {
            const byte ADC_READ_COMMAND = 0x00;
            _I2CConnector.Write(new byte[] { ADC_READ_COMMAND });

            byte[] result = new byte[3];
            _I2CConnector.Read(result);

            UInt32 value = (UInt32)((result[0] << 16) | (result[1] << 8) | result[0]);
            return value;
        }

        private struct CalibrationData
        {
            public CalibrationData(byte[] PromData)
            {
                if (PromData.Length != 16)
                {
                    throw new ArgumentException("PromData");
                }

                FactoryData = ToUShort(PromData[0], PromData[1]);
                SensT1 = ToUShort(PromData[2], PromData[3]);
                OffT1 = ToUShort(PromData[4], PromData[5]);
                TCS = ToUShort(PromData[6], PromData[7]);
                TCO = ToUShort(PromData[8], PromData[9]);
                TRef = ToUShort(PromData[10], PromData[11]);
                TempSens = ToUShort(PromData[12], PromData[13]);
                SerialCodeAndCRC = ToUShort(PromData[14], PromData[15]);
            }

            public ushort FactoryData;
            public ushort SensT1;
            public ushort OffT1;
            public ushort TCS;
            public ushort TCO;
            public ushort TRef;
            public ushort TempSens;
            public ushort SerialCodeAndCRC;


            private static ushort ToUShort(byte byte1, byte byte2)
            {
                return (ushort)((byte1 << 8) | (byte2 << 0));
            }
        }
    }
}
