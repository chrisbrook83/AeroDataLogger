using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using AeroDataLogger.Output;
using System.Threading;

namespace AeroDataLogger.I2C
{
    /// <summary>
    /// From: http://wiki.netduino.com/I2C-Bus-class.ashx
    /// 
    /// I2C Bus implementation that will help you easily manage multiple I2C devices on a single bus
    /// Author: phantomtypist
    /// Forum Thread: http://forums.netduino.com/index.php?/topic/563-i2cbus/page__p__4156#entry4156
    /// </summary>
    public class I2CBus : IDisposable
    {
        private static I2CBus _instance = null;
        private static readonly object LockObject = new object();

        private readonly I2CDevice _slaveDevice;

        public static void DestroySingleton()
        {
            lock (LockObject)
            {
                _instance.Dispose();
                _instance = null;
            }
        }
        
        public static I2CBus GetInstance()
        {
            lock (LockObject)
            {
                if (_instance == null)
                {
                    _instance = new I2CBus();
                }

                return _instance;
            }
        }

        private I2CBus()
        {
            this._slaveDevice = new I2CDevice(null);
        }

        public void Dispose()
        {
            this._slaveDevice.Dispose();
        }

        /// <summary>
        /// Generic write operation to I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="writeBuffer">The array of bytes that will be sent to the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public int Write(I2CDevice.Configuration config, byte[] writeBuffer, int transactionTimeout)
        {
            lock (_slaveDevice)
            {
                // Set i2c device configuration.
                _slaveDevice.Config = config;

                // create an i2c write transaction to be sent to the device.
                I2CDevice.I2CTransaction[] writeXAction = new I2CDevice.I2CTransaction[] 
                { 
                    I2CDevice.CreateWriteTransaction(writeBuffer) 
                };

                // the i2c data is sent here to the device.
                int transferred = 0;
                do
                {
                    transferred = _slaveDevice.Execute(writeXAction, transactionTimeout);

                    if (transferred == 0)
                    {
                        throw new Exception("Could not write to device.");
                    }
                }
                while (transferred == 0);

                // make sure the data was sent.
                if (transferred != writeBuffer.Length)
                {
                    throw new Exception("Could not write to device.");
                }

                return transferred;
            }
        }

        /// <summary>
        /// Generic read operation from I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="readBuffer">The array of bytes that will contain the data read from the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public int Read(I2CDevice.Configuration config, byte[] readBuffer, int transactionTimeout)
        {
            lock (_slaveDevice)
            {
                // Set i2c device configuration.
                 _slaveDevice.Config = config;

                // create an i2c read transaction to be sent to the device.
                I2CDevice.I2CTransaction[] readXAction = new I2CDevice.I2CTransaction[] 
                {
                    I2CDevice.CreateReadTransaction(readBuffer) 
                };

                // the i2c data is received here from the device.
                int transferred = _slaveDevice.Execute(readXAction, transactionTimeout);

                // make sure the data was received.
                if (transferred != readBuffer.Length)
                {
                    throw new Exception("Could not read from device.");
                }

                return transferred;
            }
        }

        /// <summary>
        /// Read array of bytes at specific register from the I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="register">The register to read bytes from.</param>
        /// <param name="readBuffer">The array of bytes that will contain the data read from the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void ReadRegister(I2CDevice.Configuration config, byte register, byte[] readBuffer, int transactionTimeout)
        {
            byte[] registerBuffer = { register };
            Write(config, registerBuffer, transactionTimeout);
            Read(config, readBuffer, transactionTimeout);
        }

        /// <summary>
        /// Write array of bytes value to a specific register on the I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="register">The register to send bytes to.</param>
        /// <param name="writeBuffer">The array of bytes that will be sent to the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void WriteRegister(I2CDevice.Configuration config, byte register, byte[] writeBuffer, int transactionTimeout)
        {
            byte[] registerBuffer = { register };
            Write(config, registerBuffer, transactionTimeout);
            Write(config, writeBuffer, transactionTimeout);
        }

        /// <summary>
        /// Write a byte value to a specific register on the I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="register">The register to send bytes to.</param>
        /// <param name="value">The byte that will be sent to the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void WriteRegister(I2CDevice.Configuration config, byte register, byte value, int transactionTimeout)
        {
            byte[] writeBuffer = { register, value };
            Write(config, writeBuffer, transactionTimeout);
        }

    }
}