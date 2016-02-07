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
using MidiExamples;
using System.Windows.Media;
using System.IO;

namespace GuitarMaster
{
    public class Melody
    {
        public string Name { set; get; }
        public int[] Notes;
        public int[] Rhythm;
        public ScaleName Style;
        public static int Number = 1;

        public Melody(string name, int[] notes, int[] rhythm, ScaleName style)
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
            this.Style = style;
        }
    }
    public static class Parser
    {
        public static int[] GetMassive(string text)
        {
            int[] massive;
            char[] separators = { ' ', '\r', '\n' };
            string[] mass = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            massive = new int[mass.Length];

            for (int i = 0; i < massive.Length; i++)
            {
                massive[i] = int.Parse(mass[i]);
            }
            return massive;
        }
        //public static List<Melody> FillMelodyList(string text)
        //{
            //var stack = new Stack<Line>();
            //String all = sr.ReadToEnd();
            //char[] separators = { '\r', '\n' };
            //string[] mass = all.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            //Line line = new Line();
            //for (int i = 0; i < mass.Length; i++)
            //{
            //    string[] mass2 = new string[2];
            //    mass2 = mass[i].Split(new char[] { ' ' });
            //    line.command = mass2[0];
            //    if (mass2.Length > 1)
            //        line.operand = mass2[1];
            //    stack.Push(line);
            //}

            //InvertStack(ref stack);

            //return stack;
            
        //}
    }
}
