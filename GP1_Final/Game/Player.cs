using System.Collections.Generic;
using System;

public class Player
{
    public Position Position { get; set; }
    public string Name { get; set; } = "Hero";
    public int Health { get; set; }
    public Inventory Inventory { get; private set; }
    public Quest? CurrentQuest { get; set; }
    public List<Quest> CompletedQuests { get; private set; }
    public bool HasQuest => CurrentQuest != null;

    public Player()
    {
        Position = new Position(0, 0);
        Health = 100;
        Inventory = new Inventory();
        CompletedQuests = new List<Quest>();
    }

    public void ShowInventory()
    {
        Console.Clear();
        Console.WriteLine("=== Inventory ===");
        Inventory.DisplayItems();
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public void UseItem(string itemName)
    {
        if (itemName == "Health Potion")
        {
            Health = Math.Min(100, Health + 50); // Restore 50 health, max 100
            Inventory.RemoveItem("Health Potion");
        }
    }
} 