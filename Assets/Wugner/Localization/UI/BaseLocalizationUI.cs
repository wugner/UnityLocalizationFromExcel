using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wugner.Localize
{
	public abstract class BaseLocalizationUI : MonoBehaviour
	{
		public abstract VocabularyEntryType RelatedEntryType { get; }

		[SerializeField]
		protected bool _setAtRuntime;
		[SerializeField]
		protected string _id;
		
		protected bool _areadyHasValue;

		protected virtual void Start()
		{
			if (!_setAtRuntime && !_areadyHasValue)
				Set(_id);

			Localization.AddOnSwitchLanguageDelegate(OnSwitchLanguage);
		}

		protected virtual void OnDestroy()
		{
			Localization.RemoveOnSwitchLanguageDelegate(OnSwitchLanguage);
		}

		protected virtual void OnSwitchLanguage()
		{
			if (_areadyHasValue)
				Set(_id);
		}

		public virtual void Set(string id)
		{
			_areadyHasValue = true;
			_id = id;

			var entry = Localization.GetEntry(_id);
			UpdateUIComponent(entry);
		}

		protected abstract void UpdateUIComponent(RuntimeVocabularyEntry entry);

#if UNITY_EDITOR
		[SerializeField]
		protected bool _showPreview;
		[SerializeField]
		protected string _previewLanguage;
#endif

	}
}
