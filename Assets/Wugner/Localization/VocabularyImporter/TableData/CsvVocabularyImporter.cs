using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Wugner.OpenXml;

namespace Wugner.Localize.Importer
{
	public class CsvVocabularyImporter : ITextVocabularyImporter
	{
		public List<RawVocabularyEntry> Import(string fileContent)
		{
			List<string> header = null;
			List<List<string>> body = new List<List<string>>();
			
			using (var reader = new System.IO.StringReader(fileContent))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (string.IsNullOrEmpty(line.Trim()))
					{
						continue;
					}

					var values = line.Split(',').Select(v => v.Trim()).ToList();
					if (header == null)
					{
						header = values;
					}
					else
					{
						body.Add(values);
					}
				}
			}

			if (header == null)
			{
				throw new Exception("File is empty!");
			}

			var processor = new TableDataProcessor();
			var entries = processor.Analyze(header, body);

			return entries;
		}
	}
}
