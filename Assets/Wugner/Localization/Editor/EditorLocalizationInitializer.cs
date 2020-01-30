using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Wugner.Localize.Editor
{
    [InitializeOnLoad]
	public class EditorLocalizationInitializer
    {
        static EditorLocalizationInitializer()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			EditorLocalUtility.GetOrCreateConfig();
            EditorLocalUtility.GetOrCreateEditorLocalizeConfig();
            EditorVocabularyEntriesManager.Reload();
		}

		[MenuItem("Localization/Open Config")]
		static void OpenConfig()
		{
			var config = EditorLocalUtility.GetOrCreateConfig();
			Selection.activeObject = config;
        }

        [MenuItem("Localization/ReloadImportFiles")]
        private static void ReimportVocabularyFiles()
		{
            var merger = new Importer.VocabularyMerger();
            var config = EditorLocalUtility.GetOrCreateEditorLocalizeConfig();

            foreach(var seq in config.ImporterSequence)
            {
                var entries = seq.Import();
                merger.Add(entries);
            }

            foreach (var err in merger.Errors)
            {
                Debug.LogWarning(err.ToString());
            }

            var entriesSeperatedByLanguage = merger.MergedDataByLanguage;
            if (entriesSeperatedByLanguage.Count == 0)
            {
                Debug.LogWarningFormat("Can not read any vocabulary!");
                return;
            }

            GenerateVocabularyAssets(entriesSeperatedByLanguage.Values);

            GenerateIdConstantSourceFile(entriesSeperatedByLanguage.First().Value);

            //It is important to do this after reloading, so ui components can display updated info, such as id selection or a preview.
            EditorVocabularyEntriesManager.Reload();
        }

        static void GenerateVocabularyAssets(IEnumerable<RawVocabularyEntryCollection> languageSepratedvocabularyEntryList)
        {
            var assetGenerator = new EditorVocabularyAssetGenerator();
            assetGenerator.GenerateVocabularyAssets(languageSepratedvocabularyEntryList);
        }

        static void GenerateIdConstantSourceFile(RawVocabularyEntryCollection vocabularyCollection)
        {
            var importerConfig = EditorLocalUtility.GetOrCreateEditorLocalizeConfig();
            EditorConstantFileGenerator.CreateSourceFile(vocabularyCollection, importerConfig.IdConstantNameSpace, importerConfig.IdConstantClassName);
        }
    }
}
