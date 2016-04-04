using System;
using System.IO;
using HDF5DotNet;
using SidWatchCollectionLibrary.Helpers;
using SidWatchLibrary.Objects;

namespace SidWatchCollectionLibrary.Writers
{
    public class HdfWriter : IFileWriter
    {
        private H5FileId m_FileId = null;
        private H5GroupId m_PrimaryGroupId = null;
        
        public void OpenFile(string _folder, Station _station, string _fileSuffix)
        {
            if (m_FileId != null)
            {
                throw new Exception("File already open");
            }

            string filename = string.Format("{0}_{1}.h5", _station.StationName, _fileSuffix);
            string filepath = Path.Combine(_folder, filename);

            if (File.Exists(filepath))
            {
                m_FileId = H5F.open(filepath, H5F.OpenMode.ACC_RDWR);   
            }
            else
            {
                m_FileId = H5F.create(filepath, H5F.CreateMode.ACC_TRUNC);
            }

            H5G.

            var primaryGroupInfo = H5G.getInfoByName(m_FileId, "primary");

            
            SetPrimaryGroupAttributes(m_FileId, _station);
        }

        private void SetPrimaryGroupAttributes(H5ObjectWithAttributes _groupId, Station _station)
        {
            int attributeCount = H5A.getNumberOfAttributes(_groupId);

            if (attributeCount == 0)
            {
                HD5Helper.SetAttribute(_groupId, "MonitorId", _station.MonitorId);
                HD5Helper.SetAttribute(_groupId, "StationName", _station.StationName);
                HD5Helper.SetAttribute(_groupId, "Latitude", _station.Latitude);
                HD5Helper.SetAttribute(_groupId, "Longitude", _station.Longitude);
                HD5Helper.SetAttribute(_groupId, "UtcOffset", _station.UtcOffset);
                HD5Helper.SetAttribute(_groupId, "Timezone", _station.Timezone);
                HD5Helper.SetAttribute(_groupId, "CreatedDateTime", DateTime.UtcNow.ToString("O"));
            }
        }

        public void CloseFile()
        {
            if (m_FileId != null)
            {
                H5F.close(m_FileId);
                m_FileId = null;
            }
        }

        public bool IsFileOpen()
        {
            if (m_FileId != null)
            {
                return true;
            }

            return false;
        }

        public void WriteAudioSegment(AudioSegment _segment)
        {
            if (m_FileId == null)
            {
                throw new Exception("File not open");
            }


        }
    }
}
