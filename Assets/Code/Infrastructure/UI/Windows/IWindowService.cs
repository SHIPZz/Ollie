using Code.Infrastructure.UI.MVP;
using UnityEngine;

namespace Code.Infrastructure.UI.Windows
{
	public interface IWindowService
	{
		void Bind<TView, TPresenter, TModel>()
			where TView : MonoBehaviour, IWindowView
			where TPresenter : class, IWindowPresenter<TModel, TView>
			where TModel : class, IWindowModel;

		void Open<TView>(bool onTop = false) where TView : MonoBehaviour, IWindowView;
		void Close<TView>() where TView : MonoBehaviour, IWindowView;
		bool IsOpen<TView>() where TView : MonoBehaviour, IWindowView;
		TPresenter GetPresenter<TPresenter>() where TPresenter : class, IWindowPresenter;
	}
}


