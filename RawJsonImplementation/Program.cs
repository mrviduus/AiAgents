using System.Text.Json;
using RawJsonImplementation.Models;
using RawJsonImplementation.Services;
using dotenv.net;

DotEnv.Load();
var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (openAiKey == null)
    throw new InvalidOperationException("Missing OPENAI_API_KEY");


List<ChatMessage> messages = [
    new ChatMessage
    {
        Role = ChatRole.Assistant,
        Content = "Hello, what do you want to do today?"
    }
];

Console.WriteLine(messages[0].Content);

var aiService = new OpenAiService(openAiKey);

while (true)
{
    Console.ForegroundColor = ConsoleColor.Blue;
    var input = Console.ReadLine();
    if (input == null || input?.ToLower() == "exit")
        break;
    Console.ResetColor();

    messages.Add(new ChatMessage
    {
        Role = ChatRole.User,
        Content = input!
    });

    var response = await aiService.CompleteChat(messages);
    messages.Add(response);
    Console.WriteLine(messages.Last().Content);
}
