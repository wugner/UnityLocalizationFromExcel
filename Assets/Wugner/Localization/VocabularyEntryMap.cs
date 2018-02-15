using System.Collections;
using System.Collections.Generic;

namespace Wugner.Localize
{
	[System.Serializable]
	public class VocabularyEntryMap : IEnumerable<VocabularyEntry>
	{
		string _language;
		public string Language { get { return _language; } set { _language = value; } }

		Dictionary<string, VocabularyEntry> _dataCollection = new Dictionary<string, VocabularyEntry>();
		
		public ICollection<string> IDs { get { return _dataCollection.Keys; } }
		public int Count { get { return _dataCollection.Count; } }

		public void Add(VocabularyEntry entry)
		{
			if (_dataCollection.ContainsKey(entry.ID))
			{
				throw new System.Exception(string.Format("Vocabulary id dupicate {0} in language {1}", entry.ID, Language));
			}

			_dataCollection.Add(entry.ID, entry);
		}

		public VocabularyEntry Get(string id)
		{
			VocabularyEntry entry;
			if (_dataCollection.TryGetValue(id, out entry))
			{
				return entry;
			}

			throw new System.Exception(string.Format("Can not get vocabulary {0} in language {1}", id, Language));
		}

		public IEnumerator<VocabularyEntry> GetEnumerator()
		{
			return _dataCollection.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dataCollection.Values.GetEnumerator();
		}
	}
}
