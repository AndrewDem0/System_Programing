using System;
using System.Threading;

namespace Task3
{
    class MyThread
    {
        public long Count = 0;
        public Thread Thrd;
        public static bool Stop = false;
        // Зберігаємо копію пріоритету, щоб прочитати її після смерті потоку
        public ThreadPriority SavedPriority; 

        public MyThread(string name)
        {
            Thrd = new Thread(this.Run);
            Thrd.Name = name;
        }

        public void Run()
        {
            Console.WriteLine(Thrd.Name + " starting.");
            while (!Stop)
            {
                Count++;
            }
            Console.WriteLine($"\n[{Thrd.Name}] Last 5 values: {Count-4} {Count-3} {Count-2} {Count-1} {Count}");
            Console.WriteLine(Thrd.Name + " is completed.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main thread is beginning.");

            MyThread mt1 = new MyThread("Highest_Thr");
            MyThread mt2 = new MyThread("Lowest_Thr");
            MyThread mt3 = new MyThread("AboveNorm_Thr");
            MyThread mt4 = new MyThread("Normal_Thr");
            MyThread mt5 = new MyThread("BelowNorm_Thr");

            // Встановлюємо пріоритет і ЗБЕРІГАЄМО його в нашому класі
            mt1.Thrd.Priority = mt1.SavedPriority = ThreadPriority.Highest;
            mt2.Thrd.Priority = mt2.SavedPriority = ThreadPriority.Lowest;
            mt3.Thrd.Priority = mt3.SavedPriority = ThreadPriority.AboveNormal;
            mt4.Thrd.Priority = mt4.SavedPriority = ThreadPriority.Normal;
            mt5.Thrd.Priority = mt5.SavedPriority = ThreadPriority.BelowNormal;

            mt1.Thrd.Start(); mt2.Thrd.Start(); mt3.Thrd.Start();
            mt4.Thrd.Start(); mt5.Thrd.Start();

            Thread.Sleep(5000);
            MyThread.Stop = true;

            mt1.Thrd.Join(); mt2.Thrd.Join(); mt3.Thrd.Join();
            mt4.Thrd.Join(); mt5.Thrd.Join();

            long total = mt1.Count + mt2.Count + mt3.Count + mt4.Count + mt5.Count;

            Console.WriteLine("\n========================================");
            Console.WriteLine("CPU TIME DISTRIBUTION:");
            Console.WriteLine("----------------------------------------");
            
            // Тепер передаємо SavedPriority замість звернення до Thrd.Priority
            PrintStats(mt1, total);
            PrintStats(mt3, total);
            PrintStats(mt4, total);
            PrintStats(mt5, total);
            PrintStats(mt2, total);
            
            Console.WriteLine("========================================");
        }

        static void PrintStats(MyThread mt, long total)
        {
            double pc = (double)mt.Count / total * 100;
            // Використовуємо збережене значення пріоритету
            Console.WriteLine($"{mt.Thrd.Name,-15} ({mt.SavedPriority,-12}): {pc:F2}%");
        }
    }
}