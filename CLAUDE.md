# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an **AI Agents Course** repository demonstrating how to build autonomous AI agents in C#. It contains multiple educational implementations showcasing different agent patterns, architectures, and techniques. The repository combines theoretical documentation with practical C# implementations.

## Environment Setup

### Required API Keys
Create a `.env` file in each project directory with:
```
OPENAI_API_KEY=sk-proj-xxxxxxxxxxxxxxxxxxxxxxxx
CLAUDE_API_KEY=sk-ant-api03-xxxxxxxxxxxxxxxxx
GEMINI_API_KEY=xxxxxxxxxxxxxxxxxxxxxxxx
WEATHER_API_DOTCOM_KEY=xxxxxxxxxxxxxxxxxxxx  # Only for ConsoleAgent
```

### Build & Run Commands

**Main solution (BasicOpenAiExamples):**
```bash
dotnet restore
dotnet build
dotnet run --project BasicOpenAiExamples
```

**ConsoleAgent (CLI agent with tool calling):**
```bash
cd ConsoleAgent
dotnet restore
dotnet run --provider openai --model gpt-4.1-mini
dotnet run --provider gemini --model gemini-2.0-flash-lite
dotnet run --provider claude --model claude-3-5-haiku-latest
```

**InvoiceAgentApi (Web API agent):**
```bash
cd InvoiceAgentApi
dotnet restore
dotnet run --provider openai --model gpt-5-mini
# Runs on http://localhost:5001
```

**RawJsonImplementation (Low-level):**
```bash
cd RawJsonImplementation
dotnet restore
dotnet run
```

**McpServer (Model Context Protocol server):**
```bash
cd McpServer
dotnet restore
dotnet run
# Runs on http://localhost:5050
# Use MCP Inspector: npx @modelcontextprotocol/inspector
```

## Architecture Overview

### Multi-Provider Agent Architecture

The codebase uses a unified architecture across projects:

1. **Provider Abstraction Layer** (`Startup.cs`):
   - Uses `Microsoft.Extensions.AI` as abstraction
   - Supports OpenAI, Anthropic (Claude), and Google Gemini
   - Runtime provider selection via CLI arguments (`--provider`, `--model`)
   - Unified `IChatClient` interface for all providers

2. **Tool/Function Registry Pattern** (`FunctionRegistry.cs`):
   - Uses `AIFunctionFactory.Create()` to register C# methods as AI tools
   - Tools are automatically discovered via reflection
   - Injected as `ChatOptions.Tools` array
   - Functions are invoked automatically by the framework

3. **Chat Pipeline**:
   ```
   User Input → IChatClient → Tool Invocation (if needed) → Response
   ```
   - `ChatClientBuilder` chains logging and function invocation middleware
   - Automatic function execution with `UseFunctionInvocation()`

### Memory Management Pattern

**ConsoleAgent** implements automatic conversation summarization:
- Summarizes every 5 conversation turns
- Replaces old messages with summary to manage token limits
- Pattern in `ChatAgent.cs:42-50`:
  ```csharp
  if (turnsSinceLastSummary >= SUMMARY_INTERVAL)
  {
      var summary = await SummarizeHistory(history, client, chatOptions);
      history = [history[0], new ChatMessage(ChatRole.System, summary)];
  }
  ```

### Project Structure

**BasicOpenAiExamples**: Simple chat loop using OpenAI SDK directly
**ConsoleAgent**: Full agent with weather/wardrobe/email tools, multi-provider support
**InvoiceAgentApi**: Web API agent with documentation reading and invoice management
**InvoiceApp**: Razor Pages web UI (separate from agent)
**McpServer**: MCP (Model Context Protocol) server implementation
**RawJsonImplementation**: Low-level HTTP/JSON OpenAI integration

## Key Patterns Implemented

### 1. Agent Loop Pattern
Located in: `docs/architecture/agent-loops.md`
- Think → Act → Observe → Decide cycle
- Examples show iteration limits and termination conditions
- Working memory management

### 2. ReAct Pattern (Reasoning + Acting)
Located in: `docs/architecture/react-pattern.md`
- Thought → Action → Observation interleaving
- Better for complex multi-step tasks
- Shows reasoning traces for debugging

### 3. RAG Pattern
Located in: `docs/architecture/rag-pattern.md`
- Vector-based retrieval before generation
- **RAG Ultrathink**: Iterative retrieval with reasoning about what information is needed
- Query decomposition and multi-source synthesis

### 4. Memory Systems
Located in: `docs/implementation/memory-systems.md`
- **Episodic Memory**: Time-stamped events with vector search
- **Semantic Memory**: Knowledge graph of facts and relationships
- Hybrid systems combining both

### 5. Summarization Strategies
Located in: `docs/implementation/summarization-strategies.md`
- Progressive, Rolling Window, Hierarchical, Entity-Based approaches
- Context window management for long conversations

## Creating New Tools/Functions

To add a tool to an agent:

1. Create a service with the tool method:
```csharp
public class MyService
{
    public async Task<string> MyTool(string param, CancellationToken ct)
    {
        // Implementation
        return "result";
    }
}
```

