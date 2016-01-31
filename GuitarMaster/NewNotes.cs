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

namespace GuitarMaster
{
    public static partial class Notes
    {
        public static int[] NewGetNotes(int[] scale, int countOfNotes, int[] rhythm)
        {
            int[] notes = new int[countOfNotes];

            notes[0] = 1;

            int shift = 0;
            Random random = new Random();
            int position = 0;

            for (int i = 1; i < notes.Length; i++)
            {
                shift = random.Next(1, scale.Length + 1);

                /* Повышение на октаву */
                if (shift == scale.Length)
                    notes[i] = notes[i - 1] + 12;

                int sum = 0;
                for (int j = position; j < shift % scale.Length; j++)
                {
                    sum = sum + scale[j];
                }
                position = (position + shift) % scale.Length;
                notes[i] = notes[i - 1] + sum;
            }

            return notes;
        }
        
    }
}
