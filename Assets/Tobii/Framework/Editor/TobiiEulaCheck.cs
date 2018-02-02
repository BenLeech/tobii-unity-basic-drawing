using Tobii.Gaming.Internal;

namespace Tobii.Gaming
{
	using UnityEngine;
	using UnityEditor;

	[InitializeOnLoad]
	public class TobiiEulaCheck : EditorWindow
	{
		private static readonly string TexturePath = @"Assets\Tobii\Framework\Textures\";
		private const string EulaUrl = "https://developer.tobii.com/license-agreement/";
		private static TobiiEulaCheck _window;

		static TobiiEulaCheck()
		{
			if (TobiiEulaFile.IsEulaAccepted() == false)
			{
				EditorApplication.update += Update;
				EditorApplication.playmodeStateChanged += HandleOnPlayModeChanged;
			}
		}

		private static void HandleOnPlayModeChanged()
		{
			if (EditorApplication.isPlaying && TobiiEulaFile.IsEulaAccepted() == false)
			{
				ShowWindow();
			}
		}

		private static void Update()
		{
			ShowWindow();
			EditorApplication.update -= Update;
		}

		private static void ShowWindow()
		{
#if UNITY_STANDALONE		
			_window = GetWindow<TobiiEulaCheck>(true);
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
            _window.titleContent = new GUIContent("Tobii EULA");
#else
			_window.title = "Tobii EULA";
#endif
			_window.minSize = new Vector2(400, 290);
			_window.position = new Rect(100, 75, 400, 290);
#endif			
		}

		public void OnGUI()
		{
			EditorGUILayout.BeginVertical(EditorStyles.label);
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath + "TobiiLogo.png");
#else
            var logo = (Texture2D)AssetDatabase.LoadAssetAtPath(TexturePath + "TobiiLogo.png", typeof(Texture2D));
#endif
			var rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);
			if (logo != null)
			{
				GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);
			}

			EditorGUILayout.HelpBox("To use this package please read and accept the EULA.", MessageType.Info);

			if (GUILayout.Button("Read the EULA"))
			{
				Application.OpenURL(EulaUrl);
			}

			EditorGUILayout.LabelField("");
			EditorGUILayout.LabelField("");

			EditorGUILayout.BeginHorizontal(EditorStyles.label);

			if (GUILayout.Button("Accept", EditorStyles.miniButtonRight))
			{
				EditorApplication.playmodeStateChanged -= HandleOnPlayModeChanged;
				TobiiEulaFile.SetEulaAccepted();
				_window.Close();
			}

			GUILayout.Button("", EditorStyles.miniBoldLabel);

			if (GUILayout.Button("Decline", EditorStyles.miniButtonLeft))
			{
				_window.Close();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
	}
}