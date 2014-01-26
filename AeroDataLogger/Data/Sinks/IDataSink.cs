using System;

namespace AeroDataLogger.Data
{
    internal interface IDataSink
    {
        void SaveRecord(string value);
    }
}
