public class Quest
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Item? Reward { get; private set; }
    public Position? TargetLocation { get; private set; }
    public bool IsCompleted { get; private set; }

    public Quest(string name, string description, Item? reward = null, Position? targetLocation = null)
    {
        Name = name;
        Description = description;
        Reward = reward;
        TargetLocation = targetLocation;
        IsCompleted = false;
    }

    public void Complete()
    {
        IsCompleted = true;
        Console.WriteLine($"\nQuest Completed: {Name}");
        Console.WriteLine($"Reward: {Reward?.Name}");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
} 