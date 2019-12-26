﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Wugner.Localize
{
	public interface ILocalizationVocabularyImporter
	{
		Task<List<VocabularyEntry>> Import(string fileContent);
	}
}