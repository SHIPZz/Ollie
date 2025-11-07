using System;

namespace Code.Features.Common.Time
{
	public class UnityTimeService : ITimeService
	{
		private TimeSpan _offset;

		public DateTime UtcNow => DateTime.UtcNow + _offset;
		public TimeSpan Offset => _offset;

		public void SetOffset(TimeSpan offset)
		{
			_offset = offset;
		}

		public void ResetOffset()
		{
			_offset = TimeSpan.Zero;
		}
	}
}


