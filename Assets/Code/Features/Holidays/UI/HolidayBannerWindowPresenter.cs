using System;
using Code.Infrastructure.UI.MVP;

namespace Code.Features.Holidays.UI
{
    public class HolidayBannerWindowPresenter : IWindowPresenter<HolidayBannerWindowModel, HolidayBannerWindowView>, IDisposable
	{
		private HolidayBannerWindowModel _model;
		private HolidayBannerWindowView _view;

		public void Initialize(HolidayBannerWindowModel model, HolidayBannerWindowView view)
		{
			_model = model;
			_view = view;
			_model.Changed += UpdateView;
		}

		public void Show()
		{
			UpdateView();
		}

		public void Hide()
		{
			_view.SetXmasVisible(false);
			_view.SetHalloweenVisible(false);
		}

		private void UpdateView()
		{
			_view.SetXmasVisible(_model.IsXmasActive());
			_view.SetHalloweenVisible(_model.IsHalloweenActive());
		}

        public void Dispose()
        {
            if (_model != null)
                _model.Changed -= UpdateView;
        }
	}
}


