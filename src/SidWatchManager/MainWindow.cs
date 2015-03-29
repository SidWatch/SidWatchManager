using System;
using Gtk;
using System.Drawing;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.GtkSharp;
using OxyPlot.Series;
using SidWatchLibrary.Helpers;
using SidWatchLibrary.Objects;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{
	private OxyPlot.GtkSharp.PlotView m_PlotView;
	private Button m_Record;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build();

		Fixed fixedLayout = new Fixed();
		Add (fixedLayout);

		m_PlotView = new PlotView ();
		m_PlotView.Name = "pvMain";
		m_PlotView.SetSizeRequest(400, 300);

		string audioFileName = @"/FileSync/Source/Other/SidWatch/pyPSD/data/20141014_150150_042628.txt";
		List<AudioReading> audioReadings = FileHelper.ReadAudioFile(audioFileName);

		string psdFileName = @"/FileSync/Source/Other/SidWatch/pyPSD/data/output.txt";
		List<PowerDensityReading> psdReadings = FileHelper.ReadPSDFile(psdFileName);

		m_PlotView.Model = PlotModelHelper.GetPowerSpectrumDensityGraph(psdReadings);

		fixedLayout.Put(m_PlotView, 10, 10);

		m_Record = new Button();
		m_Record.Name = "btnRecord";
		m_Record.Clicked += Record;
		m_Record.SetSizeRequest (100, 20);
		m_Record.Label = "Record";
		m_Record.TooltipText = "Record one second of audio";
		fixedLayout.Put (m_Record, 10, 320);

		ShowAll ();
	}

	private void Record(object sender, EventArgs e)
	{
		
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
