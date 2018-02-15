using Wugner.OpenXml;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public class LocalizationXmlReader
	{
		const string HEADER_ID = "ID";
		const string HEADER_TYPE = "TYPE";
		const string HEADER_REMARK = "REMARK";
		const string HEADER_CONTENT = "CONTENT_";
		const string HEADER_FONT = "FONT_";

		Dictionary<string, VocabularyEntryMap> _multiLanguageData = new Dictionary<string, VocabularyEntryMap>();

		public Dictionary<string, VocabularyEntryMap> ReadText(IEnumerable<string> xmlTexts)
		{
			foreach (var xml in xmlTexts)
			{
				var excel = new OpenXmlParser();
				excel.LoadXml(xml);

				foreach (var kv in excel)
				{
					AnalizeSheet(kv.Value);
				}
			}

			CheckMissingLanguageVocabulary();

			return _multiLanguageData;
		}

		void AnalizeSheet(ExcelSheet sheet)
		{
			sheet.SetHeaderAndSelectRow(1);
			var headers = sheet.CurrentRow;

			while (sheet.MoveNext())
			{
				var row = sheet.CurrentRow;
				var mapping = row.ValuesWithHeader;
				if (!mapping.ContainsKey(HEADER_ID))
				{
					Debug.LogErrorFormat("Sheet {0} doesn't have header {1}", sheet.Name, HEADER_ID);
					return;
				}
				if (!mapping.ContainsKey(HEADER_REMARK))
				{
					Debug.LogErrorFormat("Sheet {0} doesn't have header {1}", sheet.Name, HEADER_ID);
					return;
				}
				var id = mapping[HEADER_ID];
				var remark = mapping[HEADER_REMARK];
				VocabularyEntryType type = (VocabularyEntryType)System.Enum.Parse(typeof(VocabularyEntryType), mapping[HEADER_TYPE]);

				foreach (var kv in mapping)
				{
					if (!string.IsNullOrEmpty(kv.Value) && kv.Key.StartsWith(HEADER_CONTENT))
					{
						var language = kv.Key.Substring(HEADER_CONTENT.Length);
						var content = kv.Value;
						var font = mapping[HEADER_FONT + language];

						VocabularyEntryMap entryMap;
						if (!_multiLanguageData.TryGetValue(language, out entryMap))
						{
							entryMap = new VocabularyEntryMap() { Language = language };
							_multiLanguageData.Add(language, entryMap);
						}

						entryMap.Add(new VocabularyEntry()
						{
							ID = id,
							Type = type,
							Remark = remark,
							Language = language,
							Content = content,
							FontName = font,
						});

						if (string.IsNullOrEmpty(content))
							Debug.LogWarningFormat("Vocabulary [{0}] content in language {1} is null!", id, language);
					}
				}
			}
		}

		void CheckMissingLanguageVocabulary()
		{
			var languages = _multiLanguageData.Keys;
			HashSet<string> allIDs = new HashSet<string>();
			foreach (var vocabulryMap in _multiLanguageData)
			{
				allIDs.UnionWith(vocabulryMap.Value.IDs);
			}
			foreach (var kv in _multiLanguageData)
			{
				var language = kv.Key;
				var map = kv.Value;
				if (allIDs.Count != map.Count)
				{
					var tempHash = new HashSet<string>();
					tempHash.UnionWith(allIDs);
					tempHash.ExceptWith(kv.Value.IDs);

					foreach (var s in tempHash)
					{
						Debug.LogWarningFormat("Vocabulary [{0}] is missing in language {1}", s, language);
					}
				}
			}
		}
	}
}
