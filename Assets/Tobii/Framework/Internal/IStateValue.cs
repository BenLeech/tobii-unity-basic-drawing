//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

namespace Tobii.Gaming.Internal
{
	internal interface IStateValue<T>
	{
		/// <summary>
		/// Gets the state value.
		/// </summary>
		T Value { get; }

		/// <summary>
		/// Gets a value indicating whether the state value is valid.
		/// The state will always be unknown when disconnected from Tobii Engine.
		/// </summary>
		bool IsValid { get; }
	}
}
