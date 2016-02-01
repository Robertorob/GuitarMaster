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
    public enum ScaleName {minor, major, flamenco, blues}
    public class MyScale
    {
        public ScaleName scaleName;
        public int[] scaleIntervals;

        public MyScale() { }

        public MyScale(ScaleName _scaleNamen, int[] _scale)
        {
            scaleName = _scaleNamen;
            scaleIntervals = _scale;
        }
    }

    public static partial class Notes
    {
        public static int[] NewGetNotes(MyScale scale, int countOfNotes, int[] rhythm)
        {
            int[] notes = new int[countOfNotes];
            notes[0] = 1;

            int[] scaleIntervals = scale.scaleIntervals;

            int[] shiftValues = new int[scaleIntervals.Length];
            int[] shiftProbab = new int[scaleIntervals.Length];

            switch (scale.scaleName)
            {
                case ScaleName.major:

                    break;
                case ScaleName.minor:

                    break;
                case ScaleName.flamenco:

                    break;
                case ScaleName.blues:

                    break;
            }


            MyRandom shifts = new MyRandom(shiftValues, shiftProbab);

            Random random = new Random();

            /* Сдвиг - на сколько ступеней гаммы сдвигаемся*/
            int shift = 0;

            /* Куда сдвигаем: 0 - вниз, 1 - наверх*/
            int upOrDown = 1;

            /* Показывает, на какой ступени гаммы мы сейчас находимся(номер элемента массива) */
            int position = 0;    

            for (int i = 1; i < notes.Length; i++)
            {
                upOrDown =  random.Next(0, 2);

                /* Сюда лучше сделать распределение вероятностей */
                shift =  random.Next(1, 3);///////////////////////////////////////////////////////////

                /* Повышение или понижение на октаву */
                if (shift == scaleIntervals.Length)
                {
                    if (upOrDown == 1)
                    {
                        notes[i] = notes[i - 1] + 12;
                    }
                    else
                    {
                        notes[i] = notes[i - 1] - 12;
                    }
                    continue;
                }

                int sum = 0, k = position;

                if (upOrDown == 1)
                {
                    for (int j = 0; j < shift; j++)
                    {
                        sum = sum + scaleIntervals[k % scaleIntervals.Length];
                        k++;
                    }
                    position = (position + shift) % scaleIntervals.Length;
                }
                else
                {
                    for (int j = 0; j < shift; j++)
                    {
                        if (k - 1 < 0)
                            k  = k  + scaleIntervals.Length;
                        sum = sum - scaleIntervals[(k - 1) % scaleIntervals.Length];
                        k--;
                        if (k < 0)
                            k += scaleIntervals.Length;
                    }
                    position = (position + scaleIntervals.Length - shift) % scaleIntervals.Length;
                }

                notes[i] = notes[i - 1] + sum;
            }

            return notes;
        }
        
    }
}
