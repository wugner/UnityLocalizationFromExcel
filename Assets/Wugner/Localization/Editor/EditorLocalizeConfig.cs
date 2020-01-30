using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Wugner.Localize.Editor
{
	public class EditorLocalizeConfig : ScriptableObject
	{
		[SerializeField]
		string _idConstantNameSpace = "Wugner.Localize";
		public string IdConstantNameSpace => _idConstantNameSpace;

		[SerializeField]
		string _idConstantClassName = "IDS";
		public string IdConstantClassName => _idConstantClassName;

		[SerializeField]
		List<EditorVocabularyImportSequencer> _importerSequence;
		public List<EditorVocabularyImportSequencer> ImporterSequence { get => _importerSequence; set => _importerSequence = value; }

	}
}
