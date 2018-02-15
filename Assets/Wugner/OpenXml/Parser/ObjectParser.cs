using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wugner.OpenXml
{
	public struct ParseError
	{
		public string ErrorMsg { set; get; }
		public Exception Exception { set; get; }
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class XmlParseHeaderNameAttribute : Attribute
	{
		string _customHeaderName;
		public bool UseObjectClassNameForPrefix { set; get; }

		public XmlParseHeaderNameAttribute(string customHeaderName)
		{
			_customHeaderName = customHeaderName;
		}
		public string GetCustomHeaderName(object sourceObject)
		{
			var headerName = _customHeaderName;
			if (UseObjectClassNameForPrefix)
			{
				headerName = sourceObject.GetType().Name + headerName;
			}
			return headerName;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class XmlParseIgnoreAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class CreateInstanceByValueReflectionAttribute : Attribute
	{
		public string DefaultNamespace { private set; get; }
		public bool RecusiveParse { set; get; }

		public CreateInstanceByValueReflectionAttribute()
		{
		}
		public CreateInstanceByValueReflectionAttribute(string theNamespace)
		{
			DefaultNamespace = theNamespace;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class CreateInstanceAttribute : Attribute
	{
		public bool RecusiveParse { set; get; }
		public Type TargetType { set; get; }
	}
	
	public class ObjectParser
	{
		static ParseProvider[] _defaultProviders = new ParseProvider[]
		{
			new StringParseProvider(),
			new IntParseProvider(),
			new FloatParseProvider(),
			new BoolParseProvider(),
			new EnumParseProvider(),
            new DateTimeProvider()
        };

		ExcelRow _row;
		ParseSettings _settings;

		List<ParseError> _errors = new List<ParseError>();
		public List<ParseError> Errors { get { return _errors; } }

		public ObjectParser(ExcelRow row)
		{
			_row = row;
			_settings = ParseSettings.DefaultSettings;
		}
		public ObjectParser(ExcelRow row, ParseSettings settings)
		{
			_row = row;
			if(settings == null)
				_settings = ParseSettings.DefaultSettings;
			else
				_settings = settings;
		}

        public void ParseToObject(Type objectType, ref object obj)
		{
			_errors.Clear();

			var fieldsAndProperties = objectType.GetFieldsAndSetProperties();

			List<ParseProvider> parseProviders = new List<ParseProvider>();
			if (_settings.CustomProviders != null)
			{
				parseProviders.AddRange(_settings.CustomProviders);
			}
			parseProviders.AddRange(_defaultProviders);

			foreach (var memberInfo in fieldsAndProperties)
			{
				try
				{
					if (!SetValueByAttributes(memberInfo, ref obj))
					{
						SetValueFromRowData(memberInfo, ref obj, parseProviders);
					}
				}
				catch (Exception e)
				{
					//UnityEngine.Debug.LogException(e);
					AddError(string.Format("Error in sheet {0} row {1} field {2}. ",
						_row.Sheet.Name, _row.RowCount, memberInfo.Name), e);
				}
			}
		}

		void SetValueFromRowData(MemberInfo memberInfo, ref object obj, List<ParseProvider> parseProviders)
		{
			string headerName = memberInfo.Name;

			var customHeaderNameAttr = memberInfo.GetAttribute<XmlParseHeaderNameAttribute>();
			if (customHeaderNameAttr != null)
			{
				headerName = customHeaderNameAttr.GetCustomHeaderName(obj);
			}


			Type memberType = memberInfo.GetFieldOrPropertyType();

			object value = null;
			foreach (var provider in parseProviders)
			{
				if(provider.IsFieldNameMatch(headerName) || provider.IsTypeMatched(memberType))
				{
					if (provider.OnParseField(headerName, _row.StringValuesWithHeader, memberInfo, out value))
					{
						memberInfo.SetValue(ref obj, value);
						return;
					}
				}
			}
			throw new Exception("No parse provider match for type : " + memberType.Name);
		}
		
		bool SetValueByAttributes(MemberInfo memberInfo, ref object obj)
		{
			if (CheckXmlParseIgnoreAttribute(memberInfo))
				return true;

			if (CheckCreateInstanceAttribute(memberInfo, ref obj))
				return true;

			if (CheckCreateInstanceByValueReflectionAttribute(memberInfo, ref obj))
				return true;
			
			return false;
		}

		bool CheckXmlParseIgnoreAttribute(MemberInfo memberInfo)
		{
			return memberInfo.GetAttribute<XmlParseIgnoreAttribute>() != null;
		}

		bool CheckCreateInstanceAttribute(MemberInfo memberInfo, ref object obj)
		{
			var createAttr = memberInfo.GetAttribute<CreateInstanceAttribute>();
			if (createAttr != null)
			{
				var type = createAttr.TargetType;
				if (type == null) type = memberInfo.DeclaringType;
				var value = Activator.CreateInstance(type);

				memberInfo.SetValue(ref obj, value);

				if (createAttr.RecusiveParse)
				{
					ParseToObject(memberInfo.DeclaringType, ref value);
				}
				return true;
			}
			return false;
		}

		bool CheckCreateInstanceByValueReflectionAttribute(MemberInfo memberInfo, ref object obj)
		{
			var createAttr = memberInfo.GetAttribute<CreateInstanceByValueReflectionAttribute>();
			if (createAttr != null)
			{
				string headerName = memberInfo.Name;
				var customHeaderNameAttr = memberInfo.GetAttribute<XmlParseHeaderNameAttribute>();
				if (customHeaderNameAttr != null)
				{
					headerName = customHeaderNameAttr.GetCustomHeaderName(obj);
				}

				object className;
				if (new StringParseProvider().OnParseField(headerName, _row.StringValuesWithHeader, memberInfo, out className))
				{
					var fullName = createAttr.DefaultNamespace + "." + (string)className;
					var actualType = Type.GetType(fullName, true, true);
					var value = Activator.CreateInstance(actualType);

					memberInfo.SetValue(ref obj, value);

					if (createAttr.RecusiveParse)
					{
						ParseToObject(actualType, ref value);
					}
					return true;
				}
			}
			return false;
		}
		
		void AddError(string message, Exception e = null)
		{
			while(e != null)
			{
				message += e.Message;
				e = e.InnerException;
			}
			_errors.Add(new ParseError { ErrorMsg = message, Exception = e });
		}
	}
}
