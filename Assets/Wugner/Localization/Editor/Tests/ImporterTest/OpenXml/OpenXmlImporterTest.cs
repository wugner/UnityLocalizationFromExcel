using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Wugner.Localize.Editor;
using Wugner.Localize.Importer;

namespace Wugner.Localize.Tests
{
    public class OpenXmlImporterTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void ReadFileToRawEntry()
        {
            var content = ReadFile("Test01.xml");
            var entries = new OpenXmlTableVocabularyImporter().Import(content);
            Assert.IsNotNull(entries);
            Assert.AreEqual(6, entries.Count);
            //Assert.True(entries.Any(e => e.ID == "InventoryView/Hint_InventoryFull"));

            foreach (var e in entries)
            {
                Debug.Log(e.ID);
            }
        }

        private string ReadFile(string fileName)
        {
            return System.IO.File.ReadAllText(GetFilePath(fileName));
        }
        private string GetFilePath(string fileName)
        {
            return Application.dataPath + "/Wugner/Localization/Editor/Tests/ImporterTest/OpenXml/" + fileName;
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
