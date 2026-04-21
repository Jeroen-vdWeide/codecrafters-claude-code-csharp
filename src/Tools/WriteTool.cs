using System.Text.Json;
using OpenAI.Chat;

namespace CodeCrafters.ClaudeCode.src.Tools;

public class WriteTool
{
    public static string Name => "Write";

    private static string WriteToolParameters =>
        """
        { 
        "type": "object",
        "required": ["file_path", "content"],
        "properties": {
            "file_path": {
            "type": "string",
            "description": "The path of the file to write to"
            },
            "content": {
            "type": "string",
            "description": "The content to write to the file"
            }
        }
        }
        """;

    public static ChatTool Tool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: "Write content to a file",
        functionSchemaIsStrict: true,
        functionParameters: BinaryData.FromString(WriteToolParameters));

    public static void WriteToFile(List<ChatMessage> messages,ChatToolCall writeToolCall)
    {
        var writeJson = JsonDocument.Parse(writeToolCall.FunctionArguments.ToString());
        var filePath = writeJson.RootElement.GetProperty("file_path").ToString();
        var content = writeJson.RootElement.GetProperty("content").ToString();

        File.WriteAllText(filePath, content);

        messages.Add(new ToolChatMessage(writeToolCall.Id, content));
    }
}