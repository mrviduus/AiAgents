# Summarization Strategies for AI Agents

As AI agents engage in longer conversations and process more information, they face a critical challenge: **context window limitations**. Every AI model has a maximum number of tokens it can process at once. Effective summarization strategies are essential for managing this constraint while preserving important information.

## The Context Window Challenge

Modern language models have context limits:
- **GPT-3.5**: 4,096 - 16,385 tokens
- **GPT-4**: 8,192 - 128,000 tokens
- **Claude**: 100,000 - 200,000 tokens
- **Open-source models**: Often 2,048 - 8,192 tokens

When context exceeds these limits, agents must intelligently compress information without losing critical details.

## Core Summarization Strategies

### 1. Progressive Summarization

Build increasingly compressed summaries as conversations grow, maintaining a hierarchy from detailed to abstract.

```csharp
public class ProgressiveSummarizer
{
    private readonly ILanguageModel _llm;
    private readonly List<SummaryLevel> _summaryLevels;
    private readonly int _tokenLimit;

    public ProgressiveSummarizer(ILanguageModel llm, int tokenLimit = 2000)
    {
        _llm = llm;
        _tokenLimit = tokenLimit;
        _summaryLevels = new List<SummaryLevel>();
    }

    public async Task<string> AddContentAndSummarizeAsync(string newContent)
    {
        // Add to current level
        var currentLevel = GetOrCreateCurrentLevel();
        currentLevel.Contents.Add(newContent);
        currentLevel.TokenCount += EstimateTokens(newContent);

        // Check if we need to summarize
        if (currentLevel.TokenCount > _tokenLimit)
        {
            await CompressCurrentLevelAsync();
        }

        return GetFullContext();
    }

    private async Task CompressCurrentLevelAsync()
    {
        var currentLevel = _summaryLevels.Last();

        var prompt = $@"Summarize the following conversation/content,
                       preserving key facts, decisions, and context:
                       {string.Join("\n", currentLevel.Contents)}";

        var summary = await _llm.GenerateAsync(prompt);

        // Create new level with summary
        var newLevel = new SummaryLevel
        {
            Level = currentLevel.Level + 1,
            Summary = summary,
            TokenCount = EstimateTokens(summary),
            Timestamp = DateTime.UtcNow
        };

        // Move current content to archive
        currentLevel.Archived = true;

        // Add new compressed level
        _summaryLevels.Add(newLevel);
    }

    private string GetFullContext()
    {
        var context = new StringBuilder();

        // Add all level summaries (oldest to newest)
        foreach (var level in _summaryLevels.Where(l => l.Archived))
        {
            context.AppendLine($"[Summary Level {level.Level}]");
            context.AppendLine(level.Summary);
            context.AppendLine();
        }

        // Add current active content
        var currentLevel = _summaryLevels.LastOrDefault(l => !l.Archived);
        if (currentLevel != null)
        {
            foreach (var content in currentLevel.Contents)
            {
                context.AppendLine(content);
            }
        }

        return context.ToString();
    }

    private SummaryLevel GetOrCreateCurrentLevel()
    {
        var currentLevel = _summaryLevels.LastOrDefault(l => !l.Archived);
        if (currentLevel == null)
        {
            currentLevel = new SummaryLevel
            {
                Level = 0,
                Contents = new List<string>(),
                TokenCount = 0,
                Timestamp = DateTime.UtcNow
            };
            _summaryLevels.Add(currentLevel);
        }
        return currentLevel;
    }

    private int EstimateTokens(string text)
    {
        // Rough estimation: 1 token ≈ 4 characters
        return text.Length / 4;
    }
}

public class SummaryLevel
{
    public int Level { get; set; }
    public List<string> Contents { get; set; } = new List<string>();
    public string Summary { get; set; }
    public int TokenCount { get; set; }
    public bool Archived { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### 2. Rolling Window Summarization

Maintain a sliding window of detailed recent context with compressed older content.

```csharp
public class RollingWindowSummarizer
{
    private readonly ILanguageModel _llm;
    private readonly int _windowSize;
    private readonly int _summarySize;
    private readonly Queue<Message> _recentMessages;
    private string _compressedHistory;

