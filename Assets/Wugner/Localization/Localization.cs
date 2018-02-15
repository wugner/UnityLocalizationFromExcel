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
		
		ILocalizationSpriteManager _spriteManager;
		public ILocalizationSpriteManager SpriteManager { get { return _spriteManager; } }
		ILocalizationFontManager _fontManager;
		public ILocalizationFontManager FontManager { get { return _fontManager; } }
		ILocalizationVocabularyManager _vocabularyManager;

		string _currentLanguage;
		Font _currentDefaultFont;
		Dictionary<string, RuntimeVocabularyEntry> _currentVacabularies;

		public static Font CurrentDefaultFont { get { return Instance._currentDefaultFont; } }

		public static RuntimeVocabularyEntry Get(string id)
		{
			RuntimeVocabularyEntry ret;
			if (Instance._currentVacabularies.TryGetValue(id, out ret))
			{
				return ret;
			}
			throw new Exception(string.Format("Can not get localize data for id {0}. Current language {1}", id, Instance._currentLanguage));
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
			
			if (!string.IsNullOrEmpty(config.CustomSpriteManager))
			{
				object spm = null;
				var type = Type.GetType(config.CustomSpriteManager);
				if (type.IsSubclassOf(typeof(MonoBehaviour)))
					spm = gameObject.AddComponent(type);
				else
					spm = Activator.CreateInstance(type);

				if (spm is ILocalizationSpriteManager)
					_spriteManager = spm as ILocalizationSpriteManager;
				else
					Debug.LogErrorFormat("Custom sprites manager [{0}] does not implement ILocalizationSpritesManager!", config.CustomSpriteManager);
			}
			else
			{
				_spriteManager = gameObject.AddComponent<DefaultSpriteManager>();
			}

			if (_spriteManager != null)
				_spriteManager.Init();
		}

		void InitFont()
		{
			var config = Resources.Load<LocalizationConfig>("LocalizationConfig");
			var customFontManager = config.CustomFontManager;
			if (!string.IsNullOrEmpty(customFontManager))
			{
				object spm = null;
				var type = Type.GetType(customFontManager);
				if (type.IsSubclassOf(typeof(MonoBehaviour)))
					spm = gameObject.AddComponent(type);
				else
					spm = Activator.CreateInstance(type);

				if (spm is ILocalizationFontManager)
					_fontManager = spm as ILocalizationFontManager;
				else
					Debug.LogErrorFormat("Custom font manager [{0}] does not implement ILocalizationSpritesManager!", customFontManager);
			}
			else
			{
				_fontManager = new DefaultFontManager();
			}

			if (_fontManager != null)
				_fontManager.Init();
		}

		void InitVocabulary()
		{
			_vocabularyManager = new DefaultVocabularyManager();


			if (_vocabularyManager != null)
				_vocabularyManager.Init();
		}

		public void SwitchLanguage(string language)
		{
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
	}
}
