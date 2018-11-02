//
//  SettingsWindow.cs
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
using System.Collections.Generic;


namespace ReachBeyond.VariableObjects.Editor {

	public class SettingsWindow : EditorWindow
	{
		// TODO Make the little instruction booklet icon open something relevant

		#region Constants
		private const string EditorPrefPrefix = "ReachBeyond.VariableObjects.";

		private const string UnityVarFoldoutPref  = EditorPrefPrefix + "unityVarFoldout";
		private const string CustomVarFoldoutPref = EditorPrefPrefix + "customVarFoldout";

		private const string HorizontalScrollPref = EditorPrefPrefix + "scrollX";
		private const string VerticalScrollPref   = EditorPrefPrefix + "scrollY";
		#endregion


		#region Initialization
		[MenuItem("Window/Variable Objects")]
		public static void Init() {
			// Get existing open window or if none, make a new one:
			SettingsWindow window = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow));
			window.Show();
		}
		#endregion

		#region Events
		private void OnEnable() {
			titleContent.text = "VarObj Settings";
		}

		private void OnGUI() {

			Vector2 scrollPos = new Vector2(
				EditorPrefs.GetFloat(HorizontalScrollPref, 0f),
				EditorPrefs.GetFloat(VerticalScrollPref, 0f)
			);

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			DrawVarObjHierarchy(
				"Unity Variable Types",
				UnityVarFoldoutPref,
				ScriptFileManager.UnityVarFiles,
				hasDeleteButtons: false
			);

			DrawVarObjHierarchy(
				"Custom Variable Types",
				CustomVarFoldoutPref,
				ScriptFileManager.CustomVarFiles,
				hasDeleteButtons: true
			);

			EditorGUILayout.EndScrollView();

			EditorPrefs.SetFloat(HorizontalScrollPref, scrollPos.x);
			EditorPrefs.SetFloat(VerticalScrollPref, scrollPos.y);
		}
		#endregion


		#region GUI Drawing Functions
		private void DrawVarObjHierarchy(
			string masterLabel,
			string masterFoldoutPref,
			Dictionary<string, ScriptInfo> fileInfoDictionary,
			bool hasDeleteButtons
		) {
			bool isFoldedOut;				// A catch-all variable for storing the foldout bools
			bool markedForDeletion = false;	// For tracking when something's delete button has been pressed

			// Create the main foldout
			isFoldedOut = EditorPrefs.GetBool(masterFoldoutPref, defaultValue: false);
			isFoldedOut = EditorGUILayout.Foldout(isFoldedOut, masterLabel, toggleOnLabelClick: true);
			EditorPrefs.SetBool(masterFoldoutPref, isFoldedOut);

			// Only render the list itself if it's folded out.
			if(isFoldedOut) {

				EditorGUI.indentLevel++;

				// Step through all of the types found
				foreach(ScriptInfo fileInfo in fileInfoDictionary.Values) {

					string editorPrefKey = EditorPrefPrefix + fileInfo.Name;

					// Draw the foldout (with its delete buttons, if necessary)
					string foldoutLabel = fileInfo.Name + " (" + fileInfo.Type + ", " + fileInfo.Referability.ToString() + ") ";
					isFoldedOut = EditorPrefs.GetBool(editorPrefKey, defaultValue: false);

					if(hasDeleteButtons && isFoldedOut) {
						DrawFoldout(ref isFoldedOut, foldoutLabel, out markedForDeletion);
					}
					else {
						DrawFoldout(ref isFoldedOut, foldoutLabel);
					}

					EditorPrefs.SetBool(editorPrefKey, isFoldedOut);


					// Again, only render the list of files if the foldout is open.
					if(isFoldedOut) {
						EditorGUI.indentLevel++;
						DrawFiles(fileInfo.GUIDs);
						EditorGUI.indentLevel--;
					}

					// Handle stuff with the delete button
					if(hasDeleteButtons && markedForDeletion) {

						//Debug.Log("Delete " + fileInfo.name);
						//DeleteFilesForType(fileInfo);
						//ScriptFileManager.DeleteFilesForType(fileInfo, prompt: true);
						bool deletionConfirmed = EditorUtility.DisplayDialog(
							"Delete variable object scripts named '" + fileInfo.Name + "'?",
							"This action cannot be undone!\n(Check VarObj Settings for listing.)",
							"Delete them", "Keep them"
						);

						if(deletionConfirmed) {
							fileInfo.DeleteFiles();
						}

						markedForDeletion = false;
					}
				} // End foreach(...fileInfo...)

				EditorGUI.indentLevel--;
			}
		}


		/// <summary>
		/// Draws the list of files. These may be clicked on to selected them, but they cannot be tweaked.
		/// </summary>
		/// <param name="guids">List of GUIDs to reference.</param>
		private void DrawFiles(string[] guids) {
			foreach(string guid in guids) {
				MonoScript scriptObj = AssetDatabase.LoadAssetAtPath<MonoScript>(
					AssetDatabase.GUIDToAssetPath(guid)
				);

				using (new EditorGUI.DisabledScope(true)) {
					EditorGUILayout.ObjectField(scriptObj, typeof(MonoScript), allowSceneObjects: false);
				}
			}
		}

		/// <summary>
		/// Draws the foldout.
		/// </summary>
		/// <returns><c>true</c>, if foldout is opened by the user, <c>false</c> otherwise.</returns>
		/// <param name="foldout">If set to <c>true</c>, then the foldout will be opened.</param>
		/// <param name="content">The label to draw.</param>
		private void DrawFoldout(ref bool foldout, string content) {
			foldout = EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick: true);
		}

		/// <summary>
		/// Draws the foldout. Also has a delete button.
		/// </summary>
		/// <param name="foldout">Foldout.</param>
		/// <param name="content">Content.</param>
		/// <param name="delete">Delete.</param>
		private void DrawFoldout(ref bool foldout, string content, out bool delete) {

			const int DELETE_BUTTON_WIDTH = 50;

			Rect mainRect = EditorGUILayout.GetControlRect();	// Total space we have to work with

			Rect foldoutRect = mainRect;
			foldoutRect.width -= DELETE_BUTTON_WIDTH;

			Rect deleteRect = mainRect;
			deleteRect.width = DELETE_BUTTON_WIDTH;
			deleteRect.x += foldoutRect.width;

			foldout = EditorGUI.Foldout(foldoutRect, foldout, content, toggleOnLabelClick: true);
			delete = GUI.Button(deleteRect, "Delete");
		}
		#endregion

	} // End of class

} // End of namespace
