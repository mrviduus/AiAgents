// Program.cs (.NET 8/9 minimal API)
using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);

// Register your MCP server + discover tools in the assembly
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly()
    .WithToolsFromAssembly(); // scans for [McpServerTool] methods

builder.WebHost.UseUrls("http://localhost:5050");

builder.Services.AddSingleton<InvoiceApiClient>();

var app = builder.Build();

// Map the MCP endpoints for Streamable HTTP
// This adds the required /sse (events) and /messages endpoints.
app.MapMcp();

// (Optional) protect your MCP endpoints
// app.MapMcp().RequireAuthorization();

app.Run();