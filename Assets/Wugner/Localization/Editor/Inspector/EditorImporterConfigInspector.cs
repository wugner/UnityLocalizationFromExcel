using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace Wugner.Localize
{
	[CustomEditor(typeof(EditorVocabularyImportConfig))]
	public class EditorImporterConfigInspector : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();

			var root = new VisualElement();
			root.Add(new TextField()
			{
				label = "Id ConstantName Space",
				bindingPath = "_idConstantNameSpace",
			});
			root.Add(new TextField()
			{
				label = "Id ConstantName Name",
				bindingPath = "_idConstantClassName",
			});

			List<(string typeNameWithAssembly, string[] paths)> _typeAssemblyToPaths = new List<(string typeNameWithAssembly, string[] paths)>();
			var typeProp = serializedObject.FindProperty("_typeNameWithAssembly");
			var pathProp = serializedObject.FindProperty("_filePaths");
			for (var i = 0; i < typeProp.arraySize; i++)
			{
				var typeNameWithAssembly = typeProp.GetArrayElementAtIndex(i).stringValue;
				var paths = pathProp.GetArrayElementAtIndex(i).stringValue;
				_typeAssemblyToPaths.Add((typeNameWithAssembly, paths.Split(';')));
			}

			var types = from assemblie in System.AppDomain.CurrentDomain.GetAssemblies()
						from type in assemblie.GetTypes()
						where type.IsClass && (type.GetInterface(typeof(ITextVocabularyImporter).FullName) != null
							|| type.GetInterface(typeof(IBinaryVocabularyImporter).FullName) != null)
						select type;

			var arrElement = new UnityEngine.UIElements.ListView(_typeAssemblyToPaths, (int)EditorGUIUtility.singleLineHeight * 2, () =>
			{
				var eleroot = new VisualElement();
				var typeSelection = new DropdownMenu();
				var typeSelectionText = new TextField("Type");

				return eleroot;

			}, (element, index) =>
			{

			});

			return root;
		}
	}
}
