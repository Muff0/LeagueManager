using Data.Model;
using Shared.Dto;

namespace Data.Extensions;

public static class SeasonExtension
{
    public static SeasonDto ToSeasonDto(this Season season)
    {
        return new SeasonDto
        {
            Id = season.Id,
            LeagoL1Key = season.LeagoL1Key,
            LeagoL2Key = season.LeagoL2Key,
            Title = season.Title,
            IsActive = season.IsActive
        };
    }
}