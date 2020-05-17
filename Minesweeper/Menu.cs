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
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void easyButton_Click(object sender, EventArgs e)
        {
            new Form1(Difficulty.Easy).Show();
            Hide();
        }

        private void mediumButton_Click(object sender, EventArgs e)
        {
            new Form1(Difficulty.Medium).Show();
            Hide();
        }

        private void hardButton_Click(object sender, EventArgs e)
        {
            new Form1(Difficulty.Hard).Show();
            Hide();
        }
    }
}
