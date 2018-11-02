//
//  EventObjectInvoker.cs
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
using UnityEngine.Events;

namespace ReachBeyond.EventObjects {

	[System.Serializable]
	public class EventObjectInvoker {

		[SerializeField] EventObject  gameEvent;
		[SerializeField] UnityEvent unityEvent;

		/// <summary>
		/// Invoke the event. Same as Raise().
		/// </summary>
		public void Invoke() {
			if(gameEvent != null) {
				gameEvent.Raise();
			}

			unityEvent.Invoke();
		}

		/// <summary>
		/// Raise the event. Same as Invoke().
		/// </summary>
		public void Raise() {
			Invoke();
		}

		/// <summary>
		/// Determines whether this instance has any events to raise.
		/// </summary>
		/// <returns><c>true</c> if this instance has events; otherwise, <c>false</c>.</returns>
		public bool HasEvents() {
			return gameEvent != null || unityEvent.GetPersistentEventCount() > 0;
		}
	}

}
