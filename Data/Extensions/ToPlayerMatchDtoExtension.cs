using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Shared.Dto;

namespace Data
{
    public static class ToPlayerMatchDtoExtension
    {
        public static PlayerMatchDto ToPlayerMatchDto(this PlayerMatch playerMatch)
        {
            return new PlayerMatchDto()
            {
                PlayerId = playerMatch.PlayerId,
                MatchId = playerMatch.MatchId,
                Player = playerMatch.Player?.ToPlayerDto(),
                Match = playerMatch.Match?.ToMatchDto(),
                HasConfirmed = playerMatch.HasConfirmed,
                Color = playerMatch.Color,
                Outcome = playerMatch.Outcome,
            };
        }
    }
}
