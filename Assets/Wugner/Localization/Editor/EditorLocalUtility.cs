using UnityEngine;
using UnityEditor;
using System;

namespace Wugner
{
	public class EditorLocalUtility
	{
		public static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject
		{
			var ret = AssetDatabase.LoadAssetAtPath<T>(path);
			if (ret == null)
			{
				var folderPath = path.Substring(0, path.LastIndexOf('/'));
				if (!AssetDatabase.IsValidFolder(folderPath))
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/" + folderPath);
					AssetDatabase.Refresh();
				}
				ret = ScriptableObject.CreateInstance<T>();
				AssetDatabase.CreateAsset(ret, path);
			}
			return ret;
		}

		public static ScriptableObject LoadOrCreateAsset(string path, Type type)
		{
			var ret = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
			if (ret == null)
			{
				var folderPath = path.Substring(0, path.LastIndexOf('/'));
				if (!AssetDatabase.IsValidFolder(folderPath))
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/" + folderPath);
					AssetDatabase.Refresh();
				}
				ret = ScriptableObject.CreateInstance(type);
				AssetDatabase.CreateAsset(ret, path);
			}
			return ret;
		}
	}
}