using System;
using System.Threading;
using SidWatch.Library.Delegates;
using SidWatch.Library.Interfaces;

namespace SidWatchLibrary.Workers
{
	public abstract class AbstractWorker : IWorker, IDisposable
	{
		private Thread m_Thread;

		public abstract void Dispose ();
		public abstract void DoWork();

		public CompletedWork CompletedWork { get; set;} 

		public Thread Start()
		{
			m_Thread = new Thread (DoWork);
			m_Thread.Start();
			return m_Thread;
		}

		protected void FireComplete()
		{
			if (CompletedWork != null)
			{
				CompletedWork (this);
			}
		}
	}
}

