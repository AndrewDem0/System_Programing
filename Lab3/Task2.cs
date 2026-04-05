using System;
using System.Threading;
using System.Threading.Tasks;

namespace LabParallelTask2
{
    class Program
    {
        private const double TARGET_VALUE = 500.0;
        private const double EPSILON = 0.05;

        static void Main(string[] args)
        {
            int arraySize = 50_000_000;
            double[] data = new double[arraySize];

            Console.WriteLine("Ініціалізація масиву даних...");
            
            Parallel.For(0, data.Length, i => 
            {
                data[i] = i * 0.001; 
            });


            data[15_000_000] = 500.03; 
            data[25_000_000] = 500.01; 

            Console.WriteLine($"Пошук значення в околі числа {TARGET_VALUE} з відхиленням {EPSILON}...");

            ParallelLoopResult loopResult = Parallel.For(0, data.Length, (i, state) =>
            {
                // Математична перевірка: |x - a| <= ε
                if (Math.Abs(data[i] - TARGET_VALUE) <= EPSILON)
                {
                    Console.WriteLine($"[Потік {Environment.CurrentManagedThreadId}] Знайдено відповідність на індексі {i} (Значення: {data[i]}). Ініціація Break()...");
                    
                    state.Break();
                }
            });

            Console.WriteLine("\n--- ЗВІТ ВИКОНАННЯ ---");
            if (!loopResult.IsCompleted)
            {
                Console.WriteLine($"Статус IsCompleted: {loopResult.IsCompleted}");
                Console.WriteLine($"Найменший індекс переривання (LowestBreakIteration): {loopResult.LowestBreakIteration}");
                
                if (loopResult.LowestBreakIteration.HasValue)
                {
                    long breakIndex = loopResult.LowestBreakIteration.Value;
                    Console.WriteLine($"Підтверджене значення на індексі переривання: {data[breakIndex]}");
                }
            }
            else
            {
                Console.WriteLine("Збігів не знайдено. Усі ітерації завершено (IsCompleted: true).");
            }
        }
    }
}