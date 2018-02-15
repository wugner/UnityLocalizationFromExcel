using System;
using System.Collections.Generic;
using System.Xml;

namespace Wugner.OpenXml
{
	public class ExcelRow : List<ExcelCell>
	{
		public ExcelSheet Sheet { private set; get; }
		int _rowIndex;
		public int RowCount { get { return this._rowIndex + 1; } }

		Dictionary<string, ExcelCell> _dataMap = new Dictionary<string, ExcelCell>();
		public Dictionary<string, ExcelCell> ValuesWithHeader { get { return _dataMap; } }
		Dictionary<string, string> _stringValuesWithHeader;
		public Dictionary<string, string> StringValuesWithHeader
		{
			get
			{
				if (_stringValuesWithHeader == null)
				{
					_stringValuesWithHeader = new Dictionary<string, string>();
					foreach (var kv in _dataMap)
					{
						_stringValuesWithHeader.Add(kv.Key, kv.Value.StringValue);
					}
				}
				return _stringValuesWithHeader;
			}
		}

		public ExcelRow(XmlNode xmlNode, ExcelSheet sheet, int row)
		{
			this.Sheet = sheet;
			this._rowIndex = row;

			var list = xmlNode.SelectNodes("ns:Cell", this.Sheet.openXmlReader.nsMgr);

			int column = 0;
			foreach (XmlNode n in list)
			{
				var attrIndex = GetAttributeIndex(n, column);
				while (column < attrIndex - 1)
				{
					var cell = new ExcelCell("", row, column++);
					this.Add(cell);
				}

				var v = n.InnerText.Trim();
				if (!string.IsNullOrEmpty(v))
				{
					var cell = new ExcelCell(n.InnerText.Trim(), row, column++);
					this.Add(cell);
				}
			}
		}

		int GetAttributeIndex(XmlNode node, int column)
		{
			var itr = node.Attributes.GetEnumerator();
			while (itr.MoveNext())
			{
				var attr = itr.Current as XmlAttribute;
				if (attr.Name == "ss:Index")
				{
					int ret;
					if (int.TryParse(attr.Value, out ret))
						return ret;

					throw new Exception(string.Format("ss index value [{0}] is not number. at sheet {1} row {2} colum {3}", attr.Value, Sheet.Name, _rowIndex, column));
				}
			}
			return -1;
		}

		public void SetHeader(List<string> headers)
		{
			if (this.Count == 0)
				return;

			this._dataMap.Clear();
			this.ValuesWithHeader.Clear();

			for (int i = 0; i < headers.Count; i++)
			{
				var header = headers[i];
				if (string.IsNullOrEmpty(header))
					continue;

				if (this._dataMap.ContainsKey(header))
				{
					throw new Exception("Dupicate header " + header + " in sheet " + this.Sheet.Name);
				}

				ExcelCell cell;
				if (i < this.Count)
				{
					cell = this[i];
				}
				else
				{
					cell = new ExcelCell("", _rowIndex, i);
					Add(cell);
				}
				cell.Header = header;
				this._dataMap.Add(header, cell);
			}
		}

		public IEnumerable<ParseError> ParseToObject(Type objectType, ref object obj, ParseSettings settings = null)
		{
			if (this.Count == 0)
			{
				return new ParseError[0];
			}

			var parser = new ObjectParser(this, settings);
			parser.ParseToObject(objectType, ref obj);
			return parser.Errors;
		}

		public IEnumerable<ParseError> ParseToObject<T>(out T t, ParseSettings settings = null)
		{
			if (this.Count == 0)
			{
				t = default(T);
				return new ParseError[0];
			}

			object obj = Activator.CreateInstance<T>();
			var parser = new ObjectParser(this, settings);
			parser.ParseToObject(typeof(T), ref obj);
			t = (T)obj;
			return parser.Errors;
		}
	}
}