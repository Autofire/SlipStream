//
//  VariableTypeBuilder.cs
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
﻿
// Uncomment this to make it add the Builtin Unity label instead of the custom
// label.
//#define BUILTIN_MODE

using UnityEditor;
using Microsoft.CSharp;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace ReachBeyond.VariableObjects.Editor {

	public static class VariableTypeBuilder {

		#region Placeholder Strings
		private const string NamePlaceholder = "@Name@";
		private const string TypePlaceholder = "@Type@";
		private const string ReferablePlaceholder = "@Referable@";
		#endregion


		#region Variables and Constructor
		private readonly static CSharpCodeProvider provider;

		static VariableTypeBuilder() {
			provider = new CSharpCodeProvider();
		}
		#endregion


		/// <summary>
		/// Creates the new type of the variable based on the given parameters.
		/// After getting all of the correct templates, they will be copied
		/// into the folder specified by targetPath.
		/// 
		/// Some templates must be placed in an Editor folder. If one does not
		/// exist, one might be created. However, this function will avoid
		/// overwriting files. Once it is done, it will refresh the project.
		///
		/// Note that not all of the listen exceptions may get thrown. There
		/// are others which may get thrown by the file IO methods.
		/// </summary>
		/// 
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if any of the arguments are invalid. (See each argument
		/// for details.)
		/// </exception>
		/// 
		/// <exception cref="System.IO.DirectoryNotFoundException">
		/// If the targetPath is invalid.
		/// </exception>
		///
		/// <exception cref="System.IO.IOException">
		/// Thrown if the Editor folder cannot be created. Also thrown if any
		/// of the files we run into a pre-existing file.
		/// </exception>
		/// 
		/// <param name="readableName">
		/// The human readable name. Must be a legal C# name, and must be legal
		/// when used as a filename. The first character should be uppercase.
		/// If any of the checks fail or if the name is already taken
		/// by another variable object, then an ArgumentOutOfRangeException is
		/// thrown.
		/// </param>
		/// <param name="typeName">
		/// The name of the pre-existing C# type to be represtented. If the
		/// string fails the checks, then an ArgumentOutOfRangeException is thrown.
		/// </param>
		/// <param name="referability">
		/// Whether the C# type refered to by typeName is referable or not.
		/// If ReferabilityMode.Unknown is passed, ArgumentOutOfRangeException is thrown.
		/// </param>
		/// <param name="targetPath">
		/// Path used when attempting to create the new files. If this isn't a
		/// valid path, a DirectoryNotFoundException is thrown. If the
		/// target folder is nested in an Editor folder (or is an Editor folder
		/// itself), a ArgumentOutOfRangeException is thrown instead.
		/// </param>
		public static void CreateNewVariableType(
			string readableName,
			string typeName,
			ReferabilityMode referability,
			string targetPath
		) {

			// We'll use this to track any new files we make at any point,
			// which makes it easy to clean up after ourselves if things
			// go awry.
			List<string> newFilePaths = new List<string>();

			string editorPath = UnityPathUtils.GetEditorFolder(targetPath);

			if(!IsValidName(readableName)) {
				throw new System.ArgumentOutOfRangeException(
					"readableName",
					"Either contains invalid characters or could conflict with" +
					" a C# keyword."
				);
			}
			else if(ScriptFileManager.IsNameTaken(readableName)) {
				throw new System.ArgumentOutOfRangeException(
					"readableName",
					"Is already taken by another VariableObject type."
				);
			}
			else if(!IsValidName(typeName)) {
				throw new System.ArgumentOutOfRangeException(
					"typeName",
					"Either contains invalid characters or could conflict with" +
					" a C# keyword."
				);
			}
			else if(referability == ReferabilityMode.Unknown) {
				throw new System.ArgumentOutOfRangeException(
					"referability",
					"Must be something other than ReferabilityMode.unknown."
				);
			}
			else if(!AssetDatabase.IsValidFolder(targetPath)) {
				throw new DirectoryNotFoundException(
					"targetPath must be pointing to a pre-existing folder. If" +
					" you want to create a new folder to put the scripts in," +
					" you must do it before calling CreateNewVariableType."
				);
			}
			else if(UnityPathUtils.IsInEditorAssembly(targetPath)) {
				// If we tried putting all of our scripts in an Editor folder,
				// it would be mostly pointless because then we cannot use our
				// variable objects in the final build.
				throw new System.ArgumentOutOfRangeException(
					"targetPath",
					"Must not be nested in an Editor folder."
				);
			}
			else if(File.Exists(editorPath) && !AssetDatabase.IsValidFolder(editorPath)) {
				throw new IOException(
					editorPath + " exists as a file, so we cannot make a folder there!"
				);
			}


			// It's still possible that the editor folder doesn't exist.
			// However, we are sure that there isn't a file with that name,
			// so we can go ahead and create it if necessary.
			if(!AssetDatabase.IsValidFolder(editorPath)) {
				editorPath = AssetDatabase.GUIDToAssetPath(
					AssetDatabase.CreateFolder(targetPath, "Editor")
				);

				newFilePaths.Add(editorPath);
			}

			// At this point, everything is valid. Barring some kind of error
			// when writting to the disk, everything should be good to go.
			//
			// The only thing we are not going to check is if the files already
			// exist. We could check this before-hand, but this makes it a bit
			// more complex overall. We don't even gain very much in doing this;
			// we still need to handle an exception thrown by the file-writing
			// code, which may require cleaning up files.
			//
			// We shall build a list of all of the files which we create. That
			// way, we can delete them if something goes wrong.
			//
			//
			// Until after the AssetDatabase.Refresh in the finally block,
			// we cannot rely on Unity's AssetDatabase class. This only works
			// on files which are part of the project. However, if start we
			// refreshing, it'll refresh asynchroniously. Deleting files
			// during a refresh is risky, and may lead to errors and even lock
			// up Unity's reloading mechanisms.
			//
			// Fortunately, C# has enough tools to get the job done.

			try {
				EditorApplication.LockReloadAssemblies();

				foreach(TemplateInfo template in TemplateFileManager.Templates) {

					string newScriptPath = CreateScriptFromTemplate(
						readableName, typeName, referability,
						template, targetPath, editorPath
					);

					// Add it to the start so that, when iterating through
					// later on, the new folder would be at the back.
					if(!string.IsNullOrEmpty(newScriptPath)) {
						newFilePaths.Insert(0, newScriptPath);
					}

				} // End foreach(TemplateInfo template in Files.Templates) 
			} // End try
			catch(System.Exception e) {

				foreach(string path in newFilePaths) {

					if((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) {
						Directory.Delete(path);
					}
					else {
						File.Delete(path);
					}

					if(File.Exists(path + ".meta")) {
						File.Delete(path + ".meta");
					}
				}

				throw e;
			}
			finally {

				EditorApplication.UnlockReloadAssemblies();
				AssetDatabase.Refresh();
			}

			SetFileLabels(newFilePaths.ToArray());

		} // End CreateNewVariableType


		#region File Creation
		/// <summary>
		/// Creates a script from the given template.
		///
		/// This function does very little checking...it assumes that all names
		/// are valid C# for identifiers.
		/// 
		/// Be wary that this may throw exceptions. (See StreamReader.ReadToEnd(),
		/// StreamWriter.Write(), StreamReader's contructor, and StreamWriter's
		/// constructor.)
		///
		/// This does not issue a refresh, nor does it add any labels.
		/// </summary>
		/// <returns>
		/// The relative path (starting with the project root) to the newly
		/// created file. If it wasn't created (i.e. referability doesn't match
		/// with the types supported by the template), and empty string is
		/// returned instead.
		/// </returns>
		/// <param name="readableName">Human readable name.</param>
		/// <param name="typeName">C# name of type to be supported.</param>
		/// <param name="referability">
		/// Referability mode associated with the C# type named by typeName.
		/// </param>
		/// <param name="template">Info for the template.</param>
		/// <param name="normalPath">Path for non-editor scripts.</param>
		/// <param name="editorPath">Path for editor scripts.</param>
		private static string CreateScriptFromTemplate(
			string readableName,
			string typeName,
			ReferabilityMode referability,
			TemplateInfo template,
			string normalPath,
			string editorPath
		) {

			string templatePath = "";   // Path of the template file
			string newFileName = "";    // Name of the new file
			string newFilePath = "";    // Full path of new file (including name)

			// Before attempting to copy the template, we'll check if it
			// even matches what we need.
			if(template.IsCompatibleWith(referability)) {

				templatePath = template.path;

				newFileName = ReplaceTemplatePlaceholders(
					Path.GetFileNameWithoutExtension(templatePath),
					readableName, typeName, referability.ToString()
				) + ".cs";

				newFilePath = UnityPathUtils.Combine(
					(template.IsEngineTemplate ? normalPath : editorPath),
					newFileName
				);

				if(File.Exists(newFilePath)) {
					throw new IOException(newFilePath + " already exists!");
				}

				StreamReader templateReader = null;
				string templateContents = "";
				try {
					// After changing the conde in this block, revise the
					// exception documentaion above.
					templateReader = new StreamReader(templatePath);
					templateContents = templateReader.ReadToEnd();
				}
				finally {
					if(templateReader != null) {
						templateReader.Close();
					}
				}


				string newScriptContents = ReplaceTemplatePlaceholders(
					templateContents,
					readableName, typeName, referability.ToString()
				);


				StreamWriter scriptWriter = null;
				try {
					// After changing the conde in this block, revise the
					// exception documentaion above.
					scriptWriter = new StreamWriter(newFilePath);
					scriptWriter.Write(newScriptContents);
				}
				finally {
					if(scriptWriter != null) {
						scriptWriter.Close();
					}
				}

			} // End if( ... )

			return newFilePath;
		} // End CreateScriptFromTemplate


		/// <summary>
		/// Given the set of paths, applies the ScriptFileManager.CustomLabel
		/// to each file. Ignores folders, and doesn't preserve pre-existing
		/// labels.
		///
		/// It's probably best to reload before running this.
		/// </summary>
		/// <param name="paths">File paths.</param>
		private static void SetFileLabels(string[] paths) {
			foreach(string path in paths) {

				if(!AssetDatabase.IsValidFolder(path)) {
					UnityEngine.Object newFileObj =
						AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

#if BUILTIN_MODE
					AssetDatabase.SetLabels(
						newFileObj,
						new string[] { ScriptFileManager.UnityLabel }
					);
#else
					AssetDatabase.SetLabels(
						newFileObj,
						new string[] { ScriptFileManager.CustomLabel }
					);
#endif


				} // End if
			} // End foreach
		} // End AddLabels


		/// <summary>
		/// Replaces the placeholder text in the templateText string with the
		/// strings passed in.
		/// </summary>
		/// <returns>The built template.</returns>
		/// <param name="templateText">Template text.</param>
		/// <param name="name">Human readable name.</param>
		/// <param name="type">C# type name.</param>
		/// <param name="referability">Referability.</param>
		private static string ReplaceTemplatePlaceholders(
			string templateText,
			string name,
			string type,
			string referability
		) {
			templateText = Regex.Replace(
				templateText, NamePlaceholder, name
			);

			templateText = Regex.Replace(
				templateText, TypePlaceholder, type
			);

			templateText = Regex.Replace(
				templateText, ReferablePlaceholder, referability
			);

			return templateText;
		}
#endregion


#region Argument Checking
		/// <summary>
		/// Checks if the given name is valid. Is true if the given string
		/// contains only valid characters and does not conflict with any of the
		/// non-datatype keywords. (i.e. 'this' and 'class' are invalid, but
		/// 'int' and 'bool' are valid.)
		/// </summary>
		/// <returns><c>true</c>, if the name is valid.</returns>
		/// <param name="targetName">Name to check.</param>
		public static bool IsValidName(string targetName) {
			// IsValidIdentifier is nice, but it blocks out things like 'int'
			// I'm not sure if there is a better solution to this problem.
			string[] builtinTypeNames = {
				"bool",  "byte",   "char", "decimal", "double",
				"float", "int",    "long", "object",  "sbyte",
				"short", "string", "uint", "ulong",   "ushort"
			};	

			return builtinTypeNames.Contains(targetName)
				   || provider.IsValidIdentifier(targetName);
		}
#endregion

	} // End class

} // End namespace
