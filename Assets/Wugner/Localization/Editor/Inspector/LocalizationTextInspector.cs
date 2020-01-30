using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEngine.UI;

namespace Wugner.Localize.Editor
{
	[CustomEditor(typeof(LocalizationText))]
	public class LocalizationTextInspector : BaseLocalizationUIInspector
	{
		protected override void UpdateUI(EditorMultiLanguageEntry entryInMultiLanguage, string language)
		{
			try
			{
				var entry = entryInMultiLanguage.Get(language);
				var text = ((LocalizationText)target).GetComponent<Text>();
				text.text = entry.Content;

				var font = string.IsNullOrEmpty(entry.Font) ? GetDefaultFont(language) : GetFont(entry.Font);
				if (font != null)
					text.font = font;
			}
			catch (Exception e)
			{
				EditorGUILayout.HelpBox(e.Message, MessageType.Error);
			}
		}

		Font GetDefaultFont(string language)
		{
			var config = Resources.Load<LocalizationConfig>("LocalizationConfig");
			var lanIndex = config.LanguageSettings.FindIndex(l => l.Language == language);
			if (lanIndex < 0)
				throw new Exception(string.Format("Language {0} has not been set to config", language));

			var lan = config.LanguageSettings[lanIndex];
			
			if (lan.DefaultFont != null)
				return lan.DefaultFont;

			if (string.IsNullOrEmpty(lan.DefaultFontName))
				throw new Exception(string.Format("Font is not set for language {0}", language));

			return GetFont(lan.DefaultFontName);
		}

		Font GetFont(string fontName)
		{
			var config = Resources.Load<LocalizationConfig>("LocalizationConfig");
			
			var font = config.AllFonts.Find(f => f.name == fontName);
			if (font == null)
				throw new Exception(string.Format("Can not find font named {0} in font settings", fontName));

			return font;
		}
	}
}
