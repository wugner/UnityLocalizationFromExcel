using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public class DefaultVocabularyManager : ILocalizationVocabularyManager
	{
		Dictionary<string, Dictionary<string, RuntimeVocabularyEntry>> _languageToEntryMap = new Dictionary<string, Dictionary<string, RuntimeVocabularyEntry>>();

		public virtual void Init()
		{
			var entries = Resources.Load<VocabulariesAsset>("LocalizationVocabularies");

			if (entries != null && entries.VocabularyEntries != null)
			{
				foreach (var entry in entries.VocabularyEntries)
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
						FontName = entry.FontName,
					});
				}

				Resources.UnloadAsset(entries);
			}
		}
		public virtual Dictionary<string, RuntimeVocabularyEntry> GetByLanguage(string language)
		{
			Dictionary<string, RuntimeVocabularyEntry> v;
			if (!_languageToEntryMap.TryGetValue(language, out v))
			{
				return null;
			}
			return v;
		}
	}
}
