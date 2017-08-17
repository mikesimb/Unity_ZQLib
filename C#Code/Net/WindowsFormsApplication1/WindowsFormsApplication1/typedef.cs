using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZQLib
{
   [Serializable]
   struct DefaultMessage
    {
        public int Recog;
        public ushort Ident;
        public ushort Param;
        public ushort Tag;
        public ushort Series;
    }
    [Serializable]
    struct ClientMessage
    {
        public uint Sign;
        public byte ReservByte;
        public byte Cmd;
        public ushort DataLength;
        public uint DataIndex;

    }
}
