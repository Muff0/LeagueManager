using System.Text;
using Shared.Dto;

namespace Discord.MessageBuilders;

public class StreakMessageBuilder
{
    public static string Compose(StreakDataDto data)
    {
        if (data.TopStreakCount == 0)
            return "📊 No streak data available yet.";

        var sb = new StringBuilder();
        sb.AppendLine("🔥 **Win Streak Records**");
        sb.AppendLine();

        var topNames = FormatNames(data.TopStreak);
        var topOngoing = data.TopStreak.Any(s => s.IsOngoing);
        var topOngoingNames = FormatNames(data.TopStreak.Where(s => s.IsOngoing));
        var topBrokenNames = FormatNames(data.TopStreak.Where(s => !s.IsOngoing));

        // --- TOP STREAK ---
        if (topOngoing && data.TopStreak.All(s => s.IsOngoing))
        {
            // All top holders are currently on the streak — record is live
            sb.AppendLine($"🏆 **All-time record: {data.TopStreakCount} wins in a row** — and {topNames} {Plural(data.TopStreak.Count, "is", "are")} still going! Can anyone stop them?");
        }
        else if (topOngoing)
        {
            // Mixed: some are ongoing, some broke it previously
            sb.AppendLine($"🏆 **All-time record: {data.TopStreakCount} wins in a row**");
            sb.AppendLine($"📖 Previously achieved by {topBrokenNames}.");
            sb.AppendLine($"⚡ {topOngoingNames} {Plural(data.TopStreak.Count(s => s.IsOngoing), "is", "are")} currently matching it — the record is under threat!");
        }
        else
        {
            // Record is set, nobody is chasing it right now
            sb.AppendLine($"🏆 **All-time record: {data.TopStreakCount} wins in a row** — held by {topNames}.");
        }

        sb.AppendLine();

        // --- RUNNER-UP ---
        if (data.RunnerUpCount > 0)
        {
            var gap = data.TopStreakCount - data.RunnerUpCount;
            var ruOngoingNames = FormatNames(data.RunnerUpStreak.Where(s => s.IsOngoing));
            var ruHasOngoing = data.RunnerUpStreak.Any(s => s.IsOngoing);
            var ruAllOngoing = data.RunnerUpStreak.All(s => s.IsOngoing);
            var ruNames = FormatNames(data.RunnerUpStreak);

            if (ruHasOngoing && gap == 1)
            {
                // One win away — this is the most exciting case
                sb.AppendLine($"😤 {ruOngoingNames} {Plural(data.RunnerUpStreak.Count(s => s.IsOngoing), "is", "are")} on a {data.RunnerUpCount}-win streak — just **one win away** from the all-time record!");
            }
            else if (ruHasOngoing && gap <= 3)
            {
                sb.AppendLine($"👀 {ruOngoingNames} {Plural(data.RunnerUpStreak.Count(s => s.IsOngoing), "is", "are")} on a {data.RunnerUpCount}-win streak — only {gap} wins behind the record. Watch this space.");
            }
            else if (ruHasOngoing)
            {
                // Ongoing but not close — still worth a mention
                sb.AppendLine($"📈 Runner-up: **{data.RunnerUpCount} wins** — {ruOngoingNames} {Plural(data.RunnerUpStreak.Count(s => s.IsOngoing), "is", "are")} currently on this streak.");
                if (!ruAllOngoing)
                    sb.AppendLine($"Also previously achieved by {FormatNames(data.RunnerUpStreak.Where(s => !s.IsOngoing))}.");
            }
            else
            {
                // Runner-up is not ongoing
                if (gap == 1)
                    sb.AppendLine($"🥈 Runner-up: **{data.RunnerUpCount} wins** by {ruNames} — agonisingly close to the record!");
                else
                    sb.AppendLine($"🥈 Runner-up: **{data.RunnerUpCount} wins** by {ruNames}.");
            }
        }

        sb.AppendLine();

        // --- LONGEST ONGOING (only if different from already-mentioned ongoing) ---
        var ongoingAlreadyMentioned = data.TopStreak.Where(s => s.IsOngoing)
            .Concat(data.RunnerUpStreak.Where(s => s.IsOngoing))
            .Select(s => s.PlayerDto.Id)
            .ToHashSet();

        var unmentionedOngoing = data.LongestOngoingStreak
            .Where(s => !ongoingAlreadyMentioned.Contains(s.PlayerDto.Id))
            .ToList();

        if (unmentionedOngoing.Count > 0)
        {
            var ongoingNames = FormatNames(unmentionedOngoing);
            var count = data.LongestOngoingCount;
            sb.AppendLine($"🔥 Currently on a hot streak: {ongoingNames} with **{count} consecutive wins**. Keep an eye on them!");
        }

        return sb.ToString().TrimEnd();
    }

    private static string FormatNames(IEnumerable<Streak> streaks)
    {
        var names = streaks.Select(s => $"**{s.PlayerDto.FirstName} {s.PlayerDto.LastName}**").ToList();
        return names.Count switch
        {
            0 => "",
            1 => names[0],
            2 => $"{names[0]} and {names[1]}",
            _ => string.Join(", ", names[..^1]) + $" and {names[^1]}"
        };
    }

    private static string Plural(int count, string singular, string plural)
        => count == 1 ? singular : plural;
}
