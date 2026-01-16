using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
 
namespace Task3
{
    public class FileArgs : EventArgs
    {
        public string FileName { get; }
        public bool Cancel { get; set; }

        public FileArgs(string fileName)
        {
            FileName = fileName; 
            Cancel = false;
        }
    }

    public class FileSearcher
    {
        public event EventHandler<FileArgs> FileFound;

        public void Search(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Ошибка: Указанный путь не существует.");
                return;
            }

            try
            {
                foreach (var file in Directory.EnumerateFiles(directoryPath))
                {
                    var args = new FileArgs(file);

                    OnFileFound(args);

                    if (args.Cancel)
                    {
                        Console.WriteLine("\n[Инфо] Поиск был прерван обработчиком.");
                        break;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка: Нет доступа к одной из папок.");
            }
        }

        protected virtual void OnFileFound(FileArgs e)
        {
            FileFound?.Invoke(this, e);
        }
    }

    public static class EnumerableExtensions
    {
        public static T GetMax<T>(this IEnumerable<T> collection, Func<T, float> convertToNumber) where T : class
        {
            if (collection == null || !collection.Any())
                return null;

            T maxElement = null;
            float maxValue = float.MinValue;

            foreach (var item in collection)
            {
                float currentValue = convertToNumber(item);
                if (currentValue > maxValue)
                {
                    maxValue = currentValue;
                    maxElement = item;
                }
            }

            return maxElement;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Часть 1: Поиск максимального элемента ===");

            var items = new List<string> { "Кошка", "Александра", "Дом", "Программирование", "C#" };

            var longestWord = items.GetMax(s => s.Length);

            Console.WriteLine($"Коллекция: {string.Join(", ", items)}");
            Console.WriteLine($"Результат (самое длинное слово): {longestWord}");


            Console.WriteLine("\n=== Часть 2: Поиск файлов с событиями ===");

            var searcher = new FileSearcher();
            int fileCount = 0;

            searcher.FileFound += (sender, e) =>
            {
                fileCount++;
                Console.WriteLine($"[{fileCount}] Найден файл: {Path.GetFileName(e.FileName)}");

                if (fileCount >= 5)
                {
                    Console.WriteLine("Достигнут лимит в 5 файлов. Посылаем сигнал отмены...");
                    e.Cancel = true;
                }
            };

            string pathForSearch = @"C:\Windows";
            Console.WriteLine($"Сканируем папку: {pathForSearch}");
            searcher.Search(pathForSearch);

            Console.WriteLine("\nРабота программы завершена. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}