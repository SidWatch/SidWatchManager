using System;
using System.IO;
using sharpHDF.Library.Objects;
using SidWatchLibrary.Objects;

namespace SidWatchCollectionLibrary.Writers
{
    public class HdfWriter : IFileWriter, IDisposable
    {
        private Hdf5File m_File;
                
        public void OpenFile(string _folder, Station _station, string _fileSuffix)
        {
            if (m_File != null)
            {
                throw new Exception("File already open");
            }

            string filename = string.Format("{0}_{1}.h5", _station.StationName, _fileSuffix);
            string filepath = Path.Combine(_folder, filename);

            if (File.Exists(filepath))
            {
                m_File = new Hdf5File(filepath);
            }
            else
            {
                //Setup the new file
                m_File = Hdf5File.Create(filepath);
                SetFileAttributes(m_File, _station);
            }           
            
        }

        private void SetFileAttributes(Hdf5File _file, Station _station)
        {
            var attributes = _file.Attributes;

            if (attributes.Count == 0)
            {
                attributes.Add("MonitorId", _station.MonitorId);
                attributes.Add("StationName", _station.StationName);
                attributes.Add("Latitude", _station.Latitude);
                attributes.Add("Longitude", _station.Longitude);
                attributes.Add("UtcOffset", _station.UtcOffset);
                attributes.Add("Timezone", _station.Timezone);
                attributes.Add("CreatedDateTime", DateTime.UtcNow.ToString("O"));
            }
        }

        public void CloseFile()
        {
            if (m_File != null)
            {
                m_File.Close();
                m_File = null;
            }
        }

        public bool IsFileOpen()
        {
            if (m_File != null)
            {
                return true;
            }

            return false;
        }

        public void WriteAudioSegment(AudioSegment _segment)
        {
            if (m_File == null)
            {
                throw new Exception("File not open");
            }



        }

        public void Dispose()
        {
            CloseFile();
        }
    }
}

