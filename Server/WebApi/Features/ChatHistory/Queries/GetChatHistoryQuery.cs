using Contracts;
using MediatR;

namespace WebApi.Features.ChatHistory.Queries;

public class GetChatHistoryQuery : IRequest<Result<ChatHistoryDto,string>>
{
    public string GameId { get; set; }
}