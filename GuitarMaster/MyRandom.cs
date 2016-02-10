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
                MessageBox.Show("Ошибка программы.\nЧисло объектов и вероятностей не совпадает!");
                return;
            }

            int sum = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                sum += probabilities[i];
            }
            if (sum != 100)
            {
                MessageBox.Show("Ошибка программы.\nСумма вероятностей не равна 100!");
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
                values[i] = i;
                probabilities[i] = 100 / probabilities.Length;
            }
            probabilities[0] += 100 - probabilities.Sum();

            if (scaleName != ScaleName.Other && values.Length == 8)
            {
                probabilities = new int[] { 3, 40, 19, 19, 5, 5, 5, 4 };
            }

            if (scaleName == ScaleName.Flamenco && values.Length == 8)
            {
                probabilities = new int[] { 2, 75, 9, 3, 3, 3, 3, 2 };
            }            

            if (scaleName == ScaleName.Blues && values.Length == 7)
            {
                probabilities = new int[] { 5, 40, 24, 19, 4, 4, 4 };
            }

            return probabilities;
        }
    }
}
