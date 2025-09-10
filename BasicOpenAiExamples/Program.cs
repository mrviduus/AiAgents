using dotenv.net;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;


DotEnv.Load();

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (apiKey == null)
{
    throw new InvalidOperationException("OPENAI_API_KEY is not set in environment variables.");
}

ChatClient client = new(model: "gpt-5-nano", apiKey: apiKey);

List<ChatMessage> messages = [
    new AssistantChatMessage("Hello! How can I assist you today?")
];

Console.WriteLine(messages[0].Content[0].Text);

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("You: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "exit")
    {
        break;
    }
    Console.ResetColor();
    messages.Add(new UserChatMessage(userInput));
    ChatCompletion completion = client.CompleteChat(messages);
    var response = completion.Content[0].Text;
    messages.Add(new AssistantChatMessage(response));
    Console.WriteLine($"AI: {response}");
}