using System;
using System.IO;

namespace GameStudio.Serializers
{
	public abstract class TextDeserializer
	{
		public abstract object Deserialize(string str, Type type);
		public abstract T Deserialize<T>(string str);

		public abstract T Deserialize<T>(TextReader reader);
		public abstract object Deserialize(TextReader reader, Type type);

		public bool TryDeserialize<T>(string str, out T result)
		{
			result = default(T);

			if (string.IsNullOrWhiteSpace(str))
				return false;

			try
			{
				result = Deserialize<T>(str);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool TryDeserialize(string str, Type type, out object result)
		{
			result = null;

			if (string.IsNullOrWhiteSpace(str))
				return false;

			try
			{
				result = Deserialize(str, type);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool TryDeserialize<T>(TextReader reader, out T result)
		{
			result = default(T);

			try
			{
				result = Deserialize<T>(reader);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool TryDeserialize(TextReader reader, Type type, out object result)
		{
			result = null;

			try
			{
				result = Deserialize(reader, type);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
