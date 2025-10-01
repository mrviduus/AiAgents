# RAG Pattern: Retrieval-Augmented Generation for AI Agents

**RAG (Retrieval-Augmented Generation)** is a powerful architectural pattern that enhances AI agent responses by retrieving relevant information from external knowledge sources before generating answers. This pattern addresses key limitations of pure LLM-based systems by grounding responses in factual, up-to-date information.

## The Problem RAG Solves

Traditional LLMs face several critical limitations:
- **Knowledge Cutoff**: Models only know information from their training data
- **Hallucinations**: Tendency to generate plausible-sounding but incorrect information
- **No Domain Expertise**: Cannot access specialized or proprietary knowledge
- **Static Knowledge**: Cannot learn new information without retraining
- **No Source Attribution**: Cannot cite where information comes from

RAG elegantly solves these problems by separating knowledge retrieval from response generation.

## Core RAG Architecture

```
┌─────────────────────────────────────────┐
│          RAG AGENT FLOW                 │
├─────────────────────────────────────────┤
│                                         │
│    ┌──────────────┐                    │
│    │ User Query   │                    │
│    └──────┬───────┘                    │
│           ▼                            │
│    ┌──────────────┐                    │
│    │ Query        │ (Embedding)        │
│    │ Vectorization│                    │
│    └──────┬───────┘                    │
│           ▼                            │
│    ┌──────────────┐                    │
│    │ Similarity   │ (Vector Search)    │
│    │ Search       │                    │
│    └──────┬───────┘                    │
│           ▼                            │
│    ┌──────────────┐                    │
│    │ Retrieved    │ (Top-K Chunks)     │
│    │ Context      │                    │
│    └──────┬───────┘                    │
│           ▼                            │
│    ┌──────────────┐                    │
│    │ LLM          │ (Query + Context)  │
│    │ Generation   │                    │
│    └──────┬───────┘                    │
│           ▼                            │
│    ┌──────────────┐                    │
│    │ Augmented    │                    │
│    │ Response     │                    │
│    └──────────────┘                    │
│                                         │
└─────────────────────────────────────────┘
```

## The Three Phases of RAG

### 1. Indexing Phase (Offline)
Prepare your knowledge base for retrieval:

1. **Document Collection**: Gather source documents (PDFs, web pages, databases)
2. **Chunking**: Split documents into manageable pieces (typically 200-1000 tokens)
3. **Embedding Generation**: Convert chunks to vector representations
4. **Vector Storage**: Store embeddings in a vector database

### 2. Retrieval Phase (Runtime)
Find relevant information for the query:

1. **Query Embedding**: Convert user query to vector representation
2. **Similarity Search**: Find most similar document chunks (cosine similarity, etc.)
3. **Ranking**: Order results by relevance score
4. **Context Assembly**: Combine top-K chunks into context

### 3. Generation Phase (Runtime)
Create the final response:

1. **Prompt Construction**: Combine query + retrieved context
2. **LLM Invocation**: Generate response using augmented prompt
3. **Response Formatting**: Structure answer with citations
4. **Quality Check**: Verify response uses retrieved information

## C# Implementation

