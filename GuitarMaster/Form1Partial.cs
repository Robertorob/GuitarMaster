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
    public partial class Form1 : Form
    {
        public static Label[,] labels;
        public Button[,] buttons;
        public Note[,] grifNotes;
        public int[,] gridButtons;//кнопки, которые показывают сетку

        public bool grid = false;

        public int[] minorScale, majorScale, flamencoScale, bluesScale;
        public MyScale selectedScale;
        Note tonica;
        Melody lastMelody, savedMelody;
        double duration = 6;
        int notesCount = 16;

        OutputDevice outputDevice;
        public MediaPlayer player;
        SoundDevices sd;               

        List<Melody> tmpMelodys;
        List<Melody> melodyList;

        private void Form1_Load(object sender, EventArgs e)
        {
            melodyList = new List<Melody>();
            melodyList = Parser.GetAllMelodys();
            for (int i = 0; i < melodyList.Count; i++)
            {
                savedMelodysComboBox.Items.Add(melodyList[i].Name + "(" + melodyList[i].ScaleName.ToString() + ")");
            }
            savedMelodysComboBox.SelectedIndex = 5;

            Melody.Number = melodyList.Count + 1;

            tmpMelodys = new List<Melody>();

            gridButtons = new int[6, 16];

            /* Гаммы можно менять, но их длина должна оставаться прежней.
             * В противном случае, нужно произвести изменения в файле MyRandom.cs
             */
            minorScale = new int[7] { 2, 1, 2, 2, 1, 2, 2 };
            majorScale = new int[7] { 2, 2, 1, 2, 2, 2, 1 };
            flamencoScale = new int[7] { 1, 3, 1, 2, 1, 3, 1 };
            bluesScale = new int[] { 3, 2, 1, 1, 3, 2 };

            int[] scaleIntervals = minorScale;
            ScaleName scaleName = ScaleName.Minor;
            selectedScale = new MyScale(scaleName, scaleIntervals);

            tonica = Note.A3;

            scaleComboBox.SelectedIndex = 0;

            buttons = new Button[6, 16];
            player = new MediaPlayer();
            player.Volume = 0.3;
            outputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
            outputDevice.Open();
            outputDevice.SendProgramChange(Channel.Channel1, Instrument.AcousticGuitarSteel);

            grifNotes = new Note[6, 16];

            //Заполняем 6 струну
            for (int i = 0; i < grifNotes.GetLength(1); i++)
            {
                grifNotes[5, i] = (Note)(40 + i);
            }
            //Заполняем 5 струну
            for (int i = 0; i < grifNotes.GetLength(1); i++)
            {
                grifNotes[4, i] = (Note)(45 + i);
            }
            //Заполняем 4 струну
            for (int i = 0; i < grifNotes.GetLength(1); i++)
            {
                grifNotes[3, i] = (Note)(50 + i);
            }
            //Заполняем 3 струну
            for (int i = 0; i < grifNotes.GetLength(1); i++)
            {
                grifNotes[2, i] = (Note)(55 + i);
            }
            //Заполняем 2 струну
            for (int i = 0; i < grifNotes.GetLength(1); i++)
            {
                grifNotes[1, i] = (Note)(59 + i);
            }
            //Заполняем 1 струну
            for (int i = 0; i < grifNotes.GetLength(1); i++)
            {
                grifNotes[0, i] = (Note)(64 + i);
            }

            int tabindex = 4;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    buttons[i, j] = (Button)this.Controls["s" + (i + 1).ToString() + j.ToString()];
                    buttons[i, j].TabIndex = tabindex;
                    tabindex++;
                }
            }

            //Создание надписей над аппликатурой
            labels = new Label[6, 16]; this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    labels[i, j] = new Label();
                    labels[i, j].AutoSize = true;
                    labels[i, j].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    labels[i, j].ForeColor = System.Drawing.Color.Black;
                    Point p = new Point();
                    p.X = buttons[i, j].Location.X;// +buttons[i, j].Width / 2;
                    p.Y = buttons[i, j].Location.Y;
                    labels[i, j].Location = p;
                    labels[i, j].Name = "label" + i.ToString() + j.ToString();
                    labels[i, j].Size = new System.Drawing.Size(15, 13);
                    //labels[i, j].TabIndex = 113;
                    labels[i, j].Text = "T";
                    labels[i, j].Visible = false;
                    labels[i, j].BackColor = System.Drawing.Color.Transparent;
                    this.Controls.Add(labels[i, j]);
                    labels[i, j].BringToFront();
                }
            }

            for (int j = 0; j < 6; j++)
            {
                for (int k = 0; k < 16; k++)
                {
                    buttons[j, k].Click += grif_Click;
                }
            }
        }

        public static void Replay(MediaPlayer player)
        {
            if (player.Source.ToString() == "file:///D:/Visual_Studio_Projects/GuitarMaster/GuitarMaster/bin/Debug/Chords/Am.m4a")
            {
                player.Open(new Uri(Application.StartupPath + "\\Chords\\F.m4a", UriKind.Absolute));
                player.Play();

                return;
            }
            if (player.Source.ToString() == "file:///D:/Visual_Studio_Projects/GuitarMaster/GuitarMaster/bin/Debug/Chords/F.m4a")
            {
                player.Open(new Uri(Application.StartupPath + "\\Chords\\Dm.m4a", UriKind.Absolute));
                player.Play();
                return;
            }
            if (player.Source.ToString() == "file:///D:/Visual_Studio_Projects/GuitarMaster/GuitarMaster/bin/Debug/Chords/Dm.m4a")
            {
                player.Open(new Uri(Application.StartupPath + "\\Chords\\E.m4a", UriKind.Absolute));
                player.Play();
                return;
            }
        }

        private void generateButton_Click(object sender, EventArgs e)
        {

            int[] notes = Notes.GetNotes(Notes.Chords.Am, 1);
            int[] rhythm = Rhythm.GetRhythm(6, 4);

            //testTextBox.Text = Parser.GetString(notes);

            player.Open(new Uri(Application.StartupPath + "\\Chords\\Am.m4a", UriKind.Absolute));
            player.Play();

            SoundDevices sd = new SoundDevices(outputDevice, Channel.Channel1);

            MelodyPlayer.PlayMelody(sd, notes, player);

            notes = Notes.GetNotes(Notes.Chords.F, 2);
            //testTextBox.Text = Parser.GetString(notes);

            MelodyPlayer.PlayMelody(sd, notes, player);

            notes = Notes.GetNotes(Notes.Chords.Dm, 2);
            //testTextBox.Text = Parser.GetString(notes);

            MelodyPlayer.PlayMelody(sd, notes, player);

            notes = Notes.GetNotes(Notes.Chords.E, 4);
            //testTextBox.Text = Parser.GetString(notes);

            MelodyPlayer.PlayMelody(sd, notes, player);
        }

        private void VisibleCoordButtons(Button b, MouseEventArgs e, PictureBox p, int i, int j)
        {
            if (e.X > b.Location.X - p.Location.X && e.X < b.Location.X + b.Size.Width - p.Location.X && e.Y > b.Location.Y - p.Location.Y && e.Y < b.Location.Y + b.Size.Height - p.Location.Y)
                b.Visible = true;
            else
            {
                if (gridButtons[i, j] == 1)
                    return;
                b.Visible = false;
            }
        }

        public void DrawGrid(int i, int j, int selectedIndex)//Рисуем сетку
        {
            int jcopy = j;
            int[] scaleIntervals = selectedScale.scaleIntervals;

            for (int m = i; m < 6; m++)
            {
                int index = -1;
                //Идем по струне вправо, используя шаблон
                for (int k = j; k < 16; k += scaleIntervals[index % scaleIntervals.Length])
                {
                    if (k == j || (k - j) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index++;
                }

                index = scaleIntervals.Length;////////////////////////////
                //Идем по струне влево, используя шаблон
                for (int k = j; k >= 0; k -= scaleIntervals[Math.Abs(index % scaleIntervals.Length)])
                {
                    if ((j - k) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index--;
                    if (index == -1)
                        index = scaleIntervals.Length - 1;///////////////////////////////
                }

                //Смещение ладов при переходе на следующую струну
                //Движемся от тонких струн к басовым
                switch (m + 1)
                {
                    case 1://следующая струна - вторая
                        if (j + 5 < 16)
                            j += 5;
                        else
                            j -= 7;
                        break;
                    case 2://следующая струна - третья
                        if (j + 4 < 16)
                            j += 4;
                        else
                            j -= 8;
                        break;
                    case 3:
                        if (j + 5 < 16)
                            j += 5;
                        else
                            j -= 7;
                        break;
                    case 4:
                        if (j + 5 < 16)
                            j += 5;
                        else
                            j -= 7;
                        break;
                    case 5:
                        if (j + 5 < 16)
                            j += 5;
                        else
                            j -= 7;
                        break;
                }
            }

            j = jcopy;
            //Идем по каждой струне. От выбранной к тонким
            for (int m = i; m >= 0; m--)
            {
                int index = -1;
                //Идем по струне вправо, используя шаблон
                for (int k = j; k < 16; k += scaleIntervals[index % scaleIntervals.Length])
                {
                    if (k == j || (k - j) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index++;
                }

                index = scaleIntervals.Length;//////////////////////////////////
                //Идем по струне вправо, используя шаблон
                for (int k = j; k >= 0; k -= scaleIntervals[Math.Abs(index % scaleIntervals.Length)])
                {
                    if ((j - k) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index--;
                    if (index == -1)
                        index = scaleIntervals.Length - 1;///////////////////////////////
                }

                //Смещение ладов при переходе на следующую струну
                //Движемся от басовых струн к тонким
                switch (m - 1)
                {
                    case 0://следующая струна - первая
                        if (j + 7 < 16)
                            j += 7;
                        else
                            j -= 5;
                        break;
                    case 1:
                        if (j + 8 < 16)
                            j += 8;
                        else
                            j -= 4;
                        break;
                    case 2:
                        if (j + 7 < 16)
                            j += 7;
                        else
                            j -= 5;
                        break;
                    case 3:
                        if (j + 7 < 16)
                            j += 7;
                        else
                            j -= 5;
                        break;
                    case 4:
                        if (j + 7 < 16)
                            j += 7;
                        else
                            j -= 5;
                        break;
                }
            }
        }


        private void accompButton_Click(object sender, EventArgs e)
        {
            Accompaniment.PlayAccompanement(outputDevice);
        }      

    }
}
