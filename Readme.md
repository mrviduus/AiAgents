# AI Agents Course - Learn by Building

Learn how to build AI agents in C# through practical examples. This repository contains 6 working projects that demonstrate different AI agent patterns, from simple chatbots to advanced MCP servers.

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

## 🚀 Getting Started in 5 Minutes

### What You Need
1. **.NET 10.0** - [Download here](https://dotnet.microsoft.com/download)
2. **Code Editor** - Visual Studio 2022 or VS Code
3. **API Keys** - Get at least one:
   - OpenAI: https://platform.openai.com/api-keys
   - Claude: https://console.anthropic.com/
   - Gemini: https://makersuite.google.com/app/apikey

### First-Time Setup

#### Option 1: Docker (Easiest!)
```bash
# 1. Clone this repository
git clone https://github.com/mrviduus/AiAgents.git
cd AiAgents

# 2. Copy and edit .env file
cp .env.example .env
# Edit .env with your API keys

# 3. Run with Docker Compose
docker-compose up  # All services
# OR specific services:
docker-compose -f docker-compose.invoice.yml up  # Invoice services only
docker-compose -f docker-compose.mcp.yml up      # MCP services only
```

#### Option 2: Manual Setup
1. Clone this repository
2. Create a `.env` file in the project folder you want to run:
```bash
OPENAI_API_KEY=your-key-here
CLAUDE_API_KEY=your-key-here
GEMINI_API_KEY=your-key-here
```

## 📦 The 6 Projects - From Simple to Advanced

### 1️⃣ **BasicOpenAiExamples** - Start Here!
Your first AI chatbot in 5 lines of code.
```bash
cd BasicOpenAiExamples
echo "OPENAI_API_KEY=your-key" > .env
dotnet run
# Chat with AI directly!
```

### 2️⃣ **ConsoleAgent** - AI with Superpowers
An AI agent that can check weather, manage wardrobe, and send emails.
```bash
cd ConsoleAgent
# Pick your AI provider:
dotnet run --provider openai --model gpt-4.1-mini
dotnet run --provider claude --model claude-3-5-haiku-latest
dotnet run --provider gemini --model gemini-2.0-flash-lite
```
Try: "What should I wear today in New York?"

### 3️⃣ **InvoiceAgentApi** - Business AI Assistant
AI that manages invoices and reads documentation.
```bash
cd InvoiceAgentApi
dotnet run --provider openai --model gpt-5-mini
# API runs at http://localhost:5001
```
Pair with InvoiceApp for full experience.

### 4️⃣ **InvoiceApp** - Web Interface
Beautiful web UI for the Invoice AI.
```bash
cd InvoiceApp
dotnet run
# Open http://localhost:5000 in browser
```

### 5️⃣ **McpServer** - Connect AI to Everything
Advanced server using Anthropic's MCP protocol.
```bash
cd McpServer
dotnet run
# Server at http://localhost:5050

# Test it:
npx @modelcontextprotocol/inspector
```
[Learn about MCP](#what-is-mcp)

### 6️⃣ **RawJsonImplementation** - Under the Hood
See how AI APIs work at the lowest level.
```bash
cd RawJsonImplementation
dotnet run
# Learn by reading the code!
```

## 💡 What You'll Learn

### The Big Ideas (With Examples!)

**🤖 AI Agents** - AI that can use tools and take actions
- Example: ConsoleAgent checks weather and suggests clothes

**🧠 Memory Systems** - Give AI memory between conversations
- Example: Agent remembers your preferences

**🔄 Agent Loops** - Think → Act → Observe → Repeat
- Example: AI tries multiple approaches to solve problems

**📚 RAG Pattern** - AI that reads documents before answering
- Example: InvoiceAgentApi reads docs to answer questions

**🔌 MCP Protocol** - Connect AI to any service
- Example: McpServer exposes tools, prompts, and resources

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

## 🔌 What is MCP?

**Model Context Protocol (MCP)** is Anthropic's way to connect AI to your tools and data.

Think of it like this:
- **Without MCP**: AI can only chat
- **With MCP**: AI can use your tools, read your docs, and take actions

### MCP Gives AI Three Superpowers:

1. **🔧 Tools** - Functions AI can call
   - Example: "List all invoices", "Mark invoice #123 as paid"

2. **💬 Prompts** - Ready-made templates
   - Example: "Pay invoice for [customer name]"

3. **📄 Resources** - Documents AI can read
   - Example: Your product documentation, FAQs

### Try It Yourself:

```bash
# 1. Start the MCP server
cd McpServer
dotnet run

# 2. Test with the inspector (new terminal)
npx @modelcontextprotocol/inspector
# Opens a web UI to test your tools!
```

### Connect to Claude.ai (Advanced):

Need HTTPS? Use this tunnel:
```bash
ssh -R 80:localhost:5050 nokey@localhost.run
# Gives you: https://abc123.localhost.run
```

Then add to Claude.ai → Settings → Connectors

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

## 🐳 Docker Support

### Quick Start with Docker

All services are containerized and ready to run:

```bash
# Run everything (Invoice + MCP services)
docker-compose up

# Run specific service groups
docker-compose -f docker-compose.invoice.yml up  # Invoice services
docker-compose -f docker-compose.mcp.yml up      # MCP + Inspector

# Run with HTTPS tunnel for Claude.ai
docker-compose --profile tunnel up
```

### Service URLs

When running with Docker:
- **InvoiceApp**: http://localhost:5000
- **InvoiceAgentApi**: http://localhost:5001
- **McpServer**: http://localhost:5050
- **MCP Inspector**: http://localhost:6274

### Docker Commands

```bash
# Build images
docker-compose build

# Run in background
docker-compose up -d

# View logs
docker-compose logs -f [service-name]

# Stop services
docker-compose down

# Clean up everything
docker-compose down -v  # Removes volumes too
```

### Docker Environment Variables

Set in `.env` file (copy from `.env.example`):
```env
AI_PROVIDER=openai      # or claude, gemini
AI_MODEL=gpt-4.1-mini   # your preferred model
```

## ❓ Troubleshooting

### Common Issues & Solutions

**"API key not found"**
- Create `.env` file in the project folder (not root)
- Format: `OPENAI_API_KEY=sk-...` (no quotes)

**"Model not found"**
- OpenAI: Use `gpt-4.1-mini` or `gpt-3.5-turbo`
- Claude: Use `claude-3-5-haiku-latest`
- Gemini: Use `gemini-2.0-flash-lite`

**"Port already in use"**
- InvoiceApp: Default port 5000
- InvoiceAgentApi: Default port 5001
- McpServer: Default port 5050
- Kill existing process or change port

**"dotnet: command not found"**
- Install .NET 10.0: https://dotnet.microsoft.com/download

**MCP Inspector not working**
- Install Node.js first: https://nodejs.org
- Run in separate terminal from MCP server

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