using System;
using Microsoft.SPOT;
using AeroDataLogger.Output;

namespace AeroDataLogger.Data
{
    internal class FileDataSink : IDataSink, IDisposable
    {
        private TextFileWriter _writer = new TextFileWriter("Results", "Raw");

        public void SaveRecord(string value)
        {
            _writer.WriteLine(value);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
