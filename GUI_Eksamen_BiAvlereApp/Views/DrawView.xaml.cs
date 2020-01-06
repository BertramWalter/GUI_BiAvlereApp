using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GUI_Eksamen_BiAvlereApp.Views
{
    /// <summary>
    /// Interaction logic for DrawView.xaml
    /// </summary>
    public partial class DrawView : Window
    {
        public enum DrawingTool { Line, Ellipsis, Rectangle };

        Color drawColor;
        Point startPos;
        Point currPos;
        Shape currShape;
        bool isDrawing = false;
        DrawingTool tool;
        DispatcherTimer timer;

        public DrawView()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(DrawView_KeyDown);
            MouseDown += new MouseButtonEventHandler(DrawView_MouseDown);
            MouseUp += new MouseButtonEventHandler(DrawView_MouseUp);
            MouseMove += new MouseEventHandler(DrawView_MouseMove);
            //canvas.MouseLeftButtonDown += new MouseButtonEventHandler(MainWindow_MouseDown);
            //canvas.MouseLeftButtonUp += new MouseButtonEventHandler(MainWindow_MouseUp);
            SetColor(Colors.Yellow);

            //Sætter default index på ComboBox!
            cmbBoxFigures.SelectedIndex = 0;
        }

        void DrawView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPos = e.GetPosition(canvas);
            if (cmbBoxFigures.SelectedValue.ToString() != "Line")
            {
                if (cmbBoxFigures.SelectedValue.ToString() != "System.Windows.Shapes.Rectangle")
                {
                    tool = DrawingTool.Ellipsis;
                    currShape = new Ellipse();
                }
                else
                {
                    tool = DrawingTool.Rectangle;
                    currShape = new Rectangle();
                }
                Canvas.SetLeft(currShape, startPos.X);
                Canvas.SetTop(currShape, currPos.Y);
                currShape.Width = 2;
                currShape.Height = 2;
                currShape.Fill = new SolidColorBrush(drawColor);
            }
            else
            {
                tool = DrawingTool.Line;
                Line l = new Line(); ;
                l.Stroke = new SolidColorBrush(drawColor);
                l.StrokeThickness = 2.0;
                l.X1 = startPos.X;
                l.Y1 = currPos.Y;
                l.X2 = currPos.X + 1;
                l.Y2 = currPos.Y + 1;
                currShape = l;
            }

            // Capture the mouse and prepare for future events.
            CaptureMouse();
            canvas.Children.Add(currShape);
            isDrawing = true;
        }

        void DrawView_MouseMove(object sender, MouseEventArgs e)
        {
            currPos = e.GetPosition(canvas);
            TbMouseX.Text = currPos.X.ToString("F0");
            TbMouseY.Text = currPos.Y.ToString("F0");
            if (isDrawing)
                if (tool != DrawingTool.Line)
                {
                    double width = currPos.X - startPos.X;
                    double height = currPos.Y - startPos.Y;
                    currShape.Width = (width > 1 ? width : 2);
                    currShape.Height = (height > 1 ? height : 2);
                }
                else
                {
                    Line l = currShape as Line;
                    l.X2 = currPos.X;
                    l.Y2 = currPos.Y;
                }
        }

        void DrawView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            ReleaseMouseCapture();
        }

        private void DrawView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.B:
                    SetColor(Colors.Black);
                    break;
                case Key.Y:
                    SetColor(Colors.Yellow);
                    break;
                case Key.R:
                    SetColor(Colors.Red);
                    break;
                case Key.G:
                    SetColor(Colors.Green);
                    break;
            }
        }

        private void SetColor(Color color)
        {
            drawColor = color;
            ChosenColor.Fill = new SolidColorBrush(drawColor);
        }
    }
}
