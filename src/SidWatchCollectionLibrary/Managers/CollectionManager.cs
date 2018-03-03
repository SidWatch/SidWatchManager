using System;
using System.IO;
using System.Threading;
using Nancy.Hosting.Self;
using SidWatch.Collection.Library.Writers;
using SidWatch.Library.Calculation;
using SidWatch.Library.Helpers;
using SidWatchAudioLibrary.Factory;
using SidWatchAudioLibrary.Helpers;
using SidWatchAudioLibrary.Workers;
using SidWatchCollectionLibrary;
using SidWatchCollectionLibrary.Api;
using SidWatchLibrary.Objects;

namespace SidWatch.Collection.Library.Managers
{
    public class CollectionManager
    {
        private readonly object m_Lock = new object();
        private bool m_Stop = false;
        private Thread m_RunThread;
        private readonly AbstractAudioRecorder m_AudioRecorder;
        private NancyHost m_ApiHost;
        private Station m_Station;
        private HourlyWriter m_Writer;

        public CollectionManager()
        {
            m_AudioRecorder = AudioRecorderFactory.GetAudioRecorder(CompletedRecording);
            RecordEveryMilliseconds = Config.GetIntValue("RecordEveryMilliseconds", 5000);
            RecordForMilliseconds = Config.GetIntValue("RecordForMilliseconds", 1000);
            ApiPort = Config.GetIntValue("ApiPort", 8080);
            EnableApi = Config.GetBooleanValue("EnableApi", true);

            //TODO - LOAD from config
            m_Station = new Station
            {
                StationName = "K10001",
                Latitude = 39,
                Longitude = -104,
                MonitorId = "101",
                Timezone = "MST",
                UtcOffset = -6
            };


            //TODO load Station
            
            if (RecordEveryMilliseconds < RecordForMilliseconds)
            {
                RecordEveryMilliseconds = RecordForMilliseconds + 500;
            }

            if (EnableApi)
            {
                m_ApiHost = new NancyHost(new Uri(string.Format("http://localhost:{0}", ApiPort)));
            }
        }

        public bool Running { get; private set; }
        public bool Recording { get; private set; }
        public int RecordEveryMilliseconds { get; private set; }
        public int RecordForMilliseconds { get; private set; }

        public int ApiPort { get; private set; }
        public bool EnableApi { get; private set; }

        public void Start()
        {
            lock (m_Lock)
            {
                if (!Running)
                {
                    m_Stop = false;
                    Running = true;

                    m_Writer = new HourlyWriter(m_Station);

                    //Start Api
                    if (EnableApi)
                    {
                        HostConfiguration config = new HostConfiguration
                        {
                            UrlReservations = new UrlReservations {CreateAutomatically = true, User = "Everyone"}
                        };

                        m_ApiHost = new NancyHost(
                            new Uri(string.Format("http://localhost:{0}", ApiPort))
                            , new HostingBootstrapper()
                            , config);

                        m_ApiHost.Start();
                    }

                    //Start Audio Collection Thread
                    m_RunThread = new Thread(RunThread);
                    m_RunThread.Start();
                }
            }
        }

        public void RunThread()
        {
            DateTime nextAudioRecordTime = DateTime.UtcNow;

            do
            {
                var now = DateTime.UtcNow;

                if (now > nextAudioRecordTime
                    && !Recording)
                {
                    Recording = true;

                    m_AudioRecorder.Start();

                    nextAudioRecordTime = now.AddMilliseconds(RecordEveryMilliseconds);
                }

                Thread.Sleep(10);

            } while (!m_Stop);

            m_Writer?.Dispose();

            Running = false;
        }

        public void Stop()
        {
            m_Stop = true;

            if (EnableApi 
                && m_ApiHost != null)
            {
                m_ApiHost.Stop();
                m_ApiHost.Dispose();
                m_ApiHost = null;
            }
        }

        public void CompletedRecording(AudioSegment _segment)
        {
            Recording = false;
            string path = Config.GetStringValue("OutputPath");
            string filename = _segment.StartTime.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".json";
            string pathfilename = Path.Combine(path, filename);
            
            float minValue;
            float maxValue;

            AudioHelper.GetMinMax(_segment.Channel1, out minValue, out maxValue);

            TraceHelper.Verbose(string.Format("Found {0} samples for channel1", _segment.Channel1.Length));
            TraceHelper.Verbose(string.Format("Minimum Value Find - {0}", minValue));
            TraceHelper.Verbose(string.Format("Maximum Value Find - {0}", maxValue));

            _segment.PowerSpectrum = Signal.CalculatePowerSpectralDensity(_segment.Channel1, _segment.SamplesPerSeconds);
            
            DataCache.GetInstance().LastAudioSegment = _segment;
            
            m_Writer.WriteAudioSegment(_segment);

            TraceHelper.Info(string.Format("Second of audio received ({0})", _segment.StartTime.ToString("O")));
        }
    }
}