    public RollingWindowSummarizer(
        ILanguageModel llm,
        int windowSize = 10,
        int summarySize = 500)
    {
        _llm = llm;
        _windowSize = windowSize;
        _summarySize = summarySize;
        _recentMessages = new Queue<Message>();
        _compressedHistory = "";
    }

    public async Task<string> ProcessMessageAsync(Message message)
    {
        _recentMessages.Enqueue(message);

        // Check if we need to compress
        if (_recentMessages.Count > _windowSize)
        {
            await CompressOldestMessagesAsync();
        }

        return BuildContext();
    }

    private async Task CompressOldestMessagesAsync()
    {
        // Take half of the window for compression
        var toCompress = new List<Message>();
        var compressCount = _windowSize / 2;

        for (int i = 0; i < compressCount && _recentMessages.Count > 0; i++)
        {
            toCompress.Add(_recentMessages.Dequeue());
        }

        if (toCompress.Any())
        {
            var prompt = $@"Previous summary: {_compressedHistory}

New messages to incorporate:
{FormatMessages(toCompress)}

Create a concise summary that preserves important information, decisions, and context.
Maximum {_summarySize} tokens.";

            _compressedHistory = await _llm.GenerateAsync(prompt);
        }
    }

    private string BuildContext()
    {
        var context = new StringBuilder();

        if (!string.IsNullOrEmpty(_compressedHistory))
        {
            context.AppendLine("=== Historical Context ===");
            context.AppendLine(_compressedHistory);
            context.AppendLine();
        }

        if (_recentMessages.Any())
        {
            context.AppendLine("=== Recent Conversation ===");
            context.AppendLine(FormatMessages(_recentMessages));
        }

        return context.ToString();
    }

    private string FormatMessages(IEnumerable<Message> messages)
    {
        return string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"));
    }
}

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### 3. Hierarchical Summarization

Create summaries at different levels of abstraction for different purposes.

