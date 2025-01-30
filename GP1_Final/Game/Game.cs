using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text.Json;

public class Game
{
    private Player player;
    private Map currentMap;
    private Map worldMap;
    private bool isGameRunning;
    private Location? currentLocation;
    private Stack<Map> mapStack;
    private List<string> messageLog;  // To store recent messages
    private const int MAX_MESSAGES = 5;  // Number of messages to show
    private const int WORLD_MAP_WIDTH = 15;  // Increased map size
    private const int WORLD_MAP_HEIGHT = 15; // Increased map size
    private const string SAVE_FILE = "gamesave.json";

    public Game()
    {
        player = new Player();
        worldMap = new Map(WORLD_MAP_WIDTH, WORLD_MAP_HEIGHT); // Using larger dimensions
        currentMap = worldMap;
        isGameRunning = false;
        mapStack = new Stack<Map>();
        currentLocation = null;
        messageLog = new List<string>();
    }

    private void AddMessage(string message)
    {
        messageLog.Add(message);
        if (messageLog.Count > MAX_MESSAGES)
        {
            messageLog.RemoveAt(0);
        }
    }

    private void ClearScreen()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }

    private void RefreshDisplay()
    {
        ClearScreen();
        currentMap.Display(player.Position);
        
        // Display health
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nHealth: {player.Health}/100");
        Console.ResetColor();
        
        // Display controls
        Console.WriteLine("\nControls:");
        Console.WriteLine("Movement: u (Up), d (Down), l (Left), r (Right)");
        Console.WriteLine("Actions: t (Talk), i (Inventory), take (Take Item), use (Use Item)");
        Console.WriteLine("         save (Save Game), q (Save and Quit)");
        if (currentLocation?.HasSubMap == true)
            Console.WriteLine("         enter (Enter Location)");
        if (mapStack.Count > 0)
            Console.WriteLine("         exit (Exit to World Map)");

        // Display current quest
        if (player.CurrentQuest != null)
        {
            Console.WriteLine("\n=== Current Quest ===");
            Console.WriteLine($"Quest: {player.CurrentQuest.Name}");
            Console.WriteLine($"Objective: {player.CurrentQuest.Description}");
        }

        // Display recent messages
        Console.WriteLine("\n=== Recent Events ===");
        foreach (string message in messageLog)
        {
            Console.WriteLine(message);
        }
    }

    public void Start()
    {
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Dragon's Bane: The Quest for the Medallions ===");
            Console.WriteLine("1. New Game");
            Console.WriteLine("2. Load Game");
            Console.WriteLine("3. How to Play");
            Console.WriteLine("4. Credits");
            Console.WriteLine("5. Exit");
            Console.Write("\nSelect an option: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    StartNewGame();
                    break;
                case "2":
                    LoadGame();
                    break;
                case "3":
                    ShowHowToPlay();
                    break;
                case "4":
                    ShowCredits();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private void ShowHowToPlay()
    {
        Console.Clear();
        Console.WriteLine("=== How to Play ===\n");
        
        Console.WriteLine("OBJECTIVE:");
        Console.WriteLine("Collect all three medallions to seal away the dragon and save the land!\n");
        
        Console.WriteLine("CONTROLS:");
        Console.WriteLine("u - Move Up");
        Console.WriteLine("d - Move Down");
        Console.WriteLine("l - Move Left");
        Console.WriteLine("r - Move Right");
        Console.WriteLine("t - Talk to NPCs");
        Console.WriteLine("take - Pick up items");
        Console.WriteLine("use - Use items (e.g., Health Potion)");
        Console.WriteLine("i - View inventory");
        Console.WriteLine("save - Save game");
        Console.WriteLine("q - Quit game\n");
        
        Console.WriteLine("MAP SYMBOLS:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("P");
        Console.ResetColor();
        Console.WriteLine(" - Your character");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("!");
        Console.ResetColor();
        Console.WriteLine(" - Quest locations");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("@");
        Console.ResetColor();
        Console.WriteLine(" - NPCs to talk to");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("*");
        Console.ResetColor();
        Console.WriteLine(" - Items to collect\n");
        
        Console.WriteLine("HOW TO WIN:");
        Console.WriteLine("1. Find the three quest locations marked with '!'");
        Console.WriteLine("2. Talk to the spirits in each location");
        Console.WriteLine("3. Collect all three medallions");
        Console.WriteLine("4. Return with all medallions to win\n");
        
        Console.WriteLine("HOW TO LOSE:");
        Console.WriteLine("1. Your health starts at 100");
        Console.WriteLine("2. Dangerous areas drain 10 health per move");
        Console.WriteLine("3. If your health reaches 0, you lose\n");
        
        Console.WriteLine("TIPS:");
        Console.WriteLine("- Talk to the Village Elder for guidance");
        Console.WriteLine("- Avoid dangerous areas until necessary");
        Console.WriteLine("- Plan your route to save health");
        
        Console.WriteLine("\nPress Enter to return to main menu...");
        Console.ReadLine();
    }

    private void StartNewGame()
    {
        Console.Clear();
        ShowIntroStory();
        InitializeGame();
        GameLoop();
    }

    private void ShowIntroStory()
    {
        Console.Clear();
        TypeWriterEffect("You open your eyes in a dimly lit room...");
        Thread.Sleep(1000);
        
        TypeWriterEffect("\nAn elderly figure stands before you, his eyes filled with wisdom and concern.");
        Thread.Sleep(500);
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        TypeWriterEffect("\n\n\"Ah, you're finally awake, brave soul.\"");
        Thread.Sleep(500);
        
        TypeWriterEffect("\n\"Tell me, what is your name?\"");
        Console.ResetColor();
        
        Console.Write("\n\nEnter your name: ");
        player.Name = Console.ReadLine() ?? "Hero";
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        TypeWriterEffect($"\n\"Welcome, {player.Name}. I am the Village Elder.\"");
        TypeWriterEffect("\n\"Our land is in grave danger. An ancient dragon has awakened from its slumber.\"");
        TypeWriterEffect("\n\"Only by collecting the three sacred medallions can we hope to seal it away.\"");
        TypeWriterEffect("\n\"Here, take this map. It will guide you to the medallions.\"");
        Console.ResetColor();
        
        Console.WriteLine("\n\nPress Enter to begin your quest...");
        Console.ReadLine();
    }

    private void TypeWriterEffect(string text, int delay = 30)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
    }

    private void InitializeGame()
    {
        player = new Player();
        worldMap = new Map(WORLD_MAP_WIDTH, WORLD_MAP_HEIGHT); // Using larger dimensions
        currentMap = worldMap;
        player.Position = new Position(5, 5); // Starting position in the village
        currentLocation = currentMap.GetLocation(player.Position);
        AddMessage("Welcome to Dragon's Bane!");
        AddMessage("Your quest begins in the Village of Hope.");
        
        // Create initial quest
        Quest firstQuest = new Quest(
            "Forest Investigation",
            "Investigate the strange occurrences in the Mysterious Forest",
            new Item("Ancient Amulet", "A powerful magical amulet", 100),
            new Position(0, 1)
        );
        
        player.CurrentQuest = firstQuest;
        isGameRunning = true;  // Add this line to start the game
    }

    private void GameLoop()
    {
        while (isGameRunning)
        {
            RefreshDisplay();
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        Console.Write("\nWhat would you like to do? ");
        string input = Console.ReadLine()?.ToLower() ?? "";

        switch (input)
        {
            case "u":
                MovePlayer(0, -1);
                break;
            case "d":
                MovePlayer(0, 1);
                break;
            case "l":
                MovePlayer(-1, 0);
                break;
            case "r":
                MovePlayer(1, 0);
                break;
            case "t":
                HandleTalk();
                break;
            case "i":
                ShowInventory();
                break;
            case "take":
                HandleTakeItem();
                break;
            case "enter":
                HandleEnterLocation();
                break;
            case "exit":
                HandleExitLocation();
                break;
            case "save":
                SaveGame();
                AddMessage("Game saved successfully!");
                break;
            case "q":
                SaveGame(); // Auto-save when quitting
                isGameRunning = false;
                break;
            case "use":
                HandleUseItem();
                break;
            default:
                AddMessage("Invalid command. Try again.");
                break;
        }

        // Add damage in dangerous locations
        if (currentLocation?.Name.Contains("Mountain Peak") == true ||
            currentLocation?.Name.Contains("Dark Forest") == true)
        {
            player.Health -= 10;
            AddMessage("This dangerous place is draining your health!");
        }
        
        CheckLoseCondition();
        CheckWinCondition();
        RefreshDisplay();
    }

    private void MovePlayer(int dx, int dy)
    {
        Position newPosition = new Position(player.Position.X + dx, player.Position.Y + dy);

        if (IsValidMove(newPosition))
        {
            player.Position = newPosition;
            currentLocation = currentMap.GetLocation(newPosition);
            if (currentLocation != null)
            {
                AddMessage($"Moved to {currentLocation.Name}");
                AddMessage(currentLocation.Description);
                
                // Check if this location completes current quest
                if (player.CurrentQuest != null && 
                    player.CurrentQuest.TargetLocation != null &&
                    player.Position.X == player.CurrentQuest.TargetLocation.Value.X && 
                    player.Position.Y == player.CurrentQuest.TargetLocation.Value.Y)
                {
                    player.CurrentQuest.Complete();
                    player.Inventory.AddItem(player.CurrentQuest.Reward);
                    player.CompletedQuests.Add(player.CurrentQuest);
                    player.CurrentQuest = null;
                    
                    // Could add next quest here
                }
            }
        }
        else
        {
            AddMessage("You cannot move in that direction!");
        }
        RefreshDisplay();
    }

    private bool IsValidMove(Position newPos)
    {
        // Check if the position is within map bounds
        if (newPos.X < 0 || newPos.X >= WORLD_MAP_WIDTH || newPos.Y < 0 || newPos.Y >= WORLD_MAP_HEIGHT)
            return false;

        // Check if there's a location at the new position
        return currentMap.GetLocation(newPos) != null;
    }

    private void ShowInventory()
    {
        ClearScreen();
        Console.WriteLine("=== Inventory ===");
        player.ShowInventory();
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
        RefreshDisplay();
    }

    private void HandleTalk()
    {
        if (currentLocation?.HasNPC() == true)
        {
            ClearScreen();
            currentLocation.InteractWithNPC(player);
            AddMessage("Finished conversation.");
        }
        else
        {
            AddMessage("There's no one here to talk to.");
        }
        RefreshDisplay();
    }

    private void HandleTakeItem()
    {
        if (currentLocation?.HasItems() == true)
        {
            ClearScreen();
            currentLocation.ShowItems();
            Console.Write("\nEnter item number to take (0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0)
            {
                Item? item = currentLocation.TakeItem(choice - 1);
                if (item != null)
                {
                    player.Inventory.AddItem(item);
                    AddMessage($"Took {item.Name}");
                    
                    // Check if this item completes current quest
                    if (player.CurrentQuest != null && 
                        item.Name == player.CurrentQuest.Reward.Name)
                    {
                        player.CurrentQuest.Complete();
                        player.CompletedQuests.Add(player.CurrentQuest);
                        
                        // Check if all quests are completed
                        if (player.CompletedQuests.Count >= 3) // Assuming 3 artifacts needed
                        {
                            EndGame();
                        }
                    }
                }
            }
        }
        else
        {
            AddMessage("There are no items here to take.");
        }
        RefreshDisplay();
    }

    private void HandleEnterLocation()
    {
        if (currentLocation?.HasSubMap == true)
        {
            mapStack.Push(currentMap);
            currentMap = worldMap.CreateVillageMap();
            player.Position = new Position(2, 2);
            AddMessage("Entered the village.");
        }
        else
        {
            AddMessage("There's nothing to enter here.");
        }
        RefreshDisplay();
    }

    private void HandleExitLocation()
    {
        if (mapStack.Count > 0)
        {
            currentMap = mapStack.Pop();
            player.Position = currentMap.ParentMapPosition;
            AddMessage("Returned to the world map.");
        }
        else
        {
            AddMessage("You can't exit from here.");
        }
        RefreshDisplay();
    }

    private void EndGame()
    {
        Console.Clear();
        Console.WriteLine("=== Congratulations! ===");
        Console.WriteLine("You have collected all three artifacts!");
        Console.WriteLine("The dragon has been sealed away, and peace returns to the land.");
        Console.WriteLine("\nThank you for playing!");
        Console.WriteLine("\nPress Enter to exit...");
        Console.ReadLine();
        isGameRunning = false;
    }

    private void ShowCredits()
    {
        Console.Clear();
        Console.WriteLine("=== Credits ===");
        Console.Write("Created by ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Muhammed Usame Toktas");
        Console.ResetColor();
        Console.WriteLine("\nPress Enter to return to main menu...");
        Console.ReadLine();
    }

    private void CheckWinCondition()
    {
        // Check if player has all three medallions
        bool hasForestMedallion = player.Inventory.HasItem("Forest Medallion");
        bool hasWaterMedallion = player.Inventory.HasItem("Water Medallion");
        bool hasMountainMedallion = player.Inventory.HasItem("Mountain Medallion");

        if (hasForestMedallion && hasWaterMedallion && hasMountainMedallion)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== Congratulations! ===");
            Console.WriteLine("You have collected all three medallions!");
            Console.WriteLine("The dragon's power is sealed, and the land is saved!");
            Console.WriteLine("You have won the game!");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
            isGameRunning = false;
        }
    }

    private void CheckLoseCondition()
    {
        if (player.Health <= 0)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n=== Game Over ===");
            Console.WriteLine("Your health has reached zero!");
            Console.WriteLine("The dragon's darkness continues to spread across the land...");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
            isGameRunning = false;
        }
    }

    private void SaveGame()
    {
        var saveData = new SaveData
        {
            PlayerPosition = player.Position,
            PlayerHealth = player.Health,
            PlayerName = player.Name,
            Inventory = player.Inventory.GetItems().Select(item => item.Name).ToList(),
            CompletedQuests = player.CompletedQuests.Select(quest => quest.Name).ToList()
        };

        try
        {
            string jsonString = JsonSerializer.Serialize(saveData);
            File.WriteAllText(SAVE_FILE, jsonString);
            AddMessage("Game saved successfully!");
        }
        catch (Exception e)
        {
            AddMessage("Failed to save game: " + e.Message);
        }
    }

    private void LoadGame()
    {
        if (!File.Exists(SAVE_FILE))
        {
            Console.WriteLine("No saved game found!");
            Console.WriteLine("Press Enter to return to menu...");
            Console.ReadLine();
            return;
        }

        try
        {
            string jsonString = File.ReadAllText(SAVE_FILE);
            var saveData = JsonSerializer.Deserialize<SaveData>(jsonString);

            if (saveData != null)
            {
                // Initialize game with saved data
                player = new Player
                {
                    Position = saveData.PlayerPosition,
                    Health = saveData.PlayerHealth,
                    Name = saveData.PlayerName
                };

                // Restore inventory
                foreach (string itemName in saveData.Inventory)
                {
                    player.Inventory.AddItem(new Item(itemName, "", 0));
                }

                // Restore completed quests
                foreach (string questName in saveData.CompletedQuests)
                {
                    player.CompletedQuests.Add(new Quest(questName, ""));
                }

                worldMap = new Map(WORLD_MAP_WIDTH, WORLD_MAP_HEIGHT);
                currentMap = worldMap;
                currentLocation = currentMap.GetLocation(player.Position);
                isGameRunning = true;
                GameLoop();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to load game: " + e.Message);
            Console.WriteLine("Press Enter to return to menu...");
            Console.ReadLine();
        }
    }

    private void HandleUseItem()
    {
        var items = player.Inventory.GetItems();
        if (items.Count == 0)
        {
            AddMessage("You have no items to use!");
            return;
        }

        Console.Clear();
        Console.WriteLine("=== Your Items ===");
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i].Name}");
        }

        Console.Write("\nEnter item number to use (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= items.Count)
        {
            string itemName = items[choice - 1].Name;
            player.UseItem(itemName);
            AddMessage($"Used {itemName}");
        }
        RefreshDisplay();
    }
} 