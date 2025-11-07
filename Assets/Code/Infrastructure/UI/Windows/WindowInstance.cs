using Code.Infrastructure.UI.MVP;
using UnityEngine;

namespace Code.Infrastructure.UI.Windows
{
	public readonly struct WindowInstance
	{
		public readonly MonoBehaviour View;
		public readonly IWindowPresenter Presenter;
		public readonly IWindowModel Model;

		public WindowInstance(MonoBehaviour view, IWindowPresenter presenter, IWindowModel model)
		{
			View = view;
			Presenter = presenter;
			Model = model;
		}

		public bool IsValid => View != null && Presenter != null && Model != null;
	}
}


