using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightningTalks
{
    public partial class Clock2 : Form
    {
        public Clock2()
        {
            InitializeComponent();
        }

        #region Formの拡張スタイル設定

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
        #endregion

        #region Win32API定義
        // UpdateLayeredWindow関連のAPI定義
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        public static extern int DeleteObject(IntPtr hobject);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

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

        #region レイヤードウィンドウを設定する
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
        HotKey hotKey;
        float clockAlpha=1;

        MouseHook mh = new MouseHook();
        #region ページロード

        private void Clock_Load(object sender, EventArgs e)
        {
            mh.MouseHooked +=  mh_MouseHooked;
            hotKey = new HotKey(MOD_KEY.ALT | MOD_KEY.CONTROL | MOD_KEY.SHIFT, Keys.F);
            hotKey.HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            this.TopMost = true;
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

            //影
            g.FillEllipse(gb3, rect);

            rect.Inflate(-15, -15);

            //外枠黒
            g.FillEllipse(gb2, rect);
            rect.Inflate(-1, -1);
            g.FillEllipse(gb1, rect);
            rect.Inflate(-6, -6);
            //枠基本色のグレー
            g.FillEllipse(gb2, rect);
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
                //メモリの描画
                g.DrawLine(Pens.Black, mathUtil.getPoint(center, rect.Width / 2 - 10, 360 / 60 * i), mathUtil.getPoint(center, rect.Width / 2, 360 / 60 * i));
                //数字の描画
                if (i % 5 == 0)
                {
              
                    g.DrawString((i / 5 + 1).ToString(), new Font("MS PGOTHIC", 20), new SolidBrush(Color.Black), mathUtil.getPoint(center, rect.Width / 2 - 20, 360 / 60 * (i - 10)),strFmt);
                }
            }

            //枠を消す
            this.FormBorderStyle = FormBorderStyle.None;

            //画像を保存
            bmpClockBase = bmp;
            SetLayeredWindow(bmp);
            g.Dispose();

        }

        private void mh_MouseHooked(object sender, MouseHookedEventArgs e)
        {
            if (e.Message == MouseMessage.Move)
            {
                if (mathUtil.getDistance(Cursor.Position,new Point(this.Location.X+this.Width/2,this.Location.Y+this.Height/2))<=CLOCK_RADIUS)
                {
                    clockAlpha = mathUtil.getDistance(Cursor.Position, new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2))/100f;
                }
                else

                {
                    clockAlpha = 1f;
                }
            }
        }

        #endregion
        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            //ウィンドウの拡張スタイルを再設定する(このソースの先頭でやっている拡張スタイルの設定と同じ)
            //getWindowLongで現在の拡張スタイルに、0x20 (EX_TRANSPARENT)を排他的論理和(^)したものを設定するので、
            //ショートカットキーを押すたびにOnOffが切り替わる
            SetWindowLong(this.Handle, -20, GetWindowLong(this.Handle, -20) ^0x20);        
    }


        #region タイマー
        private void timer1_Tick(object sender, EventArgs e)
        {
            //描画用
            Bitmap bmp = new Bitmap(bmpClockBase);
            //透過後はりつけ用]
            Bitmap bmpalpha = new Bitmap(this.Width, this.Height);

            Graphics g = Graphics.FromImage(bmp);
            Graphics galpha = Graphics.FromImage(bmpalpha);

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

            g.DrawLine(byou, center, mathUtil.getPoint(center, rect.Width / 2 - 35, 360f / 60f * (DateTime.Now.Second + 45) + 360f / 60000f * (DateTime.Now.Millisecond)));
            g.DrawLine(hun, center, mathUtil.getPoint(center, rect.Width / 2 - 45, 360f / 60f * (DateTime.Now.Minute + 45)+ 360f / 60f/60f * (DateTime.Now.Second)));
            g.DrawLine(ji, center, mathUtil.getPoint(center, rect.Width / 2 - 65, 360f / 12f * (DateTime.Now.Hour + 45)+360f / 60f/12f * (DateTime.Now.Minute)));

            //ImageAttributesオブジェクトの作成
            System.Drawing.Imaging.ImageAttributes ia =
                new System.Drawing.Imaging.ImageAttributes();
            //ColorMatrixオブジェクトの作成
            System.Drawing.Imaging.ColorMatrix cm =
                new System.Drawing.Imaging.ColorMatrix();
            //ColorMatrixの行列の値を変更して、アルファ値が0.5に変更されるようにする
            cm.Matrix00 = 1;
            cm.Matrix11 = 1;
            cm.Matrix22 = 1;
            cm.Matrix33 = clockAlpha;
            cm.Matrix44 = 1;
            //ColorMatrixを設定する
            ia.SetColorMatrix(cm);

            galpha.DrawImage((Image)bmp,new Rectangle(0,0,bmp.Width,bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
            SetLayeredWindow(bmpalpha);
            
            g.Dispose();
            hun.Dispose();
            ji.Dispose();
            byou.Dispose();
            bmp.Dispose();
            bmpalpha.Dispose();
            galpha.Dispose();

        }
        #endregion

        #region フォーム移動
        bool downflg = false;
        Point holdpoint;
        private void Clock_MouseUp(object sender, MouseEventArgs e)
        {
            downflg = false;
        }

        private void Clock_MouseMove(object sender, MouseEventArgs e)
        {
            if (downflg)
            {
                this.Location = new Point(Cursor.Position.X - holdpoint.X, Cursor.Position.Y - holdpoint.Y);
            }
        }

        private void Clock_MouseDown(object sender, MouseEventArgs e)
        {
            holdpoint = e.Location;
            downflg = true;
        }
        #endregion

        

    }
}
