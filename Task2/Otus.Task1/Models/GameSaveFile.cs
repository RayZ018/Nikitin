using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

[Serializable]
[XmlRoot("u")]
public class SaveData
{
    public DateTime CreatedDate { get; set; }
    public DateTime? SaveDate { get; set; }
    public string FileName { get; set; }
    public string CurrentLocation { get; set; }
    public User User { get; set; }
    public List<Item> Items { get; set; }
}

[Serializable]
public class User
{
    [XmlAttribute("level")]
    public int Level { get; set; }
    public string Name { get; set; }

    public string Gender { get; set; }

    [JsonIgnore]
    public string Coords { get; set; }
}

[Serializable]
public class Item
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}