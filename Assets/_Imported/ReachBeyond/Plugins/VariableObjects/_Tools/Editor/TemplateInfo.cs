//
//  TemplateInfo.cs
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

namespace ReachBeyond.VariableObjects.Editor {

	public struct TemplateInfo {

		/// <summary>
		/// GUID of template.
		/// </summary>
		public readonly string GUID;

		/// <summary>
		/// Path of the template.
		/// </summary>
		public readonly string path;

		/// <summary>
		/// If true, this template is suitable for struct-based VarObjs.
		/// </summary>
		public readonly bool StructCompatible;

		/// <summary>
		/// If true, this template is suitable for class-based VarObjs.
		/// </summary>
		public readonly bool ClassCompatible;

		/// <summary>
		/// If true, the resulting script must be placed under an Editor folder.
		/// </summary>
		public readonly bool IsEditorTemplate;

		/// <summary>
		/// If true, the resulting script must not be placed under an Editor folder.
		/// </summary>
		/// <value>
		/// If true, the resulting script must not be placed under an Editor folder.
		/// </value>
		public bool IsEngineTemplate {
			get {
				return !IsEditorTemplate;
			}
		}

		public TemplateInfo(string guid, bool structComp, bool classComp, bool editor) {
			this.path = AssetDatabase.GUIDToAssetPath(guid);
			this.GUID = guid;
			this.StructCompatible = structComp;
			this.ClassCompatible = classComp;
			this.IsEditorTemplate = editor;
		}

		public override string ToString () {
			return string.Format(
				"Template Info: {0}\n" +
				"Compatible with structs: {1}\n" +
				"Compatible with classes: {2}\n" +
				"Is an {3} template.",
				new string[] {
					AssetDatabase.GUIDToAssetPath( GUID ),
					StructCompatible.ToString(),
					ClassCompatible.ToString(),
					(IsEngineTemplate ? "engine" : "editor")}
			);
		}

		/// <summary>
		/// Returns true if the given referibility mode is supported.
		/// </summary>
		/// <returns>True if compatible.</returns>
		/// <param name="target">Target referability mode.</param>
		public bool IsCompatibleWith(ReferabilityMode target) {
			if(target == ReferabilityMode.Class) {
				return ClassCompatible;
			}
			else if(target == ReferabilityMode.Struct) {
				return StructCompatible;
			}
			else {
				return false;
			}
		}


	}
}
