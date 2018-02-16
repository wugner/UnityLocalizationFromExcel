using UnityEngine;
using UnityEditor;
using System;

namespace Wugner
{
	public class EditorUtility
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
	}
}