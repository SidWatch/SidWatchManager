using System;
using System.Threading;

namespace SidWatchLibrary.Interfaces
{
	public interface IWorker
	{
		Thread Start();
	}
}

