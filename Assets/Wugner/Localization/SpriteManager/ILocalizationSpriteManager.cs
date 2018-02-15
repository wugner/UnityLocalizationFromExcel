using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public interface ILocalizationSpriteManager
	{
		void Init();
		Sprite GetSprite(string path);
		void GetSpriteAsync(string path, Action<Sprite> callback);
		void UnloadSprite(Sprite sp);
	}
}