```csharp
public class HierarchicalSummarizer
{
    private readonly ILanguageModel _llm;
    private readonly Dictionary<string, SummaryNode> _summaryTree;

    public HierarchicalSummarizer(ILanguageModel llm)
    {
        _llm = llm;
        _summaryTree = new Dictionary<string, SummaryNode>();
    }

    public async Task<SummaryHierarchy> CreateHierarchyAsync(
        List<Document> documents,
        int maxLevels = 3)
    {
        var hierarchy = new SummaryHierarchy();

        // Level 0: Original documents
        var leafNodes = documents.Select(doc => new SummaryNode
        {
            Id = Guid.NewGuid().ToString(),
            Level = 0,
            Content = doc.Content,
            Type = SummaryType.Original,
            TokenCount = EstimateTokens(doc.Content)
        }).ToList();

        hierarchy.Levels[0] = leafNodes;

        // Build higher levels
        for (int level = 1; level <= maxLevels; level++)
        {
            var previousLevel = hierarchy.Levels[level - 1];
            var currentLevel = await BuildNextLevelAsync(previousLevel, level);

            hierarchy.Levels[level] = currentLevel;

            // Stop if we've compressed to a single node
            if (currentLevel.Count == 1)
            {
                break;
            }
        }

        return hierarchy;
    }

    private async Task<List<SummaryNode>> BuildNextLevelAsync(
        List<SummaryNode> previousLevel,
        int currentLevel)
    {
        var newNodes = new List<SummaryNode>();
        var groupSize = Math.Max(2, previousLevel.Count / 5); // Group into ~5 summaries

        for (int i = 0; i < previousLevel.Count; i += groupSize)
        {
            var group = previousLevel.Skip(i).Take(groupSize).ToList();
            var summary = await SummarizeGroupAsync(group, currentLevel);

            newNodes.Add(new SummaryNode
            {
                Id = Guid.NewGuid().ToString(),
                Level = currentLevel,
                Content = summary,
                Type = GetSummaryType(currentLevel),
                Children = group.Select(g => g.Id).ToList(),
                TokenCount = EstimateTokens(summary)
            });
        }

        return newNodes;
    }

    private async Task<string> SummarizeGroupAsync(
        List<SummaryNode> nodes,
        int targetLevel)
    {
        var combinedContent = string.Join("\n\n", nodes.Select(n => n.Content));

        var prompt = targetLevel switch
        {
            1 => $"Create a detailed summary preserving key information:\n{combinedContent}",
            2 => $"Create a high-level overview of main themes:\n{combinedContent}",
            _ => $"Create an executive summary in 2-3 sentences:\n{combinedContent}"
        };

        return await _llm.GenerateAsync(prompt);
    }

    private SummaryType GetSummaryType(int level)
    {
        return level switch
        {
            1 => SummaryType.Detailed,
            2 => SummaryType.Overview,
            _ => SummaryType.Executive
        };
    }

    public async Task<string> GetContextForQueryAsync(
        string query,
        SummaryHierarchy hierarchy)
    {
        // Start with executive summary
        var context = new StringBuilder();
        var topLevel = hierarchy.GetTopLevel();

        if (topLevel != null)
        {
            context.AppendLine("=== Overview ===");
            context.AppendLine(topLevel.First().Content);
            context.AppendLine();
        }

        // Find relevant detailed sections
        var relevantNodes = await FindRelevantNodesAsync(query, hierarchy);

        foreach (var node in relevantNodes.Take(3))
        {
            context.AppendLine($"=== Relevant Detail (Level {node.Level}) ===");
            context.AppendLine(node.Content);
            context.AppendLine();
        }

        return context.ToString();
    }

    private async Task<List<SummaryNode>> FindRelevantNodesAsync(
        string query,
        SummaryHierarchy hierarchy)
    {
        // Implement semantic search across all nodes
        var allNodes = hierarchy.Levels.Values.SelectMany(level => level).ToList();

        // Placeholder for semantic similarity
        return allNodes.OrderBy(n => n.Level).Take(5).ToList();
    }

    private int EstimateTokens(string text)
    {
        return text.Length / 4;
    }
}

public class SummaryNode
{
    public string Id { get; set; }
    public int Level { get; set; }
    public string Content { get; set; }
    public SummaryType Type { get; set; }
    public List<string> Children { get; set; } = new List<string>();
    public int TokenCount { get; set; }
}

public class SummaryHierarchy
{
    public Dictionary<int, List<SummaryNode>> Levels { get; set; } = new();

    public List<SummaryNode> GetTopLevel()
    {
        var maxLevel = Levels.Keys.Max();
        return Levels[maxLevel];
    }
}

public enum SummaryType
{
    Original,
    Detailed,
    Overview,
    Executive
}

public class Document
{
    public string Id { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### 4. Entity-Based Summarization

Track and summarize information around key entities (people, projects, topics).

```csharp
public class EntityBasedSummarizer
{
    private readonly ILanguageModel _llm;
    private readonly Dictionary<string, EntitySummary> _entities;
    private readonly int _maxEntityContext;

    public EntityBasedSummarizer(ILanguageModel llm, int maxEntityContext = 500)
    {
        _llm = llm;
        _entities = new Dictionary<string, EntitySummary>();
        _maxEntityContext = maxEntityContext;
    }

    public async Task ProcessContentAsync(string content)
    {
        // Extract entities from content
        var entities = await ExtractEntitiesAsync(content);

        foreach (var entity in entities)
        {
            if (!_entities.ContainsKey(entity.Name))
            {
                _entities[entity.Name] = new EntitySummary
                {
                    Name = entity.Name,
                    Type = entity.Type,
                    FirstMention = DateTime.UtcNow,
                    Mentions = new List<Mention>()
                };
            }

            var summary = _entities[entity.Name];
            summary.Mentions.Add(new Mention
            {
                Context = content,
                Timestamp = DateTime.UtcNow,
                Sentiment = entity.Sentiment
            });

            // Compress if needed
            if (summary.Mentions.Count > 10)
            {
                await CompressEntityHistoryAsync(summary);
            }
        }
    }

