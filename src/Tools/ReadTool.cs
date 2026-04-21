using System.Text.Json;
using OpenAI.Chat;

namespace CodeCrafters.ClaudeCode.src.Tools;

public class ReadTool
{
    public static string Name => "Read";

    private static string ReadToolParameters => 
        """
        { 
          "type": "object",
          "properties": {
           "file_path": {
             "type": "string",
             "description": "The path to the file to read"
           }
          },
          "required": ["file_path"] 
        }
        """;
    
    public static ChatTool Tool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: "Read and return the contents of a file",
        functionSchemaIsStrict: true,
        functionParameters: BinaryData.FromString(ReadToolParameters));

    public static void ReadFileContents(List<ChatMessage> messages, ChatToolCall readToolCall)
    {
        using var doc = JsonDocument.Parse(readToolCall.FunctionArguments);
        if (doc.RootElement.TryGetProperty("file_path", out var pathProp)) 
        {
            var filePath = pathProp.GetString();
            if (!string.IsNullOrEmpty(filePath)) 
            {
                var content = File.ReadAllText(filePath);

                var toolMessage = ChatMessage.CreateToolMessage(readToolCall.Id, content);
                messages.Add(toolMessage);
            }
        }
    }
}