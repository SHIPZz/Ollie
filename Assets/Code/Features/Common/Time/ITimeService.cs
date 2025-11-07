using System;

namespace Code.Features.Common.Time
{
	public interface ITimeService
	{
		DateTime UtcNow { get; }
		TimeSpan Offset { get; }
		void SetOffset(TimeSpan offset);
		void ResetOffset();
	}
}


