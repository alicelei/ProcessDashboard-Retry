﻿using System;
namespace ProcessDashboard
{
	public enum NetworkAvailabilityStates { BecameAvailable, BecameUnavailable };
	public enum TimeLoggingControllerStates 
	{ 
		TimeLogCreated, 	// successfully created the new timelog in the PDES
		TimeLogUpdated, 	// the once-per-minute update to the PDES succeed
		TimeLogUpdateFailed, 	// the server doesn't know the lastest time log data. The time log is either not created or not updated
		TimeLogCanceled 	// PDES reported a confliction, and requires the mobile app to stop this timmer
	};

	public class StateChangedEventArgs : EventArgs
	{
		private Enum newState;
		private String message;

		public StateChangedEventArgs(Enum newState, String message)
		{
			this.newState = newState;
			this.message = message;
		}

		public Enum NewState
		{
			get { return newState; }
		}

		public String Message
		{
			get { return message; }
		}
	}
}

