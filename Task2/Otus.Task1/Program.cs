using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


var save = new SaveData
{
    CreatedDate = DateTime.Now,
    FileName = "FinalSave",
    CurrentLocation = "Dungeon",
    User = new User
    {
        Name = "Пушкин",
        Level = 10,
        Gender = "m",
        Coords = "15.0, 30.5"
    },
    Items = new List<Item>
    {
        new Item { Name = "Топор", Quantity = 1 },
        new Item { Name = "Зелье", Quantity = 5 }
    }
};

foreach (var item in save.Items)
{
    if (item.Quantity < 0)
    {
        throw new Exception($"Критическая ошибка: Предмет '{item.Name}' не может иметь количество {item.Quantity}!");
    }
}

var jsonOptions = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    WriteIndented = true
};
File.WriteAllText("save.json", JsonSerializer.Serialize(save, jsonOptions));
Console.WriteLine("JSON создан.");

var xmlSerializer = new XmlSerializer(typeof(SaveData));
using (var writer = new StreamWriter("save.xml"))
{
    xmlSerializer.Serialize(writer, save);
}
Console.WriteLine("XML создан.");

#pragma warning disable SYSLIB0011
BinaryFormatter bf = new BinaryFormatter();
using (FileStream fs = new FileStream("save.bin", FileMode.Create))
{
    bf.Serialize(fs, save);
}
Console.WriteLine("Binary создан.");