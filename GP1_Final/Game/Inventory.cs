using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    private List<Item> items;
    private const int MaxItems = 10;

    public Inventory()
    {
        items = new List<Item>();
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public bool AddItem(Item item)
    {
        if (items.Count >= MaxItems)
        {
            Console.WriteLine("Inventory is full!");
            return false;
        }

        items.Add(item);
        return true;
    }

    public void RemoveItem(string itemName)
    {
        var item = items.FirstOrDefault(i => i.Name == itemName);
        if (item != null)
        {
            items.Remove(item);
        }
    }

    public void DisplayItems()
    {
        if (items.Count == 0)
        {
            Console.WriteLine("Inventory is empty.");
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i].Name} - {items[i].Description}");
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Any(item => item.Name == itemName);
    }
} 