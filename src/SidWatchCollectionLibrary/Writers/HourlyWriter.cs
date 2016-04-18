using System;
using System.IO;
using SidWatchLibrary.Objects;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchCollectionLibrary.Writers
{
    public class HourlyWriter
    {
        private readonly IFileWriter m_Writer;
        private readonly Station m_Station;
        private int m_CurrentHour;
        private string m_OutputFolder;

        public HourlyWriter(
            IFileWriter _writer,
            Station _station,
            string _outputFolder = null)
        {
            m_Writer = _writer;
            m_Station = _station;
            SetOutputFolder(_outputFolder);
        }

        public HourlyWriter(
            Station _station,
            string _outputFolder = null)
        {
            var writerType = Config.GetSettingValue("FileWriter", "HDF5");
            SetOutputFolder(_outputFolder);
            m_Station = _station;

            if (writerType.Equals("HDF5", StringComparison.InvariantCultureIgnoreCase))
            {
                m_Writer = new HdfWriter();
            }
            else
            {
                //m_Writer = new FitsWriter();
            }
        }

        private void SetOutputFolder(string _outputFolder)
        {
            if (_outputFolder == null)
            {
                m_OutputFolder = Config.GetSettingValue("OutputFolder", @"c:\temp\");
            }
            else
            {
                m_OutputFolder = _outputFolder;
            }

            DirectoryHelper.BuildFolderIfMissing(m_OutputFolder);
        }

        public void WriteAudioSegment(AudioSegment _segment)
        {
            CheckFile();

            m_Writer.WriteAudioSegment(_segment);
        }

        public void CheckFile()
        {
            DateTime now = DateTime.UtcNow;

            if (now.Hour != m_CurrentHour)
            {
                m_Writer.CloseFile();
                m_CurrentHour = now.Hour;
            }

            if (!m_Writer.IsFileOpen())
            {
                string fileSuffix = now.ToString("yyyyMMdd_HH");
                m_Writer.OpenFile(m_OutputFolder, m_Station, fileSuffix);
            }
        }
    }
}
