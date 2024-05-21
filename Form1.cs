using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {   //hello
        //private double Level = Convert.ToDouble(textBox1.Text);
        //Высота и ширина для отрисовки
        private int _width;
        private int _height;
        // Bitmap для фрактала
        private Bitmap _fractal;
        // используем для отрисовки на PictureBox
        private Graphics _graph;

        SldWorks SwApp;
        IModelDoc2 swModel;

        Entity[] userSelectedSurface;


        bool[] extr = new bool[4] { false, false, false, false };
        static int counter = 0;
        static int cntr = 0;

        public Form1()
        {
            InitializeComponent();
            //инициализируем ширину и высоту
            _width = pictureBox1.Width;
            _height = pictureBox1.Height;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            double Level = Convert.ToDouble(textBox1.Text);
            //создаем Bitmap для треугольника
            _fractal = new Bitmap(_width, _height);
            // cоздаем новый объект Graphics из указанного Bitmap
            _graph = Graphics.FromImage(_fractal);
            //вершины треугольника
            PointF topPoint = new PointF(_width / 2f, 0);
            PointF leftPoint = new PointF(0, _height);
            PointF rightPoint = new PointF(_width, _height);
            //вызываем функцию отрисовки
            DrawTriangle(Level, topPoint, leftPoint, rightPoint);
            //отображаем получившийся фрактал
            pictureBox1.BackgroundImage = _fractal;

            if (textBox2.Text == "1") extr[0] = true;       //  Top
            if (textBox4.Text == "1") extr[1] = true;       //  Left
            if (textBox5.Text == "1") extr[2] = true;       //  Right

        }

        private void DrawTriangle(double level, PointF top, PointF left, PointF right)
        {
            //проверяем, закончили ли мы построение
            if (level == 0)
            {
                PointF[] points = new PointF[3]
                {
                    top, right, left
                };
                //рисуем фиолетовый треугольник
                _graph.FillPolygon(Brushes.BlueViolet, points);
            }
            else
            {
                //вычисляем среднюю точку
                var leftMid = MidPoint(top, left); //левая сторона
                var rightMid = MidPoint(top, right); //правая сторона
                var topMid = MidPoint(left, right); // основание
                //рекурсивно вызываем функцию для каждого и 3 треугольников
                DrawTriangle(level - 1, top, leftMid, rightMid);
                DrawTriangle(level - 1, leftMid, left, topMid);
                DrawTriangle(level - 1, rightMid, topMid, right);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private PointF MidPoint(PointF p1, PointF p2)
        {
            return new PointF((p1.X + p2.X) / 2f, (p1.Y + p2.Y) / 2f);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();


                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    string selectedFilePath = openFileDialog.FileName;


                    MessageBox.Show("Выбран файл: " + selectedFilePath);


                    Process.Start(selectedFilePath);
                }
                else
                {

                    Process.Start("explorer.exe");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)              //  Строим первоначальный прямоугольник 
        {
            double sideLength, height;
            sideLength = Convert.ToDouble(textBox6.Text);
            height = Convert.ToDouble(textBox7.Text);


            SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application");
            SwApp.NewPart();

            swModel = SwApp.IActiveDoc2;

            swModel.Extension.SelectByID2("Сверху", "PLANE", 0, 0, 0, false, 0, null, 0);

            swModel.SketchManager.InsertSketch(true);

            double triangleHeight = (Math.Sqrt(3) / 2) * sideLength;

            swModel.SketchManager.CreateLine(0, 0, 0, sideLength, 0, 0);
            swModel.SketchManager.CreateLine(sideLength, 0, 0, sideLength / 2, triangleHeight, 0);
            swModel.SketchManager.CreateLine(sideLength / 2, triangleHeight, 0, 0, 0, 0);


           
            swModel.SketchManager.InsertSketch(true);

           
            FeatureManager swFeatureMgr = swModel.FeatureManager;

           
            swModel.FeatureManager.FeatureExtrusion2(true, false, true, 0, 0, height, 0.01, false, false, false, false, 1.74532925199433E-02, 1.74532925199433E-02,
                false, false, false, false, true, true, true, 0, 0, true);
        }






        //private void button4_Click(object sender, EventArgs e)
        //{
        //    SelectionMgr mainComponentSurfaceSelections = (SelectionMgr)swModel.SelectionManager;
        //    userSelectedSurface = new Entity[mainComponentSurfaceSelections.GetSelectedObjectCount()];

        //    for (int i = 0; i < mainComponentSurfaceSelections.GetSelectedObjectCount(); ++i)
        //        userSelectedSurface[i] = (Entity)mainComponentSurfaceSelections.GetSelectedObject6(i + 1, -1);
        //}






        bool first = true;
        int counterC = 0;
        int index = 0;
        private void drawNapkinInSolid(int level, PointF top, PointF left, PointF right, bool extru)
        {   
            double A = Convert.ToDouble(textBox6.Text);
            if (level != 0)
            {
                var leftMid = MidPoint(top, left); //левая сторона
                var rightMid = MidPoint(top, right); //правая сторона
                var topMid = MidPoint(left, right); // основание
                //рекурсивно вызываем функцию для каждого и 3 треугольников
                drawNapkinInSolid(level - 1, top, leftMid, rightMid, true);
                drawNapkinInSolid(level - 1, leftMid, left, topMid, true);
                drawNapkinInSolid(level - 1, rightMid, topMid, right, true);
            }
            else
            {
                if (extr[index])
                {
                    //userSelectedSurface[userSelectedSurface.Length - 1].Select(true);
                    swModel.SelectByID("Спереди", "PLANE", 0, 0, 0);

                    swModel.InsertSketch2(true);

                    swModel.SketchManager.CreateLine(right.X, right.Y, 0, top.X, top.Y, 0);
                    swModel.SketchManager.CreateLine(left.X, left.Y, 0, right.X, right.Y, 0);
                    swModel.SketchManager.CreateLine(left.X, left.Y, 0, top.X, top.Y, 0);
                    swModel.SketchManager.CreateLine(top.X, top.Y, 0, top.X, top.Y, 0);
                
                    
                    Sketch sketch = swModel.GetActiveSketch();
                    swModel.InsertSketch2(true);

                    FeatureManager featureManager = swModel.FeatureManager;

                    //swModel.ClearSelection();

                    featureManager.FeatureExtrusion(true, false, true, 0, 0, A, 0, false, false, false, false, 0, 0, false, false, false, false, false, false, false);
                }

                ++counter;

                if (counter == Math.Pow(3, (int.Parse(textBox1.Text) - 1)))
                {
                    ++index;
                    counter = 0;
                }
                    
            }
        }








        private void button5_Click(object sender, EventArgs e)
        {
            SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application");
            SwApp.NewPart();
            swModel = SwApp.IActiveDoc2;

            //swModel.ClearSelection();

            PointF tp = new PointF(94.0f/ 2.0f, 0);
            PointF lp = new PointF(0, 94.0f);
            PointF rp = new PointF(94.0f, 94.0f);

            drawNapkinInSolid(int.Parse(textBox1.Text), tp, lp, rp, false);
        }


        //private void textBox3_TextChanged(object sender, EventArgs e)
        //{

        //}
    }
}