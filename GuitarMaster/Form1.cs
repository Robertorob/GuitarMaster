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

/* 
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
 * Можно вбить свою мелодию и сыграть её. На входе: ритм, ноты(СДЕЛАНО)
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
        CancellationTokenSource cts;
        Task task;
        private void newGenerateButton_Click(object sender, EventArgs e)
        {
            SetEnable(false, SetEnableMode.All);

            duration = Rhythm.GetDuration(tempoTrackBar.Value, notesCount);
            Random random = new Random();
            int divisor = random.Next(3, 5), addition = random.Next(1, 4);

            int[] rhythm = Rhythm.GetRhythm(notesCount + notesCount / divisor + addition, notesCount);
            int[] notes = Notes.NewGetNotes(selectedScale, notesCount, rhythm);

            lastMelody = new Melody(Melody.Number + "-ая мелодия", notes, rhythm, selectedScale.scaleName);
            generatedMelodysList.Add(lastMelody);
            generatedMelodysComboBox.Items.Add(lastMelody.Name + "(" + lastMelody.ScaleName.ToString() + ")");
            generatedMelodysComboBox.SelectedIndex = generatedMelodysList.IndexOf(lastMelody);
            Melody.Number++;

            rhythmTextBox.Text = Parser.GetString(rhythm);
            notesTextBox.Text = Parser.GetString(notes);

            if (sd == null)
                sd = new SoundDevices(outputDevice, Channel.Channel1);

            SetLabelsNameNotesAndRhythm(lastMelody);
            saveMelodyButton.Enabled = true;

            cts = new CancellationTokenSource();
            stopButton.Select();
            timer.Start();
            task = Task.Run(() =>
            {
                MelodyPlayer.PlayMelodyWithRhythm(sd, lastMelody, tonica, duration, grifNotes, buttons, cts.Token);
            }, cts.Token);
           
        }

        private void playAgainButton_Click(object sender, EventArgs e)
        {
            SetEnable(false, SetEnableMode.All);

            if (lastMelody != null)
            {
                if (sd == null)
                    sd = new SoundDevices(outputDevice, Channel.Channel1);

                SetLabelsNameNotesAndRhythm(lastMelody);
                saveMelodyButton.Enabled = true;

                duration = Rhythm.GetDuration(tempoTrackBar.Value, notesCount);
                cts = new CancellationTokenSource();
                stopButton.Select();
                timer.Start();
                task = Task.Run(() =>
                {
                    MelodyPlayer.PlayMelodyWithRhythm(sd, lastMelody, tonica, duration, grifNotes, buttons, cts.Token);
                }, cts.Token);
            }
        }

        public void SetLabelsNameNotesAndRhythm(Melody melody)
        {
            nameLabel.Text = melody.Name + " (" + melody.ScaleName.ToString() + ")";

            notesTextBox.Text = Parser.GetString(melody.Notes);
            rhythmTextBox.Text = Parser.GetString(melody.Rhythm);
            notesCountTextBox.Text = melody.Notes.Length.ToString();
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
            if (!int.TryParse(notesCountTextBox.Text, out res) && notesCountTextBox.Text != "")
            {
                MessageBox.Show("Вводите целое число!");
                notesCountTextBox.Text = "8";
                notesCountTextBox.SelectAll();
                return;
            }
            if(notesCountTextBox.Text != "")
                notesCount = int.Parse(notesCountTextBox.Text);
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
            SetEnable(false, SetEnableMode.All);

            int[] notes = Parser.GetMassive(notesTextBox.Text);
            int[] rhythm = Parser.GetMassive(rhythmTextBox.Text);

            if (notes.Length == 0 || rhythm.Length == 0)
            {
                MessageBox.Show("Неверный формат мелодии!");
                return;
            }

            notesCountTextBox.Text = notes.Length.ToString();
            duration = Rhythm.GetDuration(tempoTrackBar.Value, notes.Length);

            if(sd == null)
                sd = new SoundDevices(outputDevice, Channel.Channel1);

            Melody melody = new Melody("My melody", notes, rhythm, ScaleName.Other);
            cts = new CancellationTokenSource();
            stopButton.Select();
            timer.Start();
            task = Task.Run(() =>
            {
                MelodyPlayer.PlayMelodyWithRhythm(sd, melody, tonica, duration, grifNotes, buttons, cts.Token);
            }, cts.Token);
        }

        private void savedMelodysComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = savedMelodysComboBox.SelectedIndex;
            if (i < 0 || i >= savedMelodysList.Count)
                return;
            savedMelody = savedMelodysList[i];

            SetLabelsNameNotesAndRhythm(savedMelody);
            saveMelodyButton.Enabled = false;
        }

        private void playSavedMelodyButton_Click(object sender, EventArgs e)
        {
            SetEnable(false, SetEnableMode.All);

            if (savedMelody != null)
            {
                if (sd == null)
                    sd = new SoundDevices(outputDevice, Channel.Channel1);

                SetLabelsNameNotesAndRhythm(savedMelody);
                saveMelodyButton.Enabled = false;

                duration = Rhythm.GetDuration(tempoTrackBar.Value, savedMelody.Notes.Length);
                cts = new CancellationTokenSource();
                stopButton.Select();
                timer.Start();
                task = Task.Run(() =>
                {
                    MelodyPlayer.PlayMelodyWithRhythm(sd, savedMelody, tonica, duration, grifNotes, buttons, cts.Token);
                }, cts.Token);
            }
        }

        private void generatedMelodysComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = generatedMelodysComboBox.SelectedIndex;
            if (i < 0 || i >= generatedMelodysList.Count)
                return;
            lastMelody = generatedMelodysList[i];

            SetLabelsNameNotesAndRhythm(lastMelody);
            saveMelodyButton.Enabled = true;
        }

        private void playGeneratedMelodyButton_Click(object sender, EventArgs e)
        {
            SetEnable(false, SetEnableMode.All);

            if (lastMelody != null)
            {
                if (sd == null)
                    sd = new SoundDevices(outputDevice, Channel.Channel1);

                SetLabelsNameNotesAndRhythm(lastMelody);
                saveMelodyButton.Enabled = true;

                duration = Rhythm.GetDuration(tempoTrackBar.Value, lastMelody.Notes.Length);
                cts = new CancellationTokenSource();
                stopButton.Select();
                timer.Start();
                task = Task.Run(() =>
                {
                    MelodyPlayer.PlayMelodyWithRhythm(sd, lastMelody, tonica, duration, grifNotes, buttons, cts.Token);
                }, cts.Token);
            }
        }

        private void saveMelodyButton_Click(object sender, EventArgs e)
        {
            if (lastMelody != null)
            {
                if (!savedMelodysList.Contains(lastMelody))
                {
                    SetLabelsNameNotesAndRhythm(lastMelody);
                    saveMelodyButton.Enabled = true;

                    Parser.SaveMelodyToFile(lastMelody);
                    savedMelodysList.Add(lastMelody);
                    savedMelodysComboBox.Items.Add(lastMelody.Name + "(" + lastMelody.ScaleName.ToString() + ")");
                    MessageBox.Show("Мелодия \"" + lastMelody.Name + "(" + lastMelody.ScaleName.ToString() + ")" + "\" успешно сохранена");
                }
                else
                {
                    SetLabelsNameNotesAndRhythm(lastMelody);
                    saveMelodyButton.Enabled = true;
                    MessageBox.Show("Эта мелодия уже сохранена!");
                }
            }
        }

        private void deleteSavedMelodyButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Действительно хотите удалить мелодию\n\"" + savedMelody.Name + "(" + savedMelody.ScaleName.ToString() + ")\"" + "?", "Внимание", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                savedMelodysList.Remove(savedMelody);               
                Parser.RewriteMelodysFile(savedMelodysList);

                int i = savedMelodysComboBox.SelectedIndex;
                savedMelodysComboBox.Items.RemoveAt(i);
                if (i - 1 >= 0)
                    savedMelodysComboBox.SelectedIndex = i - 1;
            }
        }

        private void editGeneratedMelodyButton_Click(object sender, EventArgs e)
        {
            if (!editGenerated && lastMelody != null)
            {
                editGeneratedMelodyButton.Text = "Применить";
                nameTextBox.Text = lastMelody.Name;
                SetLabelsNameNotesAndRhythm(lastMelody);

                nameTextBox.Visible = true;
                editGenerated = true;

                SetEnable(false, SetEnableMode.Generated);       
                return;
            }
            if(editGenerated && lastMelody != null)
            {               
                int[] notes = Parser.GetMassive(notesTextBox.Text);
                int[] rhythm = Parser.GetMassive(rhythmTextBox.Text);
                if (notes == null || rhythm == null)
                {
                    MessageBox.Show("Неверный формат мелодии. Подробности смотрите в описании программы");
                    notesTextBox.Text = Parser.GetString(lastMelody.Notes);
                    rhythmTextBox.Text = Parser.GetString(lastMelody.Rhythm);
                }
                else
                {
                    if (MessageBox.Show("Действительно хотите изменить мелодию\n\"" + lastMelody.Name + "(" + lastMelody.ScaleName.ToString() + ")\"" + "?", "Внимание", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        int i = generatedMelodysList.IndexOf(lastMelody);
                        generatedMelodysComboBox.Items.RemoveAt(i);
                        generatedMelodysComboBox.Items.Insert(i, nameTextBox.Text + "(Other)");

                        int j = generatedMelodysList.IndexOf(lastMelody);
                        generatedMelodysList.RemoveAt(i);
                        lastMelody = new Melody(nameTextBox.Text, notes, rhythm, ScaleName.Other);
                        generatedMelodysList.Insert(j, lastMelody);

                        /* Здесь срабатывает событие и меняется значение notesCountTextBox */
                        generatedMelodysComboBox.SelectedIndex = i;
                        nameLabel.Text = lastMelody.Name + "(Other)";
                    }
                    else
                    {
                        notesTextBox.Text = Parser.GetString(lastMelody.Notes);
                        rhythmTextBox.Text = Parser.GetString(lastMelody.Rhythm);
                    }
                }
                editGeneratedMelodyButton.Text = "Изменить";
                nameTextBox.Visible = false;
                editGenerated = false;

                SetEnable(true, SetEnableMode.Generated);                
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editGenerated)
            {
                tabControl1.SelectedIndex = 0;
            }
            if (editSaved)
            {
                tabControl1.SelectedIndex = 1;
            }
        }

        private void editSavedMelodyButton_Click(object sender, EventArgs e)
        {
            if (!editSaved && savedMelody != null)
            {
                editSavedMelodyButton.Text = "Применить";

                SetLabelsNameNotesAndRhythm(savedMelody);
                nameTextBox.Text = savedMelody.Name;

                nameTextBox.Visible = true;
                editSaved = true;

                SetEnable(false, SetEnableMode.Saved);
                return;
            }
            if (editSaved && savedMelody != null)
            {
                int[] notes = Parser.GetMassive(notesTextBox.Text);
                int[] rhythm = Parser.GetMassive(rhythmTextBox.Text);
                if (notes == null || rhythm == null)
                {
                    MessageBox.Show("Неверный формат мелодии. Подробности смотрите в описании программы");
                    notesTextBox.Text = Parser.GetString(savedMelody.Notes);
                    rhythmTextBox.Text = Parser.GetString(savedMelody.Rhythm);
                }
                else
                {
                    if (MessageBox.Show("Действительно хотите изменить мелодию\n\"" + savedMelody.Name + "(" + savedMelody.ScaleName.ToString() + ")\"" + "?", "Внимание", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        int i = savedMelodysList.IndexOf(savedMelody);
                        savedMelodysComboBox.Items.RemoveAt(i);
                        savedMelodysComboBox.Items.Insert(i, nameTextBox.Text + "(Other)");

                        int j = savedMelodysList.IndexOf(savedMelody);
                        savedMelodysList.RemoveAt(i);
                        savedMelody = new Melody(nameTextBox.Text, notes, rhythm, ScaleName.Other);
                        savedMelodysList.Insert(j, savedMelody);

                        /* Здесь срабатывает событие и меняется значение notesCountTextBox */
                        savedMelodysComboBox.SelectedIndex = i;
                        nameLabel.Text = savedMelody.Name + "(Other)";
                        Parser.RewriteMelodysFile(savedMelodysList);
                    }
                    else
                    {
                        notesTextBox.Text = Parser.GetString(savedMelody.Notes);
                        rhythmTextBox.Text = Parser.GetString(savedMelody.Rhythm);
                    }
                }

                editSavedMelodyButton.Text = "Изменить";
                nameTextBox.Visible = false;
                editSaved = false;               

                SetEnable(true, SetEnableMode.Saved);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if(cts != null)
                cts.Cancel();
            outputDevice.SilenceAllNotes();
            SetEnable(true, SetEnableMode.All);
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (task != null)
            {
                if (task.IsCanceled || task.IsCompleted)
                {
                    SetEnable(true, SetEnableMode.All);
                    timer.Stop();
                }
            }
        }
        

    }
}
