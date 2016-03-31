using System;
using System.IO;
using System.Threading;
using Nancy.Hosting.Self;
using Nancy.ViewEngines.SuperSimpleViewEngine;
using Newtonsoft.Json;
using SidWatchAudioLibrary.Factory;
using SidWatchAudioLibrary.Helpers;
using SidWatchAudioLibrary.Workers;
using SidWatchCollectionLibrary.Api;
using SidWatchLibrary.Objects;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchCollectionLibrary.Managers
{
    public class CollectionManager
    {
        private readonly object m_Lock = new object();
        private bool m_Stop = false;
        private Thread m_RunThread;
        private readonly AbstractAudioRecorder m_AudioRecorder;
        private NancyHost m_ApiHost;

        public CollectionManager()
        {
            m_AudioRecorder = AudioRecorderFactory.GetAudioRecorder(CompletedRecording);
            RecordEveryMilliseconds = Config.GetIntValue("RecordEveryMilliseconds", 5000);
            RecordForMilliseconds = Config.GetIntValue("RecordForMilliseconds", 1000);
            ApiPort = Config.GetIntValue("ApiPort", 8080);
            EnableApi = Config.GetBooleanValue("EnableApi", true);
            
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

                    //Start Api
                    if (EnableApi)
                    {
                        HostConfiguration config = new HostConfiguration();
                        config.UrlReservations = new UrlReservations {CreateAutomatically = true, User = "Everyone"};

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

            DataCache.GetInstance().LastAudioSegment = _segment;
            
            TraceFileHelper.Info(string.Format("Writing File {0}", pathfilename));
            string json = JsonConvert.SerializeObject(_segment);
            File.WriteAllText(pathfilename, json);

            TraceFileHelper.Info(string.Format("Second of audio received ({0})", _segment.StartTime.ToString("O")));
        }
    }
}
