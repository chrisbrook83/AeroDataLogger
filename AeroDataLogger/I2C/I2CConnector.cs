using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace AeroDataLogger.I2C
{
    /// <summary>
    /// Helper class for working with the I2C bus.
    /// </summary>
    public class I2CConnector
    {
        private I2CDevice.Configuration _config;
        private I2CDevice _device;

        private const int TIMEOUT_MS = 1000;
        
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

            int written = _device.Execute(writeTransaction, TIMEOUT_MS);

            if (written == 0)
            {
                Debug.Print("No data could be sent to the module."); 
            }

            return written;
        }
        
        public int Read(byte[] resultBuffer)
        {
            var transactions = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateReadTransaction(resultBuffer)
            };

            int read = _device.Execute(transactions, TIMEOUT_MS);

            if (read == 0)
            {
                Debug.Print("Data could not be read from the module.");
            }

            return read;
        }
    }
}
