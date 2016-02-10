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
    public class SoundDevices
    {
        public OutputDevice output;
        public Channel channel;

        public SoundDevices(OutputDevice o, Channel c)
        {
            output = o;
            channel = c;
        }
    }

    public static class MelodyPlayer
    {
        public static Button FindButtonAndChangeColor(Note note, Button[,] buttons, Note[,] grifNotes, out bool visible)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (grifNotes[i, j] == note)
                    {
                        buttons[i, j].BackColor = System.Drawing.Color.Red;
                        visible = buttons[i, j].Visible;
                        buttons[i, j].Visible = true;
                        if (Form1.ActiveForm != null)
                            Form1.ActiveForm.Refresh();
                        return buttons[i, j];
                    }
                }
            }
            visible = false;
            return buttons[0, 0];
        }

        /// <summary>
        /// Проигрывает мелодию в указанном ритме от указанной ноты. 
        /// С фиксированной продолжительностью такта. 
        /// Играется столько нот, сколько звучащих 
        /// нот в ритме(столько, сколько единиц в массиве ритма)
        /// </summary>
        /// <param name="sd">Aggregates OutputDevice and Channel</param>
        /// <param name="notes">Массив нот</param>
        /// <param name="rhythm">Массив, описывающий ритм</param>
        /// <param name="tonica">Нота, от которой играется мелодия</param>
        /// <param name="duration">Продолжительность такта в секундах с точностью до тысячных</param>
        public static void PlayMelodyWithRhythm(SoundDevices sd, int[] notes, int[] rhythm, Note tonica, double duration, Note[,] grifNotes, Button[,] buttons)
        {
            /* Продолжительность такта в миллисекундах */
            int dur = (int)(duration * 1000);

            /* Продолжительность звучания одной ноты(или паузы) */
            int oneNoteDur = dur / rhythm.Length;

            Note note = Note.A0;

            int j = 0;
            for (int i = 0; i < rhythm.Length; i++)
            {
                if (rhythm[i] == 0)
                {
                    System.Threading.Thread.Sleep(oneNoteDur);
                }
                else
                {
                    /* 40 - E2, 
                     * 79 - G5 */

                    int n = ((int)tonica - 1 + notes[j]);

                    while (n > 79)
                        n = n - 12;
                    while (n < 40)
                        n = n + 12;

                    sd.output.SendNoteOff(sd.channel, note, 80);
                    note = (Note)n;

                    sd.output.SendNoteOn(sd.channel, note, 80);
                    bool visible;
                    Button button = FindButtonAndChangeColor(note, buttons, grifNotes, out visible);                    
                    
                    System.Threading.Thread.Sleep(oneNoteDur);                    

                    button.BackColor = default(System.Drawing.Color);
                    if (!visible)
                        button.Visible = false;
                    if(Form1.ActiveForm != null)
                        Form1.ActiveForm.Refresh();

                    j++;
                    if (j == notes.Length)
                        return;
                }
            }
            sd.output.SendNoteOff(sd.channel, note, 80);
        }

        public static void PlayMelodyWithRhythm(SoundDevices sd, Melody melody, Note tonica, double duration, Note[,] grifNotes, Button[,] buttons)
        {
            int[] notes = melody.Notes;
            int[] rhythm = melody.Rhythm;
            PlayMelodyWithRhythm(sd, notes, rhythm, tonica, duration, grifNotes, buttons);
        }

        public static void PlayMelody(SoundDevices sd, int[] phrase, MediaPlayer player)
        {
            Thread.Sleep(700);
            for (int i = 0; i < phrase.Length; i++)
            {
                sd.output.SendNoteOn(sd.channel, MyNote.Note(phrase[i], 4), 80);
                System.Threading.Thread.Sleep(440);
                sd.output.SendNoteOff(sd.channel, MyNote.Note(phrase[i], 4), 80);
            }
            System.Threading.Thread.Sleep(500);
            Form1.Replay(player);

        }
    }
}
