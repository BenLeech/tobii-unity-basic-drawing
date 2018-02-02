//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Base class for data streams.
	/// </summary>
	/// <typeparam name="T">Type of the provided data value object.</typeparam>
	internal abstract class DataProviderBase<T> : IDataProvider<T> where T : ITimestamped
	{
		private readonly Dictionary<int, int> _subscribers = new Dictionary<int, int>();
		private readonly List<T> _lastDataPoints = new List<T>();
		private const float PruneIntervalSecs = 2.0f;
		private float _pruneLastDataPointsTimer = PruneIntervalSecs;
		private T _last;
		private T _lastReadInFrame;
		private bool _isLastReadInFrame;


		private bool IsStarted
		{
			get { return _subscribers.Count > 0; }
		}

		// --------------------------------------------------------------------
		//  Implementation of IDataProvider<T>
		// --------------------------------------------------------------------

		/// <summary>
		/// Gets or sets the latest value of the data stream. The value is never null but 
		/// it might be invalid.
		/// </summary>
		public T Last
		{
			get
			{
				_lastReadInFrame = _last;
				_isLastReadInFrame = true;
				return _last;
			}

			protected set
			{
				_lastDataPoints.Add(value);
				_last = value;
			}
		}

		/// <summary>
		/// Gets the last possible data value that is also consistent with previous
		/// reads in the frame. As soon as the Last value is accessed, or this
		/// function is called in a frame, all subsequent calls to this function 
		/// within that frame will return the same value.
		/// </summary>
		/// <returns>The last data point that can be consistently read in the frame.</returns>
		public T GetFrameConsistentDataPoint()
		{
			if (!_isLastReadInFrame)
			{
				return Last;
			}

			return _lastReadInFrame;
		}

		/// <summary>
		/// Gets all data points since the supplied data point. 
		/// Points older than 500 ms will not be included.
		/// </summary>
		public IEnumerable<T> GetDataPointsSince(ITimestamped dataPoint)
		{
			var dataPointTimestamp = dataPoint.IsValid ? dataPoint.Timestamp : 0.0;

			return _lastDataPoints.FindAll(point =>
				(point.Timestamp > dataPointTimestamp) &&
				(point.Timestamp > Time.unscaledTime - 0.5f));
		}

		/// <summary>
		/// Starts the provider. Data will continuously be updated in the Last
		/// property as events are received from Tobii Engine.
		/// </summary>
		public void Start(int subscriberId)
		{
			var oldCount = _subscribers.Count;

			_subscribers[subscriberId] = subscriberId;
			if ((oldCount == 0) && (_subscribers.Count == 1))
			{
				OnStreamingStarted();
			}
		}

		/// <summary>
		/// Requests to stop the data provider. If there are no other clients
		/// that are currently requesting the provider to keep providing data,
		/// the provider will stop the stream of data from Tobii Engine and
		/// stop updating the Last property.
		/// </summary>
		public void Stop(int subscriberId)
		{
			_subscribers.Remove(subscriberId);

			if (_subscribers.Count == 0)
			{
				OnStreamingStopped();
			}
		}

		internal void Disconnect()
		{
			if (_subscribers.Count == 0)
			{
				OnStreamingStopped();
			}
		}

		// --------------------------------------------------------------------
		//  Implementation of IDataProviderInternal
		// --------------------------------------------------------------------

		/// <summary>
		/// Gets the unique ID of the data stream.
		/// </summary>
		internal abstract string Id
		{
			get;
		}

		/// <summary>
		/// Signals the end of the frame. Perform end-of-frame cleanup of persisted state.
		/// </summary>
		internal void EndFrame()
		{
			_isLastReadInFrame = false;
			_lastReadInFrame = default(T);
		}

		protected void Cleanup()
		{
			_pruneLastDataPointsTimer += Time.unscaledDeltaTime;
			if (_pruneLastDataPointsTimer > PruneIntervalSecs)
			{
				PruneLastDataPoints(Time.unscaledTime - 0.5f);
			}
		}

		// --------------------------------------------------------------------
		//  Protected and private methods
		// --------------------------------------------------------------------

		private void PruneLastDataPoints(float minimumTimestamp)
		{
			_lastDataPoints.RemoveAll(point => point.Timestamp < minimumTimestamp);
		}

		protected virtual void OnStreamingStarted()
		{
			// default implementation does nothing
		}

		protected virtual void OnStreamingStopped()
		{
			// default implementation does nothing
		}
	}
}
#endif
