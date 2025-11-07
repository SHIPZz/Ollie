using System;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Code.Infrastructure.UI.MVP;

namespace Code.Features.Holidays.UI
{
    public class HolidayDebugWindowPresenter : IWindowPresenter<HolidayDebugWindowModel, HolidayDebugWindowView>, IDisposable
	{
		private HolidayDebugWindowModel _model;
		private HolidayDebugWindowView _view;
		private readonly GameConfig _config;

		public HolidayDebugWindowPresenter(IConfigService configService)
		{
			_config = configService.GetConfig<GameConfig>();
		}

		public void Initialize(HolidayDebugWindowModel model, HolidayDebugWindowView view)
		{
			_model = model;
			_view = view;
			_model.Changed += UpdateView;
			_view.PlusHour.onClick.AddListener(OnPlusHour);
			_view.MinusHour.onClick.AddListener(OnMinusHour);
			_view.Reset.onClick.AddListener(OnReset);
			_view.Reload.onClick.AddListener(OnReload);
			_view.SetHalloween.onClick.AddListener(OnSetHalloween);
			_view.SetXmas.onClick.AddListener(OnSetXmas);
		}

		public void Show()
		{
			UpdateView();
		}

		public void Hide()
		{
		}

		private void UpdateView()
		{
			_view.Info.text = _model.GetInfo();
		}

		private void OnPlusHour()
		{
			_model.AddHours(_config.DebugTimeShiftHours);
		}

		private void OnMinusHour()
		{
			_model.AddHours(-_config.DebugTimeShiftHours);
		}

		private void OnReset()
		{
			_model.ResetOffset();
		}

		private void OnReload()
		{
			_model.Reload();
		}

		private void OnSetHalloween()
		{
			_model.SetHalloween();
		}

		private void OnSetXmas()
		{
			_model.SetXmas();
		}

        public void Dispose()
        {
            if (_model != null)
                _model.Changed -= UpdateView;

            if (_view != null)
            {
                _view.PlusHour.onClick.RemoveListener(OnPlusHour);

                _view.MinusHour.onClick.RemoveListener(OnMinusHour);

                _view.Reset.onClick.RemoveListener(OnReset);

                _view.Reload.onClick.RemoveListener(OnReload);
				
				_view.SetHalloween.onClick.RemoveListener(OnSetHalloween);

				_view.SetXmas.onClick.RemoveListener(OnSetXmas);
            }
        }
	}
}


