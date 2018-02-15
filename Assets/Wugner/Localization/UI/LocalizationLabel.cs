using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wugner.Localize
{
	[RequireComponent(typeof(Text))]
	public class LocalizationLabel : MonoBehaviour
	{
		[SerializeField]
		bool _setAtRuntime;
		[SerializeField]
		string _id;

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

		bool _areadyHasValue;

		private void Start()
		{
			if (!_setAtRuntime && !_areadyHasValue)
				Set(_id);
		}

		public void Set(string id, params object[] objs)
		{
			_areadyHasValue = true;
			_id = id;

			var entry = Localization.Get(_id);

			if (string.IsNullOrEmpty(entry.FontName))
				TextComponent.font = Localization.CurrentDefaultFont;
			else
				TextComponent.font = Localization.GetFont(entry.FontName);

			var str = objs.Length == 0 ? entry.Content : string.Format(entry.Content, objs);
			TextComponent.text = str;
		}

#if UNITY_EDITOR
		[SerializeField]
		bool _showPreview;
		[SerializeField]
		string _previewLanguage;
#endif

	}
}
