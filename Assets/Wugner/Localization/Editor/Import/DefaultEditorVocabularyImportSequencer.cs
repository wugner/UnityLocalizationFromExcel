using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Wugner.Localize.Editor
{
	public class DefaultEditorVocabularyImportSequencer : EditorVocabularyImportSequencer
	{
		[SerializeField]
		List<string> _openXmlTableFileOrFolderPaths = null;

		[SerializeField]
		List<string> _csvTableFileOrFolderPaths = null;

		[SerializeField]
		List<string> _xmlArrayfileOrFolderPaths = null;

		public override List<RawVocabularyEntry> Import()
		{
			List<RawVocabularyEntry> ret = new List<RawVocabularyEntry>();
			ret.AddRange(Import(new Importer.OpenXmlTableVocabularyImporter(), _openXmlTableFileOrFolderPaths));
			ret.AddRange(Import(new Importer.CsvTableVocabularyImporter(), _csvTableFileOrFolderPaths));
			ret.AddRange(Import(new Importer.XmlArrayVocabularyImporter(), _xmlArrayfileOrFolderPaths));

			return ret;
		}

		protected virtual List<RawVocabularyEntry> Import(ITextVocabularyImporter importer, IEnumerable<string> fileOrFolderPaths)
		{
			List<RawVocabularyEntry> ret = new List<RawVocabularyEntry>();

			foreach (var p in GetFilePaths(fileOrFolderPaths))
			{
				try
				{
					var text = File.ReadAllText(p);
					var entries = importer.Import(text);
					entries.ForEach(e => e.SourceInfo = p);
					ret.AddRange(entries);
				}
				catch (Exception e)
				{
					Debug.LogWarning("Error in read file: " + p);
					Debug.LogWarning(e.ToString());
				}
			}
			return ret;
		}

		protected virtual IEnumerable<string> GetFilePaths(IEnumerable<string> fileOrFolderPaths)
		{
			List<string> filePaths = new List<string>();
			foreach (var p in fileOrFolderPaths)
			{
				var temp = p.Trim('/');
				if (temp.StartsWith("Assets"))
					temp = temp.Substring(6);

				var actualPath = Application.dataPath + "/" + temp;

				if (File.Exists(actualPath))
				{
					filePaths.Add(actualPath);
				}
				else if (Directory.Exists(actualPath))
				{
					var allFiles = Directory.GetFiles(actualPath, "", SearchOption.AllDirectories);
					filePaths.AddRange(allFiles);
				}
			}

			return filePaths;
		}
	}
}
