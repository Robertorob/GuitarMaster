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

namespace GuitarMaster
{

    public static partial class Notes
    {
        public static int[] GetNotes(MyScale scale, int countOfNotes, int[] rhythm)
        {
            int[] notes = new int[countOfNotes];
            notes[0] = 1;

            int[] scaleIntervals = scale.scaleIntervals;

            /* Создадим массив сдвигов и массив вероятностей выбора этого сдвига */
            int[] shiftValues = new int[scaleIntervals.Length + 1];
            int[] shiftProbab = MyRandom.GetProbabilities(shiftValues, scale.scaleName);

            MyRandom shifts = new MyRandom(shiftValues, shiftProbab);

            Random random = new Random();

            /* Сдвиг - на сколько ступеней гаммы сдвигаемся. Случайная величина */
            int shift;

            /* Куда сдвигаем: 0 - вниз, 1 - наверх. Случайная величина */
            int upOrDown;

            /* Показывает, на какой ступени гаммы мы сейчас находимся(номер элемента массива) */
            int position = 0; 

            int[] shiftsOut = new int[notes.Length - 1];//////////////////////////////////////Отладка
            int[] notesOut = new int[notes.Length];///////////////////////////////////////////Отладка

            for (int i = 1; i < notes.Length; i++)
            {
                upOrDown =  random.Next(0, 2);

                shift = shifts.Next();

                /* Повтор ноты */
                if (shift == 0)
                {
                    notes[i] = notes[i - 1];
                    continue;
                }

                int sum = 0, k = position;

                shiftsOut[i - 1] = shift;///////////////////////Отладка
                notesOut[0] = 1;////////////////////////////////Отладка

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
                            k = k  + scaleIntervals.Length;
                        sum = sum - scaleIntervals[(k - 1) % scaleIntervals.Length];
                        k--;                        
                    }
                    position = (position + scaleIntervals.Length - shift) % scaleIntervals.Length;
                }

                notesOut[i] = position;///////////////////////////////////////Отладка

                /* Случайное повышение или понижение на октаву (для разнообразия). Вероятность 0.1 */
                if (random.Next(0, 10) == 1)
                {
                    if (upOrDown == 1)
                    {
                        sum += 12;
                    }
                    else
                    {
                        sum -= 12;
                    }
                }

                notes[i] = notes[i - 1] + sum;
            }

            //notes[notes.Length - 1] = 1;

            return notes;
        }
        
    }
}
