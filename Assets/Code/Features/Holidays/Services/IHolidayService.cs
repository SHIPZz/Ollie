using System;
using System.Collections.Generic;
using Code.Data.Holidays;

namespace Code.Features.Holidays.Services
{
	public interface IHolidayService
	{
		IReadOnlyList<HolidayTypeId> ActiveHolidays { get; }
		IReadOnlyList<HolidayPeriod> Periods { get; }
		event Action ActiveHolidaysChanged;
		void ReloadConfig();
		void Evaluate();
	}
}


