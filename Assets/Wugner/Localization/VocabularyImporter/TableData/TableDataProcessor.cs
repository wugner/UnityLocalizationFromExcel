using Wugner.OpenXml;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using System.Linq;
using System;

namespace Wugner.Localize.Importer
{
	public class TableDataProcessor
	{
		const string HEADER_ID = "ID";

		private Dictionary<string, string> _headerToField = new Dictionary<string, string>()
		{
			{ "TYPE", "Type" },
			{ "REMARK", "Remark" },
			{ "CONTENT", "Content" },
			{ "FONT", "Font" },
		};

		public List<VocabularyEntry> Analyze(List<string> header, IEnumerable<List<string>> body)
		{
			var bodyDataWithHeader = new List<Dictionary<string, string>>();
			foreach (var bodyRow in body)
			{
				var dict = new Dictionary<string, string>();
				for (var column = 0; column < header.Count; column++)
				{
					var h = header[column];
					var b = bodyRow.Count <= column ? bodyRow[column] : null;

					if (dict.ContainsKey(h))
					{
						throw new System.Exception($"Duplicate header {h}");
					}
					dict.Add(h, b);
				}
				bodyDataWithHeader.Add(dict);
			}
			return Analyze(bodyDataWithHeader);
		}

		public List<VocabularyEntry> Analyze(IEnumerable<Dictionary<string, string>> bodyDataWithHeader)
		{
			var ret = new List<VocabularyEntry>();

			Dictionary<string, string> sharedData = new Dictionary<string, string>();
			Dictionary<string, VocabularyEntry> languageToEntry = new Dictionary<string, VocabularyEntry>();

			foreach (var row in bodyDataWithHeader)
			{
				sharedData.Clear();
				languageToEntry.Clear();

				if (!row.TryGetValue(HEADER_ID, out string id))
				{
					throw new Exception("Can not find id in header");
				}

				foreach (var kv in row)
				{					
					var (language, header) = ExtractLanuageNameFromHeader(kv.Key);
					if (language == null)
					{
						sharedData.Add(header, kv.Value);
					}
					else
					{
						if (!languageToEntry.TryGetValue(language, out var entry))
						{
							entry = new VocabularyEntry();
							entry.Language = language;
							languageToEntry.Add(language, entry);
						}
						SetValueToEntry(entry, header, kv.Value);
					}
				}
				foreach (var sharedDataKv in sharedData)
				{
					foreach (var entry in languageToEntry.Values)
					{
						SetValueToEntry(entry, sharedDataKv.Key, sharedDataKv.Value);
					}
				}
				ret.AddRange(languageToEntry.Values);
			}

			return ret;
		}

		void SetValueToEntry(VocabularyEntry entry, string header, string value)
		{
			if (!_headerToField.TryGetValue(header, out string fieldName))
				fieldName = header;

			var fieldInfo = entry.GetType().GetField(fieldName);
			if (fieldInfo != null)
			{
				if (fieldInfo.FieldType == typeof(string))
				{
					fieldInfo.SetValue(entry, value);
				}
				else if (fieldInfo.FieldType.IsEnum)
				{
					var enumValue = Enum.Parse(fieldInfo.FieldType, value, true);
					fieldInfo.SetValue(entry, enumValue);
				}
				else
				{
					throw new Exception("Not support type " + fieldInfo.FieldType.Name);
				}
			}
			else
			{
				if (entry.ExtraInfo == null)
					entry.ExtraInfo = new Dictionary<string, string>();
				entry.ExtraInfo.Add(header, value);
			}
		}

		(string lanuage, string header) ExtractLanuageNameFromHeader(string headerName)
		{
			var index = headerName.LastIndexOf("_");
			if (index <= 0 || index == headerName.Length - 1)
				return (null, headerName);

			return (headerName.Substring(index + 1), headerName.Substring(0, index));
		}
	}
}
