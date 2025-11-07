using System;
using System.Collections.Generic;
using Code.Extensions;
using Code.Infrastructure.Services;
using Code.Infrastructure.UI.MVP;
using UnityEngine;

namespace Code.Infrastructure.UI.Windows
{
	public class WindowService : IWindowService
	{
		private readonly IAssetsService _assetsService;
		private readonly IWindowFactory _factory;
		private readonly Dictionary<Type, Action> _bindings = new Dictionary<Type, Action>();
		private readonly Dictionary<Type, WindowInstance> _instances = new Dictionary<Type, WindowInstance>();
		private readonly Dictionary<Type, string> _prefabKeys = new Dictionary<Type, string>();
		private int _sortingOrderCounter;

		public WindowService(IAssetsService assetsService, IWindowFactory factory)
		{
			_assetsService = assetsService;
			_factory = factory;
		}

		public void Bind<TView, TPresenter, TModel>()
			where TView : MonoBehaviour, IWindowView
			where TPresenter : class, IWindowPresenter<TModel, TView>
			where TModel : class, IWindowModel
		{
			_bindings[typeof(TView)] = () => EnsureInstance<TView, TPresenter, TModel>();
		}

		public void Open<TView>(bool onTop = false) where TView : MonoBehaviour, IWindowView
		{
			var key = typeof(TView);
			if (_bindings.TryGetValue(key, out var ensure))
				ensure();

			if (_instances.TryGetValue(key, out var pair))
			{
				pair.Presenter.Show();
				pair.View.gameObject.SetActive(true);

				if (onTop)
				{
					var canvas = pair.View.GetComponentAnywhere<Canvas>(true);

					if (canvas != null)
					{
						canvas.overrideSorting = true;
						canvas.sortingOrder = ++_sortingOrderCounter;
					}

					else
					{
						pair.View.transform.SetAsLastSibling();
					}
				}
			}
		}

	public void Close<TView>() where TView : MonoBehaviour, IWindowView
	{
		var key = typeof(TView);
		if (_instances.TryGetValue(key, out var pair))
		{
			pair.Presenter.Hide();
			pair.View.gameObject.SetActive(false);

			if (pair.Presenter is IDisposable pd)
				pd.Dispose();

			if (pair.Model is IDisposable md)
				md.Dispose();

			UnityEngine.Object.Destroy(pair.View.gameObject);

			_instances.Remove(key);

			if (_prefabKeys.TryGetValue(key, out var prefabKey))
			{
				_assetsService.Release(prefabKey);
				_prefabKeys.Remove(key);
			}
		}
	}

		public bool IsOpen<TView>() where TView : MonoBehaviour, IWindowView
		{
			var key = typeof(TView);
			return _instances.TryGetValue(key, out var pair) && pair.View.gameObject.activeSelf;
		}

		public TPresenter GetPresenter<TPresenter>() where TPresenter : class, IWindowPresenter
		{
			foreach (var kv in _instances)
				if (kv.Value.Presenter is TPresenter p)
					return p;
			return null;
		}

	private void EnsureInstance<TView, TPresenter, TModel>()
		where TView : MonoBehaviour, IWindowView
		where TPresenter : class, IWindowPresenter<TModel, TView>
		where TModel : class, IWindowModel
	{
		var key = typeof(TView);

		if (_instances.ContainsKey(key))
			return;

		try
		{
			var prefabKey = typeof(TView).Name;
			var prefab = _assetsService.Load<GameObject>(prefabKey);

			if (prefab == null)
			{
				Debug.LogWarning($"[WindowService] Failed to load prefab '{prefabKey}' for window {typeof(TView).Name}");
				return;
			}

			var result = _factory.Create<TView, TPresenter, TModel>(prefab);

			if (!result.IsValid)
			{
				Debug.LogWarning($"[WindowService] Failed to create window instance for {typeof(TView).Name}");
				return;
			}

			((TModel)result.Model).Initialize();

			((IWindowPresenter<TModel, TView>)result.Presenter).Initialize((TModel)result.Model, (TView)result.View);

			result.View.gameObject.SetActive(false);

			_instances[key] = result;
			_prefabKeys[key] = prefabKey;
		}
		catch (Exception e)
		{
			Debug.LogError($"[WindowService] Failed to ensure instance for {typeof(TView).Name}: {e.Message}");
		}
	}
	}
}


