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

		VBox verticalBox = new VBox (false, 5);
		Add (verticalBox);

		HBox graphBox = new HBox ();
		verticalBox.PackStart(graphBox, true, true, 5);

		m_PlotView = new PlotView ();
		m_PlotView.Name = "pvMain";
		m_PlotView.SetSizeRequest(400, 300);


		string audioFileName = @"/FileSync/Source/Other/SidWatch/pyPSD/data/20141014_150150_042628.txt";
		List<AudioReading> audioReadings = FileHelper.ReadAudioFile(audioFileName);

		m_PlotView.Model = PlotModelHelper.GetSoundDiagramGraph(audioReadings);

		string psdFileName = @"/FileSync/Source/Other/SidWatch/pyPSD/data/output.txt";
		List<PowerDensityReading> psdReadings = FileHelper.ReadPSDFile(psdFileName);

		//m_PlotView.Model = PlotModelHelper.GetPowerSpectrumDensityGraph(psdReadings);

		graphBox.PackStart(m_PlotView, true, true, 5);

		HBox buttonBox = new HBox ();
		verticalBox.PackEnd(buttonBox, false, false, 5);

		m_Record = new Button();
		m_Record.Name = "btnRecord";
		m_Record.Clicked += Record;
		m_Record.SetSizeRequest (100, 20);
		m_Record.Label = "Record";
		m_Record.TooltipText = "Record one second of audio";
		buttonBox.PackEnd (m_Record, false, false, 5);

		ShowAll ();
	}

	private void Record(object sender, EventArgs e)
	{
		AudioHelper.RecordAudio(1000, CompletedRecording);
	}

	private void CompletedWorke(List<AudioReading> _readings) {
		
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
