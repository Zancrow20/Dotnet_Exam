namespace WebApi.Features.Authorization.Login;

public class UserInfoDto
{
    public string Username { get; set; }
    public int Rating { get; set; }
    public string Token { get; set; }
}