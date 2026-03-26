namespace Task3
{
    class SumCalculator
    {
        public int N;

        public SumCalculator(int n)
        {
            N = n;
        }

        public int Calculate()
        {
            Console.WriteLine($"[Task 1] Calculating sum from 1 to {N}...");
            int sum = 0;
            for (int i = 1; i <= N; i++)
            {
                sum += i;
            }
            return sum;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter N: ");
            if (!int.TryParse(Console.ReadLine(), out int n)) return;

            SumCalculator calc = new SumCalculator(n);

            Task<int> task1 = new Task<int>(calc.Calculate);
            Task task2 = task1.ContinueWith(antecedent =>
            {
                Console.WriteLine($"[Task 2] Continuation started.");
                Console.WriteLine($"[Result] The total sum is: {antecedent.Result}");
            });

            task1.Start();
            task2.Wait();

            Console.WriteLine("Main thread: All work finished.");
        }
    }
}