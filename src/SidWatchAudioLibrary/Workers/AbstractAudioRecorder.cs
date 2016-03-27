using System;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Workers;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchAudioLibrary.Workers
{
    public abstract class AbstractAudioRecorder : AbstractWorker, IAudioRecorderWorker
    {
        protected AbstractAudioRecorder()
        {
            RecordForTicks = Config.GetIntValue("RecordForTicks", 1000);
            SamplesPerSecond = Config.GetIntValue("SamplesPerSecond", 96000);
            BitsPerSample = Config.GetIntValue("BitsPerSample", 32);
        }

        public int RecordForTicks { get; set; }
        public int SamplesPerSecond { get; set; }
        public int BitsPerSample { get; set; }

        public DateTime StartTime { get; protected set; }
        public DateTime EndTime { get; protected set; }
        public AudioSample Sample { get; set; }

        public abstract void EnumDevicesToConsole();

    }
}
