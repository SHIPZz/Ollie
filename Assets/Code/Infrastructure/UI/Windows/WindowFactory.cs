using Code.Infrastructure.UI.MVP;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.UI.Windows
{
	public class WindowFactory : IWindowFactory
	{
		private readonly IInstantiator _instantiator;

		public WindowFactory(IInstantiator instantiator)
		{
			_instantiator = instantiator;
		}

		public WindowInstance Create<TView, TPresenter, TModel>(GameObject prefab)
			where TView : MonoBehaviour, IWindowView
			where TPresenter : class, IWindowPresenter<TModel, TView>
			where TModel : class, IWindowModel
		{
			TView view;
			if (prefab != null)
			{
				view = _instantiator.InstantiatePrefabForComponent<TView>(prefab);
			}

			else
			{
				var go = new GameObject(typeof(TView).Name);

				view = go.AddComponent<TView>();
			}

			var model = _instantiator.Instantiate<TModel>();

			var presenter = _instantiator.Instantiate<TPresenter>();

			return new WindowInstance(view, presenter, model);
		}
	}
}


