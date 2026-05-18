using Data.Model;
using LeagueManager.ViewModel;

namespace LeagueManager.Extensions;

public static class CommandMessageExtensions

{
    public static RankChangeRequestViewModel ToRankChangeRequestViewModel(this CommandMessage commandMessage)
    {
        return new RankChangeRequestViewModel()
        {
            Id = commandMessage.Id,
            Payload = commandMessage.Payload
        };
    }
}