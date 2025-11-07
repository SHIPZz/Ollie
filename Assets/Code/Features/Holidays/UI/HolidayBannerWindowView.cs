using Code.Infrastructure.UI.MVP;
using TMPro;
using UnityEngine;

namespace Code.Features.Holidays.UI
{
	public class HolidayBannerWindowView : MonoBehaviour, IWindowView
	{
		[SerializeField] private TextMeshProUGUI _xmas;
		[SerializeField] private TextMeshProUGUI _halloween;

		public void SetXmasVisible(bool visible)
		{
			if (_xmas != null)
				_xmas.enabled = visible;
		}

		public void SetHalloweenVisible(bool visible)
		{
			if (_halloween != null)
				_halloween.enabled = visible;
		}
	}
}


