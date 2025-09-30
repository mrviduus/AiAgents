# Memory Systems for AI Agents

AI models are inherently stateless, processing each request in isolation with no memory of previous interactions. To create truly intelligent agents that can learn and maintain context over time, we need to implement sophisticated memory systems. This guide explores two fundamental types of memory and their implementation in C#.

## The Memory Challenge

Every AI interaction starts fresh - the model has no recollection of:
- Previous conversations
- Past decisions and their outcomes
- User preferences and context
- Learned patterns from experience
- Task progress and intermediate results

Memory systems solve this by providing persistent context storage and retrieval mechanisms.

## Episodic Memory

**Episodic Memory** stores specific experiences and events with their temporal context, like a detailed journal of interactions.

### Key Characteristics
- 📅 **Temporal ordering** - Events stored with timestamps
- 🎯 **Specific instances** - Exact interactions preserved
- 🔍 **Detailed context** - Full conversation threads
- 📊 **Sequential access** - Can replay event sequences
- 🏷️ **Tagged retrieval** - Find specific types of events

### C# Implementation

```csharp
public class EpisodicMemory
{
    private readonly List<Episode> _episodes;
    private readonly int _maxEpisodes;
    private readonly IVectorDatabase _vectorDb;

    public EpisodicMemory(int maxEpisodes = 1000, IVectorDatabase vectorDb = null)
    {
        _episodes = new List<Episode>();
        _maxEpisodes = maxEpisodes;
        _vectorDb = vectorDb;
    }

    public async Task<string> StoreEpisodeAsync(string context, string action, string result)
    {
        var episode = new Episode
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Context = context,
            Action = action,
            Result = result,
            Embedding = await GenerateEmbeddingAsync($"{context} {action} {result}")
        };

        _episodes.Add(episode);

        // Store in vector database for semantic search
        if (_vectorDb != null)
        {
            await _vectorDb.StoreAsync(episode.Id, episode.Embedding, episode);
        }

        // Manage memory size
        if (_episodes.Count > _maxEpisodes)
        {
            _episodes.RemoveAt(0); // Remove oldest
        }

        return episode.Id;
    }

    public async Task<List<Episode>> RecallSimilarEpisodesAsync(string query, int topK = 5)
    {
        if (_vectorDb == null)
        {
            // Fallback to recent episodes if no vector DB
            return _episodes.TakeLast(topK).ToList();
        }

        var queryEmbedding = await GenerateEmbeddingAsync(query);
        var similarIds = await _vectorDb.SearchAsync(queryEmbedding, topK);

        return _episodes.Where(e => similarIds.Contains(e.Id)).ToList();
    }

    public List<Episode> GetRecentEpisodes(int count = 10)
    {
        return _episodes.TakeLast(count).ToList();
    }

    public List<Episode> GetEpisodesByTimeRange(DateTime start, DateTime end)
    {
        return _episodes
            .Where(e => e.Timestamp >= start && e.Timestamp <= end)
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    private async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        // Implementation would call embedding model (OpenAI, etc.)
        // Placeholder for example
        await Task.Delay(10);
        return new float[1536]; // Standard embedding size
    }
}

public class Episode
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Context { get; set; }
    public string Action { get; set; }
    public string Result { get; set; }
    public float[] Embedding { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### Use Cases
- **Conversation history**: Maintaining chat context across sessions
- **Debugging trails**: Tracking agent decision sequences
- **User interaction patterns**: Learning from specific user behaviors
- **Task completion tracking**: Monitoring multi-step processes

## Semantic Memory

**Semantic Memory** stores generalized knowledge, facts, and concepts extracted from experiences - the "what" rather than the "when".

### Key Characteristics
- 🧠 **Conceptual knowledge** - Abstract facts and relationships
- 🔗 **Interconnected facts** - Knowledge graph structure
- 📚 **Accumulated wisdom** - Distilled from many experiences
- 🎯 **Context-independent** - Facts true regardless of situation
- 🔄 **Continuously updated** - Refined with new information

### C# Implementation

```csharp
public class SemanticMemory
{
    private readonly Dictionary<string, Concept> _concepts;
    private readonly Dictionary<string, List<Relationship>> _relationships;
    private readonly ILanguageModel _llm;
    private readonly IVectorDatabase _vectorDb;