### Basic RAG Agent

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RagAgent
{
    private readonly IEmbeddingModel _embeddingModel;
    private readonly IVectorDatabase _vectorDb;
    private readonly ILanguageModel _llm;
    private readonly int _topK;

    public RagAgent(
        IEmbeddingModel embeddingModel,
        IVectorDatabase vectorDb,
        ILanguageModel llm,
        int topK = 5)
    {
        _embeddingModel = embeddingModel;
        _vectorDb = vectorDb;
        _llm = llm;
        _topK = topK;
    }

    public async Task<RagResponse> QueryAsync(string userQuery)
    {
        // Step 1: Convert query to embedding
        var queryEmbedding = await _embeddingModel.GenerateEmbeddingAsync(userQuery);

        // Step 2: Retrieve relevant chunks
        var retrievedChunks = await _vectorDb.SearchAsync(
            queryEmbedding,
            _topK
        );

        // Step 3: Build augmented prompt
        var context = BuildContext(retrievedChunks);
        var augmentedPrompt = BuildPrompt(userQuery, context);

        // Step 4: Generate response
        var response = await _llm.GenerateAsync(augmentedPrompt);

        return new RagResponse
        {
            Answer = response,
            Sources = retrievedChunks.Select(c => c.Source).ToList(),
            RetrievedChunks = retrievedChunks
        };
    }

    private string BuildContext(List<DocumentChunk> chunks)
    {
        var context = "Relevant information:\n\n";
        for (int i = 0; i < chunks.Count; i++)
        {
            context += $"[{i + 1}] {chunks[i].Content}\n";
            context += $"Source: {chunks[i].Source}\n\n";
        }
        return context;
    }

    private string BuildPrompt(string query, string context)
    {
        return $@"
You are a helpful assistant that answers questions based on the provided context.

{context}

User Question: {query}

Instructions:
- Answer the question using ONLY the information from the provided context
- If the context doesn't contain enough information, say so
- Cite sources using [1], [2], etc. notation
- Be concise and accurate

Answer:";
    }
}

public class RagResponse
{
    public string Answer { get; set; }
    public List<string> Sources { get; set; }
    public List<DocumentChunk> RetrievedChunks { get; set; }
}

public class DocumentChunk
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string Source { get; set; }
    public float[] Embedding { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    public float RelevanceScore { get; set; }
}
```

### Advanced RAG with Indexing

```csharp
public class RagIndexer
{
    private readonly IEmbeddingModel _embeddingModel;
    private readonly IVectorDatabase _vectorDb;
    private readonly IDocumentProcessor _docProcessor;

    public RagIndexer(
        IEmbeddingModel embeddingModel,
        IVectorDatabase vectorDb,
        IDocumentProcessor docProcessor)
    {
        _embeddingModel = embeddingModel;
        _vectorDb = vectorDb;
        _docProcessor = docProcessor;
    }

    public async Task IndexDocumentsAsync(List<string> documentPaths)
    {
        foreach (var path in documentPaths)
        {
            // Load and parse document
            var document = await _docProcessor.LoadAsync(path);

            // Split into chunks
            var chunks = SplitIntoChunks(document, chunkSize: 500, overlap: 50);

            // Generate embeddings and store
            foreach (var chunk in chunks)
            {
                var embedding = await _embeddingModel.GenerateEmbeddingAsync(chunk.Content);

                chunk.Embedding = embedding;
                chunk.Source = path;
                chunk.Metadata = new Dictionary<string, object>
                {
                    { "documentPath", path },
                    { "chunkIndex", chunk.Index },
                    { "createdAt", DateTime.UtcNow }
                };

                await _vectorDb.UpsertAsync(chunk.Id, embedding, chunk);
            }
        }
    }

    private List<DocumentChunk> SplitIntoChunks(
        string document,
        int chunkSize = 500,
        int overlap = 50)
    {
        var chunks = new List<DocumentChunk>();
        var words = document.Split(' ');
        var chunkIndex = 0;

        for (int i = 0; i < words.Length; i += (chunkSize - overlap))
        {
            var chunkWords = words.Skip(i).Take(chunkSize).ToArray();
            var chunkContent = string.Join(" ", chunkWords);

            chunks.Add(new DocumentChunk
            {
                Id = Guid.NewGuid().ToString(),
                Content = chunkContent,
                Index = chunkIndex++
            });
        }

        return chunks;
    }
}
```

## RAG Ultrathink: Advanced Reasoning Patterns

**RAG Ultrathink** combines retrieval-augmented generation with advanced reasoning techniques to create agents capable of deep, multi-step analysis.

### Key Concepts of RAG Ultrathink

1. **Iterative Retrieval**: Multiple rounds of retrieval based on intermediate reasoning
2. **Query Decomposition**: Breaking complex queries into sub-queries
3. **Cross-Document Synthesis**: Combining information from multiple sources
4. **Confidence-Based Retrieval**: Adaptive retrieval based on uncertainty
5. **Chain-of-Thought RAG**: Reasoning about what to retrieve and why

### C# Implementation: RAG Ultrathink Agent

```csharp
public class RagUltrathinkAgent
{
    private readonly RagAgent _ragAgent;
    private readonly ILanguageModel _llm;
    private readonly StringBuilder _thoughtLog;

