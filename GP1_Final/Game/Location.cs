using System;
using System.Collections.Generic;

public class Location
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool HasSubMap { get; private set; }
    private List<NPC> npcs;
    private List<Item> items;

    public Location(string name, string description, bool hasSubMap = false)
    {
        Name = name;
        Description = description;
        HasSubMap = hasSubMap;
        npcs = new List<NPC>();
        items = new List<Item>();
    }

    public bool HasNPC() => npcs.Count > 0;
    public bool HasItems() => items.Count > 0;

    public void AddItem(Item item) => items.Add(item);
    
    public void RemoveItem(Item item) => items.Remove(item);

    public void ShowItems()
    {
        if (items.Count == 0)
        {
            Console.WriteLine("No items here.");
            return;
        }

        Console.WriteLine("\nAvailable Items:");
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i].Name} - {items[i].Description}");
        }
    }

    public Item TakeItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            Item item = items[index];
            items.RemoveAt(index);
            return item;
        }
        return null;
    }

    public void Enter()
    {
        Console.Clear();
        Console.WriteLine($"=== {Name} ===");
        Console.WriteLine(Description);
        
        if (npcs.Count > 0)
            Console.WriteLine("\nThere are people here you can talk to.");
        
        if (items.Count > 0)
            Console.WriteLine("\nYou see some items on the ground.");

        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    public void InteractWithNPC(Player player)
    {
        if (npcs.Count > 0)
        {
            npcs[0].Talk(player);
        }
    }

    public void AddNPC(NPC npc)
    {
        npcs.Add(npc);
    }

    public void AddQuest(Quest quest)
    {
        // Add quest to location
    }
} 