    public SemanticMemory(ILanguageModel llm, IVectorDatabase vectorDb)
    {
        _concepts = new Dictionary<string, Concept>();
        _relationships = new Dictionary<string, List<Relationship>>();
        _llm = llm;
        _vectorDb = vectorDb;
    }

    public async Task LearnFromEpisodeAsync(Episode episode)
    {
        // Extract facts and concepts from episode
        var extraction = await _llm.ExtractKnowledgeAsync(
            $"Extract key facts and concepts from: {episode.Context} -> {episode.Result}"
        );

        foreach (var fact in extraction.Facts)
        {
            await StoreFact(fact);
        }

        foreach (var relationship in extraction.Relationships)
        {
            await StoreRelationship(relationship);
        }
    }

    public async Task StoreFact(Fact fact)
    {
        if (!_concepts.ContainsKey(fact.Subject))
        {
            _concepts[fact.Subject] = new Concept
            {
                Name = fact.Subject,
                Properties = new Dictionary<string, object>(),
                LastUpdated = DateTime.UtcNow
            };
        }

        var concept = _concepts[fact.Subject];
        concept.Properties[fact.Property] = fact.Value;
        concept.LastUpdated = DateTime.UtcNow;

        // Store embedding for semantic search
        var embedding = await GenerateEmbeddingAsync(fact.ToString());
        await _vectorDb.StoreAsync(fact.Id, embedding, fact);
    }

    public async Task StoreRelationship(Relationship relationship)
    {
        if (!_relationships.ContainsKey(relationship.Subject))
        {
            _relationships[relationship.Subject] = new List<Relationship>();
        }

        _relationships[relationship.Subject].Add(relationship);

        // Also store reverse relationship for bidirectional lookup
        var reverseKey = relationship.Object;
        if (!_relationships.ContainsKey(reverseKey))
        {
            _relationships[reverseKey] = new List<Relationship>();
        }

        _relationships[reverseKey].Add(new Relationship
        {
            Subject = relationship.Object,
            Predicate = $"inverse_{relationship.Predicate}",
            Object = relationship.Subject
        });
    }

    public async Task<KnowledgeContext> QueryKnowledgeAsync(string query)
    {
        // Semantic search for relevant facts
        var embedding = await GenerateEmbeddingAsync(query);
        var relevantFacts = await _vectorDb.SearchAsync(embedding, topK: 10);

        // Build knowledge graph around relevant facts
        var knowledgeGraph = new KnowledgeGraph();
        foreach (var factId in relevantFacts)
        {
            var fact = await _vectorDb.GetAsync<Fact>(factId);
            knowledgeGraph.AddFact(fact);

            // Add related relationships
            if (_relationships.ContainsKey(fact.Subject))
            {
                foreach (var rel in _relationships[fact.Subject])
                {
                    knowledgeGraph.AddRelationship(rel);
                }
            }
        }

        return new KnowledgeContext
        {
            Facts = knowledgeGraph.Facts,
            Relationships = knowledgeGraph.Relationships,
            Confidence = CalculateConfidence(knowledgeGraph)
        };
    }

    public async Task<string> SummarizeKnowledgeAsync(string topic)
    {
        var knowledge = await QueryKnowledgeAsync(topic);
        var summary = await _llm.GenerateAsync(
            $"Summarize this knowledge about {topic}: {knowledge.ToPrompt()}"
        );
        return summary;
    }

    private float CalculateConfidence(KnowledgeGraph graph)
    {
        // Calculate based on number of facts, relationships, and recency
        var factCount = graph.Facts.Count;
        var relationshipCount = graph.Relationships.Count;

        return Math.Min(1.0f, (factCount + relationshipCount) / 20.0f);
    }

