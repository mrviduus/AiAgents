# AI Agents Course

## What We Learn from the Course

### Definition of GPT

**GPT (Generative Pre-trained Transformer)** is a type of large language model developed by OpenAI that uses deep learning to produce human-like text. Here are the key aspects:

#### Key Components:
- **Generative**: Creates new text based on input prompts
- **Pre-trained**: Trained on vast amounts of text data before fine-tuning
- **Transformer**: Uses the transformer architecture with attention mechanisms

#### How GPT Works:
1. **Training Phase**: The model learns patterns from billions of text examples
2. **Inference Phase**: Given a prompt, it predicts the most likely next words/tokens
3. **Attention Mechanism**: Focuses on relevant parts of the input when generating responses

#### GPT Capabilities:
- Text generation and completion
- Question answering
- Language translation
- Code generation and debugging
- Creative writing
- Summarization
- Conversational AI

#### Evolution of GPT Models:
- **GPT-1** (2018): 117M parameters, proof of concept
- **GPT-2** (2019): 1.5B parameters, showed emergent abilities
- **GPT-3** (2020): 175B parameters, breakthrough in language understanding
- **GPT-3.5** (2022): Improved efficiency and instruction following
- **GPT-4** (2023): Multimodal capabilities, improved reasoning

#### Key Learning Points:
- GPT models are **autoregressive** - they generate text one token at a time
- They exhibit **emergent behaviors** not explicitly programmed
- **Prompt engineering** is crucial for getting desired outputs
- They have limitations: hallucinations, knowledge cutoffs, and potential biases

### Neural Networks vs Large Language Models (LLMs)

Understanding the relationship and differences between Neural Networks and LLMs is fundamental to AI comprehension:

#### Neural Networks (The Foundation)
**Neural Networks** are the basic computational framework inspired by biological brain structures:

- **Structure**: Composed of interconnected nodes (neurons) organized in layers
- **Function**: Learn patterns by adjusting weights between connections
- **Types**: Feedforward, Convolutional (CNNs), Recurrent (RNNs), etc.
- **Applications**: Image recognition, classification, regression, clustering
- **Scale**: Can range from simple networks with dozens of parameters to complex ones with millions

#### Large Language Models (The Specialized Application)
**LLMs** are a specific type of neural network designed for language tasks:

- **Architecture**: Built on transformer neural networks (attention-based)
- **Scale**: Contain billions to trillions of parameters
- **Training**: Trained on massive text datasets to understand language patterns
- **Purpose**: Specifically designed for natural language processing and generation
- **Capabilities**: Text understanding, generation, reasoning, and conversation

#### Key Differences:

| Aspect | Neural Networks | Large Language Models |
|--------|----------------|----------------------|
| **Scope** | General computational framework | Specialized for language tasks |
| **Size** | Variable (small to large) | Extremely large (billions+ parameters) |
| **Input/Output** | Various data types | Primarily text (tokens) |
| **Training Data** | Task-specific datasets | Massive text corpora |
| **Architecture** | Multiple types available | Primarily transformer-based |
| **Applications** | Broad (vision, audio, games, etc.) | Language-focused tasks |

#### The Relationship:
- **LLMs ARE Neural Networks**: LLMs are a subset of neural networks
- **Specialization**: LLMs are neural networks specialized for language
- **Evolution**: LLMs represent the evolution of neural network architectures for NLP
- **Scale**: LLMs push neural networks to unprecedented scales

#### Practical Implications:
- **Neural Networks**: Used for diverse AI tasks (image recognition, game playing, robotics)
- **LLMs**: Specifically excel at language tasks (chatbots, writing assistance, code generation)
- **Integration**: Modern AI systems often combine different types of neural networks
- **Future**: Both continue to evolve, with LLMs becoming more multimodal

### AI is Always Stateless

One of the fundamental concepts when working with AI models is understanding their **stateless nature**:

#### What Does Stateless Mean?
- **No Memory Between Calls**: Each interaction with an AI model is independent
- **Fresh Start**: Every request begins with a clean slate
- **No Context Retention**: The model doesn't remember previous conversations or interactions
- **Isolation**: Each API call or prompt is processed in isolation

#### How Stateless AI Works:
```
Request 1: "What's the weather like?" → AI responds
Request 2: "What about tomorrow?" → AI has NO memory of Request 1
```

#### Implications for AI Development:

