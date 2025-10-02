# AI Agents Course - C# Implementation

Welcome to the AI Agents Course repository! This comprehensive guide covers the fundamental concepts and practical implementations of AI agents using C#.

## 📚 Table of Contents

### Core Concepts
- **[GPT Fundamentals](docs/concepts/gpt-fundamentals.md)** - Understanding GPT models and their evolution
- **[Stateless AI](docs/concepts/stateless-ai.md)** - Why AI models have no memory and how to manage state

### Architecture Patterns
- **[Agent Loops](docs/architecture/agent-loops.md)** - The core pattern for autonomous AI agents
- **[ReAct Pattern](docs/architecture/react-pattern.md)** - Deep dive into Thought → Action → Observation
- **[RAG Pattern](docs/architecture/rag-pattern.md)** - Retrieval-Augmented Generation and RAG Ultrathink for grounded responses
- **[Completion vs Reasoning Models](docs/architecture/completion-vs-reasoning-models.md)** - Choosing the right model for your use case

### Implementation Details
- **[Memory Systems](docs/implementation/memory-systems.md)** - Episodic and semantic memory for AI agents
- **[Summarization Strategies](docs/implementation/summarization-strategies.md)** - Managing context and token limits

## 🚀 Quick Start

### Prerequisites
- .NET 10.0
- Visual Studio 2022 or VS Code
- Basic understanding of C# and async programming
- API Keys: OpenAI, Claude (Anthropic), Gemini (for running examples)

### Running the Projects

Each project demonstrates different AI agent patterns and implementations:

#### **BasicOpenAiExamples** - Simple Chat Loop
Basic chat implementation using OpenAI SDK directly.
```bash
cd BasicOpenAiExamples
dotnet run
```

#### **ConsoleAgent** - CLI Agent with Tools
Multi-provider CLI agent with weather, wardrobe, and email tools. Supports OpenAI, Claude, and Gemini.
```bash
cd ConsoleAgent
dotnet run --provider openai --model gpt-4.1-mini
dotnet run --provider claude --model claude-3-5-haiku-latest
dotnet run --provider gemini --model gemini-2.0-flash-lite
```
[📖 Full documentation](ConsoleAgent/README.md)

#### **InvoiceAgentApi** - Web API Agent
Web API agent for invoice management with documentation reading capabilities.
```bash
cd InvoiceAgentApi
dotnet run --provider openai --model gpt-5-mini
# Runs on http://localhost:5001
```
[📖 Full documentation](InvoiceAgentApi/README.md)

