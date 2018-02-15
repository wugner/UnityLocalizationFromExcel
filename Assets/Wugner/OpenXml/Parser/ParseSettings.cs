using UnityEngine;
using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Wugner.OpenXml
{
	public class ParseSettings
	{

		static ParseSettings _defaultSettings = new ParseSettings
		{
		};
		public static ParseSettings DefaultSettings { get { return _defaultSettings; } }

		public ParseProvider[] CustomProviders { set; get; }

	}
}