namespace Task2
{
    class MyTask
    {
        public Task Tsk;
        public string Name;

        public MyTask(string name)
        {
            Name = name;
            Tsk = new Task(this.Run);
        }

        public void Run()
        {
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"[{Name}] Object ID: {Tsk.Id} | Current Task ID: {Task.CurrentId} | Step: {i}");
                Thread.Sleep(100);
            }
            Console.WriteLine($"--- {Name} (Id:{Tsk.Id}) finished ---");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main thread: Starting 3 tasks...");
            Console.WriteLine($"Main thread Task.CurrentId: {Task.CurrentId ?? 0} (null)");

            MyTask mt1 = new MyTask("Alpha");
            MyTask mt2 = new MyTask("Bravo");
            MyTask mt3 = new MyTask("Charlie");

            mt1.Tsk.Start();
            mt2.Tsk.Start();
            mt3.Tsk.Start();

            Task.WaitAll(mt1.Tsk, mt2.Tsk, mt3.Tsk);

            Console.WriteLine("All tasks completed. Main thread finishes.");
        }
    }
}