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
//1 5 8 13 0 5
/* Можно вбить свою мелодию. На входе: ритм, ноты, темп
 * А еще можно создать базу данных (или просто текстовые доки) и в нёё класть классные мелодии.
 * 
 * Попробовать разработать алгоритм построения ритма характерный для блюза
 * 
 * Вынести всё это дело в отдельный поток
 * 
 * Добавить приемы: секвенция, вертушка, арпеджио, повышение(понижение) октавы
 * 
 * Пока мы выдаем мелодию только под аккорд тоники, и сама мелодия не зависит от аккорда,
 * играются все ноты подряд,поэтому эта мелодия подходит в основном только под аккорд тоники
 * Для другого аккорда есть два варианта:
 * - для метода GetNotes подавать на вход другой массив интервалов. В нём будет меньше нот
 * - либо переделать метод GetNotes, что не хочется
 * 
 * Сделать метод перехода на следующую ступень. Next(int position, chord, scale). Этот метод также 
 * должен возвращать сдвиг. Т.е. будет задействована марковская цепь,  где position - состояние,
 * и есть матрица вероятностей переходов из одного состояния в другое
 * 
 * Сделать ползунок выбора темпа(СДЕЛАНО)
 * 
 * Мелодию можно проиграть заново, причем:(СДЕЛАНО)
 * - в другом темпе(СДЕЛАНО)
 * - от другой тоники(СДЕЛАНО)
 * 
 * Сделать подсветку проигрываемых нот(СДЕЛАНО)
 * 
 * Добавить блюзовую гамму, протестировать.(СДЕЛАНО)
 * 
 * Доделать shift.Next() для каждой гаммы. Вбить наборы вероятностей (СДЕЛАНО) 
 * 
 * Следующая задача: определить, что будет хранить массив мелоди(сейчас он хранит номера ступеней от 1 до 7)(СДЕЛАНО)
 * Добавить новую гамму. И прорисовать её на грифе (СДЕЛАНО)
 * 
 * Мелодия.
 * Сделать рандомный ритм.(СДЕЛАНО) Также можно задавать темп(СДЕЛАНО)
 * 
 * Задача: переделать программу.
 * 1. Аккомпанемент. Будет играть один аккорд. Еще надо решить, 
 * будет ли мелодия зависеть от аккомпанемента(должна вообще то)
 * Для начала
 * запишем аккорд: минорный, мажорный. Пока что в одной тональности. Как быть для восточных ладов? Для блюза?
 * 2. Гаммы.
 *  2.1 Нужно, чтобы при создании мелодии брались ноты из гаммы.(СДЕЛАНО)
 *  2.2 Сделать добавление своих ладов(на входе: количество нот, интервалы)
 *  2.3 Самому добавить блюзовую гамму, минорную мажорную, восточную
 * 3. Ритм. Сделать хоть какой-то римт(СДЕЛАНО)
 *  3.1 Создание рандомного ритма(СДЕЛАНО)
 *  3.2 Характерные ритмы для блюза, восточной музыки
 *  
 * 4. Сделать, чтобы мелодия игралась от тоники, выбранной на грифе. Т.е. мелодия
 * должна играться в любой тональности(СДЕЛАНО)
 * */
