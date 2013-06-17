using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;
using System;
using ExampleAccelGyroSensor.Sensor;

namespace ExampleAccelGyroSensor.I2C_Hardware
{
    /// <summary>
    /// Einfache Klasse zum Verbinden eines Moduls mit dem I²C Bus.
    /// Ist aus einem Beispiel im Forum von Netduino.com abgeleitet.
    /// </summary>
    public class I2C_Connector
    {
        /// <summary>
        /// Konfigurationen festhalten
        /// </summary>
        private I2CDevice.Configuration _Config;
        
        /// <summary>
        /// Hauptklasse für die Verbindung
        /// </summary>
        private I2CDevice _Device;
        
        /// <summary>
        /// Initialisiert die Klasse
        /// Konfiguration wird angelegt und
        /// die entsprechende Klasse I2CDevice wird initialisiert
        /// </summary>
        /// <param name="address">Byte Adresse vom Modul übergeben.</param>
        /// <param name="clockRateKHz">Taktrate in KHz festlegen</param>
        public I2C_Connector(byte address, int clockRateKHz)
        {
            // Adresse und Taktfrequenz übergeben
            _Config = new I2CDevice.Configuration(address, clockRateKHz);
            _Device = new I2CDevice(_Config);
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
            int written = this._Device.Execute(writeTransaction, 1000);

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
            int read = this._Device.Execute(transactions, 1000);

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
