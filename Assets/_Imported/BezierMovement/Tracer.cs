using UnityEngine;
using UnityEngine.Assertions;

namespace BezierMovement {
	public sealed class Tracer : MonoBehaviour {

		#region Factory
		public static Tracer CreateTracer(BezierCurveTools.BezierCurve targetCurve) {
			GameObject newObj = new GameObject();
			Tracer newTracer = newObj.AddComponent<Tracer>();

			newTracer.targetCurve = targetCurve;
			newTracer.SnapToPos(0f);

			return newTracer;
		}
		#endregion

		public BezierCurveTools.BezierCurve targetCurve;

		[Tooltip("For debugging purposes.")]
		[Range(-2f, 2f)]
		[SerializeField] private float currentPos = 0f;

		#region Unity events
		void Start() {
			Assert.IsNotNull(targetCurve, "You MUST initialize Tracer.targetCurve.");

			SnapToPos(0f);
		}

		void OnValidate() {
			SnapToPos(currentPos);
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Gets current position on the curve. 0 is start of curve, and 1 is its end.
		/// </summary>
		/// <returns>Current position on the curve.</returns>
		public float GetProgressPercent() {
			return currentPos;
		}

		/// <summary>
		/// Move by the given amount. 0 is start of curve, and 1 is its end.
		/// </summary>
		/// <param name="deltaPos">Delta position.</param>
		public void Move(float deltaPos) {
			if(!Mathf.Approximately(deltaPos, 0f)) {
				SnapToPos(currentPos + deltaPos);
			}
		}

		/// <summary>
		/// Snaps Tracer to the given position. 0f is start of curve, and 1 is its end.
		/// </summary>
		/// <param name="newPos">New position.</param>
		public void SnapToPos(float newPos) {
			//newPos = Mathf.Clamp01(newPos);
			// Get rid of non-decimal component.

			transform.position = targetCurve.GetPointAt(newPos - Mathf.Floor(newPos));
			currentPos = newPos;
		}
		#endregion
	}

}