# Deep Dive: ReAct Pattern (Thought → Action → Observation)

The **ReAct pattern** is one of the most powerful and widely-used agent loop architectures, combining reasoning and acting in an interleaved manner. Introduced in the paper "ReAct: Synergizing Reasoning and Acting in Language Models" (Yao et al., 2022), this pattern enables agents to generate both reasoning traces and task-specific actions.

## Core Philosophy

The ReAct pattern recognizes that reasoning and acting are complementary - reasoning helps the agent track goals and create plans, while acting allows it to gather information and make progress. By interleaving these, agents become more interpretable, controllable, and effective.

## The Three Phases Explained

### 1. Thought (Reasoning Phase)
- The agent analyzes the current situation
- Decomposes complex problems into manageable steps
- Forms hypotheses about what might work
- Maintains awareness of the overall goal
- Tracks what has been tried and what remains
- Example: "I need to find the population of Paris. I should search for current demographic data."

### 2. Action (Execution Phase)
- The agent selects and executes a specific tool or operation
- Actions are grounded in available tools (search, calculate, read, write, etc.)
- Each action has clear inputs and expected outputs
- Example: "Search: 'Paris population 2024 official statistics'"

### 3. Observation (Perception Phase)
- The agent receives and processes the results of its action
- Extracts relevant information from tool outputs
- Identifies errors or unexpected results
- Updates its understanding of the problem space
- Example: "Search returned: Paris population is 2.16 million in city proper, 10.9 million in urban area (2024)"

## Why ReAct Works Better Than Simple Action-Only Loops

| Aspect | Action-Only Loop | ReAct Pattern |
|--------|-----------------|---------------|
| **Interpretability** | Black box decisions | Clear reasoning traces |
| **Error Recovery** | Limited adaptation | Can reason about failures |
| **Complex Tasks** | Struggles with multi-step problems | Maintains problem decomposition |
| **Hallucination** | More prone to making up facts | Grounds responses in observations |
| **Debugging** | Difficult to trace issues | Clear thought progression |

## ReAct Implementation Strategy

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

## Practical Example - Research Task

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

## C# Implementation

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

## Advanced ReAct Patterns

### 1. ReAct with Self-Reflection
- Add periodic reflection thoughts to assess progress
- "Am I closer to the goal? Should I try a different approach?"

### 2. Hierarchical ReAct
- High-level thoughts for strategy
- Low-level thoughts for tactical execution
- Allows handling of more complex, nested tasks

### 3. Multi-Agent ReAct
- Multiple agents with specialized roles
- Shared observations but independent thoughts
- Collaborative problem-solving

## Common Pitfalls and Solutions

❌ **Pitfall**: Repetitive loops (same thought-action repeatedly)
✅ **Solution**: Track previous attempts and explicitly reason about alternatives

❌ **Pitfall**: Over-reasoning without taking action
✅ **Solution**: Limit thought length and enforce action after reasoning

❌ **Pitfall**: Ignoring negative observations
✅ **Solution**: Explicitly prompt for failure analysis in thought phase

❌ **Pitfall**: Losing track of the original goal
✅ **Solution**: Include goal reminder in each thought prompt

## When to Use ReAct Pattern

### ✅ Best For:
- Multi-step research and analysis tasks
- Troubleshooting and debugging scenarios
- Tasks requiring exploration and adaptation
- When interpretability is important
- Complex decision-making with multiple factors

### ❌ Not Ideal For:
- Simple, single-step operations
- Pure creative tasks without clear goals
- Real-time systems with strict latency requirements
- Tasks with predetermined linear workflows

## ReAct vs Other Patterns

- **vs Chain-of-Thought (CoT)**: ReAct adds actions and observations, not just reasoning
- **vs Tool Use Only**: ReAct adds explicit reasoning about tool selection and results
- **vs Plan-then-Execute**: ReAct interleaves planning and execution for better adaptation
- **vs Reflexion**: ReAct focuses on forward progress, while Reflexion emphasizes learning from mistakes

## Key Insight

The ReAct pattern's power comes from making the agent's thinking process explicit and grounded in real observations. This creates a virtuous cycle where better reasoning leads to better actions, which provide better observations, which inform better reasoning.

---
[← Previous: Agent Loops](agent-loops.md) | [Back to Main README](../../README.md) | [Next: Completion vs Reasoning Models →](completion-vs-reasoning-models.md)