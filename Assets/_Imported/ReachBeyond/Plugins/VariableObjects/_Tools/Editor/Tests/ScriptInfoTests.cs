//
//  ScriptInfoTests.cs
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

namespace ReachBeyond.VariableObjects.Editor.Tests {

	[TestFixture]
	public class ScriptInfoTests {

		[Test]
		public void TestConstructor() {
			// ARRANGE
			string name = "TestJo";
			string type = "Chromalisk";
			ReferabilityMode mode = ReferabilityMode.Class;

			// ACT
			ScriptInfo info = new ScriptInfo(name, type, mode);

			// ASSERT
			Assert.That(info.Name, Is.EqualTo(name));
			Assert.That(info.Type, Is.EqualTo(type));
			Assert.That(info.Referability, Is.EqualTo(mode));
		}

		[Test]
		public void TestGUIDAdding() {
			// ARRANGE
			string name = "Ash";
			string type = "Fool";
			string[] monsters = new string[] {
				"Pikachu", "Charmander", "Squirtle", "Bulbasaur"
			};

			// ACT
			ScriptInfo info = new ScriptInfo(name, type);
			foreach(string GUID in monsters) {
				info.AddGUID(GUID);
			}
			info.AddGUID(monsters[0]);          // Should have no effect
			info.AddGUID("");					// Also should have no effect

			// ASSERT
			Assert.That(info.GUIDs.Length, Is.EqualTo(monsters.Length));
			for(int i = 0; i < monsters.Length; i++) {
				Assert.That(info.GUIDs[i], Is.EqualTo(monsters[i]));
			}

		}

	}
}
