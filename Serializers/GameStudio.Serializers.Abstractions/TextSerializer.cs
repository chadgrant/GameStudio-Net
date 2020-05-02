using System;
using System.IO;

namespace GameStudio.Serializers
{
	public abstract class TextSerializer
	{
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
