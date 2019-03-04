using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Wugner.Localize
{
	public interface IEditorVocabularyImporter
	{
		void ImportFiles();
	}

	public class DefaultEditorVocabularyImporter : IEditorVocabularyImporter
	{
		public virtual void ImportFiles()
		{
			var filePaths = GetFilePaths();

			if (filePaths.Count() == 0)
			{
				Debug.LogWarningFormat("No input xml files!");
				return;
			}
			
			var vocabularyMapList = AnalyzeFiles(filePaths);
			if (vocabularyMapList.Count == 0)
			{
				Debug.LogWarningFormat("Can not read any vocabulary!");
				return;
			}

			GenerateVocabularyAssets(vocabularyMapList);

			//It is important to do this after reloading, so the ui component can display updated info, such as id selection or a preview.
			EditorMultiLanguageEntryCollection.Reload();

			GenerateIdConstantSourceFile(vocabularyMapList);
		}

		protected virtual IEnumerable<string> GetFilePaths()
		{
			var filePaths = new List<string>();
			var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);

			if (config.LocalizeExcelFiles != null)
			{
				foreach (var f in config.LocalizeExcelFiles)
				{
					var assetPath = AssetDatabase.GetAssetPath(f);
					filePaths.Add(Application.dataPath + assetPath.Substring(6));
				}
			}

			if (config.LocalizeExcelFilePaths != null)
			{
				foreach (var p in config.LocalizeExcelFilePaths)
				{
					var temp = p.Trim('/');
					if (temp.StartsWith("Assets"))
						temp = temp.Substring(6);

					var actualPath = Application.dataPath + "/" + temp;

					if (File.Exists(actualPath))
					{
						filePaths.Add(actualPath);
					}
					else if (Directory.Exists(actualPath))
					{
						var allFiles = Directory.GetFiles(actualPath, "", SearchOption.AllDirectories);
						filePaths.AddRange(allFiles);
					}
				}
			}
			return filePaths;
		}

		protected virtual List<VocabularyEntryMap> AnalyzeFiles(IEnumerable<string> filePaths)
		{
			var reader = new LocalizationXmlReader();
			var vocabularyMapList = reader.LoadFiles(filePaths);

			return vocabularyMapList;
		}

		protected virtual void GenerateVocabularyAssets(List<VocabularyEntryMap> vocabularyMapList)
		{
			//Generate assets that seperated by languages.
			//These assets are not only loaded at runtime to display text, but also used in editor mode to provide id selection and preview function.
			//By writing your own vocabulary manager, it is possible to load only one language vocabulary when game starting or switching to another language,
			//instead of loading all languages vocabulary.
			//Also you can output them to the streaming assets folder in json files and load them at runtime so you can add extra languages with out rebuilding the game.
			foreach (var vocabularyMap in vocabularyMapList)
			{
				var vocabularyAsset = EditorUtility.LoadOrCreateAsset<VocabulariesAsset>(string.Format(Localization.ASSETPATH_VOCABULARY, vocabularyMap.Language));
                UnityEditor.EditorUtility.SetDirty(vocabularyAsset);

                vocabularyAsset.VocabularyEntries.Clear();
				vocabularyAsset.VocabularyEntries.AddRange(vocabularyMap);
            }
            var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
            foreach (var map in vocabularyMapList)
            {
                var index = config.LanguageSettings.FindIndex(s => s.Language == map.Language);
                if (index < 0)
                {
                    config.LanguageSettings.Add(new LocalizationConfig.LanguageInfo()
                    {
                        Language = map.Language,
                        DefaultFont = Resources.Load("Library/unity default resources/Arial") as Font
                    });
                }
            }
            config.LanguageSettings.RemoveAll(s => vocabularyMapList.All(m => m.Language != s.Language));

            UnityEditor.EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
		}

		protected virtual void GenerateIdConstantSourceFile(List<VocabularyEntryMap> vocabularyMapList)
		{
			var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			EditorConstantFileGenerater.CreateSourceFile(vocabularyMapList[0], config.IdConstantNameSpace, config.IdConstantClassName);
		}
	}
}
