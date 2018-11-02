using UnityEngine;
using UnityEditor;
using System.Collections;

namespace BezierCurveTools {
	[CustomEditor(typeof(BezierCurve), editorForChildClasses: true)]
	public class BezierCurveEditor : Editor {
		BezierCurve curve;
		SerializedProperty resolutionProp;
		SerializedProperty closeProp;
		SerializedProperty pointsProp;
		SerializedProperty colorProp;
		
		private static bool showPoints = true;
		
		void OnEnable() {
			curve = (BezierCurve)target;
			
			resolutionProp = serializedObject.FindProperty("resolution");
			closeProp = serializedObject.FindProperty("_close");
			pointsProp = serializedObject.FindProperty("points");
			colorProp = serializedObject.FindProperty("drawColor");
		}

		void OnDisable() {
			UnityEditor.Tools.hidden = false;
		}
		
		public override void OnInspectorGUI() {
			UnityEditor.Tools.hidden = true;

			serializedObject.Update();
			
			EditorGUILayout.PropertyField(resolutionProp);
			EditorGUILayout.PropertyField(closeProp);
			EditorGUILayout.PropertyField(colorProp);
			
			showPoints = EditorGUILayout.Foldout(showPoints, "Points");
			
			if(showPoints) {
				int pointCount = pointsProp.arraySize;
				
				for(int i = 0; i < pointCount; i++) {
					DrawPointInspector(curve[i], i);
				}
				
				if(GUILayout.Button("Add Point")) {
					curve.AddPointAt(curve.transform.position);
				}
			}
			
			if(GUI.changed) {
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(target);
			}
		}
		
		void OnSceneGUI() {
			for(int i = 0; i < curve.pointCount; i++) {
				DrawPointSceneGUI(curve[i]);
			}
		}
		
		void DrawPointInspector(BezierPoint point, int index) {
			EditorGUILayout.BeginHorizontal();

			if(GUILayout.Button("X", GUILayout.Width(20))) {
				Undo.SetCurrentGroupName("Remove Point");

				Undo.RecordObject(curve, "Remove Point Reference");
				pointsProp.MoveArrayElement(curve.GetPointIndex(point), curve.pointCount - 1);
				pointsProp.arraySize--;
				Undo.DestroyObjectImmediate(point.gameObject);

				Undo.CollapseUndoOperations( Undo.GetCurrentGroup() );

				return;
			}

			EditorGUILayout.ObjectField(point.gameObject, typeof(GameObject), true);
			
			if(index != 0 && GUILayout.Button(@"/\", GUILayout.Width(25))) {
				UnityEngine.Object other = pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue;
				pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue = point;
				pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
			}
			
			if(index != pointsProp.arraySize - 1 && GUILayout.Button(@"\/", GUILayout.Width(25))) {
				UnityEngine.Object other = pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue;
				pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue = point;
				pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel++;
			EditorGUI.indentLevel++;

			BezierPointEditor.DrawInspector(point, drawPositionInspector: true);

			EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;	
		}
		
		static void DrawPointSceneGUI(BezierPoint point) {
			BezierPointEditor.DrawSceneGUI(point, selected: false);
		}
		
		public static void DrawOtherPoints(BezierCurve curve, BezierPoint caller) {
			foreach(BezierPoint p in curve.GetAnchorPoints()) {
				if(p != caller) DrawPointSceneGUI(p);
			}
		}
		
		[MenuItem("GameObject/Create Other/Bezier Curve")]
		public static void CreateCurve(MenuCommand command) {
			Undo.SetCurrentGroupName("Create Bezier Curve");
			GameObject curveObject = new GameObject("BezierCurve");
			Undo.RegisterCreatedObjectUndo(curveObject, "Undo Create Bezier Curve");
			//Undo.RegisterUndo(curveObject, "Undo Create Curve");
			BezierCurve curve = curveObject.AddComponent<BezierCurve>();
			
			BezierPoint p1 = curve.AddPointAt(Vector3.forward * 0.5f);
			p1.handleStyle = BezierPoint.HandleStyle.Connected;
			p1.handle1 = new Vector3(-0.28f, 0, 0);
			
			BezierPoint p2 = curve.AddPointAt(Vector3.right * 0.5f);
			p2.handleStyle = BezierPoint.HandleStyle.Connected;
			p2.handle1 = new Vector3(0, 0, 0.28f);
			
			BezierPoint p3 = curve.AddPointAt(-Vector3.forward * 0.5f);
			p3.handleStyle = BezierPoint.HandleStyle.Connected;
			p3.handle1 = new Vector3(0.28f, 0, 0);		
			
			BezierPoint p4 = curve.AddPointAt(-Vector3.right * 0.5f);
			p4.handleStyle = BezierPoint.HandleStyle.Connected;
			p4.handle1 = new Vector3(0, 0, -0.28f);
			
			curve.close = true;
		}
	}

}