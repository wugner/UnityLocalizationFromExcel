using UnityEngine;
using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace Wugner.OpenXml
{
	public class ExcelSheet : List<ExcelRow>
	{
		public string Name{ private set; get; }
		public OpenXmlParser openXmlReader{ private set; get; }
		int _currentRowNum = -1;

		List<string> _headers = new List<string>();

		public ExcelSheet(XmlNode xmlNode, OpenXmlParser xmlReader)
		{
			openXmlReader = xmlReader;
			Name = xmlNode.Attributes["ss:Name"].Value;
			var list = xmlNode.SelectNodes("ns:Table/ns:Row", openXmlReader.nsMgr);
			for (int i = 0; i < list.Count; i++)
			{
				var node = list[i];
				Add(new ExcelRow(node, this, i));
			}

			RemoveAll(r => r.Count == 0);
		}

		public void SetHeaderAndSelectRow(int rowNo)
		{
			_headers.Clear();
			var row = SelectRow(rowNo);
			foreach (var s in row)
			{
				_headers.Add(s.StringValue);
			}
			while (MoveNext())
			{
				CurrentRow.SetHeader(_headers);
			}
            this.SelectRow(rowNo);
        }

		public void Reset()
		{
			_currentRowNum = -1;
		}

		public ExcelRow SelectRow(int rowNum)
		{
			rowNum--;
			if (rowNum < 0 || rowNum >= Count)
			{
				throw new Exception("the select row " + rowNum + " is more than sheet max row " + Count + " or below 0. " +
					"in sheet " + Name + " file " + openXmlReader.fileUrl);
			}
			_currentRowNum = rowNum;
			return this[_currentRowNum];
		}
		
		public bool MoveNext()
		{
			if (_currentRowNum < Count - 1)
			{
				_currentRowNum++;
				return true;
			}
			return false;
		}
		public ExcelRow CurrentRow
		{
			get
			{
				return this[_currentRowNum];
			}
		}
	}	
}