using Claudius.CLI;

var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
    ?? throw new InvalidOperationException("ANTHROPIC_API_KEY environment variable is not set.");

var agent = new Agent(apiKey);
await agent.RunAsync();
