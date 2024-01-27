namespace Contracts;

public record GameResult(UserMove? Winner, UserMove? Loser, string Message, bool IsDraw);