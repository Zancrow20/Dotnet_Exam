using MassTransit;
using WebApi.Services.Mongo;

namespace WebApi.Services.RabbitMq;

public class GameResultsConsumer : IConsumer<RabbitMqMessage>
{
    private readonly IRatingRepository _ratingRepository;

    public GameResultsConsumer(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task Consume(ConsumeContext<RabbitMqMessage> context)
    {
        var message = context.Message;
        var winnerDelta = message.IsDraw ? 1 : 3;
        var loserDelta = message.IsDraw ? 1 : -1;

        await _ratingRepository.UpdateRatingAsync(message.WinnerId, winnerDelta, context.CancellationToken);
        await _ratingRepository.UpdateRatingAsync(message.LoserId, loserDelta, context.CancellationToken);
    }
}