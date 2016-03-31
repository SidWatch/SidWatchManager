using SidWatchLibrary.Objects;

namespace SidWatchCollectionLibrary
{
    public class DataCache
    {
        private static DataCache m_DataCache;

        private DataCache()
        {
            
        }

        public static DataCache GetInstance()
        {
            if (m_DataCache == null)
            {
                m_DataCache = new DataCache();
            }

            return m_DataCache;            
        }

        public AudioSegment LastAudioSegment { get; set; }


    }
}
