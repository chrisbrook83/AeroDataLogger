using System;
using Microsoft.SPOT;
using System.IO;
using AeroDataLogger.Output;

namespace AeroDataLogger.Logging
{
    internal static class Log
    {
        private static RS232Writer rs232Writer = new RS232Writer();
        private static TextFileWriter logFileWriter = new TextFileWriter("Logs", "Log");

        public static void WriteLine(string value)
        {
            Debug.Print(value);
            Trace.Print(value);
            rs232Writer.WriteAndFlushLine(value);
            logFileWriter.WriteLine(value);
        }
    }
}
