#region UsingStatements

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

// Original package: Bezier Curve Editor version 1.1
// Original author: Arkham Interactive (Unity store)

namespace BezierCurveTools {
	/// <summary>
	/// 	- Class for describing and drawing Bezier Curves
	/// 	- Efficiently handles approximate length calculation through 'dirty' system
	/// 	- Has static functions for getting points on curves constructed by Vector3 parameters (GetPoint, GetCubicPoint, GetQuadraticPoint, and GetLinearPoint)
	///     - Moves attached GameObject to the origin.
	/// </summary>
	[ExecuteInEditMode]
	[Serializable]
	public class BezierCurve : MonoBehaviour {
		
		#region PublicVariables
		
		/// <summary>
		///  	- the number of mid-points calculated for each pair of bezier points
		///  	- used for drawing the curve in the editor
		///  	- used for calculating the "length" variable
		/// </summary>
		public int resolution = 30;
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BezierCurve"/> is dirty.
		/// </summary>
		/// <value>
		/// <c>true</c> if dirty; otherwise, <c>false</c>.
		/// </value>
		public bool dirtyLength { get; private set; }
		
		/// <summary>
		/// 	- color this curve will be drawn with in the editor
		///		- set in the editor
		/// </summary>
		public Color drawColor = Color.white;
		
		#endregion


		#region PublicProperties
		
		/// <summary>
		///		- set in the editor
		/// 	- used to determine if the curve should be drawn as "closed" in the editor
		/// 	- used to determine if the curve's length should include the curve between the first and the last points in "points" array
		/// 	- setting this value will cause the curve to become dirty
		/// </summary>
		[SerializeField] private bool _close;
		public bool close {
			get { return _close; }
			set
			{
				if(_close == value) return;
				_close = value;
				dirtyLength = true;
			}
		}
		
		/// <summary>
		///		- set internally
		///		- gets point corresponding to "index" in "points" array
		///		- does not allow direct set
		/// </summary>
		/// <param name='index'>
		/// 	- the index
		/// </param>
		public BezierPoint this[int index] {
			get { return points[index]; }
		}
		
		/// <summary>
		/// 	- number of points stored in 'points' variable
		///		- set internally
		///		- does not include "handles"
		/// </summary>
		/// <value>
		/// 	- The point count
		/// </value>
		public int pointCount {
			get { return points.Count; }
		}
		
		/// <summary>
		/// 	- The approximate length of the curve
		/// 	- recalculates if the curve is "dirty"
		/// </summary>
		private float _length;
		public float length {
			get {
				if(dirtyLength) {
					_length = 0;
					for(int i = 0; i < pointCount - 1; i++){
						_length += ApproximateLength(points[i], points[i + 1], resolution);
					}
					
					if(close)
						_length += ApproximateLength(points[pointCount - 1], points[0], resolution);
					
					dirtyLength = false;
				}
				
				return _length;
			}
		}
		
		#endregion

		
		#region PrivateVariables
		
		/// <summary> 
		/// 	- Array of point objects that make up this curve
		///		- Populated through editor
		/// </summary>
		//[SerializeField] private BezierPoint[] points = new BezierPoint[0];
		[SerializeField] List<BezierPoint> points = new List<BezierPoint>();

		#endregion


		#region UnityFunctions
		
		protected virtual void OnDrawGizmos () {
			Gizmos.color = drawColor;
			
			if(pointCount > 1) {
				for(int i = 0; i < pointCount - 1; i++) {
					DrawCurve(points[i], points[i+1], resolution);
				}
				
				if(close)
					DrawCurve(points[pointCount - 1], points[0], resolution);
			}
		}
		
		protected virtual void Awake() {
			dirtyLength = true;
		}

		/*void Update() {
			transform.position = BoundaryController.instance.GetPosition(0f, 0f);
		}*/
		#endregion

		#region Editor functions
		#if UNITY_EDITOR

		// TODO Reset does not play nice w/ editor undo
		protected virtual void Reset() {
			UnityEditor.Undo.SetCurrentGroupName("Reset GameObject MovementCurve");

			// This step may feel unnecessary, but the order matters here!
			// If you delete the objects and THEN clear the list, when you undo the
			// reset, the list will reference at all the non-existant points for a moment,
			// resulting in a meaningless (but alarming) error.
			//
			// To fix this, we need to ensure that the Undo operation restores the points
			// and then restores the list of the points, hence the odd order.
			BezierPoint[] tmpPoints = points.ToArray();

			UnityEditor.Undo.RecordObject(this, "Clearing point references.");
			points = new List<BezierPoint>();

			foreach(BezierPoint point in tmpPoints) {
				UnityEditor.Undo.DestroyObjectImmediate(point.gameObject);
			}

			UnityEditor.Undo.CollapseUndoOperations( UnityEditor.Undo.GetCurrentGroup() );
		}

