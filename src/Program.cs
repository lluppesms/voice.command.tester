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
        Utilities.WelcomeMessage("Voice Live Command Line", "Welcome to the VoiceLive API Demo");

        var scenarioName = PromptManager.SelectAScenario();

        await VoiceAssistantSetup.StartVoiceAssistant(scenarioName);

        //AnsiConsole.MarkupLine("\n[cyan]Press any key to exit...[/]");
        //Console.ReadKey();
    }
}