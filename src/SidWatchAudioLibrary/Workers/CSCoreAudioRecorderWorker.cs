﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
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

        public CSCoreAudioRecorderWorker(CompletedRecording _complete) 
            : base(_complete)
		{
            var waveFormat = new WaveFormat(SamplesPerSecond, BitsPerSample, Channels);

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
            if (FirstData)
            {
                //Throwing away first chunk of data
                SetStartEnd();
                FirstData = false;                
            }
            else
            {               
                m_WaveWriter.Write(_e.Data, _e.Offset, _e.ByteCount);

                if (DateTime.UtcNow > EndTime)
                {
                    DoneRecording = true;
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

            DoneRecording = false;
	        FirstData = true;
	        do
	        {
	            Thread.Sleep(10);
            } while (!DoneRecording);

            m_AudioSource.Stop();
            Done();
	    }

		private void Done()
		{
		    m_MemStream.Position = 0;

            WaveFileReader reader = new WaveFileReader(m_MemStream);
		    int channels = reader.WaveFormat.Channels;

            LogFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, reader.WaveFormat.Channels);

		    var sampleSource = reader.ToSampleSource();
		    var sampleAggregator = new SampleSourceBase(sampleSource);

		    Single[] data = new Single[sampleAggregator.Length];
		    sampleAggregator.Read(data, 0, data.Length);

            List<Double> channel1 = new List<double>();
            List<Double> channel2 = new List<double>();

		    int i = 0;
		    do
		    {
                channel1.Add(data[i]);

		        i++;

		        if (channels > 1)
		        {
                    channel2.Add(data[i]);
		            i++;
		        }

		    } while (i < data.Length);
            
		    Segment = new AudioSegment
		    {
                Channels = channels,
		        StartTime = StartTime,
		        SamplesPerSeconds = SamplesPerSecond,
                Channel1 = channel1,
                Channel2 = channel2
		    };

		    SendData(Segment);
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
