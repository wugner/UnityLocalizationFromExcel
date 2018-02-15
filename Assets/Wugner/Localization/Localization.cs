using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wugner.Localize
{
	public class Localization : MonoBehaviour
	{
		public const string RESOURCES_FOLDER = "Wugner/Localization/Generated/Resources";
		public const string ASSETPATH_CONFIG = "Assets/Wugner/Localization/Generated/Resources/LocalizationConfig.asset";
		public const string ASSETPATH_VOCABULARY = "Assets/Wugner/Localization/Generated/Resources/Vocabularies_{0}.asset";

		static Localization _staticInstance;
		public static Localization Instance
		{
			get
			{
				TryCreateInstance();
				return _staticInstance;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void TryCreateInstance()
		{
			if (_staticInstance == null)
			{
				_staticInstance = new GameObject().AddComponent<Localization>();
				_staticInstance.Init();
			}
		}

		public struct Entry
		{
			public string ID;
			public string Content;
			public string FontName;
		}

		ILocalizationSpritesManager _spritesManager;
		public ILocalizationSpritesManager SpritesManager { get { return _spritesManager; } }

		Dictionary<string, Dictionary<string, Entry>> _languageToEntryMap = new Dictionary<string, Dictionary<string, Entry>>();
		Dictionary<string, Font> _fontMap = new Dictionary<string, Font>();
		Dictionary<string, Font> _defaultFontMap = new Dictionary<string, Font>();

		Dictionary<string, Entry> _currentVacabularies;
		Font _currentDefaultFont;
		public static Font CurrentDefaultFont { get { return Instance._currentDefaultFont; } }

		public static Entry Get(string id)
		{
			Entry ret;
			if (Instance._currentVacabularies.TryGetValue(id, out ret))
			{
				return ret;
			}
			throw new Exception("Can not get localize data for id " + id);
		}

		void Init()
		{
			DontDestroyOnLoad(gameObject);

			InitSpritesManager();
			InitFont();
			InitVocabulary();
		}

		void InitSpritesManager()
		{
			var config = Resources.Load<LocalizationConfig>("LocalizationConfig");
			
			if (!string.IsNullOrEmpty(config.CustomSpritesManager))
			{
				object spm = null;
				var type = Type.GetType(config.CustomSpritesManager);
				if (type.IsSubclassOf(typeof(MonoBehaviour)))
					spm = gameObject.AddComponent(type);
				else
					spm = Activator.CreateInstance(type);

				if (spm is ILocalizationSpritesManager)
					_spritesManager = spm as ILocalizationSpritesManager;
				else
					Debug.LogErrorFormat("Custom sprites manager [{0}] does not implement ILocalizationSpritesManager!");
			}
			else
			{
				_spritesManager = gameObject.AddComponent<LocalizationSpritesManager>();
			}
		}

		void InitVocabulary()
		{
			var entries = Resources.Load<VocabulariesAsset>("LocalizationVocabularies");

			foreach (var entry in entries.VocabularyEntries)
			{
				Dictionary<string, Entry> temp;
				if (!_languageToEntryMap.TryGetValue(entry.Language, out temp))
				{
					temp = new Dictionary<string, Entry>();
					_languageToEntryMap.Add(entry.Language, temp);
				}

				temp.Add(entry.ID, new Entry()
				{
					ID = entry.ID,
					Content = entry.Content,
					FontName = entry.FontName,
				});
			}
		}
		

		void InitFont()
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
		
		public void SwitchLanguage(string language)
		{
			Dictionary<string, Entry> v;
			if (!_languageToEntryMap.TryGetValue(language, out v))
			{
				Debug.LogErrorFormat("Can not find language [{0}]", language);
				return;
			}
			_currentVacabularies = v;

			Font f;
			if (!_defaultFontMap.TryGetValue(language, out f))
			{
				Debug.LogWarningFormat("Can not find font for language [{0}], use arial", language);
				f = Font.CreateDynamicFontFromOSFont("arial", 12);
			}
			_currentDefaultFont = f;
		}

		public static Font GetFont(string fontName)
		{
			if (string.IsNullOrEmpty(fontName))
			{
				Debug.LogErrorFormat("font name is empty! use default font", Instance._currentDefaultFont.name);
				return Instance._currentDefaultFont;
			}

			Font f;
			if (!Instance._fontMap.TryGetValue(fontName, out f))
			{
				f = Font.CreateDynamicFontFromOSFont(fontName, 12);
				if (f == null)
				{
					Debug.LogErrorFormat("Can not find font [{0}], use default {1}", fontName, Instance._currentDefaultFont.name);
					f = Instance._currentDefaultFont;
				}
			}
			return f;
		}
	}
}
