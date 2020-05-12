using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Malevich
{
    class Picture : IEnumerable<Figure>
    {

        // PictureBox, на котором размещён холст
        private PictureBox Canvas;

        // Фигуры рисунка
        public List<Figure> Figures;

        // Область выделения
        private Rectangle SelectionFigure;

        /// <summary>
        /// Bitmap, на котором рисуется картинка
        /// </summary>
        public Bitmap Bmp;
        /// <summary>
        /// Длина холста
        /// </summary>
        public int W { get => Canvas.Width; }
        /// <summary>
        /// Ширина холста
        /// </summary>
        public int H { get => Canvas.Height; }

        public Picture(PictureBox pBox)
        {
            Canvas = pBox;
            Bmp = new Bitmap(W,H);
            Figures = new List<Figure>();
            SelectionFigure = new Rectangle(Color.Black) { IsShown = false };
        }

        #region Работа с элементами списка

        public int Count 
        {
            get
            {
                return Figures.Count;
            }
        }

        /// <summary>
        /// Возвращает Фигуру по индексу в списке
        /// </summary>
        /// <param name="i">Индекс</param>
        /// <returns>Фигура</returns>
        public Figure this[int i]
        {
            get
            {
                return Figures[i];
            }

            set
            {
                Figures[i] = value;
            }
        }

        public void Add(Figure f)
        {
            Figures.Add(f);
        }

        public void AddRange(List<Figure> f)
        {
            Figures.AddRange(f);
        }

        public void Remove(Figure f)
        {
            Figures.Remove(f);
        }
        public void Remove(int index)
        {
            if (index >= 0 && index < Count)
                Remove(Figures[index]);
            if (Count == 0)
                Unselect();
        }

        /// <summary>
        /// Меняет два элемента списка с фигурами по индексам
        /// </summary>
        /// <param name="a">Первый индекс</param>
        /// <param name="b">Второй индекс</param>
        public void Swap(int a, int b)
        {
            if (a > Count || b > Count || a < 0 || b < 0)
                throw new InvalidCastException();
            Figure temp = Figures[a];
            Figures[a] = Figures[b];
            Figures[b] = temp;
        }

        /// <summary>
        /// Опусает фигуру на слой ниже
        /// </summary>
        /// <param name="index">Индекс фигуры</param>
        public void Down(int index)
        {
            if (index > 0 && index < Count)
            {
                Up(Figures[index]);
            }
        }
        /// <summary>
        /// Опусает фигуру на слой ниже
        /// </summary>
        /// <param name="index">Фигура</param>
        public void Down(Figure f)
        {
            Down(Figures.IndexOf(f));        
        }

        /// <summary>
        /// Опусает фигуру на слой выше
        /// </summary>
        /// <param name="index">Индекс фигуры</param>
        public void Up(Figure f)
        {
            int index = Figures.IndexOf(f);
            if (index > 0 && index < Count)
            {
                Swap(index, index -1);
            }
        }
        /// <summary>
        /// Опусает фигуру на слой выше
        /// </summary>
        /// <param name="index">Фигура</param>
        public void Up(int index) 
        {
            if (index >= 0 && index < Count - 1)
            {
                Swap(index, index + 1);
            }
        }

        /// <summary>
        /// Опускает фигуру на самый нижний слой
        /// </summary>
        /// <param name="f">Фигура</param>
        public void ToDown(Figure f)
        {
            int index = Figures.IndexOf(f);
            ToDown(index);
        }
        /// <summary>
        /// Опускает фигуру на самый нижний слой
        /// </summary>
        /// <param name="index">Индекс фигуры</param>
        public void ToDown(int index)
        {
            if (index < Count && index > 0)
            {
                while(index != 0)
                    Swap(index, --index);
            }
        }

        /// <summary>
        /// Поднимает фигуру на первый слой
        /// </summary>
        /// <param name="f">Фигура</param>
        public void ToUp(Figure f)
        {
            int index = Figures.IndexOf(f);
            ToUp(index);
        }
        /// <summary>
        /// Поднимает фигуру на первый слой
        /// </summary>
        /// <param name="index">Byltrc abuehs</param>
        public void ToUp(int index)
        {
            if (index < Count-1 && index >= 0)
            {
                while (index != Count - 1)
                    Swap(index, ++index);
            }
        }

        /// <summary>
        /// Слияние двух картинок
        /// </summary>
        /// <param name="p">Вторая картинка</param>
        public void Merge(Picture p)
        {
            Figures.AddRange(p.Figures);
        }
        #endregion

        #region Работа с фигурами
        /// <summary>
        /// Делает фигуру невидимой
        /// </summary>
        /// <param name="f">Фигура</param>
        public void Hide(Figure f)
        {
            Figures.Find(figure => f == figure).IsShown = false;
            Unselect();
        }

        /// <summary>
        /// Делает фигуру отображаемой
        /// </summary>
        /// <param name="f">Фигура</param>
        public void Show(Figure f)
        {
            Figures.Find(figure => f == figure).IsShown = true;
        }

        /// <summary>
        /// Меняет состояние IsShown. Hide/Show
        /// </summary>
        /// <param name="f">Фигура</param>
        public void Blink(Figure f)
        {
            if (f.IsShown)
                Hide(f);
            else
                Show(f);
        }

        /// <summary>
        /// Меняет состояние IsShown. Hide/Show
        /// </summary>
        /// <param name="f">Индекс фигуры</param>
        public void Blink(int index)
        {
            if(index >= 0 && index < Count)
            {
                Blink(Figures[index]);
            }
        }

        /// <summary>
        /// Нарисовать все фигуры
        /// </summary>
        public void DrawAll()
        {
            Bmp = DrawAllOnBitmap();
            SelectionFigure.Paint(Bmp);
            Canvas.Image = Bmp;
            Canvas.Refresh();
        }

        public Bitmap DrawAllOnBitmap()
        {
            Bmp = new Bitmap(W, H);
            foreach (Figure f in Figures)
            {
                f.Paint(Bmp);
            }
            return Bmp;
        }

        /// <summary>
        /// Масштабировать все фигуры
        /// </summary>
        /// <param name="k">Коэффициент масштабирования</param>
        public void ScaleAll(double k) 
        {
            foreach (Figure f in Figures)
            {
                // Новые координаты области фигуры
                int vX = (int)Math.Round(((double)f.Start.X) * k) - f.Start.X;
                int vY = (int)Math.Round(((double)f.Start.Y) * k) - f.Start.Y;
                // Перемещаем её туда
                f.Move(vX, vY);
                // Масштабируем на коэффицент
                f.Scale(k);
            }
            DrawAll();
        }

        /// <summary>
        /// Перемечтить все фигуры
        /// </summary>
        /// <param name="v">Величина вектора</param>
        public void MoveAll(int v)
        {
            foreach (Figure f in Figures)
            {
                f.Move(v,v);
            }
            DrawAll();
        }

        /// <summary>
        /// Считает суммарную площадь 
        /// </summary>
        /// <returns>Суммарная площадь</returns>
        public double TotalArea() => Figures.Sum(f=>f.Area());

        /// <summary>
        /// Считает суммарный периметр
        /// </summary>
        /// <returns>Суммарный периметр</returns>
        public double TotalPerimetr() => Figures.Sum(f => f.Perimetr());

        /// <summary>
        /// Выделяет конкретную фигуру
        /// </summary>
        /// <param name="f">Фигура</param>
        public void Select(Figure f)
        {
            int index = Figures.IndexOf(f);
            Select(index);
        }

        /// <summary>
        /// Выделяет конкретную фигуру
        /// </summary>
        /// <param name="f">Индекс фигуры</param>
        public void Select(int index)
        {
            if (index >= 0 && index < Count)
            {
                Figure f = Figures[index];
                SelectionFigure.Start   = new Point(f.Start.X, f.Start.Y);
                SelectionFigure.W       = f.W;
                SelectionFigure.H       = f.H;
                SelectionFigure.IsShown = true;
            }
        }

        /// <summary>
        /// Снимает выделение
        /// </summary>
        public void Unselect()
        {
            SelectionFigure.IsShown = false;
        }
        #endregion

        #region Moving

        /// <summary>
        /// Состояние перемещения объекта
        /// </summary>
        public bool IsSelectionFigureMove = false;

        //Вектор - расстояние от мышки до верхнего левого угла области фигуры
        private Point vector;

        /// <summary>
        /// Начинает следить за перемещением перемещением мыши
        /// </summary>
        /// <param name="mouse">Координаты мыши</param>
        public void StartMovingSelectionFigure(Point mouse)
        {
            Point start = SelectionFigure.Start;
            vector = MyMath.SubtractPoint(start, mouse);
            IsSelectionFigureMove = true;
        }

        /// <summary>
        /// Перестаёт следить за перемещением мыши
        /// </summary>
        /// <param name="mouse">Координаты мыши</param>
        public void StopMovingSelectionFigure(Point mouse)
        {
            SelectionFigure.Start = MyMath.SumPoints(mouse,vector);
            IsSelectionFigureMove = false;
        }

        /// <summary>
        /// Проверяет, находится ли мышь над областью выделенной фигуры
        /// </summary>
        /// <param name="mouse">Координаты мыши</param>
        /// <returns></returns>
        public bool IsMouseHoverSelectionFigure(Point mouse)
        {
            if (SelectionFigure.IsShown)
                return MyMath.IsMouseHoverSelectedFigure(mouse, SelectionFigure);
            else
                return false;
        }

        /// <summary>
        /// Задаёт конечные координаты выделенной фигуры и выделетиля
        /// </summary>
        /// <param name="mouse">Координаты мыши</param>
        /// <param name="f">Выделенная фигура</param>
        public void SetSelectionFigurePosition(Point mouse, Figure f)
        {
            SelectionFigure.Start = MyMath.SumPoints(mouse, vector);
            f.Start = MyMath.SumPoints(mouse, vector);
        }

        #endregion

        #region Scaling
        public bool IsSelectionFigureScale = false;
        public void StartScalingSelectionFigure()
        {
            IsSelectionFigureScale = true;
        }
        public void SetScalingFigureSize(Point m, Figure f)
        {
            int x = m.X - f.Start.X;
            int y = m.Y - f.Start.Y;
            SelectionFigure.Scale(x,y);
            f.Scale(x,y);
        }
        public void StopScalingSelectionFigure(Point mouse, Figure f)
        {
            SetScalingFigureSize(mouse, f);
            IsSelectionFigureScale = false;
        }

        public bool IsMouseHoverScalingFigure(Point mouse)
        {
            if (SelectionFigure.IsShown)
            {
                Figure f = SelectionFigure;
                int x = f.Start.X + f.W - Math.Min(5, f.W - 1);
                int y = f.Start.Y + f.H - Math.Min(5, f.H - 1);
                Point almostLR = new Point(x,y);
                return MyMath.IsMouseHoverSelectedFigure(mouse, almostLR, 5, 5);
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Возвращает индекс фигуры, которая первой находится под мышкой в момент клика.
        /// </summary>
        /// <param name="mouse">Координаты мыши</param>
        /// <returns>Индекс фигуры в списке</returns>
        public int SelectFigureIndex(Point mouse)
        {
            for(int i = Count-1; i >= 0; i--)
            {
                Figure f = Figures[i];
                if(MyMath.IsMouseHoverSelectedFigure(mouse, f.Start, f.W, f.H))
                {
                    Select(f);
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Отображает информацию о всех игурах
        /// </summary>
        public void GetInfo()
        {
            string calculations = $"Полная площадь: {TotalArea()}; Полный периметр: {TotalPerimetr()}";
            MessageBox.Show(calculations + "\r\n\r\n"+ToString(),"Информация о фигурах");
        }

        /// <summary>
        /// Возвращает строку с информацией о всех фигурах
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string seed = "";
            foreach (Figure f in Figures)
            {
                seed += f + "\r\n";
            }
            return seed;
        }

        #region Итератор
        public IEnumerable<Figure> GetEnumerable() => Figures;

        public IEnumerable<Figure> Reversed()
        {
            List<Figure> frs = GetEnumerable().ToList();
            frs.Reverse();
            return frs;
        }

        public IEnumerator<Figure> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
