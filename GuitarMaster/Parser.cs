using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Midi;
using System.Threading;
using System.Windows.Media;
using System.IO;

namespace GuitarMaster
{
    public class Melody
    {
        public string Name { set; get; }
        public int[] Notes;
        public int[] Rhythm;
        public ScaleName ScaleName;
        public static int Number = 1;

        public Melody(string name, int[] notes, int[] rhythm, ScaleName scaleName)
        {
            int sum = rhythm.Sum();
            if (sum > notes.Length)
            {
                int[] newNotes = new int[sum];
                for (int i = 0; i < notes.Length; i++)
                {
                    newNotes[i] = notes[i];
                }
            }

            this.Name = name;
            this.Notes = notes;
            this.Rhythm = rhythm;
            this.ScaleName = scaleName;
        }
    }
    public static class Parser
    {
        public static string GetString(int[] massive)
        {
            string s = "";
            
            for (int i = 0; i < massive.Length; i++)
            {
                s += massive[i].ToString() + " ";
            }
            return s;
        }

        public static int[] GetMassive(string text)
        {
            int[] massive;
            char[] separators = { ' ', '\r', '\n' };
            string[] mass = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            massive = new int[mass.Length];

            for (int i = 0; i < massive.Length; i++)
            {
                int res;
                bool success = int.TryParse(mass[i], out res);
                if (!success)
                {
                    return null;
                }
                else
                {
                    massive[i] = res;
                }
            }
            if (massive.Length == 0)
                return null;
            return massive;
        }

        public static List<Melody> GetAllMelodys()
        {
            List<Melody> list = new List<Melody>();
            StreamReader sr = new StreamReader("Melodys.txt", Encoding.Default);
            String line = "";

            while ((line = sr.ReadLine()) != null)
            {
                if (line == "")
                    continue;
                char[] separators = { ';' };
                string[] tokens = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                string name = tokens[0];
                string scale = tokens[1];
                string notesLine = tokens[2];
                string rhythmLine = tokens[3];

                ScaleName scaleName = MyScale.GetScaleName(scale);
                

                char[] separators2 = { ' ' };
                string[] stringNotes = notesLine.Split(separators2, StringSplitOptions.RemoveEmptyEntries);
                string[] stringRhythm = rhythmLine.Split(separators2, StringSplitOptions.RemoveEmptyEntries);

                int[] notes = new int[stringNotes.Length];
                int[] rhythm = new int[stringRhythm.Length];

                for (int i = 0; i < rhythm.Length; i++)
                {
                    rhythm[i] = int.Parse(stringRhythm[i]);                   
                }
                for (int i = 0; i < notes.Length; i++)
                {
                    notes[i] = int.Parse(stringNotes[i]);
                }

                Melody melody = new Melody(name, notes, rhythm, scaleName);
                list.Add(melody);
            }

            sr.Close();
            return list;

        }

        public static void SaveMelodyToFile(Melody melody)
        {
            string line =  melody.Name + ";" + melody.ScaleName.ToString() + ";" + GetString(melody.Notes) + ";" + GetString(melody.Rhythm) + ";";
            using (StreamWriter sw = new StreamWriter("Melodys.txt", true, Encoding.GetEncoding(1251)))
            {
                sw.WriteLine(line);
            }   
        }

        public static void RewriteMelodysFile(List<Melody> list)
        {
            StreamWriter sw = new StreamWriter("Melodys.txt", false, Encoding.GetEncoding(1251));
            for (int i = 0; i < list.Count; i++)
            {
                string line = list[i].Name + ";" + list[i].ScaleName.ToString() + ";" + GetString(list[i].Notes) + ";" + GetString(list[i].Rhythm) + ";";
                sw.WriteLine(line);
            }
            sw.Close();
        }
    }
}
