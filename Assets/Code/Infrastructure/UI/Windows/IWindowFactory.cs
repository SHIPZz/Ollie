using Code.Infrastructure.UI.MVP;
using UnityEngine;

namespace Code.Infrastructure.UI.Windows
{
	public interface IWindowFactory
	{
		WindowInstance Create<TView, TPresenter, TModel>(GameObject prefab)
			where TView : MonoBehaviour, IWindowView
			where TPresenter : class, IWindowPresenter<TModel, TView>
			where TModel : class, IWindowModel;
	}
}


