using System;
using SidWatch.Library.Delegates;
using SidWatch.Library.Helpers;
using SidWatchAudioLibrary.Workers;

namespace SidWatchAudioLibrary.Factory
{
    public static class AudioRecorderFactory
    {
        public static AbstractAudioRecorder GetAudioRecorder(CompletedRecording _completedRecording)
        {
            string type = Config.GetStringValue("AudioProvider", "NAudio");

            //TraceHelper.Info(string.Format("Using audio recorder - {0}", type));

            switch (type)
            {
                case "NAudio":
                    return new NAudioRecorderWorker(_completedRecording);
                //case "CSCore":
                //    return new CSCoreAudioRecorderWorker(_completedRecording);
                //case "Accord":
                //    return new AccordAudioRecorderWorker(_completedRecording);
            }

            throw new ArgumentOutOfRangeException("Unknown audio type");
        }
    }
}
