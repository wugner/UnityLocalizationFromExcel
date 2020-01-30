using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;

namespace Wugner.Localize.Importer
{
	/// <summary>
	/// xml format sample. An xml file can contains multiple entries, any field in VocabularyEntry can be a menber of an entry except ExtraInfo.
	/// Member that is not a field of VocabularyEntry will be treated as an extra info and will be added to field ExtraInfo
	/// <?xml version="1.0"?>
	///	<Entry>
	///		<ID>id01</ID>
	///		<Type></Type>
	///		<Language>En</Language>
	///		<Remark>ttt</Remark>
	///		<Content>content</Content>
	///		<FontName>arial</FontName>
	///		<FontSize>14</FontSize>
	///		
	///		<Color>a</Color>
	///	</Entry>
	/// 
	/// </summary>
	public class XmlArrayVocabularyImporter : ITextVocabularyImporter
	{
		public List<RawVocabularyEntry> Import(string fileContent)
		{
			var ret = new List<RawVocabularyEntry>();
			var xdoc = XDocument.Parse(fileContent);
			foreach (var entryElement in xdoc.Elements("Entry"))
			{
				var entry = new RawVocabularyEntry();
				foreach (var entryData in entryElement.Elements())
				{
					SetData(entry, entryData.Name.LocalName, entryData.Value);
				}
				ret.Add(entry);
			}
			
			return ret;
		}

		void SetData(RawVocabularyEntry entry, string name, string value)
		{
			var field = typeof(RawVocabularyEntry).GetField(name);
			if (field != null)
			{
				field.SetValue(name, value);
			}
			else
			{
				if (entry.Extra == null)
					entry.Extra = new Dictionary<string, string>();
				entry.Extra.Add(name, value);
			}
		}
	}
}
