using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Wugner.Localize
{
	[InitializeOnLoad]
	public class EditorLocalizationInitializer
	{
		static EditorLocalizationInitializer()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;
						
			LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			EditorMultiLanguageEntryCollection.Reload();
		}

		[MenuItem("Localization/Config")]
		static void OpenConfig()
		{
			var config = LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			Selection.activeObject = config;
		}

		static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject
		{
			var ret = AssetDatabase.LoadAssetAtPath<T>(path);
			if (ret == null)
			{
				if (!AssetDatabase.IsValidFolder(Localization.RESOURCES_FOLDER))
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/" + Localization.RESOURCES_FOLDER);
					AssetDatabase.Refresh();
				}
				ret = ScriptableObject.CreateInstance<T>();
				AssetDatabase.CreateAsset(ret, path);
			}
			return ret;
		}

		//[MenuItem("Localization/ttttttt")]
		//static void GGGGG()
		//{
		//	var config = LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
		//	var t = AssetDatabase.GetAssetPath(config.LocalizeExcelFiles[0]);
		//	Debug.Log(t);

		//	Debug.Log(Application.dataPath);
		//}

		[MenuItem("Localization/ReloadXML")]
		static void ReloadXmlFiles()
		{
			var filePaths = new List<string>();

			var config = LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			filePaths.AddRange(config.LocalizeExcelFiles.Select(f => Application.dataPath + AssetDatabase.GetAssetPath(f).Substring(6)));
			filePaths.AddRange(config.LocalizeExcelFilePaths.Select(p => Application.dataPath + "/" + p));

			if (filePaths.Count == 0)
			{
				Debug.LogWarningFormat("No input xml files!");
				return;
			}

			var reader = new LocalizationXmlReader();
			var languageToVocabularyMap = reader.LoadFiles(filePaths);

			if (languageToVocabularyMap.Count == 0)
			{
				Debug.LogWarningFormat("Can not read any vocabulary!");
				return;
			}

			foreach (var kv in languageToVocabularyMap)
			{
				var vocabularyMap = kv.Value;
				var vocabularyAsset = LoadOrCreateAsset<VocabulariesAsset>(string.Format(Localization.ASSETPATH_VOCABULARY, vocabularyMap.Language));
				vocabularyAsset.VocabularyEntries.Clear();
				vocabularyAsset.VocabularyEntries.AddRange(vocabularyMap);
			}

			AssetDatabase.SaveAssets();
			EditorMultiLanguageEntryCollection.Reload();

			EditorConstantFileGenerater.CreateSourceFile(languageToVocabularyMap.First().Value, config.IdConstantNameSpace, config.IdConstantClassName);
		}
	}
}
