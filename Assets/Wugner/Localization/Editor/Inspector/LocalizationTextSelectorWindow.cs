using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Wugner.Localize.Editor
{
	public class LocalizationTextSelectorWindow : EditorWindow
	{
		static LocalizationTextSelectorWindow _window;
		public static void Show(EntryType entryType, string current, Action<string> onSelect)
		{
			if (_window == null)
				_window = CreateInstance<LocalizationTextSelectorWindow>();
			_window._entryType = entryType;
			_window._current = current;
			_window._onSelect = onSelect;
			_window.ShowAuxWindow();
		}

		EntryType _entryType;
		string _filter;
		string _current;
		Action<string> _onSelect;

		int _selected;
		private void OnGUI()
		{
			_filter = EditorGUILayout.TextField(_filter);

			using (new EditorGUILayout.ScrollViewScope(Vector2.zero, false, false))
			{
				var entries = EditorVocabularyEntriesManager.Instance.GetEntries(_entryType)
					.Where(e => string.IsNullOrEmpty(_filter) || e.ID.Contains(_filter) || e.Remark.Contains(_filter))
					.ToList();
				{
					var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * entries.Count);

					{
						var selected = entries.FindIndex(e => e.ID == _current);
						selected = GUI.SelectionGrid(rect, selected, entries.Select(e => e.ID).ToArray(), 1, "PreferencesKeysElement");

						_current = selected < 0 ? _current : entries[selected].ID;
						_onSelect(_current);
					}
					{
						var remarkRect = rect;
						remarkRect.x += rect.width / 2;
						remarkRect.height = EditorGUIUtility.singleLineHeight;
						foreach (var e in entries)
						{
							if (e.ID != _current)
								GUI.Label(remarkRect, e.Remark, "PreferencesKeysElement");
							else
								GUI.Label(remarkRect, e.Remark, new GUIStyle("PreferencesKeysElement") { normal = new GUIStyleState() { textColor = Color.white } });
							remarkRect.y += EditorGUIUtility.singleLineHeight;
						}
					}
				}
			}
		}
	}
}
