using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Code.Infrastructure.Services
{
	public class AssetsService : IAssetsService, IDisposable
	{
		private readonly Dictionary<string, AsyncOperationHandle> _assetRequests = new();
		
		private readonly Dictionary<string, List<string>> _categories = new();

	public async UniTask InitializeAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);
			Debug.Log($"{nameof(Addressables)} initialized");
		}
		catch (Exception e)
		{
			Debug.LogError($"[AssetsService] Failed to initialize Addressables: {e.Message}");
		}
	}

		public void Dispose()
		{
			CleanUp();
		}

		public void CleanUp()
		{
			foreach (var assetRequest in _assetRequests)
			{
				Addressables.Release(assetRequest.Value);
			}

			_categories.Clear();
			_assetRequests.Clear();
		}

	public void CleanUpCategory(string category)
	{
		var number = 0;

		if (_categories.TryGetValue(category, out List<string> keys))
		{
			foreach (string key in keys)
			{
				if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle))
				{
					Addressables.Release(handle);
					_assetRequests.Remove(key);
					number++;
				}
			}

			_categories.Remove(category);
		}

		Debug.Log($"Cleaned up {number} assets from category {category}");
	}

	public void Release(string key)
	{
		if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle))
		{
			Addressables.Release(handle);
			_assetRequests.Remove(key);

			foreach (var category in _categories)
			{
				category.Value.Remove(key);
			}
		}
	}

	public TAsset Load<TAsset>(string key) where TAsset : class
	{
		try
		{
			if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle))
			{
				if (handle.Status == AsyncOperationStatus.Failed)
				{
					_assetRequests.Remove(key);
					Addressables.Release(handle);
				}
				else
				{
					object result = handle.WaitForCompletion();
					if (handle.Status == AsyncOperationStatus.Failed)
					{
						_assetRequests.Remove(key);
						Addressables.Release(handle);
						return null;
					}
					return result as TAsset;
				}
			}

			handle = Addressables.LoadAssetAsync<TAsset>(key);
			object result2 = handle.WaitForCompletion();

			if (handle.Status == AsyncOperationStatus.Failed)
			{
				Addressables.Release(handle);
				return null;
			}

			_assetRequests[key] = handle;
			return result2 as TAsset;
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[AssetsService] Failed to load asset '{key}': {e.Message}");
			return null;
		}
	}

	public TComponent LoadPrefabWithComponent<TComponent>(string key) where TComponent : Component
	{
		GameObject gameObject = Load<GameObject>(key);
		return gameObject != null ? gameObject.GetComponent<TComponent>() : null;
	}

	public async UniTask<TAsset> LoadAsync<TAsset>(string key, string category = "common", CancellationToken cancellationToken = default)
		where TAsset : class
		{
			try
			{
				if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle))
				{
					if (handle.Status == AsyncOperationStatus.Failed)
					{
						_assetRequests.Remove(key);
						Addressables.Release(handle);
					}
					else
					{
						await handle.ToUniTask(cancellationToken: cancellationToken);
						
						if (handle.Status == AsyncOperationStatus.Failed)
						{
							_assetRequests.Remove(key);
							Addressables.Release(handle);
							return null;
						}
						
						return handle.Result as TAsset;
					}
				}

				handle = Addressables.LoadAssetAsync<TAsset>(key);
				await handle.ToUniTask(cancellationToken: cancellationToken);
				
				if (handle.Status == AsyncOperationStatus.Failed)
				{
					Addressables.Release(handle);
					return null;
				}

				_assetRequests[key] = handle;
				AddToCategory(category, key);
				
				return handle.Result as TAsset;
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[AssetsService] Failed to load asset '{key}': {e.Message}");
				return null;
			}
		}

		public async UniTask<TComponent> LoadAsyncForComponent<TComponent>(string key, string category = "common", CancellationToken cancellationToken = default) where TComponent : class
		{
			var prefab = await LoadAsync<GameObject>(key, category, cancellationToken);

			return prefab.TryGetComponent(out TComponent component)
				? component
				: throw new Exception($"Failed to get component {typeof(TComponent)} from prefab {key}");
		}

		public async UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference, string category = "common",
			CancellationToken cancellationToken = default) where TAsset : class
		{
			if (assetReference == null)
				throw new ArgumentNullException(nameof(assetReference));
			
			string key = assetReference.AssetGUID;
			return await LoadAsync<TAsset>(key, category, cancellationToken);
		}
				
		public async UniTask<TComponent> LoadPrefabWithComponentAsync<TComponent>(AssetReferenceT<GameObject> assetReference, string category = "common", CancellationToken cancellationToken = default) where TComponent : Component
		{
			string key = assetReference.AssetGUID;
			return await LoadPrefabWithComponentAsync<TComponent>(key, category, cancellationToken);
		}
				
		public async UniTask<TComponent> LoadPrefabWithComponentAsync<TComponent>(string key, string category = "common", CancellationToken cancellationToken = default)  where TComponent : Component
		{
			var gameObject = await LoadAsync<GameObject>(key, category, cancellationToken);
			return gameObject.GetComponent<TComponent>();
		}

		public async UniTask<TAsset[]> LoadAllAsync<TAsset>(List<string> keys) where TAsset : class
		{
			var tasks = new List<UniTask<TAsset>>(keys.Count);

			foreach (string key in keys)
				tasks.Add(LoadAsync<TAsset>(key));

			return await UniTask.WhenAll(tasks);
		}

		public List<string> GetAssetsListByLabel<TAsset>(string label)
		{
			return GetAssetsListByLabel(label, typeof(TAsset));
		}

	public List<string> GetAssetsListByLabel(string label, Type type = null)
	{
		try
		{
			AsyncOperationHandle<IList<IResourceLocation>> operationHandle = Addressables.LoadResourceLocationsAsync(label, type);

			IList<IResourceLocation> locations = operationHandle.WaitForCompletion();

			var assetKeys = new List<string>(locations.Count);

			foreach (IResourceLocation location in locations)
				assetKeys.Add(location.PrimaryKey);

			Addressables.Release(operationHandle);
			return assetKeys;
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[AssetsService] Failed to get assets list by label '{label}': {e.Message}");
			return new List<string>();
		}
	}

	public T[] GetAssetsByLabel<T>(string label) where T : class
	{
		try
		{
			var assetKeys = GetAssetsListByLabel(label);

			var result = new T[assetKeys.Count];

			for (var i = 0; i < assetKeys.Count; i++)
			{
				var assetKey = assetKeys[i];
				var asset = Load<T>(assetKey);
				result[i] = asset;
			}

			return result;
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[AssetsService] Failed to get assets by label '{label}': {e.Message}");
			return Array.Empty<T>();
		}
	}

	public async UniTask<T[]> GetAssetsByLabelAsync<T>(string label, string category = "common", CancellationToken cancellationToken = default) where T : class
	{
		try
		{
			var assetKeys = await GetAssetsListByLabelAsync(label, cancellationToken: cancellationToken);

			var result = new T[assetKeys.Count];

			for (var i = 0; i < assetKeys.Count; i++)
			{
				var assetKey = assetKeys[i];
				AddToCategory(category, assetKey);
				var asset = await LoadAsync<T>(assetKey, cancellationToken: cancellationToken);
				result[i] = asset;
			}
			
			return result;
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[AssetsService] Failed to get assets by label '{label}': {e.Message}");
			return Array.Empty<T>();
		}
	}

		public async UniTask<List<string>> GetAssetsListByLabelAsync<TAsset>(string label)
		{
			return await GetAssetsListByLabelAsync(label, typeof(TAsset));
		}

	public async UniTask<List<string>> GetAssetsListByLabelAsync(string label, Type type = null, CancellationToken cancellationToken = default)
	{
		try
		{
			AsyncOperationHandle<IList<IResourceLocation>> operationHandle = Addressables.LoadResourceLocationsAsync(label, type);

			IList<IResourceLocation> locations = await operationHandle.ToUniTask(cancellationToken: cancellationToken);

			var assetKeys = new List<string>(locations.Count);

			foreach (IResourceLocation location in locations)
				assetKeys.Add(location.PrimaryKey);

			Addressables.Release(operationHandle);
			return assetKeys;
		}
		catch (Exception e)
		{
			Debug.LogWarning($"[AssetsService] Failed to get assets list by label '{label}': {e.Message}");
			return new List<string>();
		}
	}

		public GameObject LoadAssetFromResources(string path) => Resources.Load<GameObject>(path);

		public T LoadAssetFromResources<T>(string path) where T : UnityEngine.Object => Resources.Load<T>(path);

		private void AddToCategory(string category, string key)
		{
			if (_categories.TryGetValue(category, out List<string> keys) == false)
			{
				keys = new List<string>();
				_categories.Add(category, keys);
			}

			if (!keys.Contains(key))
				keys.Add(key);
		}
	}
}
