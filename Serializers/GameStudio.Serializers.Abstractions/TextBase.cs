using System;
using System.IO;

namespace GameStudio.Serializers
{
	/// <summary>
	/// Just TextDeserializer and TextSerializer copy/pasta combined
	/// </summary>
	public abstract class TextBase
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
        
		public abstract string Serialize(object obj, Type type);
		public abstract string Serialize<T>(T obj);
		public abstract void Serialize<T>(TextWriter writer, T obj);
		public abstract void Serialize(TextWriter writer, object obj);
		public abstract void Serialize(TextWriter writer, object obj, Type type);

		public bool TrySerialize(object obj, out string result)
		{
			result = default(string);
			if (obj == null)
				return false;

			return TrySerialize(obj, obj.GetType(), out result);
		}

		public bool TrySerialize(object obj, Type type, out string result)
		{
			result = default(string);

			if (obj == null)
				return false;

			try
			{
				result = Serialize(obj, type);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool TrySerialize<T>(T obj, out string result)
		{
			result = default(string);

			if (obj.Equals(default(T)))
				return false;

			try
			{
				result = Serialize<T>(obj);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
