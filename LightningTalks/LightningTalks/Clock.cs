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
    public partial class Clock : Form
    {
        int args;
        public Clock(int args)
        {
            this.args = args;
            InitializeComponent();
        }
        const int CLOCK_RADIUS = 115;
        Bitmap bmpClockBase;
        private void Clock_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.BackgroundImageLayout = ImageLayout.Center;
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            RectangleF rect = new RectangleF(0, 0, CLOCK_RADIUS * 2 - 2, CLOCK_RADIUS * 2 - 2);


            //点(20, 10)を青、点(220, 160)を赤として、徐々に変化する色で塗りつぶすブラシを作成
            var gb1 = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Gray, Color.Silver, 45);
            //Color.LightGray, Color.Gray)
            var gb2 = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Black, Color.DarkGray, 45);
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(rect);
            var gb3 = new System.Drawing.Drawing2D.PathGradientBrush(path);
            gb3.CenterColor = Color.FromArgb(50, 0, 0, 0);
            gb3.SurroundColors = new Color[] { Color.FromArgb(0, 0, 0, 0) };
            gb3.FocusScales = new PointF(0.8f, 0.8f);


            rect.Inflate(-15, -15);
            if (args >= 2)
            {
                //外枠黒
                g.FillEllipse(gb2, rect);
            }
            rect.Inflate(-1, -1);
            if (args >= 2)
            {
                g.FillEllipse(gb1, rect);
            }
            rect.Inflate(-6, -6);
            //枠基本色のグレー   
            if (args >= 2)
            {
                g.FillEllipse(gb2, rect);
            }
            rect.Inflate(-3, -3);
            //時計部分の基本色白
            g.FillEllipse(new SolidBrush(Color.White), rect);

            // 中点
            StringFormat strFmt = new StringFormat();
            strFmt.Alignment = StringAlignment.Center;
            strFmt.LineAlignment = StringAlignment.Center;
            // X軸中央揃え

            PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            for (int i = 0; i < 60; i++)
            {
                if (args >= 3)
                {
                    //メモリの描画
                    g.DrawLine(Pens.Black, mathUtil.getPoint(center, rect.Width / 2 - 10, 360 / 60 * i), mathUtil.getPoint(center, rect.Width / 2, 360 / 60 * i));
                    //数字の描画
                    if (i % 5 == 0)
                    {

                        g.DrawString((i / 5 + 1).ToString(), new Font("MS PGOTHIC", 20), new SolidBrush(Color.Black), mathUtil.getPoint(center, rect.Width / 2 - 20, 360 / 60 * (i - 10)), strFmt);
                    }
                }
            }


            //枠を消す
            if (args >= 5)
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            if (args >= 7)
            {
                //ウィンドウを透過する
                this.TransparencyKey = SystemColors.Control;
            }

            if (args >= 8)
            {
                //最前面に設定する
                this.TopMost = true;
            }

            //画像を保存
            bmpClockBase = bmp;
            g.Dispose();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {


            Bitmap bmp = new Bitmap(bmpClockBase);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            RectangleF rect = new RectangleF(0, 0, CLOCK_RADIUS * 2 - 2, CLOCK_RADIUS * 2 - 2);

            PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            Pen byou = new Pen(Color.Red, 1);
            Pen hun = new Pen(Color.Black, 3);
            Pen ji = new Pen(Color.Black, 5);
            hun.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            ji.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            hun.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            ji.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            if (args >= 4)
            {
                g.DrawLine(byou, center, mathUtil.getPoint(center, rect.Width / 2 - 35, 360f / 60f * (DateTime.Now.Second + 45) + 360f / 60000f * (DateTime.Now.Millisecond)));
                g.DrawLine(hun, center, mathUtil.getPoint(center, rect.Width / 2 - 45, 360f / 60f * (DateTime.Now.Minute + 45) + 360f / 60f / 60f * (DateTime.Now.Second)));
                g.DrawLine(ji, center, mathUtil.getPoint(center, rect.Width / 2 - 65, 360f / 12f * (DateTime.Now.Hour + 45) + 360f / 60f / 12f * (DateTime.Now.Minute)));
            }
            this.BackgroundImage = bmp;
            g.Dispose();
            hun.Dispose();
            ji.Dispose();
            byou.Dispose();

        }
        bool downflg = false;
        Point holdpoint;
        private void Clock_MouseUp(object sender, MouseEventArgs e)
        {
            downflg = false;
        }

        private void Clock_MouseMove(object sender, MouseEventArgs e)
        {
            //位置に影響する上と左の余白のみ取得する
            //左右対称なので/2
            //下の余白は左右の余白と同じなので-marginXしてる
            int marginX = (this.Width - this.ClientRectangle.Width)/2;
            int marginY = (this.Height - this.ClientRectangle.Height)-marginX;
            if (downflg)
            {
                this.Location = new Point(Cursor.Position.X - holdpoint.X - marginX, Cursor.Position.Y - holdpoint.Y - marginY);
            }

        }

        private void Clock_MouseDown(object sender, MouseEventArgs e)
        {

            holdpoint = e.Location;
            downflg = true;
        }

        private void Clock_MouseEnter(object sender, EventArgs e)
        {
            if (args >= 6)
            {
                this.Opacity = 0.5;
            }
        }

        private void Clock_MouseLeave(object sender, EventArgs e)
        {
            if (args >= 6)
            {
                this.Opacity = 1;
            }
        }
    }
}
