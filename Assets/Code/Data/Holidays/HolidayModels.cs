using System;

namespace Code.Data.Holidays
{
    [Serializable]
    public enum HolidayTypeId
    {
        Unknown = 0,
        Halloween = 1,
        Xmas = 2,
    }

    [Serializable]
    public class HolidayPeriodDto
    {
        public string StartDate;
        public string EndDate;
        public string HolidayType;
    }

    [Serializable]
    public class HolidayScheduleDto
    {
        public HolidayPeriodDto[] HolidaySchedule;
    }

    [Serializable]
    public class HolidayPeriod
    {
        public DateTimeOffset StartUtc;
        public DateTimeOffset EndUtc;
        public HolidayTypeId Id;
    }
}