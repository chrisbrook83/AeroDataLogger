using System;
using System.IO;
using Microsoft.SPOT;

namespace AeroDataLogger.Logging
{
    public class Log : IDisposable
    {
        private readonly FileStream _logFilestream;
        private readonly TextWriter _writer;

        public Log()
        {
            const string LOG_FOLDER = @"\SD\Logs\";

            try
            {
                string logFilename = GetNextLogFilename(LOG_FOLDER);

                _logFilestream = new FileStream(logFilename, FileMode.Create, FileAccess.Write);
                _writer = new StreamWriter(_logFilestream);
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to initialise logger: " + ex.ToString());
            }
        }

        public void Write(string message)
        {
            Debug.Print("Log: " + message);
            if (_writer != null) _writer.WriteLine(message);
            if (_logFilestream != null) _logFilestream.Flush();
        }

        private string GetNextLogFilename(string logFolder)
        {
            // Note: log filename format: "log_<int>.txt"

            DirectoryInfo di = new DirectoryInfo(logFolder);

            int maxLogIndex = -1;
            foreach (FileInfo fileInfo in di.GetFiles())
            {
                try
                {
                    if (fileInfo.Extension == ".txt")
                    {
                        string logNumber = fileInfo.Name.Split('_')[1].Split('.')[0];
                        int thisIndex = int.Parse(logNumber);
                        maxLogIndex = thisIndex > maxLogIndex ? thisIndex : maxLogIndex;
                    }
                }
                catch (Exception)
                {
                    // Some random file in the folder - ignore it
                    continue;
                }
            }

            const int BASE_LOG_INDEX = 10000;
            int nextLogIndex = maxLogIndex == -1 ? BASE_LOG_INDEX : (maxLogIndex + 1);
            string newLogFile = logFolder + "log_" + nextLogIndex.ToString() + ".txt";
            return newLogFile;
        }

        public void Dispose()
        {
            if (_writer != null) _writer.Dispose();
            if (_logFilestream != null) _logFilestream.Dispose();
        }
    }
}
