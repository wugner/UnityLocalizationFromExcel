using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
    public class DefaultVocabularyManager : IRuntimeVocabularyManager
	{
		Dictionary<string, Dictionary<string, RuntimeVocabularyEntry>> _languageToEntryMap = new Dictionary<string, Dictionary<string, RuntimeVocabularyEntry>>();

		public virtual void Init()
		{
            var assetsList = Resources.LoadAll<VocabulariesAsset>("");
            foreach (var asset in assetsList)
            {
                LoadEntries(asset);
                Resources.UnloadAsset(asset);
            }
        }

        void LoadEntries(VocabulariesAsset asset)
        {
            if (asset != null && asset.VocabularyEntries != null)
            {
                foreach (var entry in asset.VocabularyEntries)
                {
                    Dictionary<string, RuntimeVocabularyEntry> temp;
                    if (!_languageToEntryMap.TryGetValue(entry.Language, out temp))
                    {
                        temp = new Dictionary<string, RuntimeVocabularyEntry>();
                        _languageToEntryMap.Add(entry.Language, temp);
                    }

                    temp.Add(entry.ID, new RuntimeVocabularyEntry()
                    {
                        ID = entry.ID,
                        Content = entry.Content,
                        Font = entry.Font,
                        Size = entry.Size,
                        Extra = entry.Extra
                    });
                }
            }
        }

		public virtual Dictionary<string, RuntimeVocabularyEntry> GetByLanguage(string language)
		{
			Dictionary<string, RuntimeVocabularyEntry> v;
			if (!_languageToEntryMap.TryGetValue(language, out v))
            {
                //LoadEntries(Resources.Load<VocabulariesAsset>("Vocabularies_" + language));
                //if (!_languageToEntryMap.TryGetValue(language, out v))
                return null;
            }
			return v;
		}

        public void AddExtraLanguage(string language, IEnumerable<RuntimeVocabularyEntry> entries)
        {
            Dictionary<string, RuntimeVocabularyEntry> entryMap;
            if (!_languageToEntryMap.TryGetValue(language, out entryMap))
            {
                entryMap = new Dictionary<string, RuntimeVocabularyEntry>();
                _languageToEntryMap.Add(language, entryMap);
            }
            foreach (var e in entries)
            {
                entryMap.Add(e.ID, e);
            }
        }
	}
}
