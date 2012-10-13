using System;
using System.Collections.Generic;

using System.Text;

namespace bms
{
    [Flags]
    internal enum SoundType
    {
        None = 0,
        Normal = 1,
        Whistle = 2,
        Finish = 4,
        Clap = 8
    }

    [Flags]
    internal enum SampleSet
    {
        None = 0,
        Normal = 1,
        Soft = 2
    }

    internal class SoundUnit
    {
        internal int Custom = 0;
        internal SoundType Sound = SoundType.None;
        internal SampleSet Set = SampleSet.None;
        internal string File;
        public override string ToString()
        {
            return string.Format("{0}-hit{1}{2}", Set.ToString(), Sound.ToString(), Custom.ToString());
        }
    }

    static class SampleManager
    {
        private static int currentCustom = 2;
        private static SoundType currentSound = SoundType.Normal;
        private static SampleSet currentSet = SampleSet.Normal;
        internal static Dictionary<string, SoundUnit> sampleDict = new Dictionary<string, SoundUnit>();
        internal static Dictionary<string, SoundUnit> fileDict = new Dictionary<string, SoundUnit>();

        internal static void Clear()
        {
            currentCustom = 2;
            currentSound = SoundType.Normal;
            currentSet = SampleSet.Normal;
            sampleDict.Clear();
            fileDict.Clear();
        }

        internal static void Add(string key,string file)
        {
            if (fileDict.ContainsKey(file))
            {
                SoundUnit old = fileDict[file];
                sampleDict.Add(key, old);
                return;
            }
            if (sampleDict.ContainsKey(key))
            {
                return;
            }
            SoundUnit su = new SoundUnit();
            su.Custom = currentCustom;
            su.Sound = currentSound;
            su.Set = currentSet;
            su.File = file;
            sampleDict.Add(key, su);
            fileDict.Add(file, su);
            update();
        }

        internal static SoundUnit Get(string key)
        {
            SoundUnit su = null;
            if (sampleDict.TryGetValue(key, out su))
            {
                return su;
            }
            return null;
        }

        //custom>set>sound
        private static void update()
        {
            currentSound = (SoundType)((int)currentSound << 1);
            if (currentSound > SoundType.Clap)
            {
                currentSound = SoundType.Normal;
                currentSet = (SampleSet)((int)currentSet << 1);
            }
            if (currentSet > SampleSet.Soft)
            {
                currentSet = SampleSet.Normal;
                currentCustom++;
            }
        }
    }
}
