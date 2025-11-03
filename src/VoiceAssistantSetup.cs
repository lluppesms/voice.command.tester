namespace Azure.AI.VoiceLive.Samples;

public static class VoiceAssistantSetup
{
    public static async Task StartVoiceAssistant(string scenarioName)
    {
        ILoggerFactory loggerFactory;
        ILogger<Program> logger = null;
        string instructions = string.Empty;
        //string modelName = string.Empty;
        //string agentName = string.Empty;
        //string agentProject = string.Empty;
        string agentAccessToken = string.Empty;

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

            // if this scenario is an Agent, then go get a token and set up the agent endpoint parameters
            if (!string.IsNullOrEmpty(scenario.AgentName))
            {
                try
                {
                    logger.LogInformation("Generating agent access token using DefaultAzureCredential...");
                    var credential = Utilities.GetCredentials();
                    var tokenRequestContext = new TokenRequestContext(["https://ai.azure.com/.default"]);
                    var accessToken = await credential.GetTokenAsync(tokenRequestContext, default).ConfigureAwait(false);
                    agentAccessToken = accessToken.Token;
                    logger.LogInformation("Obtained agent access token successfully");

                    // Append agent parameters to the endpoint URL
                    var uriBuilder = new UriBuilder(endpoint);
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    // not sure what's correct here... adding multiple variants of the parameters to be safe  --- documentation varies...!
                    query["agent_id"] = scenario.AgentName;
                    query["project_id"] = scenario.ProjectName;
                    query["project_name"] = scenario.ProjectName;
                    query["agent_access_token"] = agentAccessToken;
                    query["agent-id"] = scenario.AgentName;
                    query["agent-project-name"] = scenario.ProjectName;
                    query["agent-access-token"] = agentAccessToken;
                    uriBuilder.Query = query.ToString();
                    endpoint = uriBuilder.ToString();
                    logger.LogInformation("Agent parameters added as query parameters: agent-id={AgentId}, agent-project-name={ProjectName}", scenario.AgentName, scenario.ProjectName);
                    logger.LogInformation($"Endpoint: {endpoint}");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine(Emoji.Known.Biohazard + $"  [red]Error generating agent access token![/]");
                    AnsiConsole.MarkupLine("[red]Please ensure you are authenticated with Azure CLI or have appropriate Azure credentials configured.[/]");
                    AnsiConsole.WriteException(ex);
                    return;
                }
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
            using var assistant = new VoiceAssistant(client, scenario.ModelName, instructions, scenario.AgentName, scenario.ProjectName, agentAccessToken, voice, loggerFactory);

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
            AnsiConsole.MarkupLine(Emoji.Known.Biohazard + $"  [red]Error![/]");
            AnsiConsole.WriteException(ex);
            AnsiConsole.MarkupLine("\n[cyan]Press any key to exit...[/]");
            Console.ReadKey();
        }
    }

}