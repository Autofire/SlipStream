//
//  ConstRefEditor.cs
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
using UnityEditor;

namespace ReachBeyond.VariableObjects.Base.Editor {
	public class ConstRefEditor : RefEditor {

		protected override string GetButtonText(bool useInternal) {
			return (useInternal ? "CVal" : "CRef");
		}

		protected override Color GetButtonContentColor(bool useInternal) {
			return (EditorGUIUtility.isProSkin ? Color.white : Color.black);
		}

		protected override Color GetButtonBackgroundColor(bool useInternal) {
			return (EditorGUIUtility.isProSkin ? Color.grey : Color.white);
		}

		protected override string GetButtonTooltip(bool useInternal) {
			if(useInternal) {
				return
					"The value is unique to this instance of this component," +
					" and it may never change.";
			}
			else {
				return
					"The value matches that of the specified variable. (If no" +
					" variable is provided, the internal value is used intead.)" +
					"\n\n" +
					"The variable's contents may not be changed by this object.";
			}
		}

	} // End class
} // End namespace
