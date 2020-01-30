using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Wugner.Localize.Editor
{
    public abstract class EditorVocabularyImportSequencer : ScriptableObject
    {
        public abstract List<RawVocabularyEntry> Import();
    }
}
