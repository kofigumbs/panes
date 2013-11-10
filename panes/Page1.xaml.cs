using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace panes
{
    public partial class Page1 : PhoneApplicationPage
    {
        private Color WHITE = Color.FromArgb(255, 255, 255, 255);
        private Color RED = Color.FromArgb(255, 255, 0, 0);
        private Color ORANGE = Color.FromArgb(255, 255, 125, 0);
        private Color YELLOW = Color.FromArgb(255, 255, 255, 0);
        private Color GREEN = Color.FromArgb(255, 0, 128, 0);
        private Color BLUE = Color.FromArgb(255, 0, 0, 255);
        private Color PURPLE = Color.FromArgb(255, 128, 0, 128);

        private const int ROWSIZE = 3;
        private const int COLSIZE = 3;
        private Random rand = new Random();


        Color[] colorCircle;

        Rectangle[,] startBoard = new Rectangle[ROWSIZE, COLSIZE];
        Rectangle[,] board = new Rectangle[ROWSIZE, COLSIZE];
        Rectangle[,] solution = new Rectangle[ROWSIZE, COLSIZE];

        private Rectangle source;
        private Color sourceColor;
        private bool sourcePicked = false;
        private Point sourcePoint;

        internal class Tuple
        {
            public Point p;
            public Color c;

            public Tuple(Point p1, Color c1)
            {
                p = p1;
                c = c1;
            }
        }

        Stack<Tuple> lastMoves = new Stack<Tuple>();

        public Page1()
        {
            colorCircle = new Color[6]{
                RED,
                ORANGE,
                YELLOW,
                GREEN,
                BLUE,
                PURPLE
            };
            InitializeComponent();
            Grid playingfield = ((System.Windows.Controls.Grid)(this.FindName("PlayingField")));
            UIElementCollection rd = playingfield.Children;
            Rectangle r = (Rectangle)rd[0];
            for (int row = 0; row < ROWSIZE; row++)
            {
                for (int col = 0; col < COLSIZE; col++)
                {
                    var a = new Rectangle();
                    a.Fill = new SolidColorBrush(colorCircle[2*rand.Next(0, 3)]);
                    a.Height = r.Height;
                    a.Width = r.Width;
                    a.Margin = r.Margin;
                    a.RadiusX = r.RadiusX;
                    a.RadiusY = r.RadiusY;
                    Grid.SetRow(a, row);
                    Grid.SetColumn(a, col);
                    board[row, col] = (Rectangle)a;
                    playingfield.Children.Add(a);
                }
            }
            solution = makeSolution(board, 10);
            Grid tarsolution = ((System.Windows.Controls.Grid)(this.FindName("Target")));
            UIElementCollection t = tarsolution.Children;
            for (int row = 0; row < ROWSIZE; row++)
            {
                for (int col = 0; col < COLSIZE; col++)
                {
                    Rectangle orig = solution[row, col];
                    var a = new Rectangle();
                    a.Fill = orig.Fill;
                    a.Height = orig.Height;
                    a.Width = orig.Width;
                    a.RadiusX = orig.RadiusX;
                    a.RadiusY = orig.RadiusY;
                    Grid.SetRow(a, row);
                    Grid.SetColumn(a, col);
                    tarsolution.Children.Add(a);
                }
            }
        }

        private void GestureListener_Tap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            if (!sourcePicked) //first tap (select the source color)
            {
                sourcePicked = true;
                source = (Rectangle)e.OriginalSource;
                sourceColor = ((SolidColorBrush)source.Fill).Color;
                sourcePoint = GetCoordsOfRect(source);
                source.Fill = new RadialGradientBrush(sourceColor, Color.FromArgb((byte)(sourceColor.A - 75), sourceColor.R, sourceColor.G, sourceColor.B));
            }
            else// second tap (change the destination rectangle)
            {
                Rectangle dest = (Rectangle)e.OriginalSource;
                sourcePicked = false;
                source.Fill = new SolidColorBrush(sourceColor);
                Color destColor = ((SolidColorBrush)dest.Fill).Color;
                Point destPoint = GetCoordsOfRect(dest);

                bool isWithinOne = (sourcePoint.X == destPoint.X && (Math.Abs(sourcePoint.Y - destPoint.Y) == 1))
                    || (sourcePoint.Y == destPoint.Y && (Math.Abs(sourcePoint.X - destPoint.X) == 1));

                if (isWithinOne)
                {
                    PushLastMove(dest,destColor);
                    dest.Fill = new SolidColorBrush(ChangeColor(sourceColor, destColor));
                    if (isSolved())
                        NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
                }
            }
        }

        private void PushLastMove(Rectangle r,Color c)
        {
            Point temp = new Point();
            for (int i = 0; i < ROWSIZE; i++)
                for (int j = 0; j < COLSIZE; j++)
                    if (r == board[i, j])
                    {
                        temp.X = i;
                        temp.Y = j;
                    }
            lastMoves.Push(new Tuple(temp,c));
        }

        private Color ChangeColor(Color from, Color to)
        {
            int fromIndex = 6, toIndex = 6;
            for (int i = 0; i < colorCircle.Length; i++)
            {
                if (colorCircle[i] == from)
                    fromIndex = i;
                else if (colorCircle[i] == to)
                    toIndex = i;
            }

            // WHITE case or within one 
            if (toIndex == 6 || Math.Abs(fromIndex - toIndex) <= 1 || (6 - Math.Abs(fromIndex - toIndex)) <= 1)
                return from;

            // Opposite color case
            else if (Math.Abs(fromIndex - toIndex) == 3)
                return WHITE;

            //gosh darn edge case
            else if ((from == RED && to == BLUE) || (from == BLUE && to == RED))
                return PURPLE;

            //second darn edge case
            else if ((from == PURPLE && to == ORANGE) || (from == ORANGE && to == PURPLE))
                return RED;
            // Adjacent color case
            else
                return colorCircle[(fromIndex + toIndex) / 2];
        }


        private Point GetCoordsOfRect(Rectangle r)
        {
            for (int i = 0; i < ROWSIZE; i++)
                for (int j = 0; j < COLSIZE; j++)
                    if (r == board[i, j])
                        return new Point(i, j);
            return new Point(-1, -1); ;
        }

        private Rectangle[,] makeSolution(Rectangle[,] b, int moves)
        {
            Rectangle[,] soln = new Rectangle[ROWSIZE, COLSIZE];
            Grid playingfield = ((System.Windows.Controls.Grid)(this.FindName("PlayingField")));
            UIElementCollection rd = playingfield.Children;
            Rectangle r = (Rectangle)rd[0];
            for (int row = 0; row < ROWSIZE; row++)
            {
                for (int col = 0; col < COLSIZE; col++)
                {
                    var a = new Rectangle();
                    a.Fill = new SolidColorBrush(colorCircle[2 * rand.Next(0, 3) + 1]);
                    a.Height = r.Height;
                    a.Width = r.Width;
                    a.Margin = r.Margin;
                    a.RadiusX = r.RadiusX;
                    a.RadiusY = r.RadiusY;
                    Grid.SetRow(a, row);
                    Grid.SetColumn(a, col);
                    soln[row, col] = (Rectangle)a;
                }
            }
            return soln;
            /*
            Rectangle[,] soln = new Rectangle[ROWSIZE, COLSIZE];
            for (int r = 0; r < ROWSIZE; r++)
            {
                for (int c = 0; c < COLSIZE; c++)
                {
                    soln[r, c] = new Rectangle();
                    soln[r, c].Fill = b[r, c].Fill;
                    soln[r, c].Height = b[r, c].Height;
                    soln[r, c].Width = b[r, c].Width;
                    soln[r, c].RadiusX = b[r, c].RadiusX;
                    soln[r, c].RadiusY = b[r, c].RadiusY;
                }
            }
            Random rand = new Random();
            for (; moves > 0; moves--)
            {
                int r = rand.Next(0, 2), c = rand.Next(0, 2);
                int tr = r, tc = c;
                if (rand.Next(0, 1) == 0)
                {
                    if (tc + 1 < 3)
                        tc += 1;
                    else
                        tc -= 1;
                }
                else
                {
                    if (tr + 1 < 3)
                        tr += 1;
                    else
                        tr -= 1;
                }
                soln[tr, tc].Fill = new SolidColorBrush(ChangeColor(((SolidColorBrush)soln[r, c].Fill).Color, ((SolidColorBrush)soln[tr, tc].Fill).Color));
            }
            return soln;*/

        }

        private bool isSolved()
        {
            for (int row = 0; row < ROWSIZE; row++)
                for (int col = 0; col < COLSIZE; col++)
                {
                    Color bcolor = ((SolidColorBrush)board[row, col].Fill).Color;
                    Color scolor = ((SolidColorBrush)solution[row, col].Fill).Color;
                    if (bcolor != scolor)
                        return false;
                }
            return true;
        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
            while (lastMoves.Count != 0)
            {
                Tuple t = lastMoves.Pop();
                board[(int)t.p.X, (int)t.p.Y].Fill = new SolidColorBrush(t.c);
            }

        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (lastMoves.Count != 0)
            {
                Tuple t = lastMoves.Pop();
                board[(int)t.p.X, (int)t.p.Y].Fill = new SolidColorBrush(t.c);
            }
        }
    }
}