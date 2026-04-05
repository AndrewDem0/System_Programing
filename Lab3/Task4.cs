using System;
using System.Threading;
using System.Threading.Tasks;

namespace LabParallelTask4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Локальні константи замість полів класу для демонстрації лексичної видимості лямбди
            const double TARGET_VALUE = 500.0;
            const double EPSILON = 0.05;

            int arraySize = 50_000_000;
            double[] data = new double[arraySize];

            Console.WriteLine("Ініціалізація масиву даних...");
            
            Parallel.For(0, data.Length, i => 
            {
                data[i] = i * 0.001; 
            });

            data[12_500_000] = 500.04; 
            data[33_000_000] = 500.01; 

            Console.WriteLine($"Пошук значення в околі {TARGET_VALUE} (відхилення {EPSILON}) через лямбда-вираз...");

            ParallelLoopResult loopResult = Parallel.ForEach(data, (currentValue, loopState) =>
            {
                if (Math.Abs(currentValue - TARGET_VALUE) <= EPSILON)
                {
                    Console.WriteLine($"[Потік {Environment.CurrentManagedThreadId}] Знайдено відповідність: {currentValue}. Виклик loopState.Break().");
                    loopState.Break();
                }
            });

            Console.WriteLine("\n--- ЗВІТ ВИКОНАННЯ ---");
            if (!loopResult.IsCompleted)
            {
                Console.WriteLine($"Статус IsCompleted: {loopResult.IsCompleted}");
                Console.WriteLine($"Найменша ітерація переривання (LowestBreakIteration): {loopResult.LowestBreakIteration}");
                
                if (loopResult.LowestBreakIteration.HasValue)
                {
                    long breakIndex = loopResult.LowestBreakIteration.Value;
                    Console.WriteLine($"Підтверджене значення: {data[breakIndex]}");
                }
            }
            else
            {
                Console.WriteLine("Збігів не знайдено. IsCompleted: true.");
            }
        }
    }
}