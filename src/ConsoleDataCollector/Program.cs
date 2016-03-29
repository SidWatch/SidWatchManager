using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using SidWatchAudioLibrary.Factory;
using SidWatchAudioLibrary.Helpers;
using SidWatchAudioLibrary.Workers;
using SidWatchLibrary.Objects;
using TreeGecko.Library.Common.Helpers;

namespace ConsoleDataCollector
{
    class Program
    {
        public static bool Recording = false;
        private static AbstractAudioRecorder m_AudioRecorder;

        static void Main(string[] args)
        {
            TraceFileHelper.SetupLogging();
            Trace.Listeners.Add(new ConsoleTraceListener());

            m_AudioRecorder = AudioRecorderFactory.GetAudioRecorder(CompletedRecording);

            if (args != null && args.Length > 0)
            {
                if (args[0].Equals("enumdevices"))
                {
                    EnumDevices();
                }
            }
            else
            {
                CollectData();
            }

            TraceFileHelper.TearDownLogging();
        }

        public static void EnumDevices()
        {
            m_AudioRecorder.EnumDevicesToConsole();
        }

        public static void CollectData()
        {
            bool stop = false;
            DateTime nextAudioRecordTime = DateTime.UtcNow;

            do
            {
                var now = DateTime.UtcNow;

                if (now > nextAudioRecordTime
                    && !Recording)
                {
                    Recording = true;

                    m_AudioRecorder.Start();

                    nextAudioRecordTime = now.AddSeconds(5);
                }

                Thread.Sleep(10);

            } while (!stop);
        }

        public static void CompletedRecording(AudioSegment _segment)
        {
            Recording = false;
            string path = Config.GetSettingValue("OutputPath");
            string filename = _segment.StartTime.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".json";
            string pathfilename = Path.Combine(path, filename);


            double minValue;
            double maxValue;

            AudioHelper.GetMinMax(_segment.Channel1, out minValue, out maxValue);

            TraceFileHelper.Verbose(string.Format("Found {0} samples for channel1", _segment.Channel1.Count));
            TraceFileHelper.Verbose(string.Format("Minimum Value Find - {0}", minValue));
            TraceFileHelper.Verbose(string.Format("Maximum Value Find - {0}", maxValue));

            if (_segment.Channels > 1)
            {
                AudioHelper.GetMinMax(_segment.Channel2, out minValue, out maxValue);

                TraceFileHelper.Verbose(string.Format("Found {0} samples for channel2", _segment.Channel2.Count));
                TraceFileHelper.Verbose(string.Format("Minimum Value Find - {0}", minValue));
                TraceFileHelper.Verbose(string.Format("Maximum Value Find - {0}", maxValue));
            }


            TraceFileHelper.Info(string.Format("Writing File {0}", pathfilename));
            string json = JsonConvert.SerializeObject(_segment);
            File.WriteAllText(pathfilename, json);

            TraceFileHelper.Info(string.Format("Second of audio received ({0})", _segment.StartTime.ToString("O")));
        }
    }
}
