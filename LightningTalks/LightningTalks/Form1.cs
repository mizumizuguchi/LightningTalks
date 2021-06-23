using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightningTalks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clock c = new Clock(Convert.ToInt32(((Button)sender).Text.Substring(0,1)));
            c.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Clock2 clock2 = new Clock2();
                clock2.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            kairu k = new kairu();
            k.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {

            Clock c = new Clock(9);
            c.Show();
        }
    }
}
