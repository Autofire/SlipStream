//
//  ScriptInfo.cs
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
using System.IO;
using System.Diagnostics;

namespace ReachBeyond.VariableObjects.Editor {

	/// <summary>
	/// This class is used for tracking all files related to the given variable object.
	/// </summary>
	public struct ScriptInfo {

		#region Properties
		/// <summary>
		/// The human readable name that is used to refer to the variable object.
		/// </summary>
		public string Name {
			get; private set;
		}

		/// <summary>
		/// The type which the variable object supports.
		/// </summary>
		public string Type {
			get; private set;
		}

		/// <summary>
		/// All file guids that go with this type.
		/// </summary>
		public string[] GUIDs {
			get {
				return _GUIDs.ToArray();
			}
		}

		public ReferabilityMode Referability {
			get; private set;
		}
		#endregion

		#region Variables
		private List<string> _GUIDs;
		#endregion

		#region Functions
		public ScriptInfo(
			string name,
			string type,
			ReferabilityMode referability = ReferabilityMode.Unknown
		) {
			this.Name = name;
			this.Type = type;
			this.Referability = referability;

			_GUIDs = new List<string>();
		}

		public void AddGUID(string newGUID) {
			if(!string.IsNullOrEmpty(newGUID) && !_GUIDs.Contains(newGUID)) {
				_GUIDs.Add(newGUID);
			}
		}

		public void DeleteFiles() {

			if(_GUIDs.Count > 0) {
				// We need to get the editor path first
				string samplePath = Path.GetDirectoryName(
					AssetDatabase.GUIDToAssetPath(_GUIDs[0])
				);
				string editorPath = UnityPathUtils.GetEditorFolder(samplePath);
				//string absEditorPath = UnityPathUtils.RelativeToAbsolute(relEditorPath);

				foreach(string GUID in _GUIDs) {
					AssetDatabase.DeleteAsset(
						AssetDatabase.GUIDToAssetPath(GUID)
					);
				}

				if(Directory.Exists(editorPath) &&
					Directory.GetFiles(editorPath).Length == 0 &&
					Directory.GetDirectories(editorPath).Length == 0
				) {
					AssetDatabase.DeleteAsset(editorPath);
				}
			} // End if
		} // End DeleteFiles

		public override string ToString () {
			string filePaths = "";

			foreach(string guid in _GUIDs) {
				filePaths += AssetDatabase.GUIDToAssetPath(guid) + '\n';
			}

			return string.Format (
				"Name: " + Name + '\n' +
				"Type: " + Type + '\n' +
				"Referability: " + Referability.ToString() + '\n' +
				filePaths );
		}
		#endregion
	} // End of class

} // End of namespace
