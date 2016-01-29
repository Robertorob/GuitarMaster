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
        //public static int[] NewGetNotes(int[] scale, int countOfNotes)
        //{
        //    int[] phrase = new int[countOfNotes];
        //    for (int i = 0; i < phrase.Length; i++)
        //    {
        //        phrase[i] = 0;
        //    }
        //    int stable = stables.Next();
        //    int position = 1;

        //    switch (tact)//2 и 3 такт одинаковы, поэтому в параметры передаем всегда 2
        //    {
        //        case 1:
        //            phrase[0] = 1;
        //            position = positions.Next(1, length);
        //            phrase[position] = stable;
        //            break;
        //        case 2:
        //            position = positions.Next(0, length);
        //            phrase[position] = stable;
        //            break;
        //        case 4:
        //            phrase[length - 1] = 1;
        //            position = length - 1;
        //            stable = 1;
        //            break;
        //    }
        //    phrase = Transitions(chord, phrase, stable, position);

        //    return phrase;
        //}
        
    }
}