		protected virtual void OnValidate() {
			// HACK: This should probably only get called once, not in OnValidate.
			// However, this was the best way to ensure that we get subscribed.
			UnityEditor.Undo.undoRedoPerformed += CheckPoints;

			CheckPoints();
		}

		// This is to get called whenever it's possible that a point may get externally
		// modified; it ensures that we have no null references floating around.
		void CheckPoints() {
			// Note that we should check for this; this callback may get run if we
			// are no longer in the scene.
			if(gameObject != null) {
				// Note that order matters here! If we prune null points first, then we'll
				// potentially put them right back, leading to a null reference exception.
				foreach(BezierPoint point in GetComponentsInChildren<BezierPoint>()) {
					if(!points.Contains(point)) {
						points.Add(point);
					}
				}

				points.RemoveAll((BezierPoint point) => { return point == null; });
			}
			else {
				UnityEditor.Undo.undoRedoPerformed -= CheckPoints;
			}
		}

		void OnDestroy() {
			UnityEditor.Undo.undoRedoPerformed -= CheckPoints;
		}



		#endif
		#endregion


		#region PublicFunctions

		/// <summary>
		/// 	- Adds the given point to the end of the curve ("points" array)
		/// </summary>
		/// <param name='point'>
		/// 	- The point to add.
		/// </param>
		public void AddPoint(BezierPoint point) {
			points.Add(point);
			dirtyLength = true;
		}
		
		/// <summary>
		/// 	- Adds a point at position
		/// </summary>
		/// <returns>
		/// 	- The point object
		/// </returns>
		/// <param name='position'>
		/// 	- Where to add the point
		/// </param>
		public BezierPoint AddPointAt(Vector3 position) {
			GameObject pointObject = new GameObject("Point "+pointCount);
			#if UNITY_EDITOR
			Undo.SetCurrentGroupName("Add Point");

			Undo.RecordObject(this, "Update curve");
			Undo.RegisterCreatedObjectUndo(pointObject, "Add New Point");
			#endif

			pointObject.transform.parent = transform;
			pointObject.transform.position = position;

			BezierPoint newPoint = CreatePoint(pointObject);
			newPoint.curve = this;	// NOTE: This ALSO will add the point to the curve!
			newPoint.handle1 = Vector3.right*0.1f;
			newPoint.handle2 = -Vector3.right*0.1f;

			#if UNITY_EDITOR
			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
			#endif

			return newPoint;
		}
		
		/// <summary>
		/// 	- Removes the given point from the curve ("points" array)
		/// </summary>
		/// <param name='point'>
		/// 	- The point to remove
		/// </param>
		public void RemovePoint(BezierPoint point) {
			points.Remove(point);
			dirtyLength = true;
		}
		
		/// <summary>
		/// 	- Gets a copy of the bezier point array used to define this curve
		/// </summary>
		/// <returns>
		/// 	- The cloned array of points
		/// </returns>
		public BezierPoint[] GetAnchorPoints() {
			return (BezierPoint[])points.ToArray();
		}
		
		/// <summary>
		/// 	- Gets the point at 't' percent along this curve
		/// </summary>
		/// <returns>
		/// 	- Returns the point at 't' percent
		/// </returns>
		/// <param name='t'>
		/// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
		/// </param>
		public Vector3 GetPointAt(float t) {
			// This seems to find a position on the curve -- that is, a percentage --
			// then calls GetPoint, which does the heavy lifting.
			//  ~AF
			if(t <= 0)
				return points[0].position;
			else if (t >= 1)
				return points[pointCount - 1].position;

			float totalPercent = 0;
			float curvePercent = 0;
			
			BezierPoint p1 = null;
			BezierPoint p2 = null;

			for(int i = 0; i < pointCount - 1; i++) {
				curvePercent = ApproximateLength(points[i], points[i + 1], 10) / length;
				if(totalPercent + curvePercent > t) {
					p1 = points[i];
					p2 = points[i + 1];
					break;
				}
				else
					totalPercent += curvePercent;
			}

			if(p1 == null) {
				// This runs in one of two situations:
				//  1 - We're set to a closed curve and the position is beyond the last point
				//  2 - The number is really high, but the calculation *barely* misses the correct
				//      value, skipping it entirely and causing it to fail to set any point.

				if(close) {
					p1 = points[pointCount - 1];
					p2 = points[0];
				}
				else {
					// HACK: This code should really be unnecessary. However, I don't
					//       understand the code well enough to come up with a good fix.	
					p1 = points[pointCount - 2];
					p2 = points[pointCount - 1];

					totalPercent -= curvePercent;	// Subtract out change above to get the correct position
				}
			}

			t -= totalPercent;

			return GetPoint(p1, p2, t / curvePercent);
		}
		
