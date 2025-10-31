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

using Azure.AI.VoiceLive.Samples.Helpers;
using Azure.AI.VoiceLive.Samples.Prompts;
using Spectre.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        Utilities.WelcomeMessage("Voice Live Command Line", "Welcome to the VoiceLive API Demo");

        var scenarioName = PromptManager.SelectAScenario();



        // NOTE: this code is working fine for the LLM Scenarios...  but the AGENT scenarios are failing with
        //   Error! System.Net.WebSockets.WebSocketException: Unable to connect to the remote server
        //   System.Net.Http.HttpRequestException: An error occurred while sending the request.
        //   System.IO.IOException: Unable to read data from the transport connection:
        //   An existing connection was forcibly closed by the remote host;



        await VoiceAssistantSetup.StartVoiceAssistant(scenarioName);

        AnsiConsole.MarkupLine("\n[cyan]Press any key to exit...[/]");
        Console.ReadKey();
    }
}