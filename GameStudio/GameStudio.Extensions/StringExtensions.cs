using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameStudio.Extensions
{
	public static class StringExtensions
	{
		// "a=1&c=2".ParseKeyValues("=","&") = ["a",1],["c",2]
		public static NameValueCollection ParseKeyValues(this string instring, char sep, char keyValueSep)
		{
			var nvc = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
			var keyvals = instring.Split(sep);
			foreach (var keyval in keyvals)
			{
				var split = keyval.Split(keyValueSep);
				if (split.Length >= 1)
					nvc.Add(split[0], split[1]);
			}
			return nvc;
		}

		public static string[] Split(this string instring, char c, StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
		{
			return instring.Split(new[] { c }, splitOptions);
		}
		
		public static bool EqualsIgnoreCase(this string str, string match)
		{
			return str.Equals(match, StringComparison.OrdinalIgnoreCase);
		}

		public static bool ContainsIgnoreCase(this IEnumerable<string> strs, string match)
		{
			return strs.Contains(match, StringComparer.OrdinalIgnoreCase);
		}

		public static IEnumerable<string> RemoveEmpty(this IEnumerable<string> items)
		{
			return items.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
		}

		public static bool IsGuid(this string str)
		{
			Guid guid;
			return !string.IsNullOrWhiteSpace(str) && Guid.TryParse(str, out guid);
		}
		
		//stolen from decompiling System.ComponentModel.DataAnnotations.EmailAddressAttribute
		static readonly Regex EmailRegex = new Regex("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		public static bool IsEmail(this string str)
		{
			return !string.IsNullOrWhiteSpace(str) && EmailRegex.Match(str).Length > 0;
		}

        //not meant to be a json parser, meant to be a basic sanity check
		public static bool IsJson(this string val)
		{
			if (String.IsNullOrWhiteSpace(val))
				return false;

			var json = val.Trim();

			//traditional Json Object { obj : 1 }
			if (json.StartsWith("{") && json.EndsWith("}"))
				return true;

			//json array [{ obj: 1}],[]...
			return json.StartsWith("[") && json.EndsWith("]") && json.Contains("{") && json.Contains("}");
		}

		#region Base64

		const char Base64Padding = '=';

		static readonly HashSet<char> Base64Characters = new HashSet<char>()
		{
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
			'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
			'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
			'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
		};

		public static bool IsBase64(this string param)
		{
			if (param == null)
			{
				// null string is not Base64 something
				return false;
			}

			// replace optional CR and LF characters
			param = param.Replace("\r", String.Empty).Replace("\n", String.Empty);

			if (param.Length == 0 ||
				(param.Length % 4) != 0)
			{
				// Base64 string should not be empty
				// Base64 string length should be multiple of 4
				return false;
			}

			// replace pad chacters
			var lengthNoPadding = param.Length;

			param = param.TrimEnd(Base64Padding);
			var lengthPadding = param.Length;

			if ((lengthNoPadding - lengthPadding) > 2)
			{
				// there should be no more than 2 pad characters
				return false;
			}

			foreach (var c in param)
			{
				if (Base64Characters.Contains(c) == false)
				{
					// string contains non-Base64 character
					return false;
				}
			}

			// nothing invalid found
			return true;
		}

		public static string ToBase64(this string input)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
		}

		public static string FromBase64(this string input)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(input));
		}

		#endregion

		const int MaxAnsiCode = 255;

		public static bool HasUnicodeCharacters(this string input)
		{
			return input.Any(c => c > MaxAnsiCode);
		}

		public static bool IsUnicode(this char c)
		{
			return c > MaxAnsiCode;
		}

		public static bool IsAllUnicodeCharacters(this string input)
		{
			return input.All(c => c > MaxAnsiCode);
		}
	}
}
