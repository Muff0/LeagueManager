using Data.Model;

namespace Data.Queries;

public class GetPlayerQuery : Query<LeagueContext, Player>
{
    public string? FirstName { get; set; }
    public string? DiscordHandle { get; set; }
    public ulong? DiscordId { get; set; }

    public override IQueryable<Player> BuildQuery(LeagueContext context)
    {
        var query = base.BuildQuery(context);

        if (DiscordId != null)
            query = query.Where(pl => pl.DiscordId == DiscordId);
        else if (DiscordHandle != null)
            query = query.Where(pl => pl.DiscordHandle == DiscordHandle);

        if (FirstName != null)
            query = query.Where(pl => pl.FirstName == FirstName);
        return query;
    }
}