# CLAUDE.md

Quick reference for AI assistants working with this codebase.

## What This Is

Educational repository teaching AI agent development in C#. Contains 6 working projects demonstrating patterns from simple chatbots to MCP servers. All projects use .NET 10.0.

## Running Projects

```bash
# 1. BasicOpenAiExamples - Simple chat
cd BasicOpenAiExamples && dotnet run

# 2. ConsoleAgent - Multi-provider with tools
cd ConsoleAgent && dotnet run --provider openai --model gpt-4.1-mini

# 3. InvoiceAgentApi - Web API (port 5001)
cd InvoiceAgentApi && dotnet run --provider openai --model gpt-5-mini

# 4. InvoiceApp - Web UI (port 5000)
cd InvoiceApp && dotnet run

# 5. McpServer - MCP protocol (port 5050)
cd McpServer && dotnet run

# 6. RawJsonImplementation - Low-level API
cd RawJsonImplementation && dotnet run
```

## .env Files Required
Each project needs `.env` with API keys:
```
OPENAI_API_KEY=sk-...
CLAUDE_API_KEY=sk-ant-...
GEMINI_API_KEY=...
WEATHER_API_DOTCOM_KEY=...  # ConsoleAgent only
```

## Key Architecture Points

### Provider Switching (All Projects)
```csharp
// Startup.cs - Runtime provider selection
var client = provider switch
{
    "openai" => new OpenAI.Chat.ChatClient(model, key).AsIChatClient(),
    "gemini" => new GeminiChatClient(options),
    "claude" => new AnthropicClient(auth).Messages,
    _ => throw new ArgumentException()
};
```

### Tool Registration (ConsoleAgent, InvoiceAgentApi)
```csharp
// FunctionRegistry.cs - Auto tool discovery
AIFunctionFactory.Create(methodInfo, service, options)
// Injected as ChatOptions.Tools[]
```

### Memory Pattern (ConsoleAgent)
```csharp
// ChatAgent.cs - Summarizes every 5 turns
if (turnsSinceLastSummary >= 5)
{
    history = [systemPrompt, await Summarize(history)];
}
```

## Documentation Map

- `docs/concepts/` - GPT fundamentals, stateless AI
- `docs/architecture/` - Agent loops, ReAct, RAG patterns
- `docs/implementation/` - Memory systems, summarization

## Adding New Tools

1. Create service method
2. Register in `Startup.cs`: `builder.Services.AddSingleton<YourService>()`
3. Add to `FunctionRegistry.cs`: `AIFunctionFactory.Create(...)`

## Model Notes

**Fast/Cheap**: GPT-3.5, Claude Haiku, Gemini Flash
**Slow/Accurate**: GPT-4, Claude Opus, o1 models

**Claude**: Needs `MaxOutputTokens = 5000`
**Token estimate**: `tokens ≈ text.Length / 4`

## MCP (Model Context Protocol) - McpServer Project

**What**: Protocol to connect AI to tools/data
**Transport**: Stream HTTP (port 5050)
**Components**: Tools, Prompts, Resources

### STDIO vs Stream HTTP
- **STDIO**: Local, Claude Desktop, stdin/stdout
- **Stream HTTP**: Remote, web, HTTP+SSE (this project uses this)

### MCP Implementation

```csharp
// Program.cs - Auto-discovery
.WithToolsFromAssembly()     // [McpServerTool]
.WithPromptsFromAssembly()   // [McpServerPrompt]
.WithResourcesFromAssembly()  // [McpServerResource]
```

**Test**: `npx @modelcontextprotocol/inspector`
**HTTPS tunnel**: `ssh -R 80:localhost:5050 nokey@localhost.run`

### Quick MCP Component Template

```csharp
[McpServerTool] // or [McpServerPrompt] or [McpServerResource]
public static Task<T> YourMethod(params) { }
```
