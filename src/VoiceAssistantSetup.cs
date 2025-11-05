namespace Azure.AI.VoiceLive.Samples;

public static class VoiceAssistantSetup
{
    public static async Task StartVoiceAssistant(string scenarioName)
    {
        #region Variables
        ILoggerFactory loggerFactory;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        ILogger<Program> logger = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        string instructions = string.Empty;
        #endregion

        try
        {
            (loggerFactory, logger) = Utilities.InitializeLogging();
            (var apiKey, var endpoint, var model, var voice, var tenantId, var useTokenCredential, var verbose) = Utilities.ReadConfig();

            // get scenario details
            var scenario = PromptManager.GetScenario(scenarioName);

            // if the Scenario specifies different voice, then use that
            voice = !string.IsNullOrEmpty(scenario.VoiceName) ? scenario.VoiceName : voice;

            // if the scenario has a prompt file name, then go load the instructions from that file
            if (!string.IsNullOrEmpty(scenario.PromptFileName))
            {
                instructions = Utilities.ReadResourceFile(scenario.PromptFileName);
            }

            // Create client with appropriate credentials
            VoiceLiveClient client;
            if (useTokenCredential)
            {
                var tokenCredential = Utilities.GetCredentials(tenantId);
                client = new VoiceLiveClient(new Uri(endpoint), tokenCredential, new VoiceLiveClientOptions());
                logger.LogInformation("Using Azure token credential");
            }
            else
            {
                var keyCredential = new AzureKeyCredential(apiKey!);
                client = new VoiceLiveClient(new Uri(endpoint), keyCredential, new VoiceLiveClientOptions());
                logger.LogInformation("Using API key credential");
            }

            // Create voice assistant
            using var assistant = new VoiceAssistant(client, scenario.ModelName, instructions, voice, loggerFactory);

            // Setup cancellation token for graceful shutdown
            using var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                logger.LogInformation("Received shutdown signal");
                cancellationTokenSource.Cancel();
            };

            // Start the assistant
            await assistant.StartAsync(cancellationTokenSource.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("\n" + Emoji.Known.WavingHand + $"  [cyan]Voice assistant shutting down... Goodbye![/]");
        }
        catch (Exception ex)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            logger.LogError(ex, "Fatal error");
#pragma warning restore CS8604 // Possible null reference argument.
            AnsiConsole.MarkupLine(Emoji.Known.Biohazard + $"  [red]Error![/]");
            AnsiConsole.WriteException(ex);
            AnsiConsole.MarkupLine("\n[cyan]Press any key to exit...[/]");
            Console.ReadKey();
        }
    }

}