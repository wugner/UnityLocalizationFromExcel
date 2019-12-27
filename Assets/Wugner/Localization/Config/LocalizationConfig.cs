using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	//[CreateAssetMenu]
	public class LocalizationConfig : ScriptableObject
	{
		[System.Serializable]
		public struct LanguageInfo
		{
			public string Language;
			public string DisplayName;
			public Font DefaultFont;
			public string DefaultFontName;
		}

		[SerializeField]
		List<LanguageInfo> _languageSettings = new List<LanguageInfo>();
		public List<LanguageInfo> LanguageSettings { get { return _languageSettings; } }
		
		[SerializeField]
		List<Object> _localizeExcelFiles = new List<Object>();
		public List<Object> LocalizeExcelFiles { get { return _localizeExcelFiles; } }
		
		[SerializeField]
		List<string> _localizeExcelFilePaths = new List<string>();
		public List<string> LocalizeExcelFilePaths { get { return _localizeExcelFilePaths; } }

		[SerializeField]
		List<Font> _allFonts = new List<Font>();
		public List<Font> AllFonts { get { return _allFonts; } }

		[SerializeField]
		string _idConstantNameSpace = null;
		public string IdConstantNameSpace { get { return _idConstantNameSpace; } }

		[SerializeField]
		string _idConstantClassName = null;
		public string IdConstantClassName { get { return _idConstantClassName; } }

		[SerializeField]
		string _customSpriteManager = null;
		public string CustomSpriteManager { get { return _customSpriteManager; } }

		[SerializeField]
		string _customFontManager = null;
		public string CustomFontManager { get { return _customFontManager; } }

		[SerializeField]
		string _customVocabularyManager = null;
		public string CustomVocabularyManager { get { return _customVocabularyManager; } }
	}
}
