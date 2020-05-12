using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Malevich
{
    abstract class Figure
    {
        /// <summary>
        /// Вершины фигуры. Координаты относительны левого верхнего.
        /// </summary>
        public List<Point> Points { get; private set; }

        /// <summary>
        /// Координаты левого верхнего угла 
        /// </summary>
        public Point Start 
        { 
            get => start;
            set
            {
                start = value;
            }
        }
        private Point start;

        #region Оформление
        /// <summary>
        /// Цвет пера и заливки
        /// </summary>
        public virtual Color MyColor 
        {
            get => pen.Color;
            set
            {
                pen.Color = value;
                brush = new SolidBrush(value);
            }
        }

        /// <summary>
        /// Толщина пера
        /// </summary>
        public float WidthLine
        {
            get => pen.Width;
            set
            {
                pen.Width = value;
            }
        }

        protected Pen pen;
        protected Brush brush;

        /// <summary>
        /// Отображение на холсте
        /// </summary>
        public bool IsShown = true;

        /// <summary>
        /// Задаёт способ рисования. 
        /// Если true, рисует
        /// Если false, заливает
        /// </summary>
        public bool IsDraw = true;

        //Перенос логики вметоды. 
        //Только чтение
        /// <summary>
        /// Длина
        /// </summary>
        public int W
        {
            get => w;
            set
            {
                if (value > 0)
                {
                    w = value;
                }
            }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public int H
        {
            get => h;
            set
            {
                if (value > 0)
                {
                    h = value;
                }
            }
        }

        protected int w = FormMain.BlockSize; 
        protected int h = FormMain.BlockSize;

        protected int size = FormMain.BlockSize;
        #endregion

        #region Конструкторы
        public Figure(Color c, params Point[] points)
        {
            Points = points.ToList();
            pen = new Pen(c, 1);
            brush = new SolidBrush(c);
            MyColor = c;
        }

        public Figure(Color c):this(c, new Point[0]) {}

        public Figure(params Point[] points) :this(Color.Black, points) { }

        public Figure(Color c, bool auto = false) :this(c)
        {
           
        }

        public Figure(Figure f):this(f.MyColor, f.Points.ToArray()) { }

        #endregion

        #region Расчёты
        /// <summary>
        /// Считает площадь фигуры
        /// </summary>
        /// <returns>Площадь фигуры</returns>
        public virtual double Area()
        {
            return MyMath.Area(this);
        }

        /// <summary>
        /// Считает периметр фигуры
        /// </summary>
        /// <returns>Периметр фигуры</returns>
        public virtual double Perimetr()
        {
            return MyMath.Perimetr(this);
        }
        #endregion

        #region Изменения
        /// <summary>
        /// Масштабирует фигуру с помощью коэффициента
        /// </summary>
        /// <param name="k">Коэффициент увеличения</param>
        public void Scale(double k)
        {
            w = Convert.ToInt32(Math.Round(w * k));
            h = Convert.ToInt32(Math.Round(h * k));
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point((int)Math.Round(Points[i].X * k), Points[i].Y);
                Points[i] = new Point(Points[i].X, (int)Math.Round(Points[i].Y * k));
            }
        }

        /// <summary>
        /// Масштабирует фигуру с помощью высоты и длины
        /// </summary>
        /// <param name="w">Новая высота фигуры</param>
        /// <param name="h">Новая длина фигуры</param>
        public void Scale(int w, int h)
        {
            double kx = (double)w / (double)this.w;
            ScaleX(kx);
            W = w;

            double ky = (double)h / (double)this.h;
            ScaleY(ky);
            H = h;
        }

        /// <summary>
        /// Масштабирует длину фигуры
        /// </summary>
        /// <param name="k">Коэффициент увеличения</param>
        private void ScaleX(double k)
        {
            for(int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point((int)Math.Round(Points[i].X*k), Points[i].Y);
            }
        }

        /// <summary>
        /// Масштабирует высоту фигуры
        /// </summary>
        /// <param name="k">Коэффициент увеличения</param>
        private void ScaleY(double k)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point(Points[i].X, (int)Math.Round(Points[i].Y * k));
            }
        }

        /// <summary>
        /// Перемещает фигуру
        /// </summary>
        /// <param name="vX">Смещение по оси абсциссы</param>
        /// <param name="vY">Смещение по оси ординаты</param>
        public void Move(int vX, int vY)
        {
            start.Offset(vX, vY);
        }
        #endregion

        public void Paint(Bitmap bmp)
        {
            if (IsShown)
            {
                if (IsDraw)
                    Draw(bmp);
                else
                    Fill(bmp);
            }
        }

        /// <summary>
        /// Рисует фигуруна заданном Bitmap
        /// </summary>
        /// <param name="bmp">Bitmap, на котором отображается фигура</param>
        protected virtual void Draw(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawPolygon(pen, MyMath.MovePoints(Points.ToArray(), start));
        }

        /// <summary>
        /// Заливает фигуру на заданном Bitmap
        /// </summary>
        /// <param name="bmp">Bitmap, на котором отображается фигура</param>
        protected virtual void Fill(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillPolygon(brush, MyMath.MovePoints(Points.ToArray(), start));
        }

        /// <summary>
        /// Создаёт строку с информацией о фигуре
        /// </summary>
        /// <returns>Строка с информацией о фигуре</returns>
        public override string ToString()
        {
            string name = $"{GetType().ToString().Split('.')[1]}: Координаты: X:{Start.X} Y:{Start.Y}; Площадь:{Area()}; Пермиетр:{Perimetr()};";
            if (IsShown)
                return "•" + name;
            else
                return "◘" + name;
        }

    }
}
