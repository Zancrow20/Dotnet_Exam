namespace Domain.Entities;

public enum Status 
{
    New,
    Started,
    Finished
}

public enum Figure
{
    Rock,
    Scissors,
    Paper
}

public class Game
{
    public string? GameId { get; set; }
    public string? Owner { get; set; }
    public string? OwnerName { get; set; }
    public string? PlayerOne { get; set; }
    public string? PlayerTwo { get; set; }
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public int MaxRating { get; set; }
}