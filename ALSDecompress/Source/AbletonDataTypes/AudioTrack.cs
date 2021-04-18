using System;
using System.Collections.Generic;
using System.Text;

namespace ALSDecompress.AbletonDataTypes
{
    class AudioTrack : Track
    {
        public AudioTrack(int _id)
        {
            id = _id;
            type = TrackType.AudioTrack;
        }
    }
}
