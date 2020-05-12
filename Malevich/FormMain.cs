using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Malevich
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// Размер новой области фигуры
        /// </summary>
        public static int BlockSize = 75;

        /// <summary>
        /// Холст для рисования
        /// </summary>
        private Picture Canvas;

        private Color MainColor = Color.Black;  // Цвет фигуры
        private Color SecondColor = Color.White;  // Заливка. Не используется

        public FormMain()
        {
            InitializeComponent();
            PrepareForm();
        }

        #region Подготовка формы

        private void PrepareForm()
        {
            Canvas = new Picture(pBoxCanvas);
            Picture savedPicture = new Picture(pBoxCanvas);
            UploadData("picture", ref savedPicture.Figures);
            Canvas.Merge(savedPicture);
            DesignButtons();
            lBoxLevels.DataSource = Canvas.ToList();
            
        }

        private void DesignButtons()
        {
            Figure point = new Dot(MainColor);
            ChangeImage(pBoxPoint, point);
            Figure line = new Line(MainColor);
            ChangeImage(pBoxLine, line);
            Figure triangle = new Triangle(MainColor, true);
            ChangeImage(pBoxTriangle, triangle);
            Figure hatch = new HatchTriangle(MainColor, true);
            ChangeImage(pBoxHatchTriangle, hatch);
            Figure equil = new EquilateralTriangle(MainColor, true);
            ChangeImage(pBoxEquilateralTriangle, equil);
            Figure right = new RightTriangle(MainColor, true);
            ChangeImage(pBoxRightTriangle, right);
            Figure tetra = new Tetrahedron(MainColor, true);
            ChangeImage(pBoxTetrahedron, tetra);
        }

        private void ChangeImage(PictureBox pBox, Figure f)
        {
            Bitmap bmp = new Bitmap(pBox.Width, pBox.Height);
            f.Paint(bmp);
            pBox.Image = bmp;
        }      

        #endregion

        #region refresh

        private void RefreshForm()
        {
            RefreshCanvas();                //выполняем перерисвоку
            RefreshListBoxLevels();         //обновляем источник данных
        }

        private void RefreshCanvas()
        {
            Canvas.DrawAll();                        //выполняем перерисвоку
        }

        private void RefreshListBoxLevels()
        {
            lBoxLevels.DataSource = Canvas.ToList(); //обновляем источник данных
        }

        #endregion

        #region Io
        //Получить объект, сериализированный из json в текстовом файле
        private void UploadData<T>(string filename, ref T array)
        {
            //если такого файла нет, выходим из функции
            if (!File.Exists($"{filename}.txt"))
                return;
            TextReader reader = new StreamReader($"{filename}.txt");
            try
            {
                //считываем весь текст из фала
                string json = reader.ReadToEnd();
                if (json == "")
                    return;
                //десериализируем json в объект T
                T result = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                //проверка на null
                if (!ReferenceEquals(result, null))
                    array = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                    "Ошибка получения данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //в любом случае выполнения закрываем соединение с файлом
                reader.Close();
            }
        }

        ///Сериализовать объект в json и записать в текстовый файл
        private void SaveData<T>(string filename, T info)
        {
            //если файл не существует, создаём его
            if (!File.Exists($"{filename}.txt"))
                File.Create($"{filename}.txt");
            //сериализируем объект типа T в json
            string json = JsonConvert.SerializeObject(info,Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            TextWriter writer = new StreamWriter($"{filename}.txt");
            
            try
            {
                //записываем весь текст в файл
                writer.Write(json);
                MessageBox.Show("Данные успешно сохранены",
                    "Результат сохранения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                    "Ошибка сохранения данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //при любом результате операции закрываем соединение с файлом
                writer.Close();
            }
        }
        #endregion

        private void AddFigure(PictureBox pBox, MouseButtons mButton)
        {
            
            Figure f = null;
            switch (pBox.Tag.ToString())
            {
                case "Point":
                    f = new Dot(MainColor);
                    break;

                case "Line":
                    f = new Line(MainColor, true);
                    break;

                case "Triangle":
                    f = new Triangle(MainColor, true);
                    break;

                case "HatchTriangle":
                    f = new HatchTriangle(MainColor, true);
                    break;

                case "EquilateralTriangle":
                    f = new EquilateralTriangle(MainColor, true);
                    break;

                case "RightTriangle":
                    f = new RightTriangle(MainColor, true);
                    break;

                case "Tetrahedron":
                    f = new Tetrahedron(MainColor, true);
                    break;

                default:
                    break;
            }
            if (!ReferenceEquals(f, null))            //по непонятнымпричинам экземпляр класса не создался
            {
                Canvas.Add(f);
                RefreshListBoxLevels();
                lBoxLevels.SelectedIndex = Canvas.Count - 1;
                if (mButton == MouseButtons.Right)
                {
                    f.IsDraw = false;
                    f.MyColor = SecondColor;
                }
            }
        }

        private void ChangeColor(PictureBox pBox, MouseButtons mButton)
        {
            if (mButton == MouseButtons.Left)
            {
                MainColor = pBox.BackColor;
                pBoxMainColor.BackColor = MainColor;
                if (lBoxLevels.SelectedIndex != -1)
                {
                    Canvas[lBoxLevels.SelectedIndex].MyColor = MainColor;
                    RefreshCanvas();
                }
            }
            else if (mButton == MouseButtons.Right)
            {
                SecondColor = pBox.BackColor;
                pBoxSecondColor.BackColor = SecondColor;
            }
        }

        private void SaveImage()
        {
            Bitmap picture = Canvas.DrawAllOnBitmap();
            try
            {
                picture.Save("picture.png", ImageFormat.Png);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                    "Ошибка сохранения данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Ваша картина успешно сохранена",
                "Результат сохранения",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #region Events

        private void pBoxColor_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox curr = (PictureBox)sender;
            ChangeColor(curr, e.Button);
        }

        private void pBoxNewFigure_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pBox = (PictureBox)sender;
            AddFigure(pBox, e.Button);
            RefreshCanvas();
        }

        private void lBoxLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lBoxLevels.SelectedIndex;
            if (index == -1)
                return;
            Canvas.Select(index);
            RefreshCanvas();
        }

        private void lBoxLevels_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            int index = lBoxLevels.IndexFromPoint(e.Location);
            lBoxLevels.SelectedIndex = index;
            cMenuStripFigure.Show(MyMath.SumPoints(e.Location, lBoxLevels.Location, pLevels.Location));
        }

        private void tStripMenuBlink_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int index = lBoxLevels.SelectedIndex;
            if (index != ListBox.NoMatches)
            {
                Canvas.Blink(index);
                RefreshForm();
                lBoxLevels.SelectedIndex = index;
            }
        }
        private void tStripMenuToUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int index = lBoxLevels.SelectedIndex;
            if (index != ListBox.NoMatches)
            {
                Canvas.ToUp(index);
                RefreshForm();
                lBoxLevels.SelectedIndex = Canvas.Count-1;
            }
        }
        private void tStripMenuToDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int index = lBoxLevels.SelectedIndex;
            if (index != ListBox.NoMatches)
            {
                Canvas.ToDown(index);
                RefreshForm();
                lBoxLevels.SelectedIndex = 0;
            }
        }
        private void tStripMenuUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int index = lBoxLevels.SelectedIndex;
            if (index != ListBox.NoMatches)
            {
                Canvas.Up(index);
                RefreshForm();
                lBoxLevels.SelectedIndex = index;
            }
        }
        private void tStripMenuDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int index = lBoxLevels.SelectedIndex;
            if (index != ListBox.NoMatches)
            {
                Canvas.Down(index);
                RefreshForm();
                lBoxLevels.SelectedIndex = index;
            }
        }
        private void tStripMenuRemove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int index = lBoxLevels.SelectedIndex;
            if (index != ListBox.NoMatches)
            {
                Canvas.Remove(index);
                RefreshForm();
            }
        }

        private void pBoxCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //мышь в правом нижнем углу выделенной области
            if (Canvas.IsMouseHoverScalingFigure(e.Location)|| Canvas.IsSelectionFigureScale)
            {
                pBoxCanvas.Cursor = Cursors.SizeNWSE;
            }
            else if (Canvas.IsSelectionFigureMove)   //В процессе перемещения
            {
                pBoxCanvas.Cursor = Cursors.Hand;
                Figure f = Canvas[lBoxLevels.SelectedIndex];
                Canvas.SetSelectionFigurePosition(e.Location, f);
                RefreshCanvas();
            }
            else if (Canvas.IsMouseHoverSelectionFigure(e.Location)) //Над выделенной фигурой
            {               
                pBoxCanvas.Cursor = Cursors.SizeAll;              
            }
            else
            {
                pBoxCanvas.Cursor = Cursors.Default;
            }
            // Записываем положение мыши
            lblMouseLocationX.Text = $"X: {e.Location.X}";
            lblMouseLocationY.Text = $"Y: {e.Location.Y}";
        }

        private void pBoxCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (Canvas.IsMouseHoverScalingFigure(e.Location) && e.Button == MouseButtons.Left)
            {
                Canvas.StartScalingSelectionFigure();
            }
            else if (Canvas.IsMouseHoverSelectionFigure(e.Location) && e.Button == MouseButtons.Left)
            {
                Canvas.StartMovingSelectionFigure(e.Location);
            }
            else
            {
                int index = Canvas.SelectFigureIndex(e.Location);
                if (index == -1)
                {
                    Canvas.Unselect();
                }
                lBoxLevels.SelectedIndex = index;
                if(e.Button == MouseButtons.Right)
                {               
                    cMenuStripFigure.Show(MyMath.SumPoints(e.Location, pBoxCanvas.Location));
                }
            }
        }

        private void pBoxCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (Canvas.IsSelectionFigureScale)
            {
                Figure f = Canvas[lBoxLevels.SelectedIndex];
                Canvas.StopScalingSelectionFigure(e.Location, f);
            }
            else if (Canvas.IsMouseHoverSelectionFigure(e.Location))
            {
                Canvas.StopMovingSelectionFigure(e.Location);
            }
            Canvas.Select(lBoxLevels.SelectedIndex);
            RefreshCanvas();
        }

        private void bScale_Click(object sender, EventArgs e)
        {
            double k = (double)numUpDScale.Value / 10.0;
            Canvas.ScaleAll(k);
            RefreshForm();
        }

        private void bMove_Click(object sender, EventArgs e)
        {
            int v = (int)numUpDMove.Value;
            Canvas.MoveAll(v);
            RefreshForm();
        }

        private void bInfo_Click(object sender, EventArgs e)
        {
            Canvas.GetInfo();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            SaveData("picture", Canvas.ToList());
            SaveImage();
        }

        private void lBoxLevels_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //Удалить фигуру при нажатии del
            if (e.KeyCode == Keys.Delete && lBoxLevels.SelectedIndex != -1)
            {
                Canvas.Remove(lBoxLevels.SelectedIndex);
                RefreshForm();
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if(WindowState != FormWindowState.Minimized)
                RefreshForm();
        }

        #endregion
    }
}
