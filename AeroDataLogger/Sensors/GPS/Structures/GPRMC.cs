using System;
using Microsoft.SPOT;
using System.Text;

namespace GPS.Structures
{
    internal struct GPRMC
    {
        public DateTime UtcTime;
        public LatLong Latitude;
        public LatLong Longitude;
        public double GroundSpeedKts;
        public double CourseDegrees;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UtcTime.ToString());
            sb.Append(", ");
            sb.Append(Latitude.ToString());
            sb.Append(", ");
            sb.Append(Longitude.ToString());
            return sb.ToString();
        }

        public GPRMC(string[] parts)
        {
            UtcTime = DateTime.MinValue;
            Latitude = new LatLong();
            Longitude = new LatLong();
            GroundSpeedKts = 0;
            CourseDegrees = 0;

            if (parts[0] != "$GPRMC" && parts.Length != 15)
            {
                return;
            }

            UtcTime = ParseDate(parts[1], parts[9]);

            if (parts[2] != "A")
            {
                // Status is not 'Active' - no nav data
                return;
            }

            Latitude = new LatLong(parts[3], parts[4]);
            Longitude = new LatLong(parts[5], parts[6]);
            double.TryParse(parts[7], out GroundSpeedKts);
            double.TryParse(parts[8], out CourseDegrees);
        }

        private DateTime ParseDate(string timeString, string dateString)
        {
            if (timeString == null || dateString == null || timeString == string.Empty || dateString == string.Empty)
            {
                return DateTime.MinValue;
            }

            try
            {
                int hours = int.Parse(timeString.Substring(0, 2));
                int mins = int.Parse(timeString.Substring(2, 2));
                int secs = int.Parse(timeString.Substring(4, 2));
                int ms = int.Parse(timeString.Split('.')[1]);

                int day = int.Parse(dateString.Substring(0, 2));
                int month = int.Parse(dateString.Substring(2, 2));
                int year = 2000 + int.Parse(dateString.Substring(4, 2));

                var dateTime = new DateTime(year, month, day, hours, mins, secs, ms);
                return dateTime;
            }
            catch (Exception)
            {
                Debug.Print("Exception parsing datetime");
                return DateTime.MinValue;
            }
        }
    }
}
