using System;
using Microsoft.SPOT;
using System.Text;

namespace GPS.Structures
{
    internal struct GPGLL
    {
        double Latitude;
        double Longitude;
        DateTime Time;
        bool DataActive;

        public GPGLL(string[] fields)
        {
            try
            {
                Latitude = 0;
                Longitude = 0;
                Time = DateTime.MinValue;
                DataActive = false;

                if (fields.Length != 8)
                {
                    Debug.Print("ParseFail: field length = " + fields.Length);
                    return;
                }

                if (fields[0] != "$GPGLL")
                {
                    return;
                }

                string timePart = fields[5];
                if (timePart == string.Empty)
                {
                    return;
                }

                int hour = int.Parse(timePart.Substring(0, 2));
                int mins = int.Parse(timePart.Substring(2, 2));
                int secs = int.Parse(timePart.Substring(4, 2));
                this.Time = new DateTime(0, 0, 0, hour, mins, secs);

                this.DataActive = (fields[6] == "A");

                switch (fields[2])
                {
                    case "N":
                        this.Latitude = double.Parse(fields[1]);
                        break;

                    case "S":
                        this.Latitude = -1 * double.Parse(fields[1]);
                        break;

                    default:
                        return;
                }

                switch (fields[4])
                {
                    case "E":
                        this.Longitude = double.Parse(fields[3]);
                        break;

                    case "W":
                        this.Longitude = -1 * double.Parse(fields[3]);
                        break;

                    default:
                        return;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Latitude);
            sb.Append(", ");
            sb.Append(Longitude);
            sb.Append(", ");
            sb.Append(Time);
            sb.Append(", ");
            sb.Append(DataActive);
            return sb.ToString();
        }

        /*
         $GPGLL,4916.45,N,12311.12,W,225444,A,*1D

Where:
     GLL          Geographic position, Latitude and Longitude
     4916.46,N    Latitude 49 deg. 16.45 min. North
     12311.12,W   Longitude 123 deg. 11.12 min. West
     225444       Fix taken at 22:54:44 UTC
     A            Data Active or V (void)
     *iD          checksum data
*/
    }
}
