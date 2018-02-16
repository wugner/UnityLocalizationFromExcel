using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wugner.Localize
{
	[CustomEditor(typeof(LocalizationConfig))]
	public class LocalizationConfigInspector : Editor
	{
		const string GENERATED_FOLDERPATH = "Wugner/Localization/Generated";
		const string CONFIG_ASSETPATH = "Assets/Wugner/Localization/Generated/LocalizationConfig.asset";

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}

		//[SerializeField]
		//string _editorLanguage;
		//public string EditorLanguage { get { return _editorLanguage; } }

		//[SerializeField]
		//List<TextAsset> _localizeXmlFiles;
		//public List<TextAsset> LocalizeXmlFiles { get { return _localizeXmlFiles; } }

		//[SerializeField]
		//List<VocabularyEntry> _vocabularyEntries;
		//public List<VocabularyEntry> VocabularyEntries { get { return _vocabularyEntries; } }

		//[SerializeField]
		//List<string> _iconPaths;
		//[SerializeField]
		//List<Sprite> _localizeIcons;
		//public List<Sprite> LocalizeIcons { get { return _localizeIcons; } }
	}
}
