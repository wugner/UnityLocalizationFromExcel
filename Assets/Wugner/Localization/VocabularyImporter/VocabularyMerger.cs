using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Wugner.Localize.Importer
{
	public class VocabularyMerger
	{
		public Dictionary<string, VocabularyEntryCollection> MergedDataByLanguage { get; }
			= new Dictionary<string, VocabularyEntryCollection>();

		public void Add(IEnumerable<VocabularyEntry> entries, Action<string> errorMessageCallback)
		{
			foreach (var from in entries)
			{
				if (!MergedDataByLanguage.TryGetValue(from.Language, out var entryCollection))
				{
					entryCollection = new VocabularyEntryCollection(from.Language);
					MergedDataByLanguage.Add(from.Language, entryCollection);
				}

				if (!entryCollection.TryGetValue(from.ID, out var to))
				{
					entryCollection.Add(from);
				}
				else
				{
					MergeEntry(from, to, errorMessageCallback);
				}
			}
		}

		protected virtual void MergeEntry(VocabularyEntry from, VocabularyEntry to, Action<string> errorMessageCallback)
		{
			var fields = typeof(VocabularyEntry).GetFields();
			foreach (var f in fields)
			{
				if (f.Name == "ID" || f.Name == "Language" || f.Name == "srcInfo")
					continue;

				var fromValue = f.GetValue(from);
				var toValue = f.GetValue(to);

				if (f.Name == "ExtraInfo")
				{
					var fromDictionary = fromValue as Dictionary<string, string>;
					var toDictionary = toValue as Dictionary<string, string>;
					if (toDictionary != null) {
						if (fromDictionary == null)
						{
							fromDictionary = new Dictionary<string, string>();
							f.SetValue(from, fromDictionary);
						}

						foreach (var toDictKv in toDictionary)
						{
							if (fromDictionary.TryGetValue(toDictKv.Key, out string fromDictValue))
							{
								var (success, data) = MergeData(fromDictValue, toDictKv.Value);
								if (success)
									fromDictionary[toDictKv.Key] = data as string;
								else
								{
									errorMessageCallback?.Invoke(
										string.Format("Found diffrent value set for same ID {0}. at field {1}.{2}  src info: {3}. {4}",
										to.ID, f.Name, toDictKv.Key, from.srcInfo, to.srcInfo));
								}
							}
							else
							{
								fromDictionary[toDictKv.Key] = toDictKv.Value;
							}
						}
					}
				}
				else
				{
					var (success, data) = MergeData(fromValue, toValue);
					if (success)
					{
						f.SetValue(to, data);
					}
					else
					{
						errorMessageCallback?.Invoke(
							string.Format("Found diffrent value set for same ID {0} at field {1}  src info: {2}. {3}",
							to.ID, f.Name, from.srcInfo, to.srcInfo));
					}
				}
			}
		}

		private (bool success, object data) MergeData(object from, object to)
		{
			from = EmptyStringToNull(ZeroEnumToNull(from));
			to = EmptyStringToNull(ZeroEnumToNull(to));

			if (to == null)
				return (true, from);

			if (from == null)
			{
				return (true, to);
			}
			if (from == to)
			{
				return (true, to);
			}
			return (false, null);
		}

		private object EmptyStringToNull(object t)
		{
			if (t is string && (string)t == "")
			{
				return null;
			}
			return t;
		}
		private object ZeroEnumToNull(object t)
		{
			if (t != null && t is Enum && (int)t == 0)
			{
				return null;
			}
			return t;
		}
	}
}
