
namespace Task1
{
    class NumberThread
    {
        public int Count = 0;
        public Thread newThrd;

        public NumberThread(string name)
        {
            newThrd = new Thread(this.Run);
            newThrd.Name = name;
            newThrd.Start();
        }

        public void Run()
        {
            Console.WriteLine(newThrd.Name + " starting.");
            for (int i = 1; i <= 40; i++)
            {
                Console.WriteLine($"[{newThrd.Name}] Number: {i}");
                Count = i; 
                
                Thread.Sleep(200); 
            }
            Console.WriteLine(newThrd.Name + " is completed.");
        }
    }

    class LetterThread
    {
        public Thread newThrd;

        public LetterThread(string name)
        {
            newThrd = new Thread(this.Run);
            newThrd.Name = name;
            newThrd.Start();
        }

        public void Run()
        {
            Console.WriteLine(newThrd.Name + " starting.");
            for (char c = 'A'; c <= 'Z'; c++)
            {
                Console.WriteLine($"[{newThrd.Name}] Letter: {c}");
                
                Thread.Sleep(300); 
            }
            Console.WriteLine(newThrd.Name + " is completed.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main thread is beginning.");

            NumberThread nt = new NumberThread("NumberThread");
            LetterThread lt = new LetterThread("LetterThread");

            nt.newThrd.Join();
            lt.newThrd.Join(); 

            Console.WriteLine("\nMain thread is completed.");
            Console.ReadLine();
        }
    }
}