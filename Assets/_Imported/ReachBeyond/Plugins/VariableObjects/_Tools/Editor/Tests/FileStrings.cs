//
//  FileStrings.cs
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


﻿using NUnit.Framework;

namespace ReachBeyond.VariableObjects.Editor.Tests {

	[TestFixture]
	public class FileStrings {

		/// <summary>
		/// Ensures that the labels all stay the same. This is not something
		/// that should be changing without a very good reasons...if these
		/// change, then a lot of things are just going to break.
		/// </summary>
		[TestCase(ScriptFileManager.BaseLabel, "ReachBeyond.VariableObjects")]
		[TestCase(ScriptFileManager.UnityLabel, "ReachBeyond.VariableObjects.Unity")]
		[TestCase(ScriptFileManager.CustomLabel, "ReachBeyond.VariableObjects.Custom")]

		[TestCase(TemplateFileManager.EngineTemplateLabel, "ReachBeyond.VariableObjects.EngineTemplate")]
		[TestCase(TemplateFileManager.EditorTemplateLabel, "ReachBeyond.VariableObjects.EditorTemplate")]
		[TestCase(TemplateFileManager.ClassCompatibleTemplateLabel, "ReachBeyond.VariableObjects.ClassCompatible")]
		[TestCase(TemplateFileManager.StructCompatibleTemplateLabel, "ReachBeyond.VariableObjects.StructCompatible")]
		public void VerifyStrings(string classLabel, string expectedLabel) {
			Assert.That(classLabel, Is.EqualTo(expectedLabel));
		}

		public void VerifyClassIdentifier() {
			Assert.That(ScriptFileManager.ClassIdentifier, Is.EqualTo("Class"));
		}

		public void VerifyStructIdentifier() {
			Assert.That(ScriptFileManager.StructIdentifier, Is.EqualTo("Struct"));
		}
	}
}
