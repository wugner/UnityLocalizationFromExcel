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
			IEditorVocabularyImporter vocabularyImpoter = null;

			var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Localization.ASSETPATH_CONFIG);
			var customEditorVocabularyImporterStr = config.CustomEditorVocabularyImpoter;
			if (string.IsNullOrEmpty(customEditorVocabularyImporterStr))
			{
				vocabularyImpoter = new DefaultEditorVocabularyImporter();
			}
			else
			{
				var type = Type.GetType(customEditorVocabularyImporterStr);
				if (type == null)
				{
					Debug.LogErrorFormat("Can not find custom editor vocabulary importer named [{0}]", customEditorVocabularyImporterStr);
					return;
				}
				vocabularyImpoter = Activator.CreateInstance(type) as IEditorVocabularyImporter;

				if (vocabularyImpoter == null)
				{
					Debug.LogErrorFormat("Custom editor vocabulary importer [{0}] does not implement interface IEditorVocabularyImporter", customEditorVocabularyImporterStr);
					return;
				}
			}

			vocabularyImpoter.ImportFiles();
		}
	}
}
