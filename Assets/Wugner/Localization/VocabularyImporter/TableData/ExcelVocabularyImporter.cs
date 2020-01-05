using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Wugner.OpenXml;
using ExcelDataReader;
using System.Data;

namespace Wugner.Localize.Importer
{
	public class ExcelVocabularyImporter : IBinaryVocabularyImporter
	{
		public List<VocabularyEntry> Import(byte[] bytes)
		{
			List<VocabularyEntry> ret = new List<VocabularyEntry>();

			var stream = new System.IO.MemoryStream(bytes);
			using (var reader = ExcelReaderFactory.CreateBinaryReader(stream))
			{
				var result = reader.AsDataSet();

                foreach (DataTable sheet in result.Tables)
                {
                    var headerRow = sheet.Rows[0];
					var headerRowStringList = headerRow.ItemArray.Select(o => o.ToString()).ToList();

					var valueList = new List<List<string>>();
					for (var i = 1; i < sheet.Rows.Count; i++)
                    {
						var row = sheet.Rows[i];
						var rowStringList = row.ItemArray.Select(o => o.ToString()).ToList();
						valueList.Add(rowStringList);
					}

					var processor = new TableDataProcessor();
					var entries = processor.Analyze(headerRowStringList, valueList);

					ret.AddRange(entries);
				}
			}

			return ret;
		}
	}
}
