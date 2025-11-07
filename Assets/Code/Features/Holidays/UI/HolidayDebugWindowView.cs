using Code.Infrastructure.UI.MVP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Features.Holidays.UI
{
	public class HolidayDebugWindowView : MonoBehaviour, IWindowView
	{
		[SerializeField] private TextMeshProUGUI _info;
		[SerializeField] private Button _plusHour;
		[SerializeField] private Button _minusHour;
		[SerializeField] private Button _reset;
		[SerializeField] private Button _reload;
		[SerializeField] private Button _setHalloween;
		[SerializeField] private Button _setXmas;

		public TextMeshProUGUI Info => _info;
		public Button PlusHour => _plusHour;
		public Button MinusHour => _minusHour;
		public Button Reset => _reset;
		public Button Reload => _reload;
		public Button SetHalloween => _setHalloween;
		public Button SetXmas => _setXmas;

	}
}


