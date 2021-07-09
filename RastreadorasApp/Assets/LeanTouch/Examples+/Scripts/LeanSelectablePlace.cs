using UnityEngine;

namespace Lean.Touch
{
	// This script will place this GameObject on the surface of a plane when selected
	public class LeanSelectablePlace : LeanSelectableBehaviour
	{
		[Tooltip("The camera we will be used (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("The conversion method used to find a world point from a screen point")]
		public LeanScreenDepth ScreenDepth;

		[Tooltip("Keep this GameObject placed on the line even when not dragging?")]
		public bool AutoSnap;

		private Vector2 screenOffset;

		protected override void OnSelect(LeanFinger finger)
		{
			base.OnSelect(finger);

			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				// Calculate finger offset
				var screenPosition = (Vector2)camera.WorldToScreenPoint(transform.position);

				screenOffset = screenPosition - finger.ScreenPosition;
			}
		}

		protected virtual void Update()
		{
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				// Is this GameObject selected?
				if (Selectable.IsSelected == true)
				{
					// Does it have a selected finger?
					var finger = Selectable.SelectingFinger;

					if (finger != null)
					{
						// Offset finger screen position
						var screenPoint = finger.ScreenPosition + screenOffset;

						// Move GameObject to closest point
						transform.position = ScreenDepth.Convert(screenPoint, camera, gameObject);
					}
				}

				if (AutoSnap == true)
				{
					var screenPoint = camera.WorldToScreenPoint(transform.position);

					// Move GameObject to closest point
					transform.position = ScreenDepth.Convert(screenPoint, camera, gameObject);
				}
			}
		}
	}
}