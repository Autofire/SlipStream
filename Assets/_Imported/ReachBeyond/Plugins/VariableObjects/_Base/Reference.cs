//
//  Reference.cs
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

namespace ReachBeyond.VariableObjects.Base {

	/// <summary>
	/// A reference to a variable. It can be both accessed and modified.
	/// </summary>
	/// <typeparam name="Type">Core variable type.</typeparam>
	/// <typeparam name="VarType">Variable type; must match up with 'Type'.</typeparam>
	[System.Serializable]
	public class Reference<Type, VarType> : ConstReference<Type, VarType>
		where VarType: Variable<Type> {

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public Type Value {
			get {
				return ConstValue;
			}
			set {
				if(UseInternal) {
					internalValue = value;
				}
				else {
					variable.StoredValue = value;
				}
			}
		} // End value
	
	} // End Reference

} // End namespace
