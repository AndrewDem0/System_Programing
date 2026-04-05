using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LabParallelTask1
{
    class Program
    {
        // Рівні складності обчислень
        enum Complexity { IntDiv10, DoubleDiv10, DoubleDivPi, DoubleExpDivPowPi, DoubleExpPiDivPowPi }

        static void Main(string[] args)
        {
            Console.WriteLine("Розпочато виконання бенчмарку...\n");
            Console.WriteLine($"{"Тип/Складність",-25} | {"Розмір",-10} | {"Послідовно (с)",-15} | {"Паралельно (с)",-15} | {"Прискорення"}");
            Console.WriteLine(new string('-', 85));

            int[] sizes = { 10_000_000, 50_000_000 }; // Різна кількість елементів

            foreach (int size in sizes)
            {
                RunExperiment(size, Complexity.IntDiv10);
                RunExperiment(size, Complexity.DoubleDiv10);
                RunExperiment(size, Complexity.DoubleDivPi);
                RunExperiment(size, Complexity.DoubleExpDivPowPi);
                RunExperiment(size, Complexity.DoubleExpPiDivPowPi);
                Console.WriteLine(new string('-', 85));
            }
        }

        static void RunExperiment(int size, Complexity complexity)
        {
            Stopwatch sw = new Stopwatch();
            double serialTime = 0, parallelTime = 0;

            if (complexity == Complexity.IntDiv10)
            {
                int[] dataSerial = new int[size];
                int[] dataParallel = new int[size];
                InitializeArray(dataSerial);
                InitializeArray(dataParallel);

                // Послідовне виконання
                sw.Start();
                for (int i = 0; i < dataSerial.Length; i++)
                {
                    dataSerial[i] = dataSerial[i] / 10;
                }
                sw.Stop();
                serialTime = sw.Elapsed.TotalSeconds;

                sw.Restart();
                // Паралельне виконання
                Parallel.For(0, dataParallel.Length, i =>
                {
                    dataParallel[i] = dataParallel[i] / 10;
                });
                sw.Stop();
                parallelTime = sw.Elapsed.TotalSeconds;
            }
            else
            {
                double[] dataSerial = new double[size];
                double[] dataParallel = new double[size];
                InitializeArray(dataSerial);
                InitializeArray(dataParallel);

                // Послідовне виконання
                sw.Start();
                for (int i = 0; i < dataSerial.Length; i++)
                {
                    ExecuteMath(ref dataSerial[i], complexity);
                }
                sw.Stop();
                serialTime = sw.Elapsed.TotalSeconds;

                sw.Restart();
                // Паралельне виконання
                Parallel.For(0, dataParallel.Length, i =>
                {
                    ExecuteMath(ref dataParallel[i], complexity);
                });
                sw.Stop();
                parallelTime = sw.Elapsed.TotalSeconds;
            }

            double speedup = serialTime / parallelTime;
            string testName = GetTestName(complexity);
            Console.WriteLine($"{testName,-25} | {size,-10} | {serialTime,-15:F5} | {parallelTime,-15:F5} | {speedup:F2}x");
        }

        // Ініціалізація базовими значеннями для уникнення ділення на нуль
        static void InitializeArray(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++) arr[i] = i + 1;
        }

        static void InitializeArray(double[] arr)
        {
            for (int i = 0; i < arr.Length; i++) arr[i] = i + 1.5;
        }

        // Маршрутизація математичних операцій
        static void ExecuteMath(ref double x, Complexity complexity)
        {
            switch (complexity)
            {
                case Complexity.DoubleDiv10:
                    x = x / 10.0;
                    break;
                case Complexity.DoubleDivPi:
                    x = x / Math.PI;
                    break;
                case Complexity.DoubleExpDivPowPi:
                    x = Math.Exp(x) / Math.Pow(x, Math.PI);
                    break;
                case Complexity.DoubleExpPiDivPowPi:
                    x = Math.Exp(Math.PI * x) / Math.Pow(x, Math.PI);
                    break;
            }
        }

        static string GetTestName(Complexity complexity)
        {
            return complexity switch
            {
                Complexity.IntDiv10 => "int: x = x / 10",
                Complexity.DoubleDiv10 => "double: x = x / 10",
                Complexity.DoubleDivPi => "double: x = x / PI",
                Complexity.DoubleExpDivPowPi => "double: x = e^x / x^PI",
                Complexity.DoubleExpPiDivPowPi => "double: x = e^(PI*x) / x^PI"
            };
        }
    }
}