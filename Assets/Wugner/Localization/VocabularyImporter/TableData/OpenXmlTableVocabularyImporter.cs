using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Wugner.OpenXml;

namespace Wugner.Localize.Importer
{
	public class OpenXmlTableVocabularyImporter : ITextVocabularyImporter
	{
		public List<RawVocabularyEntry> Import(string fileContent)
		{
			List<RawVocabularyEntry> ret = new List<RawVocabularyEntry>();

			var excel = new OpenXmlParser();
			excel.LoadXml(fileContent);
			
			foreach (var sheet in excel)
			{
				sheet.Value.SetHeaderAndSelectRow(1);
				var dataWithHeader = sheet.Value.Select(row => row.ValuesWithHeader.ToDictionary(kv => kv.Key, kv => kv.Value.StringValue));
				var processor = new TableDataProcessor();
				var entries = processor.ParseToRawEntries(dataWithHeader);

				ret.AddRange(entries);
			}

			return ret;
		}

	}
}
