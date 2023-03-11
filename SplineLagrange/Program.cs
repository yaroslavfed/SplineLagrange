using System;
using System.Diagnostics;

namespace SplineLagrange
{
    internal class Program
    {
        static List<double> x_points = new List<double>();    // точки по ОХ
        static List<double> y_points = new List<double>();    // точки по ОY 

        static private void ReadMash(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                foreach (var number in sr.ReadLine()!.Split(' '))
                {
                    x_points.Add(double.Parse(number));
                }
            };
        }

        static private double funtion(double x) => Math.Pow(x, 2);

        static void Main(string[] args)
        {
            string MapPath = @"map.txt";
            ReadMash(MapPath);

            foreach (var point in x_points)
            {
                y_points.Add(funtion(point));
                System.Console.WriteLine(point);
            }

#if true
            var result = x_points.Concat(y_points);

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
    }
}