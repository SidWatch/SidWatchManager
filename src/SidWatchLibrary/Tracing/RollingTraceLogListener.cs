using System;
using System.IO;
using SidWatch.Library.Helpers;

namespace TreeGecko.Library.Common.Tracing
{
    public class RollingTraceLogListener : AbstractTraceListener
    {
        private DateTime m_CurrentDate;
        private readonly string m_Path = @".\";
        private readonly int m_KeepDays = 7;

        public RollingTraceLogListener(string _listenerName)
            : base(_listenerName)
        {
            DirectoryHelper.BuildFolderIfMissing(m_Path);

            //TEMP - save foldername of this listener to TraceHelper so others can find it if nec.
            TraceHelper.TraceFolder = m_Path;

            m_CurrentDate = DateTime.Now;

            LogFileName = getFileName(m_Path, Config.GetStringValue("ApplicationName"), m_CurrentDate);

            SweepOldFiles();
        }

        public RollingTraceLogListener(string _listenerName, string _folder)
            : base(_listenerName)
        {
            if (_folder != null)
            {
                m_Path = _folder;
            }

            DirectoryHelper.BuildFolderIfMissing(m_Path);

            //TEMP - save foldername of this listener to TraceHelper so others can find it if nec.
            TraceHelper.TraceFolder = m_Path;

            m_CurrentDate = DateTime.Now;

            LogFileName = getFileName(m_Path, Config.GetStringValue("ApplicationName"), m_CurrentDate);

            SweepOldFiles();
        }

        public RollingTraceLogListener(string _listenerName, string _folder, int _keepDays)
            : base(_listenerName)
        {
            m_Path = _folder;
            DirectoryHelper.BuildFolderIfMissing(m_Path);

            //TEMP - save foldername of this listener to TraceHelper so others can find it if nec.
            TraceHelper.TraceFolder = m_Path;

            m_KeepDays = _keepDays;

            m_CurrentDate = DateTime.Now;

            LogFileName = getFileName(m_Path, Config.GetStringValue("ApplicationName"), m_CurrentDate);

            SweepOldFiles();
        }

        public override void WriteLine(string _text)
        {
            CheckDate();

            base.WriteLine(_text);
        }

        public override void Write(string _text)
        {
            CheckDate();

            base.Write(_text);
        }

        private string getFileName(string _path, string _name, DateTime _currentDate)
        {
            return Path.Combine(_path, _name + "_" + _currentDate.ToString("yyyyMMdd") + ".log");
        }

        private void CheckDate()
        {
            if (m_CurrentDate.Date != DateTime.Now.Date)
            {
                m_CurrentDate = DateTime.Now;
                LogFileName = getFileName(m_Path, Config.GetStringValue("ApplicationName"), m_CurrentDate);

                SweepOldFiles();
            }
        }

        private void SweepOldFiles()
        {
            int fpp = m_KeepDays;

            if (fpp > 0)
            {
                DateTime current = DateTime.Now;

                string[] files = Directory.GetFiles(m_Path, "*.log");

                foreach (string fileName in files)
                {
                    DateTime lastWrite = File.GetLastWriteTime(fileName);

                    if (lastWrite < current.AddDays(-1*fpp))
                    {
                        File.Delete(fileName);
                    }
                }
            }
        }
    }
}