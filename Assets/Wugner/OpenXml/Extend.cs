using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Wugner.OpenXml
{
	public static class Extend
	{
		public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute
		{
			var attrs = memberInfo.GetCustomAttributes(typeof(T), inherit);
			if (attrs.Length != 0)
			{
				return attrs[0] as T;
			}
			return null;
		}

		public static void SetValue(this MemberInfo memberInfo, ref object obj, object value)
		{
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				((FieldInfo)memberInfo).SetValue(obj, value);
			}
			else if (memberInfo.MemberType == MemberTypes.Property)
			{
				((PropertyInfo)memberInfo).SetValue(obj, value, null);
			} else
			{
				throw new Exception("Can not set value for " + memberInfo.Name + ". Member is neither FieldInfo nor PropertyInfo");
			}		
		}
		public static Type GetFieldOrPropertyType(this MemberInfo memberInfo)
		{
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)memberInfo).FieldType;
			}
			else if (memberInfo.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)memberInfo).PropertyType;
			}
			throw new Exception("Can not get field or property type " + memberInfo.Name + ". Member is neither FieldInfo nor PropertyInfo");
		}

		public static List<MemberInfo> GetFieldsAndSetProperties(this Type t)
		{
			return t.GetFieldsAndSetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}
		public static List<MemberInfo> GetFieldsAndSetProperties(this Type t, BindingFlags bindingAttr)
		{
			var fieldsAndProperties = new List<MemberInfo>();

			var proterties = t.GetProperties(bindingAttr).Where(p => p.GetSetMethod(true) != null).Cast<MemberInfo>();
			fieldsAndProperties.AddRange(proterties);
			
			var fields = t.GetFields(bindingAttr).Where(
				f => f.GetAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>() == null).Cast<MemberInfo>();

			fieldsAndProperties.AddRange(fields);

			return fieldsAndProperties;
		}
	}
}