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

			EditorLocalUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
            EditorLocalUtility.LoadOrCreateAsset(Constant.ASSETPATH_IMPORTER_CONFIG, typeof(EditorVocabularyImportConfig));
            EditorVocabularyEntriesManager.Reload();
		}

		[MenuItem("Localization/Open Config")]
		static void OpenConfig()
		{
			var config = EditorLocalUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
			Selection.activeObject = config;
        }

        [MenuItem("Localization/ReloadImportFiles")]
        static void ReimportVocabularyFiles()
		{
            var merger = new Importer.VocabularyMerger();

            if (!(EditorLocalUtility.LoadOrCreateAsset(Constant.ASSETPATH_IMPORTER_CONFIG, typeof(EditorVocabularyImportConfig))
                is IEditorVocabularyImportConfig importerConfig))
            {
                throw new Exception($"Asset {Constant.ASSETPATH_IMPORTER_CONFIG} does not implement IEditorVocabularyImportConfig!");
            }

            foreach (var (type, filePaths) in importerConfig.GetImportersInfo())
            {
                var importer = Activator.CreateInstance(type);
                foreach(var p in filePaths)
                {
                    if (importer is ITextVocabularyImporter textImporter)
                    {
                        var text = File.ReadAllText(p);
                        var entries = textImporter.Import(text);
                        merger.Add(entries);
                    }
                    else if (importer is IBinaryVocabularyImporter binaryImporter)
                    {
                        var bytes = File.ReadAllBytes(p);
                        var entries = binaryImporter.Import(bytes);
                        merger.Add(entries);
                    }
                }
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

        static void GenerateVocabularyAssets(IEnumerable<VocabularyEntryCollection> languageSepratedvocabularyEntryList)
        {
            var assetGenerator = new EditorVocabularyAssetGenerator();
            assetGenerator.GenerateVocabularyAssets(languageSepratedvocabularyEntryList);
        }

        static void GenerateIdConstantSourceFile(VocabularyEntryCollection vocabularyCollection)
        {
            var config = EditorLocalUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
            EditorConstantFileGenerator.CreateSourceFile(vocabularyCollection, config.IdConstantNameSpace, config.IdConstantClassName);
        }
    }
}
