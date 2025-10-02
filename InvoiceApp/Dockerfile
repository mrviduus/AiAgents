# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY InvoiceApp.csproj .
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 5000

# Copy published app
COPY --from=build /app/publish .

# Set environment for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

# Create data directory for SQLite
RUN mkdir -p /app/data
VOLUME ["/app/data"]

# Run the application
ENTRYPOINT ["dotnet", "InvoiceApp.dll"]