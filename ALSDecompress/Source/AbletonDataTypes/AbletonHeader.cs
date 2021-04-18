using System.Collections.Generic;

namespace ALSDecompress.AbletonDataTypes
{
    public class AbletonHeader
    {
        public int MajorVersion { get; }
        public string MinorVersion { get; }
        public int SchemaChangeCount { get; }
        public string Creator { get; }
        public string Revision { get; }

        public Dictionary<string, int> intValues;
        public Dictionary<string, bool> boolValues;
        public Dictionary<string, int> videoRect;
        public string viewData, annotation;

        public AbletonHeader(int majVer, string minVer, int schemaChangeCount, string creator, string revision)
        {
            MajorVersion = majVer;
            MinorVersion = minVer;
            SchemaChangeCount = schemaChangeCount;
            Creator = creator;
            Revision = revision;
        }
    }
}