using System;
using System.Diagnostics;

namespace SplineLagrange
{
    internal class Program
    {
        static List<double> points = new List<double>();    // точки по ОХ
        static List<double> fx = new List<double>();        // точки по ОY 
        static List<double> Lfx = new List<double>();       // точки по ОY 

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
            var result = points.Concat(Lfx);

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

        static private double funtion(double x) => Math.Pow(x, 6);

        static private void LagrangePolynomial()
        {
            int n = points.Count;
            double lk = 1;
            double result = 0;

            for (int i = 0; i < n; i++)
            {
                for (int k = 0; k < n; k++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (j!=k)
                        {
                            lk *= (points[i] - points[j]) / (points[k] - points[j]);
                        }
                        else
                        {
                            lk *= 1;
                        }
                    }
                    result += fx[k] * lk;
                    lk = 1;
                }
                Console.WriteLine(result);
                Lfx.Add(result);
                result = 0;
            }
        }

        static void Main(string[] args)
        {
            string MapPath = Path.Combine(Directory.GetCurrentDirectory(), "map.txt");
            ReadMash(MapPath);

            foreach (var point in points)
            {
                fx.Add(funtion(point));
            }
            LagrangePolynomial();
            DrawPlot();
        }
    }
}