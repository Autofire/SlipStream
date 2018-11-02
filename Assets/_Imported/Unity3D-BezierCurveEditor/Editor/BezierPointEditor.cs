using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System.Collections;

namespace BezierCurveTools {
	[CustomEditor(typeof(BezierPoint), editorForChildClasses: true)]
	[CanEditMultipleObjects]
	public class BezierPointEditor : Editor {

		private BezierPoint point;

		private delegate void HandleFunction(BezierPoint p, bool selected);
		private static HandleFunction[] handlers =
			new HandleFunction[] { HandleConnected, HandleBroken, HandleAbsent };

		private enum HandleNum { one, two };

		private static string HANDLE_UNDO_MSG = "Move Bezier Handle";
		private static string POINT_UNDO_MSG = "Move Bezier Point";

		#region Inspector events
		void OnEnable() {
			point = (BezierPoint)target;
		}	
		
		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawInspector(point, drawPositionInspector: false);
		}
		#endregion

		#region Inspector helpers
		public static void DrawInspector(
			BezierPoint point,
			bool drawPositionInspector)
		{
			SerializedObject serObj = new SerializedObject(point);

			SerializedProperty handleTypeProp = serObj.FindProperty("handleStyle");
			SerializedProperty handle1Prop = serObj.FindProperty("_handle1");
			SerializedProperty handle2Prop = serObj.FindProperty("_handle2");

			DrawTypeInspector(handleTypeProp, handle1Prop, handle2Prop);

			if(drawPositionInspector) {
				DrawPositionInspector(point);
			}

			DrawHandleInspector(handleTypeProp, handle1Prop, handle2Prop);

			if(GUI.changed) {
				serObj.ApplyModifiedProperties();
				EditorUtility.SetDirty(serObj.targetObject);
			}
		}

