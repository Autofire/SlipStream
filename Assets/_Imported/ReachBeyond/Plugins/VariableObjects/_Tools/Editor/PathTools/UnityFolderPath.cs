//
//  UnityFolderPath.cs
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
using System;
using UnityEditor;

namespace ReachBeyond.VariableObjects.Editor {

	[Serializable]
	public struct UnityFolderPath {

		#region Fields
		[SerializeField]
		private string _path;
		#endregion

		#region Public Fields
		/// <summary>
		/// Gets or sets the path. When setting, the folder should be an existing
		/// folder in the project's assets folder. If it isn't, then nothing
		/// changes.
		/// </summary>
		/// <value>The path.</value>
		public string Path {
			get { return _path; }
			set {
				string testedValue = TestNewPath(value);
				if(!string.IsNullOrEmpty(testedValue)) {
					_path = testedValue;
				}
			}
		}
		#endregion

		#region Constructors
		public UnityFolderPath(string target) {
			string testedTarget = TestNewPath(target);

			if(string.IsNullOrEmpty(testedTarget)) {
				_path = "Assets";
			}
			else {
				_path = testedTarget;
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Tests if the path given is valid and can be stored.
		/// </summary>
		/// <returns>The new path.</returns>
		/// <param name="newPath">New path.</param>
		private static string TestNewPath(string newPath) {
			// This function exists because we need to be able to use this
			// logic inside the constructor. However, as a struct, the constructor
			// cannot access the path field.
			newPath = UnityPathUtils.AbsoluteToRelative(newPath);
			if(AssetDatabase.IsValidFolder(newPath)) {
				return newPath;
			}
			else {
				return "";
			}
		}
		#endregion

	} // End class

} // End namespace
