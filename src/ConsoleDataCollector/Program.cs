using System;
using System.Diagnostics;
using System.Threading;
using SidWatch.Collection.Library.Managers;
using SidWatch.Library.Helpers;

namespace ConsoleDataCollector
{
    class Program
    {
        private static CollectionManager m_CollectionManager;

        static void Main(string[] _args)
        {
            TraceHelper.SetupLogging();
            Trace.Listeners.Add(new ConsoleTraceListener());

            m_CollectionManager = new CollectionManager();
            m_CollectionManager.Start();

            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Modifiers.HasFlag(ConsoleModifiers.Control)
                    && key.Key == ConsoleKey.X)
                {
                    Console.WriteLine("Stopping");
                    m_CollectionManager.Stop();
                }

                Thread.Sleep(100);
            } while (m_CollectionManager.Running);
           
            TraceHelper.TearDownLogging();
        }
    }
}
