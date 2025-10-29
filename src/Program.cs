// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

///// <summary>
///// FILE: Program.cs
///// </summary>
///// <remarks>
///// DESCRIPTION:
/////     This sample demonstrates the fundamental capabilities of the VoiceLive SDK by creating
/////     a basic voice assistant that can engage in natural conversation with proper interruption
/////     handling. This serves as the foundational example that showcases the core value
/////     proposition of unified speech-to-speech interaction.
/////
///// USAGE:
/////     dotnet run
/////
/////     update appsettings.json with your values.
/////
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
        Console.WriteLine("Voice Service initializing...");
        if (args.Length > 0 && args[0] == "--help")
        {
            Console.WriteLine("Usage: dotnet run");
            Console.WriteLine("This application starts a voice assistant using the VoiceLive SDK.");
            return;
        }
        var instructionsFile = string.Empty;
        if (args.Length > 0)
        {
            instructionsFile = args[0];
        }
        await VoiceAssistantSetup.StartVoiceAssistant(instructionsFile);
    }
}