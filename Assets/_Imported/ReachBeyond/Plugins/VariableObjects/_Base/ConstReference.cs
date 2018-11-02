//
//  ConstReference.cs
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
	/// A constant reference to variable. It cannot be changed.
	/// </summary>
	/// <typeparam name="Type">Core variable type.</typeparam>
	/// <typeparam name="VarType">Variable type; must match up with 'Type'.</typeparam>
	[System.Serializable]
	public class ConstReference<Type, VarType>
		where VarType: Variable<Type> {

		[SerializeField] private bool _useInternal = true;

		[SerializeField] protected Type internalValue;
		[SerializeField] protected VarType variable;

		protected bool UseInternal {
			get {
				if(_useInternal) {
					return true;
				}
				else if(variable == null) {
					Debug.LogWarning(
						"Reference of type " + internalValue.GetType().Name
						+ " tried to reference a variable, but came up empty!\n"
						+ "Falling back to internal value."
					);

					return true;
				}
				else {
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the readonly value; you cannot re-assign into this.
		/// In the case of mutable reference datatypes (like classes), the
		/// original can still be changed, but the referenced object cannot.
		/// </summary>
		/// <value>The const value.</value>
		public Type ConstValue {
			get {
				return UseInternal ? internalValue : variable.StoredValue;
			}
		} // End constValue

		/// <summary>
		/// Implicitly returns ConstValue.
		/// </summary>
		/// <returns>The stored value.</returns>
		/// <param name="constRef">The ConstReference object.</param>
		public static implicit operator Type(
			ConstReference<Type, VarType> constRef
		) {
			return constRef.ConstValue;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current
		/// value of ConstValue.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="T:System.String"/> that represents the current
		/// value of ConstValue.
		/// </returns>
		public override string ToString() {
			return ConstValue.ToString();
		}


	} // End ConstReference

} // End namespace
