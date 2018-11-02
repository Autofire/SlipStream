//
//  VariableTypeBuilderTests.cs
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
using UnityEditor;
using System;
using System.IO;
using UnityEngine;

namespace ReachBeyond.VariableObjects.Editor.Tests {

	[TestFixture]
	public class VariableTypeBuilderTests {

		[TestCase("byte", true)]
		[TestCase("bool", true)]
		[TestCase("char", true)]
		[TestCase("decimal", true)]
		[TestCase("double", true)]
		[TestCase("float", true)]
		[TestCase("int", true)]
		[TestCase("long", true)]
		[TestCase("object", true)]
		[TestCase("sbyte", true)]
		[TestCase("short", true)]
		[TestCase("string", true)]
		[TestCase("uint", true)]
		[TestCase("ulong", true)]
		[TestCase("ushort", true)]
		[TestCase("hanzo", true)]
		[TestCase("melon", true)]
		[TestCase("_melon", true)]
		[TestCase("melon_", true)]
		[TestCase("if", false)]
		[TestCase("public", false)]
		[TestCase("void", false)]
		[TestCase("1man", false)]
		[TestCase("one man", false)]
		[TestCase("-hehe", false)]
		[TestCase("he-he", false)]
		[TestCase("hehe+", false)]
		[TestCase("he(he", false)]
		public void TestValidNameChecking(string name, bool expected) {
			Assert.That(
				VariableTypeBuilder.IsValidName(name),
				Is.EqualTo(expected)
			);
		}

	} // End class

} // End namespace
