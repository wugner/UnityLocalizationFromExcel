using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        [System.Serializable]
        class ABCDE
        {
            [System.Serializable]
            public struct ExInfo
            {
                public string Key;
                public int Value;
            }

            public string Name;
            public List<ExInfo> ExtraInfo;
        }
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            var content = System.IO.File.ReadAllText("D:\\Practice\\testxml.txt");

            var ret = new List<Wugner.Localize.RawVocabularyEntry>();
            var xdoc = System.Xml.Linq.XDocument.Parse(content);
            foreach (var entryElement in xdoc.Root.Elements("Entry"))
            {
                Debug.Log(">>>>>>>>>>>>>>>>>>>>>");
                var entry = new Wugner.Localize.RawVocabularyEntry();
                foreach (var entryData in entryElement.Elements())
                {
                    Debug.Log("----------");
                    Debug.Log(entryData.Name.LocalName);
                    Debug.Log(entryData.Value);
                }
                ret.Add(entry);
            }

            //Assert.IsNotNull(entires);
            //Assert.AreEqual(0, entires.Length);

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
