
namespace Wugner.Localize
{
	public enum VocabularyEntryType
	{
		Text,
		Sprite
	}

	[System.Serializable]
	public struct VocabularyEntry
	{
		public string ID;
		public VocabularyEntryType Type;
		public string Remark;
		public string Language;
		public string FontName;
		public string Content;
	}
}
