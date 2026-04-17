using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

if (args.Length < 2 || args[0] != "-p")
{
    throw new Exception("Usage: program -p <prompt>");
}

var prompt = args[1];

if (string.IsNullOrEmpty(prompt))
{
    throw new Exception("Prompt must not be empty");
}

var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
var baseUrl = Environment.GetEnvironmentVariable("OPENROUTER_BASE_URL") ?? "https://openrouter.ai/api/v1";

if (string.IsNullOrEmpty(apiKey))
{
    throw new Exception("OPENROUTER_API_KEY is not set");
}

var client = new ChatClient(
    model: "anthropic/claude-haiku-4.5",
    credential: new ApiKeyCredential(apiKey),
    options: new OpenAIClientOptions { Endpoint = new Uri(baseUrl) }
);

var chatCompletionOptions = new ChatCompletionOptions();

chatCompletionOptions.Tools.Add(ChatTool.CreateFunctionTool(

    "Read", "Read and return the contents of a file", BinaryData.FromString(@"{

        ""type"": ""object"",

        ""properties"": {

            ""file_path"": {

                ""type"": ""string"",

                ""description"": ""The path to the file to read""

            }

        },

        ""required"": [""file_path""]

    }")));

ChatCompletion response = client.CompleteChat(
    messages: [new UserChatMessage(prompt)], 
    options: chatCompletionOptions
);

if (response.Content == null || response.Content.Count == 0)
{
    throw new Exception("No choices in response");
}

Console.Error.WriteLine("Logs from your program will appear here!");

Console.Write(response.Content[0].Text);