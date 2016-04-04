using System;
using System.IO;
using nom.tam.fits;
using SidWatchLibrary.Objects;

namespace SidWatchCollectionLibrary.Writers
{
    public class FitsWriter : IFileWriter
    {
        private Fits m_FitsFile = null;

        public FitsWriter()
        {
            
        }

        public void OpenFile(string _folder, Station _station, string _fileSuffix)
        {
            if (m_FitsFile != null)
            {
                throw new FileLoadException("File already open.  Close first.");
            }

            m_FitsFile = new Fits();
        }

        public void CloseFile()
        {
            if (m_FitsFile != null)
            {
                m_FitsFile.Close();
                m_FitsFile = null;
            }
        }

        public bool IsFileOpen()
        {
            if (m_FitsFile == null)
            {
                return true;
            }

            return false;
        }

        public void WriteAudioSegment(AudioSegment _segment)
        {
            if (m_FitsFile == null)
            {
                throw new Exception("File is not open");
            }

            
        }

    }
}
