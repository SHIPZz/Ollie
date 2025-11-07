using UnityEngine;

namespace Code.Extensions
{
	public static class ComponentExtensions
	{
		public static T GetComponentAnywhere<T>(this Component component, bool includeInactive = false) where T : Component
		{
			var found = component.GetComponent<T>();
			if (found != null)
				return found;

			found = component.GetComponentInParent<T>(includeInactive);
			if (found != null)
				return found;

			return component.GetComponentInChildren<T>(includeInactive);
		}
	}
}


