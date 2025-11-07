using UnityEngine;

namespace Code.Infrastructure.UI.MVP
{
	public interface IWindowPresenter
	{
		void Show();
		void Hide();
	}

	public interface IWindowPresenter<TModel, TView> : IWindowPresenter
		where TModel : IWindowModel
		where TView : MonoBehaviour, IWindowView
	{
		void Initialize(TModel model, TView view);
	}
}


