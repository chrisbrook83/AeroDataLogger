using System;
using System.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using System.Text;

namespace AeroDataLogger.Output
{
    internal class TextFileWriter : IDisposable, ILogSink
    {
        private const string VOLUME_LABEL = "SD";

        private FileStream _filestream;
        private TextWriter _writer;
        private readonly string _destinationFolder;
        private readonly string _filePrefix;
        
        private readonly object _lock = new object();

        private bool _logOnline = false;
        private string _logFilePath;

        public TextFileWriter(string folder, string filePrefix)
        {
            _destinationFolder = @"\" + VOLUME_LABEL + @"\" + folder + @"\";
            _filePrefix = filePrefix;

            // TODO: these are tricky - is Insert called during boot if the card is already in?
            //RemovableMedia.Insert += new InsertEventHandler(OnMediaInsert);
            //RemovableMedia.Eject += new EjectEventHandler(OnMediaEject);

            TryInitialise();
        }

        private void TryInitialise()
        {
            lock (_lock)
            {
                // The media-insert event gets fired multiple times, so ignore subsequent calls to this method
                if (_logOnline)
                {
                    Log.WriteLine("File writer already initialised. Output file: '" + _logFilePath + "'.");
                    return;
                }

                if (IsSDCardPresent())
                {
                    _logFilePath = GetNextLogFilename(_destinationFolder, _filePrefix);
                    Log.WriteLine("Creating file: '" + _logFilePath + "'.");
                    _filestream = new FileStream(_logFilePath, FileMode.Create, FileAccess.Write);
                    _writer = new StreamWriter(_filestream);

                    _logOnline = true;
                }
                else
                {
                    Log.WriteLine("SD card not present. File writing disabled.");
                    _logOnline = false;
                }
            }
        }

        public void TryDispose()
        {
            this.Dispose();
        }

        private void OnMediaInsert(object sender, MediaEventArgs e)
        {
            Log.WriteLine("SD card inserted. Re-initialising file writer.");
            TryInitialise();
        }

        private void OnMediaEject(object sender, MediaEventArgs e)
        {
            Log.WriteLine("SD card ejected.");
            lock (_lock)
            {
                _logOnline = false;
            }
        }

        public void Write(object[] data)
        {
            lock (_lock)
            {
                if (_logOnline)
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < data.Length; i++)
                    {
                        sb.Append(data[i]);

                        if (i != data.Length - 1)
                        {
                            sb.Append("\t");
                        }
                    }

                    WriteLineInner(sb.ToString());
                }
            }
        }
        
        public void WriteLine(string value)
        {
            lock (_lock)
            {
                WriteLineInner(value);
            }
        }

        private void WriteLineInner(string value)
        {
            if (_writer != null && _filestream != null)
            {
                _writer.WriteLine(value);

                // TODO: There is a significant performance overhead for doing this on every call...
                _writer.Flush();
                _filestream.Flush();
            }
        }

        private string GetNextLogFilename(string logFolder, string filePrefix)
        {
            // Note: log filename format: "<filePrefix>_<int>.txt"
            string nextLogFilename = null;
            DirectoryInfo di = new DirectoryInfo(logFolder);

            if (!di.Exists)
            {
                Log.WriteLine("Creating missing directory '" + di.FullName + "' on SD card.");
                Directory.CreateDirectory(di.FullName);
            }

            int maxLogIndex = -1;
            foreach (FileInfo fileInfo in di.GetFiles())
            {
                try
                {
                    if (fileInfo.Extension == ".txt" && fileInfo.Name.Substring(0, filePrefix.Length) == filePrefix)
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
            nextLogFilename = logFolder + filePrefix + "_" + nextLogIndex.ToString() + ".txt";

            return nextLogFilename;
        }

        private bool IsSDCardPresent() 
        { 
            VolumeInfo[] volumes = VolumeInfo.GetVolumes(); 
            foreach (VolumeInfo volumeInfo in volumes) 
            {
                if (volumeInfo.Name.Equals(VOLUME_LABEL))
                {
                    return true;
                }
            }

            return false; 
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer.Dispose();
            }

            if (_filestream != null)
            {
                _filestream.Close();
                _filestream.Dispose();
            }
        }
    }
}
