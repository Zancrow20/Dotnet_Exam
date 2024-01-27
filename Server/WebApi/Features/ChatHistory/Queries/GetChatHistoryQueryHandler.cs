using Contracts;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.ChatHistory.Queries;

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, Result<ChatHistoryDto,string>>
{
    private readonly AppDbContext _dbContext;

    public GetChatHistoryQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ChatHistoryDto, string>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var gameExists = await _dbContext.Games
            .FirstOrDefaultAsync(g => g.GameId == request.GameId,
                cancellationToken: cancellationToken) != null;
        if (!gameExists)
            return "Игры не существует!";
        var chatHistory = await _dbContext.ChatMessages
            .Where(m => m.GameId == request.GameId)
            .Select(m => new ChatMessageDto() {Username = m.From, Message = m.Message, To = m.To})
            .ToListAsync(cancellationToken: cancellationToken);
        return new ChatHistoryDto(){ChatMessageDtos = chatHistory};
    }
}