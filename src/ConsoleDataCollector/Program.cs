using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Workers;

namespace ConsoleDataCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            bool stop = false;
            DateTime nextAudioRecordTime = DateTime.UtcNow;

            do
            {
                var now = DateTime.UtcNow;

                if (now > nextAudioRecordTime)
                {
                    var audioWorker = new AudioRecorderWorker(CompletedRecording);
                    audioWorker.Start();

                    nextAudioRecordTime = now.AddSeconds(5);
                }

                Thread.Sleep(100);

            } while (!stop);

        }

        public static void CompletedRecording(AudioSample _sample)
        {

            Console.WriteLine("Second of audio received ({0})", _sample.StartTime.ToString("O"));
        }
    }
}
