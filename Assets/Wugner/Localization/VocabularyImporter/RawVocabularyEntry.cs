using System;
using System.Collections.Generic;

namespace Wugner.Localize
{
	[Serializable]
	public class RawVocabularyEntry
	{
		public string ID;
		/// <summary>
		/// Text or Image
		/// </summary>
		public string Type;
		public string Language;
		public string Remark;
		public string Content;

		public string FontName;
		public int? FontSize;

		public Dictionary<string, string> ExtraInfo;
		/// <summary>
		/// file name or more detail info for this entry from
		/// used for debugging output
		/// </summary>
		public string SourceInfo;
	}
}
