namespace Azure.AI.VoiceLive.Samples;

public static class VoiceAssistantSetup
{
    public static async Task StartVoiceAssistant(string instructionsFileParm)
    {
        ILoggerFactory loggerFactory;
        ILogger<Program> logger = null;
        string? instructions;

        try
        {
            (loggerFactory, logger) = Utilities.InitializeLogging();
            (var apiKey, var endpoint, var model, var voice, var instructionsFileName, var tenantId, var useTokenCredential, var verbose) = Utilities.ReadConfig();

            // Read instructions file name from config, override if passed in on command line, default if neither is found
            if (string.IsNullOrEmpty(instructionsFileName)) instructionsFileName = "instructions.txt";
            if (!string.IsNullOrEmpty(instructionsFileParm)) instructionsFileName = instructionsFileParm;
            logger.LogInformation($"Loading instructions from {instructionsFileName}...");
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
            Console.WriteLine("\n👋 Voice assistant shut down. Goodbye!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fatal error");
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }

}