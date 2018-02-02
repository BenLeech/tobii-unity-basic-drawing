//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Interface of an EyeTracking data provider.
	/// </summary>
	/// <typeparam name="T">Type of the provided data value object.</typeparam>
	internal interface IDataProvider<T> where T : ITimestamped
	{
		/// <summary>
		/// Gets the latest value of the data stream. The value is never null but 
		/// it might be invalid.
		/// </summary>
		T Last { get; }

		/// <summary>
		/// Gets the last possible data value that is also consistent with previous
		/// reads in the frame. As soon as the Last value is accessed, or this
		/// function is called in a frame, all subsequent calls to this function 
		/// within that frame will return the same value.
		/// </summary>
		/// <returns>The last data point that can be consistently read in the frame.</returns>
		T GetFrameConsistentDataPoint();

		/// <summary>
		/// Gets all data points since the supplied data point. 
		/// Points older than 500 ms will not be included.
		/// </summary>
		IEnumerable<T> GetDataPointsSince(ITimestamped dataPoint);

		/// <summary>
		/// Starts the provider. Data will continuously be updated in the Last
		/// property as events are received from Tobii Engine.
		/// </summary>
		void Start(int subscriberId);

		/// <summary>
		/// Requests to stop the data provider. If there are no other clients
		/// that are currently requesting the provider to keep providing data,
		/// the provider will stop the stream of data from Tobii Engine and
		/// stop updating the Last property.
		/// </summary>
		void Stop(int subscriberId);
	}
}
