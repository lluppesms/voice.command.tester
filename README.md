# Basic Voice Assistant Sample

This sample program is derived from the Azure VoiceLive SDK documentation and the sample program [https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/ai/Azure.AI.VoiceLive/samples/BasicVoiceAssistant](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/ai/Azure.AI.VoiceLive/samples/BasicVoiceAssistant).

This app demonstrates the fundamental capabilities of the Azure VoiceLive SDK by creating a very basic voice assistant that can engage in natural conversation in real-time using bidirectional audio streaming. It captures audio from the user's microphone, sends it to the VoiceLive service for processing, and plays back the assistant's spoken responses through the speakers. The LLM can be configured to use prompts from the prompts folder which can easily be modified by the developer.

## New VoiceLive SDK Convenience Methods

This sample now demonstrates some of the new convenience methods added to the VoiceLive SDK for better developer experience:

**Used in this sample:**
- `ClearStreamingAudioAsync()` - Clears all input audio currently being streamed
- `ConfigureSessionAsync()` - Configures conversation session options
- `CancelResponseAsync()` - Cancels the current response generation
- `SendInputAudioAsync()` - Sends audio data to the service

**Additional convenience methods available:**
- `StartAudioTurnAsync()` / `EndAudioTurnAsync()` / `CancelAudioTurnAsync()` - Audio turn management
- `AppendAudioToTurnAsync()` - Append audio data to an ongoing turn  
- `ConnectAvatarAsync()` - Connect avatar with SDP for media negotiation
- `CommitInputAudioAsync()` / `ClearInputAudioAsync()` - Audio buffer operations

These methods eliminate the need to manually construct and populate `ClientEvent` classes, providing a more developer-friendly API similar to the OpenAI SDK.

## Features

- **Real-time voice conversation**: Seamless bidirectional audio streaming
- **Interruption handling**: Graceful handling of user interruptions during assistant responses
- **Multiple voice options**: Support for both OpenAI and Azure voices
- **Cross-platform audio**: Uses NAudio for reliable audio capture and playback
- **Robust error handling**: Automatic reconnection and error recovery
- **Configurable settings**: Command line options and configuration file support

## Prerequisites

- .NET 9.0 or later
- Azure VoiceLive API key or Azure credential
- Microphone and speakers/headphones
- Audio drivers properly installed

## Setup

1. **Install dependencies**:

   ```bash
   dotnet restore
   ```

2. **Configure credentials in appsettings.json or User Secrets**:

   You will need to have an Azure VoiceLive API key. You can set it up in `appsettings.json` like this:

   ```json
   {
      "VoiceLive": {
         "ApiKey": "your-api-key",
         "Endpoint": "wss://<YOUR-AI-FOUNDRY-SERVICES-ENDPOINT-NAME>.services.ai.azure.com/voice-live/realtime?api-version=2025-10-01",
         "Model": "gpt-realtime",
         "Voice": "en-US-AvaNeural",
         "InstructionsFileName": "instructions.txt"
      }
   }
   ```

## Usage

### Basic Usage

```bash
dotnet run
```

### Command Line Options

```bash
dotnet run --help
```

## How It Works

### Architecture
The sample uses a multi-threaded architecture for optimal performance:

1. **Main Thread**: UI and event processing
2. **Audio Capture**: NAudio input stream processing
3. **Audio Send**: Async transmission to VoiceLive service
4. **Audio Playback**: NAudio output stream processing

### Key Components

#### VoiceLiveClient
- Manages authentication and connection to the service
- Provides WebSocket-based real-time communication

#### VoiceLiveSession  
- Handles bidirectional message streaming
- Manages session lifecycle and configuration

#### AudioProcessor
- Captures audio from microphone (24kHz PCM16 mono)
- Streams audio to VoiceLive service in real-time
- Plays back assistant responses through speakers
- Handles interruption scenarios

#### BasicVoiceAssistant
- Orchestrates the entire conversation flow
- Handles VoiceLive events and updates
- Manages session configuration and voice settings

### Event Flow

1. **Session Setup**: Configure audio format, voice, and turn detection
2. **Audio Capture**: Start capturing microphone input
3. **Speech Detection**: Service detects when user starts/stops speaking
4. **Response Generation**: AI generates audio response
5. **Audio Playback**: Stream response audio to speakers
6. **Interruption Handling**: Stop playback when user interrupts

## Troubleshooting

### Audio Issues

**No microphone detected**:
- Ensure microphone is connected and recognized by Windows
- Check audio permissions for the application
- Try running with `--verbose` to see detailed audio device information

**No sound output**:
- Check speaker/headphone connections
- Verify volume levels
- Ensure no other applications are exclusively using audio devices

**Poor audio quality**:
- Check microphone positioning and levels
- Reduce background noise
- Ensure stable internet connection for real-time streaming

### Connection Issues

**Authentication failed**:
- Verify API key is correct and active
- Check endpoint URL format
- Try using `--use-token-credential` for Azure authentication

**Connection timeouts**:
- Check internet connectivity
- Verify firewall allows WebSocket connections
- Try different endpoint if available

### Performance Issues

**High latency**:
- Close other bandwidth-intensive applications
- Use wired internet connection instead of WiFi
- Reduce audio buffer sizes (requires code modification)

**Audio dropouts**:
- Check system resources (CPU, memory)
- Close unnecessary applications
- Update audio drivers

## Advanced Configuration

### Custom Audio Settings
Modify `AudioProcessor.cs` to adjust:
- Buffer sizes for latency vs. stability trade-offs
- Sample rates (requires service support)
- Audio formats and compression

### Session Configuration
Modify `BasicVoiceAssistant.cs` to customize:
- Turn detection sensitivity
- Response modalities (text + audio vs. audio only)
- Conversation context and memory

### Error Handling
The sample includes robust error handling for:
- Network connectivity issues
- Audio device problems  
- Service-side errors
- Graceful shutdown scenarios

## Next Steps

This basic sample can be extended with:

1. **Voice Selection UI**: Dynamic voice switching during conversation
2. **Conversation History**: Save and replay conversations
3. **Custom Instructions**: Runtime instruction updates
4. **Multi-Language Support**: Language detection and switching
5. **Audio Effects**: Voice modulation and audio processing
6. **Analytics**: Conversation metrics and usage tracking

## References

- [Voice Live How-To Guide](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/voice-live-how-to)
- [Voice Live API for Real-Time Voice Agents](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/voice-live)
- [Voice Live Data Privacy and Security](https://learn.microsoft.com/en-us/azure/ai-foundry/responsible-ai/speech-service/voice-live/data-privacy-security)
- [NAudio Documentation](https://github.com/naudio/NAudio)
- [System.CommandLine Documentation](https://docs.microsoft.com/dotnet/standard/commandline/)
