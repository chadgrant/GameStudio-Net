using System;
using System.IO;

namespace GameStudio.Serializers
{
	public interface ITextSerializer
	{
		string Serialize<T>(T obj);
		bool TrySerialize<T>(T obj, out string result);
		void Serialize<T>(TextWriter writer, T obj);

		string Serialize(object obj);
		bool TrySerialize(object obj, out string result);
		void Serialize(TextWriter writer, object obj);

		string Serialize(object obj, Type type);
		bool TrySerialize(object obj, Type type, out string result);
		void Serialize(TextWriter writer, object obj, Type type);
	}
}
