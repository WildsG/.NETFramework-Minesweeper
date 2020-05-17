using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{   
    struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public partial class Form1 : Form
    {
        Button[,] buttons;
        int[,] values;
        bool[,] visited;
        List<Point> bombPositions = new List<Point>();
        int matrixSize = 10;
        int bombCount = 10;
        Difficulty difficulty;
        bool flag = false;
        int flagsPlaced;
        IDictionary<int, Color> colors = new Dictionary<int, Color>()
        {
             {1, Color.Blue},
             {2, Color.Green},
             {3, Color.Red},
             {4, Color.DarkBlue},
        };

        public Form1(Difficulty diff)
        {
            InitializeComponent();
            KeyPreview = true;
            difficulty = diff;
            InitializeTable();
        }

        private void InitializeTable()
        {
            Rectangle screenRectangle = RectangleToScreen(ClientRectangle);
            int titleHeight = screenRectangle.Top - Top;
            if (difficulty == Difficulty.Easy)
            {
                Size = new Size(300 + 16, 300 + titleHeight + 8);
                matrixSize = 10;
                bombCount = 10;
            }
            if (difficulty == Difficulty.Medium)
            {
                Size = new Size(600 + 16, 600 + titleHeight + 8);
                matrixSize = 20;
                bombCount = 40;
            }
            if (difficulty == Difficulty.Hard)
            {
                Size = new Size(900 + 16, 900 + titleHeight + 8);
                matrixSize = 30;
                bombCount = 90;
            }
            GenerateTable(bombCount);
            GenerateButtons();
        }

        private void GenerateTable(int bombCount)
        {   
            // hack for index out of bounds
            values = new int[matrixSize+1, matrixSize+1];
            visited = new bool[matrixSize, matrixSize];
            Random rnd = new Random();
            while(bombCount > 0)
            {
                int x = rnd.Next(0, matrixSize);
                int y = rnd.Next(0, matrixSize);
                if(values[x,y] != 9)
                {
                    values[x, y] = 9;
                    bombCount--;
                    bombPositions.Add(new Point(x, y));
                }
            }
            for (int x = 0; x < matrixSize; x++)
            {
                for (int y = 0; y < matrixSize; y++)
                {
                    if(values[x,y] == 9)
                    {
                        if (x >= 1 && values[x - 1,y + 1] != 9)
                            values[x - 1,y + 1]++;
                        if (values[x,y + 1] != 9)
                            values[x,y + 1]++;
                        if (values[x + 1,y + 1] != 9)
                            values[x + 1,y + 1]++;
                        if (values[x + 1,y] != 9)
                            values[x + 1,y]++;
                        if (y >= 1 && values[x + 1,y - 1] != 9)
                            values[x + 1,y - 1]++;
                        if (y >= 1 && values[x,y - 1] != 9)
                            values[x,y - 1]++;
                        if (y >= 1 && x >= 1 && values[x - 1,y - 1] != 9)
                            values[x - 1,y - 1]++;
                        if (x >= 1 && values[x - 1,y] != 9)
                            values[x - 1,y]++;
                    }
                }
            }
        }

        private void GenerateButtons()
        {
            buttons = new Button[matrixSize, matrixSize];
            for (int x = 0; x < matrixSize; x++)
            {
                for (int y = 0; y < matrixSize; y++)
                {   
                    Button btn = new Button
                    {
                        Size = new Size { Height = 30, Width = 30 },
                        Location = new System.Drawing.Point { X = x * 30, Y = y * 30 },
                        Text = "",
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold)
                    };

                    foreach(KeyValuePair<int, Color> color in colors)
                    {
                        if(color.Key == values[x,y])
                        {
                            btn.ForeColor = color.Value;
                            break;
                        }
                    }
                    btn.Click += new EventHandler(Button_Click);
                    Controls.Add(btn);
                    buttons[x,y] = btn;
                }
            }
        }

        private void RevealRecursive(int x, int y)
        {

            Revealing(x, y);
            visited[x,y] = true;
            Recursing(x, y);           
        }

        private void Revealing(int x, int y)
        {
            if (x - 1 >= 0 && y + 1 <= matrixSize - 1)
                RevealSingle(x - 1, y + 1);
            if (y + 1 <= matrixSize - 1)
                RevealSingle(x, y + 1);
            if (x + 1 <= matrixSize - 1 && y + 1 <= matrixSize - 1)
                RevealSingle(x + 1, y + 1);
            if (x + 1 <= matrixSize - 1)
                RevealSingle(x + 1, y);
            if (x + 1 <= matrixSize - 1 && y - 1 >= 0)
                RevealSingle(x + 1, y - 1);
            if (y - 1 >= 0)
                RevealSingle(x, y - 1);
            if (y - 1 >= 0 && x - 1 >= 0)
                RevealSingle(x - 1, y - 1);
            if (x - 1 >= 0)
                RevealSingle(x - 1, y);
        }

        private bool RevealSingle(int x, int y)
        {
            buttons[x, y].Image = null;
            if (values[x,y] == 0)
            {
                buttons[x, y].Enabled = false;
                buttons[x, y].Text = "";
            }
            else if(values[x,y] == 9)
            {
                buttons[x, y].Image = Properties.Resources.bomb;
                if (MessageBox.Show("Play Again?", "Game Over", MessageBoxButtons.YesNo) == DialogResult.No)
                    Close();
                else
                    Restart();
                return false;
            }
            else
                buttons[x, y].Text = values[x, y].ToString();
            return true;
        }

        private void Restart()
        {
            flag = false;
            Cursor = Cursors.Default;
            Controls.Clear();
            bombPositions.Clear();
            Array.Clear(values, 0, values.GetLength(0) * values.GetLength(1));
            Array.Clear(buttons, 0, buttons.GetLength(0) * buttons.GetLength(1));
            Array.Clear(visited, 0, visited.GetLength(0) * visited.GetLength(1));
            InitializeTable();
        }

        private void Recursing(int x, int y)
        {
            if (x - 1 >= 0 && y + 1 <= matrixSize - 1 && values[x - 1, y + 1] == 0 && !visited[x - 1, y + 1])
                RevealRecursive(x - 1, y + 1);
            if (y + 1 <= matrixSize - 1 && values[x, y + 1] == 0 && !visited[x, y + 1])
                RevealRecursive(x, y + 1);
            if (x + 1 <= matrixSize - 1 && y + 1 <= matrixSize - 1 && values[x + 1, y + 1] == 0 && !visited[x + 1, y + 1])
                RevealRecursive(x + 1, y + 1);
            if (x + 1 <= matrixSize - 1 && values[x + 1, y] == 0 && !visited[x + 1, y])
                RevealRecursive(x + 1, y);
            if (x + 1 <= matrixSize - 1 && y - 1 >= 0 && values[x + 1, y - 1] == 0 && !visited[x + 1, y - 1])
                RevealRecursive(x + 1, y - 1);
            if (y - 1 >= 0 && values[x, y - 1] == 0 && !visited[x, y - 1])
                RevealRecursive(x, y - 1);
            if (y - 1 >= 0 && x - 1 >= 0 && values[x - 1, y - 1] == 0 && !visited[x - 1, y - 1])
                RevealRecursive(x - 1, y - 1);
            if (x - 1 >= 0 && values[x - 1, y] == 0 && !visited[x - 1, y])
                RevealRecursive(x - 1, y);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            for (int x = 0; x < matrixSize; x++)
            {
                for (int y = 0; y < matrixSize; y++)
                {
                    if(buttons[x, y].Equals(btn))
                    {
                        if (flag && String.IsNullOrEmpty(buttons[x,y].Text))
                        {   
                            if(buttons[x, y].Image != null)
                            {
                                buttons[x, y].Image = null;
                                flagsPlaced--;
                            }
                            else
                            {
                                buttons[x, y].Image = Properties.Resources.flag;
                                flagsPlaced++;
                                checkWinCondition();
                            }
                        }
                        else if (buttons[x, y].Image == null && RevealSingle(x, y))
                            if (values[x, y] == 0)
                                RevealRecursive(x, y);
                    }
                }
            }
        }

        private void checkWinCondition()
        {
            int flagsCorrect = 0;
            foreach(Point point in bombPositions)
                if(buttons[point.X,point.Y].Image != null && values[point.X, point.Y] == 9)
                    flagsCorrect++;
 
            if(bombCount == flagsCorrect)
                if (MessageBox.Show("Play Again?", "You win", MessageBoxButtons.YesNo) == DialogResult.No)
                    Close();
                else
                    Restart();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form form1 = Application.OpenForms["Menu"];
            form1.Show();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.B)
            {
                flag = !flag;
                if (flag)
                    Cursor = new Cursor(Application.StartupPath + "\\flag.ico");
                else
                    Cursor = Cursors.Default;
            }
        }
    }
}
