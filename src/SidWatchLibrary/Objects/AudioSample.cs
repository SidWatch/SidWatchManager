﻿using System;
using System.Collections.Generic;

namespace SidWatchLibrary.Objects
{
    public class AudioSegment
    {
        public Guid Id { get; set; }
        public string Station { get; set; }
        public int Channels { get; set; }
        public DateTime StartTime { get; set; }
        public int SamplesPerSeconds { get; set; }
        public List<double> Channel1 { get; set; }
        public List<double> Channel2 { get; set; }
    }
}
