using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace self_repair2
{
    public partial class Form1 : Form
    {

        private Image image;
        private int[] unit;
        int height = 0;
        double Pr;
        double Pra;
        double Prn = 1.0;


        public Form1()
        {
            //InitializeComponent();

            unit = new int[500];
            for (int i = 0; i < 500; i++)
            {
                unit[i] = 1;
                if (i == 255)
                {
                    unit[i] = 0;
                }
            }

            Pr = 1.0;
            Pra = 0.5;


            //------------
            // フォームサイズをスクリーンサイズに合わせる処理
            //------------

            // 画面サイズを取得して、それをフォームのサイズとして設定する。
            this.Width = 1050;  // フォームの幅を指定
            this.Height = Screen.GetWorkingArea(this).Height / 2;  // フォームの高さを指定
            // フォームの配置位置を指定する。
            int LeftPosition = this.Left;  // フォームの左端位置指定
            int TopPosition = this.Top;    // フォームの上端位置指定
            this.Location = new Point(LeftPosition, TopPosition);


            /*`
             * 描画用
             */
            //ダブルバッファリング
            SetStyle(
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true
            );

            image = new Bitmap(1000, 800);
            Graphics g = Graphics.FromImage(image);
            Brush brush = new SolidBrush(Color.White);
            g.FillRectangle(brush, 0, 0, image.Width, image.Height);

            /*
             * タイマー
             */
            
            Timer timer = new Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(Test_Tick);
            timer.Start();
             

        }

        private void Test_Tick(object sender, EventArgs e)
        {
            int[] next_unit = new int[500];
            int suc_count = 0;

            //グラフィック用
            Graphics g = Graphics.FromImage(image);
            Brush brush;

            for (int unit_num = 0; unit_num < 500; unit_num++)
            {

                int x_left = unit_num - 1;
                int x_right = unit_num + 1;
                if (x_left < 0) x_left = 500-1;
                if (x_right >= 500) x_right = 0;

                int result1 = -1;
                int result2 = -1;

                int seed = Environment.TickCount;

                Random randam = new System.Random(seed++);

                double rnd_r1 = randam.NextDouble();
                double rnd_ra1 = randam.NextDouble();
                //左セルの修復
                if (Pr > rnd_r1)
                {
                    if (unit[x_left] == 0)
                    {
                        result1 = 0;
                    }
                    else if (unit[x_left] == 1)
                    {
                        if (Pra > rnd_ra1)
                        {
                            result1 = 0;
                        }
                        else
                        {
                            result1 = 1;
                        }
                    }
                }

                double rnd_r2 = randam.NextDouble();
                double rnd_ra2 = randam.NextDouble();
                //右セルによる修復
                if (Pr > rnd_r2)
                {
                    if (unit[x_right] == 0)
                    {
                        result2 = 0;
                    }
                    else if (unit[x_right] == 1)
                    {
                        if (Pra > rnd_ra2)
                        {
                            result2 = 0;
                        }
                        else
                        {
                            result2 = 1;
                        }
                    }
                }

                if (result1 > -1 && result2 > -1)
                {
                    if (result1 + result2 > 0)
                    {
                        next_unit[unit_num] = 1;
                    }
                    else if (result1 + result2 == 0)
                    {
                        next_unit[unit_num] = 0;
                        //Console.WriteLine(unit[x_left] + "  " + unit[x_right] + " " + next_unit[unit_num] + result1 + result2);
                        suc_count++;
                    }


                }
                else if (result1 < 0 && result2 > -1)
                {
                    if (result2 > 0)
                    {
                        next_unit[unit_num] = 1;
                    }
                    else if (result2 == 0)
                    {
                        next_unit[unit_num] = 0;
                    }
                }
                else if (result1 > -1 && result2 < 0)
                {
                    if (result1 > 0)
                    {
                        next_unit[unit_num] = 1;
                    }
                    else if (result1 == 0)
                    {
                        next_unit[unit_num] = 0;
                    }
                }
                else
                {
                    next_unit[unit_num] = unit[unit_num];
                }
                if (rnd_ra1 < Pra && rnd_ra2 < Pra)
                {
                    Console.Write("! ");
                }
                //描画(表示は後)
                if (unit[unit_num] == 1) brush = new SolidBrush(Color.Red);
                else brush = new SolidBrush(Color.Blue);
                g.FillRectangle(brush, unit_num, height, 1, 1);
            }

            height++;
            Invalidate();
            Console.WriteLine();
            for (int i = 0; i < 500; i++)
            {
                
                unit[i] = next_unit[i];
            }


        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(image, 0, 0);
        }

    }
}
