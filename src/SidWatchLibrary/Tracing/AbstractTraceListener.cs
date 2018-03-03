using System.Diagnostics;

namespace TreeGecko.Library.Common.Tracing
{
    public class AbstractTraceListener : DefaultTraceListener
    {
        private string mListenerName = "ListenerName";

        public AbstractTraceListener(string listenerName)
        {
            mListenerName = listenerName;
        }

        public string ListenerName
        {
            get { return mListenerName; }
        }

        public override void WriteLine(string _text)
        {
            base.WriteLine(_text);
        }

        public override void Write(string _text)
        {
            base.Write(_text);
        }
    }
}