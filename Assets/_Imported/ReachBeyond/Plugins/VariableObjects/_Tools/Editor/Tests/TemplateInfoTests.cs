//
//  TemplateInfoTests.cs
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
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NUnit.Framework.Constraints;

namespace ReachBeyond.VariableObjects.Editor.Tests {

	[TestFixture]
	public class TemplateInfoTests {

		[TestCase("Hello",  true, true, true)]
		[TestCase("Break",  true, true, false)]
		[TestCase("Mario",  true, false, true)]
		[TestCase("Luigi",  true, false, false)]
		[TestCase("Almond", false, true, true)]
		[TestCase("Food",   false, true, false)]
		[TestCase("Bye",    false, false, true)]
		[TestCase("Lemon",  false, false, false)]
		public void TestConstructor(string guid, bool structComp, bool classComp, bool isEditor) {
			// ASSIGN & ACT
			TemplateInfo info = new TemplateInfo(
				guid, structComp, classComp, isEditor
			);

			// ASSERT
			Assert.That(info.GUID, Is.EqualTo(guid));
			Assert.That(info.StructCompatible, Is.EqualTo(structComp));
			Assert.That(info.ClassCompatible, Is.EqualTo(classComp));
			Assert.That(info.IsEditorTemplate, Is.EqualTo(isEditor));
			Assert.That(info.IsEngineTemplate, Is.EqualTo(!isEditor));

		}

	}
}
