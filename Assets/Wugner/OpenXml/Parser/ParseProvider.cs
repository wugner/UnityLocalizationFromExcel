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
	public abstract class ParseProvider
	{
		private bool includeInherited = true;
		public bool IncludeInherited { get { return includeInherited; } set { includeInherited = value; } }

		public Type[] TargetTypes { set; get; }
		public string[] TargetFieldName { set; get; }

		public abstract bool OnParseField(string headerName, Dictionary<string, string> rowValues, MemberInfo memberInfo, out object value);

		public bool IsFieldNameMatch(string fieldName)
		{
			if (TargetFieldName == null)
				return false;

			return TargetFieldName.Contains(fieldName);
		}
		public bool IsTypeMatched(Type type)
		{
			if (TargetTypes == null)
				return false;
			if (IncludeInherited)
			{
				return TargetTypes.Any(target => type == target || type.IsSubclassOf(target));
			} else
			{
				return TargetTypes.Any(target => target == type);
			}
		}
	}

	public class StringParseProvider : ParseProvider
	{
		public StringParseProvider()
		{
			TargetTypes = new Type[] { typeof(string) };
		}

		public override bool OnParseField(string headerName, Dictionary<string, string> rowValues, MemberInfo memberInfo, out object value)
		{
			string str; 
			if (rowValues.TryGetValue(headerName, out str))
			{
				value = str;
				return true;
			}
			throw new Exception("No header for field " + headerName);
		}
	}

	public class IntParseProvider : ParseProvider
	{
		public IntParseProvider()
		{
			TargetTypes = new Type[] { typeof(int) };
		}

		public override bool OnParseField(string headerName, Dictionary<string, string> rowValues, MemberInfo memberInfo, out object value)
		{
			string str;
			if (rowValues.TryGetValue(headerName, out str))
			{
				try
				{
					value = int.Parse(str);
					return true;
				}
				catch (FormatException e)
				{
					throw new Exception(string.Format("Can not parse {0} to int for filed {1} ", str, headerName), e);
				}
				catch (OverflowException e)
				{
					throw new Exception(string.Format("Can not parse {0} to int for filed {1} ", str, headerName), e);
				}
			}

			throw new Exception("No header for field " + headerName);
		}
	}
	public class FloatParseProvider : ParseProvider
	{
		public FloatParseProvider()
		{
			TargetTypes = new Type[] { typeof(float) };
		}
		public override bool OnParseField(string headerName, Dictionary<string, string> rowValues, MemberInfo memberInfo, out object value)
		{
			string str;
			if (rowValues.TryGetValue(headerName, out str))
			{
				try
				{
					value = float.Parse(str);
					return true;
				}
				catch (FormatException e)
				{
					throw new Exception(string.Format("Can not parse {0} to float for filed {1} ", str, headerName), e);
				}
				catch (OverflowException e)
				{
					throw new Exception(string.Format("Can not parse {0} to float for filed {1} ", str, headerName), e);
				}
			}

			throw new Exception("No header for field " + headerName);
		}
	}
	public class BoolParseProvider : ParseProvider
	{
		public BoolParseProvider()
		{
			TargetTypes = new Type[] { typeof(bool) };
		}
		public override bool OnParseField(string headerName, Dictionary<string, string> rowValues, MemberInfo memberInfo, out object value)
		{
			string str;
			if (rowValues.TryGetValue(headerName, out str))
			{
				if (str == "0")
				{
					value = false;
					return true;
				}
				else if (str == "1")
				{
					value = true;
					return true;
				}
				else
				{
					str = str.ToLower();
					if (str == "false")
					{
						value = false;
						return true;
					}
					else if(str == "true")
					{
						value = true;
						return true;
					}
				}
				throw new Exception(string.Format("Can not parse {0} to bool for filed {1} ", str, headerName));
			}

			throw new Exception("No header for field " + headerName);
		}
	}

	public class EnumParseProvider : ParseProvider
	{
		public EnumParseProvider()
		{
			TargetTypes = new Type[] { typeof(Enum) };
		}

		public override bool OnParseField(string headerName, Dictionary<string, string> rowValues, MemberInfo memberInfo, out object value)
		{
			var fieldType = memberInfo.GetFieldOrPropertyType();

			string str;
			if (rowValues.TryGetValue(headerName, out str))
			{
				int intValue;
				if (int.TryParse(str, out intValue))
				{
					if (Enum.IsDefined(fieldType, intValue))
					{
						value = Enum.ToObject(fieldType, intValue);
						return true;
					}
					throw new Exception(string.Format("Can not parse {0} to {1} for filed {2} ", str, fieldType.Name, headerName));
				}

				try
				{
					value = Enum.Parse(fieldType, str, true);
					return true;
				}
				catch (Exception)
				{
					throw new Exception(string.Format("Can not parse {0} to {1} for filed {2} ", str, fieldType.Name, headerName));
				}
			}
			throw new Exception("No header for field " + headerName);
		}
	}

    public class DateTimeProvider : ParseProvider
    {
        public DateTimeProvider()
        {
            TargetTypes = new Type[] { typeof(DateTime) };
        }

        public override bool OnParseField(string headerName, Dictionary<string, string> rowValues, MemberInfo memberInfo, out object value)
        {
            string str;
            if (rowValues.TryGetValue(headerName, out str))
            {
                try
                {
                    value = DateTime.Parse(str);
                    return true;
                } catch (Exception ex)
                {
                    throw new Exception(string.Format("Can not parse {0} to DateTime for filed {2} ", str, headerName), ex);
                }
            }
            throw new Exception("No header for field " + headerName);
        }
    }
}