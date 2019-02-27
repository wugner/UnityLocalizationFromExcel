using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
    public class VocabulariesAsset : ScriptableObject
    {
        string _language;
        public string Language { get { return _language; } set { _language = value; } }

        [SerializeField]
        List<VocabularyEntry> _vocabularyEntries;
        public List<VocabularyEntry> VocabularyEntries
        {
            get
            {
                if (_vocabularyEntries == null)
                    _vocabularyEntries = new List<VocabularyEntry>();
                return _vocabularyEntries;
            }
        }
    }
}
