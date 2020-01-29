using System;
using System.Collections.Generic;
using System.Linq;

namespace Wugner.Localize
{
	public enum EntryType
	{
		None = 0,
		Text,
		Image
	}

	[Serializable]
	public class VocabularyEntry : UnityEngine.ISerializationCallbackReceiver
	{
		public string ID;
		public EntryType Type;
		public string Language;
		public string Remark;
		public string Content;

		public string FontName;
		public int? FontSize;

		public Dictionary<string, string> ExtraInfo;
		
		public VocabularyEntry()
		{

		}

		public static implicit operator VocabularyEntry(RawVocabularyEntry entry)
		{
			return new VocabularyEntry()
			{
				ID = entry.ID,
				Type = (EntryType)Enum.Parse(typeof(EntryType), entry.Type, true),
				Language = entry.Language,
				Remark = entry.Remark,
				Content = entry.Content,
				FontName = entry.FontName,
				FontSize = entry.FontSize,
				ExtraInfo = entry.ExtraInfo,
			};
		}

		[Serializable]
		public struct VocabularyExtrInfo
		{
			public string Key;
			public string Value;
		}
		[UnityEngine.SerializeField]
		List<VocabularyExtrInfo> _extraInfoSerialize;

		public void OnAfterDeserialize()
		{
			ExtraInfo = _extraInfoSerialize?.ToDictionary(info => info.Key, info => info.Value);
		}

		public void OnBeforeSerialize()
		{
			_extraInfoSerialize = ExtraInfo?.Select(kv => new VocabularyExtrInfo() { Key = kv.Key, Value = kv.Value }).ToList();
		}
	}
}
