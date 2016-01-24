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

    public static class Melody
    {
        public enum Chords { Am = 1, Dm = 4, F = 6, E = 5 };
        public static int[] SetOfNotes(Chords chord, int tact)
        {
            Random notesCount = new Random(), positions = new Random();//position - позиция устойчивой ноты
            MyRandom stables = new MyRandom(new int[] { 1, 3, 5 }, new int[] { 33, 33, 34 });
            switch (chord)
            {
                case Chords.Am:
                    stables = new MyRandom(new int[] { 1, 3, 5 }, new int[] { 33, 33, 34 });
                    break;
                case Chords.Dm:
                    stables = new MyRandom(new int[] { 1, 3, 5 }, new int[] { 33, 33, 34 });
                    break;
                case Chords.E:
                    stables = new MyRandom(new int[] { 1, 3, 5 }, new int[] { 33, 33, 34 });
                    break;
                case Chords.F:
                    stables = new MyRandom(new int[] { 3 }, new int[] { 100 });
                    break;
            }

            int length = 6;// notesCount.Next(4, 10);//количество нот во фразе
            int[] phrase = new int[length];
            for (int i = 0; i < phrase.Length; i++)
            {
                phrase[i] = 0;
            }
            int stable = stables.Next();
            int position;

            switch (tact)//2 и 3 такт одинаковы, поэтому в параметры передаем всегда 2
            {
                case 1:
                    phrase[0] = 1;
                    position = positions.Next(1, length);
                    phrase[position] = stable;
                    phrase = Transitions(chord, phrase, stable, position);
                    break;
                case 2:
                    position = positions.Next(0, length);
                    phrase[position] = stable;
                    phrase = Transitions(chord, phrase, stable, position);
                    break;
                case 4:
                    phrase[length - 1] = 1;
                    position = length - 1;
                    stable = 1;
                    phrase = Transitions(chord, phrase, stable, position);
                    break;
            }

            return phrase;
        }

        public static int[] Transitions(Chords chord, int[] phrase, int stable, int position)
        {
            var random = new Random();
            int[] notes;
            switch (chord)
            {
                case Chords.Am:
                    for (int i = position + 1; i < phrase.Length; i++)
                    {
                        if (random.Next(0, 2) == 1)//идем либо вверх либо вниз по гамме
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (phrase[i - 1] + random.Next(1, 3)) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                            }
                        }
                        else
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (Math.Abs(phrase[i - 1] - random.Next(1, 3))) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                            }
                        }
                    }
                    for (int i = position - 1; i >= 0; i--)
                    {
                        if (random.Next(0, 2) == 1)//идем либо вверх либо вниз по гамме
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (phrase[i + 1] + random.Next(1, 3)) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                            }
                        }
                        else
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (Math.Abs(phrase[i + 1] - random.Next(1, 3))) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                            }
                        }
                    }
                    break;

                case Chords.Dm:
                    for (int i = position + 1; i < phrase.Length; i++)
                    {
                        if (random.Next(0, 2) == 1)//идем либо вверх либо вниз по гамме
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (phrase[i - 1] + random.Next(1, 3)) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                                if (phrase[i] == 2)
                                    phrase[i] = random.Next(3, 5);
                            }
                        }
                        else
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (Math.Abs(phrase[i - 1] - random.Next(1, 3))) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                                if (phrase[i] == 2)
                                    phrase[i] = new MyRandom(new int[] { 1, 7 }, new int[] { 50, 50 }).Next();
                            }
                        }
                    }
                    for (int i = position - 1; i >= 0; i--)
                    {
                        if (random.Next(0, 2) == 1)//идем либо вверх либо вниз по гамме
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (phrase[i + 1] + random.Next(1, 3)) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                                if (phrase[i] == 2)
                                    phrase[i] = random.Next(3, 5);
                            }
                        }
                        else
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (Math.Abs(phrase[i + 1] - random.Next(1, 3))) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                                if (phrase[i] == 2)
                                    phrase[i] = new MyRandom(new int[] { 1, 7 }, new int[] { 50, 50 }).Next();
                            }
                        }
                    }
                    break;

                case Chords.E:
                    for (int i = position - 1; i >= 0; i--)
                    {
                        if (random.Next(0, 2) == 1)//идем либо вверх либо вниз по гамме
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (phrase[i + 1] + random.Next(1, 3)) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                                if (phrase[i] == 6)
                                    phrase[i] = new MyRandom(new int[] { 1, 7 }, new int[] { 50, 50 }).Next();
                            }
                        }
                        else
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = (Math.Abs(phrase[i + 1] - random.Next(1, 3))) % 7;
                                if (phrase[i] == 0)
                                    phrase[i] = 7;
                                if (phrase[i] == 6)
                                    phrase[i] = random.Next(4, 6);
                            }
                        }
                    }
                    break;

                case Chords.F:
                    notes = new int[3] { 3, 6, 7 };
                    if (position - 1 >= 0)
                        phrase[position - 1] = 4;
                    for (int i = position + 1; i < phrase.Length; i++)
                    {
                        if (random.Next(0, 2) == 1)//идем либо вверх либо вниз по гамме
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = notes[random.Next(0, 3)];////////////////////////////////////////////
                            }
                        }
                        else
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = notes[random.Next(0, 3)];/////////////////////////////////////////////
                            }
                        }
                    }
                    for (int i = position - 1; i >= 0; i--)
                    {

                        if (random.Next(0, 2) == 1)//идем либо вверх либо вниз по гамме
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = notes[random.Next(0, 3)];/////////////////////////////////////////////
                            }
                        }
                        else
                        {
                            if (phrase[i] == 0)
                            {
                                phrase[i] = notes[random.Next(0, 3)];/////////////////////////////////////////////
                            }
                        }
                    }
                    break;
            }
            return phrase;
        }

        public static void PlayPhrase(OutputDevice output, Channel channel, int[] phrase, MediaPlayer player)
        {
            Thread.Sleep(700);
            for (int i = 0; i < phrase.Length; i++)
            {
                output.SendNoteOn(channel, NoteExtensionMethods.Note(phrase[i], 4), 80);
                System.Threading.Thread.Sleep(440);
                output.SendNoteOff(channel, NoteExtensionMethods.Note(phrase[i], 4), 80);
            }
            System.Threading.Thread.Sleep(500);
            Form1.Replay(player);

        }

    }
}
