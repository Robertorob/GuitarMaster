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
    public class SoundDevices
    {
        public OutputDevice output;
        public Channel channel;

        public SoundDevices() { }

        public SoundDevices(OutputDevice o, Channel c)
        {
            output = o;
            channel = c;
        }
    }

    public static class MelodyPlayer
    {        
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
        public static void PlayMelodyWithRhythm(SoundDevices sd, int[] notes, int[] rhythm, Note tonica, double duration)
        {
            int dur = (int)(duration * 1000);
            int oneNoteDur = dur / rhythm.Length;//продолжительность звучания одной ноты(или паузы)

            int j = 0;
            for (int i = 0; i < rhythm.Length; i++)
            {
                if (rhythm[i] == 0)
                    System.Threading.Thread.Sleep(oneNoteDur);
                else
                {
                    /* 40 - E2, 
                     * 79 - G5 */
                    int n = ( (int)tonica - 1 + notes[j] );
                    while (n > 79)
                        n = n - 12;
                    while (n < 40)
                        n = n + 12;

                    Note note = (Note) n;

                    sd.output.SendNoteOn(sd.channel, note, 80);
                    System.Threading.Thread.Sleep(oneNoteDur);
                    sd.output.SendNoteOff(sd.channel, note, 80);
                    j++;
                    if (j == notes.Length)
                        return;
                }
            }
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
