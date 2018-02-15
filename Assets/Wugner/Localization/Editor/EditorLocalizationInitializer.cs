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

		[MenuItem("Localization/ReloadXML")]
		static void ReloadXmlFiles()
		{
			var config = LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			var xmlfiles = config.LocalizeXmlFiles.Select(x => x.text);
			if (xmlfiles.Count() == 0)
			{
				Debug.LogWarningFormat("No input xml files!");
				return;
			}

			var reader = new LocalizationXmlReader();
			var languageToVocabularyMap = reader.ReadText(xmlfiles);

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

			EditorConstantFileGenerater.CreateSourceFile(languageToVocabularyMap.First().Value, "Wuger.Localize", "LocalizationID");
		}
	}
}
