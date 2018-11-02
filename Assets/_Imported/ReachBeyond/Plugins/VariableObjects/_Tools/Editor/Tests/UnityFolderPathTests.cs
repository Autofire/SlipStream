//
//  UnityFolderPathTests.cs
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
using System.IO;
using UnityEngine;

namespace ReachBeyond.VariableObjects.Editor.Tests {

	[TestFixture]
	public class UnityFolderPathTests {

		[Test]
		public void TestProjectExclusivity() {
			string initPath = "Assets/ReachBeyond";
			UnityFolderPath pathObj = new UnityFolderPath(initPath);

			pathObj.Path = Path.GetPathRoot(
				Path.GetPathRoot( Application.dataPath )
			);

			Assert.That(pathObj.Path, Is.EqualTo(initPath));
		}

		[Test]
		public void TestUnchangingValue() {
			string initPath = "Assets";
			UnityFolderPath pathObj = new UnityFolderPath(initPath);

			pathObj.Path = "NonExistantMelonHead";

			Assert.That(pathObj.Path, Is.EqualTo(initPath));
		}
	} // End class

} // End namespace
