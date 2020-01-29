using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Wugner.Localize
{
	public interface ITextVocabularyImporter
	{
		List<RawVocabularyEntry> Import(string text);
	}
	public interface IBinaryVocabularyImporter
	{
		List<RawVocabularyEntry> Import(byte[] bytes);
	}
}
