﻿namespace Telia.GraphQL.Client
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
    using System.Text.RegularExpressions;

    public static class Utils
    {
		private static MethodInfo[] SelectMethods = typeof(Enumerable)
			.GetMethods()
			.Where(e => e.Name == "Select")
			.ToArray();

		// Copied from https://stackoverflow.com/a/46095771
		public static string ToPascalCase(string original)
        {
            var invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            var whiteSpace = new Regex(@"(?<=\s)");
            var startsWithLowerCaseChar = new Regex("^[a-z]");
            var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }

        public static object GetValue(this MemberInfo info, object obj)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field: return ((FieldInfo)info).GetValue(obj);
                case MemberTypes.Property: return ((PropertyInfo)info).GetValue(obj);
            }

            throw new NotImplementedException($"Utils:GetValue: Unsupported member type {info.MemberType}");
        }

        public static void SetValue(this MemberInfo info, object obj, object value)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field: ((FieldInfo)info).SetValue(obj, value); break;
                case MemberTypes.Property: ((PropertyInfo)info).SetValue(obj, value); break;

                default: throw new NotImplementedException($"Utils:SetValue: Unsupported member type {info.MemberType}");
            }
        }

        public static bool IsLinqSelectMethod(MethodCallExpression methodCallExpression)
		{
			return methodCallExpression.Method.IsGenericMethod &&
				SelectMethods.Contains(methodCallExpression.Method.GetGenericMethodDefinition());
		}

		public static bool IsEnumerable(this Type t)
		{
			if (t.IsArray)
			{
				return true;
			}

			if (typeof(IEnumerable).IsAssignableFrom(t))
			{
				return true;
			}

			if (t.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(t.GetGenericTypeDefinition()))
			{
				return true;
			}

			return false;
		}

		public static bool IsObject(this Type t)
		{
			if (t.IsClass)
			{
				return true;
			}

			return false;
		}
	}
}
