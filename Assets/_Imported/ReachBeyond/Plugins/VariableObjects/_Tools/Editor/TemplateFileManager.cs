//
//  TemplateFileManager.cs
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


﻿using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ReachBeyond.VariableObjects.Editor {

	public static class TemplateFileManager {

		/// <summary>Label for templates (NOT ACTUAL SCRIPTS).</summary>
		public const string EngineTemplateLabel =
			ScriptFileManager.BaseLabel + ".EngineTemplate";
		/// <summary>Label for templates which become editor scripts.</summary>
		public const string EditorTemplateLabel =
			ScriptFileManager.BaseLabel + ".EditorTemplate";

		/// <summary>Label for scripts that work with a class.</summary>
		public const string ClassCompatibleTemplateLabel =
			ScriptFileManager.BaseLabel + ".ClassCompatible";
		/// <summary>Label for scripts that work with a struct.</summary>
		public const string StructCompatibleTemplateLabel =
			ScriptFileManager.BaseLabel + ".StructCompatible";

		/// <summary>
		/// The list of all of the template files.
		/// </summary>
		private static TemplateInfo[] _templates;

		/// <summary>
		/// If true, then the template arrays need to be updated before we can use them again.
		/// </summary>
		private static bool templateArraysAreOutdated;

		/// <summary>
		/// Gets the the info on all of the templates.
		/// </summary>
		/// <value>The template info.</value>
		public static TemplateInfo[] Templates {
			get {
				UpdateTemplateArrays();
				return (TemplateInfo[]) _templates.Clone();
			}
		}


		#region Constructor and Events
		static TemplateFileManager() {
			templateArraysAreOutdated = true;
		}

		private class CatchPostProcessorEvents : AssetPostprocessor {
			static void OnPostprocessAllAssets(
				string[] importedAssets,
				string[] deletedAssets,
				string[] movedAssets,
				string[] movedFromAssetPaths
			) {
				if(movedFromAssetPaths == null) {
					throw new System.ArgumentNullException("movedFromAssetPaths");
				}
				if(movedAssets == null) {
					throw new System.ArgumentNullException("movedAssets");
				}
				if(deletedAssets == null) {
					throw new System.ArgumentNullException("deletedAssets");
				}
				if(importedAssets == null) {
					throw new System.ArgumentNullException("importedAssets");
				}

				// It's easiest to assume that this is the case; checking
				// everything manually requires a lot of extra checks with no
				// gain, since we are still limiting our search (below) down to
				// tags.
				templateArraysAreOutdated = true;
			}
		}
		#endregion

		private static void UpdateTemplateArrays() {
			if(templateArraysAreOutdated) {
				templateArraysAreOutdated = false;

				string[] guids = AssetDatabase.FindAssets(
					"l:" + EngineTemplateLabel + " l:" + EditorTemplateLabel
				);
				_templates = new TemplateInfo[guids.Length];

				// Cannot use foreach because we need to index through the now
				// parallel arrays.
				for(int i = 0; i < _templates.Length; i++) {
					Object templateObj =
						(Object) AssetDatabase.LoadAssetAtPath<Object>(
							AssetDatabase.GUIDToAssetPath(guids[i])
						);
					string[] labels = AssetDatabase.GetLabels(templateObj);

					_templates[i] = new TemplateInfo(
						guid: guids[i],
						structComp: labels.Contains(StructCompatibleTemplateLabel),
						classComp: labels.Contains(ClassCompatibleTemplateLabel),
						editor: labels.Contains(EditorTemplateLabel)
					);

#if REACHBEYOND_VARIABLES_DEBUG
					Debug.Log((_templates[i]).ToString());
#endif

				} // End for(i < _templates.Length)
			} // End if(templateArraysAreOutdated)
		}

	} // End class

} // End namespace
