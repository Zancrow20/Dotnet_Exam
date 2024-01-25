using WebApi.Features.Shared;

namespace WebApi.Features.Rating;

public record RatingDto(IEnumerable<UserDto> UsersRating);