using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Wugner.Localize.Editor
{
	public class EditorLocalizeConfig : ScriptableObject
	{
		[SerializeField]
		string _idConstantClassFullName = "Wugner.Localize.IDS";
		public string IdConstantClassNameWithNameSpace => _idConstantClassFullName;

		[SerializeField]
		List<EditorVocabularyImportSequencer> _importerSequence;
		public List<EditorVocabularyImportSequencer> ImporterSequence { get => _importerSequence; set => _importerSequence = value; }

	}
}
