using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public interface ILocalizationFontManager
	{
		void Init();
		Font GetFont(string fontName);
		Font GetLanguageDefaultFont(string language);
	}
}