		/// <summary>
		/// 	- Get the index of the given point in this curve
		/// </summary>
		/// <returns>
		/// 	- The index, or -1 if the point is not found
		/// </returns>
		/// <param name='point'>
		/// 	- Point to search for
		/// </param>
		public int GetPointIndex(BezierPoint point)
		{
			int result = -1;
			for(int i = 0; i < pointCount; i++)
			{
				if(points[i] == point)
				{
					result = i;
					break;
				}
			}
			
			return result;
		}
		
		/// <summary>
		/// 	- Sets this curve to 'dirty'
		/// 	- Forces the curve to recalculate its length
		/// </summary>
		public void SetDirty()
		{
			dirtyLength = true;
		}
		
		#endregion


		#region PublicStaticFunctions
		
		/// <summary>
		/// 	- Draws the curve in the Editor
		/// </summary>
		/// <param name='p1'>
		/// 	- The bezier point at the beginning of the curve
		/// </param>
		/// <param name='p2'>
		/// 	- The bezier point at the end of the curve
		/// </param>
		/// <param name='resolution'>
		/// 	- The number of segments along the curve to draw
		/// </param>
		public static void DrawCurve(BezierPoint p1, BezierPoint p2, int resolution)
		{
			int limit = resolution+1;
			float _res = resolution;
			Vector3 lastPoint = p1.position;
			Vector3 currentPoint = Vector3.zero;
			
			for(int i = 1; i < limit; i++){
				currentPoint = GetPoint(p1, p2, i/_res);
				Gizmos.DrawLine(lastPoint, currentPoint);
				lastPoint = currentPoint;
			}		
		}	

		/// <summary>
		/// 	- Gets the point 't' percent along a curve
		/// 	- Automatically calculates for the number of relevant points
		/// </summary>
		/// <returns>
		/// 	- The point 't' percent along the curve
		/// </returns>
		/// <param name='p1'>
		/// 	- The bezier point at the beginning of the curve
		/// </param>
		/// <param name='p2'>
		/// 	- The bezier point at the end of the curve
		/// </param>
		/// <param name='t'>
		/// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
		/// </param>
		public static Vector3 GetPoint(BezierPoint p1, BezierPoint p2, float t) {
			UnityEngine.Assertions.Assert.IsNotNull(p1, "Point 1 is null!");
			UnityEngine.Assertions.Assert.IsNotNull(p2, "Point 2 is null!");

			if(p1.handle2 != Vector3.zero) {
				if(p2.handle1 != Vector3.zero)
					return GetCubicCurvePoint(p1.position, p1.globalHandle2, p2.globalHandle1, p2.position, t);
				else
					return GetQuadraticCurvePoint(p1.position, p1.globalHandle2, p2.position, t);
			}
			else {
				if(p2.handle1 != Vector3.zero)
					return GetQuadraticCurvePoint(p1.position, p2.globalHandle1, p2.position, t);
				else
					return GetLinearPoint(p1.position, p2.position, t);
			}	
		}

		/// <summary>
		/// 	- Gets the point 't' percent along a third-order curve
		/// </summary>
		/// <returns>
		/// 	- The point 't' percent along the curve
		/// </returns>
		/// <param name='p1'>
		/// 	- The point at the beginning of the curve
		/// </param>
		/// <param name='p2'>
		/// 	- The second point along the curve
		/// </param>
		/// <param name='p3'>
		/// 	- The third point along the curve
		/// </param>
		/// <param name='p4'>
		/// 	- The point at the end of the curve
		/// </param>
		/// <param name='t'>
		/// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
		/// </param>
	    public static Vector3 GetCubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	    {
	        t = Mathf.Clamp01(t);

	        Vector3 part1 = Mathf.Pow(1 - t, 3) * p1;
	        Vector3 part2 = 3 * Mathf.Pow(1 - t, 2) * t * p2;
	        Vector3 part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p3;
	        Vector3 part4 = Mathf.Pow(t, 3) * p4;

	        return part1 + part2 + part3 + part4;
	    }
		
		/// <summary>
		/// 	- Gets the point 't' percent along a second-order curve
		/// </summary>
		/// <returns>
		/// 	- The point 't' percent along the curve
		/// </returns>
		/// <param name='p1'>
		/// 	- The point at the beginning of the curve
		/// </param>
		/// <param name='p2'>
		/// 	- The second point along the curve
		/// </param>
		/// <param name='p3'>
		/// 	- The point at the end of the curve
		/// </param>
		/// <param name='t'>
		/// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
		/// </param>
	    public static Vector3 GetQuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
	    {
	        t = Mathf.Clamp01(t);

	        Vector3 part1 = Mathf.Pow(1 - t, 2) * p1;
	        Vector3 part2 = 2 * (1 - t) * t * p2;
	        Vector3 part3 = Mathf.Pow(t, 2) * p3;

	        return part1 + part2 + part3;
	    }
		
