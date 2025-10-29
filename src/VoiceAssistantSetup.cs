namespace Azure.AI.VoiceLive.Samples;

public static class VoiceAssistantSetup
{
    public static async Task StartVoiceAssistant(string instructionsFileName)
    {
        ILoggerFactory loggerFactory;
        ILogger<Program> logger = null;
        string? instructions;

        try
        {
            (loggerFactory, logger) = Utilities.InitializeLogging();
            (var apiKey, var endpoint, var model, var voice, var tenantId, var useTokenCredential, var verbose) = Utilities.ReadConfig();

            instructions = Utilities.ReadResourceFile(instructionsFileName, logger);

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
            using var assistant = new VoiceAssistant(client, model, voice, instructions, loggerFactory);

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
            logger.LogError(ex, "Fatal error");
            AnsiConsole.MarkupLine(Emoji.Known.Biohazard + $"  red]Error![/]");
            AnsiConsole.WriteException(ex);
            AnsiConsole.MarkupLine("\n[cyan]Press any key to exit...[/]");
            Console.ReadKey();
        }
    }

}