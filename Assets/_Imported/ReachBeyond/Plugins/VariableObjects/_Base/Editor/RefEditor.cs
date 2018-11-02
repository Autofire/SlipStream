//
//  RefEditor.cs
//
//  Author:
//       Autofire <http://www.reach-beyond.pro/>
//
//  Copyright (c) 2018 ReachBeyond
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.


﻿using UnityEngine;
using UnityEditor;
using ReachBeyond.VariableObjects;

namespace ReachBeyond.VariableObjects.Base.Editor {

	/// <summary>
	/// Inspector editor for ReachBeyond.Variable.Reference objects.
	/// </summary>
	public class RefEditor : PropertyDrawer {

		/// <summary>
		/// Draws the inspector for a ReachBeyond.VariableObjects.Reference object.
		/// </summary>
		/// <param name="position">Position of the editor.</param>
		/// <param name="property">Varible Object property internal to some other object.</param>
		/// <param name="label">Label to use (in the left column of the inspector).</param>
		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			const int TOGGLE_BUTTON_WIDTH	= 40;
			const int FIELD_OFFSET			= 5 + TOGGLE_BUTTON_WIDTH;

			bool useInternal = property.FindPropertyRelative("_useInternal").boolValue;

			string valuePropertyName     = GetPropertyName(useInternal);
			string useInternalToggleText = GetButtonText(useInternal);

			Color defaultBGColor = GUI.backgroundColor;
			Color buttonBGColor  = GetButtonBackgroundColor(useInternal);

			label.tooltip = GetTooltip(fieldInfo);


			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID (FocusType.Passive), label);

			Rect buttonRect = new Rect(position.x             , position.y, TOGGLE_BUTTON_WIDTH        , position.height);
			Rect fieldRect  = new Rect(position.x+FIELD_OFFSET, position.y, position.width-FIELD_OFFSET, position.height);

			//EditorGUI.PropertyField( checkBoxRect, property.FindPropertyRelative("_useInternal"), GUIContent.none );
			GUI.backgroundColor = buttonBGColor;
			if(GUI.Button(buttonRect, useInternalToggleText, GetButtonStyle(useInternal))) {
				property.FindPropertyRelative("_useInternal").boolValue = !useInternal;
			}
			EditorGUI.LabelField(buttonRect, new GUIContent("", GetButtonTooltip(useInternal)));
			GUI.backgroundColor = defaultBGColor;


			EditorGUI.PropertyField( fieldRect, property.FindPropertyRelative(valuePropertyName), GUIContent.none );

			EditorGUI.EndProperty();
		} // End OnGUI

		/// <summary>
		/// Gets the name of the property to allow editing for.
		/// </summary>
		/// <returns>The name of the property which should have its editor drawn.</returns>
		/// <param name="useInternal">If set to <c>true</c>, use internal value instead of variable.</param>
		protected string GetPropertyName(bool useInternal) {
			return (useInternal ? "internalValue" : "variable");
		}

		/// <summary>
		/// Gets the text to display on the toggle button.
		/// </summary>
		/// <returns>The button text.</returns>
		/// <param name="useInternal">
		/// If <c>true</c>, return text for internal value. Otherwise, text for variable reference.
		/// </param>
		protected virtual string GetButtonText(bool useInternal) {
			return (useInternal ? "Val" : "Ref");
		}

		/// <summary>
		/// Gets the style of the button. When overriding this function, it is best to
		/// call the base GetButtonStyle, and then modify the resulting GUIStyle object.
		/// </summary>
		/// <returns>The button style.</returns>
		/// <param name="useInternal">Used for tweaking style depending on mode.</param>
		protected virtual GUIStyle GetButtonStyle(bool useInternal) {
			GUIStyle style = new GUIStyle(GUI.skin.button);

			style.active.textColor = GetButtonContentColor(useInternal);
			style.normal.textColor = GetButtonContentColor(useInternal);
			style.fontStyle = (useInternal ? FontStyle.Normal : FontStyle.Bold);

			return style;
		}

		protected virtual Color GetButtonContentColor(bool useInternal) {
			return (EditorGUIUtility.isProSkin ? Color.black : Color.white);
		}

		protected virtual Color GetButtonBackgroundColor(bool useInternal) {
			return (EditorGUIUtility.isProSkin ? Color.white : Color.grey);
		}

		protected virtual string GetButtonTooltip(bool useInternal) {
			if(useInternal) {
				return
					"The value is unique to this instance of this component.";
			}
			else {
				return
					"The value matches that of the specified variable. (If no" +
					" variable is provided, the internal value is used intead.)" +
					"\n\n" +
					"The variable's contents may be changed by this object.";
			}
		}

		// TODO Make this be in a generic class
		/// <summary>
		/// Gets the specified by the reference object's TooltipAttribute.
		/// </summary>
		/// <returns>The tooltip to display when the label is moused over.</returns>
		/// <param name="info">
		/// Field info. This comes with PropertyDrawer classes, but we have it as a parameter to make
		/// this function more flexible.
		/// </param>
		private string GetTooltip(System.Reflection.FieldInfo info) {
			// Partial credit to https://answers.unity.com/answers/1421384/view.html

			string tooltip = "";

			object[] attributes = info.GetCustomAttributes(typeof(TooltipAttribute), true);

			if(attributes.Length > 0) {
				TooltipAttribute tt = attributes[0] as TooltipAttribute;
				if (tt != null) {
					tooltip = tt.tooltip;
				}
			}

			return tooltip;
		} // End GetTooltip

	} // End RefEditor

} // End namespace