    private async Task<List<Entity>> ExtractEntitiesAsync(string content)
    {
        var prompt = $@"Extract entities (people, organizations, projects, concepts) from:
{content}

Format: Name | Type | Sentiment (positive/negative/neutral)";

        var response = await _llm.GenerateAsync(prompt);
        return ParseEntities(response);
    }

    private List<Entity> ParseEntities(string response)
    {
        // Parse LLM response into entities
        var entities = new List<Entity>();
        foreach (var line in response.Split('\n'))
        {
            var parts = line.Split('|');
            if (parts.Length == 3)
            {
                entities.Add(new Entity
                {
                    Name = parts[0].Trim(),
                    Type = parts[1].Trim(),
                    Sentiment = parts[2].Trim()
                });
            }
        }
        return entities;
    }

    private async Task CompressEntityHistoryAsync(EntitySummary entity)
    {
        var oldMentions = entity.Mentions.Take(5).ToList();
        var context = string.Join("\n", oldMentions.Select(m => m.Context));

        var prompt = $@"Summarize the history of {entity.Name}:
{context}

Create a brief summary of key facts and relationships.";

        var summary = await _llm.GenerateAsync(prompt);

        // Replace old mentions with summary
        entity.CompressedHistory = summary;
        entity.Mentions = entity.Mentions.Skip(5).ToList();
    }

    public async Task<string> GetEntityContextAsync(string entityName)
    {
        if (!_entities.ContainsKey(entityName))
        {
            return "No information available about this entity.";
        }

        var entity = _entities[entityName];
        var context = new StringBuilder();

        context.AppendLine($"=== {entity.Name} ({entity.Type}) ===");
        context.AppendLine($"First mentioned: {entity.FirstMention}");

        if (!string.IsNullOrEmpty(entity.CompressedHistory))
        {
            context.AppendLine("\nHistorical context:");
            context.AppendLine(entity.CompressedHistory);
        }

        if (entity.Mentions.Any())
        {
            context.AppendLine("\nRecent mentions:");
            foreach (var mention in entity.Mentions.TakeLast(3))
            {
                context.AppendLine($"- {mention.Timestamp:yyyy-MM-dd}: {mention.Context.Substring(0, Math.Min(100, mention.Context.Length))}...");
            }
        }

        return context.ToString();
    }

    public Dictionary<string, EntitySummary> GetAllEntities()
    {
        return _entities;
    }
}

public class Entity
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Sentiment { get; set; }
}

public class EntitySummary
{
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime FirstMention { get; set; }
    public List<Mention> Mentions { get; set; }
    public string CompressedHistory { get; set; }
    public Dictionary<string, object> Attributes { get; set; } = new();
}

public class Mention
{
    public string Context { get; set; }
    public DateTime Timestamp { get; set; }
    public string Sentiment { get; set; }
}
```

## Intelligent Context Management

Combine multiple strategies for optimal context management:

```csharp
public class IntelligentContextManager
{
    private readonly ProgressiveSummarizer _progressive;
    private readonly RollingWindowSummarizer _rolling;
    private readonly EntityBasedSummarizer _entityBased;
    private readonly ILanguageModel _llm;
    private readonly int _maxContextTokens;

    public IntelligentContextManager(
        ILanguageModel llm,
        int maxContextTokens = 4000)
    {
        _llm = llm;
        _maxContextTokens = maxContextTokens;
        _progressive = new ProgressiveSummarizer(llm);
        _rolling = new RollingWindowSummarizer(llm);
        _entityBased = new EntityBasedSummarizer(llm);
    }

