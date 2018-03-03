using System;
using System.Diagnostics;
using System.Text;
using TreeGecko.Library.Common.Tracing;

namespace SidWatch.Library.Helpers
{
    public static class TraceHelper
    {
        private static TraceLevel m_MinTraceLevel = TraceLevel.Verbose;
        private static string m_TraceFolder = "";

        public static void FlushMessages()
        {
            Trace.Flush();
        }

        public static string TraceFolder
        {
            get { return m_TraceFolder; }
            set { m_TraceFolder = value; }
        }

        public static TraceLevel MinTraceLevel
        {
            get { return m_MinTraceLevel; }
            set { m_MinTraceLevel = value; }
        }

        /// <summary>
        /// Write out Error-level message
        /// </summary>
        /// <param name="_message">Message to write</param>
        /// <param name="_parameters"></param>
        public static void Error(string _message, params object[] _parameters)
        {
            WriteLineLevel(_message, "ERR", TraceLevel.Error, _parameters);
        }

        /// <summary>
        /// Write out Warning-level message
        /// </summary>
        /// <param name="_message">Message to write</param>
        /// <param name="_parameters"></param>
        public static void Warning(string _message, params object[] _parameters)
        {
            WriteLineLevel(_message, "WRN", TraceLevel.Warning, _parameters);
        }

        /// <summary>
        /// Write out Exception-level message
        /// </summary>
        /// <param name="_message">Message to write</param>
        /// <param name="_parameters"></param>
        public static void Exception(string _message, params object[] _parameters)
        {
            WriteLineLevel(_message, "EXP", TraceLevel.Error, _parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ex"></param>
        /// <param name="_parameters"></param>
        public static void Exception(Exception _ex, params object[] _parameters)
        {
            WriteLineLevel(_ex.ToString(), "EXP", TraceLevel.Error, _parameters);
        }

        /// <summary>
        /// Write out Info-level message
        /// </summary>
        /// <param name="_message">Message to write</param>
        /// <param name="_parameters"></param>
        public static void Info(string _message, params object[] _parameters)
        {
            WriteLineLevel(_message, "INF", TraceLevel.Info, _parameters);
        }

        /// <summary>
        /// Write out Verbose-level message
        /// </summary>
        /// <param name="_message">Message to write</param>
        /// <param name="_parameters"></param>
        public static void Verbose(string _message, params object[] _parameters)
        {
            WriteLineLevel(_message, "VER", TraceLevel.Verbose, _parameters);
        }

        private static void WriteLineLevel(string _message,
            string _category,
            TraceLevel _traceLevel,
            params object[] _parameters)
        {
            //Don't log if current TraceLevel too low
            if (_traceLevel > m_MinTraceLevel)
                return;

            if (_message == "")
            {
                Trace.WriteLine("");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(DateTime.Now.ToString("s"));
            sb.Append(" ");

            if (_message.Length < 32000)
            {
                if (_parameters == null
                    || _parameters.Length == 0)
                {
                    sb.Append(_message);
                }
                else
                {
                    sb.AppendFormat(_message, _parameters);
                }
            }
            else
            {
                sb.Append("Partial Message - ");
                if (_parameters == null)
                {
                    sb.Append(_message.Substring(0, 32000));
                }
                else
                {
                    sb.AppendFormat(_message.Substring(0, 32000), _parameters);
                }
            }

            Trace.WriteLine(sb.ToString(), _category);
        }

        public static void SetupLogging()
        {
            m_TraceFolder = Config.GetStringValue("LogFolder", @"\logs\");
            RollingTraceLogListener rtll = new RollingTraceLogListener("RollingTraceLogListener", m_TraceFolder);
            Trace.Listeners.Add(rtll);
        }

        public static void TearDownLogging()
        {
            Trace.Listeners.Clear();
        }
    }
}
