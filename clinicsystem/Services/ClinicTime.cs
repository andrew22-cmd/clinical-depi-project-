namespace clinicsystem.Services
{
    /// <summary>
    /// Helpers for parsing/formatting Arabic clock strings (e.g. "10:00 ص", "02:30 م"),
    /// generating time slots from a range, and mapping Arabic week-day names.
    /// </summary>
    public static class ClinicTime
    {
        public static TimeSpan? ParseArabic(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var s = value.Trim();

            bool isPm = s.Contains('م');
            bool isAm = s.Contains('ص');

            // strip arabic AM/PM markers and any latin am/pm
            var timePart = s.Replace("ص", "").Replace("م", "")
                            .Replace("AM", "", StringComparison.OrdinalIgnoreCase)
                            .Replace("PM", "", StringComparison.OrdinalIgnoreCase)
                            .Trim();

            var parts = timePart.Split(':');
            if (parts.Length < 2) return null;
            if (!int.TryParse(parts[0], out int h)) return null;
            if (!int.TryParse(parts[1], out int m)) return null;

            if (isPm && h < 12) h += 12;
            if (isAm && h == 12) h = 0;

            if (h < 0 || h > 23 || m < 0 || m > 59) return null;
            return new TimeSpan(h, m, 0);
        }

        public static string FormatArabic(TimeSpan t)
        {
            int h = t.Hours, m = t.Minutes;
            string suffix = h >= 12 ? "م" : "ص";
            int hh = h % 12;
            if (hh == 0) hh = 12;
            return $"{hh:D2}:{m:D2} {suffix}";
        }

        /// <summary>Generate slots in [from, to) at the given interval (minutes).</summary>
        public static List<TimeSpan> GenerateSlots(TimeSpan from, TimeSpan to, int stepMinutes = 30)
        {
            var list = new List<TimeSpan>();
            if (to <= from || stepMinutes <= 0) return list;
            for (var t = from; t < to; t = t.Add(TimeSpan.FromMinutes(stepMinutes)))
                list.Add(t);
            return list;
        }

        public static List<TimeSpan> ParseSlotsCsv(string? csv)
        {
            var res = new List<TimeSpan>();
            if (string.IsNullOrWhiteSpace(csv)) return res;
            foreach (var part in csv.Split(','))
            {
                var p = ParseArabic(part);
                if (p.HasValue && !res.Contains(p.Value)) res.Add(p.Value);
            }
            res.Sort();
            return res;
        }

        private static readonly Dictionary<string, DayOfWeek> DayMap = new()
        {
            { "الأحد", DayOfWeek.Sunday },
            { "الإثنين", DayOfWeek.Monday },
            { "الاثنين", DayOfWeek.Monday },
            { "الثلاثاء", DayOfWeek.Tuesday },
            { "الأربعاء", DayOfWeek.Wednesday },
            { "الخميس", DayOfWeek.Thursday },
            { "الجمعة", DayOfWeek.Friday },
            { "السبت", DayOfWeek.Saturday },
        };

        public static string TodayArabic()
        {
            var dow = DateTime.Today.DayOfWeek;
            foreach (var kv in DayMap) if (kv.Value == dow) return kv.Key;
            return "";
        }

        /// <summary>Next calendar date (today or future) matching the given Arabic week-day.</summary>
        public static DateTime NextDateFor(string? weekDay)
        {
            if (string.IsNullOrWhiteSpace(weekDay) || !DayMap.TryGetValue(weekDay.Trim(), out var dow))
                return DateTime.Today;
            var today = DateTime.Today;
            int diff = (((int)dow - (int)today.DayOfWeek) + 7) % 7;
            return today.AddDays(diff);
        }
    }
}
