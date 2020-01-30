using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public struct RuntimeVocabularyEntry
	{
		public string ID;
		public string Content;
		public string Font;
		public int? Size;
		public Dictionary<string, string> Extra;
	}
	public interface IRuntimeVocabularyManager
	{
		void Init();
		Dictionary<string, RuntimeVocabularyEntry> GetByLanguage(string language);
	}
}
