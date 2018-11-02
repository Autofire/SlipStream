//
//  Registrater.cs
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
using UnityEngine.Assertions;

namespace ReachBeyond.VariableObjects.Base {

	public class Registrater<Type, VarType> : MonoBehaviour
		where VarType : ClassVariable<Type>
		where Type : class {

		enum ConflictSolution {
			OverwriteOther,
			KeepOther,
			KeepOtherAndDeleteSelf
		};

		enum CleanupTrigger {
			OnDisable,
			OnDestroy
		}

		[SerializeField] VarType variable;
		[SerializeField] Type targetValue;

		[Space(10)]
		[SerializeField] ConflictSolution uponConflict;
		[SerializeField] CleanupTrigger cleanupSignal;

		private void Awake() {
			Assert.IsNotNull(variable);
			Assert.IsNotNull(targetValue);
		}

		private void OnEnable() {

			// There seems to be a bug in C# that causes the normal check,
			// (variable.value == null), to ALWAYS return false, even when
			// variable.value really is null. This is despite labeling the
			// Type parameter as being a class (so it can be made null) and
			// using the NullableVariable, which also requires the value to
			// be nullable.
			//
			// However, we can always get the ToString, and it will return
			// null if the value is truly null. This is a bit of a hack,
			// though, and it is rather inefficient.
			//
			// That being said, there are times when variable.value *does*
			// turn up as being null, so it's important to check anyway.
			if(variable.StoredValue == null || variable.StoredValue.ToString() == "null") {
				variable.StoredValue = targetValue;
			}
			else if(variable.StoredValue != targetValue) {
				switch(uponConflict) {
					case ConflictSolution.OverwriteOther:
						variable.StoredValue = targetValue;
						break;

					case ConflictSolution.KeepOtherAndDeleteSelf:
						Destroy(gameObject);
						break;

					case ConflictSolution.KeepOther:
						break;
				}
			}
		}

		private void OnDisable() {
			if(cleanupSignal == CleanupTrigger.OnDisable && variable.StoredValue == targetValue) {
				variable.StoredValue = null;
			}
		}

		private void OnDestroy() {
			if(cleanupSignal == CleanupTrigger.OnDestroy && variable.StoredValue == targetValue) {
				variable.StoredValue = null;
			}
		}

	} // End class
} // End namespace
