//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using System.Collections;
using Tobii.GameIntegration;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Tobii.Gaming.Internal
{
	internal class TobiiHost : MonoBehaviour, ITobiiHost
	{
		private static bool _isConnected;
		private static TobiiHost _instance;
		private static bool _isShuttingDown;

		private GameViewBoundsProvider _gameViewBoundsProvider;
		private GameViewInfo _gameViewInfo = GameViewInfo.DefaultGameViewInfo;

		private GazePointDataProvider _gazePointDataProvider;
		private HeadPoseDataProvider _headPoseDataProvider;
		private GazeFocus _gazeFocus;
		private bool _updatedInFrame;

		private static bool HasDisplayedEulaError;

		//--------------------------------------------------------------------
		// Public Function and Properties
		//--------------------------------------------------------------------

		public static ITobiiHost GetInstance()
		{
			if (_isShuttingDown || !TobiiEulaFile.IsEulaAccepted())
			{
				if (!TobiiEulaFile.IsEulaAccepted() && !HasDisplayedEulaError)
				{
					Debug.LogError("You need to accept EULA to be able to use Tobii Unity SDK.");
					HasDisplayedEulaError = true;
				}
				return new Stubs.TobiiHostStub();
			}

			if (_instance != null) return _instance;

			var newGameObject = new GameObject("TobiiHost");
			DontDestroyOnLoad(newGameObject);
			_instance = newGameObject.AddComponent<TobiiHost>();
			return _instance;
		}


		public void Shutdown()
		{
			_isShuttingDown = true;

			Disconnect();
		}

		public DisplayInfo DisplayInfo
		{
			get
			{
				int displayHeight;
				int displayWidth;
				Interop.GetScreenSizeMm(out displayWidth, out displayHeight);
				return new DisplayInfo(displayWidth, displayHeight);
			}
		}

		public GameViewInfo GameViewInfo
		{
			get { return _gameViewInfo; }
		}

		public UserPresence UserPresence
		{
			get
			{
				var presence = Interop.GetUserPresence();
				switch (presence)
				{
					case GameIntegration.UserPresence.Away:
						return UserPresence.NotPresent;
					case GameIntegration.UserPresence.Present:
						return UserPresence.Present;
					default:
						return UserPresence.Unknown;
				}
			}
		}

		public bool IsInitialized
		{
			get { return _isConnected; }
		}

		public IGazeFocus GazeFocus
		{
			get { return _gazeFocus; }
		}

		public IDataProvider<GazePoint> GetGazePointDataProvider()
		{
			SyncData();
			return _gazePointDataProvider;
		}

		public IDataProvider<HeadPose> GetHeadPoseDataProvider()
		{
			SyncData();
			return _headPoseDataProvider;
		}


		//--------------------------------------------------------------------
		// MonoBehaviour Messages
		//--------------------------------------------------------------------

		void Awake()
		{
#if UNITY_EDITOR
			_gameViewBoundsProvider = CreateEditorScreenHelper();
#else
			_gameViewBoundsProvider = new UnityPlayerGameViewBoundsProvider();
#endif
			_gazeFocus = new GazeFocus();

			_gazePointDataProvider = new GazePointDataProvider(this);
			_headPoseDataProvider = new HeadPoseDataProvider();
		}

		void Update()
		{
			if (_gameViewBoundsProvider.Hwnd != IntPtr.Zero)
			{
				Interop.SetWindow(_gameViewBoundsProvider.Hwnd);
				if (!_isConnected)
				{
					Interop.Start(false);

					_gazePointDataProvider.Disconnect();
					_headPoseDataProvider.Disconnect();
					_isConnected = true;
				}
			}

			SyncData();

			var gameViewBounds = _gameViewBoundsProvider.GetGameViewClientAreaNormalizedBounds();
			_gameViewInfo = new GameViewInfo(gameViewBounds);

			_gazeFocus.UpdateGazeFocus();

			StartCoroutine(DoEndOfFrameCleanup());
		}

		void OnDestroy()
		{
			Shutdown();
		}

		void OnApplicationQuit()
		{
			Shutdown();
		}

		//--------------------------------------------------------------------
		// Private Functions
		//--------------------------------------------------------------------

		private void SyncData()
		{
			if (_updatedInFrame) return;

			_updatedInFrame = true;
		
			var result = Interop.Update();
			if (result)
			{
				_gazePointDataProvider.Update();
				_headPoseDataProvider.Update();
			}
		}

		private IEnumerator DoEndOfFrameCleanup()
		{
			yield return new WaitForEndOfFrame();

			_updatedInFrame = false;
			_gazePointDataProvider.EndFrame();
			_headPoseDataProvider.EndFrame();
		}

		private void Disconnect()
		{
			if (_isConnected)
			{
				Interop.Stop();
				_isConnected = false;
			}
		}

#if UNITY_EDITOR
		private static GameViewBoundsProvider CreateEditorScreenHelper()
		{
#if UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1
			return new LegacyEditorGameViewBoundsProvider();
#else
			return new EditorGameViewBoundsProvider();
#endif
		}
#endif
	}
}

#else
using Tobii.Gaming.Stubs;

namespace Tobii.Gaming.Internal
{
    internal partial class TobiiHost : TobiiHostStub
    {
        // all implementation in the stub
    }
}
#endif