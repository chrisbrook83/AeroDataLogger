using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;
using System;
using ExampleAccelGyroSensor.Sensor;

namespace ExampleAccelGyroSensor.I2C.Hardware
{
    /// <summary>
    /// Einfache Klasse zum Verbinden eines Moduls mit dem I²C Bus.
    /// Ist aus einem Beispiel im Forum von Netduino.com abgeleitet.
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

        /// <summary>
        /// Sendet den Inhalt des Byte Array zur Hardware
        /// </summary>
        /// <param name="writeBuffer">Byte Array</param>
        public int Write(byte[] writeBuffer)
        {
            // Byte Array übergeben für das ertellen einer Transaction
            I2CDevice.I2CTransaction[] writeTransaction = new I2CDevice.I2CTransaction[]
            { 
                I2CDevice.CreateWriteTransaction(writeBuffer)
            };

            // Sende die Daten an die Hardware. Timeout bei 1 Sekunde
            int written = this._device.Execute(writeTransaction, 1000);

            // Check if all data has been sent, otherwise throw exception
            if (written == 0)
            {
                // "Es konnten keine Daten an das Modul gesendet werden."
                Debug.Print("No data could be sent to the module"); 
            }

            return written;
        }
        
        /// <summary>
        /// Gets the values ​​for the addresses in the buffer  
        /// (Ruft mit den Adressen im Buffer die Werte ab)
        /// </summary>
        /// <param name="readBuffer">Byte array containing addresses for retrieval of relevant data. 
        /// (Byte Array mit Adressen für den Abruf entsprechender Daten)</param>
        public int Read(byte register, byte[] resultBuffer)
        {
            var transactions = new I2CDevice.I2CTransaction[]
            {
                //I2CDevice.CreateWriteTransaction(new byte[] { register }),
                //I2CDevice.CreateReadTransaction(new byte[] { resultBuffer })
                I2CDevice.CreateReadTransaction(resultBuffer)
            };
            
            // Read data from the hardware - timeout 1 sec... (Lese die Daten von der Hardware. Timeout von einer Sekunde)
            int read = this._device.Execute(transactions, 1000);

            // Check if the data was sent... (Prüfe, ob die Daten gesendt wurden)
            if (read == 0)
            {
                string message = "Data could not be read from the module."; //Es konnte nicht vom Modul gelesen werden.
                //throw new Exception(message);
                Debug.Print(message);
            }

            return read;
        }
    }
}
