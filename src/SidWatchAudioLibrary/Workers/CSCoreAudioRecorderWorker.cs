using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using NAudio.Wave;
using SidWatchLibrary.Delegates;
using SidWatchLibrary.Objects;
using TreeGecko.Library.Common.Helpers;
using WaveFileReader = CSCore.Codecs.WAV.WaveFileReader;
using WaveFormat = CSCore.WaveFormat;

namespace SidWatchAudioLibrary.Workers
{
    public class CSCoreAudioRecorderWorker : AbstractAudioRecorder
    {
	    private WasapiCapture m_AudioSource;
		private MemoryStream m_MemStream;
        private WaveWriter m_WaveWriter;
        private bool m_Done;
        private bool m_FirstData;

        public CSCoreAudioRecorderWorker(CompletedRecording _complete)
		{
            CompletedRecording = _complete;

            var waveFormat = new WaveFormat(SamplesPerSecond, BitsPerSample, 1);

	        string desiredAudioDevice = Config.GetSettingValue("AudioDeviceName");

            MMDevice captureDeviceInfo = null;
            var collection = EnumerateWasapiDevices();

	        if (desiredAudioDevice != null)
	        {	            
	            foreach (var device in collection)
                {
	                if (device.FriendlyName == desiredAudioDevice)
	                {
                        Console.WriteLine("Using specified device - {0}", device);
	                    captureDeviceInfo = device;
	                    break;
	                }	                
	            }
	        }

            m_AudioSource = new WasapiCapture(
                eventSync: false,
                shareMode: AudioClientShareMode.Shared,
                latency:100,
                defaultFormat: waveFormat);
            if (captureDeviceInfo != null)
            {
                m_AudioSource.Device = captureDeviceInfo;
            }

            m_AudioSource.DataAvailable += DataAvailable;
            m_AudioSource.Initialize();
		}

        private void DataAvailable(object _sender, DataAvailableEventArgs _e)
        {
            if (m_FirstData)
            {
                //Throwing away first chunk of data
                StartTime = DateTime.UtcNow;
                EndTime = StartTime.AddTicks(RecordForTicks);
                m_FirstData = false;                
            }
            else
            {               
                m_WaveWriter.Write(_e.Data, _e.Offset, _e.ByteCount);

                if (DateTime.UtcNow > EndTime)
                {
                    m_Done = true;
                }
            }
        }

        public IEnumerable<MMDevice> EnumerateWasapiDevices()
        {
            using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
            {
                return enumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
            }
        }

		public CompletedRecording CompletedRecording { get; private set; }

	    public override void DoWork()
	    {
            //Create local memory stream for this recording
	        m_MemStream = new MemoryStream();
	        m_WaveWriter = new WaveWriter(m_MemStream, m_AudioSource.WaveFormat);

            //Start recording
            m_AudioSource.Start();

            //Set the start time and end time
	        StartTime = DateTime.UtcNow;
	        EndTime = StartTime.AddSeconds(1.1);

	        m_Done = false;
	        m_FirstData = true;
	        do
	        {
	            Thread.Sleep(10);
	        } while (!m_Done);

            m_AudioSource.Stop();
            Done();
	    }

		private void Done()
		{
		    m_MemStream.Position = 0;

            WaveFileReader reader = new WaveFileReader(m_MemStream);
		    int channels = reader.WaveFormat.Channels;

            Console.WriteLine("Output - Samples Per Second - {0} ", reader.WaveFormat.SampleRate);
            Console.WriteLine("       - Bits per Sample    - {0} ", reader.WaveFormat.BitsPerSample);
            Console.WriteLine("       - Channels           - {0} ", channels);


		    var sampleSource = reader.ToSampleSource();
		    var sampleAggregator = new SampleSourceBase(sampleSource);

		    Single[] data = new Single[sampleAggregator.Length];
		    sampleAggregator.Read(data, 0, data.Length);

            List<Double> samples = new List<double>();
		    int i = 0;
		    do
		    {
                samples.Add(data[i]);

		        i++;

		        if (channels > 1)
		        {
                    //Skip every other byte
		            i++;
		        }

		    } while (i < data.Length);
            
		    Segment = new AudioSegment
		    {
		        StartTime = StartTime,
		        SamplesPerSeconds = SamplesPerSecond,
                Samples = samples
		    };

			FireComplete ();

		    if (CompletedRecording != null)
		    {
		        CompletedRecording(Segment);
		    }
		}

		public override void Dispose ()
		{
			if (m_AudioSource != null) {
                m_AudioSource.Dispose();
                m_AudioSource = null;
			}

			if (m_MemStream != null) {
				m_MemStream.Dispose ();
				m_MemStream = null;
			}
		}

        public override void EnumDevicesToConsole()
        {
            var devices = EnumerateWasapiDevices();

            int i = 1;
            foreach (var mmDevice in devices)
            {
                Console.WriteLine("{0} - {1}", i, mmDevice.FriendlyName);
                i++;
            }
        }
    }
}
