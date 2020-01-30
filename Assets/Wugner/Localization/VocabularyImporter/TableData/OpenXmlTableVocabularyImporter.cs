using System.Collections.Generic;
using System.Linq;
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
			
			foreach (var sheet in excel.Values)
			{
				sheet.SetHeaderAndSelectRow(1);

				List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
				while (sheet.MoveNext())
				{
					values.Add(sheet.CurrentRow.StringValuesWithHeader);
				}

				var processor = new TableDataProcessor();
				var entries = processor.ParseToRawEntries(values);

				ret.AddRange(entries);
			}

			return ret;
		}

	}
}
