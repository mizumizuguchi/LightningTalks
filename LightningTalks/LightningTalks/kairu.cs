using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace LightningTalks
{
    public partial class kairu : Form
    {
        public kairu()
        {
            InitializeComponent();
        }

        private void kairu_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.BackgroundImageLayout = ImageLayout.Center;

            this.TopMost = true;

            kairuact(0, 210, 50);
            timer1.Enabled = true; 
        }
        //Bitmap[] bmpkairu;

        #region "ウィンドウ透過用のものたち"

        // Form作成時にCreateParams.ExStyleにWS_EX_LAYEREDを指定するため
        // Form.CreateParamsをオーバーライドする
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                const int WS_EX_LAYERED = 0x00080000;

                System.Windows.Forms.CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | WS_EX_LAYERED;

                return cp;
            }
        }
        // UpdateLayeredWindow関連のAPI定義
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DeleteObject(IntPtr hobject);

        public const byte AC_SRC_OVER = 0;
        public const byte AC_SRC_ALPHA = 1;
        public const int ULW_ALPHA = 2;

        // UpdateLayeredWindowで使うBLENDFUNCTION構造体の定義
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        // UpdateLayeredWindowを使うための定義
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int UpdateLayeredWindow(
        IntPtr hwnd,
        IntPtr hdcDst,
        [System.Runtime.InteropServices.In()]
        ref Point pptDst,
        [System.Runtime.InteropServices.In()]
        ref Size psize,
        IntPtr hdcSrc,
        [System.Runtime.InteropServices.In()]
        ref Point pptSrc,
        int crKey,
        [System.Runtime.InteropServices.In()]
        ref BLENDFUNCTION pblend,
        int dwFlags);

        #endregion
        #region レイヤーウィンドウを設定する

        // レイヤードウィンドウを設定する
        public void SetLayeredWindow(Bitmap srcBitmap)
        {
            // スクリーンのGraphicsと、hdcを取得
            Graphics g_sc = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hdc_sc = g_sc.GetHdc();

            // BITMAPのGraphicsと、hdcを取得
            Graphics g_bmp = Graphics.FromImage(srcBitmap);
            IntPtr hdc_bmp = g_bmp.GetHdc();

            // BITMAPのhdcで、サーフェイスのBITMAPを選択する
            // このとき背景を無色透明にしておく
            IntPtr oldhbmp = SelectObject(hdc_bmp, srcBitmap.GetHbitmap(Color.FromArgb(0)));

            // BLENDFUNCTION を初期化
            BLENDFUNCTION blend = new BLENDFUNCTION();
            blend.BlendOp = AC_SRC_OVER;
            blend.BlendFlags = 0;
            blend.SourceConstantAlpha = 255;
            blend.AlphaFormat = AC_SRC_ALPHA;

            // ウィンドウ位置の設定
            Point pos = new Point(this.Left, this.Top);

            // サーフェースサイズの設定
            Size surfaceSize = new Size(this.Width, this.Height);

            // サーフェース位置の設定
            Point surfacePos = new Point(0, 0);

            // レイヤードウィンドウの設定
            UpdateLayeredWindow(this.Handle, hdc_sc, ref pos, ref surfaceSize, hdc_bmp, ref surfacePos, 0, ref blend, ULW_ALPHA);

            // 後始末
            DeleteObject(SelectObject(hdc_bmp, oldhbmp));
            g_sc.ReleaseHdc(hdc_sc);
            g_sc.Dispose();
            g_bmp.ReleaseHdc(hdc_bmp);
            g_bmp.Dispose();
        }
        #endregion
        const int CLOCK_RADIUS = 115;
        Bitmap bmpClockBase;
        private void Clock_Load(object sender, EventArgs e)
        {
          

        }
        protected int getDistance(PointF p1, PointF p2)
        {
            double distance = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            return (int)distance;
        }
        protected double getdegree(PointF p1, PointF p2)
        {
            double radian = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X); ;
            double degree = radian * 180d / Math.PI;
            return degree;
        }
        //p1から指定した角度で指定した距離進んだ時のp2を求める
        protected PointF getPoint(PointF p1, double distance, double degree)
        {
            PointF p2 = new Point();
            double radian = degree * Math.PI / 180;
            p2.Y = p1.Y + (float)(Math.Sin(radian) * distance);
            p2.X = p1.X + (float)(Math.Cos(radian) * distance);
            return p2;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
         //
        }
        int kairuindex = 0;

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            try

            {
                if(nextct>0)
                {
                    nextct--;
                    if(nextct==0)
                    {
                        kairuact(nextstart, nextend, nextspeed,false);
                    }
                }
                
                if(kairuindex>kairuend)
                {
                    kairuindex = kairustart;
                }
                kairuindex++;
                Bitmap bmp = new Bitmap(@"カイル\カイル (" + (kairuindex) + ").png");
                this.Size = bmp.Size;
                SetLayeredWindow(bmp);
                GC.Collect();
                if(kairuindex == 226)
                {
                    this.Close();
                }
            }
            catch
            {
                kairuindex = kairustart;
            }
        }
        bool mdownflg = false;
        Point holdpoint;
        bool moveflg = false;
        private void kairu_MouseDown(object sender, MouseEventArgs e)
        {
            mdownflg = true;
            holdpoint = e.Location;
        }

        private void kairu_MouseMove(object sender, MouseEventArgs e)
        {
            if (mdownflg)
            {
                if (!moveflg)
                {
                    moveflg = true;
                    kairuact(140, 155, 3);

                    try
                    { f.Close(); }
                    catch { }
                }
                this.Location = new Point(Cursor.Position.X - holdpoint.X, Cursor.Position.Y - holdpoint.Y);
            }
        }

        private void kairu_Move(object sender, EventArgs e)
        {
  
        }

        private void kairu_MouseUp(object sender, MouseEventArgs e)
        {
            mdownflg = false;
            if (moveflg)
            {
                moveflg = false;
                kairuact(0, 210, 50);
            }
        }
        int kairustart = 0;
        int kairuend = 226;
        int nextstart = 0;
        int nextend = 226;
        int nextspeed = 50;
        int nextct = 0;

        private void kairuact(int start, int end, int speed, bool nextbreak = true)
        {
            kairuindex = start;
            kairustart = start;
            kairuend = end;
            timer1.Interval = speed;
            if(nextbreak)
            {
                nextstart = 0;
                nextend = 0;
                nextspeed = 0;
                nextct = 0;
            }
            
        }
        private void kairuact(int start, int end, int speed,int nextstart,int nextend,int nextspeed)
        {
            kairuindex = start;
            kairustart = start;
            kairuend = end;
            timer1.Interval = speed;
            kairuact(start, end, speed);
            this.nextstart = nextstart;
            this.nextend = nextend;
            this.nextspeed = nextspeed;
            nextct = end - start;
        }
        hukidasi f;
        private void kairu_Click(object sender, EventArgs e)
        {
            
            if (!moveflg)
            {
                kairuact(26, 41, 50, 43, 126, 50);
                f = new hukidasi(new Point(this.Location.X + this.Width / 2, this.Location.Y));
                
                string receiveText = hukidasi.ShowHukidasi(new Point(this.Location.X + this.Width / 2, this.Location.Y));
                if(receiveText =="お前を消す方法")
                {
                    kairuact(210, 226,50);
                }
                else
                {
                    kairuact(126, 133, 50, 0, 210, 50);
                }

            }
        }
    }
}
