﻿using System;
using System.IO;
using System.Linq;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SidWatch.Library.Delegates;
using SidWatch.Library.Helpers;
using SidWatchLibrary.Objects;

namespace SidWatchAudioLibrary.Workers
{
	public class NAudioRecorderWorker : AbstractAudioRecorder 
	{
		private IWaveIn m_WaveIn;
		private WaveFileWriter m_WaveWriter;
		private MemoryStream m_MemStream;

        public NAudioRecorderWorker(CompletedRecording _complete)
            : base(_complete)
		{
             //Get the desired device
            string deviceName = Config.GetStringValue("AudioDeviceName");

            //Get all devices
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
		    int deviceId = 0;

            //Find right device
            if (devices.Count > 0)
            {
                bool foundDevice = false;
                for (int i = 0; i < WaveIn.DeviceCount; i++)
                {
                    var device = devices[i];
                    var capabilities = WaveIn.GetCapabilities(i);

                    if (deviceName.Equals(device.FriendlyName))
                    {
                        deviceId = i;
                        device.AudioEndpointVolume.Mute = false;
                        foundDevice = true;
                    }
                }

                if (foundDevice)
                {
                    Console.WriteLine("Using requested device");
                }
                else
                {
                    Console.WriteLine("Using default device");
                }

                m_WaveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(SamplesPerSecond, BitsPerSample, Channels),
                    BufferMilliseconds = 100,
                    DeviceNumber = deviceId
                };

                var waveIn = (WaveInEvent) m_WaveIn;
                waveIn.DeviceNumber = deviceId;

                m_WaveIn.RecordingStopped += RecordingStopped;
                m_WaveIn.DataAvailable += DataAvailable;
            }
		}

	    public override void DoWork()
	    {
            //Dispose of old
	        if (m_WaveWriter != null)
	        {
	            m_WaveWriter.Dispose();
	        }

            if (m_MemStream != null)
            {
                m_MemStream.Dispose();
            }
            
            //Create new
            m_MemStream = new MemoryStream();
	        m_WaveWriter = new WaveFileWriter(m_MemStream, m_WaveIn.WaveFormat);

	        DoneRecording = false;
	        FirstData = true;

	        m_WaveIn.StartRecording();
	    }

	    private void DataAvailable(object sender, WaveInEventArgs e)
        {
            if (FirstData)
            {
                SetStartEnd();
                FirstData = false;
            }
            else
            {
                m_WaveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                m_WaveWriter.Flush();

                if (m_WaveWriter.Position >= DesiredBytes)
                {
                    m_WaveIn.StopRecording();
                    DoneRecording = true;
                }                
            }
        }

		private void RecordingStopped(object sender, StoppedEventArgs e)
		{
            int desiredSamples = (RecordForMilliseconds / 1000) * SamplesPerSecond;

		    m_MemStream.Position = 0;

            WaveFileReader reader = new WaveFileReader(m_MemStream);
		    LogFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, reader.WaveFormat.Channels);

		    int channels = reader.WaveFormat.Channels;

		    int frames = Convert.ToInt32(reader.SampleCount/channels);

		    TraceHelper.Verbose(string.Format("Found {0} samples, using {1}", frames, desiredSamples));

		    if (frames > desiredSamples)
		    {
		        frames = desiredSamples;
		    }

		    float[] channel1 = new float[desiredSamples];            

            for (int i = 0; i < frames; i++)
            {
                float[] frame = reader.ReadNextSampleFrame();

                //Other array elements are channels that are not of concern.
                channel1[i] = frame[0];
            }

		    Segment = new AudioSegment
		    {
		        Channels = channels,
		        StartTime = StartTime,
		        SamplesPerSeconds = SamplesPerSecond,
		        Channel1 = channel1
		    };

		    SendData(Segment);
		}

		public override void Dispose ()
		{
			if (m_WaveIn != null) {
				m_WaveIn.Dispose ();
				m_WaveIn = null;
			}

			if (m_MemStream != null) {
				m_MemStream.Dispose ();
				m_MemStream = null;
			}
		}

	    public override void EnumDevicesToConsole()
	    {
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();

	        int i = 1;
	        foreach (var mmDevice in devices)
	        {
	            Console.WriteLine("{0} - {1}", 1, mmDevice.FriendlyName);
	            i++;
	        }
	    }
	}
}



