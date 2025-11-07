using Code.Features.Holidays.UI;
using Zenject;

namespace Code.Infrastructure.UI.Windows
{
	public class WindowsRegistry : IInitializable
	{
		private readonly IWindowService _windowService;

		public WindowsRegistry(IWindowService windowService)
		{
			_windowService = windowService;
		}

		public void Initialize()
		{
			_windowService.Bind<HolidayBannerWindowView, HolidayBannerWindowPresenter, HolidayBannerWindowModel>();
			_windowService.Bind<HolidayDebugWindowView, HolidayDebugWindowPresenter, HolidayDebugWindowModel>();
		}
	}
}


