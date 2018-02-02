//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using Tobii.Gaming.Internal;

namespace Tobii.Gaming.Stubs
{
	internal class StateValueStub<T> : EventArgs, IStateValue<T>
	{
		public StateValueStub(T value) { IsValid = false; }
		public StateValueStub() { IsValid = false; }
		public static IStateValue<T> Invalid { get { return new StateValueStub<T>(); } }
		public T Value { get; private set; }
		public bool IsValid { get; private set; }
		public override string ToString() { return "INVALID"; }
	}
}
