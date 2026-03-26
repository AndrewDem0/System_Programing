
namespace Task1
{
    class MyTask
    {
        public Task Tsk;
        public string TaskName;

        public MyTask(string name, Action action)
        {
            TaskName = name;
            Tsk = new Task(action);
        }

        public static void DisplayNumbers()
        {
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine($"[Task Numbers] Value: {i}");
                Thread.Sleep(200);
            }
        }

        public static void DisplayLetters()
        {
            for (char c = 'A'; c <= 'J'; c++)
            {
                Console.WriteLine($"[Task Letters] Value: {c}");
                Thread.Sleep(200);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main thread started.");

            MyTask m1 = new MyTask("NumberTask", MyTask.DisplayNumbers);
            MyTask m2 = new MyTask("LetterTask", MyTask.DisplayLetters);

            m1.Tsk.Start();
            m2.Tsk.Start();

            Task.WaitAll(m1.Tsk, m2.Tsk);

            Console.WriteLine("Main thread completed. All tasks finished.");
        }
    }
}