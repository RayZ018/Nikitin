using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Task4
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
            PrepareTestFiles(folderPath);

            Console.WriteLine($"Рабочая папка: {folderPath}");
            string[] files = Directory.GetFiles(folderPath, "*.txt");
            Console.WriteLine($"Найдено файлов для теста: {files.Length}\n");

            Stopwatch sw1 = Stopwatch.StartNew();
            long totalSpaces1 = await CountSpacesPerFileParallel(files);
            sw1.Stop();
            Console.WriteLine($"Сценарий 1 (Task на файл): Пробелов {totalSpaces1}, Время: {sw1.ElapsedMilliseconds} мс");

            Stopwatch sw2 = Stopwatch.StartNew();
            long totalSpaces2 = await CountSpacesPerLineParallel(files);
            sw2.Stop();
            Console.WriteLine($"Сценарий 2 (Task на строку): Пробелов {totalSpaces2}, Время: {sw2.ElapsedMilliseconds} мс");

            Console.WriteLine("\nТест завершен. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static void PrepareTestFiles(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
            Directory.CreateDirectory(path);

            for (int i = 1; i <= 10; i++)
            {
                string fileName = Path.Combine(path, $"file_{i}.txt");
                var lines = Enumerable.Range(0, 1000).Select(_ => "Word word word word word ");
                File.WriteAllLines(fileName, lines);
            }
            Console.WriteLine("Тестовые файлы успешно созданы.");
        }

        static async Task<long> CountSpacesPerFileParallel(string[] files)
        {
            var tasks = files.Select(file => Task.Run(() =>
            {
                string content = File.ReadAllText(file);
                return (long)content.Count(c => c == ' ');
            }));

            long[] results = await Task.WhenAll(tasks);
            return results.Sum();
        }

        static async Task<long> CountSpacesPerLineParallel(string[] files)
        {
            var fileTasks = files.Select(async file =>
            {
                var lines = await File.ReadAllLinesAsync(file);
                var lineTasks = lines.Select(line => Task.Run(() =>
                    (long)line.Count(c => c == ' ')));

                long[] lineResults = await Task.WhenAll(lineTasks);
                return lineResults.Sum();
            });

            long[] totalResults = await Task.WhenAll(fileTasks);
            return totalResults.Sum();
        }
    }
}