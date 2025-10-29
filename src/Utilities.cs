namespace Azure.AI.VoiceLive.Samples;

public class Utilities
{
    #region Configuration
    public static (string apiKey, string endpoint, string model, string voice, string instructionsFileName, string? tenantId, bool useTokenCredential, bool verbose) ReadConfig()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();

        string apiKey = configuration["VoiceLive:ApiKey"];
        string endpoint = configuration["VoiceLive:Endpoint"];
        string model = configuration["VoiceLive:Model"];
        string voice = configuration["VoiceLive:Voice"];
        string instructionsFileName = configuration["VoiceLive:InstructionsFileName"];
        string? tenantId = configuration["VisualStudioTenantId"];

        var useTokenCredential = string.IsNullOrEmpty(apiKey);
        var verbose = true;

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(model))
        {
            Console.WriteLine("❌ Error: No endpoint or model provided!");
            return (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, false);
        }

        return (apiKey, endpoint, model, voice, instructionsFileName, tenantId, useTokenCredential, verbose);
    }
    public static string ReadResourceFile(string fileName, ILogger<Program> logger)
    {
        var fileContents = string.Empty;
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var matchingResource = resourceNames.Where(r => r.Contains(fileName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (matchingResource != null)
            {
                using var stream = assembly.GetManifestResourceStream(matchingResource);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    fileContents = reader.ReadToEnd();
                    logger.LogInformation($"Loaded {fileContents.Length} bytes from embedded resource {fileName}");
                }
                else
                {
                    logger.LogWarning($"Resource file {fileName} not found!");
                }
            }
            else
            {
                logger.LogWarning($"Resource {fileName} not found!");
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"Failed to load contents from resource {fileName}");
            fileContents = string.Empty;
        }
        return fileContents;
    }

    #endregion

    #region Logging
    public static (ILoggerFactory, ILogger<Program>)  InitializeLogging()
    {
        // Setup logging
        var _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        var _logger = _loggerFactory.CreateLogger<Program>();
        return (_loggerFactory, _logger);
    }
    #endregion

    #region Credentials
    public static DefaultAzureCredential GetCredentials(IConfiguration configuration)
    {
        return GetCredentials(configuration["VisualStudioTenantId"], configuration["UserAssignedManagedIdentityClientId"]);
    }

    public static DefaultAzureCredential GetCredentials()
    {
        return GetCredentials(string.Empty, string.Empty);
    }

    public static DefaultAzureCredential GetCredentials(string? visualStudioTenantId)
    {
        return GetCredentials(visualStudioTenantId, string.Empty);
    }

    public static DefaultAzureCredential GetCredentials(string? visualStudioTenantId, string userAssignedManagedIdentityClientId)
    {
        if (!string.IsNullOrEmpty(visualStudioTenantId))
        {
            var azureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                VisualStudioTenantId = visualStudioTenantId,
                Diagnostics = { IsLoggingContentEnabled = true }
            });
            return azureCredential;
        }
        else
        {
            if (!string.IsNullOrEmpty(userAssignedManagedIdentityClientId))
            {
                var azureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = userAssignedManagedIdentityClientId,
                    Diagnostics = { IsLoggingContentEnabled = true }
                });
                return azureCredential;
            }
            else
            {
                var azureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    Diagnostics = { IsLoggingContentEnabled = true }
                });
                return azureCredential;
            }
        }
    }
    #endregion

    #region System Hardware
    public static bool CheckAudioSystem(ILogger logger)
    {
        try
        {
            // Try input (default device)
            using (var waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(24000, 16, 1),
                BufferMilliseconds = 50
            })
            {
                // Start/Stop to force initialization and surface any device errors
                waveIn.DataAvailable += (_, __) => { };
                waveIn.StartRecording();
                waveIn.StopRecording();
            }

            // Try output (default device)
            var buffer = new BufferedWaveProvider(new WaveFormat(24000, 16, 1))
            {
                BufferDuration = TimeSpan.FromMilliseconds(200)
            };

            using (var waveOut = new WaveOutEvent { DesiredLatency = 100 })
            {
                waveOut.Init(buffer);
                // Playing isn’t strictly required to validate a device, but it’s safe
                waveOut.Play();
                waveOut.Stop();
            }

            logger.LogInformation("Audio system check passed (default input/output initialized).");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Audio system check failed: {ex.Message}");
            return false;
        }
    }
    #endregion
}
