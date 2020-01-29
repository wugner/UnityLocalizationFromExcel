using System.Collections;
using System.Collections.Generic;

namespace Wugner.Localize
{
	[System.Serializable]
	public class RawVocabularyEntryCollection : IEnumerable<RawVocabularyEntry>
	{
		string _language;
		public string Language { get { return _language; } }

		Dictionary<string, RawVocabularyEntry> _dataCollection = new Dictionary<string, RawVocabularyEntry>();
		
		public ICollection<string> IDs { get { return _dataCollection.Keys; } }
		public int Count { get { return _dataCollection.Count; } }

		public RawVocabularyEntryCollection(string language)
		{
			_language = language;
		}

		public void Add(RawVocabularyEntry entry)
		{
			if (_dataCollection.ContainsKey(entry.ID))
			{
				throw new System.Exception(string.Format("Vocabulary id dupicate {0} in language {1}", entry.ID, Language));
			}

			_dataCollection.Add(entry.ID, entry);
		}

		public RawVocabularyEntry Get(string id)
		{
			RawVocabularyEntry entry;
			if (_dataCollection.TryGetValue(id, out entry))
			{
				return entry;
			}

			throw new System.Exception(string.Format("Can not get vocabulary {0} in language {1}", id, Language));
		}
		public bool TryGetValue(string id, out RawVocabularyEntry entry)
		{
			return _dataCollection.TryGetValue(id, out entry);
		}

		public IEnumerator<RawVocabularyEntry> GetEnumerator()
		{
			return _dataCollection.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dataCollection.Values.GetEnumerator();
		}
	}
}
