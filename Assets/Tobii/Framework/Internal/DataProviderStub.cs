using System.Collections.Generic;
using Tobii.Gaming.Internal;

namespace Tobii.Gaming.Stubs
{
	internal class DataProviderStub<T> : IDataProvider<T> where T : ITimestamped
	{
		// --------------------------------------------------------------------
		//  Implementation of IDataProvider<T>
		// --------------------------------------------------------------------

		public T Last { get; protected set; }

		public IEnumerable<T> GetDataPointsSince(ITimestamped dataPoint)
		{
			return new List<T>();
		}

		public T GetFrameConsistentDataPoint()
		{
			return Last;
		}

		public void Start(int subscriberId)
		{
			// no implementation
		}

		public void Stop(int subscriberId)
		{
			// no implementation
		}
	}

	internal class GazePointDataProviderStub : DataProviderStub<GazePoint>
	{
		public GazePointDataProviderStub()
		{
			Last = GazePoint.Invalid;
		}
	}

	internal class HeadPoseDataProviderStub : DataProviderStub<HeadPose>
	{
		public HeadPoseDataProviderStub()
		{
			Last = HeadPose.Invalid;
		}
	}
}
