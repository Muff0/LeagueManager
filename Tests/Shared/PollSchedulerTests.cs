using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shared.Settings;
using Shared.Services;

namespace Tests.Shared;

[TestFixture]
public class PollScheduleServiceTests
{
    private static PollSchedulerService CreateService(DayOfWeek day, int hour) =>
        new(Options.Create(new SchedulerSettings
        {
            PollPostDay = day,
            PollPostHour = hour
        }));

    // --- Normal future cases ---

    [Test]
    public void ReturnsNextOccurrence_WhenCandidateIsInFuture()
    {
        var svc = CreateService(DayOfWeek.Monday, 9);
        var lastRun  = new DateTime(2025, 1, 6, 9, 0, 0);  // Monday
        var now      = new DateTime(2025, 1, 8, 12, 0, 0); // Wednesday
        var expected = new DateTime(2025, 1, 13, 9, 0, 0); // next Monday

        svc.GetNextOccurrence(lastRun, now).Should().Be(expected);
    }

    [Test]
    public void ReturnsCorrectTime_WhenHourIsNotMidnight()
    {
        var svc = CreateService(DayOfWeek.Friday, 18);
        var lastRun  = new DateTime(2025, 1, 3, 18, 0, 0);  // Friday
        var now      = new DateTime(2025, 1, 6, 10, 0, 0);  // Monday
        var expected = new DateTime(2025, 1, 10, 18, 0, 0); // next Friday at 18:00

        svc.GetNextOccurrence(lastRun, now).Should().Be(expected);
    }

    [TestCase(DayOfWeek.Monday)]
    [TestCase(DayOfWeek.Tuesday)]
    [TestCase(DayOfWeek.Wednesday)]
    [TestCase(DayOfWeek.Thursday)]
    [TestCase(DayOfWeek.Friday)]
    [TestCase(DayOfWeek.Saturday)]
    [TestCase(DayOfWeek.Sunday)]
    public void ReturnsExactlySevenDaysLater_WhenNowIsJustAfterLastRun(DayOfWeek day)
    {
        var svc = CreateService(day, 9);

        // find a date that matches the target day
        var lastRun = GetNextWeekday(new DateTime(2025, 1, 6), day).AddHours(9);
        var now     = lastRun.AddMinutes(1);

        var result = svc.GetNextOccurrence(lastRun, now);

        result.Should().Be(lastRun.AddDays(7));
    }

    // --- Same day edge cases ---

    [Test]
    public void ReturnsNextWeek_WhenLastRunWasOnTargetDayAndHourExactly()
    {
        var svc = CreateService(DayOfWeek.Monday, 9);
        var lastRun  = new DateTime(2025, 1, 6, 9, 0, 0); // Monday 09:00
        var now      = new DateTime(2025, 1, 6, 9, 1, 0); // same day, 1 min later
        var expected = new DateTime(2025, 1, 13, 9, 0, 0);

        svc.GetNextOccurrence(lastRun, now).Should().Be(expected);
    }

    [Test]
    public void ReturnsNextWeek_WhenNowIsBeforeTargetHourOnTargetDay()
    {
        // lastRun was Monday, now is the next Monday but before the post hour
        var svc = CreateService(DayOfWeek.Monday, 9);
        var lastRun  = new DateTime(2025, 1, 6, 9, 0, 0);  // Monday
        var now      = new DateTime(2025, 1, 13, 8, 0, 0); // next Monday, 08:00 — not yet time
        var expected = new DateTime(2025, 1, 13, 9, 0, 0); // same day but at 09:00

        svc.GetNextOccurrence(lastRun, now).Should().Be(expected);
    }

    // --- Outage / catch-up cases ---

    [Test]
    public void ReturnsNow_WhenCandidateIsInPastDueToOutage()
    {
        var svc = CreateService(DayOfWeek.Monday, 9);
        var lastRun = new DateTime(2025, 1, 6, 9, 0, 0);  // Monday
        var now     = new DateTime(2025, 1, 20, 12, 0, 0); // two weeks later

        svc.GetNextOccurrence(lastRun, now).Should().Be(now);
    }

    [Test]
    public void ReturnsNow_WhenCandidateIsExactlyNow()
    {
        var svc = CreateService(DayOfWeek.Monday, 9);
        var lastRun = new DateTime(2025, 1, 6, 9, 0, 0);
        var now     = new DateTime(2025, 1, 13, 9, 0, 0); // exactly the candidate

        // candidate <= now → returns now
        svc.GetNextOccurrence(lastRun, now).Should().Be(now);
    }

    // --- Hour boundary ---

    [Test]
    public void ReturnsCorrectHour_WhenPostHourIsMidnight()
    {
        var svc = CreateService(DayOfWeek.Wednesday, 0);
        var lastRun  = new DateTime(2025, 1, 8, 0, 0, 0);  // Wednesday midnight
        var now      = new DateTime(2025, 1, 9, 6, 0, 0);  // Thursday
        var expected = new DateTime(2025, 1, 15, 0, 0, 0); // next Wednesday midnight

        svc.GetNextOccurrence(lastRun, now).Should().Be(expected);
    }

    // --- Helper ---

    private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
    {
        int daysUntil = ((int)day - (int)start.DayOfWeek + 7) % 7;
        return start.AddDays(daysUntil == 0 ? 7 : daysUntil);
    }
}