**✅ What This Means:**
- Each prompt must contain all necessary context
- Previous conversation history must be explicitly provided
- State management is the developer's responsibility
- Consistent behavior across identical inputs

**❌ What AI Models DON'T Do:**
- Remember your name from earlier conversations
- Learn from previous interactions during inference
- Maintain session data automatically
- Update their knowledge from conversations

#### Managing State in AI Applications:

1. **Context Windows**: Include relevant conversation history in each request
2. **Session Management**: Store conversation history on the client/server side
3. **Prompt Engineering**: Craft prompts that include necessary background information
4. **External Memory**: Use databases or files to maintain state between interactions

#### Practical Examples:

**❌ This Won't Work:**
```
Prompt 1: "My name is John"
Prompt 2: "What's my name?" 
// AI won't remember "John"
```

**✅ This Will Work:**
```
Prompt: "My name is John. Given that my name is John, what's my name?"
// AI can respond correctly because context is provided
```

#### Benefits of Stateless Design:
- **Scalability**: Easy to distribute across multiple servers
- **Reliability**: No risk of corrupted state affecting responses
- **Predictability**: Same input always produces same output
- **Simplicity**: Easier to debug and test

#### Key Takeaway:
Always design your AI applications with the understanding that **you must explicitly manage and provide context** - the AI model will never remember anything from previous interactions unless you tell it again.

### Agent Loop: The Core Pattern of AI Agents

An **Agent Loop** is the fundamental operational pattern that enables AI agents to perform complex, multi-step tasks autonomously. It's the iterative cycle that transforms a stateless AI model into an intelligent agent capable of reasoning, acting, and learning from feedback.

#### What is an Agent Loop?

The Agent Loop is a continuous cycle where an AI agent:
1. **Observes** the current state/environment
2. **Thinks** about what action to take
3. **Acts** by executing tools or commands
4. **Reflects** on the results
5. **Repeats** until the goal is achieved

#### Core Components of an Agent Loop:

```
┌─────────────────────────────────────┐
│           AGENT LOOP                │
├─────────────────────────────────────┤
│                                     │
│    ┌──────────┐                    │
│    │  INPUT   │ (User Request)     │
│    └────┬─────┘                    │
│         ▼                          │
│    ┌──────────┐                    │
│    │  THINK   │ (LLM Reasoning)    │
│    └────┬─────┘                    │
│         ▼                          │
│    ┌──────────┐                    │
│    │   ACT    │ (Tool Execution)   │
│    └────┬─────┘                    │
│         ▼                          │
│    ┌──────────┐                    │
│    │ OBSERVE  │ (Result Analysis)  │
│    └────┬─────┘                    │
│         ▼                          │
│    ┌──────────┐                    │
│    │  DECIDE  │ (Continue/Stop?)   │
│    └────┬─────┘                    │
│         │                          │
│         └──────► Loop or Exit      │
│                                     │
└─────────────────────────────────────┘
```

#### How Agent Loops Work:

1. **Initialization Phase**:
   - Receive user input/task
   - Load context and available tools
   - Set up memory/state management

2. **Reasoning Phase** (THINK):
   - Analyze the current situation
   - Determine next best action
   - Plan approach using chain-of-thought

3. **Action Phase** (ACT):
   - Execute selected tool/function
   - Make API calls
   - Perform file operations
   - Run commands

4. **Observation Phase** (OBSERVE):
   - Parse tool outputs
   - Check for errors
   - Update internal state
   - Evaluate progress toward goal

5. **Decision Phase** (DECIDE):
   - Determine if goal is achieved
   - Decide whether to continue or stop
   - Plan next iteration if needed

#### Types of Agent Loops:

**1. Simple Linear Loop**:
```python
while not task_complete:
    thought = llm.think(context)
    action = llm.decide_action(thought)
    result = execute_tool(action)
    context.update(result)
```

**2. ReAct Pattern** (Reasoning + Acting):
```
Thought → Action → Observation → Thought → Action → ...
```

##### Deep Dive: ReAct Pattern (Thought → Action → Observation)

The **ReAct pattern** is one of the most powerful and widely-used agent loop architectures, combining reasoning and acting in an interleaved manner. Introduced in the paper "ReAct: Synergizing Reasoning and Acting in Language Models" (Yao et al., 2022), this pattern enables agents to generate both reasoning traces and task-specific actions.

