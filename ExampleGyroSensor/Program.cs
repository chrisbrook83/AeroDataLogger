/// #########################################################################################################
///
///  Blog: Meine Welt in meinem Kopf
///  Post: MPU6050 Sensor auslesen mit dem Netduino
///  Postdate: 01.01.2013
///  --------------------------------------------------------------------------------------------------------
///  Kurze Information:
///  Diese Solution dient als Quellcode Beispiel und zeigt lediglich 
///  die Funktionsweise mit Klassen für die I²C Verbindung in 
///  verwendung der MPU6050 Sensors.
///  Fehlerbehandlung, sowie Logging oder andere Erweiterungen 
///  für eine stabile Laufzeit der Anwendung sind nicht vorhanden.
///  
///  Für Änderungsvorschläge oder ergänzende Informationen zu meiner
///  Beispiel Anwendung, der oder die kann mich unter der Mail Adresse 
///  j.langner@gmx.net erreichen.
///  
///  
///  Vorraussetzung:
///  Netduino Plus
///  MPU6050
/// 
/// #########################################################################################################

using System.Threading;
using Microsoft.SPOT;
using ExampleAccelGyroSensor.Sensor;

namespace ExampleAccelGyroSensor
{
    public class Program
    {
        public static void Main()
        {
            // Initialisiere den Sensor
            MPU6050 mpu6050 = new MPU6050();

            // Objekt Anlegen für Gyro- und Beschleunigungssensor
            AccelerationAndGyroData senorResult = mpu6050.GetSensorData();

            while (true)
            {
                // Daten abrufen
                senorResult = mpu6050.GetSensorData();

                // Ausgeben
                Debug.Print(senorResult.ToString());

                Thread.Sleep(100);
            }
        }

    }
}
