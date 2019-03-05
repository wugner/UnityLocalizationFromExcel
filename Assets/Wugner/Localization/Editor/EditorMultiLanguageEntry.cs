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
					_instance.LoadEntries();
				}
				return _instance;
			}
		}

		public static void Reload()
		{
			if (_instance != null)
			{
				_instance._textEntries.Clear();
				_instance._imageEntries.Clear();
			}
			else
			{
				_instance = new EditorMultiLanguageEntryCollection();
			}
			_instance.LoadEntries();
		}

		void LoadEntries()
		{
            var importer = EditorLocalizationInitializer.CreateEditorVocabularyImporter();
		    _instance.AddRange(importer.GetImportedVocabularyEntries());
		}

		Dictionary<string, EditorMultiLanguageEntry> _textEntries = new Dictionary<string, EditorMultiLanguageEntry>();
		Dictionary<string, EditorMultiLanguageEntry> _imageEntries = new Dictionary<string, EditorMultiLanguageEntry>();

		public ICollection<EditorMultiLanguageEntry> TextEntries { get { return _textEntries.Values; } }
		public ICollection<EditorMultiLanguageEntry> ImageEntries { get { return _imageEntries.Values; } }
		public ICollection<string> TextIDs { get { return _textEntries.Keys; } }
		public ICollection<string> ImageIDs { get { return _imageEntries.Keys; } }

		public void Add(VocabularyEntry entry)
		{
			var entryMap = entry.Type == VocabularyEntryType.Text ? _textEntries : _imageEntries;

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

		public ICollection<EditorMultiLanguageEntry> GetEntries(VocabularyEntryType type)
		{
			return type == VocabularyEntryType.Text ? TextEntries : ImageEntries;
		}

		public EditorMultiLanguageEntry GetTextEntry(string id)
		{
			if (_textEntries.ContainsKey(id))
				return _textEntries[id];

			return null;
		}
		public EditorMultiLanguageEntry GetImageEntry(string id)
		{
			if (_imageEntries.ContainsKey(id))
				return _imageEntries[id];

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
            return _languageToEntry.Values.GetEnumerator();
        }
	}
}
