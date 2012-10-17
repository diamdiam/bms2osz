using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace bms
{
    internal class Beatmap
    {
        internal int Column;
        internal bool Special;
        internal string Title = string.Empty;
        internal string Artist = string.Empty;
        internal string Background = string.Empty;
        internal string Diff = string.Empty;
        internal string Source = string.Empty;
        internal List<Event> EventList;
        internal List<Note> NoteList;
        internal List<Timing> TimingList;
        internal Dictionary<string, double> BpmDict;
        internal string Dir;
        internal string OrgDir;
        internal string OrgFilename;
        internal string Filename;
        internal int HPRate = 7;
        internal double FirstNoteTime= double.MaxValue;
        internal double LastNoteTime =double.MinValue;
        private bool WithSample;

        internal Beatmap()
        {
            EventList = new List<Event>();
            NoteList = new List<Note>();
            TimingList = new List<Timing>();
            BpmDict = new Dictionary<string, double>();
        }

        internal bool Save(string name, string output, bool withSample =true)
        {
            WithSample = withSample;
            int index = name.LastIndexOf('\\');
            OrgFilename = name.Substring(index + 1, name.LastIndexOf('.') - index - 1);
            Dir = output;
            OrgDir = name.Substring(0, index + 1); // with \\
            Regex reg = new Regex(@"[\\/\:\*\?\<\>\|\\""]");
            Artist = reg.Replace(Artist, "");
            Title = reg.Replace(Title, "");
            Diff = reg.Replace(Diff, "");
            Filename = string.Format("{0} - {1} ({2}) [{3}].osu", Artist, Title, "BMXC_V1", Diff);
            if(File.Exists(Dir+Filename))
                Filename = "("+OrgFilename+")"+Filename;
            //no notes in special column, convert to normal map.
            if (!Special)
            {
                Column--;
            }
            doSort();
            calculateTime();
            if(withSample)
                calculateEvent();
            calculateNote();
            writeToFile();
            return true;
        }

        private void doSort()
        {
            TimingList.Sort();
            EventList.Sort((x, y) =>
            {
                if (x.Section < y.Section)
                    return -1;
                else if (x.Section > y.Section)
                    return 1;
                else
                {
                    if (x.Offset < y.Offset)
                        return -1;
                    else if (x.Offset > y.Offset)
                        return 1;
                    else
                        return 0;
                }
            });
            //Notice: sort note by start time
            NoteList.Sort((x, y) =>
            {
                if (x.SectionStart < y.SectionStart)
                    return -1;
                else if (x.SectionStart > y.SectionStart)
                    return 1;
                else
                {
                    if (x.OffsetStart < y.OffsetStart)
                        return -1;
                    else if (x.OffsetStart > y.OffsetStart)
                        return 1;
                    else
                    {
                        if (x.Column < y.Column)
                            return -1;
                        else if (x.Column > y.Column)
                            return 1;
                        else
                            return 0;
                    }
                }
            });
        }

        private void calculateTime()
        {
            Timing current = TimingList[0].Clone();
            for (int i = 0; i < TimingList.Count; i++)
            {
                Timing t = TimingList[i];
                t.Time = ((t.Section + t.Offset) - (current.Section + current.Offset)) * (60000 * 4 / current.bpm) * current.beat + current.Time;
                current.Time = t.Time;
                current.Section = t.Section;
                current.Offset = t.Offset;
                if (t.bpm != -1)
                    current.bpm = t.bpm;
                else
                    t.bpm = current.bpm;
                if (t.beat != current.beat)
                    current.beat = t.beat;
                if (t.CustomSampleSet == -1)
                    t.CustomSampleSet = current.CustomSampleSet;
                else
                    current.CustomSampleSet = t.CustomSampleSet;
            }
        }

        private double getEventTime(Event e)
        {
            Timing t = new Timing();
            t.Section = e.Section;
            t.Offset = e.Offset;
            int index = TimingList.BinarySearch(t);
            if (index < 0)
                index = ~index;
            if (index > TimingList.Count)
                index = TimingList.Count;
            Timing target = TimingList[index==0?0:index - 1];
            return ((t.Section + t.Offset) - (target.Section + target.Offset)) * (60000 * 4 / target.bpm) * target.beat + target.Time;
        }

        private void calculateEvent()
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                Event e = EventList[i];
                e.Time = getEventTime(e);
            }
        }

        private double getNoteTime(Note n, bool head = true)
        {
            Timing t = new Timing();
            t.Section = head ? n.SectionStart : n.SectionEnd;
            t.Offset = head ? n.OffsetStart : n.OffsetEnd;
            int index = TimingList.BinarySearch(t);
            if (index < 0)
                index = ~index;
            if (index > TimingList.Count)
                index = TimingList.Count;
            Timing target = TimingList[index==0?0:index - 1];
            return ((t.Section + t.Offset) - (target.Section + target.Offset)) * (60000 * 4 / target.bpm) * target.beat + target.Time;
        }

        private void calculateNote()
        {
            for (int i = 0; i < NoteList.Count; i++)
            {
                Note n = NoteList[i];
                n.TimeStart = getNoteTime(n);
                if (n.Type == NoteType.ManiaLong)
                    n.TimeEnd = getNoteTime(n, false);
                else
                    n.TimeEnd = n.TimeStart;
                //no notes in special column
                if (!Special)
                    n.Column--;
                if (n.TimeStart < FirstNoteTime)
                    FirstNoteTime = n.TimeStart;
                if (n.TimeEnd > LastNoteTime)
                    LastNoteTime = n.TimeEnd;
            }
        }

        //bms support wav,ogg
        internal static string GetSampleFilename(string orgDir, string name, out string ext)
        {
            string oldName = name.Substring(0, name.LastIndexOf('.'));
            for (int i = 0; i < 3; i++)
            {
                if(i==0)
                    ext = ".wav";
                else if(i==1)
                    ext = ".ogg";
                else
                    ext = ".mp3";
                if (File.Exists(orgDir + oldName + ext))
                    return oldName + ext;
            }
            ext = "";
            return "";
        }

        private void moveFile()
        {
            //bg
            if (Background != "" && File.Exists(OrgDir + Background))
            {
                Image img = Image.FromFile(OrgDir + Background);
                string name = Background.Substring(0, Background.LastIndexOf('.'));
                if (File.Exists(Dir + name + ".jpg"))
                    File.Delete(Dir + name + ".jpg");
                img.Save(Dir + name + ".jpg", ImageFormat.Jpeg);
                img.Dispose();
                Background = name + ".jpg";//rename
            }
            else
            {
                Background = "";
            }
            //event sample
            if (!WithSample)
                return;
            foreach (Event e in EventList)
            {
                string file = e.Filename;
                if (file == "")
                    continue;
                if (File.Exists(Dir + file))
                    continue;
                File.Copy(OrgDir + file, Dir + file);
            }
            
        }

        private void writeToFile()
        {
            moveFile();
            using (FileStream fs = new FileStream(Dir + Filename, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("osu file format v11");
                    writer.WriteLine();
                    writer.WriteLine("[General]");
                    writer.WriteLine("AudioFilename: virtual");
                    writer.WriteLine("AudioLeadIn: 0");
                    writer.WriteLine("PreviewTime: 0");
                    writer.WriteLine("Countdown: 0");
                    writer.WriteLine("SampleSet: 0");
                    writer.WriteLine("StackLeniency: 0.4");
                    writer.WriteLine("Mode: 3");
                    writer.WriteLine("SpecialStyle:" + (Special ? "1" : "0"));//new value, for 7+1 and 5+1 only.
                    writer.WriteLine();

                    writer.WriteLine("[Metadata]");
                    writer.WriteLine("Title:" + Title);
                    writer.WriteLine("TitleUnicode:" + Title);
                    writer.WriteLine("Artist:" + Artist);
                    writer.WriteLine("ArtistUnicode:" + Artist);
                    writer.WriteLine("Creator: BMXC_V1");
                    writer.WriteLine("Version:" + Diff);
                    writer.WriteLine("Source:" + Source);
                    writer.WriteLine("Tags:");
                    writer.WriteLine("BeatmapID:0");
                    writer.WriteLine("BeatmapSetID:-1");
                    writer.WriteLine();

                    writer.WriteLine("[Difficulty]");
                    writer.WriteLine("HPDrainRate: "+HPRate.ToString());
                    writer.WriteLine("CircleSize: " + Column.ToString());
                    writer.WriteLine("OverallDifficulty: 7");
                    writer.WriteLine("ApproachRate: 7");
                    writer.WriteLine("SliderMultiplier: 0.4");
                    writer.WriteLine("SliderTickRate:1");
                    writer.WriteLine();

                    writer.WriteLine("[Events]");
                    if (Background != "")
                    {
                        writer.WriteLine("//Background and Video events");
                        writer.WriteLine("{0},{1},\"{2}\"", 0, 0, Background);
                    }
                    if (EventList.Count > 0 && WithSample)
                    {
                        writer.WriteLine("//Storyboard Sound Samples");

                        foreach (Event e in EventList)
                            writer.WriteLine("{0},{1},{2},\"{3}\",{4}", 5, (int)e.Time, 0, e.Filename, 70);
                        writer.WriteLine();
                    }

                    writer.WriteLine("[TimingPoints]");
                    double lastTime = -1;
                    foreach (Timing t in TimingList)
                    {
                        writer.WriteLine("{0:0},{1},{2},{3},{4},{5},{6},{7}", t.Time,
                                    t.changed ? 60000 / t.bpm : -100, (int)(t.beat * 4), (int)t.SampleSet,
                                     (int)t.CustomSampleSet, 70, (t.changed ? "1" : "0"), 0);
                    }
                    writer.WriteLine();

                    SoundUnit empty = new SoundUnit();
                    writer.WriteLine("[HitObjects]");
                    foreach (Note n in NoteList)
                    {
                        string extra = "";
                        SoundUnit su = n.Sound;
                        if (su == null || !WithSample)
                            su = empty;
                        if (n.Type == NoteType.ManiaLong)
                        {
                            extra = "," + (int)n.TimeEnd;
                            extra += string.Format(":{0}:{1}:{2}", (int)su.Set, 0, (int)su.Custom);
                        }
                        else
                            extra += string.Format(",{0}:{1}:{2}", (int)su.Set, 0, (int)su.Custom);
                        writer.WriteLine((int)((n.Column + 0.5) * 512 / Column) + ",192," + (int)n.TimeStart + "," + (int)n.Type + "," + (int)su.Sound + extra);
                    }
                }
            }
        }
    }
}
