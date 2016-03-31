using System.Diagnostics;
using System.Threading;
using SidWatchCollectionLibrary.Managers;
using TreeGecko.Library.Common.Helpers;

namespace ConsoleDataCollector
{
    class Program
    {
        private static CollectionManager m_CollectionManager;

        static void Main(string[] _args)
        {
            TraceFileHelper.SetupLogging();
            Trace.Listeners.Add(new ConsoleTraceListener());

            m_CollectionManager = new CollectionManager();
            m_CollectionManager.Start();

            do
            {
                Thread.Sleep(100);
            } while (true);
           
            TraceFileHelper.TearDownLogging();
        }
    }
}
