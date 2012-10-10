using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bms
{
    internal class Timing: IComparer<Timing>,IComparable<Timing>
    {
        internal int Section = -1;
        internal double Offset = -1;
        internal double bpm = -1;
        internal bool changed = false;
        internal double beat = 1; // 4/4
        internal double Time;
        internal int CustomSampleSet = -1;
        internal SampleSet SampleSet;

        public string ToString()
        {
            return string.Format("Time:{0} Sec:{2}/{3} bpm:{4}", Time, Section, Offset, bpm);
        }

        public int Compare(Timing x, Timing y)
        {
            return y.CompareTo(x);
        }

        public int CompareTo(Timing x)
        {
            if (x.Section < Section)
                return 1;
            else if (x.Section > Section)
                return -1;
            else
            {
                if (x.Offset < Offset)
                    return 1;
                else if (x.Offset > Offset)
                    return -1;
                else
                    return 0;
            }
        }

        public Timing Clone() 
        {
            Timing t = new Timing();
            t.beat = beat;
            t.bpm = bpm;
            t.CustomSampleSet = CustomSampleSet;
            t.SampleSet = SampleSet;
            t.Offset = Offset;
            t.Section = Section;
            t.Time = Time;
            return t;
        }
    }
}
