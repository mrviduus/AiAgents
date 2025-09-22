using ConsoleAgent.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAgent;

public static class FunctionRegistry
{
    public static IEnumerable<AITool> GetTools(this IServiceProvider sp)
    {
        var weatherService = sp.GetRequiredService<IWeatherService>();

        var getWeatherFn = typeof(IWeatherService)
                 .GetMethod(nameof(IWeatherService.GetWeatherInCity),
                            [typeof(string), typeof(CancellationToken)])!;

        yield return AIFunctionFactory.Create(
            getWeatherFn,
            weatherService,
            new AIFunctionFactoryOptions
            {
                Name = "get_weather",
                Description = "Get the current weather descriptions in a specified city",
            });


        var wardrobeService = sp.GetRequiredService<WardrobeService>();

        var getWardrobeFn = typeof(WardrobeService)
                 .GetMethod(nameof(WardrobeService.ListClothing),
                            [])!;

        yield return AIFunctionFactory.Create(
            getWardrobeFn,
            wardrobeService,
            new AIFunctionFactoryOptions
            {
                Name = "get_clothing_from_wardrobe",
                Description = "Lists all the clothing I have in my wardrobe",
            });

        var emailService = sp.GetRequiredService<EmailService>();

        var emailFriendFn = typeof(EmailService)
                 .GetMethod(nameof(EmailService.EmailFriend),
                            [typeof(string), typeof(string)])!;

        yield return AIFunctionFactory.Create(
            emailFriendFn,
            emailService,
            new AIFunctionFactoryOptions
            {
                Name = "email_friend",
                Description = "Sends an email to my friend with this name",
            });

    }
}
