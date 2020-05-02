using System;
using System.Reflection;

namespace GameStudio.Extensions
{
	public static class TypeExtensions
	{
		/// <summary>
		/// Reverses the order of IsAssignableFrom()
		/// So that you can use type.IsAssignableTo&lt;T&gt;
		/// Instead of typeof(T).IsAssignableFrom(type)
		/// </summary>
		public static bool IsAssignableTo<T>(this Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			return typeof(T).IsAssignableFrom(type);
		}

		public static bool IsAssignableTo(this Type type, Type other)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (other == null) throw new ArgumentNullException(nameof(other));

			return other.IsAssignableFrom(type);
		}

		public static bool IsGeneric(this Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			return type.GetTypeInfo().IsGenericTypeDefinition;
		}
	}
}
