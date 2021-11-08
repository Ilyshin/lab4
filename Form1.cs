using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
namespace lab4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            double delta = 0.001;
            ZedGraphControl zg1 = new ZedGraphControl();
            zg1.MasterPane = new MasterPane();
            zg1.MasterPane.Fill = new Fill(Color.White, Color.FromArgb(200, 230, 255), 45.0f);
            GraphPane graphPane1 = new GraphPane();
            zg1.Dock = DockStyle.Fill;
            GraphPane graphPane2 = new GraphPane();
           zg1.MasterPane.Add(graphPane1);
           zg1.MasterPane.Add(graphPane2);
            PointPairList list = FindFi(delta);
            LineItem myCurve = graphPane1.AddCurve("Ф(х)", list, Color.Red, SymbolType.None);
            graphPane1.XAxis.Scale.Max = 1;
            graphPane2.XAxis.Scale.Max = 1.05;
            graphPane2.XAxis.Title.Text = "x";
            graphPane2.YAxis.Title.Text = "|Фₐₙ - Ф|";
            graphPane1.XAxis.Title.Text = "x";
            graphPane1.YAxis.Title.Text = "Ф";
            myCurve.Line.Fill = new Fill(Color.AliceBlue, Color.White, Color.AliceBlue);

            zg1.AxisChange();
            zg1.Invalidate();

            PointPairList list2 = FindAnSolution(list, delta);
      
            LineItem myCurve2 = graphPane2.AddCurve("comparison of solutions", list2, Color.Blue, SymbolType.None);  
            TextObj text = new TextObj("h = " + delta.ToString(), 0.45, 0.9*list2[(int)(1/delta*0.5)].Y);
            TextObj text1 = new TextObj("h = " + delta.ToString(), 0.7, 1.1 * list[(int)(1 / delta * 0.5)].Y);
            text.FontSpec.Size = 15;
            text.FontSpec.Border.IsVisible = false;
            text1.FontSpec.Size = 15;
            text1.FontSpec.Border.IsVisible = false;
            graphPane2.GraphObjList.Add(text);
            graphPane1.GraphObjList.Add(text1);
            zg1.AxisChange();
            zg1.Invalidate();
            zg1.Show();
            this.Controls.Add(zg1);
          //  zg1.MasterPane.GetImage().Save("Ф.png");

        }

        private PointPairList FindAnSolution(PointPairList list1, double delta)
        {
            double pi = Math.PI;
            int i = 0;
            PointPairList list = new PointPairList();
            for (double x = 0; x<0.5+0.1*delta; x+=delta)
            {
                double y = -pi / 3 * x * x * x * x + pi / 6 * x;
                list.Add(x, Math.Abs(y-list1[i].Y));
                i++;
            }  
            i--;
            for (double x = 0.5; x <=1+0.01*delta; x +=delta)
            {
                double y = -pi / 3 *(1- x) * (1 - x) * (1 - x) * (1 - x) - pi / 6 * x +pi/6;
                list.Add(x, Math.Abs(y - list1[i].Y));
                i++;
            }
            return list;
        }

        private PointPairList FindFi(double delta)
        {
            PointPairList list = new PointPairList();
            var a = Finda(delta);
            for (double x = 0; x < 1+0.01*delta; x += delta)
            {
                list.Add(x, FindFiInPoint(x, delta, a));
            }
            return list;
        }
        private double FindFiInPoint(double x, double delta, double[] a)
        {
            double fi=0;
         //   int i = (int)(x / delta);
            if (x == 0)
                fi = 0;
            else if (Math.Abs(x - 1) <delta)
                fi = 0;
            else
            {
                for (int j = (int)(x / delta); j <= (int)(x / delta) + 2; j++)
                {

                    if (x >= delta * (j - 1) && x <= delta * (j))
                        fi += a[j] * ((x - delta * (j - 1)) / delta);
                    else if (x >= delta * (j) && x <= delta * (j + 1))
                        fi += a[j] * ((delta * (j + 1) - x) / delta);
                }
            }
            return fi;
        }


        public double[] Finda(double delta)
        {     
            var u = FindU(delta);
            var b = Findb(delta);
            var y = FindY(delta, u, b);
            var array = new double[(int)(1 / delta+1)];
            array[array.Length - 1] = y[y.Length - 1];
            for (int i = array.Length - 2; i >= 0; i--)
                array[i] = y[i] - u[i] * array[i + 1];
            array[0] = 0;
            array[array.Length - 1] = 0;
            return array;
        }


        public double[] FindU(double delta)
        {
            double[] uArr = new double[(int)(1 / delta+1)];
            uArr[0] = -0.5;
            for (int i = 1; i < uArr.Length; i++)
                uArr[i] = -1 / (2 + uArr[i - 1]);
            return uArr;

        }

        public double[] Findb(double delta)
        {
            double[] bArr = new double[(int)(1 / delta)];
            for (int i = 1; i < bArr.Length / 2; i++)
                bArr[i] = 4 * Math.PI * delta * (delta * i * delta * i + delta * delta / 6);
            for (int i = bArr.Length / 2; i < bArr.Length-1; i++)
                bArr[i] = 4 * Math.PI * delta * ((1 - i * delta) * (1 - i * delta) + delta * delta / 6);
            return bArr;
        }

        public double[] FindY (double delta, double[] u, double[] b)
        {
            double[] yArr = new double[(int)(1 / delta)];
            yArr[0] = delta*b[0]/2;
            for (int i = 1; i<yArr.Length; i++)
                yArr[i] = (b[i] + yArr[i - 1] / delta) / (1 / delta * (2 + u[i - 1]));
            return yArr;
        }
    }
}
