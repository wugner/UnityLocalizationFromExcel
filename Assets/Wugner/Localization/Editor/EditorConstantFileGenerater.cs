using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Wugner.Localize
{
	public class EditorConstantFileGenerater
	{
		public static void CreateSourceFile(VocabularyEntryMap data, string @namespace, string baseClassName)
		{
			var list = data.Select(t => new IDWithComment(t.ID, t.Remark)).ToList();
			var str = GenerateFileContent(list, @namespace, baseClassName);

			var fileFullPath = Application.dataPath + Localization.CONSTANT_ID_FILE.Substring(6);
			var directory = Directory.GetParent(fileFullPath);
			if (!Directory.Exists(directory.FullName))
				Directory.CreateDirectory(directory.FullName);

			File.WriteAllText(fileFullPath, str);

			Debug.Log("Generated id constant file. " + Localization.CONSTANT_ID_FILE);

			AssetDatabase.Refresh();
		}

		static string GenerateFileContent(List<IDWithComment> list, string @namespace, string baseClassName)
		{
			if (string.IsNullOrEmpty(@namespace))
				@namespace = "Wugner.Localize";
			if (string.IsNullOrEmpty(baseClassName))
				baseClassName = "IDS";

			list.Sort((s1, s2) => s1.ID.CompareTo(s2.ID));

			var classInfo = new ClassInfo(1, "");
			foreach (var entry in list)
			{
				classInfo.TryAdd(entry.ID, entry.Comment);
			}

			var str = new StringBuilder();
			AppendLineWithIndent(str, 0, "namespace ", @namespace);
			AppendLineWithIndent(str, 0, "{");
			AppendLineWithIndent(str, 1, "public static class ", baseClassName);
			AppendLineWithIndent(str, 1, "{");

			classInfo.AppendString(str);

			AppendLineWithIndent(str, 1, "}");
			AppendLineWithIndent(str, 0, "}");

			return str.ToString();
		}

		static void AppendLineWithIndent(StringBuilder buffer, int indentCount, params string[] objs)
		{
			buffer.Append("\r\n");
			for (var i = 0; i < indentCount; i++)
			{
				buffer.Append("\t");
			}
			foreach (var o in objs)
			{
				buffer.Append(o);
			}
		}

		struct IDWithComment
		{
			public string ID;
			public string Comment;
			public IDWithComment(string vname, string comment)
			{
				ID = vname;
				Comment = comment;
			}
		}

		class ClassInfo
		{
			int _indentLevel;
			string _classPath;
			string _className;
			string[] _splittedPath;

			List<IDWithComment> _constantVariables = new List<IDWithComment>();
			List<ClassInfo> _subClasses = new List<ClassInfo>();

			public ClassInfo(int indent, string path)
			{
				path = path.Trim('/');

				_indentLevel = indent;
				_classPath = path;

				if (!string.IsNullOrEmpty(path))
				{
					_splittedPath = path.Split('/');
					_className = _splittedPath[_splittedPath.Length - 1];
				}
				else
				{
					_splittedPath = new string[0];
					_className = "";
				}
			}

			public bool TryAdd(string fullPath, string comment)
			{
				fullPath = fullPath.Trim('/');

				if (_indentLevel > 10)
				{
					Debug.LogError("indent > 10");
					return true;
				}

				if (!string.IsNullOrEmpty(_classPath) && !fullPath.StartsWith(_classPath))
				{
					return false;
				}

				var temp = fullPath.Split('/');
				if (temp.Length == 1)
				{
					_constantVariables.Add(new IDWithComment(fullPath, comment));
					return true;
				}

				if (_splittedPath.Length == temp.Length - 1)
				{
					_constantVariables.Add(new IDWithComment(temp[temp.Length - 1], comment));
					return true;
				}

				foreach (var subClass in _subClasses)
				{
					if (subClass.TryAdd(fullPath, comment))
						return true;
				}

				var cla = new ClassInfo(_indentLevel + 1, _classPath + "/" + temp[_splittedPath.Length]);
				_subClasses.Add(cla);

				cla.TryAdd(fullPath, comment);

				return true;
			}

			public void AppendString(StringBuilder str)
			{
				if (!string.IsNullOrEmpty(_className))
				{
					AppendLineWithIndent(str, _indentLevel, "public static class ", _className);
					AppendLineWithIndent(str, _indentLevel, "{");
				}
				foreach (var v in _constantVariables)
				{
					AppendLineWithIndent(str, _indentLevel + 1, "/// <summary>");
					AppendLineWithIndent(str, _indentLevel + 1, "/// ", v.Comment);
					AppendLineWithIndent(str, _indentLevel + 1, "/// <summary>");
					AppendLineWithIndent(str, _indentLevel + 1, "public const string ", v.ID, " = \"/", _classPath, "/", v.ID, "\";");
				}
				foreach (var subClass in _subClasses)
				{
					subClass.AppendString(str);
				}

				if (!string.IsNullOrEmpty(_className))
				{
					AppendLineWithIndent(str, _indentLevel, "}");
				}
			}
		}
		
		static void Test()
		{
			var list = new List<IDWithComment>()
			{
				new IDWithComment("/View/TestView/Static/Name", "1234"),
				new IDWithComment("/View/TestView/Static/Right", "1234"),
				new IDWithComment("/View/TestView/Static/Creater/pppw", "1234"),
				new IDWithComment("/View/TestView/Static/Creater/xyz", "1234"),
				new IDWithComment("/View/WeaponView/Title", "rute"),
				new IDWithComment("/Take/ssb", "dsfajdfjsa"),
				new IDWithComment("/Take/BPS", "14444"),
			};
			var str = GenerateFileContent(list, "WugnerTest", "LocalizationID");

			var fileFullPath = Application.dataPath + "/Wugner/Localization/Generated/LocalizationID.cs";
			var directory = Directory.GetParent(fileFullPath);
			if (!Directory.Exists(directory.FullName))
				Directory.CreateDirectory(directory.FullName);

			File.WriteAllText(fileFullPath, str);
		}
	}
}