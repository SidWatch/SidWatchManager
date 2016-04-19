using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SidWatchLibrary.Objects;

namespace SidWatch.Library.Helpers
{
	public static class FileHelper
	{
		public static List<AudioReading> ReadAudioFile(string _fileName)
		{
			string[] lines = File.ReadAllLines(_fileName);
			double deltaTime = 1.0 / lines.Length;
			double overallTime = 0;

			List<AudioReading> data = new List<AudioReading> ();

			foreach (var line in lines) {
				AudioReading reading = new AudioReading();

				string[] items = line.Split ("|".ToCharArray());

				double time = 0;
				double value;

				if (items.Length == 1) {
					if (Double.TryParse (items[0], out value)) {
						reading.Value = value;
						reading.Time = overallTime;
					}

					overallTime += deltaTime;
				} else if (items.Length == 2) {
					if (Double.TryParse (items[0], out time)) {
						reading.Time = time;
					}

					if (Double.TryParse (items[1], out value)) {
						reading.Value = value;
					}
				}

				data.Add (reading);
			}

			return data;
		}

		public static void WriteAudioFile(string _fileName, List<AudioReading> _data)
		{
			StringBuilder sb = new StringBuilder ();

			foreach (var item in _data) {
				string line = string.Format ("{0}|{1}", item.Time, item.Value);
				sb.AppendLine (line);
			}

			File.WriteAllText (_fileName, sb.ToString());
		}

		public static List<PowerDensityReading> ReadPSDFile(string _fileName)
		{
			string[] lines = File.ReadAllLines(_fileName);

			List<PowerDensityReading> data = new List<PowerDensityReading> ();

			foreach (var line in lines) {
				PowerDensityReading reading = new PowerDensityReading();

				string[] items = line.Split ("|\t".ToCharArray());

				double time;
				double value;

				if (items.Length == 2) {
					if (Double.TryParse (items[0], out time)) {
						reading.Frequency = time;
					}

					if (Double.TryParse (items[1], out value)) {
						reading.Value = value;
					}
				}

				data.Add (reading);
			}

			return data;
		}

		public static void WritePSDFile(string _fileName, List<PowerDensityReading> _data)
		{
			StringBuilder sb = new StringBuilder ();

			foreach (var item in _data) {
				string line = string.Format ("{0}|{1}", item.Frequency, item.Value);
				sb.AppendLine (line);
			}

			File.WriteAllText (_fileName, sb.ToString());
		}
	}
}