**Core Philosophy:**
The ReAct pattern recognizes that reasoning and acting are complementary - reasoning helps the agent track goals and create plans, while acting allows it to gather information and make progress. By interleaving these, agents become more interpretable, controllable, and effective.

**The Three Phases Explained:**

1. **Thought (Reasoning Phase)**:
   - The agent analyzes the current situation
   - Decomposes complex problems into manageable steps
   - Forms hypotheses about what might work
   - Maintains awareness of the overall goal
   - Tracks what has been tried and what remains
   - Example: "I need to find the population of Paris. I should search for current demographic data."

2. **Action (Execution Phase)**:
   - The agent selects and executes a specific tool or operation
   - Actions are grounded in available tools (search, calculate, read, write, etc.)
   - Each action has clear inputs and expected outputs
   - Example: "Search: 'Paris population 2024 official statistics'"

3. **Observation (Perception Phase)**:
   - The agent receives and processes the results of its action
   - Extracts relevant information from tool outputs
   - Identifies errors or unexpected results
   - Updates its understanding of the problem space
   - Example: "Search returned: Paris population is 2.16 million in city proper, 10.9 million in urban area (2024)"

**Why ReAct Works Better Than Simple Action-Only Loops:**

| Aspect | Action-Only Loop | ReAct Pattern |
|--------|-----------------|---------------|
| **Interpretability** | Black box decisions | Clear reasoning traces |
| **Error Recovery** | Limited adaptation | Can reason about failures |
| **Complex Tasks** | Struggles with multi-step problems | Maintains problem decomposition |
| **Hallucination** | More prone to making up facts | Grounds responses in observations |
| **Debugging** | Difficult to trace issues | Clear thought progression |

**ReAct Implementation Strategy:**

```plaintext
LOOP until task_complete or max_iterations:
    THOUGHT: Analyze situation and plan next step
    ACTION: Execute specific tool with parameters
    OBSERVATION: Process tool output and update understanding

    IF observation contains answer:
        THOUGHT: "I now have the answer"
        RETURN final_answer
    ELSE:
        CONTINUE loop with new context
```

**Practical Example - Research Task:**

```
User Query: "What was the GDP growth rate of Japan in 2023 compared to 2022?"

Iteration 1:
THOUGHT: I need to find Japan's GDP data for both 2022 and 2023 to calculate the growth rate.
ACTION: Search("Japan GDP 2022 2023 official statistics")
OBSERVATION: Found - Japan's GDP was $4.23 trillion in 2022 and $4.41 trillion in 2023.

Iteration 2:
THOUGHT: Now I have both values. I need to calculate the growth rate: ((2023-2022)/2022)*100
ACTION: Calculate((4.41 - 4.23) / 4.23 * 100)
OBSERVATION: Result is 4.26%

Iteration 3:
THOUGHT: I have calculated the growth rate. Japan's GDP grew by 4.26% from 2022 to 2023.
ACTION: Respond("Japan's GDP growth rate in 2023 compared to 2022 was 4.26%")
```

**Advanced ReAct Patterns:**

1. **ReAct with Self-Reflection**:
   - Add periodic reflection thoughts to assess progress
   - "Am I closer to the goal? Should I try a different approach?"

2. **Hierarchical ReAct**:
   - High-level thoughts for strategy
   - Low-level thoughts for tactical execution
   - Allows handling of more complex, nested tasks

3. **Multi-Agent ReAct**:
   - Multiple agents with specialized roles
   - Shared observations but independent thoughts
   - Collaborative problem-solving

**Common Pitfalls and Solutions:**

❌ **Pitfall**: Repetitive loops (same thought-action repeatedly)
✅ **Solution**: Track previous attempts and explicitly reason about alternatives

❌ **Pitfall**: Over-reasoning without taking action
✅ **Solution**: Limit thought length and enforce action after reasoning

❌ **Pitfall**: Ignoring negative observations
✅ **Solution**: Explicitly prompt for failure analysis in thought phase

❌ **Pitfall**: Losing track of the original goal
✅ **Solution**: Include goal reminder in each thought prompt

**When to Use ReAct Pattern:**

✅ **Best For:**
- Multi-step research and analysis tasks
- Troubleshooting and debugging scenarios
- Tasks requiring exploration and adaptation
- When interpretability is important
- Complex decision-making with multiple factors

❌ **Not Ideal For:**
- Simple, single-step operations
- Pure creative tasks without clear goals
- Real-time systems with strict latency requirements
- Tasks with predetermined linear workflows

