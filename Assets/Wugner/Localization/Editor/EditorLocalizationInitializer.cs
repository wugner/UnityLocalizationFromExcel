using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace Wugner.Localize
{
	[InitializeOnLoad]
	public class EditorLocalizationInitializer
	{
		static EditorLocalizationInitializer()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			EditorMultiLanguageEntryCollection.Reload();
		}

		[MenuItem("Localization/Open Config")]
		static void OpenConfig()
		{
			var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			Selection.activeObject = config;
		}
		
		[MenuItem("Localization/ReloadImportFiles")]
		static void ReimportVocabularyFiles()
		{
            IEditorVocabularyImporter vocabularyImpoter = CreateEditorVocabularyImporter();
            if (vocabularyImpoter != null)
                vocabularyImpoter.ImportFiles();
		}

        public static IEditorVocabularyImporter CreateEditorVocabularyImporter()
        {
            IEditorVocabularyImporter vocabularyImpoter = null;

            var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
            var customEditorVocabularyImporterStr = config.CustomEditorVocabularyImpoter;
            if (!string.IsNullOrEmpty(customEditorVocabularyImporterStr))
            {
                var type = Type.GetType(customEditorVocabularyImporterStr);
                if (type == null)
                {
                    Debug.LogErrorFormat("Can not find custom editor vocabulary importer named [{0}]", customEditorVocabularyImporterStr);
                    return null;
                }
                vocabularyImpoter = Activator.CreateInstance(type) as IEditorVocabularyImporter;

                if (vocabularyImpoter == null)
                {
                    Debug.LogErrorFormat("Custom editor vocabulary importer [{0}] does not implement interface IEditorVocabularyImporter", customEditorVocabularyImporterStr);
                    return null;
                }
            }
            if (vocabularyImpoter == null)
            {
                vocabularyImpoter = new DefaultEditorVocabularyImporter();
            }
            return vocabularyImpoter;
        }
    }
}
