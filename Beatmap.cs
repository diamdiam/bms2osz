﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        internal List<Event> EventList;
        internal List<Note> NoteList;
        internal List<Timing> TimingList;
        internal Dictionary<string, double> BpmDict;
        internal string Dir;
        internal string OrgDir;
        internal string OrgFilename;
        internal string Filename;

        internal Beatmap()
        {
            EventList = new List<Event>();
            NoteList = new List<Note>();
            TimingList = new List<Timing>();
            BpmDict = new Dictionary<string, double>();
        }

        internal bool Save(string name)
        {
            int index = name.LastIndexOf('\\');
            OrgFilename = name.Substring(index + 1, name.LastIndexOf('.') - index - 1);
            Dir = name.Substring(0, index) + "\\osu_output\\";
            OrgDir = name.Substring(0, index + 1); // with \\
            Artist = Artist.Replace('\\',' ');
            Title = Title.Replace('\\',' ');
            Artist = Artist.Replace('/', ' ');
            Title = Title.Replace('/', ' ');
            Filename = string.Format("{0} - {1} ({2}) [{3}].osu", Artist, Title, "BMXC_V1", Diff);
            //no notes in special column, convert to normal map.
            if (!Special)
            {
                Column--;
            }
            doSort();
            calculateTime();
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
            Timing target = TimingList[index - 1];
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
            Timing target = TimingList[index - 1];
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
                //no notes in special column
                if (!Special)
                    n.Column--;
            }
        }

        //bms support wav,ogg
        private string getSampleFilename(string name, out string ext)
        {
            string oldName = name.Substring(0, name.LastIndexOf('.'));
            for (int i = 0; i < 2; i++)
            {
                ext = i == 0 ? ".wav" : ".ogg";
                if (File.Exists(OrgDir + oldName + ext))
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
                img.Save(Dir + name+".jpg", ImageFormat.Jpeg);
                img.Dispose();
                Background = name + ".jpg";//rename
            }
            //event sample
            foreach (Event e in EventList)
            {
                string ext = "";
                string file = getSampleFilename(e.Filename, out ext);
                if (file == "")
                    continue;
                if (File.Exists(Dir + file))
                    continue;
                File.Copy(OrgDir + file, Dir + file);
            }
            //note sample
            foreach (SoundUnit su in SampleManager.sampleDict.Values)
            {
                string ext = "";
                string file = getSampleFilename(su.File, out ext);
                if (file == "")
                    continue;
                string newFile = string.Format("{0}{1}-hit{2}{3}{4}", Dir, su.Set.ToString().ToLower(),
                                                            su.Sound.ToString().ToLower(), su.Custom.ToString(), ext);
                if (File.Exists(newFile))
                    continue;
                File.Copy(OrgDir + file, newFile);
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
                    writer.WriteLine("AudioFilename: blank.mp3");
                    writer.WriteLine("AudioLeadIn: 0");
                    writer.WriteLine("PreviewTime: 0");
                    writer.WriteLine("Countdown: 0");
                    writer.WriteLine("SampleSet: 0");
                    writer.WriteLine("StackLeniency: 0.4");
                    writer.WriteLine("Mode: 3");
                    writer.WriteLine();

                    writer.WriteLine("[Metadata]");
                    writer.WriteLine("Title:" + Title);
                    writer.WriteLine("TitleUnicode:" + Title);
                    writer.WriteLine("Artist:" + Artist);
                    writer.WriteLine("ArtistUnicode:" + Artist);
                    writer.WriteLine("Creator: BMXC_V1");
                    writer.WriteLine("Version:" + Diff);
                    writer.WriteLine("Source:");
                    writer.WriteLine("Tags:");
                    writer.WriteLine("BeatmapID:0");
                    writer.WriteLine("BeatmapSetID:-1");
                    writer.WriteLine();

                    writer.WriteLine("[Difficulty]");
                    writer.WriteLine("HPDrainRate: 7");
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
                    if (EventList.Count > 0)
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
                                    t.changed ? 60000 / t.bpm : -100, (int)(t.beat*4), (int)t.SampleSet,
                                     (int)t.CustomSampleSet, 70, (t.changed ? "1" : "0"), 0);
                    }
                    writer.WriteLine();
                    writer.WriteLine("[HitObjects]");
                    foreach (Note n in NoteList)
                    {
                        string extra = "";
                        if (n.Type == NoteType.ManiaLong){
                            extra = ","+(int)n.TimeEnd;
                            if(n.Sound!=null)
                                extra+=string.Format(":{0}:{1}:{2}", (int)n.Sound.Set, 0, (int)n.Sound.Custom);
                        }
                        else
                            extra += n.Sound != null ? string.Format(",{0}:{1}:{2}", (int)n.Sound.Set, 0, (int)n.Sound.Custom) : "";
                        writer.WriteLine((int)((n.Column + 0.5) * 512 / Column) + ",192," + (int)n.TimeStart + "," + (int)n.Type + "," + (int)(n.Sound == null ? 0 : n.Sound.Sound) + extra);
                    }
                }
            }
        }
    }
}