    public RagUltrathinkAgent(RagAgent ragAgent, ILanguageModel llm)
    {
        _ragAgent = ragAgent;
        _llm = llm;
        _thoughtLog = new StringBuilder();
    }

    public async Task<UltrathinkResponse> ProcessComplexQueryAsync(string query)
    {
        const int maxIterations = 5;
        var accumulatedContext = new List<DocumentChunk>();
        var reasoningSteps = new List<string>();

        for (int i = 0; i < maxIterations; i++)
        {
            // Step 1: Reason about what information is needed
            var thought = await ReasonAboutInformationNeedsAsync(
                query,
                reasoningSteps,
                accumulatedContext
            );

            reasoningSteps.Add(thought.Reasoning);
            _thoughtLog.AppendLine($"Iteration {i + 1} Thought: {thought.Reasoning}");

            // Step 2: Decompose into specific sub-queries if needed
            var subQueries = thought.ShouldDecompose
                ? await DecomposeQueryAsync(query, thought.Reasoning)
                : new List<string> { thought.NextQuery };

            _thoughtLog.AppendLine($"Sub-queries: {string.Join(", ", subQueries)}");

            // Step 3: Retrieve for each sub-query
            foreach (var subQuery in subQueries)
            {
                var ragResponse = await _ragAgent.QueryAsync(subQuery);
                accumulatedContext.AddRange(ragResponse.RetrievedChunks);
            }

            // Step 4: Assess if we have enough information
            var assessment = await AssessInformationCompletenessAsync(
                query,
                accumulatedContext,
                reasoningSteps
            );

            _thoughtLog.AppendLine($"Assessment: {assessment.Explanation}");

            if (assessment.HasSufficientInformation)
            {
                // Step 5: Synthesize final answer
                var finalAnswer = await SynthesizeAnswerAsync(
                    query,
                    accumulatedContext,
                    reasoningSteps
                );

                return new UltrathinkResponse
                {
                    Answer = finalAnswer,
                    ReasoningSteps = reasoningSteps,
                    Sources = accumulatedContext.Select(c => c.Source).Distinct().ToList(),
                    ThoughtLog = _thoughtLog.ToString(),
                    IterationsUsed = i + 1
                };
            }
        }

        // Max iterations reached - synthesize with available information
        var fallbackAnswer = await SynthesizeAnswerAsync(
            query,
            accumulatedContext,
            reasoningSteps
        );

        return new UltrathinkResponse
        {
            Answer = fallbackAnswer,
            ReasoningSteps = reasoningSteps,
            Sources = accumulatedContext.Select(c => c.Source).Distinct().ToList(),
            ThoughtLog = _thoughtLog.ToString(),
            IterationsUsed = maxIterations,
            IsPartialAnswer = true
        };
    }

    private async Task<ReasoningThought> ReasonAboutInformationNeedsAsync(
        string originalQuery,
        List<string> previousSteps,
        List<DocumentChunk> currentContext)
    {
        var prompt = $@"
Original Question: {originalQuery}

Previous Reasoning Steps:
{string.Join("\n", previousSteps.Select((s, i) => $"{i + 1}. {s}"))}

Current Information Available:
{(currentContext.Any() ?
    string.Join("\n", currentContext.Select(c => $"- {c.Content.Substring(0, Math.Min(100, c.Content.Length))}..."))
    : "None yet")}

Think deeply about:
1. What information do we still need to answer the question completely?
2. Should we decompose the query into more specific sub-questions?
3. What's the most important piece of missing information?

Respond in JSON format:
{{
    ""reasoning"": ""your thought process"",
    ""shouldDecompose"": true/false,
    ""nextQuery"": ""specific query to retrieve information for"",
    ""expectedInformationType"": ""what kind of information we expect""
}}";

        var response = await _llm.GenerateAsync(prompt);
        return ParseReasoningThought(response);
    }

