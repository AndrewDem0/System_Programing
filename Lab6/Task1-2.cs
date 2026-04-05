
namespace Lab6FileSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА 6: ФАЙЛОВЕ ВВЕДЕННЯ-ВИВЕДЕННЯ ===");
            
            string targetDirectory = "/home/marino/My pc/4 курс/System programing";

            Console.WriteLine($"\n[ЗАВДАННЯ 1]: Ієрархія директорії: {targetDirectory}\n");
            PrintDirectoryTree(new DirectoryInfo(targetDirectory), "", true);

            Console.WriteLine(new string('-', 60));
            
            Console.Write("\n[ЗАВДАННЯ 2]: Введіть точне ім'я файлу для пошуку (наприклад, Program.cs): ");
            string searchFileName = Console.ReadLine();
            
            if (!string.IsNullOrWhiteSpace(searchFileName))
            {
                Console.WriteLine($"\nІніціалізація пошуку файлу '{searchFileName}' у директорії {targetDirectory}...\n");
                SearchFile(new DirectoryInfo(targetDirectory), searchFileName);
            }
        }

        static void PrintDirectoryTree(DirectoryInfo directory, string indent, bool isLast)
        {
            try
            {
                // Графічне форматування вузлів дерева
                Console.Write(indent);
                Console.Write(isLast ? "└── " : "├── ");
                Console.WriteLine($"[{directory.Name}]");

                indent += isLast ? "    " : "│   ";

                FileInfo[] files = directory.GetFiles();
                DirectoryInfo[] subDirs = directory.GetDirectories();

                // Виведення файлів поточного каталогу
                for (int i = 0; i < files.Length; i++)
                {
                    bool isLastFile = (i == files.Length - 1) && (subDirs.Length == 0);
                    Console.Write(indent);
                    Console.Write(isLastFile ? "└── " : "├── ");
                    Console.WriteLine(files[i].Name);
                }

                // Рекурсивний виклик для підкаталогів
                for (int i = 0; i < subDirs.Length; i++)
                {
                    PrintDirectoryTree(subDirs[i], indent, i == subDirs.Length - 1);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Перехоплення помилки доступу (наприклад, системні папки root)
                Console.WriteLine($"{indent}└── [ПОМИЛКА ДОСТУПУ: {directory.Name}]");
            }
        }


        static void SearchFile(DirectoryInfo directory, string fileName)
        {
            try
            {
                FileInfo[] foundFiles = directory.GetFiles(fileName);

                foreach (FileInfo file in foundFiles)
                {
                    Console.WriteLine($"[ЗНАЙДЕНО]: {file.FullName}");
                    Console.WriteLine($"   ├── Розмір: {file.Length} байт");
                    Console.WriteLine($"   ├── Створено: {file.CreationTime}");
                    Console.WriteLine($"   └── Атрибути: {file.Attributes}\n");
                }

                DirectoryInfo[] subDirs = directory.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    SearchFile(subDir, fileName);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}