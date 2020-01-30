using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wugner.Localize.Editor
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
		
		public static LocalizationConfig GetOrCreateConfig()
		{
			return LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
		}
		public static EditorLocalizeConfig GetOrCreateEditorLocalizeConfig()
		{
			var config = AssetDatabase.LoadAssetAtPath<EditorLocalizeConfig>(Constant.ASSETPATH_EDITOR_CONFIG);
			if (config == null)
			{
				config = LoadOrCreateAsset<EditorLocalizeConfig>(Constant.ASSETPATH_EDITOR_CONFIG);
				var importSeq = LoadOrCreateAsset<DefaultEditorVocabularyImportSequencer>(Constant.ASSETPATH_EDITOR_IMPORTSEQ);
				config.ImporterSequence = new List<EditorVocabularyImportSequencer>() { importSeq };
				EditorUtility.SetDirty(config);
			}
			return config;
		}
	}
}