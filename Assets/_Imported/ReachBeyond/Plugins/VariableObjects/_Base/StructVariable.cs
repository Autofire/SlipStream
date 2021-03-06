//
//  StructVariable.cs
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

namespace ReachBeyond.VariableObjects.Base {

	/// <summary>
	/// An object which stores a variable. You should almost never use this directly in your code. Instead, create a
	/// class which inherits from this object. This way, Unity will know how to handle your specific usage.
	///
	/// Inheritors of this class must use a non-nullable, value-based type, such as a struct.
	/// </summary>
	/// <typeparam name="T">
	/// Type which this variable object stores. 
	/// Must be a struct or other value-based type.
	/// </typeparam>
	public class StructVariable<T> : Variable<T> where T : struct {	

		public override bool IsNull {
			get {
				// Note that _value will never be null, and it is illegal to do the check directly.
				// Thus, this is never true.
				return false;
			}
		}
	}
}
