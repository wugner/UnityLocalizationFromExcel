using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wugner.Localize.Importer;

namespace Wugner.Localize
{
	[RequireComponent(typeof(Text))]
	public class LocalizationText : BaseLocalizationUI
	{
		public override EntryType RelatedEntryType { get { return EntryType.Text; } }

		object[] _params;

		Text _text;
		Text TextComponent
		{
			get
			{
				if (_text == null)
					_text = GetComponent<Text>();
				return _text;
			}
		}
		
		public void Set(string id, params object[] objs)
		{
			_alreadyHasValue = true;
			_id = id;
			_params = objs;

			var entry = Localization.GetEntry(_id);
			UpdateUIComponent(entry);
		}

		protected override void UpdateUIComponent(RuntimeVocabularyEntry entry)
		{
			if (string.IsNullOrEmpty(entry.FontName))
				TextComponent.font = Localization.CurrentDefaultFont;
			else
				TextComponent.font = Localization.GetFont(entry.FontName);

			var str = _params == null || _params.Length == 0 ? entry.Content : string.Format(entry.Content, _params);
			TextComponent.text = str;
		}
	}
}
