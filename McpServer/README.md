# Getting Started: AI Agents in C#

## MCP Server

A remote Streamable HTTP server for the [InvoiceApp](../InvoiceApp/README.md) web application.  Uses [modelcontextprotocol/csharp-sdk](https://github.com/modelcontextprotocol/csharp-sdk) to create the MCP server as a .NET web server.

## Running Locally

```sh
cd McpServer

dotnet restore

dotnet run
```

## Testing With MCP Inspector

The [MCP Inspector](https://github.com/modelcontextprotocol/inspector) is a developer tool for testing and debugging MCP servers.  You can run it locally, with [NodeJS](https://learn.microsoft.com/en-us/windows/dev-environment/javascript/nodejs-on-windows) installed run:

```sh
npx @modelcontextprotocol/inspector
```

The inspector should open automatically, if not then open the inspector in your browser using the link provided in the terminal, for example you should see something like this:

```sh
Starting MCP inspector...
⚙️ Proxy server listening on localhost:6277
🔑 Session token: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
   Use this token to authenticate requests or set DANGEROUSLY_OMIT_AUTH=true to disable auth

🚀 MCP Inspector is up and running at:
   http://localhost:6274/

🌐 Opening browser...
```

## Testing On Claude.ai

You can connect to a remote MCP server on Claude.AI by following these instructions [Connect Claude Code to tools via MCP](https://docs.anthropic.com/en/docs/claude-code/mcp).

If you want to tunnel your localhost MCP server to a public URL with an SSL certificate you can use a tool such as [https://localhost.run/](localhost.run)

```sh
ssh -R 80:localhost:5050 -o StrictHostKeyChecking=no nokey@localhost.run
```

This will allow you to talk to your MCP Server from Claude.ai and also debug it locally in the Visual Studio debugger.