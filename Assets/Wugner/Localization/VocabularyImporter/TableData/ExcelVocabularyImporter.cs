using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Wugner.OpenXml;

namespace Wugner.Localize.Importer
{
	public class ExcelVocabularyImporter : IBinaryVocabularyImporter
	{
		public List<VocabularyEntry> Import(byte[] bytes)
		{
			var stream = new System.IO.MemoryStream(bytes);
			var reader = ExcelDataReader.ExcelReaderFactory.CreateBinaryReader(stream);
			
			//List<VocabularyEntry> ret = new List<VocabularyEntry>();

			//var excel = new OpenXmlParser();
			//excel.LoadXml(fileContent);
			
			//foreach (var sheet in excel)
			//{
			//	sheet.Value.SetHeaderAndSelectRow(1);
			//	var dataWithHeader = sheet.Value.Select(row => row.ValuesWithHeader.ToDictionary(kv => kv.Key, kv => kv.Value.StringValue));
			//	var processor = new TableDataProcessor();
			//	var entries = processor.Analyze(dataWithHeader);

			//	ret.AddRange(entries);
			//}

			return null;
		}

	}
}
