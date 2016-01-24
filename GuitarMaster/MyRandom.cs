using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarMaster
{
    class MyRandom//:Random
    {
        int[] section;
        Random random;
        public MyRandom(int[] values, int[] probabilities)// значения и вероятности
        {
            section = new int[100];
            random = new Random();

            if (values.Length != probabilities.Length)
            {
                Console.WriteLine("Число объектов и вероятностей не совпадает!");
                return;
            }

            int sum = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                sum += probabilities[i];
            }
            if (sum != 100)
            {
                Console.WriteLine("Сумма вероятностей не равна 100!");
            }

            int j = 0, k = 0;
            for (int i = 0; i < values.Length; i++)
            {
                while (j < probabilities[i] && k < 100)
                {
                    section[k] = values[i];
                    j++;
                    k++;
                }
                j = 0;
            }
        }

        public int Next()
        {
            return section[random.Next(0, 100)];
        }


    }
}
