using System;
using System.Threading.Tasks;

namespace Lesson14
{
    class Program
    {
        static async Task ParallelWork(string name)
        {
            Console.WriteLine($"{name} выполняется.");
            await Task.Delay(1000);
            Console.WriteLine($"{name} завершена.");
        }

        static void Square(int a)
        {
            Console.WriteLine($"Квадрат числа {a} равен {Math.Pow(a, 2)}");
        }

        private static SemaphoreSlim sS = new SemaphoreSlim(2, 2); 
        static async Task FileWork(int i)
        {
            string nameFile = $"Test{i}.txt";
            string contentText = $"Hello * {i}";
            await sS.WaitAsync();
            if(!File.Exists(nameFile))
            {
                File.WriteAllText(nameFile, contentText);
                await Task.Delay(2000);
                Console.WriteLine("Файл усаешно создан");
                sS.Release();
            }
            else
            {
                string content = File.ReadAllText(nameFile);
                Console.WriteLine(content);
                await Task.Delay(2000);
                sS.Release();
            }
        }
        static async Task Main(string[] args)
        {
            // // Задача 1
            // Task[] tasks = new Task[5];
            // for (int i = 0; i < tasks.Length; i++)
            // {
            //     int iNum = i + 1;
            //     tasks[i] = Task.Run(() => ParallelWork($"Задача {iNum}"));
            // }
            // await Task.WhenAll(tasks);
            // Console.WriteLine("Все задачи выполнены");

            // // Задача 2
            // Parallel.For(1, 101, Square);

            // Задача 3
            for (int i = 1; i <= 5; i++)
            {
                int taskId = i;
                Task.Run(() => FileWork(taskId));
            }
            await Task.Delay(5000);
        }
    }
}