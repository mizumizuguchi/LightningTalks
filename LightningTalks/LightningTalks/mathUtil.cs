using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


   public class mathUtil
    {
    public static int getDistance(PointF p1, PointF p2)
    {
        double distance = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
        return (int)distance;
    }
    public static  double getdegree(PointF p1, PointF p2)
    {
        double radian = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X); ;
        double degree = radian * 180d / Math.PI;
        return degree;
    }
    //p1から指定した角度で指定した距離進んだ時のp2を求める
    public static PointF getPoint(PointF p1, double distance, double degree)
    {
        PointF p2 = new Point();
        double radian = degree * Math.PI / 180;
        p2.Y = p1.Y + (float)(Math.Sin(radian) * distance);
        p2.X = p1.X + (float)(Math.Cos(radian) * distance);
        return p2;
    }
}
