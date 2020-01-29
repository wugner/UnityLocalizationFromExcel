using System.Collections.Generic;
using UnityEngine;
using Wugner.Localize.Importer;

namespace Wugner.Localize
{
    public class VocabulariesAsset : ScriptableObject
    {
        [SerializeField]
        string _language = null;
        public string Language => _language;

        [SerializeField]
        List<VocabularyEntry> _vocabularyEntries = new List<VocabularyEntry>();
        public List<VocabularyEntry> VocabularyEntries => _vocabularyEntries;
    }
}
