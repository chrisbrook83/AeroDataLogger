using System;
using Microsoft.SPOT;
using System.IO;
using AeroDataLogger.Output;

namespace AeroDataLogger.Output
{
    internal interface ILogSink
    {
        void WriteLine(string value);
    }

    internal static class Log
    {
        private static object _lock = new object();

        private static ILogSink[] _sinks = new ILogSink[0];

        public static void AttachLogSink(ILogSink sink)
        {
            lock (_lock)
            {
                // Append to array
                ILogSink[] newSinks = new ILogSink[_sinks.Length + 1];
                Array.Copy(_sinks, newSinks, _sinks.Length);
                newSinks[newSinks.Length - 1] = sink;
                _sinks = newSinks;
            }
        }

        public static void WriteLine(string value)
        {
            // Assume these are already thread-safe
            Debug.Print(value);
            Trace.Print(value);

            lock (_lock)
            {
                foreach (ILogSink sink in _sinks)
                {
                    sink.WriteLine(value);
                }
            }
        }
    }
}
