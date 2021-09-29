using System;
using System.Globalization;

namespace todo.txt.lib
{
    public enum Interval { d, w, m, y }
    public record Recurrence(DateTime Due, Interval Interval, int Period = 1)
    {
        public Recurrence Next() => Interval switch
        {
            Interval.d => new Recurrence(Due.AddDays(Period), Interval, Period),
            Interval.w => new Recurrence(Due.AddDays(Period * 7), Interval, Period),
            Interval.m => new Recurrence(Due.AddMonths(Period), Interval, Period),
            Interval.y => new Recurrence(Due.AddYears(Period), Interval, Period),
            _ => throw new NotImplementedException()
        };
    }

    public static class TodoExtensions
    {
        internal static DateTime? ParseDate(this string maybeDate)
                => DateTime.TryParseExact(maybeDate, "yyyy-MM-dd", null, DateTimeStyles.None, out var d) ? d : null;

        public static DateTime? Due(this Todo todo)
            => todo.Metadata.TryGetValue("due", out var due) ? due.ParseDate() : null;

        public static Recurrence Recurrence(this Todo todo)
        {
            // For a recurrence to exist, there must be a due date
            if (todo.Due() is not DateTime due)
            {
                return null;
            }

            if (!todo.Metadata.TryGetValue("rec", out var recStr) ||
                !Enum.TryParse<Interval>(recStr[^1..], out var rec) ||
                !int.TryParse(recStr[..^1], out var period))
            {
                return null;
            }

            return new Recurrence(due, rec, period);
        }

    }
}
