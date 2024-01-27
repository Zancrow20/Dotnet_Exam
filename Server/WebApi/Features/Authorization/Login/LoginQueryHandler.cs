using Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Services;
using WebApi.Services.Jwt;
using WebApi.Services.Mongo;

namespace WebApi.Features.Authorization.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<UserInfoDto, string>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly SignInManager<Domain.Entities.User> _signInManager;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IRatingRepository _ratingRepository;
    
    public LoginQueryHandler(UserManager<Domain.Entities.User> userManager,  
        SignInManager<Domain.Entities.User> signInManager,
        IJwtGenerator jwtGenerator,
        IRatingRepository ratingRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _ratingRepository = ratingRepository;
    }
    
    public async Task<Result<UserInfoDto, string>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return "User doesn't exists!";
        }
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded) 
            return "User doesn't exists!";
        
        var rating = await _ratingRepository.GetRatingByIdAsync(user.Id, cancellationToken);
            
        return new UserInfoDto
        {
            Token = _jwtGenerator.GenerateToken(user),
            Username = user.UserName!,
            Rating = rating!.Score
        };

    }
}