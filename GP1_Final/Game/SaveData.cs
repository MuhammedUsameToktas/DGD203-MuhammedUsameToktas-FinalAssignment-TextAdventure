using System;
using System.IO;
using System.Text.Json;

public class SaveData
{
    public Position PlayerPosition { get; set; }
    public int PlayerHealth { get; set; }
    public string PlayerName { get; set; }
    public List<string> Inventory { get; set; }
    public List<string> CompletedQuests { get; set; }

    public SaveData()
    {
        PlayerPosition = new Position(0, 0);
        PlayerHealth = 100;
        PlayerName = "Hero";
        Inventory = new List<string>();
        CompletedQuests = new List<string>();
    }
} 