using DataAccess;
using Domain.Entities;
using MediatR;

namespace WebApi.Features.ChatHistory.Commands;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public SendMessageCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new ChatMessage()
        {
            GameId = request.GameId,
            Message = request.Message,
            From = request.Username,
            To = "Server"
        };
        _dbContext.ChatMessages.Add(message);
        return await _dbContext.SaveChangesAsync(cancellationToken) >= 1;
    }
}