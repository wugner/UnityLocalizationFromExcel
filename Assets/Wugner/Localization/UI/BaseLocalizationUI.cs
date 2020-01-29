using UnityEngine;
using UnityEngine.UI;
using Wugner.Localize.Importer;

namespace Wugner.Localize
{
	public abstract class BaseLocalizationUI : MonoBehaviour
	{
		public abstract EntryType RelatedEntryType { get; }

		[SerializeField]
		protected bool _setAtRuntime;
		[SerializeField]
		protected string _id;
		
		protected bool _alreadyHasValue;

		protected virtual void Start()
		{
			if (!_setAtRuntime && !_alreadyHasValue)
				Set(_id);

			Localization.AddOnSwitchLanguageDelegate(OnSwitchLanguage);
		}

		protected virtual void OnDestroy()
		{
			Localization.RemoveOnSwitchLanguageDelegate(OnSwitchLanguage);
		}

		protected virtual void OnSwitchLanguage()
		{
			if (_alreadyHasValue)
				Set(_id);
		}

		public virtual void Set(string id)
		{
			_alreadyHasValue = true;
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
