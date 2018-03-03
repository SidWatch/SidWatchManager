using System;
using SidWatch.Library.Helpers;
using SidWatchCollectionLibrary.Writers;
using SidWatchLibrary.Objects;

namespace SidWatch.Collection.Library.Writers
{
    public class HourlyWriter : IDisposable
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
            var writerType = Config.GetStringValue("FileWriter", "HDF5");
            SetOutputFolder(_outputFolder);
            m_Station = _station;

            if (writerType.Equals("HDF5", StringComparison.InvariantCultureIgnoreCase))
            {
                m_Writer = new HdfWriter();
            }
        }

        private void SetOutputFolder(string _outputFolder)
        {
            if (_outputFolder == null)
            {
                m_OutputFolder = Config.GetStringValue("OutputFolder", @"c:\temp\");
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

        public void Dispose()
        {
            if (m_Writer != null)
            {
                m_Writer.CloseFile();
            }
        }
    }
}
