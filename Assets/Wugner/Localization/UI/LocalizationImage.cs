using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wugner.Localize.Importer;

namespace Wugner.Localize
{
	[RequireComponent(typeof(Image))]
	public class LocalizationImage : BaseLocalizationUI
	{
		public override EntryType RelatedEntryType { get { return EntryType.Image; } }

		Image _image;
		Image ImageComponent
		{
			get
			{
				if (_image == null)
					_image = GetComponent<Image>();
				return _image;
			}
		}

		protected override void UpdateUIComponent(RuntimeVocabularyEntry entry)
		{
			var sprite = Localization.Instance.SpriteManager.GetSprite(entry.Content);
			ImageComponent.sprite = sprite;
		}
	}
}
