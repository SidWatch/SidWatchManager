using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SidWatchAudioLibrary.Workers;
using SidWatchLibrary.Delegates;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Workers;

namespace ConsoleDataCollector
{
    class Program
    {
        public static bool Recording = false;

        static void Main(string[] args)
        {
            bool stop = false;
            DateTime nextAudioRecordTime = DateTime.UtcNow;
            var audioWorker = new AccordAudioRecorderWorker(CompletedRecording);

            do
            {
                var now = DateTime.UtcNow;

                if (now > nextAudioRecordTime
                    && !Recording)
                {
                    Recording = true;

                    audioWorker.Start();

                    nextAudioRecordTime = now.AddSeconds(5);
                }

                Thread.Sleep(100);

            } while (!stop);

        }

        public static void CompletedRecording(AudioSample _sample)
        {

            Recording = false;
            Console.WriteLine("Second of audio received ({0})", _sample.StartTime.ToString("O"));
        }
    }
}
