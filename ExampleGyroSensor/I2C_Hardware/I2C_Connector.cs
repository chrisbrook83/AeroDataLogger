using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;
using System;
using ExampleAccelGyroSensor.Sensor;

namespace ExampleAccelGyroSensor.I2C.Hardware
{
    /// <summary>
    /// Class for working with the I2C bus.
    /// </summary>
    public class I2CConnector
    {
        private I2CDevice.Configuration _config;
        private I2CDevice _device;
        
        public I2CConnector(byte deviceAddress, int clockRateKHz)
        {
            _config = new I2CDevice.Configuration(deviceAddress, clockRateKHz);
            _device = new I2CDevice(_config);
        }

        public int Write(byte[] writeBuffer)
        {
            I2CDevice.I2CTransaction[] writeTransaction = new I2CDevice.I2CTransaction[]
            { 
                I2CDevice.CreateWriteTransaction(writeBuffer)
            };

            int written = this._device.Execute(writeTransaction, 1000);

            if (written == 0)
            {
                Debug.Print("No data could be sent to the module"); 
            }

            return written;
        }
        
        public int Read(byte[] resultBuffer)
        {
            var transactions = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateReadTransaction(resultBuffer)
            };
            
            int read = this._device.Execute(transactions, 1000);

            if (read == 0)
            {
                string message = "Data could not be read from the module.";
                Debug.Print(message);
            }

            return read;
        }
    }
}
