using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task3
{
    // --- 3. Аргументы события согласно соглашениям .NET ---
    public class FileArgs : EventArgs
    {
        public string FileName { get; }
        public bool Cancel { get; set; } // 4. Возможность отмены поиска

        public FileArgs(string fileName)
        {
            FileName = fileName;
            Cancel = false;
        }
    }

    // --- 2. Класс для обхода каталога файлов ---
    public class FileSearcher
    {
        // 3. Определение события через стандартный EventHandler
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
                // Используем EnumerateFiles для итеративного обхода (по одному файлу)
                foreach (var file in Directory.EnumerateFiles(directoryPath))
                {
                    var args = new FileArgs(file);

                    // Вызываем событие
                    OnFileFound(args);

                    // 4. Проверка флага отмены
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

        // Защищенный метод для вызова события (стандарт C#)
        protected virtual void OnFileFound(FileArgs e)
        {
            FileFound?.Invoke(this, e);
        }
    }

    // --- 1. Функция расширения для поиска максимума ---
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

    // --- 5. Основной класс программы для демонстрации ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Часть 1: Поиск максимального элемента ===");

            var items = new List<string> { "Кошка", "Александра", "Дом", "Программирование", "C#" };

            // Находим строку с максимальной длиной через лямбда-выражение (делегат)
            var longestWord = items.GetMax(s => s.Length);

            Console.WriteLine($"Коллекция: {string.Join(", ", items)}");
            Console.WriteLine($"Результат (самое длинное слово): {longestWord}");


            Console.WriteLine("\n=== Часть 2: Поиск файлов с событиями ===");

            var searcher = new FileSearcher();
            int fileCount = 0;

            // Подписываемся на событие
            searcher.FileFound += (sender, e) =>
            {
                fileCount++;
                // 5. Вывод сообщения при срабатывании события
                Console.WriteLine($"[{fileCount}] Найден файл: {Path.GetFileName(e.FileName)}");

                // 4. Демонстрация отмены (например, после 5 найденных файлов)
                if (fileCount >= 5)
                {
                    Console.WriteLine("Достигнут лимит в 5 файлов. Посылаем сигнал отмены...");
                    e.Cancel = true;
                }
            };

            // Запускаем поиск (укажите папку, которая точно есть на ПК, например 'C:\\Windows')
            string pathForSearch = @"C:\Windows";
            Console.WriteLine($"Сканируем папку: {pathForSearch}");
            searcher.Search(pathForSearch);

            Console.WriteLine("\nРабота программы завершена. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}