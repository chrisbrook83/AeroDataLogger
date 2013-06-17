
namespace ExampleAccelGyroSensor.Sensor
{
    /// <summary>
    /// Objekt zum Auswerten der Ergebnisse und Bereitstellung der gemessenen Werten.
    /// </summary>
    public struct AccelerationAndGyroData
    {
        /// <summary>
        /// X Achse des Beschleunigungssensors
        /// </summary>
        public int Acceleration_X;
        /// <summary>
        /// Y Achse des Beschleunigungssensors
        /// </summary>
        public int Acceleration_Y;
        /// <summary>
        /// Z  Achse des Beschleunigungssensors
        /// </summary>
        public int Acceleration_Z;
        /// <summary>
        /// Temperatur Wert
        /// </summary>
        public int Temperatur;
        /// <summary>
        /// X Achse des Gyroskop
        /// </summary>
        public int Gyro_X;
        /// <summary>
        /// Y Achse des Gyroskop
        /// </summary>
        public int Gyro_Y;
        /// <summary>
        /// Z Achse des Gyroskop
        /// </summary>
        public int Gyro_Z;
        /// <summary>
        /// Erstellt das Objekt mit den Ergebnissen
        /// </summary>
        /// <param name="results"></param>
        public AccelerationAndGyroData(byte[] results)
        {
            // Ergebnis für den Beschleunigungssensors zusammenlegen durch Bitshifting
            Acceleration_X = (((int)results[0]) << 8) | results[1];
            Acceleration_Y = (((int)results[2]) << 8) | results[3];
            Acceleration_Z = (((int)results[4]) << 8) | results[5];
            
            // Ergebnis für Temperatur (Bisher noch nicht getestet)
            Temperatur = (((int)results[6]) << 8) | results[7];
            
            // Ergebnis für den Gyroskopsensors zusammenlegen durch Bitshifting
            Gyro_X = (((int)results[8]) << 8) | results[9];
            Gyro_Y = (((int)results[10]) << 8) | results[11];
            Gyro_Z = (((int)results[12]) << 8) | results[13];
        }
        /// <summary>
        /// Überschreibe die ToString() Methode
        /// Gibt alle in Werte in einem String zurück
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Acceleration: X: " + GetProzent(Acceleration_X) + "\tY:" + GetProzent(Acceleration_Y) + "\tZ:" + GetProzent(Acceleration_Z) +
                " \t Temp: " + GetProzent(Temperatur) +
                " \t Gyro: X: " + GetProzent(Gyro_X) + "\tY: " + GetProzent(Gyro_Y) + "\tZ: " + GetProzent(Gyro_Z);
        }
        /// <summary>
        /// Gibt den Wert in Prozent Wert zurück, um die Zahl kleiner zu halten.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetProzent(int value)
        {
            double d = (double)value;
            double result = System.Math.Round((d / 65535) * 100);

            return GetToThreeStellingerWorth(result.ToString());
        }
        /// <summary>
        /// Macht aus dem Wert eine drei stellige Zahl
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetToThreeStellingerWorth(string content)
        {
            if (content.Length == 1)
            {
                return "__" + content;
            }
            else if (content.Length == 2)
            {
                return "_" + content;
            }

            return content;
        }
    }
}
