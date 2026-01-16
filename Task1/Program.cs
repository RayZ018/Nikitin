using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ReflectionHomework
{
    public class F
    {
        int i1, i2, i3, i4, i5;
        public int[] mas;
        public F()
        {
            i1 = 1; i2 = 2; i3 = 3; i4 = 4; i5 = 5;
            mas = new int[] { 1, 2 };
        }
    }

    public static class CsvSerializer
    {
        public static string Serialize<T>(T obj)
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var sb = new StringBuilder();

            foreach (var field in fields)
            {
                var value = field.GetValue(obj);

                if (field.FieldType.IsArray)
                {
                    var array = value as Array;
                    sb.Append("[");
                    if (array != null)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            sb.Append(array.GetValue(i)).Append(i < array.Length - 1 ? "|" : "");
                        }
                    }
                    sb.Append("],");
                }
                else
                {
                    sb.Append(value).Append(",");
                }
            }

            return sb.ToString().TrimEnd(',');
        }

        public static T Deserialize<T>(string csv) where T : new()
        {
            var obj = new T();
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var values = csv.Split(',');

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var rawValue = values[i];

                if (field.FieldType.IsArray)
                {
                    var elements = rawValue.Trim('[', ']').Split('|', StringSplitOptions.RemoveEmptyEntries);
                    var elementType = field.FieldType.GetElementType();
                    var newArray = Array.CreateInstance(elementType, elements.Length);

                    for (int j = 0; j < elements.Length; j++)
                    {
                        newArray.SetValue(Convert.ChangeType(elements[j], elementType), j);
                    }
                    field.SetValue(obj, newArray);
                }
                else
                {
                    field.SetValue(obj, Convert.ChangeType(rawValue, field.FieldType));
                }
            }
            return obj;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const int iterations = 100000;
            var testObj = new F();
            string csvResult = "";

            Console.WriteLine($"Количество итераций: {iterations}\n");

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                csvResult = CsvSerializer.Serialize(testObj);
            }
            sw.Stop();
            long mySerializeTime = sw.ElapsedMilliseconds;

            var swConsole = Stopwatch.StartNew();
            Console.WriteLine("Результат CSV:");
            Console.WriteLine(csvResult);
            swConsole.Stop();

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var deserialized = CsvSerializer.Deserialize<F>(csvResult);
            }
            sw.Stop();
            long myDeserializeTime = sw.ElapsedMilliseconds;

            sw.Restart();
            string jsonResult = "";
            for (int i = 0; i < iterations; i++)
            {
                jsonResult = JsonSerializer.Serialize(testObj, new JsonSerializerOptions { IncludeFields = true });
            }
            sw.Stop();
            long jsonSerializeTime = sw.ElapsedMilliseconds;

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var jsonDeserialized = JsonSerializer.Deserialize<F>(jsonResult, new JsonSerializerOptions { IncludeFields = true });
            }
            sw.Stop();
            long jsonDeserializeTime = sw.ElapsedMilliseconds;

            Console.WriteLine("\n--- Отчет о производительности ---");
            Console.WriteLine($"Время на вывод в консоль: {swConsole.ElapsedMilliseconds} мс");
            Console.WriteLine($"\n[Мой Рефлекшен (CSV)]:");
            Console.WriteLine($"Сериализация: {mySerializeTime} мс");
            Console.WriteLine($"Десериализация: {myDeserializeTime} мс");
            Console.WriteLine($"\n[Standard (System.Text.Json)]:");
            Console.WriteLine($"Сериализация: {jsonSerializeTime} мс");
            Console.WriteLine($"Десериализация: {jsonDeserializeTime} мс");
        }
    }
}