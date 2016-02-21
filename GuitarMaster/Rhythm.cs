using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarMaster
{
    public static class Rhythm
    {
        /// <summary>
        /// Возвращает длительность такта в секундах
        /// </summary>
        /// <param name="tempo">Значение темпа от -60 до -10</param>
        /// <param name="notesCount">Количество нот</param>
        /// <returns></returns>
        public static double GetDuration(int tempo, int notesCount)
        {
            return (    ((double)tempo * (-1) / 10) * (double)notesCount    ) / 8;
        }

        /// <summary>
        /// Метод возвращает массив, описывающий ритм
        /// </summary>
        /// <param name="segmentsCount">Количество долей в такте (количество нот и пауз)</param>
        /// <param name="notesCount">Количество звучащих нот</param>
        /// <returns></returns>
        public static int[] GetRhythm(int segmentsCount, int notesCount)
        {           
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < segmentsCount; i++)
            {
                set.Add(i);
            }

            int[] rhythm = new int[segmentsCount];
            Random rand = new Random();

            for (int i = 0; i < notesCount; i++)
            {
                int pos = rand.Next(0, segmentsCount);
                int position = set.ElementAt(pos);
                rhythm[position] = 1;

                set.Remove(position);
                segmentsCount--;
            }

            return rhythm;
        }
    }
}
