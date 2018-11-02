//
//  UnityFolderPathInspector.cs
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


﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ReachBeyond.VariableObjects.Editor {

	[CustomPropertyDrawer(typeof(UnityFolderPath))]
	public class UnityFolderPathInspector : PropertyDrawer {

		// The following two variables are used for a workaround in Unity's 
		// UI system. We cannot call the PromptForNewString function more
		// than once in a single draw function, so we need to use a
		// delegate/callback so that the function gets called outside of the
		// drawcall.
		//
		// The only problem with this is that we cannot return the value of
		// PromptForNewString. Thus, we'll need to use these two variables to
		// store the new results.

		/// <summary>
		/// If the user specified a new path, then this gets set to true.
		/// Once read in (inside OnGUI, preferably), this should get set to
		/// false.
		/// </summary>
		private bool userPathUpdated = false;
		/// <summary>
		/// If the user specified a new path, it gets stored here. Also see
		/// userPathUpdated.
		/// </summary>
		private string userPath = "";

			
		override public void OnGUI(
			Rect position, SerializedProperty property, GUIContent label
		) {
			const float BUTTON_WIDTH = 55;
			const float PATH_OFFSET  = 5 + BUTTON_WIDTH;

			SerializedProperty pathProperty = property.FindPropertyRelative("_path");

			if(userPathUpdated) {
				pathProperty.stringValue = userPath;
				userPathUpdated = false;
			}

			string initPath = pathProperty.stringValue;

			EditorGUI.BeginProperty(position, label, property);

			label.tooltip = GetTooltip(fieldInfo);
			position = EditorGUI.PrefixLabel(
				position,
				GUIUtility.GetControlID(FocusType.Passive),
				label
			);
			//System.Attribute.GetCustomAttribute(property, typeof(TooltipAttribute));

			Rect buttonPosition = new Rect(
				position.x, position.y,
				BUTTON_WIDTH, position.height
			);
			Rect pathPosition = new Rect(
				position.x + PATH_OFFSET, position.y,
				position.width - PATH_OFFSET, position.height
			);

			if(GUI.Button(buttonPosition, "Change")) {
				// If we call this code:
				//   pathProperty.stringValue = PromptForNewString(initPath);
				// Unity throws an exception when the user attempts to open an
				// invalid folder, and then a valid one. This is probably caused
				// by opening the folder prompt twice within one inspector call.
				//
				// We can work around this limitation by delaying the call so
				// that it happens after the inspector is done drawing.
				//
				// Thanks to http://answers.unity.com/answers/1007379/view.html
				// for this solution.
				EditorApplication.delayCall +=
					() => {
						userPath = PromptForNewString(initPath);
						userPathUpdated = true;
					};
			}
			EditorGUI.LabelField(pathPosition, initPath);

			EditorGUI.EndProperty();
		}


		private string PromptForNewString(string initPathRel) {

			bool valid = false;
			string newPathAbs = "";
			string newPathRel = "";

			do {
				newPathAbs = EditorUtility.OpenFolderPanel("Choose a target folder", initPathRel, "");

				if(string.IsNullOrEmpty(newPathAbs)) {
					// They must have canceled or something
					newPathRel = initPathRel;
					valid = true;
				}
				else {
					newPathRel = UnityPathUtils.AbsoluteToRelative(newPathAbs);

					valid = UnityPathUtils.IsRelativePath(newPathRel);

					if(!valid) {
						EditorUtility.DisplayDialog(
							title: "Invalid Destination",
							message: "You must pick a folder within the project's Assets folder!",
							ok: "OK"
						);
					}
				}
			} while(!valid);

			return newPathRel;
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
				if(tt != null) {
					tooltip = tt.tooltip;
				}
			}

			return tooltip;
		} // End GetTooltip

	} // End of class
} // End of namespace
