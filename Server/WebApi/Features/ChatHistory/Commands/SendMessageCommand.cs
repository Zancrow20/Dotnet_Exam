using Contracts;
using MediatR;

namespace WebApi.Features.ChatHistory.Commands;

public class SendMessageCommand : IRequest<bool>
{
    public string GameId { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
}