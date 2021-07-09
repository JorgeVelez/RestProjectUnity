using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This component allows you to get the center and delta of all fingers
	public class LeanMultiSet : MonoBehaviour
	{
		public enum DeltaCoordinatesType
		{
			Screen,
			Scaled
		}

		// Event signature
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("If the finger didn't move, ignore it?")]
		public bool IgnoreIfStatic;

		[Tooltip("If RequiredSelectable.IsSelected is false, ignore?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The coordinate space of the OnSetDelta values")]
		public DeltaCoordinatesType DeltaCoordinates;

		public Vector2Event OnSetCenter;

		public Vector2Event OnSetDelta;

		protected virtual void Update()
		{
			// Get fingers
			var fingers = LeanSelectable.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount, RequiredSelectable);

			if (fingers.Count > 0)
			{
				// Get delta
				var delta = LeanGesture.GetScreenDelta(fingers);

				// Ignore?
				if (delta.sqrMagnitude == 0.0f)
				{
					return;
				}

				// Scale?
				if (DeltaCoordinates == DeltaCoordinatesType.Scaled)
				{
					delta *= LeanTouch.ScalingFactor;
				}

				// Call events
				if (OnSetCenter != null)
				{
					OnSetCenter.Invoke(LeanGesture.GetScreenCenter(fingers));
				}

				if (OnSetDelta != null)
				{
					OnSetDelta.Invoke(delta);
				}
			}
		}
	}
}