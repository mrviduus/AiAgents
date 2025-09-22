using GeminiDotnet.Extensions.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Anthropic.SDK;
using GeminiDotnet;
using ConsoleAgent.Services;

namespace ConsoleAgent;

public static class Startup
{
    public static void ConfigureServices(HostApplicationBuilder builder, string provider, string model)
    {
        var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
        var geminiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")!;
        var claudeKey = Environment.GetEnvironmentVariable("CLAUDE_API_KEY")!;


        builder.Services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Information));
        builder.Services.AddSingleton<ILoggerFactory>(sp =>
            LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information)));

        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var client = provider switch
            {
                "openai" => new OpenAI.Chat.ChatClient(
                                string.IsNullOrWhiteSpace(model) ? "gpt-4.1-mini" : model,
                                openAiKey).AsIChatClient(),

                "gemini" => new GeminiChatClient(new GeminiDotnet.GeminiClientOptions { ApiKey = geminiKey, ModelId = model, ApiVersion = GeminiApiVersions.V1Beta }),

                "claude" => new AnthropicClient(new APIAuthentication(claudeKey)).Messages,

                _ => throw new ArgumentException($"Unknown provider: {provider}")
            };

            return new ChatClientBuilder(client)
                .UseLogging(loggerFactory)
                .UseFunctionInvocation(loggerFactory, c =>
                {
                    c.IncludeDetailedErrors = true;
                })
                .Build(sp);
        });

        builder.Services.AddTransient<ChatOptions>(sp => new ChatOptions
        {
            Tools = [.. FunctionRegistry.GetTools(sp)],
            ModelId = model,
            Temperature = 1,
            MaxOutputTokens = 5000 // needed for claude
        });


        builder.Services.AddSingleton<IWeatherService>(_ =>
        {
            var weatherKey = Environment.GetEnvironmentVariable("WEATHER_API_DOTCOM_KEY")!;
            return new WeatherApiDotComService(weatherKey);
        });
        builder.Services.AddSingleton<WardrobeService>();
        builder.Services.AddSingleton<EmailService>();

    }
}
