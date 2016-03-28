using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SidWatchAudioLibrary.Factory;
using SidWatchAudioLibrary.Workers;
using SidWatchLibrary.Delegates;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Workers;
using TreeGecko.Library.Common.Helpers;

namespace ConsoleDataCollector
{
    class Program
    {
        public static bool Recording = false;
        private static AbstractAudioRecorder m_AudioRecorder;

        static void Main(string[] args)
        {
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

                Thread.Sleep(100);

            } while (!stop);
        }

        public static void CompletedRecording(AudioSegment _segment)
        {
            Recording = false;
            string path = Config.GetSettingValue("OutputPath");
            string filename = _segment.StartTime.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".json";
            string pathfilename = Path.Combine(path, filename);

            Console.WriteLine("Writing File {0}", pathfilename);

            string json = JsonConvert.SerializeObject(_segment);

            File.WriteAllText(pathfilename, json);

            Console.WriteLine("Second of audio received ({0})", _segment.StartTime.ToString("O"));
        }
    }
}
