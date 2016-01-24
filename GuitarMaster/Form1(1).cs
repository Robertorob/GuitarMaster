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
        public Note[,] grifnotes;
        public int[] minorPattern;
        public int[] majorPattern;
        public bool grid;
        public int[,] gridButtons;//кнопки, которые показывают сетку

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
            int[] pattern = new int[7];
            switch (selectedIndex)
            {
                case 0://Выбран натуральный минор
                    pattern = minorPattern;
                    break;
                case 1://Выбран натуральный мажор
                    pattern = majorPattern;
                    break;

            }
            for (int m = i; m < 6; m++)
            {
                int index = -1;
                //Идем по струне вправо, используя минорный шаблон
                for (int k = j; k < 16; k += pattern[index % 7])
                {
                    if (k == j || (k - j) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index++;
                }

                index = 7;
                //Идем по струне влево, используя минорный шаблон
                for (int k = j; k >= 0; k -= pattern[Math.Abs(index % 7)])
                {
                    if ((j - k) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index--;
                    if (index == -1)
                        index = 6;
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
                //Идем по струне вправо, используя минорный шаблон
                for (int k = j; k < 16; k += pattern[index % 7])
                {
                    if (k == j || (k - j) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index++;
                }

                index = 7;
                //Идем по струне вправо, используя минорный шаблон
                for (int k = j; k >= 0; k -= pattern[Math.Abs(index % 7)])
                {
                    if ((j - k) % 12 == 0)
                    {
                        labels[m, k].Visible = true;
                    }
                    buttons[m, k].Visible = true;
                    gridButtons[m, k] = 1;
                    index--;
                    if (index == -1)
                        index = 6;
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

        private void Form1_Load(object sender, EventArgs e)
        {

            grid = false;

            gridButtons = new int[6, 16];

            minorPattern = new int[7] { 2, 1, 2, 2, 1, 2, 2 };
            majorPattern = new int[7] { 2, 2, 1, 2, 2, 2, 1 };

            patternComboBox.SelectedIndex = 0;

            buttons = new Button[6, 16];
            player = new MediaPlayer();
            player.Volume = 0.3;
            //player.MediaEnded += new EventHandler(player_Ended);
            outputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
            outputDevice.Open();
            outputDevice.SendProgramChange(Channel.Channel1, Instrument.AcousticGuitarSteel);

            grifnotes = new Note[6, 16];

            //Заполняем 6 струну
            for (int i = 0; i < 16; i++)
            {
                grifnotes[5, i] = (Note)(40 + i);
            }
            //Заполняем 5 струну
            for (int i = 0; i < grifnotes.GetLength(1); i++)
            {
                grifnotes[4, i] = (Note)(45 + i);
            }
            //Заполняем 4 струну
            for (int i = 0; i < grifnotes.GetLength(1); i++)
            {
                grifnotes[3, i] = (Note)(50 + i);
            }
            //Заполняем 3 струну
            for (int i = 0; i < grifnotes.GetLength(1); i++)
            {
                grifnotes[2, i] = (Note)(55 + i);
            }
            //Заполняем 2 струну
            for (int i = 0; i < grifnotes.GetLength(1); i++)
            {
                grifnotes[1, i] = (Note)(59 + i);
            }
            //Заполняем 1 струну
            for (int i = 0; i < grifnotes.GetLength(1); i++)
            {
                grifnotes[0, i] = (Note)(64 + i);
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
                    p.X = buttons[i, j].Location.X + buttons[i, j].Width / 2;
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

    }
}
