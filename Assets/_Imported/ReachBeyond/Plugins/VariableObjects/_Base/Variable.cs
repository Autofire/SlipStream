//
//  Variable.cs
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
using ReachBeyond.EventObjects;

namespace ReachBeyond.VariableObjects.Base {

	/// <summary>
	/// An object which holds a variable.
	///
	/// You should never use this directly. See ClassVariable and StructVariable.
	/// </summary>
	/// <typeparam name="T">Type which this variable object stores.</typeparam>
	public abstract class Variable<T> : EventObject {

		[Header("Config")]
		[Tooltip("This value gets applied when the game loads.")]
		[SerializeField] private T _defaultValue;

		[Header("Debugging")]
		[Tooltip("This is only exposed so that you may see the value during " +
		         "runtime and make changes to it."
		        )]
		[SerializeField] private T _value;

		public virtual void OnEnable() {
			_value = _defaultValue;
			this.hideFlags = HideFlags.DontUnloadUnusedAsset;
		}

		/// <summary>
		/// Gets or sets the value. Note that setting this ALWAYS raises the event, even if the new value is the same.
		/// </summary>
		/// <value>The value which is being held.</value>
		public T StoredValue {
			set {
				_value = value;
				Raise();
			}
			get { return _value; }
		}

		/// <summary>
		/// Whether the value being stored is null. The existance of this property does not mean that the variable is a
		/// nullable type.
		/// </summary>
		/// <value><c>true</c> if storedValue == null; otherwise, <c>false</c>.</value>
		public abstract bool IsNull {
			get;
		}
	}
}
