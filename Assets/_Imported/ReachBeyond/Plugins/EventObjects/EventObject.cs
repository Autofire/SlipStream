//
//  EventObject.cs
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


﻿using System.Collections.Generic;
using UnityEngine;

namespace ReachBeyond.EventObjects {

	[CreateAssetMenu(menuName = "EventObject", order = -20100)]
	public class EventObject : ScriptableObject {

		private List<EventObjectListener> listeners = new List<EventObjectListener>();

		/// <summary>
		/// Raise the event, alerting all listeners that the event has been triggered.
		/// </summary>
		public void Raise() {
			for(int i = listeners.Count - 1; i >= 0; i--) {
				listeners[i].OnRaiseEvent();
			}
		}

		/// <summary>
		/// Adds a listener to be signaled when the event is raised.
		/// </summary>
		/// <param name="listener">Listener to add.</param>
		/// <returns>True if successfully registered; false otherwise.</returns>
		public bool RegisterListener(EventObjectListener listener) {
			// If this ever bogs down, begin using a HashSet in addition
			// to a list. However, this could get quite memory heavy if we
			// did it all the time.
			if(!listeners.Contains(listener)) {
				listeners.Add(listener);
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Unregisters the listener.
		/// </summary>
		/// <param name="listener">Listener to unregister.</param>
		/// <returns>True if successfully unregistered; false otherwise.</returns>
		public bool UnregisterListener(EventObjectListener listener) {
			return listeners.Remove(listener);
		}

		// If we handled args:
		//
		// Listeners without args can recieve all events
		// Listers with args can only recieve events with args
		//
		// Events with args can be sent to all listeners
		// Events without args can only be sent to listeners without args
	}

}
