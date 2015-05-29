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

		public static PlotModel GetComparisonGraph(double[] _values1, double[] _values2)
		{
			var model = new PlotModel { Title = "Delta" };

			List<DataPoint> points1 = new List<DataPoint> ();
			for (int i = 0; i < _values1.Length; i++) {
				DataPoint dp = new DataPoint (i, _values1[i]);
				points1.Add(dp);
			}

			var lineSeries1 = new LineSeries ();
			lineSeries1.ItemsSource = points1;

			List<DataPoint> points2 = new List<DataPoint> ();
			for (int i = 0; i < _values2.Length; i++) {
				DataPoint dp = new DataPoint (i, _values2[i]);
				points2.Add(dp);
			}

			var lineSeries2 = new LineSeries ();
			lineSeries2.ItemsSource = points2;

			model.Series.Add(lineSeries1);
			model.Series.Add(lineSeries2);

			LogarithmicAxis yAxis = new LogarithmicAxis();
			//LinearAxis yAxis = new LinearAxis();
			yAxis.Position = AxisPosition.Left;
			yAxis.Maximum = 3000;
			yAxis.Minimum = 0;
			yAxis.Title = "Difference";

			model.Axes.Add (yAxis);

			LinearAxis xAxis = new LinearAxis ();
			xAxis.Position = AxisPosition.Bottom;
			xAxis.Minimum = 0;
			xAxis.Maximum = 512;
			xAxis.Title = "Frequency Bin";
			model.Axes.Add (xAxis);

			return model;
		}
	}
}

