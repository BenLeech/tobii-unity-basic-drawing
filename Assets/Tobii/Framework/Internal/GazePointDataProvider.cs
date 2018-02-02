//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using Tobii.GameIntegration;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Provider of gaze point data. When the provider has been started it
	/// will continuously update the Last property with the latest gaze point 
	/// value received from Tobii Engine.
	/// </summary>
	internal class GazePointDataProvider : DataProviderBase<GazePoint>
	{
		private readonly ITobiiHost _tobiiHost;

		/// <summary>
		/// Creates a new instance.
		/// Note: don't create instances of this class directly. Use the <see cref="TobiiHost.GetGazePointDataProvider"/> method instead.
		/// </summary>
		/// <param name="eyeTrackingHost">Eye Tracking Host.</param>
		public GazePointDataProvider(ITobiiHost tobiiHost)
		{
			_tobiiHost = tobiiHost;
			Last = GazePoint.Invalid;
		}

		protected override void OnStreamingStarted()
		{
			Interop.SubscribeToStream(TobiiSubscription.TobiiSubscriptionStandardGaze);
		}

		protected override void OnStreamingStopped()
		{
			Interop.UnsubscribeFromStream(TobiiSubscription.TobiiSubscriptionStandardGaze);
		}

		internal override string Id
		{
			get { return "GazePointDataStream"; }
		}

		internal void Update()
		{
			var gazePoints = Interop.GetNewGazePoints(UnitType.Normalized);
			foreach (var gazePoint in gazePoints)
			{
				OnGazePoint(gazePoint);
			}

			Cleanup();
		}

		private void OnGazePoint(GameIntegration.GazePoint gazePoint)
		{
			long eyetrackerCurrentUs = gazePoint.TimeStampMicroSeconds; // TODO awaiting new API from tgi;
			float timeStampUnityUnscaled = Time.unscaledTime - ((eyetrackerCurrentUs - gazePoint.TimeStampMicroSeconds) / 1000000f);

			var bounds = _tobiiHost.GameViewInfo.NormalizedClientAreaBounds;

			if (float.IsNaN(bounds.x)
				|| float.IsNaN(bounds.y)
				|| float.IsNaN(bounds.width)
				|| float.IsNaN(bounds.height)
				|| bounds.width < float.Epsilon
				|| bounds.height < float.Epsilon)
				return;

			var x = (gazePoint.X - bounds.x) / bounds.width;
			var y = (gazePoint.Y - bounds.y) / bounds.height;
			Last = new GazePoint(new Vector2(x, 1 - y), timeStampUnityUnscaled, gazePoint.TimeStampMicroSeconds);
		}
	}
}
#endif
