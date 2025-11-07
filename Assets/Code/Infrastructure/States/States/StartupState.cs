using System;
using System.Threading;
using Code.Features.Holidays.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.States
{
    public class StartupState : IEnterState
    {
        private readonly IAssetsService _assetsService;
        private readonly IConfigService _configService;
        private readonly IStateMachine _stateMachine;
        private readonly IHolidayService _holidayService;

        public StartupState(IAssetsService assetsService, IConfigService configService,IStateMachine stateMachine, IHolidayService holidayService)
        {
            _stateMachine = stateMachine;
            _assetsService = assetsService;
            _configService = configService;
            _holidayService = holidayService;
        }

	public async UniTask EnterAsync(CancellationToken cancellationToken = default)
	{
		Debug.Log("[StartupState] 🚀 ENTER START UP STATE");
		
		try
		{
			await _assetsService.InitializeAsync(cancellationToken);
		}
		catch (Exception e)
		{
			Debug.LogError($"[StartupState] Failed to initialize AssetsService: {e.Message}");
		}
		
		try
		{
			await _configService.InitializeAsync(cancellationToken);
		}
		catch (Exception e)
		{
			Debug.LogError($"[StartupState] Failed to initialize ConfigService: {e.Message}");
		}
		
		try
		{
			await _configService.LoadConfigAsync<GameConfig>(cancellationToken);
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[StartupState] Failed to load GameConfig: {e.Message}");
		}
		
		try
		{
			_holidayService.ReloadConfig();
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[StartupState] Failed to reload holiday config: {e.Message}");
		}

		try
		{
			await _stateMachine.EnterAsync<SetupUIState>(cancellationToken);
		}
		catch (Exception e)
		{
			Debug.LogError($"[StartupState] Failed to enter SetupUIState: {e.Message}");
		}
	}
    }
}