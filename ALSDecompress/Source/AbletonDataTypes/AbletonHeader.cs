using System.Collections.Generic;

namespace ALSDecompress.AbletonDataTypes
{
    class AbletonHeader
    {
        public int MajorVersion { get; }
        public string MinorVersion { get; }
        public int SchemaChangeCount { get; }
        public string Creator { get; }
        public string Revision { get; }       

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