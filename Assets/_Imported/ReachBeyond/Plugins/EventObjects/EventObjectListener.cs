//
//  EventObjectListener.cs
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

	public class EventObjectListener : MonoBehaviour {

		[SerializeField] EventObject _triggerEvent;
		[SerializeField] UnityEvent _response;

		/// <summary>
		/// Gets/sets the triggerEvent object. Note that changing this
		/// value will cause the listener to automatically re-register.
		///
		/// Making this null effectively disables the event.
		/// </summary>
		public EventObject triggerEvent {
			set {
				if(_triggerEvent != value) {
					TryUnregister();
					_triggerEvent = value;
					TryRegister();
				}
			}
			get { return _triggerEvent; }
		}

		/// <summary>
		/// Gets/sets the response. Making this null effectively disables
		/// the event.
		/// </summary>
		public UnityEvent response {
			set { _response = value; }
			get { return _response; }
		}

		/// <summary>
		/// Invoke the selected response. If response is set to
		/// null, then nothing happens.
		/// </summary>
		public void OnRaiseEvent() {
			if(response != null) {
				response.Invoke();
			}
		}


		#region Registration helpers

		/// <summary>
		/// Attempts to register to triggerEvent.
		/// </summary>
		private void TryRegister() {
			if(triggerEvent != null) {
				triggerEvent.RegisterListener(this);
			}
		}

		/// <summary>
		/// Attempts to unregister from triggerEvent.
		/// </summary>
		private void TryUnregister() {
			if(triggerEvent != null) {
				triggerEvent.UnregisterListener(this);
			}
		}

		#endregion


		#region Unity events

		private void OnEnable() {
			TryRegister();
		}

		private void OnDisable() {
			TryUnregister();
		}

		#endregion


	}

}
