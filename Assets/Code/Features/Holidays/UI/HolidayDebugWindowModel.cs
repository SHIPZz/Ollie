using System;
using System.Linq;
using Code.Data.Holidays;
using Code.Features.Common.Time;
using Code.Features.Holidays.Services;
using Code.Infrastructure.UI.MVP;

namespace Code.Features.Holidays.UI
{
    public class HolidayDebugWindowModel : IWindowModel, IDisposable
	{
		private readonly IHolidayService _holidayService;
		private readonly ITimeService _timeService;
		public event Action Changed;

        public HolidayDebugWindowModel(IHolidayService holidayService, ITimeService timeService)
        {
            _holidayService = holidayService;
            _timeService = timeService;
        }

        public void Initialize()
        {
            _holidayService.ActiveHolidaysChanged += OnChanged;
        }

		public void AddHours(int hours)
		{
			var offset = _timeService.Offset + TimeSpan.FromHours(hours);
			_timeService.SetOffset(offset);
			_holidayService.Evaluate();
			OnChanged();
		}

		public void ResetOffset()
		{
			_timeService.ResetOffset();
			_holidayService.Evaluate();
			OnChanged();
		}

		public void Reload()
		{
			_holidayService.ReloadConfig();
			OnChanged();
		}

		public void SetHalloween()
		{
			SetHoliday(HolidayTypeId.Halloween);
		}

		public void SetXmas()
		{
			SetHoliday(HolidayTypeId.Xmas);
		}

		private void SetHoliday(HolidayTypeId holidayTypeId)
		{
			var period = _holidayService.Periods.FirstOrDefault(p => p.Id == holidayTypeId);
			if (period == null) return;

			var targetTime = period.StartUtc;
			var offset = targetTime - DateTime.UtcNow;
			
			_timeService.SetOffset(offset);
			_holidayService.Evaluate();
			OnChanged();
		}


		public string GetInfo()
		{
			var now = _timeService.UtcNow;
			var offset = _timeService.Offset;
			var active = string.Join(", ", _holidayService.ActiveHolidays.Select(x => x.ToString()));
			return $"UTC: {now:O}\nOffset: {offset}\nActive: {active}";
		}

		private void OnChanged()
		{
			Changed?.Invoke();
		}

        public void Dispose()
        {
            _holidayService.ActiveHolidaysChanged -= OnChanged;
        }
	}
}