    private async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        // Implementation would call embedding model
        await Task.Delay(10);
        return new float[1536];
    }
}

public class Concept
{
    public string Name { get; set; }
    public Dictionary<string, object> Properties { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class Fact
{
    public string Id { get; set; }
    public string Subject { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
    public float Confidence { get; set; }
    public DateTime Learned { get; set; }

    public override string ToString()
    {
        return $"{Subject} has {Property}: {Value}";
    }
}

public class Relationship
{
    public string Subject { get; set; }
    public string Predicate { get; set; }
    public string Object { get; set; }
    public float Confidence { get; set; }
}

public class KnowledgeGraph
{
    public List<Fact> Facts { get; set; } = new List<Fact>();
    public List<Relationship> Relationships { get; set; } = new List<Relationship>();

    public void AddFact(Fact fact)
    {
        if (!Facts.Any(f => f.Id == fact.Id))
        {
            Facts.Add(fact);
        }
    }

    public void AddRelationship(Relationship relationship)
    {
        if (!Relationships.Any(r =>
            r.Subject == relationship.Subject &&
            r.Predicate == relationship.Predicate &&
            r.Object == relationship.Object))
        {
            Relationships.Add(relationship);
        }
    }
}

public class KnowledgeContext
{
    public List<Fact> Facts { get; set; }
    public List<Relationship> Relationships { get; set; }
    public float Confidence { get; set; }

    public string ToPrompt()
    {
        var prompt = "Known facts:\n";
        foreach (var fact in Facts)
        {
            prompt += $"- {fact}\n";
        }

        prompt += "\nRelationships:\n";
        foreach (var rel in Relationships)
        {
            prompt += $"- {rel.Subject} {rel.Predicate} {rel.Object}\n";
        }

        return prompt;
    }
}
```

## Episodic vs Semantic Memory

| Aspect | Episodic Memory | Semantic Memory |
|--------|-----------------|-----------------|
| **Content** | Specific events and experiences | General knowledge and facts |
| **Temporal** | Time-stamped and sequential | Timeless and abstract |
| **Retrieval** | "Remember when..." | "I know that..." |
| **Updates** | Append new episodes | Refine and merge knowledge |
| **Storage** | Linear/chronological | Graph/network structure |
| **Example** | "User asked about Python at 2pm" | "Python is a programming language" |

## Hybrid Memory Systems

Most effective AI agents combine both memory types:

```csharp
public class HybridMemorySystem
{
    private readonly EpisodicMemory _episodicMemory;
    private readonly SemanticMemory _semanticMemory;
    private readonly ILanguageModel _llm;

    public HybridMemorySystem(
        EpisodicMemory episodicMemory,
        SemanticMemory semanticMemory,
        ILanguageModel llm)
    {
        _episodicMemory = episodicMemory;
        _semanticMemory = semanticMemory;
        _llm = llm;
    }

    public async Task<MemoryContext> PrepareContextAsync(string query)
    {
        // Get recent relevant episodes
        var recentEpisodes = await _episodicMemory.RecallSimilarEpisodesAsync(query, 5);
        var episodicContext = FormatEpisodes(recentEpisodes);

        // Get relevant semantic knowledge
        var knowledge = await _semanticMemory.QueryKnowledgeAsync(query);
        var semanticContext = knowledge.ToPrompt();

        // Combine both types of memory
        return new MemoryContext
        {
            Query = query,
            EpisodicContext = episodicContext,
            SemanticContext = semanticContext,
            CombinedPrompt = $@"
                Based on previous interactions:
                {episodicContext}

                Known facts:
                {semanticContext}

                Current query: {query}
            "
        };
    }

    public async Task LearnFromInteractionAsync(
        string context,
        string action,
        string result)
    {
        // Store as episode
        var episodeId = await _episodicMemory.StoreEpisodeAsync(context, action, result);

        // Extract and store semantic knowledge
        var episode = new Episode
        {
            Context = context,
            Action = action,
            Result = result,
            Timestamp = DateTime.UtcNow
        };
        await _semanticMemory.LearnFromEpisodeAsync(episode);
    }

    private string FormatEpisodes(List<Episode> episodes)
    {
        var formatted = new StringBuilder();
        foreach (var episode in episodes)
        {
            formatted.AppendLine($"[{episode.Timestamp:yyyy-MM-dd HH:mm}]");
            formatted.AppendLine($"Context: {episode.Context}");
            formatted.AppendLine($"Action: {episode.Action}");
            formatted.AppendLine($"Result: {episode.Result}");
            formatted.AppendLine();
        }
        return formatted.ToString();
    }
}

public class MemoryContext
{
    public string Query { get; set; }
    public string EpisodicContext { get; set; }
    public string SemanticContext { get; set; }
    public string CombinedPrompt { get; set; }
}
```

## Memory Management Strategies

### 1. Forgetting Mechanisms
```csharp
public class AdaptiveForgetting
{
    public void ApplyForgetting(List<Episode> episodes)
    {
        var now = DateTime.UtcNow;

        foreach (var episode in episodes.ToList())
        {
            var age = (now - episode.Timestamp).TotalDays;
            var importance = CalculateImportance(episode);

            // Exponential decay with importance weighting
            var retentionProbability = importance * Math.Exp(-age / 30.0);

            if (Random.NextDouble() > retentionProbability)
            {
                episodes.Remove(episode);
            }
        }
    }

    private float CalculateImportance(Episode episode)
    {
        // Factor in success, uniqueness, user feedback
        return 0.5f; // Placeholder
    }
}
```

### 2. Memory Consolidation
```csharp
public class MemoryConsolidation
{
    public async Task ConsolidateMemoriesAsync(
        List<Episode> episodes,
        SemanticMemory semanticMemory)
    {
        // Group similar episodes
        var clusters = ClusterEpisodes(episodes);

        foreach (var cluster in clusters)
        {
            // Extract common patterns
            var pattern = await ExtractPatternAsync(cluster);

            // Store as semantic knowledge
            await semanticMemory.StoreFact(new Fact
            {
                Subject = pattern.Subject,
                Property = "pattern",
                Value = pattern.Description,
                Confidence = pattern.Confidence
            });

            // Optionally compress episodic memories
            CompressCluster(cluster);
        }
    }
}
```

### 3. Working Memory
```csharp
public class WorkingMemory
{
    private readonly Queue<MemoryItem> _items;
    private readonly int _capacity;

    public WorkingMemory(int capacity = 7)
    {
        _capacity = capacity;
        _items = new Queue<MemoryItem>();
    }

    public void Add(MemoryItem item)
    {
        if (_items.Count >= _capacity)
        {
            _items.Dequeue(); // Remove oldest
        }
        _items.Enqueue(item);
    }

    public string GetContext()
    {
        return string.Join("\n", _items.Select(i => i.Content));
    }
}
```

## Best Practices

1. **Balance Episodic and Semantic**: Use episodic for recent context, semantic for established knowledge
2. **Implement Forgetting**: Prevent unbounded memory growth with intelligent pruning
3. **Use Vector Embeddings**: Enable semantic similarity search for better retrieval
4. **Compress Old Memories**: Summarize or consolidate aging episodic memories
5. **Index Efficiently**: Use appropriate data structures for quick retrieval
6. **Version Knowledge**: Track when facts were learned and confidence levels
7. **Handle Conflicts**: Resolve contradictions between memories
8. **Privacy Considerations**: Implement data retention policies and user control

## Key Takeaway

Effective memory systems transform stateless AI models into intelligent agents that can learn, remember, and improve over time. By combining episodic memory for specific experiences with semantic memory for general knowledge, agents can maintain context, learn from interactions, and provide increasingly personalized and accurate responses.

---
[← Previous: Completion vs Reasoning Models](../architecture/completion-vs-reasoning-models.md) | [Back to Main README](../../README.md) | [Next: Summarization Strategies →](summarization-strategies.md)