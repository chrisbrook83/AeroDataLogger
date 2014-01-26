using System;
using System.Text;
namespace AeroDataLogger.Sensors.AccelGyro
{
    public struct AccelerationAndGyroData
    {
        /// <summary>
        /// X-axis acceleration
        /// </summary>
        public double Ax;

        /// <summary>
        /// Y-axis acceleration
        /// </summary>
        public double Ay;

        /// <summary>
        /// Z -axis acceleration
        /// </summary>
        public double Az;

        /// <summary>
        /// Temperature
        /// </summary>
        public double Temp;

        /// <summary>
        /// X-axis angular rate
        /// </summary>
        public double Rx;

        /// <summary>
        /// Y-axis angular rate
        /// </summary>
        public double Ry;

        /// <summary>
        /// Z-axis angular rate
        /// </summary>
        public double Rz;

        public AccelerationAndGyroData(double aX, double aY, double aZ, double temp, double rX, double rY, double rZ)
        {
            Ax = aX;
            Ay = aY;
            Az = aZ;
            Temp = temp;
            Rx = rX;
            Ry = rY;
            Rz = rZ;
        }

        public override string ToString()
        {
            var temp = AccelerationAndGyroDataBuilder.ConvertValueToString(Temp, 2, 1);
            var aX = AccelerationAndGyroDataBuilder.ConvertValueToString(Ax, 1, 3);
            var aY = AccelerationAndGyroDataBuilder.ConvertValueToString(Ay, 1, 3);
            var aZ = AccelerationAndGyroDataBuilder.ConvertValueToString(Az, 1, 3);
            var rX = AccelerationAndGyroDataBuilder.ConvertValueToString(Rx, 3, 1);
            var rY = AccelerationAndGyroDataBuilder.ConvertValueToString(Ry, 3, 1);
            var rZ = AccelerationAndGyroDataBuilder.ConvertValueToString(Rz, 3, 1);

            return "T: " + temp + "degC" +
                "\tG: " + aX + "X   " + aY + "Y   " + aZ + "Z" +
                "\tR: " + rX + "X   " + rY + "Y   " + rZ + "Z";
        }
    }
}