namespace GuitarMaster
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
        }

        private void newGenerateButton_Click(object sender, EventArgs e)
        {
            notesCount = int.Parse(notesCountTextBox.Text);
            duration = Rhythm.GetDuration(tempoTrackBar.Value, notesCount);
            Random random = new Random();
            int divisor = random.Next(3, 5), addition = random.Next(1, 6);

            if (notesCount <= 0)
            {
                MessageBox.Show("Число нот должно быть больше нуля!");
                return;
            }

            int[] rhythm = Rhythm.GetRhythm(notesCount + notesCount / divisor + addition + 1, notesCount);
            int[] notes = Notes.NewGetNotes(selectedScale, notesCount, rhythm);

            lastMelody = new Melody(Melody.Number + "-ая мелодия", notes, rhythm, selectedScale.scaleName);
            tmpMelodys.Add(lastMelody);
            tmpMelodysComboBox.Items.Add(lastMelody.Name + "(" + lastMelody.ScaleName.ToString() + ")");
            Melody.Number++;

            rhythmTextBox.Text = Parser.GetString(rhythm);
            notesTextBox.Text = Parser.GetString(notes);

            if(sd == null)
                sd = new SoundDevices(outputDevice, Channel.Channel1);

            SetNameLabels(lastMelody);
            MelodyPlayer.PlayMelodyWithRhythm(sd, lastMelody, tonica, duration, grifNotes, buttons);
        }

        private void playAgainButton_Click(object sender, EventArgs e)
        {
            if (lastMelody != null)
            {
                if (sd == null)
                    sd = new SoundDevices(outputDevice, Channel.Channel1);
                SetNameLabels(lastMelody);
                MelodyPlayer.PlayMelodyWithRhythm(sd, lastMelody, tonica, duration, grifNotes, buttons);
            }
        }

        public void SetNameLabels(Melody melody)
        {
            nameLabel.Text = melody.Name;
            scaleLabel.Text = melody.ScaleName.ToString();
        }

        private void grifPBox_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    VisibleCoordButtons(buttons[i, j], e, grifPBox, i, j);
                }
            }

        }

        private void grif_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if ((Button)sender == buttons[i, j])
                    {
                        outputDevice.SendNoteOn(Channel.Channel1, grifNotes[i, j], 80);
                        if (!grid)
                        {
                            grid = true;
                            DrawGrid(i, j, scaleComboBox.SelectedIndex);
                            tonica = grifNotes[i, j];
                        }
                        return;
                    }
                }
            }
        }

        private void clearGrid_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    buttons[i, j].Visible = false;
                    gridButtons[i, j] = 0;
                    labels[i, j].Visible = false;
                }
            }

            grid = false;
        }

        private void scaleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int[] scaleIntervals = minorScale;
            ScaleName scaleName = ScaleName.Minor;
            
            switch (scaleComboBox.SelectedIndex)
            {
                case 0://Выбран натуральный минор
                    scaleIntervals = minorScale;
                    scaleName = ScaleName.Minor;
                    break;
                case 1://Выбран натуральный мажор
                    scaleIntervals = majorScale;
                    scaleName = ScaleName.Major;
                    break;
                case 2:
                    scaleIntervals = flamencoScale;
                    scaleName = ScaleName.Flamenco;
                    break;
                case 3:
                    scaleIntervals = bluesScale;
                    scaleName = ScaleName.Blues;
                    break;
            }

            selectedScale = new MyScale(scaleName, scaleIntervals);
        }

        private void notesCountTextBox_TextChanged(object sender, EventArgs e)
        {
            int res;
            if (!int.TryParse(notesCountTextBox.Text, out res))
            {
                MessageBox.Show("Вводите целое число!");
                notesCountTextBox.Text = "16";
                notesCountTextBox.SelectAll();
            }
        }

        private void notesCountTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            notesCountTextBox.SelectAll();
        }

        private void tempoTrackBar_Scroll(object sender, EventArgs e)
        {
            duration = (double)( ((double)tempoTrackBar.Value * (-1) / 10) * notesCount ) / 8;
        }

        private void playYourMelodyButton_Click(object sender, EventArgs e)
        {
            int[] notes = Parser.GetMassive(notesTextBox.Text);
            int[] rhythm = Parser.GetMassive(rhythmTextBox.Text);

            if(notes.Length == 0 || rhythm.Length == 0 || notes.Length < rhythm.Sum())
            {
                MessageBox.Show("Неверный формат мелодии!");
                return;
            }

            notesCount = rhythm.Sum();
            duration = Rhythm.GetDuration(tempoTrackBar.Value, notesCount);

            if(sd == null)
                sd = new SoundDevices(outputDevice, Channel.Channel1);

            Melody melody = new Melody("My melody", notes, rhythm, ScaleName.Other);
            MelodyPlayer.PlayMelodyWithRhythm(sd, melody, tonica, duration, grifNotes, buttons);
        }

        private void savedMelodysComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = savedMelodysComboBox.SelectedIndex;
            if (i < 0 || i >= melodyList.Count)
                return;
            savedMelody = melodyList[i];
            SetNameLabels(savedMelody);
        }

        private void playSavedMelodyButton_Click(object sender, EventArgs e)
        {
            if(sd == null)
                sd = new SoundDevices(outputDevice, Channel.Channel1);

            duration = Rhythm.GetDuration(tempoTrackBar.Value, savedMelody.Notes.Length);
            MelodyPlayer.PlayMelodyWithRhythm(sd, savedMelody, tonica, duration, grifNotes, buttons);
        }

        private void tmpMelodysComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = tmpMelodysComboBox.SelectedIndex;
            if (i < 0 || i >= tmpMelodys.Count)
                return;
            lastMelody = tmpMelodys[i];
            SetNameLabels(lastMelody);
        }

        private void playGeneredMelodyButton_Click(object sender, EventArgs e)
        {
            if (sd == null)
                sd = new SoundDevices(outputDevice, Channel.Channel1);

            duration = Rhythm.GetDuration(tempoTrackBar.Value, lastMelody.Notes.Length);
            MelodyPlayer.PlayMelodyWithRhythm(sd, lastMelody, tonica, duration, grifNotes, buttons);
        }

    }
}
