using System;
using SidWatch.Library.Delegates;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Workers;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchAudioLibrary.Workers
{
    public abstract class AbstractAudioRecorder : AbstractWorker, IAudioRecorderWorker
    {
        protected AbstractAudioRecorder(CompletedRecording _complete)
        {
            RecordForMilliseconds = Config.GetIntValue("RecordForMilliseconds", 1000);
            SamplesPerSecond = Config.GetIntValue("SamplesPerSecond", 96000);
            BitsPerSample = Config.GetIntValue("BitsPerSample", 32);
            Channels = Config.GetIntValue("Channels", 1);
            ChannelToKeep = Config.GetIntValue("ChannelToKeep", 0);

            DesiredSamples = (RecordForMilliseconds/1000)*SamplesPerSecond;
            DesiredBytes = DesiredSamples*BitsPerSample/8;

            CompletedRecording = _complete;
        }

        public CompletedRecording CompletedRecording { get; private set; }

        public long DesiredSamples { get; private set; }
        public long DesiredBytes { get; private set; }
        public int Channels { get; private set; }
        public int ChannelToKeep { get; private set; }
        public int RecordForMilliseconds { get; private set; }
        public int SamplesPerSecond { get; private set; }
        public int BitsPerSample { get; private set; }
        public bool FirstData { get; protected set; }
        public bool DoneRecording { get; protected set; }

        public DateTime StartTime { get; protected set; }
        public DateTime EndTime { get; protected set; }
        public AudioSegment Segment { get; set; }

        public abstract void EnumDevicesToConsole();

        protected void LogFormat(int _sampleRate, int _bitsPerSample, int _channels)
        {
            TraceFileHelper.Verbose(string.Format("Output - Samples Per Second - {0} ", _sampleRate));
            TraceFileHelper.Verbose(string.Format("       - Bits per Sample    - {0} ", _bitsPerSample));
            TraceFileHelper.Verbose(string.Format("       - Channels           - {0} ", _channels));
        }

        protected void SetStartEnd()
        {
            DateTime now = DateTime.UtcNow;
            StartTime = now;
            EndTime = now.AddMilliseconds(RecordForMilliseconds);
        }

        protected void SendData(AudioSegment _segment)
        {
            FireComplete();

            if (CompletedRecording != null)
            {
                CompletedRecording(_segment);
            }
        }
    }
}
