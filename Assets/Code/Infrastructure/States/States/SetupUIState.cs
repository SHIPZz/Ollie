using System.Threading;
using Code.Features.Holidays.UI;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.UI.Windows;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.States
{
	public class SetupUIState : IEnterState
	{
		private readonly IWindowService _windowService;
		private readonly IStateMachine _stateMachine;

		public SetupUIState(IWindowService windowService, IStateMachine stateMachine)
		{
			_windowService = windowService;
			_stateMachine = stateMachine;
		}

		public async UniTask EnterAsync(CancellationToken cancellationToken = default)
		{
			_windowService.Bind<HolidayBannerWindowView,HolidayBannerWindowPresenter,HolidayBannerWindowModel>();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
			_windowService.Bind<HolidayDebugWindowView, HolidayDebugWindowPresenter, HolidayDebugWindowModel>();

#endif
			await _stateMachine.EnterAsync<LoadGameRunnerState>(cancellationToken);
		}
	}
}


