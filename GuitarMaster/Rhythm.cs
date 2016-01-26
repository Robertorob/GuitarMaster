using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarMaster
{
    public static class Rhythm
    {
        public static int[] GetRhythm(int time, int countOfNotes)
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < time; i++)
            {
                set.Add(i);
            }

            int[] rhythm = new int[time];
            Random rand = new Random();

            for (int i = 0; i < countOfNotes; i++)
            {
                int pos = rand.Next(0, time);
                int position = set.ElementAt(pos);
                rhythm[position] = 1;

                set.Remove(position);
                time--;
            }

            return rhythm;
        }
    }
}
