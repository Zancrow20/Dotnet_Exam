namespace WebApi.Configuration;

public class RabbitMqConfig
{
    public static string RabbitMqConfigString = "RabbitMqConfig";
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}