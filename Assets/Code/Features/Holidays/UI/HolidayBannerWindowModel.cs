using System;
using System.Linq;
using Code.Data.Holidays;
using Code.Features.Holidays.Services;
using Code.Infrastructure.UI.MVP;

namespace Code.Features.Holidays.UI
{
    public class HolidayBannerWindowModel : IWindowModel, IDisposable
	{
		private readonly IHolidayService _holidayService;
		public event Action Changed;

        public HolidayBannerWindowModel(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        public void Initialize()
        {
            _holidayService.ActiveHolidaysChanged += OnChanged;
        }

		public bool IsXmasActive()
		{
			return _holidayService.ActiveHolidays.Contains(HolidayTypeId.Xmas);
		}

		public bool IsHalloweenActive()
		{
			return _holidayService.ActiveHolidays.Contains(HolidayTypeId.Halloween);
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


