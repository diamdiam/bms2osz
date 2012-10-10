using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bms
{
    [Flags]
    internal enum NoteType
    {
        Normal = 1,
        Slider = 2,
        NewCombo = 4,
        NormalNewCombo = 5,
        SliderNewCombo = 6,
        Spinner = 8,
        ManiaLong = 16,
        ColourHax = 112,
        Hold = 128
    }

    

    internal class Note
    {
        internal int Column;
        internal int SectionStart;
        internal int SectionEnd;
        internal double OffsetStart;
        internal double OffsetEnd;
        internal double TimeStart;
        internal double TimeEnd;
        internal NoteType Type;
        internal SoundUnit Sound;

        public string ToString()
        {
            return string.Format("Col:{0} Time:{1:0} Sec:{2}/{3}", Column, TimeStart, SectionStart, OffsetStart);
        }
    }
}
