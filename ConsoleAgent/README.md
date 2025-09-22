# Getting Started: AI Agents in C#

## Console Agent

A CLI agent that can help you plan what to wear - whatever the weather! Integrates with [weatherapi.com](https://www.weatherapi.com) to fetch the live weather in any city and works with three different AI providers via [Microsoft.Extensions.AI](https://learn.microsoft.com/en-us/dotnet/ai/microsoft-extensions-ai).

## API Keys
Create a file in this directory called `.env` and add all the API keys according to the instructions in the [README](../README.md) in the root of this repository.

Your complete `.env` file should look something like this:

```
OPENAI_API_KEY=sk-proj-xxxxxxxxxxxxxxxxxxxxxxxx
CLAUDE_API_KEY=sk-ant-api03-xxxxxxxxxxxxxxxxx
GEMINI_API_KEY=xxxxxxxxxxxxxxxxxxxxxxxx
WEATHER_API_DOTCOM_KEY=xxxxxxxxxxxxxxxxxxxx
```

## Running Locally

```sh
cd ConsoleAgent

dotnet restore

# Run with CLI arguments to choose a provider and model, for example....

dotnet run  --provider openai --model gpt-5-mini

dotnet run  --provider gemini --model gemini-2.0-flash-lite

dotnet run  --provider claude --model claude-3-5-haiku-latest
```

## Using The Agent

You can talk to your AI agent by entering your messages into the command line after running `dotnet run`