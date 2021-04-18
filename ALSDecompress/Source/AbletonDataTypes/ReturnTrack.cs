using System;
using System.Collections.Generic;
using System.Text;

namespace ALSDecompress.AbletonDataTypes
{
    class ReturnTrack : Track
    {
        public ReturnTrack(int _id)
        {
            id = _id;
            type = TrackType.ReturnTrack;
        }
    }
}
