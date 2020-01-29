using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	//[CreateAssetMenu]
	public class LocalizationConfig : ScriptableObject
	{
		[SerializeField]
		bool _customInit = false;
		public bool CustomInit => _customInit;

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
		List<Font> _allFonts = new List<Font>();
		public List<Font> AllFonts { get { return _allFonts; } }
	}
}
