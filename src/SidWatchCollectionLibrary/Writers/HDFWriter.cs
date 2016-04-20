using System;
using System.Collections.Generic;
using System.IO;
using sharpHDF.Library.Enums;
using sharpHDF.Library.Objects;
using SidWatchCollectionLibrary.Writers;
using SidWatchLibrary.Objects;

namespace SidWatch.Collection.Library.Writers
{
    public class HdfWriter : IFileWriter, IDisposable
    {
        private readonly object m_Lock = new object();
        private Hdf5File m_File;
        private Hdf5Group m_AudioGroup;
        private Hdf5Group m_PowerSpectrumGroup;
                
        public void OpenFile(string _folder, Station _station, string _fileSuffix)
        {
            lock (m_Lock)
            {
                if (m_File != null)
                {
                    throw new Exception("File already open");
                }

                string filename = string.Format("{0}_{1}.h5", _station.StationName, _fileSuffix);
                string filepath = Path.Combine(_folder, filename);

                if (!File.Exists(filepath))
                {
                    CreateFile(filepath, _station);
                }

                m_File = new Hdf5File(filepath);

                var temp = m_File.Groups[0];
                if (temp.Name == "AudioReadings")
                {
                    m_AudioGroup = temp;
                    m_PowerSpectrumGroup = m_File.Groups[1];
                }
                else
                {
                    m_PowerSpectrumGroup = temp;
                    m_AudioGroup = m_File.Groups[1];
                }
            }
        }

        private void CreateFile(string filepath, Station _station)
        {
            //Setup the new file
            m_File = Hdf5File.Create(filepath);
            SetFileAttributes(m_File, _station);

            m_AudioGroup = m_File.Groups.Add("AudioReadings");
            m_PowerSpectrumGroup = m_File.Groups.Add("PowerSpectrum");

            m_File.Close();
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
            lock (m_Lock)
            {
                if (m_File != null)
                {
                    m_AudioGroup = null;
                    m_PowerSpectrumGroup = null;
                    m_File.Close();
                    m_File = null;
                }
            }
        }

        public bool IsFileOpen()
        {
            lock (m_Lock)
            {
                if (m_File != null)
                {
                    return true;
                }

                return false;
            }
        }

        public void WriteAudioSegment(AudioSegment _segment)
        {
            lock (m_Lock)
            {
                if (_segment == null)
                {
                    throw new ArgumentNullException();
                }

                if (m_File == null)
                {
                    throw new Exception("File not open");
                }

                string name = _segment.StartTime.ToString("O");

                //Save the audio data to the file
                List<Hdf5DimensionProperty> audioProps = new List<Hdf5DimensionProperty>
                {
                    new Hdf5DimensionProperty {CurrentSize = (ulong) _segment.Channel1.LongLength}
                };
                Hdf5Dataset audioDataset = m_AudioGroup.Datasets.Add(name, Hdf5DataTypes.Single, audioProps);
                audioDataset.SetData(_segment.Channel1);

                //Save the power spectrum to the file
                List<Hdf5DimensionProperty> powerSpectrumProps = new List<Hdf5DimensionProperty>
                {
                    new Hdf5DimensionProperty {CurrentSize = (ulong) _segment.PowerSpectrum.GetLength(0)},
                    new Hdf5DimensionProperty {CurrentSize = (ulong) _segment.PowerSpectrum.GetLength(1)}
                };
                Hdf5Dataset powerSpectrumDataset = m_PowerSpectrumGroup.Datasets.Add(name, Hdf5DataTypes.Single, powerSpectrumProps);
                powerSpectrumDataset.SetData(_segment.PowerSpectrum);

            }
        }

        public void Dispose()
        {
            CloseFile();
        }
    }
}

