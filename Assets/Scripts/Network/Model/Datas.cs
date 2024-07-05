using System;

namespace Data
{
    public interface IDataBase { }

    [Serializable]
    public class AbstractData : IDataBase
    {
        public string UserID;
    }

    namespace Demo
    {
        [Serializable]
        public class DemoData : AbstractData
        {
            public string Name;
            public int Score;
        }
    }

    namespace Master
    {
        [Serializable]
        public class NameData : AbstractData
        {
            public string Name;
        }

        [Serializable]
        public class ScoreData : AbstractData
        {
            public int Score;
        }

        [Serializable]
        public class VersionData : AbstractData
        {
            public int Version;
        }
    }
}
