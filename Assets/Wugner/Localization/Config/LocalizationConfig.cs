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
		List<TextAsset> _localizeXmlFiles = new List<TextAsset>();
		public List<TextAsset> LocalizeXmlFiles { get { return _localizeXmlFiles; } }

		[SerializeField]
		List<string> _localizeXmlPaths = new List<string>();
		public List<string> LocalizeXmlPaths { get { return _localizeXmlPaths; } }

		[SerializeField]
		List<Font> _allFonts = new List<Font>();
		public List<Font> AllFonts { get { return _allFonts; } }

		[SerializeField]
		string _idConstantNameSpace;
		public string IdConstantNameSpace { get { return _idConstantNameSpace; } }

		[SerializeField]
		string _idConstantClassName;
		public string IdConstantClassName { get { return _idConstantClassName; } }

		[SerializeField]
		string _customSpritesManager;
		public string CustomSpritesManager { get { return _customSpritesManager; } }
	}
}