2. Register in DI (`Startup.cs`):
```csharp
builder.Services.AddSingleton<MyService>();
```

3. Add to `FunctionRegistry.cs`:
```csharp
var myService = sp.GetRequiredService<MyService>();
yield return AIFunctionFactory.Create(
    typeof(MyService).GetMethod(nameof(MyService.MyTool),
        [typeof(string), typeof(CancellationToken)])!,
    myService,
    new AIFunctionFactoryOptions
    {
        Name = "my_tool",
        Description = "What this tool does"
    });
```

## Model Selection Guidelines

From `docs/architecture/completion-vs-reasoning-models.md`:

- **Completion Models** (GPT-3.5, Claude Instant, Gemini Flash):
  - Fast (50-200ms), cheap ($0.50-3 per million tokens)
  - Use for: simple tasks, high volume, real-time needs

- **Reasoning Models** (GPT-4, Claude Opus, o1):
  - Slow (10-60s), expensive ($15-60 per million tokens)
  - Use for: complex reasoning, accuracy-critical tasks, multi-step problems

## Important Implementation Details

### ChatOptions Configuration
All agents configure:
```csharp
new ChatOptions
{
    Tools = [.. FunctionRegistry.GetTools(sp)],
    ModelId = model,
    Temperature = 1,
    MaxOutputTokens = 5000  // Required for Claude
}
```

### Provider-Specific Notes
- **Claude**: Requires `MaxOutputTokens` set (default insufficient)
- **OpenAI**: Default model is `gpt-4.1-mini` if not specified
- **Gemini**: Uses `GeminiApiVersions.V1Beta`

### Token Estimation
Rough estimate throughout codebase: `tokens ≈ text.Length / 4`

### Message Roles
Standard roles used: `System`, `User`, `Assistant` (via `ChatRole` enum)

## Documentation Structure

All theoretical concepts in `/docs`:
- `/concepts`: GPT fundamentals, stateless AI nature
- `/architecture`: Agent loops, ReAct, RAG, model selection
- `/implementation`: Memory systems, summarization strategies

Each doc includes:
- Theoretical explanation
- C# implementation examples
- Real-world use cases
- Best practices and pitfalls

## Common Development Patterns

### Adding a New Agent Project
1. Reference `Microsoft.Extensions.AI` and provider SDKs
2. Create `Startup.cs` with provider switching logic
3. Create `FunctionRegistry.cs` for tools
4. Implement main loop in `Program.cs` or separate class
5. Add `.env` support with `dotenv.net`

### Switching AI Providers
Runtime selection via CLI:
```bash
dotnet run --provider openai --model gpt-4.1-mini
dotnet run --provider claude --model claude-3-5-haiku-latest
dotnet run --provider gemini --model gemini-2.0-flash-lite
```

Provider switch is in `Startup.ConfigureServices`:
```csharp
var client = provider switch
{
    "openai" => new OpenAI.Chat.ChatClient(model, openAiKey).AsIChatClient(),
    "gemini" => new GeminiChatClient(new GeminiClientOptions { ... }),
    "claude" => new AnthropicClient(new APIAuthentication(claudeKey)).Messages,
    _ => throw new ArgumentException($"Unknown provider: {provider}")
};
```

## Model Context Protocol (MCP)

The **McpServer** project demonstrates MCP (Model Context Protocol) - Anthropic's standardized protocol for connecting AI to external tools, data sources, and services. MCP enables AI agents to interact with external systems in a uniform way.

### What is MCP?

MCP provides three core primitives:
- **Tools**: Functions the AI can call to perform actions
- **Prompts**: Predefined user prompts for common workflows
- **Resources**: Files, documents, and data the AI can access

### Transport Types: STDIO vs Stream HTTP

MCP supports two transport mechanisms:

#### STDIO Transport
- **Communication**: Subprocess via stdin/stdout
- **Protocol**: Newline-delimited JSON-RPC messages
- **Best for**: Local, same-machine deployments
- **Security**: No encryption needed (same machine)
- **Use case**: Claude Desktop local servers

#### Stream HTTP Transport (Used in this project)
- **Communication**: HTTP POST/GET with optional SSE (Server-Sent Events)
- **Protocol**: Stateless HTTP with streaming support
- **Best for**: Remote access, multiple clients, web deployments
- **Security**: Supports HTTPS/TLS for remote connections
- **Use case**: Claude.ai web integration, remote servers
- **Note**: Replaced older HTTP+SSE in protocol version 2025-03-26

### McpServer Project Architecture

**Location**: `McpServer/`
**Port**: `http://localhost:5050`
**SDK**: `ModelContextProtocol.AspNetCore` v0.3.0-preview.3

```bash
cd McpServer
dotnet restore
dotnet run
```

**Key Setup** (from `Program.cs`):
```csharp
builder.Services
    .AddMcpServer()
    .WithHttpTransport()              // Streamable HTTP transport
    .WithPromptsFromAssembly()        // Auto-discover [McpServerPrompt]
    .WithResourcesFromAssembly()      // Auto-discover [McpServerResource]
    .WithToolsFromAssembly();         // Auto-discover [McpServerTool]

// Maps /sse (events) and /messages endpoints
app.MapMcp();
```

