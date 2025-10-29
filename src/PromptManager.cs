namespace Azure.AI.VoiceLive.Samples;

public static class PromptManager
{
    public static IReadOnlyList<PromptFileDefinition> PromptFiles = [
            new ("Very Basic LLM Agent", "instructions-basic.txt"),
            new ("Zara - a friendly agent to chat with", "instructions-zara.txt"),
            new ("Santa's Hotline", "instructions-santa.txt")
        ];
    //private static readonly List<PromptFileDefinition> _promptFiles = [
    //        new("Very Basic LLM Agent", "instructions-basic.txt"),
    //        new("Zara - a friendly agent to chat with", "instructions-zara.txt"),
    //        new("Santa's Hotline", "instructions-santa.txt"),
    //    ];

    //static PromptManager()
    //{
    //    InitializePromptFileList();
    //}

    // public static IReadOnlyList<PromptFileDefinition> PromptFiles => _promptFiles;

    //public static void InitializePromptFileList()
    //{
    //    _promptFiles.Clear();
    //    _promptFiles.AddRange(
    //    [
    //        new("Very Basic LLM Agent", "instructions-basic.txt"),
    //        new("Zara - a friendly agent to chat with", "instructions-zara.txt"),
    //        new("Santa's Hotline", "instructions-santa.txt"),
    //    ]);
    //}

    public static (string, string) SelectAnAgent()
    {
        var promptNames = from prompt in PromptFiles select prompt.Name;
        var selectedAgent = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]What agent would you like to talk to?[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to select an agent)[/]")
                .AddChoices(promptNames));
        var instructionsFile = GetInstructionFileName(selectedAgent);
        return (selectedAgent, instructionsFile);
    }

    public static string GetInstructionFileName(string promptName)
    {
        return PromptFiles.FirstOrDefault(p => p.Name == promptName)?.InstructionFileName ?? "instructions.txt";
    }
}

public class PromptFileDefinition
{
    public string Name { get; set; }
    public string InstructionFileName { get; set; }
    public PromptFileDefinition()
    {
        Name = string.Empty;
        InstructionFileName = string.Empty;
    }
    public PromptFileDefinition(string instructionFileName)
    {
        Name = instructionFileName;
        InstructionFileName = instructionFileName;
    }
    public PromptFileDefinition(string name, string instructionFileName)
    {
        Name = name;
        InstructionFileName = instructionFileName;
    }
}
