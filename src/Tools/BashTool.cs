using System.Diagnostics;
using System.Text.Json;
using OpenAI.Chat;

namespace CodeCrafters.ClaudeCode.src.Tools;

public class BashTool
{
    public static string Name => "Bash";

    private static string BashToolParameters =>
    """
    {
      "type": "object",
      "required": ["command"],
      "properties": {
        "command": {
          "type": "string",
          "description": "The command to execute"
          }   
      }
    }
    """;

    public static ChatTool Tool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: "Execute a shell command",
        functionSchemaIsStrict: true,
        functionParameters: BinaryData.FromString(BashToolParameters));


    public static void ExecuteCommand(List<ChatMessage> messages, ChatToolCall bashToolCall)
    {
        var bashJson = JsonDocument.Parse(bashToolCall.FunctionArguments.ToString());
        var command = bashJson.RootElement.GetProperty("command").ToString();

        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(startInfo);

        var stdout = process?.StandardOutput.ReadToEnd();
        var stderr = process?.StandardError.ReadToEnd();

        process?.WaitForExit();

        var result = string.IsNullOrEmpty(stderr) ? stdout : $"ERROR:\n{stderr}";

        messages.Add(new ToolChatMessage(bashToolCall.Id, result));
    }
}