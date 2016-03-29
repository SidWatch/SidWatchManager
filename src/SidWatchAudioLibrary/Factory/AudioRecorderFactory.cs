using System;
using SidWatchAudioLibrary.Workers;
using SidWatchLibrary.Delegates;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchAudioLibrary.Factory
{
    public static class AudioRecorderFactory
    {
        public static AbstractAudioRecorder GetAudioRecorder(CompletedRecording _completedRecording)
        {
            string type = Config.GetSettingValue("AudioProvider", "NAudio");

            TraceFileHelper.Info(string.Format("Using audio recorder - {0}", type));

            switch (type)
            {
                case "NAudio":
                    return new NAudioRecorderWorker(_completedRecording);
                case "CSCore":
                    return new CSCoreAudioRecorderWorker(_completedRecording);
                case "Accord":
                    return new AccordAudioRecorderWorker(_completedRecording);
            }

            throw new ArgumentOutOfRangeException("Unknown audio type");
        }
    }
}
