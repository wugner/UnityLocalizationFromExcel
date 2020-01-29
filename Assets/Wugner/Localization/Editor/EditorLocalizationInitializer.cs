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

			EditorLocalUtility.GetOrCreateConfig();
            EditorLocalUtility.GetEditorVocalbularyImportConfig();
            EditorVocabularyEntriesManager.Reload();
		}

		[MenuItem("Localization/Open Config")]
		static void OpenConfig()
		{
			var config = EditorLocalUtility.GetOrCreateConfig();
			Selection.activeObject = config;
        }

        [MenuItem("Localization/ReloadImportFiles")]
        static void ReimportVocabularyFiles()
		{
            var merger = new Importer.VocabularyMerger();
            var importerConfig = EditorLocalUtility.GetEditorVocalbularyImportConfig();

            foreach (var (type, filePaths) in importerConfig.GetImportersInfo())
            {
                var importer = Activator.CreateInstance(type);
                foreach(var p in filePaths)
                {
                    if (importer is ITextVocabularyImporter textImporter)
                    {
                        try
                        {
                            var text = File.ReadAllText(p);
                            var entries = textImporter.Import(text);
                            entries.ForEach(e => e.SourceInfo = p);
                            merger.Add(entries);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Error in read file: " + p);
                            Debug.LogWarning(e.ToString());
                        }
                    }
                    else if (importer is IBinaryVocabularyImporter binaryImporter)
                    {
                        try
                        {
                            var bytes = File.ReadAllBytes(p);
                            var entries = binaryImporter.Import(bytes);
                            entries.ForEach(e => e.SourceInfo = p);
                            merger.Add(entries);
                        }
                        catch(Exception e)
                        {
                            Debug.LogWarning("Error in read file: " + p);
                            Debug.LogWarning(e.ToString());
                        }
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

        static void GenerateVocabularyAssets(IEnumerable<RawVocabularyEntryCollection> languageSepratedvocabularyEntryList)
        {
            var assetGenerator = new EditorVocabularyAssetGenerator();
            assetGenerator.GenerateVocabularyAssets(languageSepratedvocabularyEntryList);
        }

        static void GenerateIdConstantSourceFile(RawVocabularyEntryCollection vocabularyCollection)
        {
            var importerConfig = EditorLocalUtility.GetEditorVocalbularyImportConfig();
            EditorConstantFileGenerator.CreateSourceFile(vocabularyCollection, importerConfig.IdConstantNameSpace, importerConfig.IdConstantClassName);
        }
    }
}
