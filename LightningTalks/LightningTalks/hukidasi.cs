using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightningTalks
{
    public partial class hukidasi : Form
    {
        Point hukidasipoint;
        public hukidasi(Point kairupoint)
        {
            InitializeComponent();
            hukidasipoint = kairupoint;
        }
        public string ReturnValue;
        const int RADIUS = 20;
        const int BORDER = 1;
        const int MARGIN_BOTTOM = 15;
        const int HUKIHUTI_WIDTH = 20;
        private void hukidasi_Load(object sender, EventArgs e)
        {

            this.FormBorderStyle = FormBorderStyle.None;

            this.TransparencyKey = SystemColors.Control;
            this.Location = new Point(hukidasipoint.X - this.Width / 2, hukidasipoint.Y - this.Height);
            this.DoubleBuffered = true;
            this.BackgroundImageLayout = ImageLayout.Center;
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bmp);

            GraphicsPath myPath = new GraphicsPath();

            myPath.StartFigure();
            myPath.AddArc(0, 0, RADIUS, RADIUS, 180, 90);
            myPath.AddArc(this.Width - RADIUS - 1, 0, RADIUS, RADIUS, 270, 90);
            myPath.AddArc(this.Width - RADIUS - 1, this.Height - RADIUS - MARGIN_BOTTOM, RADIUS, RADIUS, 0, 90);
            myPath.AddLine(this.Width / 2 + HUKIHUTI_WIDTH / 2, this.Height - MARGIN_BOTTOM, this.Width / 2, this.Height-1);
            myPath.AddLine(this.Width / 2, this.Height-1, this.Width / 2 - HUKIHUTI_WIDTH / 2, this.Height - MARGIN_BOTTOM);
            myPath.AddArc(0, this.Height-RADIUS - MARGIN_BOTTOM, RADIUS, RADIUS, 90, 90);
            myPath.CloseFigure();

            g.FillPath(new SolidBrush(Color.FromArgb(255, 255, 203)), myPath);
            g.DrawPath(new Pen(Color.FromArgb(0, 0, 0)), myPath);

            g.DrawRectangle(new Pen(Color.FromArgb(234,223,212)),textBox1.Location.X - 1, textBox1.Location.Y - 1, textBox1.Width + 1, textBox1.Height + 1);

            this.BackgroundImage = bmp;
            g.Dispose();
            this.TopMost = true;
            //ボタンのRegion指定
            drawButton(pictureBox1, "検索(S)");
            drawButton(pictureBox2, "オプション(O)");


        }
        const int BUTTON_RADIUS = 2;
        const int BUTTON_BODER = 1;
        private void drawButton(PictureBox b, string caption)
        {
            //GraphicsPathオブジェクトの作成
            GraphicsPath myPath = new GraphicsPath();
            GraphicsPath myPath2 = new GraphicsPath();

            myPath.StartFigure();
            myPath.AddLine(BUTTON_RADIUS, 0, b.Width - BUTTON_RADIUS, 0);
            myPath.AddLine(b.Width, BUTTON_RADIUS, b.Width, b.Height - BUTTON_RADIUS);
            myPath.AddLine(b.Width - BUTTON_RADIUS, b.Height, BUTTON_RADIUS, b.Height);
            myPath.AddLine(0, b.Height - BUTTON_RADIUS, 0, BUTTON_RADIUS);
            myPath.CloseFigure();
            myPath2.StartFigure();
            myPath2.AddLine(BUTTON_RADIUS, 0, b.Width - BUTTON_RADIUS - 1, 0);
            myPath2.AddLine(b.Width - 1, BUTTON_RADIUS, b.Width - 1, b.Height - BUTTON_RADIUS - 1);
            myPath2.AddLine(b.Width - BUTTON_RADIUS - 1, b.Height - 1, BUTTON_RADIUS, b.Height - 1);
            myPath2.AddLine(0, b.Height - BUTTON_RADIUS - 1, 0, BUTTON_RADIUS);
            myPath2.CloseFigure();

            Bitmap bmp = new Bitmap(b.Width, b.Height);
            Graphics g = Graphics.FromImage(bmp);


            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            // g.SmoothingMode = SmoothingMode.HighQuality;
            //g.FillPath(new SolidBrush(Color.FromArgb(50,0, 0, 0)), myPath);
            g.FillPath(new SolidBrush(Color.FromArgb(255,255,203)), myPath);
            g.DrawPath(new Pen(Color.FromArgb(193,191,185)), myPath2);


            g.DrawString(caption, new Font("Microsoft Sans Serif",8), new SolidBrush(Color.Black), b.ClientRectangle, sf);
            b.Image = bmp;
            b.Region = new Region(myPath);

        }
        static public string ShowHukidasi(Point kairupoint)
        {
            hukidasi f = new hukidasi(kairupoint);
            f.ShowDialog();
            string receiveText = f.ReturnValue;
            f.Dispose();
            return receiveText;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.ReturnValue = textBox1.Text;
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