    private async Task<List<string>> DecomposeQueryAsync(string query, string reasoning)
    {
        var prompt = $@"
Original complex question: {query}

Reasoning: {reasoning}

Decompose this into 2-4 specific sub-questions that, when answered together,
will provide a complete answer to the original question.

Return as JSON array of strings.";

        var response = await _llm.GenerateAsync(prompt);
        return ParseSubQueries(response);
    }

    private async Task<InformationAssessment> AssessInformationCompletenessAsync(
        string query,
        List<DocumentChunk> context,
        List<string> reasoning)
    {
        var prompt = $@"
Question: {query}

Retrieved Information:
{string.Join("\n\n", context.Select((c, i) => $"[{i + 1}] {c.Content}"))}

Reasoning So Far:
{string.Join("\n", reasoning)}

Assess: Do we have enough information to provide a complete, accurate answer?

Respond in JSON:
{{
    ""hasSufficientInformation"": true/false,
    ""explanation"": ""why we do or don't have enough"",
    ""missingInformation"": [""list of what's still missing""],
    ""confidenceLevel"": 0.0-1.0
}}";

        var response = await _llm.GenerateAsync(prompt);
        return ParseInformationAssessment(response);
    }

    private async Task<string> SynthesizeAnswerAsync(
        string query,
        List<DocumentChunk> context,
        List<string> reasoning)
    {
        var prompt = $@"
Question: {query}

All Retrieved Information:
{string.Join("\n\n", context.Select((c, i) => $"[{i + 1}] {c.Content}\nSource: {c.Source}"))}

Your Reasoning Process:
{string.Join("\n", reasoning.Select((r, i) => $"{i + 1}. {r}"))}

Synthesize a comprehensive answer that:
1. Directly answers the original question
2. Integrates information from multiple sources
3. Cites sources using [1], [2], etc.
4. Acknowledges any limitations or uncertainties
5. Shows the logical flow from reasoning to conclusion

Answer:";

        return await _llm.GenerateAsync(prompt);
    }

    private ReasoningThought ParseReasoningThought(string response)
    {
        // Implement JSON parsing logic
        return new ReasoningThought();
    }

    private List<string> ParseSubQueries(string response)
    {
        // Implement JSON parsing logic
        return new List<string>();
    }

    private InformationAssessment ParseInformationAssessment(string response)
    {
        // Implement JSON parsing logic
        return new InformationAssessment();
    }
}

public class UltrathinkResponse
{
    public string Answer { get; set; }
    public List<string> ReasoningSteps { get; set; }
    public List<string> Sources { get; set; }
    public string ThoughtLog { get; set; }
    public int IterationsUsed { get; set; }
    public bool IsPartialAnswer { get; set; }
}

public class ReasoningThought
{
    public string Reasoning { get; set; }
    public bool ShouldDecompose { get; set; }
    public string NextQuery { get; set; }
    public string ExpectedInformationType { get; set; }
}

public class InformationAssessment
{
    public bool HasSufficientInformation { get; set; }
    public string Explanation { get; set; }
    public List<string> MissingInformation { get; set; }
    public float ConfidenceLevel { get; set; }
}
```

## RAG Patterns Comparison

| Pattern | Use Case | Complexity | Quality | Speed |
|---------|----------|------------|---------|-------|
| **Basic RAG** | Simple Q&A, documentation lookup | Low | Good | Fast |
| **Multi-Query RAG** | Questions requiring multiple perspectives | Medium | Better | Medium |
| **Iterative RAG** | Complex research tasks | High | Best | Slow |
| **RAG Ultrathink** | Deep analysis, synthesis across sources | Very High | Excellent | Slowest |

## Chunking Strategies

### 1. Fixed-Size Chunking
```csharp
public List<string> FixedSizeChunking(string text, int chunkSize = 500)
{
    var words = text.Split(' ');
    var chunks = new List<string>();

    for (int i = 0; i < words.Length; i += chunkSize)
    {
        chunks.Add(string.Join(" ", words.Skip(i).Take(chunkSize)));
    }

    return chunks;
}
```

### 2. Semantic Chunking
```csharp
public async Task<List<string>> SemanticChunking(string text)
{
    // Split by paragraphs or sections
    var sections = text.Split("\n\n");
    var chunks = new List<string>();
    var currentChunk = "";

    foreach (var section in sections)
    {
        if ((currentChunk + section).Length > 1000)
        {
            chunks.Add(currentChunk);
            currentChunk = section;
        }
        else
        {
            currentChunk += "\n\n" + section;
        }
    }

    if (!string.IsNullOrEmpty(currentChunk))
    {
        chunks.Add(currentChunk);
    }

    return chunks;
}
```

### 3. Sliding Window Chunking
```csharp
public List<string> SlidingWindowChunking(
    string text,
    int windowSize = 500,
    int overlap = 50)
{
    var words = text.Split(' ');
    var chunks = new List<string>();

    for (int i = 0; i < words.Length; i += (windowSize - overlap))
    {
        var chunk = string.Join(" ", words.Skip(i).Take(windowSize));
        chunks.Add(chunk);
    }

    return chunks;
}
```

## Vector Databases for RAG

Popular options for C# implementations:

- **Pinecone**: Cloud-native, easy to use
- **Weaviate**: Open-source with hybrid search
- **Qdrant**: High-performance, written in Rust
- **Milvus**: Scalable, feature-rich
- **Azure Cognitive Search**: Integrated with Azure ecosystem
- **PostgreSQL with pgvector**: SQL-based vector storage

## Best Practices

### 1. Chunking
- ✅ Keep chunks between 200-1000 tokens
- ✅ Use overlap (10-20%) to preserve context
- ✅ Preserve semantic boundaries (paragraphs, sections)
- ✅ Include metadata (source, date, author)

### 2. Retrieval
- ✅ Retrieve more than you need (top 10-20), then rerank
- ✅ Use hybrid search (vector + keyword) when possible
- ✅ Filter by metadata before vector search
- ✅ Implement relevance thresholds

### 3. Generation
- ✅ Explicitly instruct model to use retrieved context
- ✅ Ask for citations in the prompt
- ✅ Handle cases where context is insufficient
- ✅ Include confidence levels in responses

### 4. Evaluation
- ✅ Track retrieval accuracy (are relevant docs retrieved?)
- ✅ Measure answer quality (does it use retrieved info?)
- ✅ Monitor hallucination rate
- ✅ Collect user feedback

## Common Pitfalls and Solutions

❌ **Pitfall**: Retrieving irrelevant chunks
✅ **Solution**: Improve embeddings, use reranking, add metadata filters

❌ **Pitfall**: Context too long for LLM
✅ **Solution**: Summarize chunks, use selective retrieval

❌ **Pitfall**: Ignoring retrieved information
✅ **Solution**: Stronger prompting, add "quote from context" instruction

❌ **Pitfall**: Stale information
✅ **Solution**: Implement incremental updates, timestamp filtering

## When to Use RAG vs Fine-Tuning

| Scenario | RAG | Fine-Tuning |
|----------|-----|-------------|
| Frequently changing information | ✅ | ❌ |
| Large knowledge base | ✅ | ❌ |
| Need citations | ✅ | ❌ |
| Learning new patterns/style | ❌ | ✅ |
| Domain-specific language | ❌ | ✅ |
| Cost-sensitive at scale | ❌ | ✅ |

## Key Takeaway

RAG transforms AI agents from knowledge-limited text generators into powerful research assistants grounded in real information. By combining retrieval with generation, RAG agents can provide accurate, cited, and up-to-date responses. **RAG Ultrathink** takes this further by adding iterative reasoning, enabling agents to tackle complex queries requiring multi-source synthesis and deep analysis.

---
[← Previous: ReAct Pattern](react-pattern.md) | [Back to Main README](../../README.md) | [Next: Memory Systems →](../implementation/memory-systems.md)
