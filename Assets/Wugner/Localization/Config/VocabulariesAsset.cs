using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public class VocabulariesAsset : ScriptableObject
	{
		string _language;
		public string Language { get { return _language; } set { _language = value; } }

		[SerializeField]
		List<VocabularyEntry> _vocabularyEntries = new List<VocabularyEntry>();
		public List<VocabularyEntry> VocabularyEntries { get { return _vocabularyEntries; } }
	}
}