		private static BezierPoint.HandleStyle DrawTypeInspector(
			SerializedProperty handleTypeProp,
			SerializedProperty handle1Prop,
			SerializedProperty handle2Prop)
		{
			Assert.IsNotNull(handleTypeProp);
			Assert.IsNotNull(handle1Prop);
			Assert.IsNotNull(handle2Prop);

			BezierPoint.HandleStyle newHandleType = (BezierPoint.HandleStyle)EditorGUILayout.EnumPopup(
				"Handle Type", (BezierPoint.HandleStyle)handleTypeProp.intValue );

			if(newHandleType != (BezierPoint.HandleStyle)handleTypeProp.intValue)
			{
				handleTypeProp.intValue = (int)newHandleType;

				switch(newHandleType) {
				case BezierPoint.HandleStyle.Connected:
					if(handle1Prop.vector3Value != Vector3.zero)
						handle2Prop.vector3Value = -handle1Prop.vector3Value;
					else if(handle2Prop.vector3Value != Vector3.zero)
						handle1Prop.vector3Value = -handle2Prop.vector3Value;
					else {
						handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);	
						handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);	
					}
					break;
				case BezierPoint.HandleStyle.Broken:
					if(handle1Prop.vector3Value == Vector3.zero
						&& handle2Prop.vector3Value == Vector3.zero)
					{
						handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);
						handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);
					}
					break;
				case BezierPoint.HandleStyle.None:
					handle1Prop.vector3Value = Vector3.zero;
					handle2Prop.vector3Value = Vector3.zero;
					break;
				}
			}

			return newHandleType;
		}

		private static void DrawPositionInspector(BezierPoint point) {
			Vector3 newPointPos = EditorGUILayout.Vector3Field(
				"Position : ",
				point.transform.localPosition
			);

			if(newPointPos != point.transform.localPosition) {
				Undo.RegisterCompleteObjectUndo(point.transform, POINT_UNDO_MSG);
				point.transform.localPosition = newPointPos;
			}
		}

		private static void DrawHandleInspector(
			SerializedProperty handleTypeProp,
			SerializedProperty handle1Prop,
			SerializedProperty handle2Prop)
		{
			BezierPoint.HandleStyle handleType = (BezierPoint.HandleStyle) handleTypeProp.enumValueIndex;

			if(handleType != BezierPoint.HandleStyle.None) {

				Vector3 newHandle1 =
					EditorGUILayout.Vector3Field("Handle 1", handle1Prop.vector3Value);
				Vector3 newHandle2 =
					EditorGUILayout.Vector3Field("Handle 2", handle2Prop.vector3Value);

				if(handleType == BezierPoint.HandleStyle.Connected) {
					if(newHandle1 != handle1Prop.vector3Value) {
						handle1Prop.vector3Value = newHandle1;
						handle2Prop.vector3Value = -newHandle1;
					}

					else if(newHandle2 != handle2Prop.vector3Value) {
						handle1Prop.vector3Value = -newHandle2;
						handle2Prop.vector3Value = newHandle2;			
					}
				}
				else {
					handle1Prop.vector3Value = newHandle1;
					handle2Prop.vector3Value = newHandle2;
				}
			}
		}
		#endregion

		#region Unity scene events
		void OnSceneGUI() {
			DrawSceneGUI(point, selected: true);

			BezierCurveEditor.DrawOtherPoints(point.curve, point);
		}

		public static void DrawSceneGUI(BezierPoint p, bool selected) {
			DrawLabel(p, selected);
			DrawPoint(p, selected);
			handlers[(int)p.handleStyle](p, selected);
			DrawHandleLines(p);
		}
		#endregion

		#region Handle types
		private static void HandleConnected(BezierPoint p, bool selected) {
			Vector3 newGlobal1 = DrawHandle(p, HandleNum.one, selected);
			
			if(newGlobal1 != p.globalHandle1){
				Undo.RegisterCompleteObjectUndo(p, HANDLE_UNDO_MSG);
				p.globalHandle1 = newGlobal1;
				p.globalHandle2 = -(newGlobal1 - p.position) + p.position;
			}
			
			Vector3 newGlobal2 = DrawHandle(p, HandleNum.two, selected);
			
			if(newGlobal2 != p.globalHandle2){
				Undo.RegisterCompleteObjectUndo(p, HANDLE_UNDO_MSG);
				p.globalHandle1 = -(newGlobal2 - p.position) + p.position;
				p.globalHandle2 = newGlobal2;
			}
		}
		
		private static void HandleBroken(BezierPoint p, bool selected){
			Vector3 newGlobal1 = DrawHandle(p, HandleNum.one, selected);
			Vector3 newGlobal2 = DrawHandle(p, HandleNum.two, selected);

			if(newGlobal1 != p.globalHandle1)
			{
				Undo.RegisterCompleteObjectUndo(p, HANDLE_UNDO_MSG);
				p.globalHandle1 = newGlobal1;
			}
			
			if(newGlobal2 != p.globalHandle2)
			{
				Undo.RegisterCompleteObjectUndo(p, HANDLE_UNDO_MSG);
				p.globalHandle2 = newGlobal2;
			}
		}
		
		private static void HandleAbsent(BezierPoint p, bool selected)
		{
			p.handle1 = Vector3.zero;
			p.handle2 = Vector3.zero;
		}
		#endregion

		#region Drawers
		private static void DrawLabel(BezierPoint p, bool selected) {
			// We don't need a label if we're selected to the object in question.
			if(!selected) {
				Handles.Label(
					p.position + new Vector3(0, HandleUtility.GetHandleSize(p.position) * 0.4f, 0),
					p.gameObject.name
				);
			}
		}

		private static void DrawPoint(BezierPoint p, bool selected) {
			Handles.color = Color.green;

			Vector3 newPosition = Handles.FreeMoveHandle(
				p.position,
				p.transform.rotation,
				HandleUtility.GetHandleSize(p.position)*(selected ? 0.2f : 0.1f),
				Vector3.zero,
				PointCap(selected)
			);

			if(p.position != newPosition) {
				Undo.RegisterCompleteObjectUndo(p.transform, POINT_UNDO_MSG);
				p.position = newPosition;
			}
		}

		private static Vector3 DrawHandle(BezierPoint p, HandleNum num, bool selected) {
			Handles.color = Color.cyan;

			return Handles.FreeMoveHandle(
				(num == HandleNum.one ? p.globalHandle1 : p.globalHandle2),
				p.transform.rotation,
				HandleUtility.GetHandleSize(p.globalHandle1)*(selected ? 0.15f : 0.075f),
				Vector3.zero,
				HandleCap(selected)
			);
		}

		private static void DrawHandleLines(BezierPoint p) {
			Handles.color = Color.yellow;
			Handles.DrawLine(p.position, p.globalHandle1);
			Handles.DrawLine(p.position, p.globalHandle2);
		}
		#endregion

		#region Cap selector
		private static Handles.CapFunction PointCap(bool selected) {
			if(selected) {
				return Handles.CubeHandleCap;
			}
			else {
				return Handles.RectangleHandleCap;
			}
		}

		private static Handles.CapFunction HandleCap(bool selected) {
			if(selected) {
				return Handles.SphereHandleCap;
			}
			else {
				return Handles.CircleHandleCap;
			}
		}
		#endregion
	}
}