using System;
using System.Collections.Generic;
using System.Xml;

namespace Wugner.OpenXml
{
	public class OpenXmlParser : Dictionary<string, ExcelSheet>
	{
		public string fileUrl { private set; get; }
		public XmlNamespaceManager nsMgr { private set; get; }

		XmlDocument _xmldoc;
		public OpenXmlParser()
		{
		}

		public void LoadXml(string text)
		{
			_xmldoc = new XmlDocument();
			_xmldoc.LoadXml(text);
			AnalyzeFile();
		}

		public void LoadFromPath(string filePath)
		{
			_xmldoc = new XmlDocument();
			_xmldoc.Load(filePath);
			AnalyzeFile();
		}

		public void AnalyzeFile()
		{
			nsMgr = new XmlNamespaceManager(_xmldoc.NameTable);
			nsMgr.AddNamespace("ns", "urn:schemas-microsoft-com:office:spreadsheet");

			var sheetList = _xmldoc.DocumentElement.SelectNodes("/ns:Workbook/ns:Worksheet", nsMgr);
			foreach (XmlNode node in sheetList)
			{
				ExcelSheet s = new ExcelSheet(node, this);
				Add(s.Name, s);
			}
		}
		
		public ExcelSheet SelectSheet(string sheetName)
		{
			if (ContainsKey(sheetName))
			{
				return this[sheetName];
			}
			else
			{
				throw new Exception("there is no sheet named " + sheetName + " in the xml file " + fileUrl);
			}
		}
	}
}