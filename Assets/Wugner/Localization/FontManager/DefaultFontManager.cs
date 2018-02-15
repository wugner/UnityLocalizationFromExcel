using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public class DefaultFontManager : ILocalizationFontManager
	{
		Dictionary<string, Font> _fontMap = new Dictionary<string, Font>();
		Dictionary<string, Font> _defaultFontMap = new Dictionary<string, Font>();
		
		public void Init()
		{
			var settings = Resources.Load<LocalizationConfig>("LocalizationConfig");
			foreach (var fo in settings.AllFonts)
			{
				_fontMap.Add(fo.name, fo);
			}
			foreach (var languageSettings in settings.LanguageSettings)
			{
				var language = languageSettings.Language;
				var font = languageSettings.DefaultFont != null ? languageSettings.DefaultFont : GetFont(languageSettings.DefaultFontName);
				if (font != null)
				{
					_defaultFontMap.Add(language, font);
					if (!_fontMap.ContainsKey(font.name))
						_fontMap.Add(font.name, font);
				}
			}
		}
		public Font GetFont(string fontName)
		{
			if (string.IsNullOrEmpty(fontName))
			{
				return null;
			}

			Font f;
			if (!_fontMap.TryGetValue(fontName, out f))
			{
				f = Font.CreateDynamicFontFromOSFont(fontName, 12);
			}
			return f;
		}

		public Font GetLanguageDefaultFont(string language)
		{
			Font f;
			if (!_defaultFontMap.TryGetValue(language, out f))
			{
				return null;
			}
			return f;
		}
	}
}
