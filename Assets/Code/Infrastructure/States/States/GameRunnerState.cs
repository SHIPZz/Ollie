using System;
using System.Threading;
using Code.Features.Holidays.UI;
using Code.Infrastructure.Services;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.UI.Windows;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.States
{
    public class GameRunnerState : IEnterState, IExitableState, IDisposable
    {
        private readonly IWindowService _windowService;
        private readonly IAssetsService _assetsService;

        public GameRunnerState(IWindowService windowService, IAssetsService assetsService)
        {
            _windowService = windowService;
            _assetsService = assetsService;
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _windowService.Open<HolidayDebugWindowView>(onTop: true);
#endif

            _windowService.Open<HolidayBannerWindowView>();
            await UniTask.CompletedTask;
        }

        public async UniTask ExitAsync(CancellationToken cancellationToken = default)
        {
            Cleanup();

            await UniTask.CompletedTask;
        }

        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            _windowService.Close<HolidayDebugWindowView>();
#endif

            _windowService.Close<HolidayBannerWindowView>();
            _assetsService.CleanUp();
        }
    }
}