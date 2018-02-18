using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEngine.UI;

namespace Wugner.Localize
{
	[CustomEditor(typeof(LocalizationImage))]
	public class LocalizationImageInspector : BaseLocalizationUIInspector
	{
		protected override void UpdateUI(EditorMultiLanguageEntry entryInMultiLanguage, string language)
		{
			try
			{
				var entry = entryInMultiLanguage.Get(language);
				var image = ((LocalizationText)target).GetComponent<Image>();
				image.sprite = Resources.Load<Sprite>(entry.Content);
			}
			catch (Exception e)
			{
				EditorGUILayout.HelpBox(e.Message, MessageType.Error);
			}
		}
	}
}
