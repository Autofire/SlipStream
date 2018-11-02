//
//  UnityPathUtils.cs
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
using System.IO;
using System.Text.RegularExpressions;

namespace ReachBeyond.VariableObjects.Editor {

	/// <summary>
	/// Utilities to make working with Unity paths easier.
	/// 
	/// These are not intended to be bullet-proof...when working with relative
	/// paths, these are paths which AssetDatabase will return. (i.e. they
	/// always start with the "Assets" folder.)
	/// 
	/// Absolute paths include the path of the project. These are to be used
	/// with C# things.
	/// </summary>
	public static class UnityPathUtils {

		/// <summary>
		/// Checks if the given path is a relative path. (Relative to the
		/// project's root folder.)
		/// </summary>
		/// <returns>True if the path is relative to the project.</returns>
		/// <param name="path">Path to check.</param>
		public static bool IsRelativePath(string path) {
			return path.StartsWith(
				"Assets",
				System.StringComparison.CurrentCulture
			);
		}

		/// <summary>
		/// Checks if the given path is an absolute path that points into this
		/// project's assets folder.
		/// </summary>
		/// <returns>Returns true if absolute path was given.</returns>
		/// <param name="path">Path to check.</param>
		public static bool IsAbsoluteProjectPath(string path) {
			return path.StartsWith(
				Application.dataPath,
				System.StringComparison.CurrentCulture
			);
		}

		/// <summary>
		/// Converts the absolute path to a relative path. Note that this path
		/// must be pointing at the project's "Assets" folder. Otherwise,
		/// the original path is returned.
		/// </summary>
		/// <returns>The relative path.</returns>
		/// <param name="absPath">Absolute path.</param>
		public static string AbsoluteToRelative(string absPath) {

			string relPath;

			if(IsAbsoluteProjectPath(absPath)) {
				relPath = absPath.Remove(0, Application.dataPath.Length);
				relPath = relPath.TrimStart(new char[] { '/', '\\' });
				relPath = UnityPathUtils.Combine("Assets", relPath);
			}
			else {
				relPath = absPath;
			}

			return relPath;
		}

		/// <summary>
		/// Converts the given relative path into an absolute path. Note that
		/// this only works for paths that are pointing at the "Assets" folder
		/// of the current project. Otherwise, the original path is returned.
		/// </summary>
		/// <returns>The to absolute.</returns>
		/// <param name="relPath">Rel path.</param>
		public static string RelativeToAbsolute(string relPath) {

			string absPath;

			if(IsRelativePath(relPath)) {
				absPath = relPath.Remove(0, "Assets".Length + 1);
				absPath = absPath.TrimStart(new char[] { '/', '\\' });
				absPath = UnityPathUtils.Combine(Application.dataPath, absPath);
			}
			else {
				absPath = relPath;
			}

			return absPath;
		}

		/// <summary>
		/// Given a path, checks if it would be part of the Editor assembly.
		/// (That is, if it has a folder named "Editor" in its path.) If not,
		/// the Editor folder is appended to the path.
		/// 
		/// No checks are done to ensure that the folders actually exist, nor
		/// are any folders created.
		/// </summary>
		/// <returns>
		/// The path to the nearest editor folder.
		/// </returns>
		/// <param name="initPath">Path to closes Editor folder.</param>
		public static string GetEditorFolder(string initPath) {
			string editorPath = "";

			if(!IsInEditorAssembly(initPath)) {
				editorPath = UnityPathUtils.Combine(initPath, "Editor");
			}
			else {
				editorPath = initPath;
			}

			return editorPath;
		}

		/// <summary>
		/// Returns true if the given path contains a folder named "Editor."
		/// </summary>
		/// <returns>True if there's a folder named "Editor," false otherwise.</returns>
		/// <param name="path">Path to check.</param>
		public static bool IsInEditorAssembly(string path) {
			return Regex.IsMatch(path, "([\\\\/]|\\A)Editor([\\\\/]|\\z)");
		}

		/// <summary>
		/// Combines the two paths. If path2 is an absolute path (of any kind),
		/// path2 is returned. (It's assumed that they cannot be combined in
		/// this case.)
		/// 
		/// Otherwise, path2 is appended to path1. A '/' is used as the
		/// delimiter, as Unity uses this one primarily.
		/// </summary>
		/// <returns>
		/// The combined result, i.e. path1 + path2, or path2.
		/// (See summary for more details.)
		/// </returns>
		/// <param name="path1">First path.</param>
		/// <param name="path2">Second path.</param>
		public static string Combine(string path1, string path2) {
			if(Path.IsPathRooted(path2) ||
			   Regex.IsMatch(path2, "[A-Z]:[/\\\\]")) {
				return path2;
			}
			else {
				path1 = path1.TrimEnd('/');

				return path1 + '/' + path2;
			}
		}


	} // End class
} // End namespace
