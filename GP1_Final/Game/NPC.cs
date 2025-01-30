using System;
using System.Collections.Generic;

public class NPC
{
    public string Name { get; private set; }
    public string[] Dialogues { get; private set; }
    private List<Dialogue> dialogues;
    private int currentDialogueIndex;
    public bool HasQuest { get; private set; }
    public Quest? ActiveQuest { get; private set; }

    public NPC(string name)
    {
        Name = name;
        Dialogues = new string[] { "Hello traveler!" };  // Default dialogue
        dialogues = new List<Dialogue>();
        currentDialogueIndex = 0;
        InitializeDialogues();
    }

    public NPC(string name, string[] dialogueStrings)
    {
        Name = name;
        Dialogues = dialogueStrings;
        dialogues = new List<Dialogue>();  // Using the field, not creating a new variable
        currentDialogueIndex = 0;
        InitializeDialogues();
    }

    private void InitializeDialogues()
    {
        switch (Name)
        {
            case "Village Elder":
                CreateElderDialogues();
                break;
            case "Forest Spirit":
                CreateForestSpiritDialogues();
                break;
            default:
                CreateDefaultDialogues();
                break;
        }
    }

    private void CreateForestSpiritDialogues()
    {
        var dialogue = new Dialogue("The forest whispers ancient secrets...");
        dialogue.AddOption(
            "Tell me about the Forest Medallion.",
            "It is hidden deep within the Sacred Grove, protected by ancient magic.",
            null
        );
        dialogue.AddOption(
            "How can I prove myself worthy?",
            "Help restore balance to the forest, and the medallion shall be yours.",
            null
        );
        dialogues.Add(dialogue);
    }

    private void CreateDefaultDialogues()
    {
        var dialogue = new Dialogue($"Greetings, traveler. I am {Name}.");
        dialogue.AddOption(
            "What brings you here?",
            "I'm just passing through these lands.",
            null
        );
        dialogues.Add(dialogue);
    }

    private void CreateElderDialogues()
    {
        var introDialogue = new Dialogue("Welcome to our village, brave soul.");
        introDialogue.AddOption(
            "Tell me about the dragon.",
            "An ancient evil has awakened. The dragon threatens to destroy everything we hold dear.",
            null
        );
        introDialogue.AddOption(
            "How can I help?",
            "You must collect the three medallions. Only then will you have the power to face the dragon.",
            () => {
                HasQuest = true;
                ActiveQuest = new Quest(
                    "The Dragon's Threat", 
                    "Collect all three medallions to seal away the dragon."
                );
            }
        );
        dialogues.Add(introDialogue);
    }

    public void Talk(Player player)
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            var dialogue = dialogues[currentDialogueIndex];
            bool conversationActive = true;

            while (conversationActive)
            {
                dialogue.Display();
                
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= dialogue.Options.Count)
                {
                    Console.Clear();
                    if (choice == 0)
                    {
                        conversationActive = false;
                    }
                    else
                    {
                        var option = dialogue.Options[choice - 1];
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n{Name}: {option.Response}");
                        Console.ResetColor();
                        option.Effect?.Invoke();
                        
                        if (HasQuest && ActiveQuest != null && !player.HasQuest)
                        {
                            player.CurrentQuest = ActiveQuest;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\nNew Quest: {ActiveQuest.Name}");
                            Console.WriteLine(ActiveQuest.Description);
                            Console.ResetColor();
                        }
                        
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                    }
                }
            }
        }
    }
} 