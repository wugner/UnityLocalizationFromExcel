using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEngine.UI;

namespace Wugner.Localize
{
	[CustomEditor(typeof(LocalizationLabel))]
	public class LocalizationLabelInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var entires = EditorMultiLanguageEntryCollection.Instance.TextIDs;

			var setAtRuntimeProp = serializedObject.FindProperty("_setAtRuntime");
			EditorGUILayout.PropertyField(setAtRuntimeProp);
			if (setAtRuntimeProp.boolValue)
			{
				serializedObject.ApplyModifiedProperties();
				return;
			}

			var idProp = serializedObject.FindProperty("_id");
			{
				var rect = EditorGUILayout.GetControlRect(true);
				var leftRect = rect; leftRect.width = EditorGUIUtility.labelWidth;
				var rightRecct = rect; rightRecct.x += leftRect.width; rightRecct.width -= leftRect.width;
				EditorGUI.LabelField(leftRect, "ID");
				if (GUI.Button(rightRecct, idProp.stringValue, "ObjectField"))
				{
					LocalizationTextSelectorWindow.Show(idProp.stringValue, str =>
					{
						idProp.stringValue = str;
						serializedObject.ApplyModifiedProperties();
					});
				}
			}


			if (string.IsNullOrEmpty(idProp.stringValue))
			{
				EditorGUILayout.HelpBox("Click to select an id", MessageType.Warning);
				serializedObject.ApplyModifiedProperties();
				return;
			}
			var ids = EditorMultiLanguageEntryCollection.Instance.TextIDs;
			if (!ids.Contains(idProp.stringValue))
			{
				EditorGUILayout.HelpBox("Id is not exist! Please reselect one", MessageType.Error);
				serializedObject.ApplyModifiedProperties();
				return;
			}

			{
				var rect = EditorGUILayout.GetControlRect();
				rect.y += rect.height / 2 - 1;
				rect.height = 1;
				EditorGUI.DrawRect(rect, Color.grey);
			}

			if (((Behaviour)target).enabled)
			{
				var rect = EditorGUILayout.GetControlRect();
				var prefixRect = rect; prefixRect.width = EditorGUIUtility.labelWidth;
				var dropDownRect = prefixRect; dropDownRect.x += prefixRect.width; dropDownRect.width = (rect.width - prefixRect.width) / 2;
				var preBtnRect = dropDownRect; preBtnRect.x += dropDownRect.width; preBtnRect.width = dropDownRect.width / 2;
				var nextBtnRect = preBtnRect; nextBtnRect.x += preBtnRect.width;

				EditorGUI.LabelField(prefixRect, "Preview");

				var previewLanguageProp = serializedObject.FindProperty("_previewLanguage");
				var entryInMultiLanguage = EditorMultiLanguageEntryCollection.Instance.GetTextEntry(idProp.stringValue);

				var languages = entryInMultiLanguage.Languages.ToList();
				var preIndex = languages.IndexOf(previewLanguageProp.stringValue);
				if (preIndex < 0) preIndex = 0;

				var newIndex = EditorGUI.Popup(dropDownRect, preIndex, languages.ToArray());
				if (newIndex <= 0) newIndex = 0;
				
				EditorGUI.BeginDisabledGroup(newIndex == 0);
				if (GUI.Button(preBtnRect, "<<"))
					newIndex = newIndex <= 0 ? languages.Count - 1 : newIndex - 1;
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginDisabledGroup(newIndex == languages.Count - 1);
				if (GUI.Button(nextBtnRect, ">>"))
					newIndex = newIndex >= languages.Count - 1 ? 0 : newIndex + 1;
				EditorGUI.EndDisabledGroup();

				previewLanguageProp.stringValue = languages[newIndex];
				SetText(entryInMultiLanguage, languages[newIndex]);
			}

			serializedObject.ApplyModifiedProperties();
		}

		void SetText(EditorMultiLanguageEntry entryInMultiLanguage, string language)
		{
			try
			{
				var entry = entryInMultiLanguage.Get(language);
				var text = ((LocalizationLabel)target).GetComponent<Text>();
				text.text = entry.Content;

				var font = string.IsNullOrEmpty(entry.FontName) ? GetDefaultFont(language) : GetFont(entry.FontName);
				if (font != null)
					text.font = font;
			}
			catch (Exception e)
			{
				EditorGUILayout.HelpBox(e.Message, MessageType.Error);
			}
		}

		Font GetDefaultFont(string language)
		{
			var config = Resources.Load<LocalizationConfig>("LocalizationConfig");
			var lanIndex = config.LanguageSettings.FindIndex(l => l.Language == language);
			if (lanIndex < 0)
				throw new Exception(string.Format("Language {0} has not been set to config", language));

			var lan = config.LanguageSettings[lanIndex];
			
			if (lan.DefaultFont != null)
				return lan.DefaultFont;

			if (string.IsNullOrEmpty(lan.DefaultFontName))
				throw new Exception(string.Format("Font is not set for language {0}", language));

			return GetFont(lan.DefaultFontName);
		}

		Font GetFont(string fontName)
		{
			var config = Resources.Load<LocalizationConfig>("LocalizationConfig");
			
			var font = config.AllFonts.Find(f => f.name == fontName);
			if (font == null)
				throw new Exception(string.Format("Can not find font named {0} in font settings", fontName));

			return font;
		}
	}
}