**ReAct vs Other Patterns:**

- **vs Chain-of-Thought (CoT)**: ReAct adds actions and observations, not just reasoning
- **vs Tool Use Only**: ReAct adds explicit reasoning about tool selection and results
- **vs Plan-then-Execute**: ReAct interleaves planning and execution for better adaptation
- **vs Reflexion**: ReAct focuses on forward progress, while Reflexion emphasizes learning from mistakes

**Key Insight:**
The ReAct pattern's power comes from making the agent's thinking process explicit and grounded in real observations. This creates a virtuous cycle where better reasoning leads to better actions, which provide better observations, which inform better reasoning.

**3. Plan-Execute Loop**:
```
Create Plan → Execute Step 1 → Check → Execute Step 2 → ... → Complete
```

**4. Reflexive Loop with Memory**:
```
Think → Act → Observe → Reflect → Store Memory → Think (with memory) → ...
```

#### Key Concepts in Agent Loops:

**Tool Integration**:
- Agents call external tools (search, file operations, APIs)
- Tools extend agent capabilities beyond text generation
- Results feed back into the reasoning process

**Context Management**:
- Maintain conversation history
- Track completed actions
- Store intermediate results
- Manage token limits

**Error Handling**:
- Retry failed actions
- Adjust strategy based on errors
- Graceful degradation
- User feedback integration

**Termination Conditions**:
- Goal achievement
- Maximum iterations reached
- Error threshold exceeded
- User intervention

#### Practical Implementation Examples:

**Python Example:**
```python
class AgentLoop:
    def __init__(self, llm, tools, max_iterations=10):
        self.llm = llm
        self.tools = tools
        self.max_iterations = max_iterations
        self.memory = []

    def run(self, task):
        iteration = 0
        while iteration < self.max_iterations:
            # Think: Generate next action
            context = self.build_context(task, self.memory)
            action = self.llm.generate_action(context)

            # Act: Execute the action
            if action.type == "tool_use":
                result = self.tools[action.tool].execute(action.params)
            elif action.type == "response":
                return action.content

            # Observe: Process result
            self.memory.append({
                "action": action,
                "result": result
            })

            # Decide: Check if task is complete
            if self.is_task_complete(result):
                return self.generate_final_response()

            iteration += 1

        return "Max iterations reached"
```

**C# Implementation Examples:**

