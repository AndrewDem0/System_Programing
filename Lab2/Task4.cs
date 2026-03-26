namespace Task4
{
    class MathWorker
    {
        public void CalculateFactorial(int n)
        {
            long result = 1;
            for (int i = 1; i <= n; i++) result *= i;
            
            Console.WriteLine($"[Factorial] {n}! = {result}");
        }

        public void CalculateSum(int n)
        {
            long sum = 0;
            for (int i = 1; i <= n; i++) sum += i;
            
            Console.WriteLine($"[Sum] Total from 1 to {n} = {sum}");
        }

        public void ShowMessage()
        {
            Console.WriteLine("[Message] Task started...");
            Thread.Sleep(300);
            Console.WriteLine("[Message] 300ms pause finished.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MathWorker worker = new MathWorker();
            
            Console.Write("Enter N for calculations: ");
            if (!int.TryParse(Console.ReadLine(), out int n)) return;

            Console.WriteLine("\n--- Parallel.Invoke starting ---");
            
            Parallel.Invoke(
                () => worker.CalculateFactorial(n),
                () => worker.CalculateSum(n),
                () => worker.ShowMessage()
            );

            Console.WriteLine("--- Parallel.Invoke finished ---");
            Console.WriteLine("Main thread: All parallel methods completed.");
        }
    }
}