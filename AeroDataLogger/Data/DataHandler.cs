using System;
using Microsoft.SPOT;
using System.Text;

namespace AeroDataLogger.Data
{
    internal class DataHandler
    {
        private readonly IDataSink[] _dataSinks;
        public DataHandler(IDataSink[] dataSinks)
        {
            _dataSinks = dataSinks;
        }

        public void WriteHeader(string[] columnHeaders)
        {
            string headerLine = ConvertToTabSeparatedString(columnHeaders);
            foreach (IDataSink sink in _dataSinks)
            {
                sink.SaveRecord(headerLine);
            }
        }
        
        public void HandleData(object[] data)
        {
            string line = ConvertToTabSeparatedString(data);
            foreach (IDataSink sink in _dataSinks)
            {
                sink.SaveRecord(line);
            }
        }

        // Note, generics are not supported in NETMF
        public string ConvertToTabSeparatedString(object[] data)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString());

                if (i != data.Length - 1)
                {
                    sb.Append("\t");
                }
            }

            return sb.ToString();
        }
    }
}
