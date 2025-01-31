using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

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

        private static object locker = new object();
        static int[] WorkNotParallel(int[] ints, int p)
        {
            int parts = p;
            var ch = ints.Chunk(parts);
            int[] z = new int[ints.Length/parts];
            int index = 0;
            foreach (var item in ch)
            {
                int result = 0;
                foreach (var item1 in item)
                {
                    result += item1; 
                }
                z[index] = result;
                index++;
            }
            return z;
        }

        static int[] WorkWithParallel(int[] ints, int p)
        {
            var chunks = ints.Chunk(ints.Length/p).ToArray();
            int[] z = new int[chunks.Length];

            Parallel.For(0, p, i =>
            {
                int result = 0;
                foreach (var item in chunks[i])
                {                    
                    lock (locker)
                    {
                        result += item;
                    }
                }
                
                z[i] = result;
            });

            return z;
        }
        static void Main(string[] args)
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

            // // Задача 3
            // for (int i = 1; i <= 5; i++)
            // {
            //     int taskId = i;
            //     Task.Run(() => FileWork(taskId));
            // }
            // await Task.Delay(5000);

            // Задача 4
            int[] a = new int[60];
            Random r = new Random();
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = r.Next(1, 100);
            }
            Stopwatch sw = Stopwatch.StartNew();
            int[] z = WorkNotParallel(a, 10);
            sw.Stop();
            foreach (var item in z)
            {
                Console.WriteLine($"Результат:{item}");
            }
            Console.WriteLine($"Время: {sw.ElapsedMilliseconds} мс");


            sw.Restart();
            int[] z2 = WorkWithParallel(a, 10);
            sw.Stop();
            foreach (var item in z)
            {
                Console.WriteLine($"Результат:{item}");
            }
            Console.WriteLine($"Время: {sw.ElapsedMilliseconds} мс");
        }
    }
}