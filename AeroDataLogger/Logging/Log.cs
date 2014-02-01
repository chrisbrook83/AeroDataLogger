using System;
using Microsoft.SPOT;
using System.IO;
using AeroDataLogger.Output;

namespace AeroDataLogger.Output
{
    internal interface ILogSink
    {
        void WriteLine(string value);
        void TryDispose();
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

        public static void DetachLogSink(ILogSink sink)
        {
            lock (_lock)
            {
                // Remove from array
                ILogSink[] newSinks = new ILogSink[_sinks.Length - 1];
                for (int i = 0, j = 0; i < _sinks.Length; i++)
                {
                    if (_sinks[i] != sink)
                    {
                        newSinks[j] = _sinks[i];
                        j++;
                    }
                }
                
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

        public static void Close()
        {
            foreach (ILogSink sink in _sinks)
            {
                DetachLogSink(sink);
                sink.TryDispose();
            }
        }
    }
}
