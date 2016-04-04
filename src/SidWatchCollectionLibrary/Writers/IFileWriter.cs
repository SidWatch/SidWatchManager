using System;
using SidWatchLibrary.Objects;

namespace SidWatchCollectionLibrary.Writers
{
    public interface IFileWriter
    {
        void OpenFile(string _folder, Station _station, string _fileSuffix);

        void CloseFile();

        bool IsFileOpen();

        void WriteAudioSegment(AudioSegment _segment);
    }
}
