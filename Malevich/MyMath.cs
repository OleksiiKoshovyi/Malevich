using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Malevich
{
    static class MyMath
    {
        public static double Distance(Point p1, Point p2)
        {
            double w = p1.X - p2.X;
            double h = p1.Y - p2.Y;
            return Math.Sqrt( w * w + h * h );
        }

        /// <summary>
        /// Площадь треугольника по методу Герона
        /// </summary>
        /// <param name="t">Треугольник, площадь которого нужно посчитать</param>
        /// <returns>Площадь треугольника</returns>
        public static double TriangleAreaByHeron (Triangle t)
        {
            List<Point> p = t.Points;
            // Стороны треугольника
            List<double> d = new List<double>(); 
            for (int i = 0; i < p.Count; i++)
            {
                d.Add( Distance(p[i], p[(i + 1) % p.Count]) );
            }
            double pp = d.Sum() / 2; // Полупериметр
            return Math.Sqrt( pp * (pp - d[0]) * (pp - d[1]) * (pp - d[2]) );
        }
        /// <summary>
        /// Площадь треугольника по методу Герона 
        /// </summary>
        /// <param name="p1">Первая точка треугольника</param>
        /// <param name="p2">Вторая точка треугольника</param>
        /// <param name="p3">третья точка треугольника</param>
        /// <returns>Площадь треугольника</returns>
        public static double TriangleAreaByHeron(Point p1, Point p2, Point p3) => 
            TriangleAreaByHeron(new Triangle(p1, p2, p3));

        /// <summary>
        /// Считает площадь правильного треуугольника
        /// </summary>
        /// <param name="a">Сторона треугольника</param>
        /// <returns>Площадь правильного треуугольника</returns>
        public static double EquilateralTriangleArea(double a) => a * a * Math.Sqrt(3) / 4;

        /// <summary>
        /// Считает площадь прямоугольного треугольника
        /// </summary>
        /// <param name="a">Первый катет</param>
        /// <param name="b">Второй катет</param>
        /// <returns>Площадь прямоугольного треугольника</returns>
        public static double RightTriangleArea(double a, double b) => a * b / 2;

        /// <summary>
        /// Считает площадь геометрической фигуры фигуры. В случае трёхмерной фигуры считается площадь проекции.
        /// </summary>
        /// <param name="f">Фигура, площадь которой нужно посчитать</param>
        /// <returns>Площадь фигуры</returns>
        public static double Area(Figure f)
        {
            List<Point> p = f.Points;
            Point bP = p[0];//Базовая точка
            double seed = 0;
            //разбиваем фигуру на триугольники и находим суммарную площадь
            for(int i = 2; i < p.Count; i++)
            {
                seed += TriangleAreaByHeron(bP, p[i - 1], p[i]);
            }
            return Math.Round(seed, 3);
        }

        /// <summary>
        /// Считает периметр геометрической фигуры
        /// </summary>
        /// <param name="f">Фигура, периметр которой нужно посчитать</param>
        /// <returns>Периметр фигуры</returns>
        public static double Perimetr(Figure f)
        {
            List<Point> p = f.Points;
            double seed = 0;
            for(int i = 0; i < p.Count; i++)
            {
                seed += Distance(p[i], p[(i + 1) % p.Count]);
            }
            return Math.Round(seed, 3);
        }

        /// <summary>
        /// Смещает точки в массиве по заданному вектору
        /// </summary>
        /// <param name="arr">Массив точек</param>
        /// <param name="bPoint">Базовая точка или вектор смещения</param>
        /// <returns>Массив смещённых точек</returns>
        public static Point[] MovePoints(Point[] arr, Point bPoint)
        {
            Point[] result = (Point[])arr.Clone();
            for (int i = 0; i < arr.Length; i++)
            {
                result[i].Offset(bPoint.X, bPoint.Y);
            }
            return result;
        }

        /// <summary>
        /// Инициализирует точку сумсарных координат
        /// </summary>
        /// <param name="arrP">Массив точек</param>
        /// <returns>Точка суммарных координат</returns>
        public static Point SumPoints(params Point[] arrP)
        {
            Point res = new Point(0, 0);
            foreach(Point p in arrP)
            {
                res.Offset(p.X, p.Y);
            }
            return res;
        }

        /// <summary>
        /// Отнимает от координат первой точки координаты второй
        /// </summary>
        /// <param name="p1">Первая точка</param>
        /// <param name="p2">Вторая точка</param>
        /// <returns></returns>
        public static Point SubtractPoint(Point p1, Point p2)
        {
            int x = p1.X - p2.X;
            int y = p1.Y - p2.Y;
            return new Point(x, y);
        }

        /// <summary>
        /// Проверяет, находится ли точка внутри квадрата
        /// </summary>
        /// <param name="m">Точка</param>
        /// <param name="rect">Квадрат</param>
        /// <returns>Находится ли точка внутри квадртата</returns>
        public static bool IsMouseHoverSelectedFigure(Point m, Rectangle rect)
        {
            Point s = rect.Start;
            int w = rect.W;
            int h = rect.H;
            return IsMouseHoverSelectedFigure(m, s, w, h);
        }

        /// <summary>
        /// Проверяет, находится ли точка внутри квадрата
        /// </summary>
        /// <param name="m">Точка</param>
        /// <param name="s">Точка - левый верхний угол квадрата</param>
        /// <param name="w">Длина квадрата</param>
        /// <param name="h">Высота квадрата</param>
        /// <returns></returns>
        public static bool IsMouseHoverSelectedFigure(Point m, Point s, int w, int h)
        {
            bool x = m.X >= s.X && m.X <= s.X + w;
            bool y = m.Y >= s.Y && m.Y <= s.Y + h;

            return x && y;
        }
    }
}
