using Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Services.Mongo;

namespace WebApi.Features.Authorization.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string, string>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IUserStore<Domain.Entities.User> _userStore;
    private readonly IRatingRepository _ratingRepository;

    public RegisterCommandHandler(UserManager<Domain.Entities.User> userManager,
        IUserStore<Domain.Entities.User> userStore,
        IRatingRepository ratingRepository)
    {
        _userManager = userManager;
        _userStore = userStore;
        _ratingRepository = ratingRepository;
    }

    public async Task<Result<string, string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new Domain.Entities.User();
        
        await _userStore.SetUserNameAsync(user, request.Username, cancellationToken);
        
        var checkPasswordRes = request.Password == request.ConfirmPassword;
        if (!checkPasswordRes)
            return new Result<string, string>(failure: "Пароли не совпадают!");

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return new Result<string, string>(failure: "Ошибка регистрации. Попробуйте снова!");

        var userId = (await _userManager.FindByNameAsync(request.Username))!.Id;
        
        var rating = new Domain.Entities.Rating() { Score = 1000, UserId = userId };
        await _ratingRepository.InsertRatingAsync(rating, cancellationToken);
        
        return new Result<string, string>(success: "Вы успешно зарегистрировались!");
    }
}