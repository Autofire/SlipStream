//
//  BoolRefEditor.cs
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


using UnityEngine;
using UnityEditor;

namespace ReachBeyond.VariableObjects.Editor {

	[CustomPropertyDrawer(typeof(BoolReference))]
	public class BoolRefEditor : Base.Editor.RefEditor { }

	[CustomPropertyDrawer(typeof(BoolConstReference))]
	public class BoolConstRefEditor : Base.Editor.ConstRefEditor { }
}



/* DO NOT REMOVE -- START VARIABLE OBJECT INFO -- DO NOT REMOVE **
{
    "name": "Bool",
    "type": "bool",
    "referability": "Struct"
}
** DO NOT REMOVE --  END VARIABLE OBJECT INFO  -- DO NOT REMOVE */
