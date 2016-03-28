using System;
using System.IO;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Workers;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchAudioLibrary.Workers
{
    public abstract class AbstractAudioRecorder : AbstractWorker, IAudioRecorderWorker
    {
        private MemoryStream m_MemoryStream;

        protected AbstractAudioRecorder()
        {
            RecordForTicks = Config.GetIntValue("RecordForTicks", 1000);
            SamplesPerSecond = Config.GetIntValue("SamplesPerSecond", 96000);
            BitsPerSample = Config.GetIntValue("BitsPerSample", 32);
        }

        public int RecordForTicks { get; set; }
        public int SamplesPerSecond { get; set; }
        public int BitsPerSample { get; set; }
        public bool FirstData { get; protected set; }

        public DateTime StartTime { get; protected set; }
        public DateTime EndTime { get; protected set; }
        public AudioSegment Segment { get; set; }


        public abstract void EnumDevicesToConsole();

        public virtual void Start()
        {
            FirstData = false;
            if (m_MemoryStream != null)
            {
                m_MemoryStream.Dispose();
            }
            m_MemoryStream = new MemoryStream();
        }

        public void BytesReceived(byte[] _data)
        {
            if (FirstData)
            {
                StartTime = DateTime.Now;
                EndTime = StartTime.AddTicks(RecordForTicks);
                FirstData = false;
            }
            else
            {
                
            }
        }
    }
}
