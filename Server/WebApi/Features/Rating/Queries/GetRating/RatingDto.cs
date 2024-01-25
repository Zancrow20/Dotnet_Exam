using WebApi.Features.Shared;

namespace WebApi.Features.Rating.Queries.GetRating;

public record RatingDto(IEnumerable<UserDto> UsersRating);