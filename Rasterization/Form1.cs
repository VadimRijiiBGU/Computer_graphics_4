using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Rasterization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static int scale = 1;
        int x1, x2, y1, y2, r;
        int padding = 30;
        Tuple<int, int> focusPoint = new Tuple<int, int>(0,0);
        private static HashSet<Tuple<int, int>> points = new HashSet<Tuple<int, int>>();

        private void changeTime(long time)
        {
            label4.Text = $"Time: {time} ticks";
        }

        private void Swap(ref int x1, ref int x2)
        {
            int t = x2;
            x2 = x1;
            x1 = t;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();

            x1 = int.Parse(textBox1.Text);
            x2 = int.Parse(textBox3.Text);
            y1 = int.Parse(textBox2.Text);
            y2 = int.Parse(textBox4.Text);

            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            if (x1 == x2)
            {
                for (int i = startY; i <= endY; i++)
                {
                    points.Add(new Tuple<int, int>(x1, i));
                }
            }
            else
            {
                int startX = Math.Min(x1, x2);
                int endX = Math.Max(x1, x2);

                double k = (y1 - y2) * 1.0 / (x1 - x2);
                double b = y1 - k * x1;
                if (Math.Abs(k) >= 1)
                {
                    for (int i = startY; i <= endY; i++)
                    {
                        points.Add(new Tuple<int, int>((int)Math.Round((i - b) / k), i));
                    }
                }
                else
                {
                    for (int i = startX; i <= endX; i++)
                    {
                        points.Add(new Tuple<int, int>(i, (int)Math.Round(k * i + b)));
                    }
                }
            }

            clock.Stop();
            changeTime(clock.ElapsedTicks);
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            int startX = -1000 / (scale * 2) + focusPoint.Item1;
            int startY = -600 / (scale * 2) + focusPoint.Item2;
            int endX = startX + 1000 / scale;
            int endY = startY + 600 / scale;

            for (int i = startX; i <= endX; i += (endX - startX) / 40)
            {
                int newX = (i - startX) * scale + padding;
                e.Graphics.DrawString(i.ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), newX - 5, 0);
                e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(newX, padding), new Point(newX, 600 + padding));
            }

            for (int i = startY; i <= endY; i += (endY - startY) / 20)
            {
                int newY = (i - startY) * scale + padding;
                e.Graphics.DrawString(i.ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), 0, newY - 5);
                e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(padding, newY), new Point(1000 + padding, newY));
            }

            if (startX <= 0 && endX >= 0)
            {
                int newX = -startX * scale + padding;
                e.Graphics.DrawLine(new Pen(Brushes.Blue), new Point(newX, padding), new Point(newX, 600 + padding));
            }

            if (startY <= 0 && endY >= 0)
            {
                int newY = -startY * scale + padding;
                e.Graphics.DrawLine(new Pen(Brushes.Blue), new Point(padding, newY), new Point(1000 + padding, newY));
            }

            drawAllPoints(e.Graphics);
        }

        private void drawAllPoints(Graphics g)
        {
            int frameLeftX = focusPoint.Item1 - 500 / scale;
            int frameRightX = focusPoint.Item1 + 500 / scale;
            int frameLeftY = focusPoint.Item2 - 300 / scale;
            int frameRightY = focusPoint.Item2 + 300 / scale;

            foreach (Tuple<int, int> tuple in points)
            {
                if (tuple.Item1 >= frameLeftX && tuple.Item1 <= frameRightX && tuple.Item2 >= frameLeftY && tuple.Item2 <= frameRightY)
                {
                    g.FillRectangle(Brushes.Green, (tuple.Item1 - frameLeftX) * scale + padding, (tuple.Item2 - frameLeftY) * scale + padding, scale, scale);
                }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int realX = (e.X - 530) / scale;
            int realY = (e.Y - 400) / scale;

            if (e.Button == MouseButtons.Right)
            {
                if (scale != 1)
                {
                    scale /= 5;
                    focusPoint = new Tuple<int, int>(focusPoint.Item1 + realX, focusPoint.Item2 + realY);
                }
            }
            else
            {
                if (scale < 25)
                {
                    scale *= 5;
                    focusPoint = new Tuple<int, int>(focusPoint.Item1 + realX, focusPoint.Item2 + realY);
                }
            }

            if (scale == 1) focusPoint = new Tuple<int, int>(0, 0);

            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            points.Clear();
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            x1 = int.Parse(textBox1.Text);
            x2 = int.Parse(textBox3.Text);
            y1 = int.Parse(textBox2.Text);
            y2 = int.Parse(textBox4.Text);

            int L = Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
            double dX = (x2 - x1) * 1.0 / L;
            double dY = (y2 - y1) * 1.0 / L;
            points.Add(new Tuple<int, int>(x1, y1));
            double prevX = x1;
            double prevY = y1;
            int i = 1;
            while (i < L)
            {
                prevX += dX;
                prevY += dY;
                points.Add(new Tuple<int, int>((int)Math.Round(prevX), (int)Math.Round(prevY)));
                i++;
            }

            points.Add(new Tuple<int, int>(x2, y2));
            stopwatch.Stop();
            changeTime(stopwatch.ElapsedTicks);
            pictureBox1.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            x1 = int.Parse(textBox1.Text);
            x2 = int.Parse(textBox3.Text);
            y1 = int.Parse(textBox2.Text);
            y2 = int.Parse(textBox4.Text);
            var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            if (steep)
            {
                Swap(ref x1, ref y1);
                Swap(ref x2, ref y2);
            }

            if (x1 > x2)
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int error = dx / 2;
            int ystep = (y1 < y2) ? 1 : -1;
            int y = y1;
            for (int x = x1; x <= x2; x++)
            {
                points.Add(new Tuple<int, int>(steep ? y : x, steep ? x : y));
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            stopwatch.Stop();
            changeTime(stopwatch.ElapsedTicks);
            pictureBox1.Refresh();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs ea)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            x1 = int.Parse(textBox1.Text);
            y1 = int.Parse(textBox2.Text);
            r = int.Parse(textBox5.Text);
            int x = 0;
            int y = r;
            int e = 3 - 2 * r;
            points.Add(new Tuple<int, int>(x + x1, y + y1));
            points.Add(new Tuple<int, int>(x + x1, -y + y1));
            points.Add(new Tuple<int, int>(-x + x1, y + y1));
            points.Add(new Tuple<int, int>(-x + x1, -y + y1));

            points.Add(new Tuple<int, int>(y + x1, x + y1));
            points.Add(new Tuple<int, int>(-y + x1, x + y1));
            points.Add(new Tuple<int, int>(y + x1, -x + y1));
            points.Add(new Tuple<int, int>(-y + x1, -x + y1));
            while (x < y)
            {
                if (e >= 0)
                {
                    e = e + 4 * (x - y) + 10;
                    x = x + 1;
                    y = y - 1;
                }
                else
                {
                    e = e + 4 * x + 6;
                    x = x + 1;
                }

                points.Add(new Tuple<int, int>(x + x1, y + y1));
                points.Add(new Tuple<int, int>(x + x1, -y + y1));
                points.Add(new Tuple<int, int>(-x + x1, y + y1));
                points.Add(new Tuple<int, int>(-x + x1, -y + y1));

                points.Add(new Tuple<int, int>(y + x1, x + y1));
                points.Add(new Tuple<int, int>(-y + x1, x + y1));
                points.Add(new Tuple<int, int>(y + x1, -x + y1));
                points.Add(new Tuple<int, int>(-y + x1, -x + y1));

                
            }
            stopwatch.Stop();
            changeTime(stopwatch.ElapsedTicks);
            pictureBox1.Refresh();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tbx_Click(object sender, EventArgs e)
        {

        }
    }
}