**1. Basic Agent Loop in C#:**
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AgentLoop
{
    private readonly ILanguageModel _llm;
    private readonly Dictionary<string, ITool> _tools;
    private readonly int _maxIterations;
    private readonly List<AgentMemory> _memory;

    public AgentLoop(ILanguageModel llm, Dictionary<string, ITool> tools, int maxIterations = 10)
    {
        _llm = llm;
        _tools = tools;
        _maxIterations = maxIterations;
        _memory = new List<AgentMemory>();
    }

    public async Task<string> RunAsync(string task)
    {
        int iteration = 0;

        while (iteration < _maxIterations)
        {
            // Think: Generate next action
            var context = BuildContext(task, _memory);
            var action = await _llm.GenerateActionAsync(context);

            // Act: Execute the action
            string result;
            if (action.Type == ActionType.ToolUse)
            {
                result = await _tools[action.ToolName].ExecuteAsync(action.Parameters);
            }
            else if (action.Type == ActionType.Response)
            {
                return action.Content;
            }
            else
            {
                result = "Unknown action type";
            }

            // Observe: Process result
            _memory.Add(new AgentMemory
            {
                Action = action,
                Result = result,
                Timestamp = DateTime.UtcNow
            });

            // Decide: Check if task is complete
            if (await IsTaskCompleteAsync(result))
            {
                return await GenerateFinalResponseAsync();
            }

            iteration++;
        }

        return "Maximum iterations reached";
    }

    private string BuildContext(string task, List<AgentMemory> memory)
    {
        var context = $"Task: {task}\n\n";
        context += "Previous actions:\n";

        foreach (var item in memory)
        {
            context += $"- {item.Action.Description}: {item.Result}\n";
        }

        return context;
    }

    private async Task<bool> IsTaskCompleteAsync(string result)
    {
        // Implement task completion logic
        var prompt = $"Based on this result: {result}, is the task complete? (yes/no)";
        var response = await _llm.GenerateAsync(prompt);
        return response.Contains("yes", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<string> GenerateFinalResponseAsync()
    {
        var summary = "Task completed. Actions performed:\n";
        foreach (var item in _memory)
        {
            summary += $"- {item.Action.Description}\n";
        }
        return summary;
    }
}

// Supporting classes and interfaces
public interface ILanguageModel
{
    Task<string> GenerateAsync(string prompt);
    Task<AgentAction> GenerateActionAsync(string context);
}

public interface ITool
{
    Task<string> ExecuteAsync(Dictionary<string, object> parameters);
}

public class AgentAction
{
    public ActionType Type { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string ToolName { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}

public class AgentMemory
{
    public AgentAction Action { get; set; }
    public string Result { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum ActionType
{
    ToolUse,
    Response,
    Reasoning
}
```

**2. ReAct Pattern Implementation in C#:**
```csharp
public class ReActAgent
{
    private readonly ILanguageModel _llm;
    private readonly ToolRegistry _toolRegistry;
    private readonly StringBuilder _thoughtLog;

    public ReActAgent(ILanguageModel llm, ToolRegistry toolRegistry)
    {
        _llm = llm;
        _toolRegistry = toolRegistry;
        _thoughtLog = new StringBuilder();
    }

    public async Task<string> ProcessAsync(string objective)
    {
        const int maxSteps = 15;
        var observations = new List<string>();

        for (int step = 0; step < maxSteps; step++)
        {
            // Reasoning step
            var thought = await ThinkAsync(objective, observations);
            _thoughtLog.AppendLine($"Thought {step + 1}: {thought}");

            // Check if we have a final answer
            if (thought.StartsWith("Final Answer:"))
            {
                return thought.Replace("Final Answer:", "").Trim();
            }

            // Action step
            var action = await DecideActionAsync(thought);
            _thoughtLog.AppendLine($"Action {step + 1}: {action}");

            // Observation step
            var observation = await ExecuteActionAsync(action);
            observations.Add(observation);
            _thoughtLog.AppendLine($"Observation {step + 1}: {observation}");
        }

        return "Could not complete the objective within the step limit.";
    }

    private async Task<string> ThinkAsync(string objective, List<string> observations)
    {
        var prompt = $@"
Objective: {objective}

Previous observations:
{string.Join("\n", observations)}

What should I do next? Think step by step.
If you have enough information to answer, start with 'Final Answer:'";

        return await _llm.GenerateAsync(prompt);
    }

    private async Task<string> DecideActionAsync(string thought)
    {
        var prompt = $@"
Based on this thought: {thought}

Available tools: {string.Join(", ", _toolRegistry.GetToolNames())}

What action should I take? Format: ToolName: parameters";

        return await _llm.GenerateAsync(prompt);
    }

    private async Task<string> ExecuteActionAsync(string action)
    {
        var parts = action.Split(':', 2);
        if (parts.Length != 2)
        {
            return "Invalid action format";
        }

        var toolName = parts[0].Trim();
        var parameters = parts[1].Trim();

        if (_toolRegistry.HasTool(toolName))
        {
            return await _toolRegistry.ExecuteToolAsync(toolName, parameters);
        }

        return $"Tool '{toolName}' not found";
    }

    public string GetThoughtLog() => _thoughtLog.ToString();
}
```

**3. Advanced Agent with State Management in C#:**
```csharp
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class StatefulAgent : IDisposable
{
    private readonly ILanguageModel _llm;
    private readonly IToolExecutor _toolExecutor;
    private readonly ConcurrentQueue<AgentTask> _taskQueue;
    private readonly AgentState _state;
    private readonly CancellationTokenSource _cts;
    private readonly SemaphoreSlim _semaphore;

    public StatefulAgent(ILanguageModel llm, IToolExecutor toolExecutor)
    {
        _llm = llm;
        _toolExecutor = toolExecutor;
        _taskQueue = new ConcurrentQueue<AgentTask>();
        _state = new AgentState();
        _cts = new CancellationTokenSource();
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async Task<AgentResult> ExecuteTaskAsync(string task, CancellationToken cancellationToken = default)
    {
        var agentTask = new AgentTask
        {
            Id = Guid.NewGuid(),
            Description = task,
            Status = TaskStatus.Pending
        };

        _taskQueue.Enqueue(agentTask);

        try
        {
            await _semaphore.WaitAsync(cancellationToken);
            return await ProcessTaskAsync(agentTask, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<AgentResult> ProcessTaskAsync(AgentTask task, CancellationToken cancellationToken)
    {
        task.Status = TaskStatus.InProgress;
        _state.CurrentTask = task;

        var result = new AgentResult { TaskId = task.Id };
        var iterations = 0;
        const int maxIterations = 20;

        while (iterations < maxIterations && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Planning phase
                var plan = await PlanNextStepAsync(task, _state);
                _state.AddPlan(plan);

                // Execution phase
                var stepResult = await ExecuteStepAsync(plan);
                _state.AddResult(stepResult);

                // Reflection phase
                var reflection = await ReflectOnProgressAsync(task, stepResult);

                if (reflection.IsComplete)
                {
                    task.Status = TaskStatus.Completed;
                    result.Success = true;
                    result.Output = reflection.FinalOutput;
                    break;
                }

                if (reflection.RequiresAdjustment)
                {
                    await AdjustStrategyAsync(reflection.Feedback);
                }

                iterations++;
            }
            catch (Exception ex)
            {
                task.Status = TaskStatus.Failed;
                result.Success = false;
                result.Error = ex.Message;
                _state.AddError(ex);

                // Attempt recovery
                if (await CanRecoverAsync(ex))
                {
                    continue;
                }
                break;
            }
        }

        if (iterations >= maxIterations)
        {
            result.Success = false;
            result.Error = "Maximum iterations exceeded";
        }

        _state.CurrentTask = null;
        return result;
    }

    private async Task<Plan> PlanNextStepAsync(AgentTask task, AgentState state)
    {
        var context = new PlanningContext
        {
            Task = task.Description,
            PreviousActions = state.GetRecentActions(),
            AvailableTools = _toolExecutor.GetAvailableTools(),
            CurrentState = state.GetSnapshot()
        };

        var prompt = BuildPlanningPrompt(context);
        var response = await _llm.GenerateAsync(prompt);

        return ParsePlan(response);
    }

    private async Task<StepResult> ExecuteStepAsync(Plan plan)
    {
        var result = new StepResult { PlanId = plan.Id };

        foreach (var action in plan.Actions)
        {
            var actionResult = action.Type switch
            {
                ActionType.ToolUse => await _toolExecutor.ExecuteToolAsync(action.ToolName, action.Parameters),
                ActionType.Reasoning => await _llm.GenerateAsync(action.Prompt),
                ActionType.Memory => StoreInMemory(action.Data),
                _ => "Unknown action type"
            };

            result.ActionResults.Add(actionResult);
        }

        return result;
    }

    private async Task<Reflection> ReflectOnProgressAsync(AgentTask task, StepResult result)
    {
        var prompt = $@"
Task: {task.Description}
Recent result: {result.Summary}
State: {_state.GetSnapshot()}

Questions:
1. Is the task complete?
2. Are we making progress?
3. Should we adjust our approach?

Provide a JSON response with: isComplete, requiresAdjustment, feedback, finalOutput";

        var response = await _llm.GenerateAsync(prompt);
        return ParseReflection(response);
    }

    private string BuildPlanningPrompt(PlanningContext context)
    {
        return $@"
Current task: {context.Task}

Previous actions:
{string.Join("\n", context.PreviousActions)}

Available tools:
{string.Join("\n", context.AvailableTools)}

Current state:
{context.CurrentState}

Generate the next step plan. Include specific actions and expected outcomes.";
    }

    // Helper methods and classes
    private Plan ParsePlan(string response)
    {
        // Implementation for parsing LLM response into Plan object
        return new Plan();
    }

    private Reflection ParseReflection(string response)
    {
        // Implementation for parsing LLM response into Reflection object
        return new Reflection();
    }

    private string StoreInMemory(object data)
    {
        _state.Memory.Add(data);
        return "Stored in memory";
    }

    private async Task<bool> CanRecoverAsync(Exception ex)
    {
        // Implement recovery logic
        return false;
    }

    private async Task AdjustStrategyAsync(string feedback)
    {
        _state.StrategyAdjustments.Add(feedback);
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _semaphore?.Dispose();
    }
}

// Supporting classes
public class AgentState
{
    public AgentTask CurrentTask { get; set; }
    public List<Plan> Plans { get; } = new();
    public List<StepResult> Results { get; } = new();
    public List<object> Memory { get; } = new();
    public List<Exception> Errors { get; } = new();
    public List<string> StrategyAdjustments { get; } = new();

    public void AddPlan(Plan plan) => Plans.Add(plan);
    public void AddResult(StepResult result) => Results.Add(result);
    public void AddError(Exception ex) => Errors.Add(ex);
    public List<string> GetRecentActions() => Results.TakeLast(5).Select(r => r.Summary).ToList();
    public string GetSnapshot() => $"Plans: {Plans.Count}, Results: {Results.Count}, Errors: {Errors.Count}";
}

public class AgentTask
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
}

public class AgentResult
{
    public Guid TaskId { get; set; }
    public bool Success { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
}

public class Plan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<PlannedAction> Actions { get; set; } = new();
}

public class PlannedAction
{
    public ActionType Type { get; set; }
    public string ToolName { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public string Prompt { get; set; }
    public object Data { get; set; }
}

public class StepResult
{
    public Guid PlanId { get; set; }
    public List<string> ActionResults { get; } = new();
    public string Summary => string.Join("; ", ActionResults);
}

public class Reflection
{
    public bool IsComplete { get; set; }
    public bool RequiresAdjustment { get; set; }
    public string Feedback { get; set; }
    public string FinalOutput { get; set; }
}
```

**4. Usage Example with Dependency Injection:**
```csharp
// Program.cs - Setting up the agent with DI
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register LLM service (e.g., OpenAI, Azure OpenAI)
        services.AddSingleton<ILanguageModel, OpenAILanguageModel>();

        // Register tools
        services.AddSingleton<ITool, WebSearchTool>();
        services.AddSingleton<ITool, FileSystemTool>();
        services.AddSingleton<ITool, DatabaseTool>();

        // Register tool executor
        services.AddSingleton<IToolExecutor, ToolExecutor>();

        // Register agent
        services.AddScoped<StatefulAgent>();

        // Configure options
        services.Configure<AgentOptions>(options =>
        {
            options.MaxIterations = 10;
            options.TimeoutSeconds = 300;
            options.EnableLogging = true;
        });
    })
    .Build();

// Using the agent
var agent = host.Services.GetRequiredService<StatefulAgent>();
var result = await agent.ExecuteTaskAsync("Research the latest trends in AI and create a summary report");

if (result.Success)
{
    Console.WriteLine($"Task completed: {result.Output}");
}
else
{
    Console.WriteLine($"Task failed: {result.Error}");
}
```

#### Benefits of Agent Loops:

✅ **Autonomous Operation**: Agents can complete complex tasks without step-by-step human guidance
✅ **Error Recovery**: Can adapt when things go wrong
✅ **Tool Orchestration**: Coordinate multiple tools to achieve goals
✅ **Progressive Refinement**: Improve solutions through iteration
✅ **Context Awareness**: Build understanding through multiple observations

#### Challenges and Considerations:

❗ **Infinite Loops**: Need robust termination conditions
❗ **Context Limits**: Managing token limits in long conversations
❗ **Error Propagation**: Mistakes can compound across iterations
❗ **Cost Management**: Each iteration costs API calls
❗ **Latency**: Multiple rounds add up to slower responses

#### Best Practices for Agent Loops:

1. **Set Clear Termination Conditions**: Always define when to stop
2. **Implement Timeouts**: Prevent runaway loops
3. **Log Everything**: Track all actions for debugging
4. **Use Checkpoints**: Save state periodically
5. **Provide Escape Hatches**: Allow user intervention
6. **Optimize Context**: Prune unnecessary history
7. **Handle Failures Gracefully**: Plan for tool failures
8. **Monitor Resource Usage**: Track API calls and costs

#### Real-World Applications:

- **Code Generation**: Write, test, debug, and refine code iteratively
- **Research Assistants**: Search, read, summarize, and synthesize information
- **Task Automation**: Break down complex tasks and execute step-by-step
- **Problem Solving**: Explore solutions, test hypotheses, and iterate
- **Data Analysis**: Load data, analyze, visualize, and interpret results

#### Key Takeaway:
The Agent Loop transforms stateless AI models into powerful autonomous agents by providing a structured pattern for reasoning, acting, and learning from feedback. Understanding and implementing effective agent loops is crucial for building sophisticated AI applications that can handle complex, multi-step tasks.

---

*This repository contains examples and exercises from our AI Agents course, demonstrating practical applications of GPT and other AI technologies.*
