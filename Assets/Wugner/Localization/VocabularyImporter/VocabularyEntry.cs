using System.Collections.Generic;

namespace Wugner.Localize
{
	public enum VocabularyEntryType
	{
		None = 0,
		Text,
		Image
	}

	[System.Serializable]
	public class VocabularyEntry
	{
		public string ID;
		public VocabularyEntryType Type;
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
		public string srcInfo;
	}
}
