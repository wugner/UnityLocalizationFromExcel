using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Wugner.Localize
{
	public interface ITextVocabularyImporter
	{
		List<VocabularyEntry> Import(string text);
	}
	public interface IBinaryVocabularyImporter
	{
		List<VocabularyEntry> Import(byte[] bytes);
	}
}
