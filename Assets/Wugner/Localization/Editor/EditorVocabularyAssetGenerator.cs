using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Wugner.Localize
{
    public interface IEditorVocabularyAssetGenerator
    {
        void GenerateVocabularyAssets(IEnumerable<RawVocabularyEntryCollection> languageSepratedvocabularyEntryList);
    }

    public class EditorVocabularyAssetGenerator
    {
        public void GenerateVocabularyAssets(IEnumerable<RawVocabularyEntryCollection> languageSepratedvocabularyEntryList)
        {
            //Generate assets that seperated by languages.
            //These assets are not only loaded at runtime to display text, but also used in editor mode to provide id selection and preview function.
            //By writing your own vocabulary manager, it is possible to load only one language vocabulary when game starting or switching to another language,
            //instead of loading all languages vocabulary.
            //Also you can output them to the streaming assets folder in json files and load them at runtime so you can add extra languages with out rebuilding the game.
            foreach (var vocabularyMap in languageSepratedvocabularyEntryList)
            {
                var vocabularyAsset = EditorLocalUtility.LoadOrCreateAsset<VocabulariesAsset>(
                    string.Format(Constant.ASSETPATH_VOCABULARY, vocabularyMap.Language));
                UnityEditor.EditorUtility.SetDirty(vocabularyAsset);

                vocabularyAsset.VocabularyEntries.Clear();
                vocabularyAsset.VocabularyEntries.AddRange(vocabularyMap.Cast<VocabularyEntry>());
            }
            //var config = EditorUtility.LoadOrCreateAsset<LocalizationConfig>(Constant.ASSETPATH_CONFIG);
            //foreach (var map in vocabularyMapList)
            //{
            //    var index = config.LanguageSettings.FindIndex(s => s.Language == map.Language);
            //    if (index < 0)
            //    {
            //        config.LanguageSettings.Add(new LocalizationConfig.LanguageInfo()
            //        {
            //            Language = map.Language,
            //            DefaultFont = Resources.Load("Library/unity default resources/Arial") as Font
            //        });
            //    }
            //}
            //config.LanguageSettings.RemoveAll(s => vocabularyMapList.All(m => m.Language != s.Language));

            //UnityEditor.EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }
    }
}
