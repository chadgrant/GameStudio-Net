using System;

namespace GameStudio
{
    public struct EuidInfoV1
    {
        public Studio Studio { get; set; }
        public Application Application { get; set; }
        public uint PlayerId { get; set; }
    }

    public struct EuidInfo
    {
        public Studio Studio { get; set; }
        public Application Application { get; set; }
        public ushort Type { get; set; }
        public uint PlayerId { get; set; }
        public uint Random { get; set; }
        public DateTime Created { get; set; }
    }
}