    public async Task<string> PrepareContextAsync(string query, Message newMessage)
    {
        // Process new message through all summarizers
        await _progressive.AddContentAndSummarizeAsync(newMessage.Content);
        await _rolling.ProcessMessageAsync(newMessage);
        await _entityBased.ProcessContentAsync(newMessage.Content);

        // Build optimal context based on query
        var contextBuilder = new ContextBuilder(_maxContextTokens);

        // Add query-specific entity information
        var entities = await ExtractEntitiesFromQueryAsync(query);
        foreach (var entity in entities)
        {
            var entityContext = await _entityBased.GetEntityContextAsync(entity);
            contextBuilder.TryAdd(entityContext, ContextPriority.High);
        }

        // Add recent conversation context
        var recentContext = await _rolling.ProcessMessageAsync(newMessage);
        contextBuilder.TryAdd(recentContext, ContextPriority.Medium);

        // Add compressed historical context
        var historicalContext = await _progressive.AddContentAndSummarizeAsync("");
        contextBuilder.TryAdd(historicalContext, ContextPriority.Low);

        return contextBuilder.Build();
    }

    private async Task<List<string>> ExtractEntitiesFromQueryAsync(string query)
    {
        var prompt = $"Extract entity names from: {query}";
        var response = await _llm.GenerateAsync(prompt);
        return response.Split(',').Select(e => e.Trim()).ToList();
    }
}

public class ContextBuilder
{
    private readonly List<ContextItem> _items = new();
    private readonly int _maxTokens;

    public ContextBuilder(int maxTokens)
    {
        _maxTokens = maxTokens;
    }

    public void TryAdd(string content, ContextPriority priority)
    {
        _items.Add(new ContextItem
        {
            Content = content,
            Priority = priority,
            TokenCount = EstimateTokens(content)
        });
    }

    public string Build()
    {
        var orderedItems = _items.OrderBy(i => i.Priority).ToList();
        var result = new StringBuilder();
        var currentTokens = 0;

        foreach (var item in orderedItems)
        {
            if (currentTokens + item.TokenCount <= _maxTokens)
            {
                result.AppendLine(item.Content);
                currentTokens += item.TokenCount;
            }
        }

        return result.ToString();
    }

    private int EstimateTokens(string text)
    {
        return text.Length / 4;
    }
}

public class ContextItem
{
    public string Content { get; set; }
    public ContextPriority Priority { get; set; }
    public int TokenCount { get; set; }
}

public enum ContextPriority
{
    High = 0,
    Medium = 1,
    Low = 2
}
```

## Choosing the Right Strategy

| Strategy | Best For | Pros | Cons |
|----------|----------|------|------|
| **Progressive** | Long conversations | Maintains detail hierarchy | Can lose nuance in compression |
| **Rolling Window** | Chat applications | Recent context preserved | Older context heavily compressed |
| **Hierarchical** | Document processing | Multiple abstraction levels | Complex to implement |
| **Entity-Based** | Knowledge tracking | Preserves entity relationships | May miss non-entity information |

## Best Practices

1. **Preserve Critical Information**
   - Always retain user goals and constraints
   - Keep track of decisions and agreements
   - Maintain error/warning context

2. **Adaptive Compression**
   - Adjust compression ratio based on content importance
   - Use different strategies for different content types
   - Monitor information loss

3. **Semantic Preservation**
   - Use embeddings to identify similar content
   - Merge redundant information
   - Maintain semantic relationships

4. **User Control**
   - Allow users to mark important information
   - Provide summaries for user validation
   - Enable context reset when needed

5. **Performance Optimization**
   - Cache summaries when possible
   - Batch summarization operations
   - Use appropriate model sizes for summarization

## Key Takeaway

Effective summarization is crucial for building AI agents that can handle long-running tasks and conversations. By combining multiple strategies and adapting to context needs, agents can maintain coherent understanding while working within token limitations. The key is balancing information preservation with compression efficiency.

---
[← Previous: Memory Systems](memory-systems.md) | [Back to Main README](../../README.md)