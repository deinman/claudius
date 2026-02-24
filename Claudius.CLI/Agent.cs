using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;

namespace Claudius.CLI;

public class Agent
{
    private const string YouColor = "\u001b[94m";
    private const string AssistantColor = "\u001b[93m";
    private const string ResetColor = "\u001b[0m";

    private readonly AnthropicClient _client;
    private readonly ToolRegistry _registry;
    private readonly string _systemPrompt;
    private readonly List<MessageParam> _conversation = new();

    public Agent(string apiKey)
    {
        _client = new AnthropicClient { ApiKey = apiKey };
        _registry = new ToolRegistry();
        _systemPrompt = _registry.BuildSystemPrompt();
    }

    public async Task RunAsync()
    {
        Console.WriteLine(_systemPrompt);

        while (true)
        {
            Console.Write($"{YouColor}You:{ResetColor}: ");
            var userInput = Console.ReadLine();
            if (userInput is null) break;

            _conversation.Add(new MessageParam
            {
                Role = Role.User,
                Content = userInput.Trim()
            });

            while (true)
            {
                var response = await CallLlmAsync();
                var toolInvocations = ToolParser.ExtractToolInvocations(response);

                if (toolInvocations.Count == 0)
                {
                    Console.WriteLine($"{AssistantColor}Assistant:{ResetColor}: {response}");
                    _conversation.Add(new MessageParam
                    {
                        Role = Role.Assistant,
                        Content = response
                    });
                    break;
                }

                _conversation.Add(new MessageParam
                {
                    Role = Role.Assistant,
                    Content = response
                });

                foreach (var (name, args) in toolInvocations)
                {
                    var tool = _registry.GetTool(name);
                    Console.WriteLine($"{name} {JsonSerializer.Serialize(args)}");

                    Dictionary<string, object>? result;
                    if (tool != null)
                    {
                        result = await tool.ExecuteAsync(args);
                    }
                    else
                    {
                        result = new Dictionary<string, object> { ["error"] = $"Unknown tool: {name}" };
                    }

                    var resultJson = JsonSerializer.Serialize(result);
                    _conversation.Add(new MessageParam
                    {
                        Role = Role.User,
                        Content = $"tool_result({resultJson})"
                    });
                }
            }
        }
    }

    private async Task<string> CallLlmAsync()
    {
        var parameters = new MessageCreateParams
        {
            Model = Model.ClaudeSonnet4_5_20250929,
            MaxTokens = 2000,
            System = _systemPrompt,
            Messages = _conversation
        };

        var response = await _client.Messages.Create(parameters);
        return string.Join("", response.Content
            .Select(block => block.Value)
            .OfType<TextBlock>()
            .Select(tb => tb.Text));
    }
}
