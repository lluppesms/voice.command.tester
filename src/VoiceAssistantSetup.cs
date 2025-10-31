using Azure.Core;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Azure.AI.VoiceLive.Samples;

public static class VoiceAssistantSetup
{
    public static async Task StartVoiceAssistant(string scenarioName)
    {
        ILoggerFactory loggerFactory;
        ILogger<Program> logger = null;
        string instructions = string.Empty;
        string agentAccessToken;

        try
        {
            (loggerFactory, logger) = Utilities.InitializeLogging();
            (var apiKey, var endpoint, var model, var voice, var tenantId, var useTokenCredential, var verbose) = Utilities.ReadConfig();

            var scenario = PromptManager.GetScenario(scenarioName);

            if (!string.IsNullOrEmpty(scenario.PromptFileName))
            {
                instructions = Utilities.ReadResourceFile(scenario.PromptFileName);
            }

            //if (!string.IsNullOrEmpty(scenario.AgentName))
            //{
            //    try
            //    {
            //        logger.LogInformation("Generating agent access token using DefaultAzureCredential...");
            //        var credential = Utilities.GetCredentials();
            //        var tokenRequestContext = new TokenRequestContext(["https://ai.azure.com/.default"]);
            //        var accessToken = await credential.GetTokenAsync(tokenRequestContext, default).ConfigureAwait(false);
            //        agentAccessToken = accessToken.Token;
            //        logger.LogInformation("Obtained agent access token successfully");


            //        // Append agent parameters to the endpoint URL
            //        var uriBuilder = new UriBuilder(endpoint);
            //        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            //        query["agent-id"] = scenario.AgentName;
            //        query["agent-project-name"] = scenario.ProjectName;
            //        query["agent-access-token"] = agentAccessToken;
            //        uriBuilder.Query = query.ToString();
            //        endpoint = uriBuilder.ToString();
            //        logger.LogInformation("Agent parameters added as query parameters: agent-id={AgentId}, agent-project-name={ProjectName}", scenario.AgentName, scenario.ProjectName);
            //    }
            //    catch (Exception ex)
            //    {
            //        AnsiConsole.MarkupLine(Emoji.Known.Biohazard + $"  [red]Error generating agent access token![/]");
            //        AnsiConsole.MarkupLine("[red]Please ensure you are authenticated with Azure CLI or have appropriate Azure credentials configured.[/]");
            //        AnsiConsole.WriteException(ex);
            //        return;
            //    }
            //}

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