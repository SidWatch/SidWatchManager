using System;
using System.Collections.Generic;
using System.Threading;
using SidWatchLibrary.Objects;

namespace SidWatchLibrary.Workers
{
	public class PSDWorker : AbstractWorker
	{
		private List<AudioReading> m_Readings;

		public PSDWorker(List<AudioReading> _readings)
		{
			m_Readings = _readings;
		}

		public override void DoWork ()
		{
			
		}

		public override void Dispose ()
		{
			m_Readings = null;
		}

		public List<PowerDensityReading> PowerDensityReadings { get; private set; }
	}
}

