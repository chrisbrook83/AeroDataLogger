using System;
using Microsoft.SPOT;
using System.Text;

namespace GPS.Structures
{
    internal struct LatLong
    {
        public string Direction;

        // Deg Min Sec
        public int Degrees;
        public int Minutes;
        public double Seconds;

        // Deg + Mins.0
        public double FractionalMinutes;

        // Deg.0
        public double FractionalDegrees;

        public override string ToString()
        {
            return Direction + FractionalDegrees.ToString();
        }

        public LatLong(string value, string northSouth)
        {
            Degrees = 0;
            Minutes = 0;
            Seconds = 0;
            FractionalMinutes = 0;
            FractionalDegrees = 0;
            Direction = northSouth;

            if (value == null || northSouth == null)
            {
                return;
            }

            try
            {
                var raw = double.Parse(value);
                FractionalMinutes = (raw % 100);
                Degrees = (int)((raw - FractionalMinutes) / 100);
                Minutes = (int)FractionalMinutes;
                Seconds = (FractionalMinutes - Minutes) * 60;
                FractionalDegrees = Degrees + (FractionalMinutes / 60);
                return;
            }
            catch (Exception)
            {
                Debug.Print("Exception parsing LatLong");
                return;
            }
        }
    }
}
