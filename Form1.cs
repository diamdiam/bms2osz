using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using bms.Properties;

namespace bms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btLoad_Click(object sender, EventArgs e)
        {
            if (ofdLoad.ShowDialog() == DialogResult.OK)
            {
                tbLoad.Text = ofdLoad.FileName;
            }
        }


        Beatmap map;
        Note[] pendingNote;
        int longType = 1;
        string longObj = string.Empty;
        double lastMapTime = double.MinValue;
        Dictionary<string, string> wavDict;

        private void processBMS(string[] files)
        {
            string dir = ofdLoad.FileName.Substring(0, ofdLoad.FileName.LastIndexOf('\\') + 1);
            string ext;
            for (int i = 0; i < files.Length; i++)
            {
                map = new Beatmap();
                wavDict.Clear();
                SampleManager.sampleDict.Clear();
                pendingNote = new Note[10];
                longType = 1;
                longObj = string.Empty;
                try
                {
                    using (FileStream fs = new FileStream(files[i], FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                        {
                            while (!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                if (line.IndexOf('#') < 0)
                                    continue;
                                line = line.Substring(1);
                                //try split with :
                                if (line.IndexOf(':') > 0)
                                {
                                    string[] arr = line.Split(':');
                                    if (arr.Length == 2)
                                    {
                                        arr[0] = arr[0].Trim();
                                        arr[1] = arr[1].Trim();
                                        if (arr[0].Length == 5)
                                        {
                                            ParseDataField(arr);
                                            continue;
                                        }
                                    }
                                }
                                //try split with space
                                int spaceInx = line.IndexOf(' ');
                                if (spaceInx > 0)
                                {
                                    string[] arr = new string[2];
                                    arr[0] = line.Substring(0, spaceInx).Trim().ToUpper();
                                    arr[1] = line.Substring(spaceInx).Trim();
                                    if (arr[0].StartsWith("BPM") && arr[0].Length == 5)
                                    {
                                        string key = arr[0].Substring(3, 2);
                                        map.BpmDict.Add(key, double.Parse(arr[1]));
                                        continue;
                                    }
                                    if (arr[0].StartsWith("WAV") && arr[0].Length == 5)
                                    {
                                        string key = arr[0].Substring(3, 2);
                                        arr[1] = Beatmap.GetSampleFilename(dir,arr[1], out ext);
                                        if (arr[1] !="")
                                        {
                                            SampleManager.Add(key, arr[1]);
                                            wavDict.Add(key, arr[1]);
                                        }
                                        continue;
                                    }
                                    switch (arr[0])
                                    {
                                        case "PLAYER":
                                            if (int.Parse(arr[1]) != 1)
                                            {
                                                throw new Exception("Can not convert 2 player bms file.");
                                            }
                                            break;
                                        case "TITLE":
                                            map.Title = arr[1];
                                            break;
                                        case "ARTIST":
                                            map.Artist = arr[1];
                                            break;
                                        case "BPM":
                                            Timing t = new Timing();
                                            t.Section = 0;
                                            t.Offset = 0;
                                            t.CustomSampleSet = 2;
                                            t.bpm = double.Parse(arr[1]);
                                            t.changed = true;
                                            t.beat = 1;
                                            map.TimingList.Add(t);
                                            break;
                                        case "STAGEFILE":
                                            map.Background = arr[1];
                                            break;
                                        case "PLAYLEVEL":
                                            map.Diff = "Lv." + arr[1];
                                            break;
                                        case "RANK":
                                            //rank = 0~3  / insane~easy
                                            int rank = int.Parse(arr[1]);
                                            map.HPRate = 9 - rank * 2;
                                            map.HPRate = Math.Max(3, Math.Min(9, map.HPRate));// 3~9
                                            break;
                                        case "LNTYPE":
                                            longType = int.Parse(arr[1]);
                                            break;
                                        case "LNOBJ":
                                            longObj = arr[1];
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    for (int j = 0; j < pendingNote.Length; j++)
                    {
                        if (pendingNote[j] != null)
                            throw new Exception("Something wrong with the bms");
                    }
                    if (tb_artist.Text.Trim() != "")
                        map.Artist = tb_artist.Text;
                    if (tb_title.Text.Trim() != "")
                        map.Title = tb_title.Text;
                    if (tb_version.Text.Trim() != "")
                        map.Diff = tb_version.Text;
                    if (tb_source.Text.Trim() != "")
                        map.Source = tb_source.Text;
                    map.Save(files[i]);
                    if (map.LastNoteTime > lastMapTime)
                        lastMapTime = map.LastNoteTime;
                    tbResult.Text += string.Format("[Done] title:{0} notes:{1}\r\n", map.Title,map.NoteList.Count);
                }
                catch (Exception ex)
                {
                    tbResult.Text += "[Error] " + ex.Message + "\r\n";
                }
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            wavDict = new Dictionary<string, string>();
            SampleManager.Clear();
            string path = ofdLoad.FileName;
            string dir = path.Substring(0, path.LastIndexOf('\\')+1);
            string[] files;
            if (!cb_all.Checked)
            {
                files = new string[1];
                files[0] = path;
            }
            else
            {
                string[] bms = Directory.GetFiles(dir,"*.bms");
                string[] bme = Directory.GetFiles(dir,"*.bme");
                files = new string[bms.Length + bme.Length];
                bms.CopyTo(files, 0);
                bme.CopyTo(files, bms.Length);
            }
            DirectoryInfo di = new DirectoryInfo(dir + "osu_output\\");
            if (di.Exists)
                di.Delete(true);
            di.Create();
            processBMS(files);
            //note sample
            foreach (SoundUnit su in SampleManager.fileDict.Values)
            {
                string file = su.File;
                if (file == "")
                {
                    continue; //shouldn't run to here
                }
                string newFile = string.Format("{0}{1}-hit{2}{3}{4}", dir+"osu_output\\", su.Set.ToString().ToLower(),
                                                            su.Sound.ToString().ToLower(), su.Custom.ToString(), file.Substring(file.LastIndexOf('.')));
                if (File.Exists(newFile))
                    continue;
                File.Copy(dir + file, newFile);
            }
            //blank mp3
        /*    int minuts = (int)Math.Ceiling(lastMapTime / 1000 / 60);
            if (minuts < 2)
                minuts = 2;
            if (minuts > 4)
                tbResult.Text += "[Warning] Missing blank.mp3 in output.\r\n";
            else
            {
                byte[] data;
                if(minuts == 2)
                    data = Resources.blank2;
                else if(minuts == 3)
                    data = Resources.blank3;
                else
                    data= Resources.blank4;
                using (FileStream fs = new FileStream(dir + "osu_output\\blank.mp3", FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Write(data, 0, data.Length);
                    }
                }
            }*/
        }   

        private int[] line2key(string line)
        {
            int[] arr = new int[2];
            arr[0] = int.Parse(line.Substring(0, 3));
            arr[1] = int.Parse(line.Substring(3, 2));
            return arr;
        }

        /// <summary>
        /// 00000100-> 00 00 01 00
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<string> line2arr(string line)
        {
            int index = 0;
            List<string> result = new List<string>(line.Length / 2);
            while (index < line.Length - 1)
            {
                result.Add(line.Substring(index, 2));
                index += 2;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">[00111:00000100]</param>
        private void ParseDataField(string[] arr)
        {
            int[] key = line2key(arr[0]);// 001 00
            List<string> val = line2arr(arr[1]);// 00 01 AW 00
            switch (key[1])
            {
                case 1:  //BGM
                    for (int i = 0; i < val.Count; i++)
                    {
                        if (val[i] == "00")
                            continue;
                        string fn;
                        if (wavDict.TryGetValue(val[i], out fn))
                        {
                            Event e = new Event();
                            e.Section = key[0];
                            e.Filename = fn;
                            e.Offset = 1.0 * i / val.Count;
                            map.EventList.Add(e);
                        }
                    }
                    break;
                case 2:  //beat change
                    {
                        int Section = key[0];
                        int Offset = 0;
                        Timing target = map.TimingList.Find(t => t.Section == Section && t.Offset == Offset);
                        if (target == null)
                        {
                            Timing t = new Timing();
                            t.beat = double.Parse(arr[1]);
                            t.Section = Section;
                            t.Offset = Offset;
                            map.TimingList.Add(t);
                        }else
                            target.beat = double.Parse(arr[1]);
                    }
                    break;
                case 3:   //hardcode bpm
                    for (int i = 0; i < val.Count; i++)
                    {
                        if (val[i] == "00")
                            continue;
                        Timing t = new Timing();
                        t.Section = key[0];
                        t.Offset = 1.0 * i / val.Count;
                        t.bpm = Convert.ToInt32(val[i], 16);
                        t.changed = true;
                        map.TimingList.Add(t);
                    }
                    break;
                case 8:  //pre-defined bpm
                    for (int i = 0; i < val.Count; i++)
                    {
                        if (val[i] == "00")
                            continue;
                        double bpm;
                        if (map.BpmDict.TryGetValue(val[i], out bpm))
                        {
                            Timing t = new Timing();
                            t.Section = key[0];
                            t.Offset = 1.0 * i / val.Count;
                            t.bpm = bpm;
                            t.changed = true;
                            map.TimingList.Add(t);
                        }
                    }
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 18:
                case 19:
                    {
                        for (int i = 0; i < val.Count; i++)
                        {
                            if (val[i] == "00")
                                continue;
                            int col = key[1] == 16 ? 0 : (key[1] < 16 ? key[1] - 10 : key[1] - 12);
                            if (longType == 2 && val[i] == longObj && pendingNote[col] != null)
                            {
                                pendingNote[col].SectionEnd = key[0];
                                pendingNote[col].OffsetEnd = 1.0 * i / val.Count;
                                pendingNote[col].Type = NoteType.ManiaLong;
                                pendingNote[col] = null;
                                continue;
                            }
                            Note n = new Note();
                            n.Column = col;
                            n.SectionStart = key[0];
                            n.OffsetStart = 1.0 * i / val.Count;
                            n.Type = NoteType.Normal;
                            n.Sound = SampleManager.Get(val[i]);
                            map.NoteList.Add(n);
                            if(col>map.Column-1)
                                map.Column = col + 1; //reset map column
                            if (col == 0)
                                map.Special = true;
                            if (longType == 2)
                                pendingNote[col] = n;
                        }
                    }
                    break;
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 58:
                case 59:
                    {
                        if (longType != 1)
                            break;
                        for (int i = 0; i < val.Count; i++)
                        {
                            if (val[i] == "00")
                                continue;
                            int col = key[1] == 56 ? 0 : (key[1] < 56 ? key[1] - 50 : key[1] - 52);
                            if (pendingNote[col] != null)
                            {
                                pendingNote[col].SectionEnd = key[0];
                                pendingNote[col].OffsetEnd = 1.0 * i / val.Count;
                                pendingNote[col] = null;
                                continue;
                            }
                            else
                            {
                                Note n = new Note();
                                n.Column = col;
                                n.SectionStart = key[0];
                                n.OffsetStart = 1.0 * i / val.Count;
                                n.Type = NoteType.ManiaLong;
                                n.Sound = SampleManager.Get(val[i]);
                                map.NoteList.Add(n);
                                if (col > map.Column - 1)
                                    map.Column = col + 1; //reset map column
                                if (col == 0)
                                    map.Special = true;
                                pendingNote[col] = n;
                            }

                        }
                    }
                    break;
            }
        }
    }
}
