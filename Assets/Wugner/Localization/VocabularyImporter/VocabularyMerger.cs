using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize.Importer
{
	public struct MergeError
	{
		public string ID;
		public string Language;
		public string FieldName;

		public string SrcInfoA;
		public string ValueA;
		public string SrcInfoB;
		public string ValueB;

		public override string ToString()
		{
			return $"Same id[{ID}] language[{Language}] field[{FieldName}] but different value. \n"
				+ $"ValueA: {ValueA} \n"
				+ $"SrcInfoA: {SrcInfoA} \n"
				+ $"ValueB: {ValueB} \n"
				+ $"SrcInfoB: {SrcInfoB} \n";
		}
	}

	public class VocabularyMerger
	{
		public List<MergeError> Errors = new List<MergeError>();
		public Dictionary<string, VocabularyEntryCollection> MergedDataByLanguage { get; }
			= new Dictionary<string, VocabularyEntryCollection>();

		public void Add(IEnumerable<VocabularyEntry> entries)
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
					MergeEntry(from, to);
				}
			}
		}

		protected virtual void MergeEntry(VocabularyEntry from, VocabularyEntry to)
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
					if (fromDictionary == null)
						continue;

					var toDictionary = toValue as Dictionary<string, string>;
					if (toDictionary == null)
					{
						toDictionary = new Dictionary<string, string>();
						f.SetValue(to, toDictionary);
					}

					foreach (var fromDictKv in fromDictionary)
					{
						var key = fromDictKv.Key;
						if (toDictionary.TryGetValue(key, out string toDictValue))
						{
							var (success, data) = MergeData(fromDictKv.Value, toDictValue);
							if (success)
							{
								toDictionary[key] = data as string;
							}
							else
							{
								Errors.Add(new MergeError()
								{
									ID = from.ID,
									Language = from.Language,
									FieldName = f.Name + "." + fromDictKv.Key,
									SrcInfoA = from.SrcInfo,
									ValueA = toDictValue,
									SrcInfoB = to.SrcInfo,
									ValueB = fromDictKv.Value
								});
							}
						}
						else
						{
							toDictionary[key] = fromDictKv.Value;
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
						Errors.Add(new MergeError()
						{
							ID = from.ID,
							Language = from.Language,
							FieldName = f.Name,
							SrcInfoA = from.SrcInfo,
							ValueA = fromValue.ToString(),
							SrcInfoB = to.SrcInfo,
							ValueB = toValue.ToString()
						});
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
