namespace WebApi.Features.ChatHistory.Queries;

public class ChatHistoryDto
{
    public List<ChatMessageDto> ChatMessageDtos { get; set; }
}

public class ChatMessageDto
{
    public string? From { get; set; }
    public string? To { get; set; }
    public string Message { get; set; }

    public bool IsSystemMessage => From == "Server" && To == null;
}