### MCP Tools (`McpTools.cs`)

Tools are functions AI agents can invoke. Uses attribute-based registration:

```csharp
[McpServerToolType]
public static class McpTools
{
    [McpServerTool, Description("Retrieves a list of all invoices in InvoiceApp")]
    public static Task<List<Invoice>> ListInvoices(InvoiceApiClient client)
    {
        return client.ListInvoices();
    }

    [McpServerTool, Description("Finds the invoice with this name")]
    public static Task<Invoice> FindInvoiceByName(string invoiceName, InvoiceApiClient client)
    {
        return client.FindInvoiceByName(invoiceName);
    }

    [McpServerTool, Description("Creates an invoice and returns the new invoice object")]
    public static Task<Invoice> CreateInvoice(CreateInvoiceRequest createRequest, InvoiceApiClient client)
    {
        return client.CreateInvoice(createRequest);
    }

    [McpServerTool, Description("Marks an invoice as paid")]
    public static Task MarkAsPaid(string invoiceId, InvoiceApiClient client)
    {
        return client.MarkAsPaid(invoiceId);
    }
}
```

**Pattern**:
- Static class with `[McpServerToolType]` attribute
- Methods marked with `[McpServerTool]` attribute
- DI-injected services as method parameters
- Auto-discovered by `.WithToolsFromAssembly()`

### MCP Prompts (`McpPrompts.cs`)

Prompts are predefined user messages for standardizing agent interactions:

```csharp
[McpServerPromptType]
public static class McpPrompts
{
    [McpServerPrompt, Description("Creates a prompt to pay an invoice.")]
    public static ChatMessage Summarize([Description("The name of the invoice to mark as paid")] string invoiceName) =>
        new(ChatRole.User, $"Find the invoice \"{invoiceName}\" and mark it as paid");
}
```

**Pattern**:
- Static class with `[McpServerPromptType]` attribute
- Methods marked with `[McpServerPrompt]` attribute
- Returns `ChatMessage` objects
- Useful for workflow templates

### MCP Resources (`McpResources.cs`)

Resources expose files and documentation to AI agents:

```csharp
[McpServerResourceType]
public static class McpResources
{
    [McpServerResource(MimeType = "text/markdown"), Description("Document describing how to use the InvoiceApp platform")]
    public static string GetDocumentationMarkdown()
    {
        return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Docs", "getting-started.md"));
    }
}
```

**Pattern**:
- Static class with `[McpServerResourceType]` attribute
- Methods marked with `[McpServerResource]` attribute
- Specify MIME type in attribute
- Returns file content as string

**Available Resources** (in `McpServer/Docs/`):
- `getting-started.md` - InvoiceApp introduction
- `creating-invoice.md` - How to create invoices
- `managing-invoices.md` - How to manage invoices
- `viewing-invoices.md` - How to view invoices

### Testing with MCP Inspector

The **MCP Inspector** is a developer tool for testing and debugging MCP servers:

```bash
npx @modelcontextprotocol/inspector
```

Opens browser at `http://localhost:6274/` with:
- Tool testing interface
- Prompt preview
- Resource inspection
- Real-time message debugging

### Integration Options

#### Claude Desktop (Local)
- **Requirement**: STDIO transport only
- **Current McpServer**: Uses HTTP, not compatible
- **To use**: Would need separate STDIO implementation

#### Claude.ai Web (Remote)
- **Requirement**: HTTPS endpoint required
- **Local Development**: Use tunneling service

**Tunnel localhost with localhost.run**:
```bash
ssh -R 80:localhost:5050 -o StrictHostKeyChecking=no nokey@localhost.run
```

This provides a public HTTPS URL for Claude.ai to connect to your local MCP server.

**Connect to Claude.ai**:
1. Get HTTPS URL from localhost.run tunnel
2. Follow [Connect Claude Code to tools via MCP](https://docs.anthropic.com/en/docs/claude-code/mcp)
3. Available for Pro, Max, Team, Enterprise plans
4. Supports OAuth 2.0 authentication

### Creating New MCP Components

#### Adding a Tool:
```csharp
[McpServerTool, Description("Your tool description")]
public static Task<ReturnType> YourTool(string param, YourService service)
{
    return service.DoSomething(param);
}
```

#### Adding a Prompt:
```csharp
[McpServerPrompt, Description("Your prompt description")]
public static ChatMessage YourPrompt([Description("Parameter description")] string param) =>
    new(ChatRole.User, $"Your template with {param}");
```

#### Adding a Resource:
```csharp
[McpServerResource(MimeType = "text/plain"), Description("Your resource description")]
public static string YourResource()
{
    return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "your-file.txt"));
}
```

**Important**:
- All components are auto-discovered via assembly scanning
- No manual registration needed
- Services injected via DI
- Files in `Docs/` are copied to output directory (see `.csproj`)
