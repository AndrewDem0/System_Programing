
namespace Task2
{
    // Клас для звичайних (foreground) потоків
    class PriorityThread
    {
        public int Count = 0;
        public Thread newThrd;

        public PriorityThread(string name)
        {
            newThrd = new Thread(this.Run);
            newThrd.Name = name;

            newThrd.Start();
        }

        public void Run()
        {
            Console.WriteLine(newThrd.Name + " (Priority) starting.");
            // Виконуємо обмежену кількість ітерацій, щоб потік завершився
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine($"[{newThrd.Name}] Working... {i}");
                Thread.Sleep(200);
            }
            Console.WriteLine(newThrd.Name + " is completed.");
        }
    }

    class BackgroundThread
    {
        public Thread newThrd;

        public BackgroundThread(string name)
        {
            newThrd = new Thread(this.Run);
            newThrd.Name = name;
            newThrd.IsBackground = true;
            
            newThrd.Start();
        }

        public void Run()
        {
            Console.WriteLine(newThrd.Name + " (Background) starting.");
            int count = 0;
            // Нескінченний цикл
            while (true)
            {
                Console.WriteLine($"---> [{newThrd.Name}] I'm working in background... {count++}");
                Thread.Sleep(100);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main thread is beginning.");

            PriorityThread t1 = new PriorityThread("Priority_1");
            PriorityThread t2 = new PriorityThread("Priority_2");

            BackgroundThread bg = new BackgroundThread("Background_Worker");

            t1.newThrd.Join();
            t2.newThrd.Join();

            Console.WriteLine("Main thread is completed. All priority threads are dead.");

        }
    }
}