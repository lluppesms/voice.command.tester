///// <summary>
///// FILE: Program.cs
///// </summary>
///// <remarks>
///// DESCRIPTION:
/////     This sample demonstrates the fundamental capabilities of the VoiceLive SDK by creating
/////     a basic voice assistant that can engage in natural conversation with proper interruption
/////     handling. This serves as the foundational example that showcases the core value
/////     proposition of unified speech-to-speech interaction.
///// USAGE:
/////     update appsettings.json with your values.
/////     dotnet run
///// REQUIREMENTS:
/////     - Azure.AI.VoiceLive
/////     - Azure.Identity
/////     - NAudio (for audio capture and playback)
/////     - Microsoft.Extensions.Configuration
/////     - System.CommandLine
///// </remarks>

public class Program
{
    public static async Task Main(string[] args)
    {
        Utilities.WelcomeMessage("Live Voice Command Center", "Welcome to the LiveVoice API Demo");

        (var agentName, var instructionsFile) = PromptManager.SelectAnAgent();

        AnsiConsole.MarkupLine("\n" + Emoji.Known.Construction + $"  [cyan]Initializing Voice Service Connection to Agent {agentName}...[/]");
        await VoiceAssistantSetup.StartVoiceAssistant(instructionsFile);

        AnsiConsole.MarkupLine("\n[cyan]Press any key to exit...[/]");
        Console.ReadKey();

    }
}