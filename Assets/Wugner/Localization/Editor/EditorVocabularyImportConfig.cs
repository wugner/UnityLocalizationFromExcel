using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Wugner.Localize
{
	public interface IEditorVocabularyImportConfig
	{
		List<(Type, IEnumerable<string>)> GetConfigData();
	}

	[CreateAssetMenu]
	public class EditorVocabularyImportConfig : ScriptableObject, IEditorVocabularyImportConfig
	{
		[SerializeField]
		List<string> _openXmlfilePaths;

		public List<(Type, IEnumerable<string>)> GetConfigData()
		{
			return new List<(Type, IEnumerable<string>)> { 
				(typeof(Importer.OpenXmlVocabularyImporter), GetFilePaths(_openXmlfilePaths)) 
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
