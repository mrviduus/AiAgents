# Getting Started: AI Agents in C#

## Invoice Agent API

An API that provides the Agent API for the [InvoiceApp](../InvoiceApp/README.md) demo application.  Can read invoices, make changes to invoices, and read the documentation pages.

## API Keys
Create a file in this directory called `.env` and add the provider API keys according to the instructions in the [README](../README.md) in the root of this repository.

Your complete `.env` file should look something like this:

```
OPENAI_API_KEY=sk-proj-xxxxxxxxxxxxxxxxxxxxxxxx
CLAUDE_API_KEY=sk-ant-api03-xxxxxxxxxxxxxxxxx
GEMINI_API_KEY=xxxxxxxxxxxxxxxxxxxxxxxx
```

## Running Locally

```sh
cd InvoiceAgentApi

dotnet restore

# Run with CLI arguments to choose a provider and model, for example....

dotnet run  --provider openai --model gpt-5-mini

dotnet run  --provider gemini --model gemini-2.0-flash-lite

dotnet run  --provider claude --model claude-3-5-haiku-latest
```

## Using The Agent

Run [InvoiceApp](../InvoiceApp/README.md) according to the instructions in that project, then type your messages into the chat popup inside that application.

You can also talk to the agent directly over it's API at `http://localhost:5000/api/invoices` by using [Postman (or alternative)](https://www.reddit.com/r/softwaretesting/comments/14ze0g9/looking_for_a_postman_alternative/).