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

		/// <summary>
		/// font name
		/// </summary>
		public string Font;
		/// <summary>
		/// font size
		/// </summary>
		public int? Size;

		public Dictionary<string, string> Extra;
		/// <summary>
		/// file name or more detail info for this entry from
		/// used for debugging output
		/// </summary>
		public string SourceInfo;
	}
}
