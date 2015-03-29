using System;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using SidWatchLibrary.Objects;

namespace SidWatchLibrary.Helpers
{
	public static class PlotModelHelper
	{
		public static PlotModel GetSoundDiagramGraph(List<AudioReading> _readings)
		{
			var model = new PlotModel { Title = "Sound Diagram" };

			List<DataPoint> points = new List<DataPoint> ();
			foreach (var item in _readings) {
				DataPoint dp = new DataPoint (item.Time, item.Value);
				points.Add (dp);
			}

			var lineSeries = new LineSeries ();
			lineSeries.ItemsSource = points;
			lineSeries.Decimator = Decimator.Decimate;

			model.Series.Add (lineSeries);

			
			return model;
		}

		public static PlotModel GetPowerSpectrumDensityGraph(List<PowerDensityReading> _readings)
		{
			var model = new PlotModel { Title = "Power Spectrum Density" };

			List<DataPoint> points = new List<DataPoint> ();
			foreach (var item in _readings) {
				DataPoint dp = new DataPoint (item.Frequency, item.Value);
				points.Add (dp);
			}

			var lineSeries = new LineSeries ();
			lineSeries.ItemsSource = points;
				
			model.Series.Add(lineSeries);

			LogarithmicAxis yAxis = new LogarithmicAxis ();
			yAxis.Position = AxisPosition.Left;
			yAxis.Maximum = 100000;
			yAxis.Minimum = 0;
			yAxis.Title = "Power";

			model.Axes.Add (yAxis);

			LinearAxis xAxis = new LinearAxis ();
			xAxis.Position = AxisPosition.Bottom;
			xAxis.Minimum = 0;
			xAxis.Maximum = 48000;
			xAxis.Title = "Frequency (Htz)";
			model.Axes.Add (xAxis);

			return model;
		}
	}
}

