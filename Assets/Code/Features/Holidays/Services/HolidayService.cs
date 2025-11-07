using System;
using System.Collections.Generic;
using System.Globalization;
using Code.Data;
using Code.Data.Holidays;
using Code.Features.Common.Time;
using Code.Infrastructure.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace Code.Features.Holidays.Services
{
	public class HolidayService : IHolidayService
	{
		private readonly IAssetsService _assetsService;
		private readonly ITimeService _timeService;
		private readonly List<HolidayPeriod> _periods = new();
		private readonly List<HolidayTypeId> _active = new();
		public event Action ActiveHolidaysChanged;

		public HolidayService(IAssetsService assetsService, ITimeService timeService)
		{
			_assetsService = assetsService;
			_timeService = timeService;
		}

		public IReadOnlyList<HolidayTypeId> ActiveHolidays => _active;
		public IReadOnlyList<HolidayPeriod> Periods => _periods;

	public void ReloadConfig()
	{
		try
		{
			_periods.Clear();
			var text = LoadText();
			var dto = Parse(text);
			FillPeriods(dto);
			Evaluate();
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[HolidayService] Failed to reload config: {e.Message}");
			_periods.Clear();
			_active.Clear();
		}
	}

		public void Evaluate()
		{
			var now = _timeService.UtcNow;
			var newActive = ListPool<HolidayTypeId>.Get();
			try
			{
				foreach (var period in _periods)
				{
					if (IsPeriodActive(period, now) && !newActive.Contains(period.Id))
					{
						newActive.Add(period.Id);
					}
				}
				ApplyActive(newActive);
			}
			finally
			{
				ListPool<HolidayTypeId>.Release(newActive);
			}
		}

		private static bool IsPeriodActive(HolidayPeriod period, DateTime now)
		{
			return period.StartUtc.UtcDateTime <= now && now <= period.EndUtc.UtcDateTime;
		}

	private string LoadText()
	{
		try
		{
			var addr = _assetsService.Load<TextAsset>(AddressableKeys.Configs.HolidaySchedule);
			if (addr != null)
			{
				return addr.text;
			}

			var res = _assetsService.LoadAssetFromResources<TextAsset>("configs/holiday_schedule");
			if (res != null)
			{
				return res.text;
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[HolidayService] Failed to load holiday schedule text: {e.Message}");
		}

		return null;
	}

	private static HolidayScheduleDto Parse(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}
		
		var normalized = NormalizeKeys(text);
		try
		{
			var dto = JsonUtility.FromJson<HolidayScheduleDto>(normalized);
			return dto;
		}
		catch
		{
			return null;
		}
	}

	private void FillPeriods(HolidayScheduleDto dto)
	{
		if (dto?.HolidaySchedule == null || dto.HolidaySchedule.Length == 0)
		{
			return;
		}
		
		_periods.Clear();
		foreach (var periodDto in dto.HolidaySchedule)
		{
			if (TryParseHolidayPeriod(periodDto, out var period))
			{
				_periods.Add(period);
			}
		}
	}

		private bool TryParseHolidayPeriod(HolidayPeriodDto dto, out HolidayPeriod period)
		{
			period = default;

			if (dto == null)
			{
				return false;
			}
			if (!Enum.TryParse(dto.HolidayType, true, out HolidayTypeId type))
			{
				return false;
			}
			if (!DateTimeOffset.TryParse(dto.StartDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var start))
			{
				return false;
			}
			if (!DateTimeOffset.TryParse(dto.EndDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var end))
			{
				return false;
			}
			if (end < start)
			{
				return false;
			}

			period = new HolidayPeriod { StartUtc = start.ToUniversalTime(), EndUtc = end.ToUniversalTime(), Id = type };
			return true;
		}

		private void ApplyActive(List<HolidayTypeId> next)
		{
			var currentActiveSet = new HashSet<HolidayTypeId>(_active);
			var nextActiveSet = new HashSet<HolidayTypeId>(next);

			if (currentActiveSet.SetEquals(nextActiveSet))
			{
				return;
			}
			
			_active.Clear();
			_active.AddRange(next);
			ActiveHolidaysChanged?.Invoke();
		}

		private static string NormalizeKeys(string json)
		{
			return json
				.Replace("\"holidaySchedule\"", "\"HolidaySchedule\"")
				.Replace("\"startDate\"", "\"StartDate\"")
				.Replace("\"endDate\"", "\"EndDate\"")
				.Replace("\"holidayType\"", "\"HolidayType\"");
		}
	}
}

