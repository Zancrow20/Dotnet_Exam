namespace WebApi.Services.RabbitMq;

public record RabbitMqMessage(string WinnerId, string LoserId, bool IsDraw);