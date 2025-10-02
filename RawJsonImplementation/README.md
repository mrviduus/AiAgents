# Getting Started: AI Agents in C#

## Raw JSON Example

This project demonstrates chat completion requests from a command line application in C#. It does not use any NuGet packages, the API requests are made using `HttpClient`.

## OpenAI Key

Create a file in this directory called `.env` and add your [OpenAI Key](https://platform.openai.com/api-keys) like this:

```
OPENAI_API_KEY=sk-proj-xxxxxxxxxxxxxxxxxxxxxxxx
```

## Running Locally

```sh
cd RawJsonImplementation

dotnet restore

dotnet run
```

## Using The Agent

You can talk to your AI agent by entering your messages into the command line after running `dotnet run`