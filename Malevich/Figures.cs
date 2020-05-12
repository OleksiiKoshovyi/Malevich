using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Malevich
{
    class Triangle : Figure
    {
        public Triangle(Color c, Point p1, Point p2, Point p3) : base(c, p1, p2, p3) { }

        public Triangle(Point p1, Point p2, Point p3) : base(Color.Black, p1, p2, p3) { }

        public Triangle(Color c, bool auto = false) : base(c)
        {
            if (auto)
            {
                Random random = new Random();
                int topX = random.Next(0, FormMain.BlockSize);
                Point top = new Point(topX, 0);

                int leftX = random.Next(0, FormMain.BlockSize / 4);
                int leftY = random.Next(FormMain.BlockSize / 2, FormMain.BlockSize);
                Point left = new Point(leftX, leftY);

                int rightX = random.Next(FormMain.BlockSize / 4 * 3, FormMain.BlockSize);
                int rightY = random.Next(FormMain.BlockSize / 2, FormMain.BlockSize);
                Point right = new Point(rightX, rightY);

                Points.AddRange(new Point[] { top, left, right });
            }
        }

    }

    class HatchTriangle : Triangle
    {
        public HatchTriangle(Color c, bool auto = false) : base(c, auto) 
        { 
            brush = new HatchBrush(
                HatchStyle.BackwardDiagonal,
                MyColor,
                Color.White);

        }

        public override Color MyColor
        {
            get
            {
                return base.MyColor;
            }
            set
            {
                pen.Color = value;
                brush = new HatchBrush(HatchStyle.BackwardDiagonal,value,Color.White);
            }
        }

        protected override void Draw(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillPolygon(brush, MyMath.MovePoints(Points.ToArray(), Start));
            g.DrawPolygon(pen, MyMath.MovePoints(Points.ToArray(), Start));
        }
    }

    class EquilateralTriangle : Figure
    {
        public EquilateralTriangle(Color c, bool auto = false) : base(c)
        {
            if (auto)
            {
                Point top = new Point(FormMain.BlockSize / 2, 0);
                //h*ctg(60*) половина стороны треугольника
                int deltaX  = (int)Math.Round(FormMain.BlockSize / Math.Sqrt(3));
                Point left  = new Point(FormMain.BlockSize/2 - deltaX, FormMain.BlockSize);
                Point right = new Point(FormMain.BlockSize/2 + deltaX, FormMain.BlockSize);
                Points.AddRange(new Point[] { top, left, right });
            }
        }
    }

    class RightTriangle : Figure
    {
        public RightTriangle(Color c, bool auto = false) : base(c)
        {
            if (auto)
            {
                Point top = new Point(0, 0);
                Point left = new Point(0, FormMain.BlockSize);
                Point right = new Point(FormMain.BlockSize, FormMain.BlockSize);
                Points.AddRange(new Point[] { top, left, right });
            }
        }
    }

    class Tetrahedron : Figure
    {
        public Tetrahedron(Color c, bool auto = false) : base(c)
        {
            if (auto)
            {
                Point top = new Point(FormMain.BlockSize / 2, 0);
                Point left = new Point(0, FormMain.BlockSize);
                Point bottom = new Point(FormMain.BlockSize, FormMain.BlockSize);
                Point right = new Point(FormMain.BlockSize, FormMain.BlockSize / 2);
                Points.AddRange(new Point[] { top, left, bottom, right });
            }
        }

        protected override void Draw(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            Point[] NewPoints = MyMath.MovePoints(Points.ToArray(), Start);
            g.DrawPolygon(pen, NewPoints);
            g.DrawLine(pen, NewPoints[0], NewPoints[2]);
            g.DrawLine(pen, NewPoints[1], NewPoints[3]);
        }
    }

    class Dot : Figure
    {
        public Dot(Color c) : base(c, new Point(0,0))
        {
            Scale(5, 5);
        }

        protected override void Draw(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            SolidBrush MyBrush = new SolidBrush(MyColor);
            g.FillEllipse(MyBrush, Start.X, Start.Y, (float)W, (float)H);
        }
    }

    class Line : Figure
    {
        public Line(Color c, bool auto = true) : base(c)
        {
            Point first = new Point(0,FormMain.BlockSize);
            Point last = new Point(FormMain.BlockSize, 0);
            if (auto)
                Points.AddRange(new Point[] {first, last});
        }
    }

    class Rectangle : Figure
    {
        public Rectangle(Color c, bool auto = false) :base(c)
        {
            if (auto)
            {
                Point lt = new Point(0, 0);
                Point rt = new Point(W, 0);
                Point lb = new Point(0, H);
                Point rb = new Point(W, H);
                Points.AddRange(new Point[] { lt, rt, rb, lb });
                pen.DashStyle = DashStyle.Dash;
            }
        }

        public Rectangle(Color c, int w, int h) :this(c)
        {
            Scale(w,h);
        }

        protected override void Draw(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            pen.DashStyle = DashStyle.Dash;
            g.DrawRectangle(pen, Start.X,Start.Y,W,H);            
        }
    }
}
