using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Wugner.Localize
{
    [InitializeOnLoad]
	public class EditorLocalizationInitializer
    {
        static EditorLocalizationInitializer()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
            EditorUtility.LoadOrCreateAsset<EditorVocabularyImportConfig>(Constant.ASSETPATH_IMPORTER_CONFIG);
            EditorMultiLanguageEntryCollection.Reload();
		}

		[MenuItem("Localization/Open Config")]
		static void OpenConfig()
		{
			var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
			Selection.activeObject = config;
        }

        [MenuItem("Localization/ReloadImportFiles")]
		static void ReimportVocabularyFiles()
		{
            var merger = new Importer.VocabularyMerger();

            var importerConfig = EditorUtility.LoadOrCreateAsset<EditorVocabularyImportConfig>(Constant.ASSETPATH_IMPORTER_CONFIG);
            foreach(var (type, filePaths) in importerConfig.GetConfigData())
            {
                var importer = Activator.CreateInstance(type) as ILocalizationVocabularyImporter;
                foreach(var p in filePaths)
                {
                    var text = File.ReadAllText(p);
                    var task = importer.Import(text);
                    task.Wait();

                    merger.Add(task.Result, Debug.LogWarning);
                }
            }

            var entriesSeperatedByLanguage = merger.MergedDataByLanguage;
            if (entriesSeperatedByLanguage.Count == 0)
            {
                Debug.LogWarningFormat("Can not read any vocabulary!");
                return;
            }

            GenerateVocabularyAssets(entriesSeperatedByLanguage.Values);

            //It is important to do this after reloading, so ui components can display updated info, such as id selection or a preview.
            EditorMultiLanguageEntryCollection.Reload();

            GenerateIdConstantSourceFile(entriesSeperatedByLanguage.First().Value);
        }

        static void GenerateVocabularyAssets(IEnumerable<VocabularyEntryCollection> vocabularyMapList)
        {
            //Generate assets that seperated by languages.
            //These assets are not only loaded at runtime to display text, but also used in editor mode to provide id selection and preview function.
            //By writing your own vocabulary manager, it is possible to load only one language vocabulary when game starting or switching to another language,
            //instead of loading all languages vocabulary.
            //Also you can output them to the streaming assets folder in json files and load them at runtime so you can add extra languages with out rebuilding the game.
            foreach (var vocabularyMap in vocabularyMapList)
            {
                var vocabularyAsset = EditorUtility.LoadOrCreateAsset<VocabulariesAsset>(
                    string.Format(Constant.ASSETPATH_VOCABULARY, vocabularyMap.Language));
                UnityEditor.EditorUtility.SetDirty(vocabularyAsset);

                vocabularyAsset.VocabularyEntries.Clear();
                vocabularyAsset.VocabularyEntries.AddRange(vocabularyMap);
            }
            //var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
            //foreach (var map in vocabularyMapList)
            //{
            //    var index = config.LanguageSettings.FindIndex(s => s.Language == map.Language);
            //    if (index < 0)
            //    {
            //        config.LanguageSettings.Add(new LocalizationConfig.LanguageInfo()
            //        {
            //            Language = map.Language,
            //            DefaultFont = Resources.Load("Library/unity default resources/Arial") as Font
            //        });
            //    }
            //}
            //config.LanguageSettings.RemoveAll(s => vocabularyMapList.All(m => m.Language != s.Language));

            //UnityEditor.EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        static void GenerateIdConstantSourceFile(VocabularyEntryCollection vocabularyCollection)
        {
            var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
            EditorConstantFileGenerater.CreateSourceFile(vocabularyCollection, config.IdConstantNameSpace, config.IdConstantClassName);
        }
    }
}
