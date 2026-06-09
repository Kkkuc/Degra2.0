using WebApplication.DTOs.Scheduler;

namespace WebApplication.Models.SchedulerSlots;

public static class ScheduleTimeSlots
{
    public static IReadOnlyList<ScheduleTimeSlotDto> All { get; } =
    [
        new(
            Index: 0,
            StartTime: new TimeSpan(8, 30, 0),
            EndTime: new TimeSpan(9, 15, 0)),

        new(
            Index: 1,
            StartTime: new TimeSpan(9, 15, 0),
            EndTime: new TimeSpan(10, 0, 0)),

        new(
            Index: 2,
            StartTime: new TimeSpan(10, 15, 0),
            EndTime: new TimeSpan(11, 0, 0)),

        new(
            Index: 3,
            StartTime: new TimeSpan(11, 0, 0),
            EndTime: new TimeSpan(11, 45, 0)),

        new(
            Index: 4,
            StartTime: new TimeSpan(12, 0, 0),
            EndTime: new TimeSpan(12, 45, 0)),

        new(
            Index: 5,
            StartTime: new TimeSpan(12, 45, 0),
            EndTime: new TimeSpan(13, 30, 0)),

        new(
            Index: 6,
            StartTime: new TimeSpan(14, 0, 0),
            EndTime: new TimeSpan(14, 45, 0)),

        new(
            Index: 7,
            StartTime: new TimeSpan(14, 45, 0),
            EndTime: new TimeSpan(15, 30, 0)),

        new(
            Index: 8,
            StartTime: new TimeSpan(16, 0, 0),
            EndTime: new TimeSpan(16, 45, 0)),

        new(
            Index: 9,
            StartTime: new TimeSpan(16, 45, 0),
            EndTime: new TimeSpan(17, 30, 0)),

        new(
            Index: 10,
            StartTime: new TimeSpan(17, 40, 0),
            EndTime: new TimeSpan(18, 25, 0)),

        new(
            Index: 11,
            StartTime: new TimeSpan(18, 25, 0),
            EndTime: new TimeSpan(19, 10, 0)),

        new(
            Index: 12,
            StartTime: new TimeSpan(19, 20, 0),
            EndTime: new TimeSpan(20, 5, 0)),

        new(
            Index: 13,
            StartTime: new TimeSpan(20, 5, 0),
            EndTime: new TimeSpan(20, 50, 0))
    ];

    public static int? FindIndexByStartTime(TimeSpan startTime)
    {
        return All
            .FirstOrDefault(slot => slot.StartTime == startTime)
            ?.Index;
    }

    public static int? CalculateDurationInSlots(
        TimeSpan startTime,
        TimeSpan endTime)
    {
        var startSlot = All
            .FirstOrDefault(slot => slot.StartTime == startTime);

        var endSlot = All
            .FirstOrDefault(slot => slot.EndTime == endTime);

        if (startSlot is null || endSlot is null)
        {
            return null;
        }

        if (endSlot.Index < startSlot.Index)
        {
            return null;
        }

        return endSlot.Index - startSlot.Index + 1;
    }
}