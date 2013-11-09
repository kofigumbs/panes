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
        private Color ORANGE = Color.FromArgb(255, 255, 165, 0);
        private Color YELLOW = Color.FromArgb(255, 255, 255, 0);
        private Color GREEN = Color.FromArgb(255, 0, 128, 0);
        private Color BLUE = Color.FromArgb(255, 0, 0, 255);
        private Color PURPLE = Color.FromArgb(255, 128, 0, 128);

        private const int ROWSIZE = 3;
        private const int COLSIZE = 3;

        Color[] colorCircle;

        Rectangle[,] board = new Rectangle[ROWSIZE,COLSIZE];
        Rectangle[,] solution = new Rectangle[ROWSIZE, COLSIZE];

        private Rectangle source;
        private Color sourceColor;
        private bool sourcePicked = false;

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
            Random rand = new Random();
            for (int row = 0; row < ROWSIZE; row++)
            {
                for (int col = 0; col < COLSIZE; col++)
                {
                    var a = new Rectangle();
                    a.Fill = new SolidColorBrush(colorCircle[(int)rand.Next(0,5)]);
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
            solution = makeSolution(board, 3);
            Grid tarsolution = ((System.Windows.Controls.Grid)(this.FindName("Target")));
            UIElementCollection t = tarsolution.Children;
            for (int row = 0; row < ROWSIZE; row++)
            {
                for (int col = 0; col < COLSIZE; col++)
                {
                    Rectangle orig = solution[row,col];
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
            if (!sourcePicked) //first tap (select the source color0
            {
                sourcePicked = true;
                source = (Rectangle)e.OriginalSource;
                sourceColor = ((SolidColorBrush)source.Fill).Color;
                source.Fill = new RadialGradientBrush(sourceColor, Color.FromArgb((byte)(sourceColor.A - 50), sourceColor.R, sourceColor.G, sourceColor.B));
            }
            else// second tap (change the destination rectangle)
            {
                Rectangle dest = (Rectangle)e.OriginalSource;
                sourcePicked = false;
                source.Fill = new SolidColorBrush(sourceColor);
                Color destColor = ((SolidColorBrush)dest.Fill).Color;
                dest.Fill = new SolidColorBrush(ChangeColor(sourceColor, destColor));
            }
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

        private Rectangle[,] makeSolution(Rectangle[,] b,int moves)
        {
            Rectangle[,] soln = new Rectangle[3, 3];
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
            return soln;
        }

        private bool isSolved()
        {
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    if (board[row, col].Fill != solution[row, col].Fill)
                        return false;
            return true;
        }
    }
}