﻿using System.Diagnostics;
using System.Drawing;
using static System.Math;

namespace SplineLagrange
{
    internal class Program
    {
        static List<double> points = new List<double>();    // известные точки
        static List<double> x = new List<double>();         // весь х
        static List<double> Lfx = new List<double>();       // значение полинома Лагранжа в x

        static double a, b;                                 // границы области
        static double globalStep = 0.001;                   // шаг для вывода графика

        static private void ReadMash(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                foreach (var number in sr.ReadLine()!.Split(' '))
                {
                    points.Add(double.Parse(number));
                }
            };
            a = points[0];
            b = points[points.Count - 1];
        }

        static private void OutputToImage()
        {
            var result = x.Concat(Lfx);

            using Process myProcess = new Process();
            myProcess.StartInfo.FileName = "python";
            myProcess.StartInfo.Arguments = @"script.py";
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardInput = true;
            myProcess.StartInfo.RedirectStandardOutput = false;
            myProcess.Start();

            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                foreach (var point in points)
                    sw.Write(point + " ");
                sw.WriteLine();

                foreach (var point in points)
                    sw.Write(function(point) + " ");
                sw.WriteLine();

                foreach (var point in x)
                    sw.Write(point + " ");
                sw.WriteLine();

                foreach (var point in Lfx)
                    sw.Write(point + " ");
                sw.WriteLine();
            };
        }

        static private double function(double x) => 1/(1+25*Pow(x, 2));                   // Math.Pow(x, 3);      Math.Pow(E, Sin(PI * x));      Math.Pow(Math.Abs(x), Sin(Math.PI * x))
        static private double ksi(double x, double xi, double h) => (x - xi) / h;
        static private double alfa(double xiplus, double xi, double h) => (xiplus - xi) / h;

        #region PiecewiseQuadraticLagrange
        static private double BasicFunctions(int i, double x, double xi, double xiplus, double h)
        {
            switch (i)
            {
                case 0:
                    return ((ksi(x, xi, h) - alfa(xiplus, xi, h)) / ((-alfa(xiplus, xi, h)))) * ((ksi(x, xi, h) - 1) / (-1));
                case 1:
                    return (ksi(x, xi, h) / alfa(xiplus, xi, h)) * ((ksi(x, xi, h) - 1) / (alfa(xiplus, xi, h) - 1));
                case 2:
                    return (ksi(x, xi, h) / 1) * ((ksi(x, xi, h) - alfa(xiplus, xi, h)) / ((1 - alfa(xiplus, xi, h))));
                default:
                    return 0;
            }
        }

        static private void PiecewiseQuadraticLagrange()
        {
            List<double> splitMesh = new List<double>();

            Console.Write("Узлы: ");
            foreach (double point in points)
                Console.Write("{0} ", point);
            Console.WriteLine();

            for (int i = 0; i < points.Count - 1; i++)
            {
                double middle = points[i];
                splitMesh.Add(points[i]);
                middle += (points[i + 1] - points[i]) / 2;
                splitMesh.Add(middle);
            }
            splitMesh.Add(b);

            Console.Write("Новые узлы: ");
            foreach (double x in splitMesh)
                Console.Write("{0} ", x);
            Console.WriteLine();

            int n = splitMesh.Count;

            double result = 0;
            for (int p = 0; p < n - 1; p += 2)
            {
                double step = splitMesh[p + 2] - splitMesh[p];
                for (double k = splitMesh[p]; k <= splitMesh[p + 2]; k += globalStep)
                {
                    x.Add(k);
                    result = function(splitMesh[p]) * BasicFunctions(0, k, splitMesh[p], splitMesh[p + 1], step) + function(splitMesh[p + 1]) * BasicFunctions(1, k, splitMesh[p], splitMesh[p + 1], step) + function(splitMesh[p + 2]) * BasicFunctions(2, k, splitMesh[p], splitMesh[p + 1], step);
                    Console.WriteLine("x: {0}\ty: {1}", k, result);
                    Lfx.Add(result);
                }
            }
            result = function(splitMesh[n - 1]);
            Console.WriteLine("x: {0}\ty: {1}", splitMesh[n - 1], result);
            x.Add(splitMesh[n - 1]);
            Lfx.Add(result);

            points.Clear();
            foreach (var point in splitMesh)
            {
                points.Add(point);
            }
        }
        #endregion PiecewiseQuadraticLagrange

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
                double step = points[p + 1] - points[p];
                for (double k = points[p]; k <= points[p + 1]; k += globalStep)
                {
                    x.Add(k);
                    //result = function(points[p]) * (points[p + 1] - k) / (step) + function(points[p + 1]) * (k - points[p]) / (step);
                    result = function(points[p]) * (1 - ksi(k, points[p], step)) + function(points[p + 1]) * ksi(k, points[p], step);
                    Console.WriteLine("x: {0}\ty: {1}", k, result);
                    Lfx.Add(result);
                }
            }
            result = function(b);
            Console.WriteLine("x: {0}\ty: {1}", b, result);
            x.Add(b);
            Lfx.Add(result);
        }
        #endregion PiecewiseLinearLagrange

        #region LagrangePolynomial
        static private void LagrangePolynomial()
        {
            int n = points.Count;

            Console.Write("Узлы: ");
            foreach (double point in points)
                Console.Write("{0} ", point);
            Console.WriteLine();

            for (double k = a; k < b; k += globalStep)
            {
                x.Add(k);
                double sum = 0;
                for (int i = 0; i < n; i++)
                {
                    double li = 1;
                    for (int j = 0; j < n; j++)
                    {
                        double step = points[i] - points[j];
                        if (j != i)
                        {
                            li *= (k - points[j]) / step;
                        }
                        else
                        {
                            li *= 1;
                        }
                    }
                    li *= function(points[i]);
                    sum += li;
                }
                Lfx.Add(sum);
                Console.WriteLine("x: {0}\ty: {1}", k, sum);
            }
            Console.WriteLine("x: {0}\ty: {1}", b, function(b));
            x.Add(b);
            Lfx.Add(function(b));
        }
        #endregion LagrangePolynomial

        #region TwoPointPiecewiseCubicLagrange

        static private void TwoPointPiecewiseCubicLagrange()
        {
            List<double> splitMesh = new List<double>();

            Console.Write("Узлы: ");
            foreach (double point in points)
                Console.Write("{0} ", point);
            Console.WriteLine();

            for (int i = 0; i < points.Count - 1; i++)
            {
                double middle = points[i];
                splitMesh.Add(points[i]);
                middle += (points[i + 1] - points[i]) / 2;
                splitMesh.Add(middle);
            }
            splitMesh.Add(b);

            Console.Write("Новые узлы: ");
            foreach (double x in splitMesh)
                Console.Write("{0} ", x);
            Console.WriteLine();

            int n = splitMesh.Count;

            double result = 0;
            for (int p = 0; p < n - 1; p += 2)
            {
                double step = splitMesh[p + 2] - splitMesh[p];
                for (double k = splitMesh[p]; k <= splitMesh[p + 2]; k += globalStep)
                {
                    x.Add(k);
                    result = function(splitMesh[p]) * BasicFunctions(0, k, splitMesh[p], splitMesh[p + 1], step) + function(splitMesh[p + 1]) * BasicFunctions(1, k, splitMesh[p], splitMesh[p + 1], step) + function(splitMesh[p + 2]) * BasicFunctions(2, k, splitMesh[p], splitMesh[p + 1], step);
                    Console.WriteLine("x: {0}\ty: {1}", k, result);
                    Lfx.Add(result);
                }
            }
            result = function(splitMesh[n - 1]);
            Console.WriteLine("x: {0}\ty: {1}", splitMesh[n - 1], result);
            x.Add(splitMesh[n - 1]);
            Lfx.Add(result);

            points.Clear();
            foreach (var point in splitMesh)
            {
                points.Add(point);
            }
        }
        #endregion TwoPointPiecewiseCubicLagrange

        static void Main(string[] args)
        {
            string mapPath = Path.Combine(Directory.GetCurrentDirectory(), "map.txt");
            ReadMash(mapPath);

            #region AllMethods
            //LagrangePolynomial();
            //PiecewiseLinearLagrange();
            PiecewiseQuadraticLagrange();
            //TwoPointPiecewiseCubicLagrange();
            #endregion AllMethods

            OutputToImage();
        }
    }
}