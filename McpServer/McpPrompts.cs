using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

[McpServerPromptType]
public static class McpPrompts
{
    [McpServerPrompt, Description("Creates a prompt to pay an invoice.")]
    public static ChatMessage Summarize([Description("The name of the invoice to mark as paid")] string invoiceName) =>
        new(ChatRole.User, $"Find the invoice \"{invoiceName}\" and mark it as paid");

}