public class DialogueOption
{
    public string Text { get; set; }
    public string Response { get; set; }
    public Action Effect { get; set; }

    public DialogueOption(string text, string response, Action effect = null)
    {
        Text = text;
        Response = response;
        Effect = effect;
    }
}

public class Dialogue
{
    public List<DialogueOption> Options { get; private set; }
    public string Introduction { get; private set; }

    public Dialogue(string introduction)
    {
        Introduction = introduction;
        Options = new List<DialogueOption>();
    }

    public void AddOption(string text, string response, Action effect = null)
    {
        Options.Add(new DialogueOption(text, response, effect));
    }

    public void Display()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(Introduction);
        Console.ResetColor();
        Console.WriteLine();

        for (int i = 0; i < Options.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Options[i].Text}");
        }
        Console.WriteLine("0. End conversation");
    }
} 