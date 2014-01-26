using System;
using Microsoft.SPOT;

namespace AeroDataLogger.Data
{
    internal class DebuggerDataSink : IDataSink
    {
        public void SaveRecord(string value)
        {
            Debug.Print(value);
        }
    }
}
