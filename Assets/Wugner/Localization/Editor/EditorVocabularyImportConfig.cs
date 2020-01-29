using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Wugner.Localize
{
	public interface IEditorVocabularyImportConfig
	{
		string IdConstantNameSpace { get; }
		string IdConstantClassName { get; }
		List<(Type type, IEnumerable<string> filePaths)> GetImportersInfo();
	}

	public class EditorVocabularyImportConfig : ScriptableObject, IEditorVocabularyImportConfig
	{
		[SerializeField]
		string _idConstantNameSpace = "Wugner.Localize";
		public string IdConstantNameSpace { get { return _idConstantNameSpace; } }

		[SerializeField]
		string _idConstantClassName = "IDS";
		public string IdConstantClassName { get { return _idConstantClassName; } }

		[SerializeField]
		List<string> _openXmlfileOrFolderPaths = null;

		[SerializeField]
		List<string> _csvfileOrFolderPaths = null;

		public List<(Type type, IEnumerable<string> filePaths)> GetImportersInfo()
		{
			return new List<(Type, IEnumerable<string>)> { 
				(typeof(Importer.OpenXmlVocabularyImporter), GetFilePaths(_openXmlfileOrFolderPaths)),
				(typeof(Importer.CsvVocabularyImporter), GetFilePaths(_csvfileOrFolderPaths))
			};
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

			return fileOrFolderPaths;
		}
	}
}
