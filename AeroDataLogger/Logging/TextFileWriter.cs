using System;
using System.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using System.Text;

namespace AeroDataLogger.Logging
{
    public class TextFileWriter : IDisposable
    {
        private const string VOLUME_LABEL = "SD";

        private readonly FileStream _filestream;
        private readonly TextWriter _writer;

        private bool _logOnline = false;

        public TextFileWriter(string folder, string filePrefix)
        {
            string loggingFolder = @"\" + VOLUME_LABEL + @"\" + folder + @"\";

            RemovableMedia.Insert += new InsertEventHandler(OnMediaInsert);
            RemovableMedia.Eject += new EjectEventHandler(OnMediaEject);

            try
            {
                if (IsSDCardPresent())
                {
                    string logFilename;
                    if (TryGetNextLogFilename(loggingFolder, filePrefix, out logFilename))
                    {
                        _filestream = new FileStream(logFilename, FileMode.Create, FileAccess.Write);
                        _writer = new StreamWriter(_filestream);
                        _logOnline = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Exception thrown whilst initialising logger: " + ex.ToString());
            }
        }

        private void OnMediaInsert(object sender, MediaEventArgs e)
        {
            Debug.Print("SD Card inserted at " + e.Time);
            if (IsSDCardPresent())
            {
                _logOnline = true;
            }
        }

        private void OnMediaEject(object sender, MediaEventArgs e)
        {
            Debug.Print("SD Card ejected");
            _logOnline = false;
        }

        public void Write(object[] data)
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

            WriteLine(sb.ToString());
        }
        
        public void WriteLine(string value)
        {
            if (_logOnline)
            {
                if (_writer != null) _writer.WriteLine(value);
                if (_filestream != null) _filestream.Flush();
            }
        }

        private bool TryGetNextLogFilename(string logFolder, string filePrefix, out string nextLogFilename)
        {
            // Note: log filename format: "<filePrefix>_<int>.txt"
            nextLogFilename = null;
            DirectoryInfo di = new DirectoryInfo(logFolder);

            if (!di.Exists)
            {
                Debug.Print("No SD card present?");
                return false;
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
            return true;
        }

        private bool IsSDCardPresent() 
        { 
            VolumeInfo[] volumes = VolumeInfo.GetVolumes(); 
            foreach (VolumeInfo volumeInfo in volumes) 
            {
                if (volumeInfo.Name.Equals(VOLUME_LABEL))
                {
                    Debug.Print("SD card present.");
                    return true;
                }
            }

            Debug.Print("SD not present.");
            return false; 
        }

        public void Dispose()
        {
            if (_writer != null) _writer.Dispose();
            if (_filestream != null) _filestream.Dispose();
        }
    }
}
