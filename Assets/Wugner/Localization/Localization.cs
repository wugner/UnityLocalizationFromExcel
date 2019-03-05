using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wugner.Localize
{
	public class Localization : MonoBehaviour
	{
		public const string RESOURCES_FOLDER = "Wugner/Localization/Generated/Resources";
		public const string CONSTANT_ID_FILE = "Assets/Wugner/Localization/Generated/LocalizationID.cs";
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
			InitSpritesManager(config.CustomSpriteManager);
			InitFontManager(config.CustomFontManager);
			InitVocabularyManager(config.CustomVocabularyManager);
		}

		void InitSpritesManager(string customSpriteManagerName)
		{
			if (!string.IsNullOrEmpty(customSpriteManagerName))
			{
				var type = Type.GetType(customSpriteManagerName);
				if (type == null)
				{
					Debug.LogErrorFormat("Can not find custom sprite manager [{0}]", customSpriteManagerName);
				}
				else
				{
					if (type.IsSubclassOf(typeof(MonoBehaviour)))
						_spriteManager = gameObject.AddComponent(type) as ILocalizationSpriteManager;
					else
						_spriteManager = Activator.CreateInstance(type) as ILocalizationSpriteManager;

					if (_spriteManager == null)
					{
						Debug.LogErrorFormat("Custom sprites manager [{0}] does not or implement interface ILocalizationSpritesManager!", customSpriteManagerName);
					}
				}
			}
			
			if (_spriteManager == null)
				_spriteManager = gameObject.AddComponent<DefaultSpriteManager>();
			
			_spriteManager.Init();
		}

		void InitFontManager(string customFontManagerName)
		{
			if (!string.IsNullOrEmpty(customFontManagerName))
			{
				var type = Type.GetType(customFontManagerName);
				if (type == null)
				{
					Debug.LogErrorFormat("Can not find custom font manager [{0}]", customFontManagerName);
				}
				else
				{

					if (type.IsSubclassOf(typeof(MonoBehaviour)))
						_fontManager = gameObject.AddComponent(type) as ILocalizationFontManager;
					else
						_fontManager = Activator.CreateInstance(type) as ILocalizationFontManager;

					if (_fontManager == null)
					{
						Debug.LogErrorFormat("Custom font manager [{0}] does not exist or implement interface ILocalizationSpritesManager!", customFontManagerName);
					}
				}
			}

			if (_fontManager == null)
				_fontManager = new DefaultFontManager();
			
			_fontManager.Init();
		}

		void InitVocabularyManager(string customVocabularyManagerName)
		{
			if (!string.IsNullOrEmpty(customVocabularyManagerName))
			{
				var type = Type.GetType(customVocabularyManagerName);
				if (type == null)
				{
					Debug.LogErrorFormat("Can not find custom vocabulary manager [{0}]", customVocabularyManagerName);
				}
				else
				{
					if (type.IsSubclassOf(typeof(MonoBehaviour)))
						_vocabularyManager = gameObject.AddComponent(type) as ILocalizationVocabularyManager;
					else
						_vocabularyManager = Activator.CreateInstance(type) as ILocalizationVocabularyManager;

					if (_vocabularyManager == null)
					{
						Debug.LogErrorFormat("Custom vocabulary manager [{0}] does not exist or implement interface ILocalizationSpritesManager!", customVocabularyManagerName);
					}
				}
			}

			if (_vocabularyManager == null)
				_vocabularyManager = new DefaultVocabularyManager();
			
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

			if (_onSwitchLanguage != null)
				_onSwitchLanguage();
		}

        RuntimeVocabularyEntry GetEntryImp(string id)
        {
            if (_currentVacabularies == null)
            {
                if (_languageSettings == null || _languageSettings.Count == 0)
                    throw new Exception("Language settings are empty!");
                SwitchLanguage(_languageSettings[0].Language);
            }

            RuntimeVocabularyEntry ret;
            if (Instance._currentVacabularies.TryGetValue(id.Trim('/'), out ret))
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
