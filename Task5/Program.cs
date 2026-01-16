using System;

namespace Task5
{
    public interface IMyCloneable<T>
    {
        T Clone();
    }

    public class Device : IMyCloneable<Device>, ICloneable
    {
        public string Brand { get; set; }

        public Device(string brand) => Brand = brand;

        public virtual Device Clone() => new Device(Brand);

        object ICloneable.Clone() => Clone();

        public override string ToString() => $"Device: {Brand}";
    }

    public class Computer : Device, IMyCloneable<Computer>, ICloneable
    {
        public int RamSize { get; set; }

        public Computer(string brand, int ramSize) : base(brand)
        {
            RamSize = ramSize;
        }

        public override Computer Clone() => new Computer(Brand, RamSize);

        object ICloneable.Clone() => Clone();

        public override string ToString() => $"Computer: {Brand}, RAM: {RamSize}GB";
    }

    public class Laptop : Computer, IMyCloneable<Laptop>, ICloneable
    {
        public double Weight { get; set; }

        public Laptop(string brand, int ramSize, double weight) : base(brand, ramSize)
        {
            Weight = weight;
        }

        public override Laptop Clone() => new Laptop(Brand, RamSize, Weight);

        object ICloneable.Clone() => Clone();

        public override string ToString() => $"Laptop: {Brand}, RAM: {RamSize}GB, Weight: {Weight}kg";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Тестирование паттерна Прототип ===\n");

            Laptop originalLaptop = new Laptop("ASUS", 16, 1.5);
            Laptop clonedLaptop = originalLaptop.Clone();

            Console.WriteLine($"Оригинал: {originalLaptop}");
            Console.WriteLine($"Клон:     {clonedLaptop}");

            clonedLaptop.Brand = "Apple";
            Console.WriteLine("\nИзменили бренд у клона на Apple...");
            Console.WriteLine($"Оригинал после изменения: {originalLaptop.Brand}");
            Console.WriteLine($"Клон после изменения:    {clonedLaptop.Brand}");

            PrintComparison();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void PrintComparison()
        {
            Console.WriteLine("\n=== Сравнение интерфейсов ===");
            Console.WriteLine("1. IMyCloneable<T> (Generic):");
            Console.WriteLine("   + Преимущество: Типобезопасность. Не нужно делать приведение (Laptop l = original.Clone()).");
            Console.WriteLine("   - Недостаток: Кастомный интерфейс, не поддерживается стандартными коллекциями .NET.");

            Console.WriteLine("\n2. ICloneable:");
            Console.WriteLine("   + Преимущество: Стандарт платформы .NET.");
            Console.WriteLine("   - Недостаток: Возвращает 'object'. Нужно писать ((Laptop)original.Clone()), что может вызвать ошибку.");
        }
    }
}