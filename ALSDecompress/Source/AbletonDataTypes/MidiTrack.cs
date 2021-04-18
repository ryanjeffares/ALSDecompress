using System;
using System.Collections.Generic;
using System.Text;

namespace ALSDecompress.AbletonDataTypes
{
    class MidiTrack : Track
    {
        public MidiTrack(int _id)
        {
            id = _id;
            type = TrackType.MidiTrack;
        }
    }
}
