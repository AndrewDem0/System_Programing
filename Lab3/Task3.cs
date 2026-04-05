using System;
using System.Threading;
using System.Threading.Tasks;

namespace LabParallelTask3
{
    class Program
    {
        // Константи умов пошуку
        private const double TARGET_VALUE = 500.0;
        private const double EPSILON = 0.05;

        // Іменований метод, що відповідає сигнатурі Action<double, ParallelLoopState>
        static void ProcessElement(double value, ParallelLoopState pls)
        {
            // Математична перевірка: |x - a| <= ε
            if (Math.Abs(value - TARGET_VALUE) <= EPSILON)
            {
                Console.WriteLine($"[Потік {Environment.CurrentManagedThreadId}] Знайдено відповідність. Значення: {value}. Ініціація Break().");
                
                pls.Break();
            }
        }

        static void Main(string[] args)
        {
            int arraySize = 50_000_000;
            double[] data = new double[arraySize];

            Console.WriteLine("Ініціалізація масиву даних...");
            
            Parallel.For(0, data.Length, i => 
            {
                data[i] = i * 0.001; 
            });

            data[20_000_000] = 500.04; 
            data[40_000_000] = 500.02; 

            Console.WriteLine($"Пошук значення в околі {TARGET_VALUE} (відхилення {EPSILON}) за допомогою ForEach...");

            ParallelLoopResult loopResult = Parallel.ForEach(data, ProcessElement);

            Console.WriteLine("\n--- ЗВІТ ВИКОНАННЯ ---");
            if (!loopResult.IsCompleted)
            {
                Console.WriteLine($"Статус IsCompleted: {loopResult.IsCompleted}");
                Console.WriteLine($"Найменша ітерація переривання (LowestBreakIteration): {loopResult.LowestBreakIteration}");
                
                if (loopResult.LowestBreakIteration.HasValue)
                {
                    long breakIndex = loopResult.LowestBreakIteration.Value;
                    Console.WriteLine($"Підтверджене значення за індексом ітерації {breakIndex}: {data[breakIndex]}");
                }
            }
            else
            {
                Console.WriteLine("Збігів не знайдено.");
            }
        }
    }
}