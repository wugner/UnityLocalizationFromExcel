using UnityEngine;
using UnityEditor;
using System;
using Wugner.Localize;

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

		public static LocalizationConfig GetOrCreateConfig()
		{
			return EditorLocalUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
		}
		public static IEditorVocabularyImportConfig GetEditorVocalbularyImportConfig()
		{
			if (!(EditorLocalUtility.LoadOrCreateAsset(Constant.ASSETPATH_IMPORTER_CONFIG, typeof(EditorVocabularyImportConfig))
				is IEditorVocabularyImportConfig importerConfig))
			{
				throw new Exception($"Asset {Constant.ASSETPATH_IMPORTER_CONFIG} does not implement IEditorVocabularyImportConfig!");
			}
			return importerConfig;
		}
	}
}