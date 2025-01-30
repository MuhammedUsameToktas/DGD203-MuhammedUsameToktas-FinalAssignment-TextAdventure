using System;

public class Map
{
    private Location[,] locations;
    private int width;
    private int height;
    private Random random;
    public bool IsSubMap { get; private set; }
    public Position ParentMapPosition { get; private set; }

    public Map(int width, int height, bool isSubMap = false, Position parentMapPosition = new Position())
    {
        this.width = width;
        this.height = height;
        this.IsSubMap = isSubMap;
        this.ParentMapPosition = parentMapPosition;
        locations = new Location[width, height];
        random = new Random();
        InitializeMap();
    }

    private void InitializeMap()
    {
        if (!IsSubMap)
        {
            InitializeMainMap();
        }
    }

    private void InitializeMainMap()
    {
        // Clear the map first
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                locations[x, y] = null;

        // Create paths first to ensure connectivity
        CreatePaths();

        // Place the village in the center
        int centerX = width / 2;
        int centerY = height / 2;
        locations[centerX, centerY] = new Location("Village of Hope", 
            "The last bastion of hope in these troubled times. The village elder awaits your arrival.", true);

        // Create quest-critical locations spread far apart
        // Forest Medallion (far east)
        locations[width - 2, centerY] = new Location("Sacred Grove", 
            "An ancient grove humming with magical energy. The Forest Medallion must be nearby.");
        
        // Water Medallion (far north)
        locations[centerX, 1] = new Location("Water Spirit's Shrine", 
            "An ancient shrine dedicated to the water spirit. The Water Medallion's power resonates here.");
        
        // Mountain Medallion (far west)
        locations[1, centerY] = new Location("Mountain Peak", 
            "The frigid peak where the Mountain Medallion is said to be hidden.");

        // Place medallions and NPCs in quest locations
        locations[width - 2, centerY]?.AddItem(new Item("Forest Medallion", 
            "An ancient medallion pulsing with nature's power.", 100));
        locations[width - 2, centerY]?.AddNPC(new NPC("Forest Spirit"));
        
        locations[centerX, 1]?.AddItem(new Item("Water Medallion", 
            "A medallion that flows with water's essence.", 100));
        locations[centerX, 1]?.AddNPC(new NPC("Water Spirit"));
        
        locations[1, centerY]?.AddItem(new Item("Mountain Medallion", 
            "A medallion carved from ancient stone.", 100));
        locations[1, centerY]?.AddNPC(new NPC("Mountain Hermit"));

        // Add random locations around the paths
        string[] locationNames = {
            "Mystic Grove", "Ancient Ruins", "Crystal Cave", "Hidden Valley",
            "Enchanted Clearing", "Abandoned Temple", "Misty Lake", "Stone Circle",
            "Forgotten Shrine", "Dark Forest", "Mountain Pass", "Sacred Spring"
        };

        string[] locationDescs = {
            "A mystical place filled with strange energies.",
            "Ruins of an ancient civilization.",
            "A cave glittering with mysterious crystals.",
            "A peaceful valley hidden from the world.",
            "A clearing where magic flows freely.",
            "An old temple reclaimed by nature.",
            "A lake shrouded in eternal mist.",
            "Ancient stones arranged in a perfect circle.",
            "A shrine dedicated to forgotten gods.",
            "A dense, dark forest with ancient trees.",
            "A treacherous path through the mountains.",
            "A spring with healing properties."
        };

        // Place random locations
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (locations[x, y] == null && random.Next(100) < 70) // 70% chance of location
                {
                    int index = random.Next(locationNames.Length);
                    locations[x, y] = new Location(locationNames[index], locationDescs[index]);
                }
            }
        }

        // Add NPCs
        string[] npcNames = {
            "Wandering Merchant",
            "Lost Traveler",
            "Mysterious Stranger",
            "Forest Spirit",
            "Mountain Hermit",
            "Ancient Guardian",
            "Village Elder"
        };

        // Add NPCs and items to random valid locations
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (locations[x, y] != null && (x != centerX || y != centerY))
                {
                    // 30% chance for NPC
                    if (random.Next(100) < 30)
                    {
                        int npcIndex = random.Next(npcNames.Length);
                        locations[x, y].AddNPC(new NPC(npcNames[npcIndex]));
                    }

                    // 20% chance for item
                    if (random.Next(100) < 20)
                    {
                        locations[x, y].AddItem(CreateRandomItem());
                    }
                }
            }
        }

        // Always add the Village Elder to the starting village
        locations[centerX, centerY].AddNPC(new NPC("Village Elder"));
    }

    private void CreatePaths()
    {
        int centerX = width / 2;
        int centerY = height / 2;

        // Create paths in four directions from center
        for (int x = 0; x < width; x++)
        {
            locations[x, centerY] = new Location("Path", "A well-traveled path through the land.");
        }
        for (int y = 0; y < height; y++)
        {
            locations[centerX, y] = new Location("Path", "A well-traveled path through the land.");
        }

        // Create some diagonal and connecting paths
        for (int i = 0; i < 5; i++)
        {
            int x1 = random.Next(width);
            int y1 = random.Next(height);
            int x2 = random.Next(width);
            int y2 = random.Next(height);
            CreatePathBetween(x1, y1, x2, y2);
        }
    }

    private void CreatePathBetween(int x1, int y1, int x2, int y2)
    {
        int x = x1;
        int y = y1;
        while (x != x2 || y != y2)
        {
            if (locations[x, y] == null)
            {
                locations[x, y] = new Location("Path", "A well-traveled path through the land.");
            }
            if (x < x2) x++;
            if (x > x2) x--;
            if (y < y2) y++;
            if (y > y2) y--;
        }
    }

    private Item CreateRandomItem()
    {
        string[] itemNames = {
            "Health Potion", "Magic Scroll", "Ancient Coin", "Mysterious Gem",
            "Enchanted Ring", "Rusty Key", "Sacred Relic", "Crystal Shard"
        };
        string[] itemDescs = {
            "Restores health when consumed",
            "Contains ancient magical knowledge",
            "Currency from a lost civilization",
            "Glows with mysterious energy",
            "A ring with strange markings",
            "Might unlock something important",
            "A powerful artifact of the old world",
            "Fragment of a larger crystal"
        };
        int[] itemValues = {50, 75, 100, 150, 200, 125, 300, 175};

        int index = random.Next(itemNames.Length);
        return new Item(itemNames[index], itemDescs[index], itemValues[index]);
    }

    public void Display(Position playerPos)
    {
        Console.Clear();
        Console.WriteLine("\n=== " + (IsSubMap ? "Village Map" : "World Map") + " ===");
        Console.WriteLine("+--------------------+");
        
        for (int y = 0; y < height; y++)
        {
            Console.Write("| ");
            for (int x = 0; x < width; x++)
            {
                if (x == playerPos.X && y == playerPos.Y)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("P ");
                    Console.ResetColor();
                }
                else if (locations[x, y] != null)
                {
                    // Story-critical locations
                    if (locations[x, y].Name == "Village of Hope")
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("H ");
                    }
                    else if (locations[x, y].Name.Contains("Medallion") || 
                             locations[x, y].Name.Contains("Sacred Grove") ||
                             locations[x, y].Name.Contains("Water Spirit's Shrine") ||
                             locations[x, y].Name.Contains("Mountain Peak"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("! ");
                    }
                    else if (locations[x, y].HasNPC())
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("@ ");
                    }
                    else if (locations[x, y].HasItems())
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("* ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("# ");
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(". ");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("|");
        }
        
        Console.WriteLine("+--------------------+");
        
        // Updated legend with new colors
        Console.WriteLine("\nLegend:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("P");
        Console.ResetColor();
        Console.Write(" - You    ");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("@");
        Console.ResetColor();
        Console.Write(" - NPC    ");
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("*");
        Console.ResetColor();
        Console.Write(" - Item    ");
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("H");
        Console.ResetColor();
        Console.Write(" - Village    ");
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("!");
        Console.ResetColor();
        Console.Write(" - Quest Location    ");
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("#");
        Console.ResetColor();
        Console.Write(" - Location    ");
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(".");
        Console.ResetColor();
        Console.WriteLine(" - Empty");
        
        // Display current location info
        if (locations[playerPos.X, playerPos.Y] != null)
        {
            Location currentLoc = locations[playerPos.X, playerPos.Y];
            Console.WriteLine($"\nCurrent Location: {currentLoc.Name}");
            Console.WriteLine($"Description: {currentLoc.Description}");
            
            if (currentLoc.HasNPC())
                Console.WriteLine("There's someone here you can talk to (press 't')");
            if (currentLoc.HasItems())
                Console.WriteLine("There are items here you can take (press 'take')");
        }
    }

    public Map CreateVillageMap()
    {
        Map villageMap = new Map(5, 5, true, new Position(0, 0));
        
        // Village Locations
        villageMap.locations[2, 2] = new Location("Village Square", 
            "The heart of the village, where people gather.");
        
        villageMap.locations[1, 2] = new Location("Blacksmith", 
            "The sound of hammering fills the air.");
        
        villageMap.locations[3, 2] = new Location("Healer's Hut", 
            "A small hut filled with herbs and potions.");

        // Add NPCs with single-argument constructor
        villageMap.locations[1, 2].AddNPC(new NPC("Master Smith"));
        villageMap.locations[3, 2].AddNPC(new NPC("Wise Healer"));
        
        // Add Items
        villageMap.locations[2, 1].AddItem(new Item("Health Potion", 
            "Restores your health when consumed.", 30));
        
        return villageMap;
    }

    public string GetBoundaryMessage(Position pos)
    {
        string message = "You cannot go that way: ";
        
        if (pos.X < 0)
            return message + "You've reached the western boundary of the known world.";
        if (pos.X >= width)
            return message + "You've reached the eastern boundary of the known world.";
        if (pos.Y < 0)
            return message + "You've reached the northern boundary of the known world.";
        if (pos.Y >= height)
            return message + "You've reached the southern boundary of the known world.";
        
        return message + "This path is blocked.";
    }

    public bool IsValidPosition(Position pos)
    {
        return pos.X >= 0 && pos.X < width && 
               pos.Y >= 0 && pos.Y < height;
    }

    public Location GetLocation(Position pos)
    {
        if (IsValidPosition(pos))
            return locations[pos.X, pos.Y];
        return null;
    }
} 