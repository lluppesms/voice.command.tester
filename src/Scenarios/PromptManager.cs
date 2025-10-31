using Azure.AI.VoiceLive.Samples.Helpers;

namespace Azure.AI.VoiceLive.Samples.Prompts;

public static class PromptManager
{
    public static IReadOnlyList<ScenarioDefinition> ScenarioList { get; private set; } = new List<ScenarioDefinition>();

    static PromptManager()
    {
        InitializeScenarioFileList();
    }

    public static void InitializeScenarioFileList()
    {
        var scenarioData = Utilities.ReadResourceFile("Scenarios.json");
        ScenarioList = !string.IsNullOrEmpty(scenarioData) ? JsonSerializer.Deserialize<List<ScenarioDefinition>>(scenarioData) : new List<ScenarioDefinition>();
    }

    public static string SelectAScenario()
    {
        var promptNames = from scenario in ScenarioList select scenario.Name;
        var selectedScenarioName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]What agent would you like to talk to?[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to select an agent)[/]")
                .AddChoices(promptNames));
        return selectedScenarioName; // , instructionsFile);
    }

    public static string GetPromptFileName(string scenarioName)
    {
        return ScenarioList.FirstOrDefault(p => p.Name == scenarioName)?.PromptFileName ?? "instructions.txt";
    }
    public static ScenarioDefinition GetScenario(string scenarioName)
    {
        return ScenarioList.FirstOrDefault(p => p.Name == scenarioName)?? new ScenarioDefinition();
    }
}

public class ScenarioDefinition
{
    /// <summary>
    /// Name of the scenario that the user picks from a list
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// If you want to talk directly to an LLM, provide a model name and a prompt file
    /// </summary>
    public string ModelName { get; set; }
    /// <summary>
    /// If you want to talk directly to an LLM, provide a model name and a prompt file
    /// </summary>
    public string PromptFileName { get; set; }

    /// <summary>
    /// If you want to talk to an agent, provide an agent name and a AI Foundry project name
    /// </summary>
    public string AgentName { get; set; }
    /// <summary>
    /// If you want to talk to an agent, provide an agent name and a AI Foundry project name
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// With either method, you can optionally specify a voice name.  If not specified, the default voice from the app settings will be used.
    /// </summary>
    public string VoiceName { get; set; }

    public ScenarioDefinition()
    {
        Name = string.Empty;
        PromptFileName = string.Empty;
        ModelName = string.Empty;
        AgentName = string.Empty;
        ProjectName = string.Empty;
        VoiceName = string.Empty;
    }
}
