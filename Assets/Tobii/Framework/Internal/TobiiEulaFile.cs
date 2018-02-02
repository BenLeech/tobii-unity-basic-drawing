namespace Tobii.Gaming.Internal
{
#if UNITY_EDITOR
    using System.IO;
#else
    using UnityEngine;
#endif

    public static class TobiiEulaFile
    {
	    private const string ResourcePath = "TobiiSDKEulaAccepted";
#if UNITY_EDITOR
		private static readonly string DirectoryPath = "Assets" + Path.DirectorySeparatorChar + "Tobii" + Path.DirectorySeparatorChar + "Resources";
        private static readonly string FilePath = DirectoryPath + Path.DirectorySeparatorChar + ResourcePath + ".json";
#endif

	    private static bool _eulaAccepted;

		public static bool IsEulaAccepted()
		{
#if UNITY_EDITOR
			Initialize();
#endif

			if (_eulaAccepted) return true;

#if UNITY_EDITOR

#if UNITY_STANDALONE
			if (File.Exists(FilePath))
			{
				var text = File.ReadAllText(FilePath);
				_eulaAccepted = text == "{\"EulaAccepted\": \"true\"}";
			}
#endif

			return _eulaAccepted;
#else
            TextAsset text = Resources.Load<TextAsset>(ResourcePath);
			_eulaAccepted = (null != text);

            return _eulaAccepted;
#endif
		}

#if UNITY_EDITOR
		public static void SetEulaAccepted()
        {
#if UNITY_STANDALONE			
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            File.WriteAllText(FilePath, "{\"EulaAccepted\": \"true\"}");
#endif
        }

	    public static void Initialize()
	    {
#if UNITY_STANDALONE			
			if (Directory.Exists(DirectoryPath) == false)
			{
				Directory.CreateDirectory(DirectoryPath);
			}

		    if (!File.Exists(FilePath))
		    {
				File.WriteAllText(FilePath, "{\"EulaAccepted\": \"false\"}");
			}
#endif			
		}
#endif
    }
}