#### **McpServer** - Model Context Protocol Server
Streamable HTTP server implementing MCP (Model Context Protocol) with Tools, Prompts, and Resources.
```bash
cd McpServer
dotnet run
# Runs on http://localhost:5050
# Test with: npx @modelcontextprotocol/inspector
```
[📖 Full documentation](McpServer/README.md) | [📖 MCP Details](#-model-context-protocol-mcp)

#### **RawJsonImplementation** - Low-Level OpenAI
Low-level HTTP/JSON implementation for understanding OpenAI API internals.
```bash
cd RawJsonImplementation
dotnet run
```

#### **InvoiceApp** - Web UI
Razor Pages web application (companion to InvoiceAgentApi).
```bash
cd InvoiceApp
dotnet run
```

### Basic Agent Implementation

```csharp
public class SimpleAgent
{
    private readonly ILanguageModel _llm;
    private readonly Dictionary<string, ITool> _tools;

    public SimpleAgent(ILanguageModel llm)
    {
        _llm = llm;
        _tools = new Dictionary<string, ITool>();
    }

    public async Task<string> ProcessAsync(string task)
    {
        // Think
        var action = await _llm.DecideActionAsync(task);

        // Act
        var result = await _tools[action.Tool].ExecuteAsync(action.Parameters);

        // Observe and respond
        return await _llm.GenerateResponseAsync(result);
    }
}
```

## 🏗️ Key Concepts

### 1. Agent Loop Pattern
Transform stateless AI models into autonomous agents through iterative reasoning and action cycles.

### 2. Memory Management
Implement episodic and semantic memory to overcome AI's stateless nature.

### 3. RAG (Retrieval-Augmented Generation)
Ground AI responses in factual information by retrieving relevant context from knowledge bases before generation.

### 4. Model Selection
Choose between fast completion models and powerful reasoning models based on your requirements.

## 📖 Documentation Structure

```
docs/
├── concepts/           # Fundamental AI concepts
│   ├── gpt-fundamentals.md
│   └── stateless-ai.md
├── architecture/       # Design patterns and architectures
│   ├── agent-loops.md
│   ├── react-pattern.md
│   ├── rag-pattern.md
│   └── completion-vs-reasoning-models.md
└── implementation/     # Practical implementation guides
    ├── memory-systems.md
    └── summarization-strategies.md
```

## 💡 Best Practices

1. **Always manage context** - AI models are stateless; you must provide context
2. **Implement proper error handling** - Agent loops can fail; plan for recovery
3. **Monitor costs** - Each API call has a cost; implement budgets
4. **Use appropriate models** - Completion for speed, reasoning for accuracy
5. **Log everything** - Essential for debugging agent behavior

## 🛠️ Technology Stack

- **Language**: C# (.NET 10.0)
- **AI Models**: OpenAI GPT, Azure OpenAI, Claude, Google Gemini
- **Patterns**: Agent Loops, ReAct, RAG, Memory Systems, MCP
- **Tools**: Dependency Injection, Async/Await, LINQ, Vector Databases
- **Frameworks**: Microsoft.Extensions.AI, Model Context Protocol (MCP)

## 📝 Code Examples

All code examples in this repository are written in C# and follow Microsoft's coding conventions. Each concept includes:
- Theoretical explanation
- Practical implementation
- Real-world use cases
- Best practices

## 🎯 Learning Objectives

After studying this repository, you will understand:
- How GPT and LLMs work under the hood
- Why AI is stateless and how to manage state
- How to build autonomous AI agents
- Different memory strategies for AI systems
- When to use completion vs reasoning models
- How to implement the ReAct pattern
- How to build RAG systems and use RAG Ultrathink for deep reasoning
- Effective summarization techniques

## 🔌 Model Context Protocol (MCP)

The **McpServer** project demonstrates Anthropic's Model Context Protocol - a standardized way to connect AI to external tools, data sources, and services.

### What is MCP?

MCP provides three core primitives for AI integration:
- **Tools**: Functions the AI can call to perform actions (e.g., list invoices, create invoice, mark as paid)
- **Prompts**: Predefined user prompts for common workflows (e.g., "pay invoice")
- **Resources**: Files and documentation the AI can access (e.g., getting-started.md)

### Transport Types

#### STDIO Transport
- **Use case**: Local development with Claude Desktop
- **Communication**: Subprocess via stdin/stdout
- **Security**: No encryption needed (same machine)

#### Stream HTTP Transport (Used in McpServer)
- **Use case**: Remote access, web integration, Claude.ai
- **Communication**: HTTP POST/GET with optional SSE (Server-Sent Events)
- **Security**: Supports HTTPS/TLS for remote connections
- **Port**: `http://localhost:5050`

### Quick Start

```bash
# Run the MCP server
cd McpServer
dotnet run

# Test with MCP Inspector (in another terminal)
npx @modelcontextprotocol/inspector
```

### Integration

**For Claude.ai Web** (requires HTTPS):
```bash
# Tunnel localhost to public HTTPS URL
ssh -R 80:localhost:5050 -o StrictHostKeyChecking=no nokey@localhost.run
```

Then connect via Claude.ai Settings → Connectors (requires Pro/Team/Enterprise plan).

[📖 Full MCP Documentation](McpServer/README.md) | [📖 MCP in CLAUDE.md](CLAUDE.md#model-context-protocol-mcp)

## 🔗 Quick Links

### Documentation
- [Agent Loop Examples](docs/architecture/agent-loops.md#c-implementation-examples)
- [Memory System Implementation](docs/implementation/memory-systems.md)
- [ReAct Pattern in C#](docs/architecture/react-pattern.md#c-implementation)
- [RAG and RAG Ultrathink](docs/architecture/rag-pattern.md#rag-ultrathink-advanced-reasoning-patterns)
- [Model Selection Guide](docs/architecture/completion-vs-reasoning-models.md#when-to-use-each-type)

### Projects
- [ConsoleAgent README](ConsoleAgent/README.md)
- [InvoiceAgentApi README](InvoiceAgentApi/README.md)
- [McpServer README](McpServer/README.md)
- [RawJsonImplementation README](RawJsonImplementation/README.md)

## 🤝 Contributing

This is an educational repository from our AI Agents course. Feel free to:
- Report issues
- Suggest improvements
- Share your implementations
- Ask questions

## 📄 License

This repository is for educational purposes. All code examples are provided as-is for learning.

---

*This repository contains examples and exercises from our AI Agents course, demonstrating practical applications of AI agent architectures in C#.*