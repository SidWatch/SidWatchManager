using System;
using Nancy.Hosting.Self;
using SidWatch.Library.Helpers;

namespace SidWatchLibrary.Workers
{
	public class NancyWorker : AbstractWorker
	{
		public override void Dispose ()
		{
			if (m_Host != null) {
				m_Host.Stop ();
				m_Host.Dispose ();			
			}
		}
			
		private NancyHost m_Host;

		public int Port { get; private set; }

		public NancyWorker () 
		{
			Port = Config.GetIntValue ("WebServerPort", 8500);
		}

		public override void DoWork ()
		{
			string url = string.Format ("http://localhost:{0}", Port);
			m_Host = new NancyHost (new Uri (url));
			m_Host.Start ();
		}

		public void Stop()
		{
			if (m_Host != null) {
				m_Host.Stop ();
			}
		}
	}
}

