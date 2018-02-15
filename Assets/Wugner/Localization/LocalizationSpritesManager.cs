using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wugner.Localize
{
	public interface ILocalizationSpritesManager
	{
		Sprite GetSprite(string path);
		void GetSpriteAsync(string path, Action<Sprite> callback);
		void UnloadSprite(Sprite sp);
	}

	public class LocalizationSpritesManager : MonoBehaviour, ILocalizationSpritesManager
	{
		Dictionary<Sprite, int> _referenceCounter = new Dictionary<Sprite, int>();

		public virtual Sprite GetSprite(string path)
		{
			return Resources.Load<Sprite>(path);
		}

		public virtual void GetSpriteAsync(string path, Action<Sprite> callback)
		{
			StartCoroutine(LoadSpriteCoroutine(path, callback));
		}

		IEnumerator LoadSpriteCoroutine(string path, Action<Sprite> callback)
		{
			var req = Resources.LoadAsync<Sprite>(path);
			yield return req;

			var sp = req.asset as Sprite;
			if (sp == null)
			{
				Debug.LogErrorFormat("Load sprite [{0}] failed!", path);
			}
			else
			{
				if (_referenceCounter.ContainsKey(sp))
					_referenceCounter[sp] = _referenceCounter[sp] + 1;
				else
					_referenceCounter.Add(sp, 1);
				
				callback(sp);
			}
		}

		public virtual void UnloadSprite(Sprite sp)
		{
			int count;
			if (_referenceCounter.TryGetValue(sp, out count))
			{
				count--;
				if (count <= 0)
				{
					_referenceCounter.Remove(sp);
					Resources.UnloadAsset(sp);
				}
			}
			else
			{
				Resources.UnloadAsset(sp);
			}
		}
	}
}
