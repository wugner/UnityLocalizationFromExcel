using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wugner.Localize
{
	public class Localization : MonoBehaviour
	{
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
        List<LocalizationConfig.LanguageInfo> _languageSettings;

        ILocalizationSpriteManager _spriteManager;
		public ILocalizationSpriteManager SpriteManager { get { return _spriteManager; } }

		ILocalizationFontManager _fontManager;
		public ILocalizationFontManager FontManager { get { return _fontManager; } }

		ILocalizationVocabularyManager _vocabularyManager;

		string _currentLanguage;
		public string CurrentLanguage { get { return _currentLanguage; } }

		Font _currentDefaultFont;
		Dictionary<string, RuntimeVocabularyEntry> _currentVacabularies;

		public static Font CurrentDefaultFont { get { return Instance._currentDefaultFont; } }

		void Init()
		{
			DontDestroyOnLoad(gameObject);

			var config = Resources.Load<LocalizationConfig>("LocalizationConfig");
            _languageSettings = config.LanguageSettings;

			if (!config.CustomInit)
			{
				InitSpritesManager();
				InitFontManager();
				InitVocabularyManager();
			}
		}

		public void CustomInit(
			(ILocalizationVocabularyManager customVocabularManager, 
			ILocalizationSpriteManager customSpriteManager, 
			ILocalizationFontManager customFontManager) customInitConfig)
		{
			InitSpritesManager(customInitConfig.customSpriteManager);
			InitFontManager(customInitConfig.customFontManager);
			InitVocabularyManager(customInitConfig.customVocabularManager);
		}

		void InitSpritesManager(ILocalizationSpriteManager customSpriteManager = null)
		{	
			_spriteManager = customSpriteManager ?? gameObject.AddComponent<DefaultSpriteManager>();
			_spriteManager.Init();
		}

		void InitFontManager(ILocalizationFontManager customFontManager = null)
		{
			_fontManager = customFontManager ?? new DefaultFontManager();
			_fontManager.Init();
		}

		void InitVocabularyManager(ILocalizationVocabularyManager customVocabularyManager = null)
		{
			_vocabularyManager = customVocabularyManager ?? new DefaultVocabularyManager();
			_vocabularyManager.Init();
		}

		public void SwitchLanguage(string language)
		{
			_currentLanguage = language;
			_currentVacabularies = _vocabularyManager.GetByLanguage(language);
			if (_currentVacabularies == null)
			{
				Debug.LogErrorFormat("Can not find language [{0}]", language);
			}
			_currentDefaultFont = _fontManager.GetLanguageDefaultFont(language);
			if (_currentDefaultFont == null)
			{
				Debug.LogWarningFormat("Can not find font for language [{0}], use arial", language);
				_currentDefaultFont = Font.CreateDynamicFontFromOSFont("arial", 12);
			}

			_onSwitchLanguage?.Invoke();
		}

        RuntimeVocabularyEntry GetEntryImp(string id)
        {
            if (_currentVacabularies == null)
            {
                if (_languageSettings == null || _languageSettings.Count == 0)
                    throw new Exception("Language settings are empty!");
                SwitchLanguage(_languageSettings[0].Language);
            }

			if (Instance._currentVacabularies.TryGetValue(id.Trim('/'), out RuntimeVocabularyEntry ret))
			{
				return ret;
			}
			throw new Exception(string.Format("Can not get localize data for id {0}. Current language {1}", id.Trim('/'), Instance._currentLanguage));
        }
        public static RuntimeVocabularyEntry GetEntry(string id)
		{
            return Instance.GetEntryImp(id);
		}
		
		public static Font GetFont(string fontName)
		{
			if (string.IsNullOrEmpty(fontName))
				return Instance._currentDefaultFont;

			var f = Instance._fontManager.GetFont(fontName);
			if (f == null)
				Debug.LogErrorFormat("Can not find font named [{0}], use default [{1}]", fontName, Instance._currentDefaultFont);

			return f;
		}

		event Action _onSwitchLanguage;

		public static void AddOnSwitchLanguageDelegate(Action onSwitchLanguage)
		{
			Instance._onSwitchLanguage += onSwitchLanguage;
		}
		public static void RemoveOnSwitchLanguageDelegate(Action onSwitchLanguage)
		{
			if (_staticInstance != null)
			{
				_staticInstance._onSwitchLanguage -= onSwitchLanguage;
			}
		}

	}
}
