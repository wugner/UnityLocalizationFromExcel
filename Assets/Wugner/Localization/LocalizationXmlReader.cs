using Wugner.OpenXml;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using System.Linq;

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

		public List<VocabularyEntryMap> LoadFiles(IEnumerable<string> filePaths)
		{
			foreach (var filePath in filePaths)
			{
				if (string.IsNullOrEmpty(filePath))
					continue;

				try
				{
					if (filePath.EndsWith("xml"))
					{
						TryLoadXml(filePath);
					}
					else
					{
						TryLoadExcelOrCsv(filePath);
					}
				}
				catch (IOException ioException)
				{
					Debug.LogException(ioException);
				}
				catch (System.Exception ex)
				{
					Debug.LogErrorFormat("File format is not valid! [{0}]", filePath);
					Debug.LogException(ex);
				}
			}

			CheckMissingLanguageVocabulary();
			return _multiLanguageData.Values.ToList();
		}

		public Dictionary<string, VocabularyEntryMap> LoadXmlFiles(IEnumerable<string> xmlTexts)
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

		void TryLoadXml(string filePath)
		{
			var excel = new OpenXmlParser();
			excel.LoadFromPath(filePath);

			foreach (var kv in excel)
			{
				AnalizeSheet(kv.Value);
			}
		}

		void TryLoadExcelOrCsv(string filePath)
		{
			using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
			{
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					var data = new List<List<string>>();
					do
					{
						if (reader.RowCount == 0)
							continue;

						while (reader.Read())
						{
							var count = reader.FieldCount;
							if (count == 0)
								continue;

							var rowData = new List<string>();
							for (var i = 0; i < count; i++)
							{
								rowData.Add(reader.GetString(i));
							}
							data.Add(rowData);
						}
					} while (reader.NextResult());

					var sheet = new ExcelSheet(data);
					AnalizeSheet(sheet);
				}
			}
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