		/// <summary>
		/// 	- Gets point 't' percent along a linear "curve" (line)
		/// 	- This is exactly equivalent to Vector3.Lerp
		/// </summary>
		/// <returns>
		///		- The point 't' percent along the curve
		/// </returns>
		/// <param name='p1'>
		/// 	- The point at the beginning of the line
		/// </param>
		/// <param name='p2'>
		/// 	- The point at the end of the line
		/// </param>
		/// <param name='t'>
		/// 	- Value between 0 and 1 representing the percent along the line (0 = 0%, 1 = 100%)
		/// </param>
	    public static Vector3 GetLinearPoint(Vector3 p1, Vector3 p2, float t)
	    {
	        return p1 + ((p2 - p1) * t);
	    }
		
		/// <summary>
		/// 	- Gets point 't' percent along n-order curve
		/// </summary>
		/// <returns>
		/// 	- The point 't' percent along the curve
		/// </returns>
		/// <param name='t'>
		/// 	- Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
		/// </param>
		/// <param name='points'>
		/// 	- The points used to define the curve
		/// </param>
		public static Vector3 GetPoint(float t, params Vector3[] points){
			t = Mathf.Clamp01(t);
			
			int order = points.Length-1;
			Vector3 point = Vector3.zero;
			Vector3 vectorToAdd;
			
			for(int i = 0; i < points.Length; i++){
				vectorToAdd = points[points.Length-i-1] * (BinomialCoefficient(i, order) * Mathf.Pow(t, order-i) * Mathf.Pow((1-t), i));
				point += vectorToAdd;
			}
			
			return point;
		}
		
		/// <summary>
		/// 	- Approximates the length
		/// </summary>
		/// <returns>
		/// 	- The approximate length
		/// </returns>
		/// <param name='p1'>
		/// 	- The bezier point at the start of the curve
		/// </param>
		/// <param name='p2'>
		/// 	- The bezier point at the end of the curve
		/// </param>
		/// <param name='resolution'>
		/// 	- The number of points along the curve used to create measurable segments
		/// </param>
		public static float ApproximateLength(BezierPoint p1, BezierPoint p2, int resolution = 10)
		{
			float _res = resolution;
			float total = 0;
			Vector3 lastPosition = p1.position;
			Vector3 currentPosition;
			
			for(int i = 0; i < resolution + 1; i++)
			{
				currentPosition = GetPoint(p1, p2, i / _res);
				total += (currentPosition - lastPosition).magnitude;
				lastPosition = currentPosition;
			}
			
			return total;
		}
		
		#endregion

		#region ProtectedFunctions
		protected virtual BezierPoint CreatePoint(GameObject targetObj) {
			return targetObj.AddComponent<BezierPoint>();
		}
		#endregion


		#region UtilityFunctions
		
		private static int BinomialCoefficient(int i, int n){
			return 	Factoral(n)/(Factoral(i)*Factoral(n-i));
		}
		
		private static int Factoral(int i){
			if(i == 0) return 1;
			
			int total = 1;
			
			while(i-1 >= 0){
				total *= i;
				i--;
			}
			
			return total;
		}

		#endregion
		
		/* needs testing
		public Vector3 GetPointAtDistance(float distance)
		{
			if(close)
			{
				if(distance < 0) while(distance < 0) { distance += length; }
				else if(distance > length) while(distance > length) { distance -= length; }
			}
			
			else
			{
				if(distance <= 0) return points[0].position;
				else if(distance >= length) return points[points.Length - 1].position;
			}
			
			float totalLength = 0;
			float curveLength = 0;
			
			BezierPoint firstPoint = null;
			BezierPoint secondPoint = null;
			
			for(int i = 0; i < points.Length - 1; i++)
			{
				curveLength = ApproximateLength(points[i], points[i + 1], resolution);
				if(totalLength + curveLength >= distance)
				{
					firstPoint = points[i];
					secondPoint = points[i+1];
					break;
				}
				else totalLength += curveLength;
			}
			
			if(firstPoint == null)
			{
				firstPoint = points[points.Length - 1];
				secondPoint = points[0];
				curveLength = ApproximateLength(firstPoint, secondPoint, resolution);
			}
			
			distance -= totalLength;
			return GetPoint(distance / curveLength, firstPoint, secondPoint);
		}
		*/
	}
}