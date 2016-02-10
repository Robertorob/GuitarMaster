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
    public enum Chords { Am = 1, Dm = 4, E = 5, F = 6 }
    public enum Pattern { Minor, Major }
    public enum Direction { Down, Up }
    public static class Accompaniment
    {
        public static void PlayAccord(Note root, Pattern pattern, Direction direction, OutputDevice outputDevice)
        {
            if (pattern == Pattern.Minor)//тоника на 5 струне
            {
                if (direction == Direction.Down)
                {
                    outputDevice.SendNoteOn(Channel.Channel1, root, 80);
                    Thread.Sleep(15);//20
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 7), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 3), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 7), 80);
                }
                else
                {
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 7), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 3), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 7), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, root, 80);
                }
            }
            else //Мажор, тоника на 6 струне
            {
                if (direction == Direction.Down)
                {
                    outputDevice.SendNoteOn(Channel.Channel1, root, 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 7), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 4), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 7), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 24), 80);
                }
                else
                {
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 24), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 7), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12 + 4), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 12), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, (Note)((int)root + 7), 80);
                    Thread.Sleep(15);
                    outputDevice.SendNoteOn(Channel.Channel1, root, 80);
                }
            }

        }

        public static void PlayTact(Note root, Pattern pattern, OutputDevice outputDevice)
        {
            PlayAccord(root, pattern, Direction.Down, outputDevice);
            Thread.Sleep(400);
            PlayAccord(root, pattern, Direction.Down, outputDevice);
            Thread.Sleep(150);
            PlayAccord(root, pattern, Direction.Up, outputDevice);
            Thread.Sleep(400);
            PlayAccord(root, pattern, Direction.Up, outputDevice);
            Thread.Sleep(200);
            PlayAccord(root, pattern, Direction.Down, outputDevice);
            Thread.Sleep(150);
            PlayAccord(root, pattern, Direction.Up, outputDevice);
            Thread.Sleep(200);
            PlayAccord(root, pattern, Direction.Down, outputDevice);
            Thread.Sleep(400);
            PlayAccord(root, pattern, Direction.Down, outputDevice);
            Thread.Sleep(150);
            PlayAccord(root, pattern, Direction.Up, outputDevice);
            Thread.Sleep(400);
            PlayAccord(root, pattern, Direction.Up, outputDevice);
            Thread.Sleep(200);
            PlayAccord(root, pattern, Direction.Down, outputDevice);
            //Thread.Sleep(150);
            //PlayAccord(root, pattern, Direction.Up, outputDevice);
            Thread.Sleep(400);
        }

        public static void PlayAccompanement(dynamic outputDevice)
        {
            PlayTact(Note.A2, Pattern.Minor, outputDevice);
            PlayTact(Note.F2, Pattern.Major, outputDevice);
            PlayTact(Note.D3, Pattern.Minor, outputDevice);
            PlayTact(Note.E2, Pattern.Major, outputDevice);
        }
    }
}
