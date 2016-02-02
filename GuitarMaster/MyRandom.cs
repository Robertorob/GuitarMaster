using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarMaster
{
    /// <summary>
    /// Класс для генерации псевдослучайных последовательностей
    /// с неравномерным дискретным распределением вероятностей
    /// </summary>
    class MyRandom//:Random
    {
        int[] section;
        Random random;
        /// <summary>
        /// Создает экземпляр класса с набором значений values
        /// и с набором вероятностей probabilities
        /// </summary>
        /// <param name="values">Массив значений. Число значений должно совпадать
        /// с числом вероятностей</param>
        /// <param name="probabilities">Массив вероятностей для массива значений.
        /// Число значений должно совпадать
        /// с числом вероятностей. Вероятность находится в отрезке [0, 100]. 
        /// Сумма вероятностей должна быть равна 100</param>
        public MyRandom(int[] values, int[] probabilities)
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

        public static int[] GetProbabilities(int[] values, ScaleName scaleName)
        {
            int[] probabilities = new int[values.Length];

            /* По умолчанию сделаем равномерное распределение */
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = i + 1;
                probabilities[i] = 100 / probabilities.Length;
            }
            probabilities[0] += 100 - probabilities.Sum();

            if (scaleName != ScaleName.Other && values.Length == 7)
            {
                probabilities = new int[] { 40, 20, 20, 5, 5, 5, 5 };
            }

            if (scaleName == ScaleName.Blues && values.Length == 6)
            {
                probabilities = new int[] { 40, 25, 20, 5, 5, 5 };
            }

            return probabilities;
        }
    }
}
