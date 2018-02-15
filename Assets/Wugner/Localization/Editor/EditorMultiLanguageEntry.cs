using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;

namespace Wugner.Localize
{
	public class EditorMultiLanguageEntryCollection
	{
		static EditorMultiLanguageEntryCollection _instance;
		public static EditorMultiLanguageEntryCollection Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new EditorMultiLanguageEntryCollection();
					_instance.LoadEntriesFromAsset();
				}
				return _instance;
			}
		}

		public static void Reload()
		{
			if (_instance != null)
			{
				_instance._textEntries.Clear();
				_instance._spriteEntries.Clear();
			}
			else
			{
				_instance = new EditorMultiLanguageEntryCollection();
			}
			_instance.LoadEntriesFromAsset();
		}

		void LoadEntriesFromAsset()
		{
			var assets = Resources.LoadAll<VocabulariesAsset>("");
			foreach (var a in assets)
			{
				_instance.AddRange(a.VocabularyEntries);
			}
		}

		Dictionary<string, EditorMultiLanguageEntry> _textEntries = new Dictionary<string, EditorMultiLanguageEntry>();
		Dictionary<string, EditorMultiLanguageEntry> _spriteEntries = new Dictionary<string, EditorMultiLanguageEntry>();

		public ICollection<EditorMultiLanguageEntry> TextEntries { get { return _textEntries.Values; } }
		public ICollection<EditorMultiLanguageEntry> SpriteEntries { get { return _spriteEntries.Values; } }
		public ICollection<string> TextIDs { get { return _textEntries.Keys; } }
		public ICollection<string> SpriteIDs { get { return _spriteEntries.Keys; } }

		public void Add(VocabularyEntry entry)
		{
			var entryMap = entry.Type == VocabularyEntryType.Text ? _textEntries : _spriteEntries;

			EditorMultiLanguageEntry multiLanguageEntry;
			if (!entryMap.TryGetValue(entry.ID, out multiLanguageEntry))
			{
				multiLanguageEntry = new EditorMultiLanguageEntry()
				{
					ID = entry.ID,
					Remark = entry.Remark
				};
				entryMap.Add(entry.ID, multiLanguageEntry);
			}

			multiLanguageEntry.Add(entry);
		}

		public void AddRange(IEnumerable<VocabularyEntry> entries)
		{
			foreach (var e in entries)
			{
				Add(e);
			}
		}

		public EditorMultiLanguageEntry GetTextEntry(string id)
		{
			if (_textEntries.ContainsKey(id))
				return _textEntries[id];

			return null;
		}
		public EditorMultiLanguageEntry GetSpriteEntry(string id)
		{
			if (_spriteEntries.ContainsKey(id))
				return _spriteEntries[id];

			return null;
		}
	}

	public class EditorMultiLanguageEntry : IEnumerable<VocabularyEntry>
	{
		public string ID { set; get; }
		public string Remark { set; get; }

		Dictionary<string, VocabularyEntry> _languageToEntry = new Dictionary<string, VocabularyEntry>();
		public ICollection<string> Languages { get { return _languageToEntry.Keys; } }

		public void Add(VocabularyEntry entry)
		{
			if (_languageToEntry.ContainsKey(entry.Language))
			{
				Debug.LogErrorFormat("Language [{0}] dupicate in [{1}]", entry.Language, entry.ID);
				return;
			}

			_languageToEntry.Add(entry.Language, entry);
		}

		public VocabularyEntry Get(string language)
		{
			if (_languageToEntry.Count == 0)
				throw new System.Exception(string.Format("Vocabulary {0} has no entry for any language!", language));

			if (string.IsNullOrEmpty(language))
				return _languageToEntry.First().Value;

			if (_languageToEntry.ContainsKey(language))
				return _languageToEntry[language];

			throw new System.Exception(string.Format("Can not find language {0} for {1}", language, ID));
		}

		public IEnumerator<VocabularyEntry> GetEnumerator()
		{
			return _languageToEntry.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new System.NotImplementedException();
		}
	}
}
