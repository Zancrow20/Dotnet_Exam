namespace WebApi.Features.ChatHistory.Queries;

public class ChatHistoryDto
{
    public List<ChatMessageDto> ChatMessageDtos { get; set; }
}

public class ChatMessageDto
{
    public string? Username { get; set; }
    public string? To { get; set; }
    public string Message { get; set; }

    public bool IsSystemMessage => Username == "Server" && To == null;
}