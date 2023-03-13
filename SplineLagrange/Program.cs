using System.Diagnostics;
using static System.Math;

namespace SplineLagrange
{
    internal class Program
    {
        static List<double> points = new List<double>();    // известные точки
        static List<double> x = new List<double>();         // весь х
        //static List<double> fx = new List<double>();      // f(x)
        static List<double> Lfx = new List<double>();       // значение полинома Лагранжа в x

        static double a, b;                                 // границы области
        static double globalStep = 0.001;                    // шаг для вывода графика
        static double step;                    // шаг на элементе

        static private void ReadMash(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                foreach (var number in sr.ReadLine()!.Split(' '))
                {
                    points.Add(double.Parse(number));
                }
            };
        }

        static private void DrawPlot()
        {
#if true
            //var result = points.Concat(Lfx);
            var result = x.Concat(Lfx);

            using Process myProcess = new Process();
            myProcess.StartInfo.FileName = "python";
            myProcess.StartInfo.Arguments = @"script.py";
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardInput = true;
            myProcess.StartInfo.RedirectStandardOutput = false;
            myProcess.Start();

            using BinaryWriter writer = new BinaryWriter(myProcess.StandardInput.BaseStream);
            Array.ForEach(result.ToArray(), writer.Write);
            writer.Flush();
#endif
        }

        static private double funtion(double x) => Math.Pow(E, Sin(PI * x));     //Math.Pow(x, 3); //Math.Pow(E, Sin(PI * x));



        #region PiecewiseLinearLagrange
        static private void PiecewiseLinearLagrange()
        {
            int n = points.Count;

            Console.Write("Узлы: ");
            foreach (double point in points)
                Console.Write("{0} ", point);                
            Console.WriteLine();

            double result = 0;
            for (int p = 0; p < n - 1; p++)
            {
                step = points[p + 1] - points[p];
                for (double k = points[p]; k <= points[p + 1]; k += globalStep)
                {
                    x.Add(k);
                    result = funtion(points[p]) * (points[p + 1] - k) / (step) + funtion(points[p + 1]) * (k - points[p]) / (step);
                    Console.WriteLine("x: {0}\ty: {1}", k, result);
                    Lfx.Add(result);
                }
            }
        }
        #endregion PiecewiseLinearLagrange

        #region LagrangePolynomial
        static private void LagrangePolynomial()
        {
            int n = points.Count;

            foreach (double point in points)
                Console.Write("{0} ", point);
            Console.WriteLine();

            for (double k = a; k <= b; k += globalStep)
            {
                x.Add(k);
                double sum = 0;
                for (int i = 0; i < n; i++)
                {
                    double li = 1;
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                        {
                            li *= (k - points[j]) / (points[i] - points[j]);
                        }
                        else
                        {
                            li *= 1;
                        }
                    }
                    li *= funtion(points[i]);
                    sum += li;
                }
                Lfx.Add(sum);
                Console.WriteLine("x: {0}\ty: {1}", k, sum);
            }
        }
        #endregion LagrangePolynomial

        static void Main(string[] args)
        {
            string MapPath = Path.Combine(Directory.GetCurrentDirectory(), "map.txt");
            ReadMash(MapPath);
            //LagrangePolynomial();
            //PiecewiseLinearLagrange();
            DrawPlot();
        }
    }
}