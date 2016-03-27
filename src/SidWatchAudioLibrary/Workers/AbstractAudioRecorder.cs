using System;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Workers;

namespace SidWatchAudioLibrary.Workers
{
    public abstract class AbstractAudioRecorder : AbstractWorker, IAudioRecorderWorker
    {
        public int RecordForTicks { get; set; }
        public int SamplesPerSecond { get; set; }

        public DateTime StartTime { get; protected set; }
        public DateTime EndTime { get; protected set; }
        public AudioSample Sample { get; set; }

    }
}
