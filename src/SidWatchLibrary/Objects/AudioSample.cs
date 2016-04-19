using System;

namespace SidWatchLibrary.Objects
{
    public class AudioSegment
    {
        public Guid Id { get; set; }
        public string Station { get; set; }
        public int Channels { get; set; }
        public DateTime StartTime { get; set; }
        public int SamplesPerSeconds { get; set; }

        public float[] Channel1 { get; set; }
        public float[,] PowerSpectrum { get; set; }
    }
}
