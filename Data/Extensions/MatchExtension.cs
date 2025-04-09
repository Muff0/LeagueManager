using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Dto;

namespace Data
{
    public static class MatchExtension
    {
        public static MatchDto ToMatchDto(this Match match)
        {
            return new MatchDto()
            {
                Id = match.Id,
                LeagoKey = match.LeagoKey,
                ScheduleTime = match.GameTimeUTC.GetValueOrDefault(),
                GameLink = match.Link,
                IsPlayed = match.IsComplete,
                Players = match.PlayerMatches?.Select(pm => pm.ToPlayerMatchDto()).ToArray(),
            };
        }
    }
}
