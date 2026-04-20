using System.Text.Json;
using OpenAI.Chat;

namespace CodeCrafters.ClaudeCode.src.Tools;

public class ReadTool
{
    public string ReadFileContents(ChatToolCall readToolCall)
    {
        using var doc = JsonDocument.Parse(readToolCall.FunctionArguments);
        if (doc.RootElement.TryGetProperty("file_path", out var pathProp)) 
        {
            var filePath = pathProp.GetString();
            if (!string.IsNullOrEmpty(filePath)) 
            {
                Console.Write(File.ReadAllText(filePath));
                return filePath;
            }

            return string.Empty;
        }

        return string.Empty;
    }
}